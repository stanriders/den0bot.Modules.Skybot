using den0bot.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Telegram.Bot.Types;

namespace den0bot.Modules.Skybot
{
    public class Module_Notepad : IModule
    {
        private Dictionary<long, List<string>> notepadList = new();

        public Module_Notepad()
        {
            AddCommand(new Command
            {
                Names = { "np", "notepad" },
                Action = Notepad
            });
        }

        private ICommandAnswer Notepad(Message msg)
        {
            if (Regex.IsMatch(msg.Text, @"n(?:ote)?p(?:ad)?\s+(\d+)$"))
            {
                return new TextCommandAnswer(ProcessNotepadRead(
                    int.Parse(Regex.Match(msg.Text, @"n(?:ote)?p(?:ad)?\s+(\d+)$", RegexOptions.IgnoreCase).Groups[1].ToString()),
                    msg.Chat.Id));
            }
            else if (Regex.IsMatch(msg.Text, @"n(?:ote)?p(?:ad)?\s+list$"))
            {
                return new TextCommandAnswer(ProcessNotepadList(msg.Chat.Id));
            }
            else if (Regex.IsMatch(msg.Text, @"^n(ote)?p(ad)?\s+"))
            {
                string message = new Regex(@"^n(ote)?p(ad)?\s+").Replace(msg.Text, string.Empty);
                return new TextCommandAnswer(ProcessNotepadWrite(message, msg.From.FirstName, msg.Chat.Id));
            }

            return null;
        }

        public string ProcessNotepadList(long chat)
        {
            if (notepadList[chat].Count == 0)
                return "Соник еще не успел заспамить, потому записей нет.";
            else
                return $"Всего найдено {notepadList[chat].Count} записей. Читай любую.";
        }

        private string ProcessNotepadRead(int arrIndex, long chat)
        {
            if (arrIndex == 0)
                return "Не пытайся меня обкурячить, пиши нормальное число!";

            if (notepadList.Keys.All(x => x.ToString() != chat.ToString()))
                return "Ваще голяк в базе по этому чату.";

            if (notepadList[chat].Count > (arrIndex - 1))
                return notepadList[chat][arrIndex - 1];
            else
                return "Ты охуел, нет столько записей!";
        }

        private string ProcessNotepadWrite(string str, string user, long chat)
        {
            string text = "Добавлено " + DateTime.Now.ToShortDateString() + " в " + DateTime.Now.ToLongTimeString() + " пользователем " + user + "\n" + str;

            //SQLHandler sqlHand = new SQLHandler(_dbFolderPath);

            //sqlHand.SetString(chat.ToString(), "TEXT", text);

            if (notepadList.Keys.Any(x => x.ToString() == chat.ToString()))
                notepadList[chat].Add(text);
            else
                notepadList.Add(chat, new List<string> { text });

            return "Записал под номером " + (notepadList[chat].Count) + ", такие дела";
        }
    }
}
