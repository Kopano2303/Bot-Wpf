using System;
using System.Collections.Generic;
using System.Text;

namespace WpfBot
{
    internal class ActivityItem
    {
        public string Message { get; set; }
        public DateTime Time { get; set; }

        public override string ToString()
        {
            return $"[{Time:HH:mm}] {Message}";
        }
    }
}