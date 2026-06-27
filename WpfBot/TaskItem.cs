using System;
using System.Collections.Generic;
using System.Text;

namespace WpfBot
{
    public class TaskItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? ReminderTime { get; set; }
        public bool Completed { get; set; }

        public override string ToString()
        {
            string reminder = ReminderTime.HasValue
                ? $" (Reminder: {ReminderTime.Value})"
                : "";

            return $"{Title} - {Description}{reminder} - {(Completed ? "Done" : "Pending")}";
        }
    }
}