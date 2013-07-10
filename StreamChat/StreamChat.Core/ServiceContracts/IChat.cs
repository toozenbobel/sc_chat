namespace StreamChat.Core.ServiceContracts
{
	/// <summary>
	/// Абстракция чата
	/// </summary>
	public interface IChat
	{
		string ChatUri { get; set; }
		IChatLoadingService ChatLoader { get; }
	}
}