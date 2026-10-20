using System.IO.Compression;

namespace CatsCompression.Tests;

public class CompressDirectoryTests : IDisposable
{
    private readonly TempFolder temp = new();
    private readonly FileCompUtility utility = new();

    public void Dispose() => temp.Dispose();

    [Fact]
    public void Folder_with_subfolders_round_trips()
    {
        byte[] binary = new byte[50_000];
        new Random(7).NextBytes(binary);
        temp.WriteFile(Path.Combine("src", "readme.txt"), "top level");
        temp.WriteFile(Path.Combine("src", "a", "b", "deep.txt"), "nested");
        temp.WriteFile(Path.Combine("src", "a", "data.bin"), binary);
        string zip = temp.PathTo("src.zip");

        utility.CompressDirectory(temp.PathTo("src"), zip);
        utility.DecompressFile(zip, temp.PathTo("out"));

        Assert.Equal("top level", File.ReadAllText(temp.PathTo("out", "readme.txt")));
        Assert.Equal("nested", File.ReadAllText(temp.PathTo("out", "a", "b", "deep.txt")));
        Assert.Equal(binary, File.ReadAllBytes(temp.PathTo("out", "a", "data.bin")));
    }

    [Fact]
    public void Folder_contents_go_in_the_zip_not_the_folder_itself()
    {
        temp.WriteFile(Path.Combine("src", "file.txt"), "x");
        string zip = temp.PathTo("src.zip");

        utility.CompressDirectory(temp.PathTo("src"), zip);

        using ZipArchive archive = ZipFile.OpenRead(zip);
        Assert.Equal("file.txt", Assert.Single(archive.Entries).FullName);
    }

    [Fact]
    public void Empty_subfolders_are_kept()
    {
        temp.WriteFile(Path.Combine("src", "file.txt"), "x");
        Directory.CreateDirectory(temp.PathTo("src", "empty"));
        string zip = temp.PathTo("src.zip");

        utility.CompressDirectory(temp.PathTo("src"), zip);
        utility.DecompressFile(zip, temp.PathTo("out"));

        Assert.True(Directory.Exists(temp.PathTo("out", "empty")));
    }

    [Fact]
    public void Empty_folder_makes_a_valid_empty_zip()
    {
        Directory.CreateDirectory(temp.PathTo("src"));
        string zip = temp.PathTo("src.zip");

        utility.CompressDirectory(temp.PathTo("src"), zip);

        using ZipArchive archive = ZipFile.OpenRead(zip);
        Assert.Empty(archive.Entries);
    }

    [Fact]
    public void Saving_the_zip_inside_the_folder_is_refused()
    {
        temp.WriteFile(Path.Combine("src", "file.txt"), "x");
        string zip = temp.PathTo("src", "inside.zip");

        Assert.Throws<ArgumentException>(() => utility.CompressDirectory(temp.PathTo("src"), zip));

        Assert.False(File.Exists(zip));
        Assert.Empty(temp.LeftoverTempFiles());
    }

    [Fact]
    public void Saving_the_zip_in_a_nested_subfolder_is_refused()
    {
        temp.WriteFile(Path.Combine("src", "sub", "file.txt"), "x");

        Assert.Throws<ArgumentException>(() => utility.CompressDirectory(temp.PathTo("src"), temp.PathTo("src", "sub", "inside.zip")));
    }

    [Fact]
    public void Inside_check_ignores_letter_case()
    {
        temp.WriteFile(Path.Combine("src", "file.txt"), "x");

        Assert.Throws<ArgumentException>(() => utility.CompressDirectory(temp.PathTo("src"), temp.PathTo("SRC", "inside.zip")));
    }

    [Fact]
    public void Sibling_folder_with_a_similar_name_is_allowed()
    {
        // "src2" starts with "src" but is not inside it.
        temp.WriteFile(Path.Combine("src", "file.txt"), "x");
        Directory.CreateDirectory(temp.PathTo("src2"));
        string zip = temp.PathTo("src2", "out.zip");

        utility.CompressDirectory(temp.PathTo("src"), zip);

        Assert.True(File.Exists(zip));
    }

    [Fact]
    public void Missing_folder_fails_without_leaving_files_behind()
    {
        string zip = temp.PathTo("out.zip");

        Assert.Throws<DirectoryNotFoundException>(() => utility.CompressDirectory(temp.PathTo("missing"), zip));

        Assert.False(File.Exists(zip));
        Assert.Empty(temp.LeftoverTempFiles());
    }

    [Fact]
    public void Many_small_files_are_all_included()
    {
        for (int i = 0; i < 500; i++)
            temp.WriteFile(Path.Combine("src", $"dir{i % 10}", $"file{i}.txt"), $"content {i}");
        string zip = temp.PathTo("src.zip");

        utility.CompressDirectory(temp.PathTo("src"), zip);

        using ZipArchive archive = ZipFile.OpenRead(zip);
        Assert.Equal(500, archive.Entries.Count);
    }
}
