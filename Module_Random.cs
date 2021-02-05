using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using den0bot.Types;
using den0bot.Util;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;

namespace den0bot.Modules.Skybot
{
	public class Module_Random : IModule
	{
		private DateTime lastCheckTime = DateTime.Now;

		// probability of triggering bot that defined as one from the specified number
		private int interval;
		private Dictionary<long, List<string>> _textList = new();

		public sealed class Database : DbContext
		{
			[Table("TData")]
			public class TData
			{
				//[Key] [Column("message")] public string Message { get; set; }

				//[Column("answer")] public string Answer { get; set; }
			}

			public Database(string connectionString)
			{
				Database.SetConnectionString(connectionString);
				Database.EnsureCreated();
			}

			public DbSet<TData> Data { get; set; }
		}

		public Module_Random()
		{
			using (var connection = new Database(GetConfigVariable("dbpath")))
			{
				//_textList = connection.Data.ToDictionary(x=> x.);
			}

			interval = Convert.ToInt32(GetConfigVariable("interval"));

			AddCommand(new Command
			{
				Name = "set random interval",
				Action = SetInterval
			});

		}

		public override void Think()
		{
			if (_textList.Count > 0 && lastCheckTime < DateTime.Now)
			{
				foreach (var dict in _textList)
				{
					int chanceMsg = RNG.Next(0, dict.Value.Count);
					string answer = dict.Value[chanceMsg];

					int chanceAns = RNG.Next(1, interval);
					if (chanceAns == 1)
					{
						_ = API.SendMessage(answer, dict.Key);
					}
				}

				lastCheckTime = DateTime.Now.AddSeconds(interval);
			}
		}

		private ICommandAnswer SetInterval(Message msg)
		{
			try
			{
				interval = Convert.ToInt32(msg.Text.Replace("set random interval ", ""));
				return new TextCommandAnswer("New random interval equal " + interval);
			}
			catch (Exception)
			{
				return new TextCommandAnswer("Аккуратней играйся с такими командами блеать!");
			}
		}
	}
}