using den0bot.Types;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Telegram.Bot.Types;

namespace den0bot.Modules.Skybot
{
	public class Module_Organizer : IModule
	{
		private class TData
		{
			public string Text { get; init; }
			public DateTime Date { get; init; }
			public long ChatID { get; init; }
			public bool Sent { get; set; }
		}

		private readonly List<TData> dataList = new();

		private readonly Regex regex =
			new(@"org(?:aniser)?\s+(?:(\d\d?):(\d\d?)\s?(\d\d?)?\.?(\d\d?)?\s+(.+)|(\d\d?):(\d\d?)\s+(.+))",
				RegexOptions.IgnoreCase | RegexOptions.Compiled);

		private DateTime lastCheckTime = DateTime.Now;
		private const int interval = 60; // seconds

		public Module_Organizer()
		{
			AddCommand(new Command
			{
				Names = {"org", "organizer"},
				Action = Add
			});
		}

		public override void Think()
		{
			if (dataList.Count > 0 && lastCheckTime < DateTime.Now)
			{
				foreach (var orgData in dataList)
				{
					if (orgData.Sent) continue;
					if (orgData.Date.ToShortDateString() != DateTime.Now.ToShortDateString()) continue;
					if (orgData.Date.ToShortTimeString() != DateTime.Now.ToShortTimeString()) continue;

					// we don't care if it'll send or not
					_ = API.SendMessage(orgData.Text, orgData.ChatID);

					orgData.Sent = true; // to prevent posting the same entry multiple times
				}

				lastCheckTime = DateTime.Now.AddSeconds(interval);
			}
		}

		private ICommandAnswer Add(Message msg)
		{
			Match mvOrg = regex.Match(msg.Text);

			if (mvOrg.Success)
			{
				try
				{
					DateTime date = new DateTime(
						DateTime.Now.Year,
						int.Parse(mvOrg.Groups[4].Value),
						int.Parse(mvOrg.Groups[3].Value),
						int.Parse(mvOrg.Groups[1].Value),
						int.Parse(mvOrg.Groups[2].Value), 0);

					dataList.Add(new TData
					{
						Text = mvOrg.Groups[5].Value,
						Date = date,
						ChatID = msg.Chat.Id
					});

					return new TextCommandAnswer("Добавлено напоминание в " + date.Hour + ":" + date.Minute + " " + date.Day + "." + date.Month);
				}
				catch (Exception)
				{
					try
					{
						DateTime date = new DateTime(
							DateTime.Now.Year,
							DateTime.Now.Month,
							DateTime.Now.Day,
							int.Parse(mvOrg.Groups[1].Value),
							int.Parse(mvOrg.Groups[2].Value), 0);

						dataList.Add(new TData
						{
							Text = mvOrg.Groups[5].Value,
							Date = date,
							ChatID = msg.Chat.Id,
						});

						return new TextCommandAnswer("Добавлено напоминание в " + date.Hour + ":" + date.Minute);
					}
					catch (Exception)
					{
						return new TextCommandAnswer("Напоминание не добавлено, ибо ты криворукий мудак.");
					}
				}
			}

			return null;
		}
	}
}