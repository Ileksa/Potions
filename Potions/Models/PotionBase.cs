using Functional.Maybe;

namespace Potions
{
	public abstract class PotionBase
	{
		public int Level { get; set; }
		public string Name { get; set; }
		public Effect Effect { get; set; }
		public Duration Duration { get; set; }
		public Maybe<int> Price { get; set; }

		public const string DefaultName = "Undefined";
		public bool HasDefaultName => Name == DefaultName;

		public PotionBase()
		{
			Level = 1;
			Name = DefaultName;
			Effect = Effect.Empty;
			Price = Maybe<int>.Nothing;
		}
	}
}
