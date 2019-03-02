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

		public PotionParser(IItemPool itemPool)
		{
			_itemPool = itemPool;
		}
		private readonly char[] _separators = new char[] { '+', '>', '→', '?' };
		private readonly string _intPattern = "\\d+";
		/// <summary>
		/// Распознает зелье и записывает его в result.
		/// </summary>
		public Result ParsePotion(StreamReader sr, out Potion potion)
		{
			potion = new Potion();
			var result = Result.Success;

			while (true)
			{
				var line = sr.ReadLine();
				Result res = null;

				switch (line)
				{
					case string s when s == Environment.NewLine:
						return result;
					case string s when s.StartsWith("+"):
						res = SetEffect(potion, line);
						break;
					case string s when s.StartsWith("Длительность"):
						res = SetDuration(potion, line);
						break;
					case string s when s.StartsWith("Рецепт"):
						res = SetRecipeAndLevel(potion, line);
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
		}

		/// <summary>
		/// Разбирает переданную строку на элементы рецепта. В случае успеха возвращает true и записывает рецепт в переменную result. Иначе возвращает false и текст ошибки.
		/// </summary>
		public Result ParseRecipe(string recipe, out List<Item> result)
		{
			var itemsNames = recipe.Split(_separators, StringSplitOptions.RemoveEmptyEntries);
			result = new List<Item>();

			foreach (var itemName in itemsNames)
			{
				var clearedItemName = itemName.Trim();
				var item = _itemPool.Find(clearedItemName);
				if (!item.HasValue)
				{
					return Result.Failed($"Не удалось распознать ингредиент с названием {itemName}");
				}
				result.Add(item.Value);
			}
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

		private Result SetRecipeAndLevel(Potion potion, string line)
		{
			var recipe = line
				.Replace("Рецепт:", String.Empty)
				.Replace("Рецепт", String.Empty)
				.Trim(' ');


			var result = ParseRecipe(recipe, out List<Item> items);
			if (result.IsFailed)
			{
				return result;
			}

			var level = items.Select(i => i.Level).Max();

			potion.Recipe = items;
			potion.Level = level;

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
