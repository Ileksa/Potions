using System;
using System.Collections.Generic;
using System.Linq;

namespace Potions
{
	/// <summary>
	/// Рецепт зелья.
	/// </summary>
	public class Recipe : IComparable<Recipe>
	{
		private List<Item> _items;
		public IReadOnlyCollection<Item> Items { get { return _items; } }
		public Recipe(IReadOnlyCollection<Item> items)
		{
			_items = items.ToList();
		}

		public int CompareTo(Recipe other)
		{
			var min = Math.Min(_items.Count, other._items.Count);
			for (int i = 0; i < min; i++)
			{
				var stringComparison = String.Compare(_items[i].Name, other._items[i].Name);
				if (stringComparison != 0)
				{
					return stringComparison;
				}
			}

			if (_items.Count == other._items.Count)
			{
				return 0;
			}

			return (_items.Count < other._items.Count) ? -1 : 1;
		}

		public static bool operator==(Recipe r1, Recipe r2)
		{
			return r1.CompareTo(r2) == 0;
		}

		public static bool operator !=(Recipe r1, Recipe r2)
		{
			return !(r1 == r2);
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
