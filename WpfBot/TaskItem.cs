using System;

namespace WpfBot
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? ReminderTime { get; set; }
        public bool Completed { get; set; }

        public override string ToString()
        {
            string reminder = "No Reminder";
            if (ReminderTime.HasValue)
                reminder = ReminderTime.Value.ToString("dd/MM/yyyy HH:mm");

            string status = "Pending";
            if (Completed)
                status = "Completed";

            return Title + "\nDescription: " + Description + "\nReminder: " + reminder + "\nStatus: " + status;
        }
    }
}