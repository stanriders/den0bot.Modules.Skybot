using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using den0bot.Util;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;

namespace den0bot.Modules.Skybot
{
	public class Module_Trigger : IModule, IReceiveAllMessages
	{
		public sealed class Database : DbContext
		{
			[Table("TData")]
			public class TData
			{
				/// <summary>
				/// Список ID пользователя(-ей) через пробел
				/// </summary>
				public string UserID { get; set; }

				/// <summary>
				/// Список слов через пробел
				/// </summary>
				public string Words { get; set; }

				/// <summary>
				/// Количество слов, необходимых для срабатывания триггера
				/// </summary>
				public int Count { get; set; }

				/// <summary>
				/// Время, в течение которого должны быть найдены слова из Words Count раз
				/// </summary>
				public int Timeout { get; set; }

				/// <summary>
				/// Ответы бота через ;
				/// </summary>
				public string Answers { get; set; }

				/// <summary>
				/// Уникальный ID триггера, позволяет добавлять и идентифицировать несколько триггеров в коллекцию TUserData с одинаковым ChatID
				/// </summary>
				public int DataID { get; set; }
			}

			public Database(string connectionString)
			{
				Database.SetConnectionString(connectionString);
				Database.EnsureCreated();
			}

			public DbSet<TData> Data { get; set; }
		}

		private class TUserData
		{
			/// <summary>
			/// Совпавший по UserID триггер
			/// </summary>
			public Database.TData TData { get; set; }

			/// <summary>
			/// Количество слов, найденных сейчас до истечения таймаута
			/// </summary>
			public int Count { get; set; }

			/// <summary>
			/// Время последнего обновления счетчика
			/// </summary>
			public DateTime LastTime { get; set; }

			/// <summary>
			/// ID чата для ответа
			/// </summary>
			public long ChatID { get; set; }
		}

		private List<Database.TData> _tDataList; // коллекция триггеров из БД
		private List<TUserData> _tUserDataList = new(); // коллекция подобранных, но еще не сработавших триггеров

		public Module_Trigger()
		{
			using (var db = new Database(GetConfigVariable("dbpath")))
				_tDataList = db.Data.ToList();
		}

		public async Task ReceiveMessage(Message message)
		{
			if (_tDataList.Any())
				await API.SendMessage(AnalyseMessage(message.Text.ToLower(), message.Chat.Id, message.From.Username),
					message.Chat.Id);
		}

		private string AnalyseMessage(string message, long chatId, string userId)
		{
			foreach (var tData in _tDataList)
			{
				// проверяем совпадение ника в триггере
				if (tData.UserID.Split(new Char[] {' '}).Any(x => x == userId || x == "<all>"))
				{
					var wordList = message
						.Replace(',', ' ')
						.Replace('.', ' ')
						.Replace('?', ' ')
						.Replace('!', ' ')
						.Replace('-', ' ')
						.Split(new Char[] {' '})
						.ToList();

					// првоеряем совпадение любого слова из сообщения со словом из триггера
					if (tData.Words
						.Split(new Char[] {' '})
						.ToList()
						.Intersect(wordList)
						.ToList()
						.Count > 0)
					{
						// проверяем наличие этого триггера в коллекции найденных триггеров по ID триггера и чата
						if (_tUserDataList.Any(x => x.TData.DataID == tData.DataID && x.ChatID == chatId))
						{
							// триггер найден, проверяем кол-во найденных слов с необходимым для срабатывания
							var tDataFound =
								_tUserDataList.Find(x => x.TData.DataID == tData.DataID && x.ChatID == chatId);
							tDataFound.Count++;

							if (tDataFound.Count == tDataFound.TData.Count)
							{
								string[] answerList = tDataFound.TData.Answers.Split(new Char[] {';'});

								_tUserDataList.Remove(tDataFound);

								// проверяем истечение таймаута
								if ((int) (DateTime.Now - tDataFound.LastTime).TotalSeconds <= tDataFound.TData.Timeout)
								{
									return answerList[RNG.Next(0, answerList.Length /* - 1*/)];
								}
							}
						}
						else
						{
							// триггер не найден, добавляем в коллекцию
							_tUserDataList.Add(new TUserData
							{
								TData = tData,
								Count = 1,
								LastTime = DateTime.Now,
								ChatID = chatId,
							});
						}

						break;
					}
				}
			}

			return string.Empty;
		}
	}
}