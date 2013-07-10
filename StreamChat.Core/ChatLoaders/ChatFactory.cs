using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cirrious.CrossCore;
using StreamChat.Core.ServiceContracts;

namespace StreamChat.Core.ChatLoaders
{
	public class ChatFactory : IChatResolveService
	{
		public ChatFactory()
		{
			RegisterChat(new Sc2TvChat(), 0);
		}

		private readonly Dictionary<long, IChat> _chatMap = new Dictionary<long, IChat>(); 

		public void RegisterChat(IChat chat, long sourceId)
		{
			if (_chatMap.Keys.All(k => k != sourceId))
				_chatMap.Add(sourceId, chat);
			else
				_chatMap[sourceId] = chat;
		}

		public IChat ResolveUri(long sourceId)
		{
			if (_chatMap.Keys.Any(k => k != sourceId))
				return _chatMap[sourceId];

			return null;
		}
	}

	public class Sc2TvChat : IChat
	{
		private readonly IChatLoadingService _chatLoader = new Sc2TvChatLoader();

		public IChatLoadingService ChatLoader
		{
			get
			{
				return _chatLoader;
			}
		}
	}
}
