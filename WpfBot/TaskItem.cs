using System;

namespace WpfBot
{
    internal class TaskItem
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime? ReminderTime { get; set; }

        public bool Completed { get; set; }

        public override string ToString()
        {
            string reminder = ReminderTime.HasValue
                ? ReminderTime.Value.ToString("dd/MM/yyyy HH:mm")
                : "No Reminder";

            return $"{Title}\nDescription: {Description}\nReminder: {reminder}\nStatus: {(Completed ? "Completed" : "Pending")}";
        }
    }
}