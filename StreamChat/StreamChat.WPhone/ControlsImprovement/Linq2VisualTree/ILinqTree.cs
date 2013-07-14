using System.Collections.Generic;

namespace StreamChat.WPhone.ControlsImprovement.Linq2VisualTree.LinqToVisualTree
{
	/// <summary>
	/// Defines an interface that must be implemented to generate the LinqToTree methods
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface ILinqTree<T>
	{
		IEnumerable<T> Children();

		T Parent { get; }
	}
}