using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Cirrious.MvvmCross.Plugins.Messenger;
using Newtonsoft.Json;
using RestSharp;
using StreamChat.Core.ApplicationObjects;
using StreamChat.Core.ServiceContracts;

namespace StreamChat.Core.ChatLoaders
{
	public class Sc2TvChatLoader : IChatLoadingService
	{
		private readonly ICommunicationService _communicationService;
		private readonly IChatContainer _chatContainer;

		public Sc2TvChatLoader(ICommunicationService communicationService, IChatContainer chatContainer)
		{
			_communicationService = communicationService;
			_chatContainer = chatContainer;
		}

		private string _streamerId = string.Empty;
		private string _streamerNick = string.Empty;

		public IEnumerable<IMessage> GetMessages(IChat chat)
		{
			if (string.IsNullOrWhiteSpace(_streamerId))
			{
				GetStreamerId(chat.ChatUri);
			}

			string chatUrl = string.Format("http://chat.sc2tv.ru/memfs/channel-{0}.json", _streamerId);

			string chatPage = _communicationService.SendWebRequest(chatUrl, null, Method.GET);
			if (chatPage != null)
			{
				var result = JsonConvert.DeserializeObject<Sc2TvMessageContainer>(chatPage);
				if (result != null)
				{
					return result.Messages;
				}
			}

			return null;
		}

		private void GetStreamerId(string chatUri)
		{
			string page = _communicationService.SendWebRequest(chatUri, null, Method.GET);
			if (page != null)
			{
				Regex rx = new Regex("\\<link.*?\\\"canonical\\\".*?href=\\\"http://sc2tv.ru/node/(.*?)\\\"");
				Match m = rx.Match(page);
				if (m.Success)
				{
					_streamerId = m.Groups[1].Value;

					//rx = new Regex(".*?author\\\".*?title.*?\\>(.*?)\\<");
					//m = rx.Match(page);

					//_streamerNick = m.Groups[1].Value;
					var targetChat = _chatContainer.GetChats().FirstOrDefault(c => c.ChatUri == chatUri);

					//if (targetChat != null)
					//	targetChat.StreamerNick = _streamerNick;
				}
			}
		}
	}
}
