using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Phone.Controls;
using StreamChat.WPhone.ControlsImprovement.Linq2VisualTree.LinqToVisualTree;

namespace StreamChat.WPhone.ControlsImprovement.MetroInMotion
{
	public static class MetroInMotion
	{
		#region AnimationLevel

		public static int GetAnimationLevel(DependencyObject obj)
		{
			return (int)obj.GetValue(AnimationLevelProperty);
		}

		public static void SetAnimationLevel(DependencyObject obj, int value)
		{
			obj.SetValue(AnimationLevelProperty, value);
		}


		public static readonly DependencyProperty AnimationLevelProperty =
			DependencyProperty.RegisterAttached("AnimationLevel", typeof(int),
			typeof(MetroInMotion), new PropertyMetadata(-1));

		#endregion

		#region Tilt

		public static double GetTilt(DependencyObject obj)
		{
			return (double)obj.GetValue(TiltProperty);
		}

		public static void SetTilt(DependencyObject obj, double value)
		{
			obj.SetValue(TiltProperty, value);
		}


		public static readonly DependencyProperty TiltProperty =
			DependencyProperty.RegisterAttached("Tilt", typeof(double),
			typeof(MetroInMotion), new PropertyMetadata(2.0, OnTiltChanged));

		/// <summary>
		/// The extent of the tilt action, the larger the number, the bigger the tilt
		/// </summary>
		private static double TiltAngleFactor = 4;

		/// <summary>
		/// The extent of the scaling action, the smaller the number, the greater the scaling.
		/// </summary>
		private static double ScaleFactor = 100;

		private static void OnTiltChanged(DependencyObject d,
		  DependencyPropertyChangedEventArgs args)
		{
			FrameworkElement targetElement = d as FrameworkElement;

			double tiltFactor = GetTilt(d);

			// create the required transformations
			var projection = new PlaneProjection();
			var scale = new ScaleTransform();
			var translate = new TranslateTransform();

			var transGroup = new TransformGroup();
			transGroup.Children.Add(scale);
			transGroup.Children.Add(translate);

			// associate with the target element
			targetElement.Projection = projection;
			targetElement.RenderTransform = transGroup;
			targetElement.RenderTransformOrigin = new Point(0.5, 0.5);

			targetElement.MouseLeftButtonDown += (s, e) =>
			{
				var clickPosition = e.GetPosition(targetElement);

				// find the maximum of width / height
				double maxDimension = Math.Max(targetElement.ActualWidth, targetElement.ActualHeight);

				// compute the normalised horizontal distance from the centre
				double distanceFromCenterX = targetElement.ActualWidth / 2 - clickPosition.X;
				double normalisedDistanceX = 2 * distanceFromCenterX / maxDimension;

				// rotate around the Y axis 
				projection.RotationY = normalisedDistanceX * TiltAngleFactor * tiltFactor;

				// compute the normalised vertical distance from the centre
				double distanceFromCenterY = targetElement.ActualHeight / 2 - clickPosition.Y;
				double normalisedDistanceY = 2 * distanceFromCenterY / maxDimension;

				// rotate around the X axis, 
				projection.RotationX = -normalisedDistanceY * TiltAngleFactor * tiltFactor;

				// find the distance to centre
				double distanceToCentre = Math.Sqrt(normalisedDistanceX * normalisedDistanceX
				  + normalisedDistanceY * normalisedDistanceY);

				// scale accordingly
				double scaleVal = tiltFactor * (1 - distanceToCentre) / ScaleFactor;
				scale.ScaleX = 1 - scaleVal;
				scale.ScaleY = 1 - scaleVal;

				// offset the plane transform
				var rootElement = Application.Current.RootVisual as FrameworkElement;
				var relativeToCentre = (targetElement.GetRelativePosition(rootElement).Y - rootElement.ActualHeight / 2) / 2;
				translate.Y = -relativeToCentre;
				projection.LocalOffsetY = +relativeToCentre;

			};

			targetElement.ManipulationCompleted += (s, e) =>
			{
				var sb = new Storyboard();
				sb.Children.Add(CreateAnimation(null, 0, 0.1, "RotationY", projection));
				sb.Children.Add(CreateAnimation(null, 0, 0.1, "RotationX", projection));
				sb.Children.Add(CreateAnimation(null, 1, 0.1, "ScaleX", scale));
				sb.Children.Add(CreateAnimation(null, 1, 0.1, "ScaleY", scale));
				sb.Begin();

				translate.Y = 0;
				projection.LocalOffsetY = 0;
			};

		}


		#endregion

		#region IsPivotAnimated

		public static bool GetIsPivotAnimated(DependencyObject obj)
		{
			return (bool)obj.GetValue(IsPivotAnimatedProperty);
		}

		public static void SetIsPivotAnimated(DependencyObject obj, bool value)
		{
			obj.SetValue(IsPivotAnimatedProperty, value);
		}

		public static readonly DependencyProperty IsPivotAnimatedProperty =
			DependencyProperty.RegisterAttached("IsPivotAnimated", typeof(bool),
			typeof(MetroInMotion), new PropertyMetadata(false, OnIsPivotAnimatedChanged));

		private static void OnIsPivotAnimatedChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
		{
			ItemsControl list = d as ItemsControl;

			list.Loaded += (s2, e2) =>
			{
				// locate the pivot control that this list is within
				var pivot = list.Ancestors<Pivot>().Single() as Pivot;

				// and its index within the pivot
				int pivotIndex = pivot.Items.IndexOf(list.Ancestors<PivotItem>().Single());

				bool selectionChanged = false;

				pivot.SelectionChanged += (s3, e3) =>
				{
					selectionChanged = true;
				};

				// handle manipulation events which occur when the user
				// moves between pivot items
				pivot.ManipulationCompleted += (s, e) =>
				{
					if (!selectionChanged)
						return;

					selectionChanged = false;

					if (pivotIndex != pivot.SelectedIndex)
						return;

					// determine which direction this tab will be scrolling in from
					bool fromRight = e.TotalManipulation.Translation.X <= 0;


					// iterate over each of the items in view
					var items = list.GetItemsInView().ToList();
					for (int index = 0; index < items.Count; index++)
					{
						var lbi = items[index];

						list.Dispatcher.BeginInvoke(() =>
						{
							var animationTargets = lbi.Descendants()
												   .Where(p => MetroInMotion.GetAnimationLevel(p) > -1);
							foreach (FrameworkElement target in animationTargets)
							{
								// trigger the required animation
								GetSlideAnimation(target, fromRight).Begin();
							}
						});
					};

				};
			};
		}


		#endregion

		/// <summary>
		/// Animates each element in order, creating a 'peel' effect. The supplied action
		/// is invoked when the animation ends.
		/// </summary>
		public static void Peel(this IEnumerable<FrameworkElement> elements, Action endAction)
		{
			var elementList = elements.ToList();
			var lastElement = elementList.Last();

			// iterate over all the elements, animating each of them
			double delay = 0;
			foreach (FrameworkElement element in elementList)
			{
				var sb = GetPeelAnimation(element, delay);

				// add a Completed event handler to the last element
				if (element.Equals(lastElement))
				{
					sb.Completed += (s, e) =>
					{
						endAction();
					};
				}

				sb.Begin();
				delay += 50;
			}
		}


		/// <summary>
		/// Enumerates all the items that are currently visible in am ItemsControl. This implementation assumes
		/// that a VirtualizingStackPanel is being used as the ItemsPanel.
		/// </summary>
		public static IEnumerable<FrameworkElement> GetItemsInView(this ItemsControl itemsControl)
		{
			// locate the stack panel that hosts the items
			VirtualizingStackPanel vsp = itemsControl.Descendants<VirtualizingStackPanel>().First() as VirtualizingStackPanel;

			// iterate over each of the items in view
			int firstVisibleItem = (int)vsp.VerticalOffset;
			int visibleItemCount = (int)vsp.ViewportHeight;
			for (int index = firstVisibleItem; index <= firstVisibleItem + visibleItemCount + 1; index++)
			{
				var item = itemsControl.ItemContainerGenerator.ContainerFromIndex(index) as FrameworkElement;
				if (item == null)
					continue;

				yield return item;
			}
		}

		/// <summary>
		/// Creates a PlaneProjection and associates it with the given element, returning
		/// a Storyboard which will animate the PlaneProjection to 'peel' the item
		/// from the screen.
		/// </summary>
		private static Storyboard GetPeelAnimation(FrameworkElement element, double delay)
		{
			Storyboard sb;

			var projection = new PlaneProjection()
			{
				CenterOfRotationX = -0.1
			};
			element.Projection = projection;

			// compute the angle of rotation required to make this element appear
			// at a 90 degree angle at the edge of the screen.
			var width = element.ActualWidth;
			var targetAngle = Math.Atan(1000 / (width / 2));
			targetAngle = targetAngle * 180 / Math.PI;

			// animate the projection
			sb = new Storyboard();
			sb.BeginTime = TimeSpan.FromMilliseconds(delay);
			sb.Children.Add(CreateAnimation(0, -(180 - targetAngle), 0.3, "RotationY", projection));
			sb.Children.Add(CreateAnimation(0, 23, 0.3, "RotationZ", projection));
			sb.Children.Add(CreateAnimation(0, -23, 0.3, "GlobalOffsetZ", projection));
			return sb;
		}

		private static DoubleAnimation CreateAnimation(double? from, double? to, double duration,
		  string targetProperty, DependencyObject target)
		{
			var db = new DoubleAnimation();
			db.To = to;
			db.From = from;
			db.EasingFunction = new SineEase();
			db.Duration = TimeSpan.FromSeconds(duration);
			Storyboard.SetTarget(db, target);
			Storyboard.SetTargetProperty(db, new PropertyPath(targetProperty));
			return db;
		}

		/// <summary>
		/// Creates a TranslateTransform and associates it with the given element, returning
		/// a Storyboard which will animate the TranslateTransform with a SineEase function
		/// </summary>
		private static Storyboard GetSlideAnimation(FrameworkElement element, bool fromRight)
		{
			double from = fromRight ? 80 : -80;

			Storyboard sb;
			double delay = (MetroInMotion.GetAnimationLevel(element)) * 0.1 + 0.1;

			TranslateTransform trans = new TranslateTransform() { X = from };
			element.RenderTransform = trans;

			sb = new Storyboard();
			sb.BeginTime = TimeSpan.FromSeconds(delay);
			sb.Children.Add(CreateAnimation(from, 0, 0.8, "X", trans));
			return sb;
		}

	}
}
