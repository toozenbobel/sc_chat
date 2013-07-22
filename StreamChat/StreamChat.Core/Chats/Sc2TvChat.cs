using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Cirrious.CrossCore;
using StreamChat.Core.Annotations;
using StreamChat.Core.ChatLoaders;
using StreamChat.Core.ServiceContracts;

namespace StreamChat.Core.Chats
{
	public class Sc2TvChat : IChat, INotifyPropertyChanged
	{
		public long SourceId { get; set; }

		public string ServiceName
		{
			get
			{
				return "SC2TV";
			}
		}

		public string ChatUri { get; set; }

		private readonly IChatLoadingService _chatLoader = (IChatLoadingService) Mvx.IocConstruct(typeof(Sc2TvChatLoader));
		public IChatLoadingService ChatLoader
		{
			get
			{
				return _chatLoader;
			}
		}

		private string _streamerNick;
		private string _streamerId;

		public string StreamerNick
		{
			get
			{
				return _streamerNick;
			}
			set
			{
				_streamerNick = value;
				OnPropertyChanged("StreamerNick");
			}
		}

		public string StreamerId
		{
			get
			{
				return _streamerId;
			}
			set
			{
				_streamerId = value;
				OnPropertyChanged("StreamerId");
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
				handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
