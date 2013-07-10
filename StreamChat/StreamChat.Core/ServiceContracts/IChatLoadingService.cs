using System.Collections.Generic;

namespace StreamChat.Core.ServiceContracts
{
	/// <summary>
	/// Загрузчик сообщений чата
	/// </summary>
	public interface IChatLoadingService
	{
		IEnumerable<IMessage> GetMessages();
	}
}