using Functional.Maybe;

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
    }
}
