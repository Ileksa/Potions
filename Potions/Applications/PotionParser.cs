using System;
using System.Collections.Generic;
using System.IO;

namespace Potions
{
    public class PotionParser
    {
        private readonly IItemPool _itemPool;

        public PotionParser(IItemPool itemPool)
        {
            _itemPool = itemPool;
        }
        private readonly char[] _separators = new char[] { '+', '>', '→', '?' };

        /// <summary>
        /// Распознает зелье и записывает его в result. Возвращает true в случае успеха, и false и текст ошибки в случае неудачи.
        /// </summary>
        public (bool, string) ParsePotion(StreamReader sr, out Potion result)
        {
            result = new Potion();
            var isSuccess = true;
            var errorMessage = (string) null;

            while (true)
            {
                var line = sr.ReadLine();
                var res = true;
                var e = (string) null;

                switch(line)
                {
                    case "\n":
                        return (isSuccess, errorMessage);
                    case string s when s.StartsWith("+"):
                        (res, e) = SetEffect(result, line);
                        break;
                    case string s when s.StartsWith("Длительность"):
                        (res, e) = SetDuration(result, line);
                        break;
                    case string s when s.StartsWith("Рецепт"):
                        (res, e) = SetRecipe(result, line);
                        break;
                    case string s when s.StartsWith("Ценность"):
                        (res, e) = SetPrice(result, line);
                        break;
                    default:
                        (res, e) = SetName(result, line);
                        break;
                }

                isSuccess = isSuccess && res;
                errorMessage = errorMessage ?? e;
            }
        }

        /// <summary>
        /// Разбирает переданную строку на элементы рецепта. В случае успеха возвращает true и записывает рецепт в переменную result. Иначе возвращает false и текст ошибки.
        /// </summary>
        public (bool, string) ParseRecipe(string recipe, out List<Item> result)
        {
            var itemsNames = recipe.Split(_separators, StringSplitOptions.RemoveEmptyEntries);
            result = new List<Item>();

            foreach(var itemName in itemsNames)
            {
                var clearedItemName = itemName.Trim();
                var item = _itemPool.Find(clearedItemName);
                if (!item.HasValue)
                {
                    return (false, $"Не удалось распознать ингредиент с названием {itemName}");
                }
                result.Add(item.Value);
            }
            return (true, null);
        }

        private (bool, string) SetEffect(Potion potion, string line)
        {
            throw new NotImplementedException();
        }

        private (bool, string) SetDuration(Potion potion, string line)
        {
            throw new NotImplementedException();
        }

        private (bool, string) SetRecipe(Potion potion, string line)
        {
            throw new NotImplementedException();
        }

        private (bool, string) SetPrice(Potion potion, string line)
        {
            throw new NotImplementedException();
        }

        private (bool, string) SetName(Potion potion, string line)
        {
            throw new NotImplementedException();
        }
    }
}
