using System.Collections.Generic;

namespace Potions
{
	public class Potion: PotionBase
	{
		private List<Recipe> _recipes;
		public IReadOnlyCollection<Recipe> Recipes { get { return _recipes; } }

		public Potion()
		{
			_recipes = new List<Recipe>();
		}

		public Potion Add(Recipe recipe)
		{
			_recipes.Add(recipe);
			return this;
		}

		public Potion AddRange(IReadOnlyCollection<Recipe> recipes)
		{
			_recipes.AddRange(recipes);
			return this;
		}
	}
}
