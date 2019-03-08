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
		private readonly char[] _separators = new char[] { '+', '>', '→', '?' };
		private readonly string _intPattern = "\\d+";
		private readonly string _numberListPattern = "^\\d+\\)";

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

			while (!sr.EndOfStream)
			{
				var line = sr.ReadLine();
				Result res = null;

				switch (line)
				{
					case string s when s == String.Empty:
						return result;
					case string s when s.StartsWith("+"):
						res = SetEffect(potion, line);
						break;
					case string s when s.StartsWith("Длительность"):
						res = SetDuration(potion, line);
						break;
					case string s when (s.StartsWith("Рецепт")|| IsRecipe(s)):
						res = AddRecipe(potion, line);
						break;
					case string s when s.StartsWith("Ценность"):
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
			if (line.Length > 0 && Char.IsDigit(line[0]))
			{
				var match = Regex.Match(line, _numberListPattern);
				return match.Success;
			}
			return false;
		}

		/// <summary>
		/// Разбирает переданную строку на элементы рецепта. В случае успеха возвращает true и записывает рецепт в переменную result. Иначе возвращает false и текст ошибки.
		/// </summary>
		public Result ParseRecipe(string recipe, out Recipe result)
		{
			var itemsNames = recipe.Split(_separators, StringSplitOptions.RemoveEmptyEntries);
			var items = new List<Item>();

			foreach (var itemName in itemsNames)
			{
				var clearedItemName = itemName.Trim();
				var item = _itemPool.Find(clearedItemName);
				if (!item.HasValue)
				{
					result = null;
					return Result.Failed($"Не удалось распознать ингредиент с названием {itemName}");
				}
				items.Add(item.Value);
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
			var recipeText = line
				.Replace("Рецепт:", String.Empty)
				.Replace("Рецепт", String.Empty);

			var match = Regex.Match(line, _numberListPattern);
			if (match.Success)
			{
				recipeText = recipeText.Substring(match.Length);
			}

			recipeText = recipeText.Trim(' ');

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
	}
}
