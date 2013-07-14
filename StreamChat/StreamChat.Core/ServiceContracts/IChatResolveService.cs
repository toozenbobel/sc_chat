using System.Collections.Generic;

namespace StreamChat.Core.ServiceContracts
{
	/// <summary>
	/// Контейнер для чатов
	/// </summary>
	public interface IChatResolveService
	{
		IEnumerable<IChat> GetAllChats();
		void Register(IChat chat, long sourceId);
		IChat Resolve(long sourceId);
		IChat CreateNewInstance(long sourceId);
	}
}