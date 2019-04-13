using System.Collections.Generic;

namespace Potions
{
	public class Chain: PotionBase
	{
		public Recipe Recipe { get; private set; }

		public Chain(Recipe recipe)
		{
			Recipe = recipe;
		}

		public Chain(IReadOnlyCollection<Item> items)
		{
			Recipe = new Recipe(items);
		}

	}
}
