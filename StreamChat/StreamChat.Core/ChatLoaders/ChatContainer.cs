using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Messenger;
using StreamChat.Core.MvxMessages;
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
		private readonly IChatResolveService _resolveService;
		private readonly IMvxMessenger _messenger;

		public ChatContainer(IChatResolveService resolveService, IMvxMessenger messenger)
		{
			_resolveService = resolveService;
			_messenger = messenger;
		}

		private readonly List<IChat> _chats = new List<IChat>(); 

		public void AddChat(long sourceId, string chatUri)
		{
			IChat newChat = _resolveService.CreateNewInstance(sourceId);
			newChat.ChatUri = chatUri;
			_chats.Add(newChat);

			_messenger.Publish(new ChatAddedMessage(this));
		}

		public IEnumerable<IChat> GetChats()
		{
			return _chats;
		}
	}
}
