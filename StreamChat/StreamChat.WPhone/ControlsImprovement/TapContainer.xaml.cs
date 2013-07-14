using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.Phone.Shell;
using GestureEventArgs = Microsoft.Phone.Controls.GestureEventArgs;

namespace StreamChat.WPhone.ControlsImprovement
{
	public partial class TapContainer
	{
		public TapContainer()
		{
			InitializeComponent();
		}


		public object TapParameter
		{
			get
			{
				return GetValue(TapParameterProperty);
			}
			set
			{
				SetValue(TapParameterProperty, value);
			}
		}

		public static readonly DependencyProperty TapParameterProperty = DependencyProperty.Register("TapParameter",
																									 typeof(object),
																									 typeof(TapContainer),
																									 new PropertyMetadata(null));
		public ICommand TapCommand
		{
			get
			{
				return (ICommand)GetValue(TapCommandProperty);
			}
			set
			{
				SetValue(TapCommandProperty, value);
			}
		}

		// Using a DependencyProperty as the backing store for TapCommand.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty TapCommandProperty =
			DependencyProperty.Register("TapCommand", typeof(ICommand), typeof(TapContainer), new PropertyMetadata(null));

		private void LayoutRootTap(object sender, System.Windows.Input.GestureEventArgs gestureEventArgs)
		{
			if (TapCommand != null)
				TapCommand.Execute(TapParameter);
		}
	}
}
