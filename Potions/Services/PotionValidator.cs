using System;

namespace Potions
{
	public class PotionValidator : IPotionValidator
	{
		public Result Check(Potion potion)
		{
			var effect = potion.Effect;
			if (Math.Abs(effect.Concentration)
				+ Math.Abs(effect.Stability)
				+ Math.Abs(effect.Efficiency)
				+ Math.Abs(effect.Reflection) == 0)
			{
				return Result.Failed($"{potion.Name} не имеет эффекта");
			}

			if (potion.Recipes.Count == 0)
			{
				return Result.Failed($"{potion.Name} не имеет рецептов");
			}

			if (potion.Duration == Duration.Undefined)
			{
				return Result.Failed($"{potion.Name} не указана длительность");
			}

			return Result.Success;
		}
	}
}
