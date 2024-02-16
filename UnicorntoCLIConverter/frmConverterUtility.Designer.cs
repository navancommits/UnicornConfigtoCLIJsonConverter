namespace UnicorntoCLIConverter
{
    partial class frmConverterUtility
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label2 = new Label();
            label1 = new Label();
            txtJson = new RichTextBox();
            txtConfig = new RichTextBox();
            btnPreview = new Button();
            label3 = new Label();
            BrowseFolderButton = new FolderBrowserDialog();
            txtSelectedPath = new TextBox();
            button1 = new Button();
            button2 = new Button();
            SuspendLayout();
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(906, 95);
            label2.Name = "label2";
            label2.Size = new Size(282, 25);
            label2.TabIndex = 14;
            label2.Text = "Converted CLI JSON file data here:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(73, 83);
            label1.Name = "label1";
            label1.Size = new Size(288, 25);
            label1.TabIndex = 13;
            label1.Text = "Paste Unicorn Config file data here:";
            // 
            // txtJson
            // 
            txtJson.Location = new Point(906, 135);
            txtJson.Name = "txtJson";
            txtJson.Size = new Size(761, 655);
            txtJson.TabIndex = 12;
            txtJson.Text = "";
            // 
            // txtConfig
            // 
            txtConfig.Location = new Point(73, 135);
            txtConfig.Name = "txtConfig";
            txtConfig.Size = new Size(678, 648);
            txtConfig.TabIndex = 11;
            txtConfig.Text = "";
            // 
            // btnPreview
            // 
            btnPreview.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            btnPreview.Location = new Point(766, 406);
            btnPreview.Name = "btnPreview";
            btnPreview.Size = new Size(125, 75);
            btnPreview.TabIndex = 10;
            btnPreview.Text = "Preview>>";
            btnPreview.UseVisualStyleBackColor = true;
            btnPreview.Click += btnPreview_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(73, 26);
            label3.Name = "label3";
            label3.Size = new Size(265, 25);
            label3.TabIndex = 15;
            label3.Text = "Folder Path for Bulk Conversion:";
            // 
            // txtSelectedPath
            // 
            txtSelectedPath.Location = new Point(355, 26);
            txtSelectedPath.Name = "txtSelectedPath";
            txtSelectedPath.Size = new Size(833, 31);
            txtSelectedPath.TabIndex = 16;
            // 
            // button1
            // 
            button1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            button1.Location = new Point(1194, 18);
            button1.Name = "button1";
            button1.Size = new Size(125, 47);
            button1.TabIndex = 17;
            button1.Text = "Browse";
            button1.UseVisualStyleBackColor = true;
            button1.Click += BrowseFolderButton_Click;
            // 
            // button2
            // 
            button2.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            button2.Location = new Point(1338, 18);
            button2.Name = "button2";
            button2.Size = new Size(125, 47);
            button2.TabIndex = 18;
            button2.Text = "Convert";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // frmConverterUtility
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1741, 842);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(txtSelectedPath);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(txtJson);
            Controls.Add(txtConfig);
            Controls.Add(btnPreview);
            Name = "frmConverterUtility";
            Text = "Convert Unicorn Config File to Sitecore CLI Module JSON File";
            WindowState = FormWindowState.Maximized;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label2;
        private Label label1;
        private RichTextBox txtJson;
        private RichTextBox txtConfig;
        private Button btnPreview;
        private Label label3;
        private FolderBrowserDialog BrowseFolderButton;
        private TextBox txtSelectedPath;
        private Button button1;
        private Button button2;
    }
}
