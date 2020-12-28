// Skybot 2013-2017

using System;
using den0bot.Util;
using Telegram.Bot.Types;

namespace den0bot.Modules.Skybot
{
	public class Module_Xyu : IModule
	{
		private readonly string[] phraseArray;

		public Module_Xyu()
		{
			string dbPath = GetConfigVariable("dbpath");
			if (System.IO.File.Exists(dbPath))
				phraseArray = System.IO.File.ReadAllLines(dbPath);

			AddCommand(new Command
			{
				Name = "xyu",
				Action = Xyu
			});
		}

		private string Xyu(Message msg)
		{
			if (phraseArray.Length > 0)
				return phraseArray[RNG.Next(0, phraseArray.Length - 1)].Replace("%w", msg.Text.Remove(0, 5));

			return String.Empty;
		}
	}
}
