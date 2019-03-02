using System;

namespace Potions
{
	/// <summary>
	/// Результат выполнения работы. Если работа выполнена успешно, то Success установлен в true, а в ErrorMessage - null. 
	/// Иначе Success установлен в false, а в ErrorMessage содержится описание ошибки.
	/// </summary>
	public class Result
	{
		public bool IsSuccess { get; }
		public string ErrorMessage { get; }

		protected Result()
		{
			IsSuccess = true;
			ErrorMessage = null;
		}

		protected Result(string errorMessage)
		{
			IsSuccess = false;
			ErrorMessage = errorMessage;
		}

		public static Result Success => new Result();
		public static Result Failed(string errorMessage) => new Result(errorMessage);

		public static Result Combine(Result res1, Result res2)
		{
			if (res1.IsSuccess)
			{
				if (res2.IsSuccess)
				{
					return Success;
				}
				else
				{
					return Failed(res2.ErrorMessage);
				}
			}
			else
			{
				if (res2.IsSuccess)
				{
					return Failed(res1.ErrorMessage);
				}
				else
				{
					return Failed(res1.ErrorMessage + Environment.NewLine + res2.ErrorMessage);
				}
			}
		}
	}
}
