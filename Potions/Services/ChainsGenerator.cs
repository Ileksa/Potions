using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Potions
{
	public class ChainsGenerator
	{
		private IItemPool _itemPool;

		public ChainsGenerator(IItemPool itemPool)
		{
			_itemPool = itemPool;
		}

		//Схема для всех уровней: И1 И1 А И2 И2 А И3 И3 А И4 И4 А И5 И5 А Сезонный/Редкий
		public IReadOnlyCollection<Chain> TenUsualOneSeasonalOrRarePatternOne(IReadOnlyCollection<Potion> alreadyExists)
		{
			var perms = Permutations(5);
			var chains = new List<Chain>(perms.Count * 5 * 3);
			for (int level = 1; level <= 3; level++)
			{
				var all = _itemPool.All(level, includeActions: true);
				var items = all.Where(i => !i.IsAction).ToList(); //здесь надо требовать, чтобы их было 5
				var action = all.Where(i => i.IsAction).First();
				var seasonalAndRare = _itemPool.All(level, Rarity.Seasonal)
					.Concat(_itemPool.All(level, Rarity.Rare));
				foreach(var perm in perms)
				{
					//создание одного рецепта по заданной схеме
					var recipeItems = new List<Item>(11 + 5);
					foreach(var p in perm)
					{
						recipeItems.Add(items[p]);
						recipeItems.Add(items[p]);
						recipeItems.Add(action);
					}

					var withoutRareRecipe = new Recipe(new List<Item>(recipeItems));
					var withoutRareChain = new Chain(withoutRareRecipe) { Level = level };
					chains.Add(withoutRareChain);

					foreach(var rareItem in seasonalAndRare)
					{
						var recipe = new Recipe(recipeItems.Append(rareItem).ToList());
						var chain = new Chain(recipe) { Level = level };
						chains.Add(chain);
					}
				}

			}
			return ApplyExistentPotions(chains, alreadyExists);
		}

		private IReadOnlyCollection<Chain> ApplyExistentPotions(
			IReadOnlyCollection<Chain> chains, IReadOnlyCollection<Potion> alreadyExist)
		{
			foreach(var chain in chains)
			{
				var found = false;
				foreach(var potion in alreadyExist)
				{
					foreach(var recipe in potion.Recipes)
					{
						if (chain.Recipe == recipe)
						{
							chain.Name = potion.Name;
							chain.Effect = potion.Effect;
							chain.Duration = potion.Duration;
							chain.Price = potion.Price;
							found = true;
							break;
						}
					}

					if (found)
					{
						break;
					}
				}
			}
			return chains;
		}

		//все перестановки от 0 до n, не включая n
		private List<List<int>> Permutations(int n)
		{
			Contract.Requires(n < 13);
			var permutations = new List<List<int>>();
			var current = Enumerable.Range(0, n).ToList();
			var iterations = Enumerable.Range(1, n).Aggregate((accumulator, next) => accumulator * next);

			permutations.Add(new List<int>(current));
			for (int i = 1; i < iterations; i++)
			{
				var swap1 = -1;
				for (int j = n - 2; j >= 0; j--)
				{
					if (current[j] < current[j + 1])
					{
						swap1 = j;
						break;
					}
				}
				var swap2 = 1;
				for (int j = n - 1; j > swap1; j--)
				{
					if (current[swap1] < current[j])
					{
						swap2 = j;
						break;
					}
				}

				var t = current[swap1];
				current[swap1] = current[swap2];
				current[swap2] = t;
				current.Reverse(swap1 + 1, n - swap1 - 1);
				permutations.Add(new List<int>(current));
			}
			return permutations;
		}

	}
}
