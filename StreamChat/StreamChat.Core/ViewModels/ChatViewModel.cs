using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using StreamChat.Core.ChatLoaders;
using StreamChat.Core.ServiceContracts;

namespace StreamChat.Core.ViewModels
{
	public class ChatViewModel : ViewModelBase
	{
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

		private void LoadMessages()
		{
			StartAsyncTask(() => _loadingService.GetMessages(),
			               messages =>
				               {

				               });
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
