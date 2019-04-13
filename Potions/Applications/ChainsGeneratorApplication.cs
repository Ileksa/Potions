using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Potions
{
	public class ChainsGeneratorApplication: IApplication
	{
		private readonly ChainsGenerator _chainsGenerator;
		private readonly IItemPool _itemPool;
		private readonly IPotionValidator _potionValidator;
		private readonly PotionParser _potionParser;
		private readonly BBCodeChainPrinter _potionPrinter;

		//не ок реализация, поправить, когда будет инъекция
		public ChainsGeneratorApplication()
		{
			_itemPool = new ItemPool();
			_potionValidator = new PotionValidator();
			_potionParser = new PotionParser(_itemPool, _potionValidator);
			_chainsGenerator = new ChainsGenerator(_itemPool);
			_potionPrinter = new BBCodeChainPrinter("result.txt");
		}

		public void Run()
		{
			while (true)
			{
				Console.WriteLine("Введите путь к файлу с существующими зельями. Чтобы выйти, введите пустую строку.");
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
					var successPotions = results
						.Where(r => r.Item2.IsSuccess)
						.Select(r => r.Item1)
						.ToArray();
					var errors = results
						.Where(r => r.Item2.IsFailed)
						.Select(r => r.Item2)
						.ToArray();

					var chains = _chainsGenerator.TenUsualOneSeasonalOrRarePatternOne(successPotions);
					_potionPrinter.Print(chains);

					foreach (var error in errors)
					{
						Console.WriteLine("Не удалось распознать зелье");
						Console.WriteLine(error.ErrorMessage);
						Console.WriteLine();
					}
				}
			}
		}
	}
}
