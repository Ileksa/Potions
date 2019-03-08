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

		private void PrintByLevel(StreamWriter sw, IReadOnlyCollection<Potion> potions, int level)
		{
			var potionsByLevel = potions.Where(p => p.Level == level).ToArray();
			if (potionsByLevel.Length == 0)
			{
				return;
			}

			var concentrationPotions = potionsByLevel.Where(p => p.Effect.Concentration > 0 && p.Effect.Stability == 0 && p.Effect.Efficiency == 0).ToArray();
			var stabilityPotions = potionsByLevel.Where(p => p.Effect.Concentration == 0 && p.Effect.Stability > 0 && p.Effect.Efficiency == 0).ToArray();
			var efficiencyPotions = potionsByLevel.Where(p => p.Effect.Concentration == 0 && p.Effect.Stability == 0 && p.Effect.Efficiency > 0).ToArray();
			var oldStylePotions = potionsByLevel.Where(p => p.Effect.Concentration > 0 && p.Effect.Stability > 0 && p.Effect.Efficiency > 0).ToArray();

			PrintLevelHeader(sw, $"Рецепты зелий {level} уровня");
			if (concentrationPotions.Length > 0)
			{
				PrintEffectHeader(sw, "Концентрация");
				PrintTimeGroups(sw, concentrationPotions);
			}
			if (stabilityPotions.Length > 0)
			{
				PrintEffectHeader(sw, "Устойчивость");
				PrintTimeGroups(sw, stabilityPotions);
			}
			if (efficiencyPotions.Length > 0)
			{
				PrintEffectHeader(sw, "Эффективность");
				PrintTimeGroups(sw, efficiencyPotions);
			}
			if (oldStylePotions.Length > 0)
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
			sw.WriteLine(header.Bold().Underlined().Size(5));
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
			sw.WriteLine(header.Bold().Underlined().Size(3));
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

		private void Print(StreamWriter sw, IReadOnlyCollection<Recipe> recipes)
		{
			switch (recipes.Count)
			{
				case 0:
					break;
				case 1:
					sw.WriteLine("Рецепт".Bold()
						+ ": "
						+ String.Join(" + ", recipes.First().Items.Select(i => i.Name)));
					break;
				default:
					sw.WriteLine("Рецепты:".Bold());
					recipes.ForEach((r, index) =>
					{
						sw.WriteLine($"{index + 1}) "
							+ String.Join(" + ", r.Items.Select(i => i.Name)));
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
