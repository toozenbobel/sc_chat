using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StreamChat.Core.ServiceContracts
{
	public interface IMessagePostingService
	{
		void PostMessage(string text);
	}

	public interface IAuthenticationService
	{
		bool DoAuth(string login, string password);
		string GetSession();
	}
}
