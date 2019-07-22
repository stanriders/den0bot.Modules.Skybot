// Skybot 2013-2017

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using den0bot.Util;
using SQLite;
using Telegram.Bot.Types;

namespace den0bot.Modules.Skybot
{
    // Main answering module
    public class Module_Answer : IModule, IReceiveAllMessages
    {
        private readonly SQLiteConnection connection;
        private readonly Dictionary<long, DateTime> nextPost = new Dictionary<long, DateTime>(); // chatID, time
        private const int cooldown = 5; // minutes

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

		public async Task ReceiveMessage(Message msg)
		{
			if (!nextPost.ContainsKey(msg.Chat.Id))
				nextPost.Add(msg.Chat.Id, DateTime.Now);

			if (nextPost[msg.Chat.Id] < DateTime.Now)
			{
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
