using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cirrious.MvvmCross.Plugins.Messenger;

namespace StreamChat.Core.MvxMessages
{
	internal class ChatAddedMessage : MvxMessage
	{
		public ChatAddedMessage(object sender) : base(sender)
		{
		}
	}
}
