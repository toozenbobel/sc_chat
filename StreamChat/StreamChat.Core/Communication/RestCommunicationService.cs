using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using RestSharp;
using StreamChat.Core.ServiceContracts;

namespace StreamChat.Core.Communication
{
	public class RestCommunicationService : ICommunicationService
	{
		private const int TIMEOUT = 10000;

		private readonly RestClient _client = new RestClient();

		public string SendWebRequest(string path, IDictionary<string, object> parameters, Method requestMethod = Method.POST)
		{
			try
			{
				// RestClient Client = new RestClient(ServerUrl);
				_client.ClearHandlers();
				AutoResetEvent autoReset = new AutoResetEvent(false);

				var request = new RestRequest();
				request.Method = requestMethod;
				request.Resource = path;

				if (parameters != null)
					foreach (var parameter in parameters)
					{
						request.AddParameter(parameter.Key, parameter.Value);
					}

				string result = string.Empty;
				Exception exception = null;


				//request.AddParameter("hash", DateTime.Now);

				_client.ExecuteAsync(request, response =>
				{
					try
					{
						result = response.Content;
					}
					catch (Exception ex)
					{
						exception = ex;
					}
					finally
					{
						autoReset.Set();
					}

				});

				if (autoReset.WaitOne(TIMEOUT))
				{
					if (exception != null)
						throw exception;

					return result;
				}

				throw new TimeoutException("Web request timed out");
			}
			catch (Exception e)
			{
				return null;
			}
		}
	}
}
