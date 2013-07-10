using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using StreamChat.Core.ChatLoaders;
using StreamChat.Core.ServiceContracts;

namespace StreamChat.Core.ViewModels
{
	public class AddChatPageViewModel : ViewModelBase
	{
		private readonly IChatResolveService _chatResolveService;
		private readonly IChatContainer _chatContainer;

		public AddChatPageViewModel(IChatResolveService chatResolveService, IChatContainer chatContainer)
		{
			_chatResolveService = chatResolveService;
			_chatContainer = chatContainer;
		}

		public void Init()
		{
			ChatSources = _chatResolveService.GetAllChats().Select(ch => new ChatSource()
				{
					DisplayName = ch.ServiceName,
					SourceId = ch.SourceId
				}).ToList();

			SelectedChatSource = ChatSources.FirstOrDefault();
		}

		private List<ChatSource> _chatSource = new List<ChatSource>();
		public List<ChatSource>  ChatSources
		{
			get
			{
				return _chatSource;
			}
			set
			{
				_chatSource = value;
				OnPropertyChanged(() => ChatSources);
			} 
		}

		private ChatSource _selectedChatSource;
		public ChatSource SelectedChatSource
		{
			get
			{
				return _selectedChatSource;
			}
			set
			{
				_selectedChatSource = value;
				OnPropertyChanged(() => SelectedChatSource);
			}
		}

#if DEBUG
		private string _chatUri = "http://sc2tv.ru/content/toozenbobel";
#else
		private string _chatUri;
#endif
		public string ChatUri
		{
			get
			{
				return _chatUri;
			}
			set
			{
				_chatUri = value;
				OnPropertyChanged(() => ChatUri);
			}
		}

		public ICommand ConfirmCommand
		{
			get
			{
				return new MvxCommand(AddChat);
			}
		}

		private void AddChat()
		{
			if (SelectedChatSource != null && !string.IsNullOrWhiteSpace(SelectedChatSource.DisplayName))
			{
				_chatContainer.AddChat(SelectedChatSource.SourceId, ChatUri);
				Close(this);
			}
		}
	}

	public class ChatSource
	{
		public string DisplayName { get; set; }
		public long SourceId { get; set; }
	}
}