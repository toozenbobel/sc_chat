using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using StreamChat.Core.ChatLoaders;
using StreamChat.Core.ServiceContracts;

namespace StreamChat.Core.ViewModels
{
	public class ChatViewModel : ViewModelBase
	{
		private readonly object _locker = new object();
		private readonly IChatContainer _chatContainer;

		public ChatViewModel(IChatContainer chatContainer)
		{
			_chatContainer = chatContainer;
		}

		private IChat _data;
		public IChat Data
		{
			get
			{
				return _data;
			}
			set
			{
				_data = value;
				OnPropertyChanged("Data");
			}
		}

		private bool _isActive;
		public bool IsActive
		{
			get
			{
				return _isActive;
			}
			set
			{
#pragma warning disable 665
				if (_isActive = value)
#pragma warning restore 665
				{
					NewMessagesCount = 0;
				}
				OnPropertyChanged(() => IsActive);
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

		public void Init(string chatUri)
		{
			if (Data == null)
			{
				Data = _chatContainer.GetChats().FirstOrDefault(c => c.ChatUri == chatUri);
			}

			LoadMessages();
		}

		#region Authenticating

		private string _login;
		public string Login
		{
			get
			{
				return _login;
			}
			set
			{
				_login = value;
				OnPropertyChanged(() => Login);
			}
		}

		private string _password;
		public string Password
		{
			get
			{
				return _password;
			}
			set
			{
				_password = value;
				OnPropertyChanged(() => Password);
			}
		}

		public ICommand LoginCommand
		{
			get { return new MvxCommand(DoLogin); }
		}

		private void DoLogin()
		{
			if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password))
				return;

			StartAsyncTask(() => Data.Authenticator.DoAuth(Login, Password),
			               result =>
				               {

				               });
		}

		#endregion

		#region Message loading

		private bool _isLoadingMessages;
		public bool IsLoadingMessages
		{
			get
			{
				return _isLoadingMessages;
			}
			set
			{
				_isLoadingMessages = value;
				OnPropertyChanged("IsLoadingMessages");
			}
		}

		private void LoadMessages()
		{
			IsLoadingMessages = true;
			StartAsyncTask(() => Data.ChatLoader.GetMessages(Data),
			               messages =>
				               {
					               IsLoadingMessages = false;

					               if (messages != null)
					               {
						               Messages.Clear();

						               foreach (var message in messages)
						               {
							               Messages.Add(message);
						               }
					               }

					               ScheduleUpdate();
				               });
		}

		private void UpdateMessages()
		{
			StartAsyncTask(() => Data.ChatLoader.GetMessages(Data),
				messages =>
					{
						if (messages != null)
						{
							var loadedMessages = messages.ToList();
							int iMessageToAdd;
							for (iMessageToAdd = 0; iMessageToAdd < loadedMessages.Count(); iMessageToAdd++)
							{
								if (Messages.Any(m => m.Equals(loadedMessages[iMessageToAdd])))
								{
									iMessageToAdd--;
									break;
								}
							}

							if (!IsActive && iMessageToAdd >= 0)
								NewMessagesCount += iMessageToAdd + 1;

							for(;iMessageToAdd >= 0; iMessageToAdd--)
							{
								Messages.Insert(0, loadedMessages[iMessageToAdd]);
							}

							if (Messages.Count > 50)
							{
								for (int iMessageToDelete = 51; iMessageToDelete < Messages.Count; iMessageToDelete++)
									Messages.RemoveAt(iMessageToDelete);
							}
						}

						ScheduleUpdate();
					});
		}

		#endregion

		#region Auto update

		private Timer _timer;
#if DEBUG
		private const double UPDATE_INTERVAL = 2.0;
#else
		private const double UPDATE_INTERVAL = 7.0;
#endif

		private void ScheduleUpdate()
		{
			lock (_locker)
			{
				if (_timer == null)
				{
					_timer = new Timer(OnTimerTick, null, TimeSpan.FromSeconds(UPDATE_INTERVAL), TimeSpan.Zero);
				}
				else
				{
					_timer.Change(TimeSpan.FromSeconds(1.0), TimeSpan.Zero);
				}
			}
		}

		private void OnTimerTick(object state)
		{
			lock (_locker)
			{
				_timer.Dispose();
				_timer = null;
			}

			UpdateMessages();
		}

		#endregion

		private readonly ObservableCollection<IMessage> _messages = new ObservableCollection<IMessage>();
		public ObservableCollection<IMessage> Messages
		{
			get
			{
				return _messages;
			}
		}

		public ICommand TapCommand
		{
			get
			{
				return new MvxCommand(() =>
					{
						if (Data != null && !string.IsNullOrWhiteSpace(Data.ChatUri))
						{
							ShowViewModel<ChatViewModel>(new { chatUri = Data.ChatUri });
						}
					});
			}
		}
	}
}
