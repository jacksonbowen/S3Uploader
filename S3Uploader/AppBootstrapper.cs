using System.Collections.Generic;
using System.Windows;
using Caliburn.Micro;
using S3Uploader.ViewModels;

namespace S3Uploader
{
	public class AppBootstrapper
		: BootstrapperBase
	{
		public AppBootstrapper()
		{
			Initialize();
		}


		protected override void OnStartup(object sender, StartupEventArgs args)
		{
			var settings = new Dictionary<string, object>
			{
				{ "SizeToContent", SizeToContent.Manual },
//				{ "WindowState" , WindowState.Maximized }
				{ "Height", 600 },
				{ "Width", 1000 }
			};

			DisplayRootViewFor<RootViewModel>(settings);
		}
	}
}
