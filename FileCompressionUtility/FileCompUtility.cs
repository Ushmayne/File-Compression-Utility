using System.IO.Compression;
using System.IO;
//a class to handle file compression and decompression

public class FileCompUtility
{
    public static void CompressFile(string fileToCompress, string destinationFilePath)
    {
        //creating new file stream to create a zip archive
        using (FileStream zipToCreate = new FileStream(destinationFilePath, FileMode.Create))
        using (ZipArchive archive = new ZipArchive(zipToCreate, ZipArchiveMode.Update))
        {
            ZipArchiveEntry zipEntry = archive.CreateEntry(Path.GetFileName(fileToCompress));//creating a new entry

            //writing the contents of the file to the zip entry
            using (StreamWriter writer = new StreamWriter(zipEntry.Open()))
            using (StreamReader reader = new StreamReader(fileToCompress))
            {
                writer.Write(reader.ReadToEnd());
            }
        }
    }

    public void CompressDirectory(string directoryPath, string destinationArchive)//creates zup archive from the directory abd its contents
    {
        ZipFile.CreateFromDirectory(directoryPath, destinationArchive);
    }

    public void DecompressFile(string zipFilePath, string destinationDirectory)//decompress the zip file to the specified directory
    {
        ZipFile.ExtractToDirectory(zipFilePath, destinationDirectory);
    }

}