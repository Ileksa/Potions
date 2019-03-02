using System.Collections.Generic;

namespace Potions
{
	public interface IIPotionPrinter
	{
		void Print(Potion potion);
		void Print(IReadOnlyCollection<Potion> potions);
	}
}
