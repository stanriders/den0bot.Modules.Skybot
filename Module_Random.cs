using System;
using System.Collections.Generic;
using System.Text;
using den0bot.Util;
using SQLite;
using Telegram.Bot.Types;

namespace den0bot.Modules.Skybot
{
	public class Module_Random : IModule
	{
		private DateTime lastCheckTime = DateTime.Now;

		// probability of triggering bot that defined as one from the specified number
		private int interval;
		private Dictionary<long, List<string>> _textList = new Dictionary<long, List<string>>();

		public Module_Random()
		{
			using (var connection = new SQLiteConnection(GetConfigVariable("dbpath")))
			{
				//connection.CreateTable<TData>();
				//_tDataList = connection.Table<TData>().ToList();
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

		private string SetInterval(Message msg)
		{
			try
			{
				interval = Convert.ToInt32(msg.Text.Replace("set random interval ", ""));
				return "New random interval equal " + interval;
			}
			catch (Exception)
			{
				return "Аккуратней играйся с такими командами блеать!";
			}
		}
	}
}