using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Cirrious.MvvmCross.Plugins.File;
using Newtonsoft.Json;
using StreamChat.Core.ServiceContracts;

namespace StreamChat.Core.ViewModels
{
	public class Settings
	{
		public bool PreventScreenLock { get; set; }
	}

	public class SettingsPageViewModel : ViewModelBase
	{
		public const string FILENAME = "settings.json";

		private readonly IScreenLockService _screenLockService;
		private readonly IMvxFileStore _fileService;

		public SettingsPageViewModel(IScreenLockService screenLockService, IMvxFileStore fileService)
		{
			_screenLockService = screenLockService;
			_fileService = fileService;
		}

		public void Init()
		{
			LoadSettingsFromFile();
		}

		private bool _isBusy;
		public bool IsBusy
		{
			get
			{
				return _isBusy;
			}
			set
			{
				_isBusy = value;
				OnPropertyChanged(() => IsBusy);
			}
		}

		private bool _preventScreenLock;
		public bool PreventScreenLock
		{
			get
			{
				return _preventScreenLock;
			}
			set
			{
				_preventScreenLock = value;
				OnPropertyChanged(() => PreventScreenLock);
				UpdateService();
			}
		}

		private void UpdateService()
		{
			if (PreventScreenLock)
				_screenLockService.DisableScreenLock();
			else
				_screenLockService.EnableScreenLock();

			WriteSettingsToFile();
		}

		private void LoadSettingsFromFile()
		{
			string json;
			if (_fileService.TryReadTextFile(FILENAME, out json) && json != null)
			{
				Settings fromFile = JsonConvert.DeserializeObject<Settings>(json);
				if (fromFile != null)
				{
					PreventScreenLock = fromFile.PreventScreenLock;
				}
			}
		}

		private void WriteSettingsToFile()
		{
			IsBusy = true;

			bool preventScreenLock = PreventScreenLock;

			_fileService.WriteFile(FILENAME, stream =>
				{
					using (var writer = new StreamWriter(stream))
					{
						var jsonString = JsonConvert.SerializeObject(new Settings()
							{
								PreventScreenLock = preventScreenLock
							});
						writer.Write(jsonString);
					}
				});

			IsBusy = false;
		}
	}
}
