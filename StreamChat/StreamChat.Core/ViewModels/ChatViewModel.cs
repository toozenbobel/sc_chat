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
		private readonly IChatLoadingService _loadingService;
		private readonly IChatContainer _chatContainer;

		public ChatViewModel(IChatLoadingService loadingService, IChatContainer chatContainer)
		{
			_loadingService = loadingService;
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

		public void Init(string chatUri)
		{
			if (Data == null)
			{
				Data = _chatContainer.GetChats().FirstOrDefault(c => c.ChatUri == chatUri);
			}

			LoadMessages();
		}

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
			StartAsyncTask(() => _loadingService.GetMessages(Data),
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
			StartAsyncTask(() => _loadingService.GetMessages(Data),
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

		#region Auto update

		private Timer _timer;
		private const double UPDATE_INTERVAL = 2.0;

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
							ShowViewModel<ChatViewModel>(
								new
									{
										chatUri = Data.ChatUri
									});
						}
					});
			}
		}
	}
}
