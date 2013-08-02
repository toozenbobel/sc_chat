using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StreamChat.Core.ServiceContracts
{
	public interface IScreenLockService
	{
		void EnableScreenLock();
		void DisableScreenLock();
	}
}
