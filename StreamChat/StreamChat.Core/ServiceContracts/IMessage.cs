using System;
using Newtonsoft.Json;

namespace StreamChat.Core.ServiceContracts
{
	/// <summary>
	/// Абстракция чат сообщения
	/// </summary>
	public interface IMessage
	{
		string From { get; set; }
		string Text { get; set; }
		DateTime Timestamp { get; }

		bool Equals(IMessage message);
	}

	public class Sc2TvMessageContainer
	{
		public Sc2TvMessage[] Messages { get; set; }
	}

	public class Sc2TvMessage : IMessage
	{
		[JsonProperty("name")]
		public string From { get; set; }

		[JsonProperty("message")]
		public string Text { get; set; }

		[JsonProperty("date")]
		public DateTime Timestamp { get; set; }

		public bool Equals(IMessage message)
		{
			if (From == message.From &&
			    Timestamp == message.Timestamp)
				return true;
			
			return false;
		}
	}
}