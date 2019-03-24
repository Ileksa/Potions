using Functional.Maybe;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Potions
{
	public class BBCodePotionPrinter : IIPotionPrinter
	{
		protected string Path;
		public BBCodePotionPrinter(string path)
		{
			Path = path;
		}

		public void Print(Potion potion)
		{
			using (var sw = new StreamWriter(Path))
			{
				Print(potion);
			}
		}

		public void Print(IReadOnlyCollection<Potion> potions)
		{
			using (var sw = new StreamWriter(Path))
			{
				PrintByLevel(sw, potions, 1);
				PrintByLevel(sw, potions, 2);
				PrintByLevel(sw, potions, 3);
			}
		}

		private Potion CombinePotions(IEnumerable<Potion> potions)
		{
			var first = potions.First();
			var result = new Potion()
			{
				Level = first.Level,
				Name = first.Name,
				Effect = first.Effect,
				Duration = first.Duration,
				Price = potions.Select(p => p.Price).FirstMaybe(p => p.HasValue)
			};

			foreach (var p in potions)
			{
				result.AddRange(p.Recipes);
			}

			return result;
	}

		//фильтрует, коллапсирует и сортирует зелья
		private IReadOnlyCollection<Potion> OrganizePotions(Potion[] potions, Func<Potion, bool> filterPredicate)
		{
			var filtered = potions.Where(filterPredicate);
			var groupped = filtered.GroupBy(p => p.Name, (key, ps) => CombinePotions(ps));
			var ordered = groupped.OrderBy(p => p.Price.HasValue ? p.Price.Value : 0);
			return ordered.ToArray();
		}

		private void PrintByLevel(StreamWriter sw, IReadOnlyCollection<Potion> potions, int level)
		{
			var potionsByLevel = potions.Where(p => p.Level == level).ToArray();
			if (potionsByLevel.Length == 0)
			{
				return;
			}

			var concentrationPotions = OrganizePotions(potionsByLevel, p => p.Effect.Concentration > 0 && p.Effect.Stability == 0 && p.Effect.Efficiency == 0);
			var stabilityPotions = OrganizePotions(potionsByLevel, p => p.Effect.Concentration == 0 && p.Effect.Stability > 0 && p.Effect.Efficiency == 0);
			var efficiencyPotions = OrganizePotions(potionsByLevel, p => p.Effect.Concentration == 0 && p.Effect.Stability == 0 && p.Effect.Efficiency > 0);
			var oldStylePotions = OrganizePotions(potionsByLevel, p => p.Effect.Concentration > 0 && p.Effect.Stability > 0 && p.Effect.Efficiency > 0);

			PrintLevelHeader(sw, $"Рецепты зелий {level} уровня");
			if (concentrationPotions.Count > 0)
			{
				PrintEffectHeader(sw, "Концентрация");
				PrintTimeGroups(sw, concentrationPotions);
			}
			if (stabilityPotions.Count > 0)
			{
				PrintEffectHeader(sw, "Устойчивость");
				PrintTimeGroups(sw, stabilityPotions);
			}
			if (efficiencyPotions.Count > 0)
			{
				PrintEffectHeader(sw, "Эффективность");
				PrintTimeGroups(sw, efficiencyPotions);
			}
			if (oldStylePotions.Count > 0)
			{
				PrintEffectHeader(sw, "Старого образца");
				PrintTimeGroups(sw, oldStylePotions);
			}
		}

		private void PrintLevelHeader(StreamWriter sw, string header)
		{
			sw.WriteLine(header.Bold().Underlined().Size(7).Center());
		}

		private void PrintEffectHeader(StreamWriter sw, string header)
		{
			sw.WriteLine(header.Bold().Underlined().Size(5).Center());
		}

		private void PrintTimeGroups(StreamWriter sw, IReadOnlyCollection<Potion> potions)
		{
			PrintGroup(sw, "Пять минут", potions.Where(p => p.Duration == Duration.FiveMinutes).ToArray());
			PrintGroup(sw, "Час", potions.Where(p => p.Duration == Duration.Hour).ToArray());
			PrintGroup(sw, "Пять часов", potions.Where(p => p.Duration == Duration.FiveHours).ToArray());
			PrintGroup(sw, "Неделя", potions.Where(p => p.Duration == Duration.Week).ToArray());
			PrintGroup(sw, "Месяц", potions.Where(p => p.Duration == Duration.Month).ToArray());
			PrintGroup(sw, "Два месяца", potions.Where(p => p.Duration == Duration.TwoMonths).ToArray());
		}

		private void PrintGroup(StreamWriter sw, string header, IReadOnlyCollection<Potion> potions)
		{
			if (potions.Count == 0)
			{
				return;
			}
			sw.WriteLine(header.Bold().Underlined().Size(3).Center());
			sw.Write(BBCodeStringExtenstions.SpoilerOpen);
			foreach(var potion in potions)
			{
				Print(sw, potion);
			}
			sw.WriteLine(BBCodeStringExtenstions.SpoilerClose);
		}

		private void Print(StreamWriter sw, Potion potion)
		{
			PrintName(sw, potion.Name);
			Print(sw, potion.Effect);
			Print(sw, potion.Duration);
			Print(sw, potion.Recipes);
			PrintPrice(sw, potion.Price);
			sw.WriteLine();
		}

		private void Print(StreamWriter sw, Effect effect)
		{
			if (effect.Concentration > 0)
			{
				sw.WriteLine("+" + effect.Concentration + " концентрации");
			}
			if (effect.Stability > 0)
			{
				sw.WriteLine("+" + effect.Stability + " устойчивости");
			}
			if (effect.Efficiency > 0)
			{
				sw.WriteLine("+" + effect.Efficiency + "% эффективности");
			}
			if (effect.Reflection > 0)
			{
				sw.WriteLine("+" + effect.Reflection + " отражения");
			}
		}

		private void Print(StreamWriter sw, Duration duration)
		{
			string str = String.Empty;
			switch (duration)
			{
				case Duration.Undefined:
					break;
				case Duration.FiveMinutes:
					str = "пять минут";
					break;
				case Duration.Hour:
					str = "час";
					break;
				case Duration.FiveHours:
					str = "пять часов";
					break;
				case Duration.Week:
					str = "неделя";
					break;
				case Duration.Month:
					str = "месяц";
					break;
				case Duration.TwoMonths:
					str = "два месяца";
					break;
			}
			if (!String.IsNullOrWhiteSpace(str))
			{
				sw.WriteLine("Длительность".Bold() + ": " + str + "");
			}
		}

		private void PrintName(StreamWriter sw, string name)
		{
			sw.WriteLine(name.Bold());
		}

		private IReadOnlyCollection<Recipe> Sort(IReadOnlyCollection<Recipe> recipes)
		{
			var result = new List<Recipe>(recipes);
			result.Sort();
			return result;
		}

		private string ItemToString(Item item)
		{
			if (item.IsAction)
			{
				var bold = item.Name.Bold();
				switch(item.Name)
				{
					case "Простое помешивание":
						return bold.Color("32511f");
					case "Простое заклинание":
						return bold.Color("5a2b59");
					case "Нагревание":
						return bold.Color("6a3611");
					case "Свет полной луны":
						return bold.Color("5487bb");
					default:
						return bold;
				}
			}
			else if (item.Rarity != Rarity.Usual)
			{
				return item.Name.Italic();
			}
			else
			{
				return item.Name;
			}
		}

		private void Print(StreamWriter sw, IReadOnlyCollection<Recipe> recipes)
		{
			switch (recipes.Count)
			{
				case 0:
					break;
				case 1:
					sw.WriteLine("Рецепт".Bold()
						+ ": "
						+ String.Join(" + ", recipes.First().Items.Select(ItemToString)));
					break;
				default:
					var sorted = Sort(recipes);
					sw.WriteLine("Рецепты:".Bold());
					sorted.ForEach((r, index) =>
					{
						sw.WriteLine($"{index + 1}) "
							+ String.Join(" + ", r.Items.Select(ItemToString)));
					});
					break;
			}
		}

		private void PrintPrice(StreamWriter sw, Maybe<int> price)
		{
			if (price.HasValue)
			{
				sw.WriteLine("Ценность".Bold() + ": " + price.Value);
			}
		}
	}

	public static class BBCodeStringExtenstions
	{
		public static string Bold(this string s) => "[b]" + s + "[/b]";
		public static string Italic(this string s) => "[i]" + s + "[/i]";
		public static string Underlined(this string s) => "[u]" + s + "[/u]";
		public static string Center(this string s) => "[c]" + s + "[/c]";
		public static string Size(this string s, int size) => $"[size={size}]" + s + "[/size]";
		public static string Color(this string s, string hexColor)
		{
			var color = hexColor.StartsWith("#") ? hexColor : "#" + hexColor;
			return $"[color={color}]" + s + "[/color]";
		}

		public static string SpoilerOpen => "[spoiler]";
		public static string SpoilerClose => "[/spoiler]";
		public static string HorizonalLine => "[hr]";
	}
}
