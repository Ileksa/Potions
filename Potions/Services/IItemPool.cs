using Functional.Maybe;
using System.Collections.Generic;

namespace Potions
{
	/// <summary>
	/// Пул ингредиентов.
	/// </summary>
	public interface IItemPool
	{
		/// <summary>
		/// Получить объект элемента зельеварения по названию.
		/// </summary>
		Maybe<Item> Find(string name);

		/// <summary>
		/// Проверяет, является ли переданный элемент зельеварения полнолунием.
		/// </summary>
		bool IsFullMoon(Item item);

		IReadOnlyCollection<Item> All(int level, Rarity rarity = Rarity.Usual, bool includeActions = true);
	}
}
