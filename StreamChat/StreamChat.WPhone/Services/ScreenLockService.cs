using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Phone.Shell;
using StreamChat.Core.ServiceContracts;

namespace StreamChat.WPhone.Services
{
	public class ScreenLockService : IScreenLockService
	{
		private readonly object _lockObject = new object();

		private bool _isEnabled = true;

		public void EnableScreenLock()
		{
			lock (_lockObject)
			{
				if (!_isEnabled)
				{
					PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Enabled;
					_isEnabled = true;
				}
			}
		}

		public void DisableScreenLock()
		{
			lock (_lockObject)
			{
				if (_isEnabled)
				{
					PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
					_isEnabled = false;
				}
			}
		}
	}
}
