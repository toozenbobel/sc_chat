using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.File;
using Cirrious.MvvmCross.Plugins.Messenger;
using Cirrious.MvvmCross.ViewModels;
using Newtonsoft.Json;
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
		private readonly IMvxFileStore _fileService;
		private readonly IScreenLockService _screenLockService;
		private MvxSubscriptionToken _token;

		public MainPageViewModel(IChatContainer chatContainer, IMvxMessenger messenger, IChatResolveService resolveService, IMvxFileStore fileService, IScreenLockService screenLockService)
		{
			this._chatContainer = chatContainer;
			
			_resolveService = resolveService;
			_fileService = fileService;
			_screenLockService = screenLockService;
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

			LoadAndApplySettings();
		}

		private void LoadAndApplySettings()
		{
			string json;
			if (_fileService.TryReadTextFile(SettingsPageViewModel.FILENAME, out json))
			{
				Settings settings = JsonConvert.DeserializeObject<Settings>(json);
				if (settings != null)
				{
					if (settings.PreventScreenLock)
					{
						_screenLockService.DisableScreenLock();
					}
					else
					{
						_screenLockService.EnableScreenLock();
					}
				}
			}
		}

		private void UpdateChatsList()
		{
			Chats = null;
			Chats = _chatContainer.GetChats().Select(ch =>
				{
					var newChat = Mvx.IocConstruct(typeof (ChatViewModel)) as ChatViewModel;
					if (newChat != null)
					{
						newChat.Data = ch;
						newChat.Init(ch.ChatUri);
						newChat.PropertyChanged += OnChatPropertyChanged;
					}
					return newChat;
				}).ToList();

			if (Chats.Any())
				Chats.First().IsActive = true;
		}

		private void OnChatPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "NewMessagesCount")
			{
				UpdateNewMessagesCount();
			}
		}

		private int _newMessagesCount;
		public int NewMessagesCount
		{
			get
			{
				return _newMessagesCount;
			}
			set
			{
				_newMessagesCount = value;
				OnPropertyChanged(() => NewMessagesCount);
			}
		}

		private void UpdateNewMessagesCount()
		{
			NewMessagesCount = Chats.Select(c => c.NewMessagesCount).Sum();
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

		private int _selectedChatIndex;
		public int SelectedChatIndex
		{
			get
			{
				return _selectedChatIndex;
			}
			set
			{
				_selectedChatIndex = value;
				SetActiveChat(_selectedChatIndex);
				OnPropertyChanged(() => SelectedChatIndex);
			}
		}

		private void SetActiveChat(int index)
		{
			var activeChat = Chats.SingleOrDefault(c => c.IsActive);
			if (activeChat != null)
				activeChat.IsActive = false;

			Chats[index].IsActive = true;
		}

		public ICommand AddChatCommand
	    {
		    get
		    {
			    return new MvxCommand(() => ShowViewModel<AddChatPageViewModel>());
		    }
	    }

		public ICommand GoSettingsCommand
		{
			get
			{
				return new MvxCommand(() => ShowViewModel<SettingsPageViewModel>());
			}
		}

		public ICommand RemoveActiveChatCommand
		{
			get {return new MvxCommand(RemoveActiveChat);}
		}

		private void RemoveActiveChat()
		{
			var activeChat = Chats.SingleOrDefault(c => c.IsActive);
			if (activeChat != null)
			{
				_chatContainer.RemoveChat(activeChat.Data);
				UpdateChatsList();
			}
		}
    }
}
