using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Threading;
using Ccr.MaterialDesign.MVVM;
using S3Uploader.AWS;
using S3Uploader.Models;
using S3Uploader.Settings;

namespace S3Uploader.ViewModels
{
	public class RootViewModel
		: ViewModelBase
	{
		private ObservableCollection<UploadedFile> _uploadedFiles;
		private string _fileUploadDirectory = AppSettings.Instance.UploadDirectory.FullName;
		private bool _automaticUpload = AppSettings.Instance.AutomaticUpload;
		private int _scanTimeSpanSeconds = AppSettings.Instance.ScanTimeSpanSeconds;
		private bool _showNotifications = true;
		private readonly DispatcherTimer _autoScanTimer;

		
		public ObservableCollection<UploadedFile> UploadedFiles
		{
			get => _uploadedFiles;
			set
			{
				_uploadedFiles = value;
				NotifyOfPropertyChange(() => UploadedFiles);
			}
		}

		public string FileUploadDirectory
		{
			get => _fileUploadDirectory;
			set
			{
				_fileUploadDirectory = value;
				AppSettings.Instance.UploadDirectory = new DirectoryInfo(value);
				NotifyOfPropertyChange(() => FileUploadDirectory);
			}
		}

		public bool AutomaticUpload
		{
			get => _automaticUpload;
			set
			{
				_automaticUpload = value;
				AppSettings.Instance.AutomaticUpload = value;
				NotifyOfPropertyChange(() => AutomaticUpload);

				if (value)
				{
					_autoScanTimer.Start();
				}
				else
				{
					_autoScanTimer.Stop();
				}
			}
		}

		public int ScanTimeSpanSeconds
		{
			get => _scanTimeSpanSeconds;
			set
			{
				_scanTimeSpanSeconds = value;
				AppSettings.Instance.ScanTimeSpanSeconds = value;
				NotifyOfPropertyChange(() => ScanTimeSpanSeconds);

				_autoScanTimer.Interval = TimeSpan.FromSeconds(value);
			}
		}

		public bool ShowNotifications
		{
			get => _showNotifications;
			set
			{
				_showNotifications = value;
				AppSettings.Instance.ShowNotifications = value;
				NotifyOfPropertyChange(() => ShowNotifications);
			}
		}


		public RootViewModel()
		{
			if (!AppSettings.Instance.UploadDirectory.Exists)
			{
				AppSettings.Instance.UploadDirectory.Create();

				AppSettings.Instance.UploadDirectory.CreateSubdirectory("Upload");
				AppSettings.Instance.UploadDirectory.CreateSubdirectory("Completed");
			}
			UploadedFiles = new ObservableCollection<UploadedFile>();
			_autoScanTimer = new DispatcherTimer()
			{
				Interval = TimeSpan.FromSeconds(ScanTimeSpanSeconds)
			};
			_autoScanTimer.Tick += onAutoScanTimerTick;

			if (AutomaticUpload)
			{
				_autoScanTimer.Start();
			}
		}

		private void onAutoScanTimerTick(object sender, EventArgs args)
		{
			_autoScanTimer.Stop();
			scanDirectory();
			_autoScanTimer.Start();
		}


		private void scanDirectory()
		{
			var uploadDirectory = new DirectoryInfo(
				AppSettings.Instance.UploadDirectory + @"\Upload\");
			var files = uploadDirectory.GetFiles();

			foreach (var file in files.Select(t => new UploadedFile(t)))
			{
				using (var awsClient = new AWSClient("jb-test-s3-1"))
				{
					UploadedFiles.Add(file);
					awsClient.UploadFileAsync(file.FileInfo)
						.ConfigureAwait(true);
				}

				file.HasUploaded = true;
				var destinationPath = file.FileInfo.Directory.Parent + @"\Completed\" + file.FileInfo.Name;
				file.FileInfo.MoveTo(destinationPath);
				var destinationFileInfo = new FileInfo(destinationPath);
				file.FileInfo = destinationFileInfo;
			}
		}
	}
}
