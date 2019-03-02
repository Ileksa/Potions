using System.Collections.Generic;

namespace Potions
{
	public class Potion
	{
		public int Level { get; set; }
		public string Name { get; set; }
		public Effect Effect { get; set; }
		public Duration Duration { get; set; }
		public int Price { get; set; }

		public IReadOnlyCollection<Item> Recipe { get; set; }

		public Potion()
		{
			Level = 1;
			Name = "Undefined";
			Effect = Effect.Empty;
			Recipe = new List<Item>();
		}
	}
}
