// Skybot 2013-2017

using System;
using System.Text.RegularExpressions;
using den0bot.Util;
using Telegram.Bot.Types;

namespace den0bot.Modules.Skybot
{
	public class Module_Roll : IModule, IReceiveAllMessages
	{
		public void ReceiveMessage(Message msg)
		{
			Match match = Regex.Match(msg.Text, @"(^\d)d(\d+$)");
			if (match.Success)
			{
				try
				{
					if (short.Parse(match.Groups[1].Value) <= 10) // ограничения: максимум роллов - 10, максимальное число рандома - int.MaxValue
					{
						string result = string.Empty;
						for (int i = 1; i <= short.Parse(match.Groups[1].Value);)
						{
							string diceresult = RNG.Next(1, int.Parse(match.Groups[2].Value) + 1).ToString();
							result += i + " ролл, " + "результат: " + diceresult + "\n";
							i++;
						}

						API.SendMessage(result, msg.Chat);
					}
					else
					{
						API.SendMessage("Не знаю таких чисел.", msg.Chat);
					}
				}
				catch (Exception e)
				{
					Log.Error(e.Message);
					API.SendMessage("Нихуя ты загнул.", msg.Chat);
				}
			}
		}
	}
}
