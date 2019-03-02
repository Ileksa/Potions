namespace Potions
{
	public class PotionValidator : IPotionValidator
	{
		public Result Check(Potion potion)
		{
			return Result.Success;
		}
	}
}
