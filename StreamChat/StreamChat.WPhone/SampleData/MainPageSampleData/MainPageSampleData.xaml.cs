﻿//      *********    DO NOT MODIFY THIS FILE     *********
//      This file is regenerated by a design tool. Making
//      changes to this file can cause errors.
namespace Expression.Blend.SampleData.MainPageSampleData
{
	using System; 

// To significantly reduce the sample data footprint in your production application, you can set
// the DISABLE_SAMPLE_DATA conditional compilation constant and disable sample data at runtime.
#if DISABLE_SAMPLE_DATA
	internal class MainPageSampleData { }
#else

	public class MainPageSampleData : System.ComponentModel.INotifyPropertyChanged
	{
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}

		public MainPageSampleData()
		{
			try
			{
				System.Uri resourceUri = new System.Uri("/StreamChat.WPhone;component/SampleData/MainPageSampleData/MainPageSampleData.xaml", System.UriKind.Relative);
				if (System.Windows.Application.GetResourceStream(resourceUri) != null)
				{
					System.Windows.Application.LoadComponent(this, resourceUri);
				}
			}
			catch (System.Exception)
			{
			}
		}

		private Chats _Chats = new Chats();

		public Chats Chats
		{
			get
			{
				return this._Chats;
			}
		}

		private double _NewMessagesCount = 0;

		public double NewMessagesCount
		{
			get
			{
				return this._NewMessagesCount;
			}

			set
			{
				if (this._NewMessagesCount != value)
				{
					this._NewMessagesCount = value;
					this.OnPropertyChanged("NewMessagesCount");
				}
			}
		}
	}

	public class ChatsItem : System.ComponentModel.INotifyPropertyChanged
	{
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}

		private Data _Data = new Data();

		public Data Data
		{
			get
			{
				return this._Data;
			}

			set
			{
				if (this._Data != value)
				{
					this._Data = value;
					this.OnPropertyChanged("Data");
				}
			}
		}

		private double _NewMessagesCount = 0;

		public double NewMessagesCount
		{
			get
			{
				return this._NewMessagesCount;
			}

			set
			{
				if (this._NewMessagesCount != value)
				{
					this._NewMessagesCount = value;
					this.OnPropertyChanged("NewMessagesCount");
				}
			}
		}
	}

	public class Chats : System.Collections.ObjectModel.ObservableCollection<ChatsItem>
	{ 
	}

	public class Data : System.ComponentModel.INotifyPropertyChanged
	{
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}

		private string _ServiceName = string.Empty;

		public string ServiceName
		{
			get
			{
				return this._ServiceName;
			}

			set
			{
				if (this._ServiceName != value)
				{
					this._ServiceName = value;
					this.OnPropertyChanged("ServiceName");
				}
			}
		}
	}
#endif
}
