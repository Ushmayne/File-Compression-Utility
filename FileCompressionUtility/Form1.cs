namespace FileCompressionUtility;
using System.IO;
using System.Windows.Forms;
using System.IO.Compression;


public partial class Form1 : Form
{

    private FileCompUtility fileCompressionUtility;
    public Form1()
    {
        InitializeComponent();
        fileCompressionUtility = new FileCompUtility();
    }
    private void btnCompressFileClick(object sender, EventArgs e)//response to when compress file button is clicked
    {
        using (OpenFileDialog openFileDialog = new OpenFileDialog())
        {
            openFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileToCompress = openFileDialog.FileName;
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "ZIP Archive (*.zip)|*.zip";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string destinationArchive = saveFileDialog.FileName;
                    FileCompUtility.CompressFile(fileToCompress, destinationArchive);
                    MessageBox.Show("File compressed successfully!");
                }
            }
        }
    }


    private void btnDecompressFileClick(object sender, EventArgs e)//response to when decompress file button is clicked
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
                        fileCompressionUtility.DecompressFile(zipFilePath, destinationDirectory);
                        MessageBox.Show("File decompressed successfully!");
                    }
                }
            }
        }
    }

    private void btnCompressDirectoryClick(object sender, EventArgs e)//response to when compress directory button is clicked
    {
        using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
        {
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                string directoryToCompress = folderDialog.SelectedPath;
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "ZIP Archive (*.zip)|*.zip";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string destinationArchive = saveFileDialog.FileName;
                    fileCompressionUtility.CompressDirectory(directoryToCompress, destinationArchive);
                    MessageBox.Show("Directory compressed successfully!");
                }
            }
        }
    }

    private void instructionsLabelClick(object sender, EventArgs e)//response to when instructions button is clicked
    {
        MessageBox.Show("Instructions:\n1. Click 'Compress File' to compress a single file.\n2. Click 'Decompress File' to decompress a ZIP file.\n3. Click 'Compress Directory' to compress an entire directory.", "Instructions", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
    
    
}
