using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

		public string GetString(string path)
		{
			AutoResetEvent autoReset = new AutoResetEvent(false);
			Exception exception = null;
			string result = string.Empty;

			HttpWebRequest request = WebRequest.CreateHttp(path);
			request.BeginGetResponse(ar =>
				{
					try
					{
						HttpWebRequest req = (HttpWebRequest)ar.AsyncState;
						HttpWebResponse response = (HttpWebResponse)req.EndGetResponse(ar);
						using (StreamReader reader = new StreamReader(response.GetResponseStream()))
						{
							string resultString = reader.ReadToEnd();
							result = resultString;
						}
					}
					catch (Exception ex)
					{
						exception = ex;
					}
					finally
					{
						autoReset.Set();
					}
				}, request);

			if (autoReset.WaitOne(TIMEOUT))
			{
				if (exception != null)
					throw exception;

				return result;
			}

			throw new TimeoutException("Web request timed out");
		}

		public string SendWebRequest(string path, IDictionary<string, object> parameters, Method requestMethod = Method.POST)
		{
			try
			{
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
