using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Potions.Applications
{
	public class ConsoleApplication
	{
		private readonly IItemPool _itemPool;
		private readonly IPotionValidator _potionValidator;
		private readonly PotionParser _potionParser;
		private readonly IIPotionPrinter _potionPrinter;

		public ConsoleApplication()
		{
			//впилить ninject и вообще по уму сделать
			_itemPool = new ItemPool();
			_potionValidator = new PotionValidator();
			_potionParser = new PotionParser(_itemPool, _potionValidator);
			_potionPrinter = new ConsolePotionPrinter();
		}

		public void Run()
		{
			while (true)
			{
				Console.WriteLine("Введите путь к файлу. Чтобы выйти, введите пустую строку.");
				var path = Console.ReadLine();
				if (path == String.Empty)
				{
					return;
				}
				if (!File.Exists(path))
				{
					continue;
				}

				using (var sr = new StreamReader(path))
				{
					var results = _potionParser.ParsePotions(sr);
					foreach (var result in results)
					{
						if (result.Item2.IsSuccess)
						{
							Console.WriteLine("Успех!");
							_potionPrinter.Print(result.Item1);
						}
						else
						{
							Console.WriteLine("Не удалось распознать зелье");
							Console.WriteLine(result.Item2.ErrorMessage);
						}
					}
				}
			}
		}
	}
}
