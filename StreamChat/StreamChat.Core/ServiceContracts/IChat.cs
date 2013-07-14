namespace StreamChat.Core.ServiceContracts
{
	/// <summary>
	/// Абстракция чата
	/// </summary>
	public interface IChat
	{
		long SourceId { get; set; }
		string ServiceName { get; }
		string ChatUri { get; set; }
		IChatLoadingService ChatLoader { get; }
		string StreamerNick { get; set; }
	}
}