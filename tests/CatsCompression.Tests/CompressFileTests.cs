using System.IO.Compression;

namespace CatsCompression.Tests;

public class CompressFileTests : IDisposable
{
    private readonly TempFolder temp = new();

    public void Dispose() => temp.Dispose();

    [Fact]
    public void Text_file_round_trips_exactly()
    {
        string source = temp.WriteFile("notes.txt", string.Concat(Enumerable.Repeat("hello world\r\n", 1000)));
        string zip = temp.PathTo("notes.zip");

        FileCompUtility.CompressFile(source, zip);
        FileCompUtility.DecompressFile(zip, temp.PathTo("out"));

        Assert.Equal(File.ReadAllBytes(source), File.ReadAllBytes(temp.PathTo("out", "notes.txt")));
    }

    [Fact]
    public void Binary_file_round_trips_byte_for_byte()
    {
        // Every byte value, plus random data, so nothing is treated as text.
        byte[] data = new byte[300_000];
        new Random(42).NextBytes(data);
        for (int i = 0; i < 256; i++) data[i] = (byte)i;
        string source = temp.WriteFile("image.bin", data);
        string zip = temp.PathTo("image.zip");

        FileCompUtility.CompressFile(source, zip);
        FileCompUtility.DecompressFile(zip, temp.PathTo("out"));

        Assert.Equal(data, File.ReadAllBytes(temp.PathTo("out", "image.bin")));
    }

    [Fact]
    public void Archive_holds_one_entry_named_after_the_file()
    {
        string source = temp.WriteFile(Path.Combine("deep", "folder", "report.csv"), "a,b,c");
        string zip = temp.PathTo("report.zip");

        FileCompUtility.CompressFile(source, zip);

        using ZipArchive archive = ZipFile.OpenRead(zip);
        ZipArchiveEntry entry = Assert.Single(archive.Entries);
        Assert.Equal("report.csv", entry.FullName);
    }

    [Fact]
    public void Compressible_data_gets_smaller()
    {
        string source = temp.WriteFile("log.txt", string.Concat(Enumerable.Repeat("INFO request handled in 12ms\n", 10_000)));
        string zip = temp.PathTo("log.zip");

        FileCompUtility.CompressFile(source, zip);

        Assert.True(new FileInfo(zip).Length < new FileInfo(source).Length / 10);
    }

    [Fact]
    public void Saving_the_zip_over_the_source_file_is_refused()
    {
        string source = temp.WriteFile("data.txt", "keep me");

        Assert.Throws<ArgumentException>(() => FileCompUtility.CompressFile(source, source));

        Assert.Equal("keep me", File.ReadAllText(source));
        Assert.Empty(temp.LeftoverTempFiles());
    }

    [Fact]
    public void Existing_zip_is_replaced()
    {
        string zip = temp.WriteFile("out.zip", "old contents, not a zip");
        string source = temp.WriteFile("new.txt", "new");

        FileCompUtility.CompressFile(source, zip);

        using ZipArchive archive = ZipFile.OpenRead(zip);
        Assert.Equal("new.txt", Assert.Single(archive.Entries).FullName);
    }

    [Fact]
    public void Missing_source_fails_without_leaving_files_behind()
    {
        string zip = temp.PathTo("out.zip");

        Assert.Throws<FileNotFoundException>(() => FileCompUtility.CompressFile(temp.PathTo("missing.txt"), zip));

        Assert.False(File.Exists(zip));
        Assert.Empty(temp.LeftoverTempFiles());
    }

    [Fact]
    public void Failed_compression_keeps_the_previous_zip_intact()
    {
        string zip = temp.WriteFile("out.zip", "previous archive");

        Assert.Throws<FileNotFoundException>(() => FileCompUtility.CompressFile(temp.PathTo("missing.txt"), zip));

        Assert.Equal("previous archive", File.ReadAllText(zip));
        Assert.Empty(temp.LeftoverTempFiles());
    }

    [Fact]
    public void Locked_source_file_fails_cleanly()
    {
        string source = temp.WriteFile("locked.txt", "busy");
        string zip = temp.PathTo("out.zip");

        using (new FileStream(source, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
        {
            Assert.ThrowsAny<IOException>(() => FileCompUtility.CompressFile(source, zip));
        }

        Assert.False(File.Exists(zip));
        Assert.Empty(temp.LeftoverTempFiles());
    }
}
