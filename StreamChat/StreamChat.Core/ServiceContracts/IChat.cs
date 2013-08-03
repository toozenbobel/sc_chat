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
		IAuthenticationService Authenticator { get; }
		IMessagePostingService Poster { get; }
		string StreamerNick { get; set; }
		string StreamerId { get; set; }
	}
}