using System.Collections.Generic;

namespace Potions
{
	/// <summary>
	/// Рецепт зелья.
	/// </summary>
	public class Recipe
	{
		public IReadOnlyCollection<Item> Items { get; }
		public Recipe(IReadOnlyCollection<Item> items)
		{
			Items = items;
		}
	}
}
