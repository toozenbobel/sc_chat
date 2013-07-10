using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using StreamChat.Core.ChatLoaders;
using StreamChat.Core.ServiceContracts;

namespace StreamChat.Core.ViewModels
{
	public class MainPageViewModel 
		: ViewModelBase
    {
		private readonly IChatContainer _chatContainer;

		public MainPageViewModel(IChatContainer chatContainer)
		{
			this._chatContainer = chatContainer;
		}

		//public void Init()
		//{
			
		//}

		public override void Start()
		{
			Chats = _chatContainer.GetChats().ToList();
		}

		private List<IChat> _chats = new List<IChat>();
		public List<IChat> Chats
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
