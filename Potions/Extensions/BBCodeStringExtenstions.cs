namespace Potions
{
	public static class BBCodeStringExtenstions
	{
		public static string Bold(this string s) => "[b]" + s + "[/b]";
		public static string Italic(this string s) => "[i]" + s + "[/i]";
		public static string Underlined(this string s) => "[u]" + s + "[/u]";
		public static string Center(this string s) => "[c]" + s + "[/c]";
		public static string Size(this string s, int size) => $"[size={size}]" + s + "[/size]";
		public static string Color(this string s, string hexColor)
		{
			var color = hexColor.StartsWith("#") ? hexColor : "#" + hexColor;
			return $"[color={color}]" + s + "[/color]";
		}

		public static string SpoilerOpen => "[spoiler]";
		public static string SpoilerClose => "[/spoiler]";
		public static string HorizonalLine => "[hr]";
	}
}
