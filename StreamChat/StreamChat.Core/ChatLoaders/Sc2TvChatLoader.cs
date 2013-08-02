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

		readonly Dictionary<string, string> _smilesUri = new Dictionary<string, string>(); 

		public Sc2TvChatLoader(ICommunicationService communicationService, IChatContainer chatContainer)
		{
			_communicationService = communicationService;
			_chatContainer = chatContainer;
		}

		public IEnumerable<IMessage> GetMessages(IChat chat)
		{
			if (!_smilesUri.Any())
			{
				UpdateSmiles();
			}

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
							return ProcessSmiles(result.Messages);
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

		private IEnumerable<IMessage> ProcessSmiles(IEnumerable<Sc2TvMessage> messages)
		{
			foreach (var sc2TvMessage in messages)
			{
				yield return ProcessSmiles(sc2TvMessage);
			}
		}

		Regex _extractSmile = new Regex("\\:s\\:\\w\\w.*?\\:");

		private IMessage ProcessSmiles(IMessage sc2TvMessage)
		{
			var text = sc2TvMessage.Text;
			text = _extractSmile.Replace(text, match => string.Format("<img src='{0}'></img>", _smilesUri[match.Value]));
			sc2TvMessage.Text = text;
			return sc2TvMessage;
		}

		private void UpdateSmiles()
		{
			string js = _communicationService.SendWebRequest("http://chat.sc2tv.ru/js/smiles.js", null, Method.GET);
			if (js != null)
			{
				_smilesUri.Clear();
				Regex smiles = new Regex("\\'(.*?)\\'.*?\\'(.*?)\\',.*?\\}", RegexOptions.Multiline);

				foreach (Match m in smiles.Matches(js))
					_smilesUri[":s" + m.Groups[1].Value] = "http://chat.sc2tv.ru/img/" + m.Groups[2].Value;
			}
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
