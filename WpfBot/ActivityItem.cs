using System;

namespace WpfBot
{
    public class ActivityItem
    {
        public DateTime Time { get; set; }
        public string Message { get; set; }

        public ActivityItem()
        {
            Time = DateTime.Now;
        }

        public ActivityItem(string message)
        {
            Time = DateTime.Now;
            Message = message;
        }

        public override string ToString()
        {
            return "[" + Time.ToString("dd/MM/yyyy HH:mm:ss") + "] " + Message;
        }
    }
}