namespace FileSplit
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose ( bool disposing )
		{
			if ( disposing && ( components != null ) )
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent ()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this._inputFileButton = new System.Windows.Forms.Button();
			this._inputFileTextBox = new System.Windows.Forms.TextBox();
			this._outputPathButton = new System.Windows.Forms.Button();
			this._outputPathFolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
			this._inputFileOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
			this._outputPathTextBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this._numberOfRecordsNumericUpDown = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this._numberOfFilesTextBox = new System.Windows.Forms.TextBox();
			this._splitFileButton = new System.Windows.Forms.Button();
			this._cancelSplitButton = new System.Windows.Forms.Button();
			this._headingsCheckBox = new System.Windows.Forms.CheckBox();
			this._splitBackgroundWorker = new System.ComponentModel.BackgroundWorker();
			this._calcFilesBackgroundWorker = new System.ComponentModel.BackgroundWorker();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this._toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this._toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
			this._cancelToolStripSplitButton = new System.Windows.Forms.ToolStripSplitButton();
			this._cancelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			((System.ComponentModel.ISupportInitialize)(this._numberOfRecordsNumericUpDown)).BeginInit();
			this.statusStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _inputFileButton
			// 
			this._inputFileButton.Location = new System.Drawing.Point(256, 36);
			this._inputFileButton.Name = "_inputFileButton";
			this._inputFileButton.Size = new System.Drawing.Size(75, 23);
			this._inputFileButton.TabIndex = 0;
			this._inputFileButton.Text = "Input File";
			this._inputFileButton.UseVisualStyleBackColor = true;
			this._inputFileButton.Click += new System.EventHandler(this._inputFileButton_Click);
			// 
			// _inputFileTextBox
			// 
			this._inputFileTextBox.Location = new System.Drawing.Point(14, 12);
			this._inputFileTextBox.Name = "_inputFileTextBox";
			this._inputFileTextBox.ReadOnly = true;
			this._inputFileTextBox.Size = new System.Drawing.Size(559, 20);
			this._inputFileTextBox.TabIndex = 1;
			// 
			// _outputPathButton
			// 
			this._outputPathButton.Location = new System.Drawing.Point(256, 95);
			this._outputPathButton.Name = "_outputPathButton";
			this._outputPathButton.Size = new System.Drawing.Size(75, 23);
			this._outputPathButton.TabIndex = 2;
			this._outputPathButton.Text = "Output Path";
			this._outputPathButton.UseVisualStyleBackColor = true;
			this._outputPathButton.Click += new System.EventHandler(this._outputPathButton_Click);
			// 
			// _inputFileOpenFileDialog
			// 
			this._inputFileOpenFileDialog.Filter = "CSV files|*.csv|All Files|*.*";
			this._inputFileOpenFileDialog.SupportMultiDottedExtensions = true;
			this._inputFileOpenFileDialog.Title = "Select Input File";
			// 
			// _outputPathTextBox
			// 
			this._outputPathTextBox.Location = new System.Drawing.Point(14, 68);
			this._outputPathTextBox.Name = "_outputPathTextBox";
			this._outputPathTextBox.ReadOnly = true;
			this._outputPathTextBox.Size = new System.Drawing.Size(559, 20);
			this._outputPathTextBox.TabIndex = 3;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(88, 135);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(131, 13);
			this.label1.TabIndex = 4;
			this.label1.Text = "Number of records per file:";
			// 
			// _numberOfRecordsNumericUpDown
			// 
			this._numberOfRecordsNumericUpDown.Increment = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this._numberOfRecordsNumericUpDown.Location = new System.Drawing.Point(222, 131);
			this._numberOfRecordsNumericUpDown.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
			this._numberOfRecordsNumericUpDown.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			this._numberOfRecordsNumericUpDown.Name = "_numberOfRecordsNumericUpDown";
			this._numberOfRecordsNumericUpDown.Size = new System.Drawing.Size(78, 20);
			this._numberOfRecordsNumericUpDown.TabIndex = 5;
			this._numberOfRecordsNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this._numberOfRecordsNumericUpDown.ThousandsSeparator = true;
			this._numberOfRecordsNumericUpDown.Value = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
			this._numberOfRecordsNumericUpDown.ValueChanged += new System.EventHandler(this._numberOfRecordsNumericUpDown_ValueChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(316, 135);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(80, 13);
			this.label2.TabIndex = 6;
			this.label2.Text = "Number of files:";
			// 
			// _numberOfFilesTextBox
			// 
			this._numberOfFilesTextBox.Location = new System.Drawing.Point(399, 131);
			this._numberOfFilesTextBox.Name = "_numberOfFilesTextBox";
			this._numberOfFilesTextBox.ReadOnly = true;
			this._numberOfFilesTextBox.Size = new System.Drawing.Size(100, 20);
			this._numberOfFilesTextBox.TabIndex = 7;
			this._numberOfFilesTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// _splitFileButton
			// 
			this._splitFileButton.Location = new System.Drawing.Point(144, 173);
			this._splitFileButton.Name = "_splitFileButton";
			this._splitFileButton.Size = new System.Drawing.Size(75, 23);
			this._splitFileButton.TabIndex = 9;
			this._splitFileButton.Text = "Split File";
			this._splitFileButton.UseVisualStyleBackColor = true;
			this._splitFileButton.Click += new System.EventHandler(this._splitFileButton_Click);
			// 
			// _cancelSplitButton
			// 
			this._cancelSplitButton.Location = new System.Drawing.Point(366, 173);
			this._cancelSplitButton.Name = "_cancelSplitButton";
			this._cancelSplitButton.Size = new System.Drawing.Size(75, 23);
			this._cancelSplitButton.TabIndex = 10;
			this._cancelSplitButton.Text = "Cancel Split";
			this._cancelSplitButton.UseVisualStyleBackColor = true;
			this._cancelSplitButton.Click += new System.EventHandler(this._cancelSplitButton_Click);
			// 
			// _headingsCheckBox
			// 
			this._headingsCheckBox.AutoSize = true;
			this._headingsCheckBox.Checked = true;
			this._headingsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this._headingsCheckBox.Location = new System.Drawing.Point(357, 39);
			this._headingsCheckBox.Name = "_headingsCheckBox";
			this._headingsCheckBox.Size = new System.Drawing.Size(129, 17);
			this._headingsCheckBox.TabIndex = 11;
			this._headingsCheckBox.Text = "First Row Is Headings";
			this._headingsCheckBox.UseVisualStyleBackColor = true;
			// 
			// _splitBackgroundWorker
			// 
			this._splitBackgroundWorker.WorkerReportsProgress = true;
			this._splitBackgroundWorker.WorkerSupportsCancellation = true;
			this._splitBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(_splitBackgroundWorker_DoWork);
			this._splitBackgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this._splitBackgroundWorker_ProgressChanged);
			this._splitBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this._splitBackgroundWorker_RunWorkerCompleted);
			// 
			// _calcFilesBackgroundWorker
			// 
			this._calcFilesBackgroundWorker.WorkerReportsProgress = true;
			this._calcFilesBackgroundWorker.WorkerSupportsCancellation = true;
			this._calcFilesBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this._calcFilesBackgroundWorker_DoWork);
			this._calcFilesBackgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this._calcFilesBackgroundWorker_ProgressChanged);
			this._calcFilesBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this._calcFilesBackgroundWorker_RunWorkerCompleted);
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._toolStripStatusLabel,
            this._toolStripProgressBar,
            this._cancelToolStripSplitButton});
			this.statusStrip1.Location = new System.Drawing.Point(0, 215);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(586, 22);
			this.statusStrip1.TabIndex = 12;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// _toolStripStatusLabel
			// 
			this._toolStripStatusLabel.Name = "_toolStripStatusLabel";
			this._toolStripStatusLabel.Size = new System.Drawing.Size(161, 17);
			this._toolStripStatusLabel.Text = "Select input file and output path";
			// 
			// _toolStripProgressBar
			// 
			this._toolStripProgressBar.Name = "_toolStripProgressBar";
			this._toolStripProgressBar.Size = new System.Drawing.Size(200, 16);
			this._toolStripProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			// 
			// _cancelToolStripSplitButton
			// 
			this._cancelToolStripSplitButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this._cancelToolStripSplitButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._cancelToolStripMenuItem});
			this._cancelToolStripSplitButton.Image = ((System.Drawing.Image)(resources.GetObject("_cancelToolStripSplitButton.Image")));
			this._cancelToolStripSplitButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._cancelToolStripSplitButton.Name = "_cancelToolStripSplitButton";
			this._cancelToolStripSplitButton.Size = new System.Drawing.Size(32, 20);
			this._cancelToolStripSplitButton.Text = "toolStripSplitButton1";
			// 
			// _cancelToolStripMenuItem
			// 
			this._cancelToolStripMenuItem.Name = "_cancelToolStripMenuItem";
			this._cancelToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this._cancelToolStripMenuItem.Text = "Cancel";
			this._cancelToolStripMenuItem.Click += new System.EventHandler(this._cancelToolStripMenuItem_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(586, 237);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this._headingsCheckBox);
			this.Controls.Add(this._cancelSplitButton);
			this.Controls.Add(this._splitFileButton);
			this.Controls.Add(this._numberOfFilesTextBox);
			this.Controls.Add(this.label2);
			this.Controls.Add(this._numberOfRecordsNumericUpDown);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._outputPathTextBox);
			this.Controls.Add(this._outputPathButton);
			this.Controls.Add(this._inputFileTextBox);
			this.Controls.Add(this._inputFileButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.Text = "File Split";
			this.Load += new System.EventHandler(this.MainForm_Load);
			((System.ComponentModel.ISupportInitialize)(this._numberOfRecordsNumericUpDown)).EndInit();
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button _inputFileButton;
		private System.Windows.Forms.TextBox _inputFileTextBox;
		private System.Windows.Forms.Button _outputPathButton;
		private System.Windows.Forms.FolderBrowserDialog _outputPathFolderBrowserDialog;
		private System.Windows.Forms.OpenFileDialog _inputFileOpenFileDialog;
		private System.Windows.Forms.TextBox _outputPathTextBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown _numberOfRecordsNumericUpDown;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox _numberOfFilesTextBox;
		private System.Windows.Forms.Button _splitFileButton;
		private System.Windows.Forms.Button _cancelSplitButton;
		private System.Windows.Forms.CheckBox _headingsCheckBox;
		private System.ComponentModel.BackgroundWorker _splitBackgroundWorker;
		private System.ComponentModel.BackgroundWorker _calcFilesBackgroundWorker;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel _toolStripStatusLabel;
		private System.Windows.Forms.ToolStripProgressBar _toolStripProgressBar;
		private System.Windows.Forms.ToolStripSplitButton _cancelToolStripSplitButton;
		private System.Windows.Forms.ToolStripMenuItem _cancelToolStripMenuItem;
	}
}

