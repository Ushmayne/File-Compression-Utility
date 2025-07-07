namespace FileCompressionUtility;

partial class Form1
{
     private System.ComponentModel.IContainer components = null;//declaring the buttons that are going to be used in the form
    private Button btnCompressFile;
    private Button btnDecompressFile;
    private Button btnCompressDirectory;
    private Button instructionsLabel;
    
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    // Initializing all the components of form
    private void InitializeComponent()
    {
        this.btnCompressFile = new System.Windows.Forms.Button();
        this.btnDecompressFile = new System.Windows.Forms.Button();
        this.btnCompressDirectory = new System.Windows.Forms.Button();
        this.instructionsLabel = new System.Windows.Forms.Button();



        // btnCompressFile
        this.btnCompressFile.Location = new System.Drawing.Point(50, 30);
        this.btnCompressFile.Name = "btnCompressFile";
        this.btnCompressFile.Size = new System.Drawing.Size(200, 30);
        this.btnCompressFile.TabIndex = 0;
        this.btnCompressFile.Text = "Compress File";
        this.btnCompressFile.UseVisualStyleBackColor = true;
        this.btnCompressFile.Click += new System.EventHandler(this.btnCompressFileClick);
        
        // btnDecompressFile
        this.btnDecompressFile.Location = new System.Drawing.Point(50, 80);
        this.btnDecompressFile.Name = "btnDecompressFile";
        this.btnDecompressFile.Size = new System.Drawing.Size(200, 30);
        this.btnDecompressFile.TabIndex = 1;
        this.btnDecompressFile.Text = "Decompress File";
        this.btnDecompressFile.UseVisualStyleBackColor = true;
        this.btnDecompressFile.Click += new System.EventHandler(this.btnDecompressFileClick);
        
        // btnCompressDirectory
        this.btnCompressDirectory.Location = new System.Drawing.Point(50, 130);
        this.btnCompressDirectory.Name = "btnCompressDirectory";
        this.btnCompressDirectory.Size = new System.Drawing.Size(200, 30);
        this.btnCompressDirectory.TabIndex = 2;
        this.btnCompressDirectory.Text = "Compress Directory";
        this.btnCompressDirectory.UseVisualStyleBackColor = true;
        this.btnCompressDirectory.Click += new System.EventHandler(this.btnCompressDirectoryClick);


        //instructionsLabel
        this.instructionsLabel.Location = new System.Drawing.Point(50, 180);
        this.instructionsLabel.Name = "instructionsLabel";
        this.instructionsLabel.TabIndex = 3;
        this.instructionsLabel.Size = new System.Drawing.Size(200, 30);
        this.instructionsLabel.Text = "Click for instructions";
        this.instructionsLabel.UseVisualStyleBackColor = true;
        this.instructionsLabel.Click += new System.EventHandler(this.instructionsLabelClick);

        // Form1
        this.ClientSize = new System.Drawing.Size(300, 260);
        this.Controls.Add(this.btnCompressDirectory);
        this.Controls.Add(this.btnDecompressFile);
        this.Controls.Add(this.btnCompressFile);
        this.Controls.Add(this.instructionsLabel);
        this.Name = "Zip and Unzip Please";
        this.Text = "Zip and Unzip Please";
        this.ResumeLayout(false);
        this.PerformLayout();
    }
}
