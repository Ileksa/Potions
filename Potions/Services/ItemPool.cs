using System.Collections.Generic;
using System.Linq;

using Functional.Maybe;

namespace Potions
{
	public sealed class ItemPool : IItemPool
	{
		private IDictionary<string, Item> _items;
		public ItemPool()
		{
			_items = _actions
				.Append(_fullMoon)
				.Concat(_firstLevelIngrs)
				.Concat(_secondLevelIngrs)
				.Concat(_thirdLevelIngrs)
				.Concat(_firstLevelSeasonIngrs)
				.Concat(_secondLevelSeasonIngrs)
				.Concat(_thirdLevelSeasonIngrs)
				.Concat(_rareIngs)
				.ToDictionary(item => item.Name.ToLower());
		}

		public Maybe<Item> Find(string name)
		{
			var clearedName = name.ToLower();
			if (_items.ContainsKey(clearedName))
			{
				return _items[clearedName].ToMaybe();
			}
			else
			{
				return Maybe<Item>.Nothing;
			}

		}

		public bool IsFullMoon(Item item)
		{
			return item.Equals(_fullMoon);
		}


		private readonly Item _fullMoon = new Item(2, "Полнолуние", true);
		private readonly List<Item> _actions = new List<Item>()
		{
			new Item(1, "Простое помешивание", true),
			new Item(2, "Простое заклинание", true),
			new Item(3, "Нагревание", true)
		};

		private readonly List<Item> _firstLevelIngrs = new List<Item>()
		{
			new Item(1, "Аконит"),
			new Item(1, "Воронец колосистый"),
			new Item(1, "Соцветия вербены"),
			new Item(1, "Соцветия папоротника"),
			new Item(1, "Сушеные гусеницы")
		};

		private readonly List<Item> _secondLevelIngrs = new List<Item>()
		{
			new Item(2, "Мята"),
			new Item(2, "Плоды можжевельника"),
			new Item(2, "Сушеные жуки"),
			new Item(2, "Цветок лаванды"),
			new Item(2, "Чеснок")
		};

		private readonly List<Item> _thirdLevelIngrs = new List<Item>()
		{
			new Item(3, "Белладонна"),
			new Item(3, "Глаз змеи"),
			new Item(3, "Капля родниковой воды"),
			new Item(3, "Мимбулус Мимблетония"),
			new Item(3, "Молодая еловая шишка")
		};

		private readonly List<Item> _firstLevelSeasonIngrs = new List<Item>()
		{
			new Item(1, "Первая снежинка", rarity: Rarity.Seasonal),
			new Item(1, "Лепестки гиацинта", rarity: Rarity.Seasonal),
			new Item(1, "Страусиная яичная скорлупа", rarity: Rarity.Seasonal),
			new Item(1, "Кленовый листок", rarity: Rarity.Seasonal)
		};

		private readonly List<Item> _secondLevelSeasonIngrs = new List<Item>()
		{
			new Item(2, "Тисовые иголки", rarity: Rarity.Seasonal),
			new Item(2, "Одуванчики", rarity: Rarity.Seasonal),
			new Item(2, "Жало скорпиона", rarity: Rarity.Seasonal),
			new Item(2, "Волшебный желудь", rarity: Rarity.Seasonal)
		};

		private readonly List<Item> _thirdLevelSeasonIngrs = new List<Item>()
		{
			new Item(3, "Отражение северного сияния", rarity: Rarity.Seasonal),
			new Item(3, "Запах весенней грозы", rarity: Rarity.Seasonal),
			new Item(3, "Солнечный зайчик", rarity: Rarity.Seasonal),
			new Item(3, "Прядь волос мавки", rarity: Rarity.Seasonal)
		};

		private readonly List<Item> _rareIngs = new List<Item>()
		{
			new Item(1, "Перо гиппогрифа", rarity: Rarity.Rare),
			new Item(2, "Волос единорога", rarity: Rarity.Rare),
			new Item(3, "Перо феникса", rarity: Rarity.Rare)
		};
	}
}
