using System;
using System.IO;
using Ccr.MaterialDesign.MVVM;

namespace S3Uploader.Models
{
	[Serializable]
	public class UploadedFile
		: ViewModelBase
	{
		private bool _hasUploaded;
		private FileInfo _fileInfo;
		private DateTime _uploadTimeStamp;


		public bool HasUploaded
		{
			get => _hasUploaded;
			set
			{
				_hasUploaded = value;
				NotifyOfPropertyChange(() => HasUploaded);
			}
		}

		public FileInfo FileInfo
		{
			get => _fileInfo;
			set
			{
				_fileInfo = value;
				NotifyOfPropertyChange(() => FileInfo);
			}
		}

		public DateTime UploadTimeStamp
		{
			get => _uploadTimeStamp;
			set
			{
				_uploadTimeStamp = value;
				NotifyOfPropertyChange(() => UploadTimeStamp);
			}
		}


		public UploadedFile()
		{
		}

		public UploadedFile(
			FileInfo fileInfo) : this()
		{
			FileInfo = fileInfo;
			UploadTimeStamp = DateTime.Now;
		}
	}
}