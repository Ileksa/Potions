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

		public IReadOnlyCollection<Item> All(int level, Rarity rarity = Rarity.Usual, bool includeActions = true)
		{
			return _items
				.Values
				.Where(i => !IsFullMoon(i)
					&& i.Level == level
					&& i.Rarity == rarity
					&& (includeActions ? true : !i.IsAction))
				.ToList();
		}

		private readonly Item _fullMoon = new Item(2, "Свет полной луны", true, shortName: "Луна");
		private readonly List<Item> _actions = new List<Item>()
		{
			new Item(1, "Простое помешивание", true, shortName: "Помш"),
			new Item(2, "Простое заклинание", true, shortName: "Закл"),
			new Item(3, "Нагревание", true, shortName: "Нагр")
		};

		private readonly List<Item> _firstLevelIngrs = new List<Item>()
		{
			new Item(1, "Аконит", shortName: "Акнт"),
			new Item(1, "Воронец колосистый", shortName: "Врнц"),
			new Item(1, "Соцветия вербены", shortName: "Верб"),
			new Item(1, "Соцветия папоротника", shortName: "Папр"),
			new Item(1, "Сушеные гусеницы", shortName: "Гусц")
		};

		private readonly List<Item> _secondLevelIngrs = new List<Item>()
		{
			new Item(2, "Мята", shortName: "Мята"),
			new Item(2, "Плоды можжевельника", shortName: "Мжвл"),
			new Item(2, "Сушеные жуки", shortName: "Жуки"),
			new Item(2, "Цветок лаванды", shortName: "Лвнд"),
			new Item(2, "Чеснок", shortName: "Чснк")
		};

		private readonly List<Item> _thirdLevelIngrs = new List<Item>()
		{
			new Item(3, "Белладонна", shortName: "Блдн"),
			new Item(3, "Глаз змеи", shortName: "Глзм"),
			new Item(3, "Капля родниковой воды", shortName: "Капл"),
			new Item(3, "Мимбулус Мимблетония", shortName: "Ммбл"),
			new Item(3, "Молодая еловая шишка", shortName: "Шишк")
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
