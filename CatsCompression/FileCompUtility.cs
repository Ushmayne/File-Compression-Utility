using System.IO.Compression;

namespace CatsCompression;

//handles compressing and extracting zip files
public static class FileCompUtility
{
    public static void CompressFile(string fileToCompress, string destinationFilePath)
    {
        if (SamePath(fileToCompress, destinationFilePath))
            throw new ArgumentException("The zip file can't replace the file being compressed. Choose a different name for the zip.");

        WriteViaTempFile(destinationFilePath, tempPath =>
        {
            using (FileStream zipToCreate = new FileStream(tempPath, FileMode.CreateNew))
            using (ZipArchive archive = new ZipArchive(zipToCreate, ZipArchiveMode.Create))
            {
                //copies the raw bytes of the file into the archive, so binary files are stored exactly and nothing is loaded into memory
                archive.CreateEntryFromFile(fileToCompress, Path.GetFileName(fileToCompress));
            }
        });
    }

    public static void CompressDirectory(string directoryPath, string destinationArchive)//creates zip archive from the directory and its contents
    {
        if (IsInsideDirectory(destinationArchive, directoryPath))
            throw new ArgumentException("The zip file can't be saved inside the folder being compressed. Choose a location outside that folder.");

        WriteViaTempFile(destinationArchive, tempPath => ZipFile.CreateFromDirectory(directoryPath, tempPath));
    }

    public static void DecompressFile(string zipFilePath, string destinationDirectory, bool overwriteFiles = false)//decompress the zip file to the specified directory
    {
        ZipFile.ExtractToDirectory(zipFilePath, destinationDirectory, overwriteFiles);
    }

    //lists the files in the archive that already exist in the destination, so the user can be asked before anything is extracted
    public static List<string> FindExistingFiles(string zipFilePath, string destinationDirectory)
    {
        using (ZipArchive archive = ZipFile.OpenRead(zipFilePath))
        {
            return archive.Entries
                .Where(entry => entry.Name != "")//folder entries have no file name
                .Select(entry => entry.FullName)
                .Where(name => File.Exists(Path.Combine(destinationDirectory, name)))
                .ToList();
        }
    }

    //writes to a temporary file first and only replaces the destination once it succeeded, so a failure never leaves a broken zip behind
    private static void WriteViaTempFile(string destinationPath, Action<string> write)
    {
        string tempPath = destinationPath + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            write(tempPath);
            File.Move(tempPath, destinationPath, overwrite: true);//the save dialog already asked the user before replacing
        }
        finally
        {
            if (File.Exists(tempPath))
                File.Delete(tempPath);
        }
    }

    private static bool SamePath(string first, string second)
    {
        return string.Equals(Path.GetFullPath(first), Path.GetFullPath(second), StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsInsideDirectory(string path, string directory)
    {
        string directoryWithSeparator = Path.TrimEndingDirectorySeparator(Path.GetFullPath(directory)) + Path.DirectorySeparatorChar;
        return Path.GetFullPath(path).StartsWith(directoryWithSeparator, StringComparison.OrdinalIgnoreCase);
    }

}
