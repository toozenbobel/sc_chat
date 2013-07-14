using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Messenger;
using Cirrious.MvvmCross.ViewModels;
using StreamChat.Core.ChatLoaders;
using StreamChat.Core.MvxMessages;
using StreamChat.Core.ServiceContracts;

namespace StreamChat.Core.ViewModels
{
	public class MainPageViewModel 
		: ViewModelBase
    {
		private readonly IChatContainer _chatContainer;
		private readonly IChatLoadingService _chatLoadingService;
		private MvxSubscriptionToken _token;

		public MainPageViewModel(IChatContainer chatContainer, IMvxMessenger messenger, IChatLoadingService chatLoadingService)
		{
			this._chatContainer = chatContainer;
			_chatLoadingService = chatLoadingService;
			_token = messenger.Subscribe<ChatAddedMessage>(OnChatAdded);
		}

		private void OnChatAdded(ChatAddedMessage obj)
		{
			UpdateChatsList();	
		}

		public void Init()
		{
			UpdateChatsList();
		}

		public void UpdateChatsList()
		{
			Chats = _chatContainer.GetChats().Select(ch =>
				{
					var newChat = Mvx.IocConstruct(typeof (ChatViewModel)) as ChatViewModel;
					if (newChat != null)
					{
						newChat.Data = ch;
					}
					return newChat;
				}).ToList();
		}

		private List<ChatViewModel> _chats = new List<ChatViewModel>();
		public List<ChatViewModel> Chats
		{
			get
			{
				return _chats;
			}
			set
			{
				_chats = value;
				OnPropertyChanged(() => Chats);
			}
		}

		public ICommand AddChatCommand
	    {
		    get
		    {
			    return new MvxCommand(() => ShowViewModel<AddChatPageViewModel>());
		    }
	    }
    }
}
