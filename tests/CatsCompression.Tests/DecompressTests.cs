using System.IO.Compression;

namespace CatsCompression.Tests;

public class DecompressTests : IDisposable
{
    private readonly TempFolder temp = new();

    public void Dispose() => temp.Dispose();

    // Builds a zip with the given entry names and text contents. A name ending in "/" is a folder entry.
    private string MakeZip(string name, params (string Entry, string Text)[] entries)
    {
        string zip = temp.PathTo(name);
        using ZipArchive archive = ZipFile.Open(zip, ZipArchiveMode.Create);
        foreach (var (entry, text) in entries)
        {
            ZipArchiveEntry created = archive.CreateEntry(entry);
            if (!entry.EndsWith('/'))
            {
                using StreamWriter writer = new(created.Open());
                writer.Write(text);
            }
        }
        return zip;
    }

    [Fact]
    public void Extracts_into_a_folder_that_does_not_exist_yet()
    {
        string zip = MakeZip("a.zip", ("hello.txt", "hi"));

        FileCompUtility.DecompressFile(zip, temp.PathTo("new", "folder"));

        Assert.Equal("hi", File.ReadAllText(temp.PathTo("new", "folder", "hello.txt")));
    }

    [Fact]
    public void Existing_file_is_not_overwritten_by_default()
    {
        string zip = MakeZip("a.zip", ("hello.txt", "from zip"));
        temp.WriteFile(Path.Combine("out", "hello.txt"), "original");

        Assert.ThrowsAny<IOException>(() => FileCompUtility.DecompressFile(zip, temp.PathTo("out")));

        Assert.Equal("original", File.ReadAllText(temp.PathTo("out", "hello.txt")));
    }

    [Fact]
    public void Existing_file_is_overwritten_when_allowed()
    {
        string zip = MakeZip("a.zip", ("hello.txt", "from zip"));
        temp.WriteFile(Path.Combine("out", "hello.txt"), "original");

        FileCompUtility.DecompressFile(zip, temp.PathTo("out"), overwriteFiles: true);

        Assert.Equal("from zip", File.ReadAllText(temp.PathTo("out", "hello.txt")));
    }

    [Fact]
    public void Non_zip_file_is_rejected()
    {
        string fake = temp.WriteFile("fake.zip", "this is just text");

        Assert.Throws<InvalidDataException>(() => FileCompUtility.DecompressFile(fake, temp.PathTo("out")));
    }

    [Fact]
    public void Truncated_zip_is_rejected()
    {
        string source = temp.WriteFile("big.txt", string.Concat(Enumerable.Repeat("some text ", 5000)));
        string zip = temp.PathTo("big.zip");
        FileCompUtility.CompressFile(source, zip);
        byte[] bytes = File.ReadAllBytes(zip);
        string damaged = temp.WriteFile("damaged.zip", bytes[..(bytes.Length / 2)]);

        Assert.Throws<InvalidDataException>(() => FileCompUtility.DecompressFile(damaged, temp.PathTo("out")));
    }

    [Fact]
    public void Entry_escaping_the_destination_is_blocked()
    {
        string zip = MakeZip("evil.zip", ("../escaped.txt", "gotcha"));

        Assert.ThrowsAny<IOException>(() => FileCompUtility.DecompressFile(zip, temp.PathTo("out")));

        Assert.False(File.Exists(temp.PathTo("escaped.txt")));
    }

    [Fact]
    public void Deeply_escaping_entry_is_blocked()
    {
        string zip = MakeZip("evil.zip", ("safe/../../../escaped.txt", "gotcha"));

        Assert.ThrowsAny<IOException>(() => FileCompUtility.DecompressFile(zip, temp.PathTo("out", "inner")));

        Assert.False(File.Exists(temp.PathTo("escaped.txt")));
        Assert.False(File.Exists(temp.PathTo("out", "escaped.txt")));
    }

    [Fact]
    public void FindExistingFiles_lists_only_files_that_would_be_replaced()
    {
        string zip = MakeZip("a.zip",
            ("same.txt", "zip"),
            ("new.txt", "zip"),
            ("sub/", ""),
            ("sub/nested.txt", "zip"),
            ("sub/other.txt", "zip"));
        temp.WriteFile(Path.Combine("out", "same.txt"), "existing");
        temp.WriteFile(Path.Combine("out", "sub", "nested.txt"), "existing");

        List<string> existing = FileCompUtility.FindExistingFiles(zip, temp.PathTo("out"));

        Assert.Equal(["same.txt", "sub/nested.txt"], existing.Order());
    }

    [Fact]
    public void FindExistingFiles_ignores_folders_that_already_exist()
    {
        string zip = MakeZip("a.zip", ("sub/", ""), ("sub/file.txt", "zip"));
        Directory.CreateDirectory(temp.PathTo("out", "sub"));

        Assert.Empty(FileCompUtility.FindExistingFiles(zip, temp.PathTo("out")));
    }

    [Fact]
    public void FindExistingFiles_is_empty_for_a_new_destination()
    {
        string zip = MakeZip("a.zip", ("a.txt", "x"), ("b.txt", "y"));

        Assert.Empty(FileCompUtility.FindExistingFiles(zip, temp.PathTo("does-not-exist")));
    }

    [Fact]
    public void FindExistingFiles_rejects_a_non_zip_file()
    {
        string fake = temp.WriteFile("fake.zip", "not a zip");

        Assert.Throws<InvalidDataException>(() => FileCompUtility.FindExistingFiles(fake, temp.PathTo("out")));
    }

}
