using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cirrious.CrossCore;
using StreamChat.Core.Chats;
using StreamChat.Core.ServiceContracts;

namespace StreamChat.Core.ChatLoaders
{
	public class ChatFactory : IChatResolveService
	{
		private readonly Dictionary<long, IChat> _chatMap = new Dictionary<long, IChat>(); 

		public void Register(IChat chat, long sourceId)
		{
			if (_chatMap.Keys.All((k => k != sourceId)))
			{
				_chatMap.Add(sourceId, chat);
			}
			else
			{
				_chatMap[sourceId] = chat;
			}
		}

		public IChat Resolve(long sourceId)
		{
			if (_chatMap.Keys.Any(k => k == sourceId))
				return _chatMap[sourceId];

			return null;
		}

		public IChat CreateNewInstance(long sourceId)
		{
			if (_chatMap.Keys.All((k => k != sourceId)))
			{
				return null;
			}

			Type chatType = _chatMap[sourceId].GetType();
			var newChat = (IChat) Mvx.IocConstruct(chatType);
			newChat.SourceId = sourceId;
			return newChat;
		}

		public IEnumerable<IChat> GetAllChats()
		{
			return _chatMap.Values;
		}
	}
}
