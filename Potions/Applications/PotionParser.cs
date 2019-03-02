using System;
using System.Collections.Generic;
using System.IO;
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
					case "\n":
						return result;
					case string s when s.StartsWith("+"):
						res = SetEffect(potion, line);
						break;
					case string s when s.StartsWith("Длительность"):
						res = SetDuration(potion, line);
						break;
					case string s when s.StartsWith("Рецепт"):
						res = SetRecipe(potion, line);
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
			throw new NotImplementedException();
		}

		private Result SetRecipe(Potion potion, string line)
		{
			throw new NotImplementedException();
		}

		private Result SetPrice(Potion potion, string line)
		{
			throw new NotImplementedException();
		}

		private Result SetName(Potion potion, string line)
		{
			throw new NotImplementedException();
		}
	}
}
