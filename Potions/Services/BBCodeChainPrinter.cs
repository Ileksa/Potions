using MoreLinq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Potions
{
	public class BBCodeChainPrinter
	{
		protected string Path;
		public BBCodeChainPrinter(string path)
		{
			Path = path;
		}

		public void Print(Chain chain)
		{
			using (var sw = new StreamWriter(Path))
			{
				Print(sw, chain);
			}
		}

		private void Print(StreamWriter sw, Chain chain)
		{
			var recipe = String.Join(" + ", chain.Recipe.Items.Select(i => ItemToString(i)));
			var description = String.Empty;
			if (!chain.HasDefaultName)
			{
				var effs = new List<string>();
				if (chain.Effect.Concentration > 0)
				{
					effs.Add($"{chain.Effect.Concentration} конц");
				}
				if (chain.Effect.Stability > 0)
				{
					effs.Add($"{chain.Effect.Stability} уст");
				}
				if (chain.Effect.Efficiency > 0)
				{
					effs.Add($"{chain.Effect.Efficiency} эфф");
				}

				description = " " + String.Join(", ", effs) + ", " + ToString(chain.Duration);
			}
			sw.WriteLine(recipe + " =" + description);
		}

		private string ToString(Duration duration)
		{
			switch (duration)
			{
				case Duration.FiveMinutes:
					return "5 минут";
				case Duration.Hour:
					return "час";
				case Duration.FiveHours:
					return "5 часов";
				case Duration.Week:
					return "неделя";
				case Duration.Month:
					return "месяц";
				case Duration.TwoMonths:
					return "2 месяца";
				case Duration.Undefined:
				default:
					return "НЕ ОПРЕДЕЛЕНО";
			}
		}

		private string ItemToString(Item item)
		{
			if (item.IsAction)
			{
				var bold = item.ShortName.Bold();
				switch (item.Name)
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
				return item.Name;
			}
			else
			{
				return item.ShortName;
			}
		}

		public void Print(IReadOnlyCollection<Chain> chains)
		{
			using (var sw = new StreamWriter(Path))
			{
				for (int i = 1; i <= 3; i++)
				{
					chains
						.Where(c => c.Level == i)
						.OrderBy(c => c.Recipe)
						.ForEach(c => Print(sw, c));
				}
			}
		}
	}
}
