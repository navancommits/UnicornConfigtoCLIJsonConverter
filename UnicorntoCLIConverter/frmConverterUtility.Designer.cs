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
            chkMoveJsontoserfolderloc = new CheckBox();
            SuspendLayout();
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(1542, 124);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(382, 32);
            label2.TabIndex = 14;
            label2.Text = "Converted CLI JSON file data here:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(68, 113);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(390, 32);
            label1.TabIndex = 13;
            label1.Text = "Paste Unicorn Config file data here:";
            // 
            // txtJson
            // 
            txtJson.Location = new Point(1533, 174);
            txtJson.Margin = new Padding(4, 4, 4, 4);
            txtJson.Name = "txtJson";
            txtJson.Size = new Size(1317, 1258);
            txtJson.TabIndex = 12;
            txtJson.Text = "";
            txtJson.TextChanged += txtJson_TextChanged;
            // 
            // txtConfig
            // 
            txtConfig.Location = new Point(75, 174);
            txtConfig.Margin = new Padding(4, 4, 4, 4);
            txtConfig.Name = "txtConfig";
            txtConfig.Size = new Size(1233, 1258);
            txtConfig.TabIndex = 11;
            txtConfig.Text = "";
            // 
            // btnPreview
            // 
            btnPreview.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            btnPreview.Location = new Point(1336, 722);
            btnPreview.Margin = new Padding(4, 4, 4, 4);
            btnPreview.Name = "btnPreview";
            btnPreview.Size = new Size(162, 96);
            btnPreview.TabIndex = 10;
            btnPreview.Text = "Preview>>";
            btnPreview.UseVisualStyleBackColor = true;
            btnPreview.Click += btnPreview_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(68, 33);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(356, 32);
            label3.TabIndex = 15;
            label3.Text = "Folder Path for Bulk Conversion:";
            // 
            // txtSelectedPath
            // 
            txtSelectedPath.Location = new Point(461, 33);
            txtSelectedPath.Margin = new Padding(4, 4, 4, 4);
            txtSelectedPath.Name = "txtSelectedPath";
            txtSelectedPath.Size = new Size(847, 39);
            txtSelectedPath.TabIndex = 16;
            // 
            // button1
            // 
            button1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            button1.Location = new Point(1336, 23);
            button1.Margin = new Padding(4, 4, 4, 4);
            button1.Name = "button1";
            button1.Size = new Size(162, 60);
            button1.TabIndex = 17;
            button1.Text = "Browse";
            button1.UseVisualStyleBackColor = true;
            button1.Click += BrowseFolderButton_Click;
            // 
            // button2
            // 
            button2.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            button2.Location = new Point(1521, 23);
            button2.Margin = new Padding(4, 4, 4, 4);
            button2.Name = "button2";
            button2.Size = new Size(162, 60);
            button2.TabIndex = 18;
            button2.Text = "Convert";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // chkMoveJsontoserfolderloc
            // 
            chkMoveJsontoserfolderloc.AutoSize = true;
            chkMoveJsontoserfolderloc.Location = new Point(1700, 36);
            chkMoveJsontoserfolderloc.Name = "chkMoveJsontoserfolderloc";
            chkMoveJsontoserfolderloc.Size = new Size(666, 36);
            chkMoveJsontoserfolderloc.TabIndex = 19;
            chkMoveJsontoserfolderloc.Text = "Ensure JSON File is in same location as Serialization folder";
            chkMoveJsontoserfolderloc.UseVisualStyleBackColor = true;
            // 
            // frmConverterUtility
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(2481, 1471);
            Controls.Add(chkMoveJsontoserfolderloc);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(txtSelectedPath);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(txtJson);
            Controls.Add(txtConfig);
            Controls.Add(btnPreview);
            Margin = new Padding(4, 4, 4, 4);
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
        private CheckBox chkMoveJsontoserfolderloc;
    }
}
