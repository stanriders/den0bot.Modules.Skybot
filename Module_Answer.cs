// Skybot 2013-2017

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using den0bot.Util;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;

namespace den0bot.Modules.Skybot
{
	// Main answering module
	public class Module_Answer : IModule, IReceiveAllMessages
	{
		public sealed class Database : DbContext
		{
			[Table("words")]
			public class Word
			{
				[Key] [Column("message")] public string Message { get; set; }

				[Column("answer")] public string Answer { get; set; }
			}

			public Database(string connectionString)
			{
				Database.SetConnectionString(connectionString);
				Database.EnsureCreated();
			}

			public DbSet<Word> Words { get; set; }
		}

		private readonly Dictionary<long, DateTime> nextPost = new(); // chatID, time
		private const int cooldown = 5; // minutes

		private readonly List<Database.Word> dbCache;

		public Module_Answer()
		{
			using (var db = new Database(GetConfigVariable("dbpath")))
				dbCache = db.Words.ToList();

		}

		public async Task ReceiveMessage(Message msg)
		{
			if (!nextPost.ContainsKey(msg.Chat.Id))
				nextPost.Add(msg.Chat.Id, DateTime.Now);

			if (nextPost[msg.Chat.Id] < DateTime.Now)
			{
				try
				{
					var words = dbCache.Where(x => x.Message == msg.Text.ToLower()).ToArray();
					if (words.Length > 0)
					{
						string result;
						if (words.Length == 1)
						{
							result = words[0].Answer;
						}
						else
						{
							result = words[RNG.Next(0, words.Length)].Answer;
						}

						// for backward compatibility with old base 
						result = result.Replace("%username%", msg.From.FirstName);

						await API.SendMessage(result, msg.Chat.Id);

						nextPost[msg.Chat.Id] = DateTime.Now.AddMinutes(cooldown);
					}
				}
				catch (Exception e)
				{
					Log.Error(e.Message);
				}
			}
		}
	}
}