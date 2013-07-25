using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Messenger;
using Cirrious.MvvmCross.ViewModels;
using StreamChat.Core.ChatLoaders;
using StreamChat.Core.Chats;
using StreamChat.Core.MvxMessages;
using StreamChat.Core.ServiceContracts;

namespace StreamChat.Core.ViewModels
{
	public class MainPageViewModel 
		: ViewModelBase
    {
		private readonly IChatContainer _chatContainer;
		private readonly IChatResolveService _resolveService;
		private MvxSubscriptionToken _token;

		public MainPageViewModel(IChatContainer chatContainer, IMvxMessenger messenger, IChatResolveService resolveService)
		{
			this._chatContainer = chatContainer;
			
			_resolveService = resolveService;
			_token = messenger.Subscribe<ChatAddedMessage>(OnChatAdded);
		}

		private void OnChatAdded(ChatAddedMessage obj)
		{
			UpdateChatsList();	
		}

		public void Init()
		{
			_resolveService.Register(new Sc2TvChat() { SourceId = 0 }, 0);
			UpdateChatsList();
		}

		private void UpdateChatsList()
		{
			Chats = _chatContainer.GetChats().Select(ch =>
				{
					var newChat = Mvx.IocConstruct(typeof (ChatViewModel)) as ChatViewModel;
					if (newChat != null)
					{
						newChat.Data = ch;
						newChat.Init(ch.ChatUri);
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
