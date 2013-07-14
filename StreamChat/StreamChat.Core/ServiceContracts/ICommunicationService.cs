using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;

namespace StreamChat.Core.ServiceContracts
{
	public interface ICommunicationService
	{
		string SendWebRequest(string path, IDictionary<string, object> parameters, Method requestMethod = Method.POST);
	}
}
