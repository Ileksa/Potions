using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Potions
{
	public class ConsolePotionPrinter : IIPotionPrinter
	{
		public void Print(Potion potion)
		{
			Console.WriteLine(potion.Name);
			Console.WriteLine("Уровень: " + potion.Level);
			Console.WriteLine("Количество ингредиентов: " + potion.Recipe.Count);
		}

		public void Print(IReadOnlyCollection<Potion> potions)
		{
			throw new NotImplementedException();
		}
	}
}
