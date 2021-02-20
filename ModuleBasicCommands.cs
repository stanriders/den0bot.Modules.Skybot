// Skybot 2013-2017

using System;
using System.Diagnostics;
using System.IO;
using den0bot.Types;
using den0bot.Util;
using Telegram.Bot.Types;
using File = System.IO.File;

namespace den0bot.Modules.Skybot
{
	public class Module_BasicCommands : IModule
	{
		private readonly DateTime startup_time = DateTime.Now;

		public Module_BasicCommands()
		{
			AddCommands(new[]
			{
				new Command
				{
					Name = "help",
					Action = _ => new TextCommandAnswer("Лучше попроси рассказать анекдот")
				},
				new Command
				{
					Name = "status",
					Action = _ => new TextCommandAnswer($"Подключенные модули:\n{string.Join('\n',Bot.Modules)}\nНе падаем уже с {startup_time}")
				},
				new Command
				{
					Name = "hardware",
					Action = Hardware
				},
				new Command
				{
					Name = "version",
					Action = Version
				},
				new Command
				{
					Names = {"changelog", "log"},
					Action = ChangeLog
				},
			});
		}

		private ICommandAnswer Hardware(Message msg)
		{
			string result = "Жаримся на таких статах:" +
			                $"\nBot uptime: {(DateTime.Now - startup_time):dd\\.hh\\:mm\\:ss}" +
			                $"\nSystem uptime: {TimeSpan.FromMilliseconds(Environment.TickCount):dd\\.hh\\:mm\\:ss}" +
			                $"\nSystem date: {DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()}" +
			                $"\nOS Version: {Environment.OSVersion}";

			if (Environment.Is64BitOperatingSystem) result += " x64";
			else result += " x86";

			result += "\nEnvironment version: " + Environment.Version;
			return new TextCommandAnswer(result);
		}

		private ICommandAnswer Version(Message msg)
		{
			string result = "Я (опять) родился, (блять)!";

			FileVersionInfo fileVersion =
				FileVersionInfo.GetVersionInfo("den0bot.dll");
			result += "\nVersion info: " + fileVersion.FileVersion;

			FileInfo fileInfo = new FileInfo("den0bot.dll");
			result += "\nBuild date: " + fileInfo.LastWriteTime;

			return new TextCommandAnswer(result);
		}

		private ICommandAnswer ChangeLog(Message msg)
		{
			string result = string.Empty;

			if (File.Exists(Directory.GetCurrentDirectory() + "\\changelog.txt"))
			{
				StreamReader fs = new StreamReader(Directory.GetCurrentDirectory() + "\\changelog.txt",
					System.Text.Encoding.GetEncoding(1251));
				string log = fs.ReadLine();

				while (log != null)
				{
					result += log + "\n";
					log = fs.ReadLine();
				}

				fs.Close();
			}
			else
			{
				result += "В результате террористических действий повстанцев чейнджлог был похищен.";
			}

			return new TextCommandAnswer(result);
		}
	}
}