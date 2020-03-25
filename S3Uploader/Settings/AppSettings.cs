using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using S3Uploader.Models;

namespace S3Uploader.Settings
{
	[Serializable]
	public class AppSettings
	{
		#region Singleton
		private static AppSettings _instance;
		public static AppSettings Instance
		{
			get => _instance ??= LoadFromBinaryStream();
		}

		#endregion

		public static DirectoryInfo DataStorageDirectory = new DirectoryInfo(
			Environment.GetFolderPath(
				Environment.SpecialFolder.LocalApplicationData)
			+ @"\S3Uploader\UserConfig\");

		public static FileInfo DataStorageFile = new FileInfo(
			DataStorageDirectory.FullName + @"\AppSettings.bin");


		private string _theme = "DeepPurple";
		private UploadedFile[] _uploadedFiles;
		private DirectoryInfo _uploadDirectory = new DirectoryInfo(
			Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
			+ @"\S3Uploader\");

		private bool _automaticUpload = true;
		private int _scanTimeSpanSeconds = 5;
		private bool _showNotifications = true;



		private AppSettings()
		{
		}
		
		
		public string Theme
		{
			get => Instance._theme;
			set
			{
				Instance._theme = value;
				OnPropertyChanged();
			}
		}

		public UploadedFile[] UploadedFiles
		{
			get => _uploadedFiles;
			set
			{
				Instance._uploadedFiles = value;
				OnPropertyChanged();
			}
		}

		public DirectoryInfo UploadDirectory
		{
			get => _uploadDirectory;
			set
			{
				Instance._uploadDirectory = value;
				OnPropertyChanged();

				if (!value.Exists)
				{
					value.Create();
					value.CreateSubdirectory("Upload");
					value.CreateSubdirectory("Completed");
				}
			}
		}

		public bool AutomaticUpload
		{
			get => _automaticUpload;
			set
			{
				Instance._automaticUpload = value;
				OnPropertyChanged();
			}
		}

		public int ScanTimeSpanSeconds
		{
			get => _scanTimeSpanSeconds;
			set
			{
				Instance._scanTimeSpanSeconds = value;
				OnPropertyChanged();
			}
		}


		public bool ShowNotifications
		{
			get => _showNotifications;
			set
			{
				Instance._showNotifications = value;
				OnPropertyChanged();
			}
		}

		public void SaveToBinaryStream()
		{
			IFormatter formatter = new BinaryFormatter();
			if (!DataStorageDirectory.Exists)
				DataStorageDirectory.Create();

			Stream stream = new FileStream(
				DataStorageFile.FullName,
				FileMode.Create, 
				FileAccess.Write, 
				FileShare.None);

			formatter.Serialize(stream, this);
			stream.Close();
		}

		private static AppSettings LoadFromBinaryStream()
		{
			if (!DataStorageFile.Exists)
				return new AppSettings();

			try
			{
				IFormatter formatter = new BinaryFormatter();
				using Stream stream = new FileStream(
					DataStorageFile.FullName, 
					FileMode.Open, 
					FileAccess.Read, 
					FileShare.Read);

				var obj = (AppSettings)formatter.Deserialize(stream);
				stream.Close();
				return obj;
			}
			catch
			{
				return new AppSettings();
			}
		}

		#region INotifyPropertyChanged
		[field: NonSerialized]
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(
			[CallerMemberName] string PropertyName = null)
		{
			SaveToBinaryStream();

			PropertyChanged?.Invoke(
				this,
				new PropertyChangedEventArgs(PropertyName));
		}
		#endregion
	}
}
