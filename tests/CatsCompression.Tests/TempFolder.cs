namespace CatsCompression.Tests;

// A fresh folder under %TEMP% for each test, deleted afterwards.
public sealed class TempFolder : IDisposable
{
    public string Root { get; } = Path.Combine(Path.GetTempPath(), "FileCompTests_" + Guid.NewGuid().ToString("N"));

    public TempFolder()
    {
        Directory.CreateDirectory(Root);
    }

    public string PathTo(params string[] parts) => Path.Combine([Root, .. parts]);

    public string WriteFile(string relativePath, byte[] contents)
    {
        string path = PathTo(relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllBytes(path, contents);
        return path;
    }

    public string WriteFile(string relativePath, string text) => WriteFile(relativePath, System.Text.Encoding.UTF8.GetBytes(text));

    // Any leftover "*.tmp" files mean a failed write wasn't cleaned up.
    public string[] LeftoverTempFiles() => Directory.GetFiles(Root, "*.tmp", SearchOption.AllDirectories);

    public void Dispose()
    {
        try { Directory.Delete(Root, recursive: true); } catch (IOException) { } catch (UnauthorizedAccessException) { }
    }
}
