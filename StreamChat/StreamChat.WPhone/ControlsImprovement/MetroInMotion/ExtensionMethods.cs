using System.Windows;

namespace StreamChat.WPhone.ControlsImprovement.MetroInMotion
{
	public static class ExtensionMethods
	{
		public static Point GetRelativePosition(this UIElement element, UIElement other)
		{
			return element.TransformToVisual(other)
			              .Transform(new Point(0, 0));
		}
	}
}