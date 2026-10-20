namespace CatsCompression;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
        using (Stream iconStream = typeof(MainForm).Assembly.GetManifestResourceStream("app.ico")!)
            Icon = new Icon(iconStream);
    }
    private async void btnCompressFileClick(object sender, EventArgs e)//response to when compress file button is clicked
    {
        using (OpenFileDialog openFileDialog = new OpenFileDialog())
        {
            openFileDialog.Filter = "All Files (*.*)|*.*|Text Files (*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileToCompress = openFileDialog.FileName;
                using SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "ZIP Archive (*.zip)|*.zip";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string destinationArchive = saveFileDialog.FileName;
                    await RunOperation(() => FileCompUtility.CompressFile(fileToCompress, destinationArchive), "File compressed successfully!");
                }
            }
        }
    }


    private async void btnDecompressFileClick(object sender, EventArgs e)//response to when decompress file button is clicked
    {
        using (OpenFileDialog openFileDialog = new OpenFileDialog())
        {
            openFileDialog.Filter = "ZIP Files (*.zip)|*.zip";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string zipFilePath = openFileDialog.FileName;
                using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
                {
                    if (folderDialog.ShowDialog() == DialogResult.OK)
                    {
                        string destinationDirectory = folderDialog.SelectedPath;

                        //checking for files that would be replaced before extracting anything
                        List<string> existingFiles;
                        try
                        {
                            existingFiles = FileCompUtility.FindExistingFiles(zipFilePath, destinationDirectory);
                        }
                        catch (Exception ex)
                        {
                            ShowError(ex);
                            return;
                        }

                        bool overwrite = false;
                        if (existingFiles.Count > 0)
                        {
                            DialogResult answer = MessageBox.Show($"{existingFiles.Count} file(s) in this archive already exist in the destination folder (for example \"{existingFiles[0]}\").\n\nDo you want to overwrite them?", "Files already exist", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (answer != DialogResult.Yes)
                                return;
                            overwrite = true;
                        }

                        await RunOperation(() => FileCompUtility.DecompressFile(zipFilePath, destinationDirectory, overwrite), "File decompressed successfully!");
                    }
                }
            }
        }
    }

    private async void btnCompressDirectoryClick(object sender, EventArgs e)//response to when compress directory button is clicked
    {
        using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
        {
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                string directoryToCompress = folderDialog.SelectedPath;
                using SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "ZIP Archive (*.zip)|*.zip";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string destinationArchive = saveFileDialog.FileName;
                    await RunOperation(() => FileCompUtility.CompressDirectory(directoryToCompress, destinationArchive), "Directory compressed successfully!");
                }
            }
        }
    }

    private void instructionsLabelClick(object sender, EventArgs e)//response to when instructions button is clicked
    {
        MessageBox.Show("Instructions:\n1. Click 'Compress File' to compress a single file.\n2. Click 'Decompress File' to decompress a ZIP file.\n3. Click 'Compress Directory' to compress an entire directory.", "Instructions", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    //runs the work off the UI thread so the window stays responsive, and shows a readable message if it fails
    private async Task RunOperation(Action operation, string successMessage)
    {
        SetBusy(true);
        try
        {
            await Task.Run(operation);
            MessageBox.Show(successMessage);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
        finally
        {
            SetBusy(false);
        }
    }

    private void SetBusy(bool busy)//disables the buttons while an operation is running
    {
        btnCompressFile.Enabled = !busy;
        btnDecompressFile.Enabled = !busy;
        btnCompressDirectory.Enabled = !busy;
        UseWaitCursor = busy;
    }

    private static void ShowError(Exception ex)//turns exceptions into messages the user can act on
    {
        string message = ex switch
        {
            ArgumentException => ex.Message,
            FileNotFoundException or DirectoryNotFoundException => "The file or folder could not be found. It may have been moved or deleted.",
            PathTooLongException => "The file path is too long. Try a shorter folder or file name.",
            UnauthorizedAccessException => "Access was denied. Check that you have permission to read the source and write to the destination.",
            InvalidDataException => "This file is not a valid zip archive, or it is damaged.",
            IOException => "The operation could not be completed: " + ex.Message,
            _ => "Something went wrong: " + ex.Message
        };
        MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }


}
