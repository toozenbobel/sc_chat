using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StreamChat.Core.ServiceContracts
{
	/// <summary>
	/// Сервис хранения и записи пользовательских настроек
	/// </summary>
	public interface ISettingsService
	{
		IEnumerable<string> LoadChats();
		void AddChat();
		void RemoveChat(long chatId);
	}
}
