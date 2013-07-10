using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StreamChat.Core.ServiceContracts
{
	public interface ISettingsService
	{
		IEnumerable<string> LoadChats();
		void AddChatUri(string uri, long sourceId);
		void RemoveChatUri(string uri);
	}
	
	public interface IChatResolveService
	{
		void RegisterChat(IChat chat, long sourceId);
		IChat ResolveUri(long sourceId);
	}

	public interface IChat
	{
		IChatLoadingService ChatLoader { get; }
	}

	public interface IChatLoadingService
	{
		IEnumerable<IMessage> GetMessages();
	}

	public interface IMessage
	{
		string From { get; }
		string Text { get; }
		DateTime Timestamp { get; }
	}
}
