namespace StreamChat.Core.ServiceContracts
{
	/// <summary>
	/// Контейнер для чатов
	/// </summary>
	public interface IChatResolveService
	{
		void Register(IChat chat, long sourceId);
		IChat Resolve(long sourceId);
	}
}