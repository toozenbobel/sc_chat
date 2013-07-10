using System;

namespace StreamChat.Core.ServiceContracts
{
	/// <summary>
	/// Абстракция чат сообщения
	/// </summary>
	public interface IMessage
	{
		string From { get; }
		string Text { get; }
		DateTime Timestamp { get; }
	}
}