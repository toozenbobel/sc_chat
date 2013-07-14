using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StreamChat.WPhone.ControlsImprovement.MetroInMotion
{
	/// <summary>
	/// Animates an element so that it flies out and flies in!
	/// </summary>
	public class ItemFlyInAndOutAnimations
	{
		private Popup _popup;

		private Canvas _popupCanvas;

		private FrameworkElement _targetElement;

		private Point _targetElementPosition;

		private Image _targetElementClone;

		private Rectangle _backgroundMask;

		private static TimeSpan _flyInSpeed = TimeSpan.FromMilliseconds(200);

		private static TimeSpan _flyOutSpeed = TimeSpan.FromMilliseconds(300);

		public ItemFlyInAndOutAnimations()
		{
			// construct a popup, with a Canvas as its child
			_popup = new Popup();
			_popupCanvas = new Canvas();
			_popup.Child = _popupCanvas;
		}

		public static void TitleFlyIn(FrameworkElement title)
		{
			TranslateTransform trans = new TranslateTransform();
			trans.X = 300;
			trans.Y = -50;
			title.RenderTransform = trans;

			var sb = new Storyboard();

			// animate the X position
			var db = CreateDoubleAnimation(300, 0,
			                               new SineEase(), trans, TranslateTransform.XProperty, _flyInSpeed);
			sb.Children.Add(db);

			// animate the Y position
			db = CreateDoubleAnimation(-100, 0,
			                           new SineEase(), trans, TranslateTransform.YProperty, _flyInSpeed);
			sb.Children.Add(db);

			sb.Begin();
		}

		/// <summary>
		/// Animate the previously 'flown-out' element back to its original location.
		/// </summary>
		public void ItemFlyIn()
		{
			if (_popupCanvas.Children.Count != 2)
				return;

			_popup.IsOpen = true;
			_backgroundMask.Opacity = 0.0;

			Image animatedImage = _popupCanvas.Children[1] as Image;

			var sb = new Storyboard();

			// animate the X position
			var db = CreateDoubleAnimation(_targetElementPosition.X - 100, _targetElementPosition.X,
			                               new SineEase(),
			                               _targetElementClone, Canvas.LeftProperty, _flyInSpeed);
			sb.Children.Add(db);

			// animate the Y position
			db = CreateDoubleAnimation(_targetElementPosition.Y - 50, _targetElementPosition.Y,
			                           new SineEase(),
			                           _targetElementClone, Canvas.TopProperty, _flyInSpeed);
			sb.Children.Add(db);

			sb.Completed += (s, e) =>
				{
					// when the animation has finished, hide the popup once more
					_popup.IsOpen = false;

					// restore the element we have animated
					_targetElement.Opacity = 1.0;

					// and get rid of our clone
					_popupCanvas.Children.Clear();
				};

			sb.Begin();
		}


		/// <summary>
		/// Animate the given element so that it flies off screen, fading 
		/// everything else that is on screen.
		/// </summary>
		public void ItemFlyOut(FrameworkElement element, Action action)
		{
			_targetElement = element;
			var rootElement = Application.Current.RootVisual as FrameworkElement;

			_backgroundMask = new Rectangle()
				{
					Fill = new SolidColorBrush(Colors.Black),
					Opacity = 0.0,
					Width = rootElement.ActualWidth,
					Height = rootElement.ActualHeight
				};
			_popupCanvas.Children.Add(_backgroundMask);

			_targetElementClone = new Image()
				{
					Source = new WriteableBitmap(element, null)
				};
			_popupCanvas.Children.Add(_targetElementClone);

			_targetElementPosition = element.GetRelativePosition(rootElement);
			Canvas.SetTop(_targetElementClone, _targetElementPosition.Y);
			Canvas.SetLeft(_targetElementClone, _targetElementPosition.X);

			var sb = new Storyboard();

			// animate the X position
			var db = CreateDoubleAnimation(_targetElementPosition.X, _targetElementPosition.X + 500,
			                               new SineEase() { EasingMode = EasingMode.EaseIn },
			                               _targetElementClone, Canvas.LeftProperty, _flyOutSpeed);
			sb.Children.Add(db);

			// animate the Y position
			db = CreateDoubleAnimation(_targetElementPosition.Y, _targetElementPosition.Y + 50,
			                           new SineEase() { EasingMode = EasingMode.EaseOut },
			                           _targetElementClone, Canvas.TopProperty, _flyOutSpeed);
			sb.Children.Add(db);

			// fade out the other elements
			db = CreateDoubleAnimation(0, 1,
			                           null, _backgroundMask, UIElement.OpacityProperty, _flyOutSpeed);
			sb.Children.Add(db);

			sb.Completed += (s, e2) =>
				{
					action();

					// hide the popup, by placing a task on the dispatcher queue, this
					// should be executed after the navigation has occurred
					element.Dispatcher.BeginInvoke(() =>
						{
							_popup.IsOpen = false;
						});
				};

			// hide the element we have 'cloned' into the popup
			element.Opacity = 0.0;

			// open the popup
			_popup.IsOpen = true;

			// begin the animation
			sb.Begin();
		}

		public static DoubleAnimation CreateDoubleAnimation(double from, double to, IEasingFunction easing,
		                                                    DependencyObject target, object propertyPath, TimeSpan duration)
		{
			var db = new DoubleAnimation();
			db.To = to;
			db.From = from;
			db.EasingFunction = easing;
			db.Duration = duration;
			Storyboard.SetTarget(db, target);
			Storyboard.SetTargetProperty(db, new PropertyPath(propertyPath));
			return db;
		}
	}
}