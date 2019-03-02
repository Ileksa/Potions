namespace Potions
{
	public class Effect
	{
		/// <summary>
		/// Концентрация.
		/// </summary>
		public int Concentration { get; set; }

		/// <summary>
		/// Устойчивость.
		/// </summary>
		public int Stability { get; set; }

		/// <summary>
		/// Эффективность.
		/// </summary>
		public int Efficiency { get; set; }

		/// <summary>
		/// Отражение.
		/// </summary>
		public int Reflection { get; set; }

		public static Effect Empty => new Effect();
	}
}
