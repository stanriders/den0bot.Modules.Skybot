// Skybot 2013-2017

using System;
using System.Collections.Generic;
using System.Linq;
using den0bot.Util;
using SQLite;
using Telegram.Bot.Types;

namespace den0bot.Modules.Skybot
{
    // Main answering module
    public class Module_Answer : IModule, IReceiveAllMessages
    {
        private readonly SQLiteConnection connection;

		// not camelcase for backward compat
        private class words
        {
	        [PrimaryKey]
	        public string message { get; set; }

	        public string answer { get; set; }
		}
        private readonly List<words> dbCache;

		public Module_Answer() : base()
        {
            connection = new SQLiteConnection(GetConfigVariable("dbpath"));
            connection.CreateTable<words>();

            dbCache = connection.Table<words>().ToList();
        }

        public void ReceiveMessage(Message msg)
        {
            // ignoring triggers
            if (msg.Text.IndexOf('/', 0, 1) >= 0)
                return;

            try
            {
	            var words = dbCache.Where(x => x.message == msg.Text.ToLower()).ToArray();
	            if (words.Length > 0)
	            {
		            string result;
					if (words.Length == 1)
		            {
			            result = words[0].answer;
		            }
		            else
					{
						result = words[RNG.Next(0, words.Length)].answer;
					}

		            // for backward compatibility with old base 
		            result = result.Replace("%username%", msg.From.FirstName);

					API.SendMessage(result, msg.Chat);
				}
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }
    }
}
