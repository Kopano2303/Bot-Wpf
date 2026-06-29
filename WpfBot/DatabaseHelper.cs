using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace WpfBot
{
    public class DatabaseHelper
    {
        private string connectionString = "server=localhost;database=CyberBotDB;uid=root;pwd=;";

        public bool AddTask(TaskItem task)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string sql = "INSERT INTO Tasks (Title, Description, ReminderTime, Completed) VALUES (@title, @description, @reminder, @completed)";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@title", task.Title);
                    cmd.Parameters.AddWithValue("@description", task.Description);

                    if (task.ReminderTime.HasValue)
                        cmd.Parameters.AddWithValue("@reminder", task.ReminderTime.Value);
                    else
                        cmd.Parameters.AddWithValue("@reminder", DBNull.Value);

                    cmd.Parameters.AddWithValue("@completed", task.Completed);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<TaskItem> GetTasks()
        {
            List<TaskItem> tasks = new List<TaskItem>();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string sql = "SELECT * FROM Tasks";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        TaskItem task = new TaskItem();
                        task.Title = reader["Title"].ToString();
                        task.Description = reader["Description"].ToString();
                        task.Completed = Convert.ToBoolean(reader["Completed"]);

                        if (reader["ReminderTime"] != DBNull.Value)
                            task.ReminderTime = Convert.ToDateTime(reader["ReminderTime"]);

                        tasks.Add(task);
                    }
                }
            }
            catch (Exception)
            {
            }

            return tasks;
        }

        public TaskItem GetTask(string title)
        {
            TaskItem task = null;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string sql = "SELECT * FROM Tasks WHERE Title = @title";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@title", title);

                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        task = new TaskItem();
                        task.Title = reader["Title"].ToString();
                        task.Description = reader["Description"].ToString();
                        task.Completed = Convert.ToBoolean(reader["Completed"]);

                        if (reader["ReminderTime"] != DBNull.Value)
                            task.ReminderTime = Convert.ToDateTime(reader["ReminderTime"]);
                    }
                }
            }
            catch (Exception)
            {
            }

            return task;
        }

        public bool CompleteTask(string title)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string sql = "UPDATE Tasks SET Completed = 1 WHERE Title = @title";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@title", title);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateReminder(string title, DateTime reminder)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string sql = "UPDATE Tasks SET ReminderTime = @reminder WHERE Title = @title";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@title", title);
                    cmd.Parameters.AddWithValue("@reminder", reminder);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteTask(string title)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string sql = "DELETE FROM Tasks WHERE Title = @title";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@title", title);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}