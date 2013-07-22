using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Cirrious.CrossCore.Core;
using Cirrious.CrossCore.Exceptions;
using Cirrious.MvvmCross.Plugins.Messenger;
using Newtonsoft.Json;
using RestSharp;
using StreamChat.Core.ApplicationObjects;
using StreamChat.Core.ServiceContracts;

namespace StreamChat.Core.ChatLoaders
{
	public class Sc2TvChatLoader : MvxMainThreadDispatchingObject, IChatLoadingService
	{
		private readonly ICommunicationService _communicationService;
		private readonly IChatContainer _chatContainer;

		public Sc2TvChatLoader(ICommunicationService communicationService, IChatContainer chatContainer)
		{
			_communicationService = communicationService;
			_chatContainer = chatContainer;
		}

		public IEnumerable<IMessage> GetMessages(IChat chat)
		{
			string streamerId;

			if (string.IsNullOrWhiteSpace(chat.StreamerId))
			{
				streamerId = GetStreamerId(chat);
			}
			else
			{
				streamerId = chat.StreamerId;
			}

			if (!string.IsNullOrWhiteSpace(streamerId))
			{
				string chatUrl = string.Format("http://chat.sc2tv.ru/memfs/channel-{0}.json", streamerId);

				string chatPage = _communicationService.SendWebRequest(chatUrl, null, Method.GET);
				if (chatPage != null)
				{
					try
					{
						var result = JsonConvert.DeserializeObject<Sc2TvMessageContainer>(chatPage);
						if (result != null)
						{
							return result.Messages;
						}
					}
					catch (Exception e)
					{
						Debug.WriteLine("Failed to parse messages {0}", e.ToLongString());
						return null;
					}

				}
			}

			return null;
		}

		private string GetStreamerId(IChat chat)
		{
			string page = _communicationService.SendWebRequest(chat.ChatUri, null, Method.GET);
			if (page != null)
			{
				Regex rx = new Regex("\\<link.*?\\\"canonical\\\".*?href=\\\"http://sc2tv.ru/node/(.*?)\\\"");
				Match m = rx.Match(page);
				if (m.Success)
				{
					string streamerId = m.Groups[1].Value;
					
					InvokeOnMainThread(() => chat.StreamerId = streamerId);
					return streamerId;
				}
			}

			return null;
		}
	}
}
