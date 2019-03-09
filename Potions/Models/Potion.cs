using Functional.Maybe;
using System.Collections.Generic;

namespace Potions
{
	public class Potion
	{
		public int Level { get; set; }
		public string Name { get; set; }
		public Effect Effect { get; set; }
		public Duration Duration { get; set; }
		public Maybe<int> Price { get; set; }

		private List<Recipe> _recipes;
		public IReadOnlyCollection<Recipe> Recipes { get { return _recipes; } }

		public Potion()
		{
			Level = 1;
			Name = "Undefined";
			Effect = Effect.Empty;
			_recipes = new List<Recipe>();
			Price = Maybe<int>.Nothing;
		}

		public void Add(Recipe recipe)
		{
			_recipes.Add(recipe);
		}

		public void AddRange(IReadOnlyCollection<Recipe> recipes)
		{
			_recipes.AddRange(recipes);
		}
	}
}
