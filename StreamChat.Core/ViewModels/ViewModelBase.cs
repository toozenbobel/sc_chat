using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.ViewModels;
using StreamChat.Core.ApplicationObjects;

namespace StreamChat.Core.ViewModels
{

	public class ViewModelBase : MvxViewModel
	{
		public void StartAsyncTask<T>(Func<T> task, Action<T> callback)
		{
			ThreadPool.QueueUserWorkItem(_ =>
				{
					var result = task();

					InvokeOnMainThread(() => callback(result));
				});
		}

		#region MVVM

		public void OnPropertyChanged(string propertyName)
		{
			RaisePropertyChanged(propertyName);
		}

		public void OnPropertyChanged<T>(Expression<Func<T>> property)
		{
			RaisePropertyChanged(property);
		}


		protected void Die(string error)
		{
			Mvx.Resolve<IErrorReporter>().Die(error);
		}

		#endregion
	}
}
