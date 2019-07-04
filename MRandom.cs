// Skybot 2013-2017

using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using den0bot.Util;
using Telegram.Bot.Types;
using File = System.IO.File;

namespace den0bot.Modules.Skybot
{
    class MRandom : IModule
    {
        // probability of triggering bot that defined as one from the specified number
        // this value is checked once a minute
        private int _interval = 240;
        private string _dbFolderPath = "\\plugins\\MRandom.db";

        private Dictionary<string, List<string>> _textList = new Dictionary<string, List<string>>();
        private Timer _randomTimer;

        public MRandom()
        {
            if (File.Exists(Directory.GetCurrentDirectory() + _dbFolderPath))
                LoadBase();

            if (_textList.Count == 0) return;

            _randomTimer = new Timer(60 * 1000);
            _randomTimer.Elapsed += new ElapsedEventHandler(OnSendTimerEvent);
            _randomTimer.Enabled = true;
            _randomTimer.AutoReset = true;

			AddCommand(new Command()
			{
				Name = "set random interval",
				Action = SetInterval
			});
        }

        private void LoadBase()
        {
            SQLHandler sqlHand = new SQLHandler(_dbFolderPath);

            var tableList = sqlHand.GetTables();

            foreach (var tableName in tableList)
            {
                var textList = sqlHand.GetList(
                    tableName: tableName,
                    returnField: "TEXT" );

                _textList.Add(tableName, textList);
            }
        }

        private void OnSendTimerEvent(object source, ElapsedEventArgs e)
        {
            foreach (var dict in _textList)
            {
                int chanceMsg = RNG.Next(0, dict.Value.Count);
                string answer = dict.Value[chanceMsg];

                int chanceAns = RNG.Next(1, _interval);
                if (chanceAns == 1)
                {
                    foreach (var API in _skyBot.APIs)
                    {
                        if (API.Status == APIStatus.Connected)
                            API.SendMessage(dict.Value[chanceMsg], dict.Key);
                    }
                }
            }
        }

        private string SetInterval(Message msg)
        {
	        try
	        {
		        _interval = Convert.ToInt32(msg.Text.Replace("set random interval ", ""));
		        return "New random interval equal " + _interval;
	        }
	        catch (Exception)
	        {
		        return "Аккуратней играйся с такими командами блеать!";
	        }
        }
    }
}

