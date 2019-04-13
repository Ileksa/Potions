using System;
using System.Collections.Generic;

namespace Potions
{
	public class ConsolePotionPrinter
	{
		public void Print(Potion potion)
		{
			Console.WriteLine(potion.Name);
			Console.WriteLine("Уровень: " + potion.Level);
			Console.WriteLine("Количество рецептов: " + potion.Recipes.Count);
		}

		public void Print(IReadOnlyCollection<Potion> potions)
		{
			throw new NotImplementedException();
		}
	}
}
