using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cirrious.CrossCore.Core;

namespace StreamChat.Core.ApplicationObjects
{
	public interface IErrorReporter
	{
		void Die(string error);
	}

	public class ErrorEventArgs : EventArgs
	{
		public string Message { get; private set; }

		public ErrorEventArgs(string message)
		{
			Message = message;
		}
	}

	public interface IErrorSource
	{
		event EventHandler<ErrorEventArgs> ErrorReported;
	}

	public class ErrorApplicationObject : MvxMainThreadDispatchingObject, IErrorReporter, IErrorSource
	{
		public void Die(string error)
		{
			if (ErrorReported == null)
				return;

			InvokeOnMainThread(() =>
			{
				var handler = ErrorReported;
				if (handler != null)
					handler(this, new ErrorEventArgs(error));
			});
		}

		public event EventHandler<ErrorEventArgs> ErrorReported;
	}
}
