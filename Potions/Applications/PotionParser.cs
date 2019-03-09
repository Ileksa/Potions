using Functional.Maybe;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Potions
{
	public class PotionParser
	{
		private readonly IItemPool _itemPool;
		private readonly IPotionValidator _potionValidator;

		public PotionParser(IItemPool itemPool, IPotionValidator potionValidator)
		{
			_itemPool = itemPool;
			_potionValidator = potionValidator;
		}
		private readonly char[] _separators = new char[] { '+', '>', '→', '?', '-' };
		private readonly char[] _trimSeparators = new char[] { ' ', ':', '.'};
		private readonly string _intPattern = "\\d+";
		private readonly string _numberListPattern = "^\\d+\\)";
		private readonly string _multiplyItem = "(x|х)\\d+";

		public IReadOnlyCollection<Tuple<Potion, Result>> ParsePotions(StreamReader sr)
		{
			var results = new List<Tuple<Potion, Result>>();
			while (!sr.EndOfStream)
			{
				var res = ParsePotion(sr, out Potion potion);
				results.Add(new Tuple<Potion, Result>(potion, res));
			}
			return results;
		}

		/// <summary>
		/// Распознает зелье и записывает его в result.
		/// </summary>
		public Result ParsePotion(StreamReader sr, out Potion potion)
		{
			potion = new Potion();
			var result = Result.Success;
			var endOfPotion = false;
			while (!sr.EndOfStream && !endOfPotion)
			{
				var line = sr.ReadLine();
				Result res = null;

				switch (line)
				{
					case string s when s == String.Empty:
						endOfPotion = true;
						res = Result.Success;
						break;
					case string s when s.StartsWith("+"):
						res = SetEffect(potion, line);
						break;
					case string s when s.StartsWith("Длительность"):
						res = SetDuration(potion, line);
						break;
					case string s when (IsRecipe(s)):
						res = AddRecipe(potion, line);
						break;
					case string s when s.StartsWith("Ценность") || s.StartsWith("Цена"):
						res = SetPrice(potion, line);
						break;
					default:
						res = SetName(potion, line);
						break;
				}

				result = Result.Combine(result, res);
			}
			SetLevel(potion);
			return result.IsSuccess ? _potionValidator.Check(potion) : result;
		}

		private bool IsRecipe(string line)
		{
			if (line.StartsWith("Рецепт"))
			{
				return true;
			}

			if (line.StartsWith("="))
			{
				return true;
			}

			if (line.Length > 0 && Char.IsDigit(line[0]))
			{
				var match = Regex.Match(line, _numberListPattern);
				return match.Success;
			}
			return false;
		}

		/// <summary>
		/// Разбирает переданную строку на элементы рецепта.
		/// </summary>
		public Result ParseRecipe(string recipe, out Recipe result)
		{
			result = null;
			var itemsNames = recipe.Split(_separators, StringSplitOptions.RemoveEmptyEntries);
			var items = new List<Item>();

			foreach (var itemName in itemsNames)
			{
				var clearedItemName = itemName;
				int count = 1;
				var match = Regex.Match(itemName, _multiplyItem);
				if (match.Success)
				{
					if (!Int32.TryParse(match.Value.Substring(1), out count))
					{
						return Result.Failed($"Не удалось получить целое число из совпадения {match.Value} вида (x|х)\\d+ из строки {itemName}");
					}
					clearedItemName = itemName.Substring(0, match.Index);
				}

				clearedItemName = clearedItemName.Trim(_trimSeparators);
				clearedItemName = CorrectItemNameIfPossible(clearedItemName);
				var item = _itemPool.Find(clearedItemName);
				if (!item.HasValue)
				{
					return Result.Failed($"Не удалось распознать ингредиент с названием \'{itemName}\'");
				}

				for (int i = 0; i < count; i++)
				{
					items.Add(item.Value);
				}
			}
			result = new Recipe(items);
			return Result.Success;
		}

		private Result SetEffect(Potion potion, string line)
		{
			var match = Regex.Match(line, _intPattern);
			if (!match.Success)
			{
				return Result.Failed($"Не удалось определить величину эффекта в строке {line}");
			}

			if (!Int32.TryParse(match.Value, out int value))
			{
				return Result.Failed($"Не удалось получить целое число из совпадения {match.Value} из строки {line}");
			}

			switch (line.ToLower())
			{
				case string s when s.Contains("устойчивости"):
					potion.Effect.Stability = value;
					break;
				case string s when s.Contains("эффективности"):
					potion.Effect.Efficiency = value;
					break;
				case string s when s.Contains("концентрации"):
					potion.Effect.Concentration = value;
					break;
				default:
					return Result.Failed($"Не удалось распознать эффект в строке {line}");
			}

			return Result.Success;
		}

		private Result SetDuration(Potion potion, string line)
		{
			switch (line.ToLower())
			{
				case string s when s.Contains("пять минут"):
					potion.Duration = Duration.FiveMinutes;
					break;
				case string s when s.Contains("час"):
					potion.Duration = Duration.Hour;
					break;
				case string s when s.Contains("пять часов"):
					potion.Duration = Duration.FiveHours;
					break;
				case string s when s.Contains("неделя"):
					potion.Duration = Duration.Week;
					break;
				case string s when s.Contains("месяц"):
					potion.Duration = Duration.Month;
					break;
				case string s when s.Contains("два месяца"):
					potion.Duration = Duration.TwoMonths;
					break;
				default:
					return Result.Failed($"Не удалось распознать длительность в строке {line}");
			}

			return Result.Success;
		}

		private void SetLevel(Potion potion)
		{
			if (potion.Recipes.Count == 0)
			{
				return;
			}

			potion.Level = potion
				.Recipes
				.Select(r => r.Items
					.Where(i => !i.IsAction)
					.Select(i => i.Level)
					.Max())
				.Max();
		}

		private Result AddRecipe(Potion potion, string line)
		{
			if (line.StartsWith("Рецепты"))
			{
				return Result.Success;
			}

			var recipeText = line
				.Replace("Рецепт:", String.Empty)
				.Replace("Рецепт", String.Empty)
				.Replace("=", String.Empty);

			var match = Regex.Match(line, _numberListPattern);
			if (match.Success)
			{
				recipeText = recipeText.Substring(match.Length);
			}

			recipeText = recipeText.Trim(_trimSeparators);

			var result = ParseRecipe(recipeText, out Recipe recipe);
			if (result.IsFailed)
			{
				return result;
			}

			if (recipe.Items.Count > 0)
			{
				potion.Add(recipe);
			}

			return Result.Success;
		}

		private Result SetPrice(Potion potion, string line)
		{
			var match = Regex.Match(line, _intPattern);
			if (!match.Success)
			{
				return Result.Failed($"Не удалось определить ценность в строке {line}");
			}

			if (!Int32.TryParse(match.Value, out int value))
			{
				return Result.Failed($"Не удалось получить целое число из совпадения {match.Value} из строки {line}");
			}

			potion.Price = value.ToMaybe();
			return Result.Success;
		}

		private Result SetName(Potion potion, string line)
		{
			potion.Name = line;
			return Result.Success;
		}

		//ключ - неверное название, значение - верное название
		private Dictionary<string, string> correctItemNames = new Dictionary<string, string>()
		{ //сортировать по алфавиту по ключу
			{"вербена", "соцветия вербены" },
			{"воронец", "воронец колосистый" },
			{"гусеницы", "сушеные гусеницы" },
			{"лаванда", "цветок лаванды"},
			{"можжевельник", "плоды можжевельника" },
			{"папоротник", "соцветия папоротника" },
			{"плоды можжевельник", "плоды можжевельника" },
			{"помешивание", "простое помешивание" },
			{"простое простое заклинание", "простое заклинание" },
			{"соцветие вербены", "соцветия вербены" },
			{"соцветие папоротника", "соцветия папоротника" },
			{"соцветия папоротник", "соцветия папоротника" },
			{"сушеные сушеные жуки", "сушеные жуки" }
		};

		private string CorrectItemNameIfPossible(string itemName)
		{
			var lower = itemName.ToLower();
			return correctItemNames.ContainsKey(lower)
				? correctItemNames[lower]
				: itemName;
		}
	}
}
