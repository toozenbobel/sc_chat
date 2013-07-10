using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StreamChat.Core.ChatLoaders;
using StreamChat.Core.ServiceContracts;

namespace StreamChat.Core.Chats
{
	public class Sc2TvChat : IChat
	{
		public string ChatUri { get; set; }

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
