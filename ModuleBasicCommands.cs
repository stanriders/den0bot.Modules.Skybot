﻿// Skybot 2013-2017

using System;
using System.Diagnostics;
using System.IO;
using Telegram.Bot.Types;
using File = System.IO.File;

namespace den0bot.Modules.Skybot
{
    public class Module_BasicCommands : IModule
    {
	    private readonly DateTime startup_time = DateTime.Now;
		public Module_BasicCommands()
        {
            AddCommands(new []
            {
				new Command
				{
					Name = "help",
					Action = _ => "Лучше попроси рассказать анекдот"
				},
				new Command
				{
					Name = "status",
					Action = _ => $"Подключенные модули:\nModule_Answer\nModule_Roll\nНе падаем уже с {startup_time}"
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

        private string Hardware(Message msg)
        {
            string result = "Жаримся на таких статах:" + 
                "\nBot uptime: " + (DateTime.Now - startup_time) +
                "\nSystem uptime: " + TimeSpan.FromMilliseconds(Environment.TickCount) +
                "\nSystem date: " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() +
                "\nOS Version: " + Environment.OSVersion;

            if (Environment.Is64BitOperatingSystem) result += " x64";
            else result += " x86";

            result += "\nEnvironment version: " + Environment.Version;
            return result;
        }

        private string Version(Message msg)
        {
            string result = "Я (опять) родился, (блять)!";

            FileVersionInfo fileVersion = FileVersionInfo.GetVersionInfo(Directory.GetCurrentDirectory() + "\\den0bot.exe");
            result += "\nVersion info: " + fileVersion.FileVersion;

            FileInfo fileInfo = new FileInfo(Directory.GetCurrentDirectory() + "\\den0bot.exe");
            result += "\nBuild date: " + fileInfo.LastWriteTime;

            return result;
        }

        private string ChangeLog(Message msg)
        {
            string result = string.Empty;

            if (File.Exists(Directory.GetCurrentDirectory() + "\\changelog.txt"))
            {
                StreamReader fs = new StreamReader(Directory.GetCurrentDirectory() + "\\changelog.txt", System.Text.Encoding.GetEncoding(1251));
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

            return result;
        }
    }
}