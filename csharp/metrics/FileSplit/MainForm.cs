using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace FileSplit
{
	public partial class MainForm : Form
	{
		private long _currentNumberOfLinesInFile;

		public MainForm ()
		{
			InitializeComponent();
		}

		private void HandleControls ()
		{
			_splitFileButton.Enabled = false;
			_cancelSplitButton.Enabled = false;
			_inputFileTextBox.Enabled = !_splitBackgroundWorker.IsBusy;
			if ( _inputFileTextBox.Text.Length > 0 && _outputPathTextBox.Text.Length > 0 )
			{
				if ( _splitBackgroundWorker.IsBusy )
				{
					_cancelSplitButton.Enabled = true;
				}
				else if ( !_calcFilesBackgroundWorker.IsBusy )
				{
					_splitFileButton.Enabled = true;

				}
			}
		}

		private void CalculateNumberOfFiles ()
		{
			_toolStripStatusLabel.Text = @"Calculating";
			_toolStripProgressBar.Visible = true;
			_cancelToolStripSplitButton.Visible = true;
			_currentNumberOfLinesInFile = 0;
			_calcFilesBackgroundWorker.RunWorkerAsync(
				new CalculateFilesWorker.WorkerArgs(_inputFileTextBox.Text,
													_headingsCheckBox.Checked,
													(int)_numberOfRecordsNumericUpDown.Value));
		}

		private void SplitFiles ()
		{
			_toolStripStatusLabel.Text = @"Splitting Files";
			_toolStripProgressBar.Visible = true;
			_cancelToolStripSplitButton.Visible = false;

			string outputFile = _outputPathTextBox.Text + @"\" + Path.GetFileName(_inputFileTextBox.Text);
			int numberOfFiles;
			int.TryParse(_numberOfFilesTextBox.Text, out numberOfFiles);

			_splitBackgroundWorker.RunWorkerAsync(
				new SplitFileWorker.WorkerArgs(_inputFileTextBox.Text,
									outputFile,
									(int)_numberOfRecordsNumericUpDown.Value,
									numberOfFiles,
									_currentNumberOfLinesInFile,
									_headingsCheckBox.Checked));
			
		}

		private void MainForm_Load ( object sender, EventArgs e )
		{
			HandleControls();
			_toolStripProgressBar.Visible = false;
		}

		private void _inputFileButton_Click ( object sender, EventArgs e )
		{
			if ( _inputFileOpenFileDialog.ShowDialog(this) == DialogResult.OK )
			{
				_inputFileTextBox.Text = _inputFileOpenFileDialog.FileName;
				_outputPathFolderBrowserDialog.SelectedPath = Path.GetDirectoryName(_inputFileTextBox.Text);
				_outputPathTextBox.Text = _outputPathFolderBrowserDialog.SelectedPath;
				CalculateNumberOfFiles();
			}
			HandleControls();
		}

		private void _outputPathButton_Click ( object sender, EventArgs e )
		{
			if ( _outputPathFolderBrowserDialog.ShowDialog(this) == DialogResult.OK )
			{
				_outputPathTextBox.Text = _outputPathFolderBrowserDialog.SelectedPath;
			}
			HandleControls();
		}

		private void _splitFileButton_Click ( object sender, EventArgs e )
		{
			SplitFiles();
			HandleControls();
		}

		private void _cancelSplitButton_Click ( object sender, EventArgs e )
		{
			_splitBackgroundWorker.CancelAsync();
		}

		private void _numberOfRecordsNumericUpDown_ValueChanged ( object sender, EventArgs e )
		{
			_numberOfFilesTextBox.Text = Math.Ceiling(_currentNumberOfLinesInFile / _numberOfRecordsNumericUpDown.Value).ToString();
		}

		private void _splitBackgroundWorker_DoWork ( object sender, DoWorkEventArgs e )
		{
			var splitFileWorker = new SplitFileWorker();
			var worker = sender as BackgroundWorker;
			e.Result = splitFileWorker.SplitFile(e.Argument as SplitFileWorker.WorkerArgs, worker);
			if ( worker.CancellationPending )
			{
				e.Cancel = true;
			}
		}

		private void _splitBackgroundWorker_ProgressChanged ( object sender, ProgressChangedEventArgs e )
		{
			_toolStripProgressBar.Value = e.ProgressPercentage;
		}

		private void _splitBackgroundWorker_RunWorkerCompleted ( object sender, RunWorkerCompletedEventArgs e )
		{
			_toolStripProgressBar.Value = 0;
			_toolStripProgressBar.Visible = false;
			if ( e.Error != null )
			{
				MessageBox.Show(this, e.Error.Message, "Split Files Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				_toolStripStatusLabel.Text = "Error";
			}
			else if ( e.Cancelled )
			{
				MessageBox.Show(this, "Cancelled", "Split Files", MessageBoxButtons.OK, MessageBoxIcon.Information);
				_toolStripStatusLabel.Text = "Cancelled";
			}
			else
			{
				_toolStripStatusLabel.Text = "";
			}
			HandleControls();
		}

		private void _calcFilesBackgroundWorker_DoWork ( object sender, DoWorkEventArgs e )
		{
			var worker = sender as BackgroundWorker;
			var filesWorker = new CalculateFilesWorker();
			e.Result = filesWorker.NumberOfFiles((CalculateFilesWorker.WorkerArgs)e.Argument, worker);
		}

		private void _calcFilesBackgroundWorker_ProgressChanged ( object sender, ProgressChangedEventArgs e )
		{
			var worker = sender as BackgroundWorker;
			_toolStripProgressBar.Value = e.ProgressPercentage;
		}

		private void _calcFilesBackgroundWorker_RunWorkerCompleted ( object sender, RunWorkerCompletedEventArgs e )
		{
			_toolStripProgressBar.Value = 0;
			_toolStripProgressBar.Visible = false;
			_cancelToolStripSplitButton.Visible = false;
			if ( e.Error != null )
			{
				MessageBox.Show(this, e.Error.Message, @"Calculate Files Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				_toolStripStatusLabel.Text = "Error";
			}
			else if ( e.Cancelled )
			{
				_toolStripStatusLabel.Text = @"Cancelled";
			}
			else
			{
				_toolStripStatusLabel.Text = "";
				var result = e.Result as CalculateFilesWorker.WorkerReturn;
				_currentNumberOfLinesInFile = result.NumberOfInputLines;
				_numberOfFilesTextBox.Text = result.NumberOfFiles.ToString();
			}
			HandleControls();
		}

		private void _cancelToolStripMenuItem_Click ( object sender, EventArgs e )
		{
			_calcFilesBackgroundWorker.CancelAsync();
		}

	}
}
