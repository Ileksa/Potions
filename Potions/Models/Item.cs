namespace Potions
{
	public class Item
	{
		public int Level { get; protected set; }
		public string Name { get; protected set; }
		public bool IsAction { get; protected set; }
		public Rarity Rarity { get; protected set; }

		public string _shortName;
		public string ShortName => _shortName != null ? _shortName : Name.Substring(0, 4);

		public Item(int level, string name, bool isAction = false, Rarity rarity = Rarity.Usual, string shortName = null)
		{
			Level = level;
			Name = name;
			IsAction = isAction;
			Rarity = rarity;
			_shortName = shortName;
		}

		public override bool Equals(object obj)
		{
			var item = obj as Item;
			if (item == null)
			{
				return false;
			}
			return item.Level == Level
				&& item.Name == Name
				&& item.IsAction == IsAction
				&& item.Rarity == Rarity;
		}

		public override int GetHashCode()
		{
			return Level.GetHashCode()
				^ Name.GetHashCode()
				^ IsAction.GetHashCode()
				^ Rarity.GetHashCode();
		}
	}
}
