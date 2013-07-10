using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cirrious.CrossCore;
using StreamChat.Core.ServiceContracts;

namespace StreamChat.Core.ChatLoaders
{
	public interface IChatContainer
	{
		void AddChat(long sourceId, string chatUri);
		IEnumerable<IChat> GetChats();
	}

	public class ChatContainer : IChatContainer
	{
		private IChatResolveService _resolveService;

		public ChatContainer()
		{
			_resolveService = Mvx.Resolve<IChatResolveService>();
		}

		private readonly List<IChat> _chats = new List<IChat>(); 

		public void AddChat(long sourceId, string chatUri)
		{
			IChat newChat = _resolveService.Resolve(sourceId);
			newChat.ChatUri = chatUri;

			_chats.Add(newChat);
		}

		public IEnumerable<IChat> GetChats()
		{
			return _chats;
		}
	}
}
