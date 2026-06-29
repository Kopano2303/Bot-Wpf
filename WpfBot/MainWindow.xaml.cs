using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfBot
{
    public partial class MainWindow : Window
    {
        private CbBot bot;
        private DatabaseHelper db;

        private List<Question> quizQuestions = new List<Question>();
        private int currentQuestionIndex = 0;
        private int quizScore = 0;
        private bool quizRunning = false;
        private bool waitingForNext = false;

        public MainWindow()
        {
            InitializeComponent();

            db = new DatabaseHelper();
            bot = new CbBot("CyberBot");

            ChatBox.Items.Add("CyberBot: Welcome to the Cybersecurity Awareness Chatbot!");
            ChatBox.Items.Add("CyberBot: Type 'help' to see what I can do.");

            LoadTaskList();
            LoadActivityLog();
        }

        private void InputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Send_Click(sender, null);
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            string input = InputBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(input))
                return;

            ChatBox.Items.Add("You: " + input);
            InputBox.Clear();

            string response = bot.GetResponse(input);

            if (bot.PendingTask != null)
            {
                db.AddTask(bot.PendingTask);
                bot.PendingTask = null;
                LoadTaskList();
            }

            ChatBox.Items.Add("CyberBot: " + response);
            ChatBox.ScrollIntoView(ChatBox.Items[ChatBox.Items.Count - 1]);

            LoadActivityLog();
            StatusText.Text = "Status: Response sent";
        }

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            string title = TaskTitleBox.Text.Trim();
            string desc = TaskDescBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("Please enter a task title.", "Missing Title",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(desc))
                desc = "Cybersecurity task";

            DateTime? reminderTime = null;
            int days = 0;

            if (ReminderCombo.SelectedIndex == 1) days = 1;
            else if (ReminderCombo.SelectedIndex == 2) days = 3;
            else if (ReminderCombo.SelectedIndex == 3) days = 7;
            else if (ReminderCombo.SelectedIndex == 4) days = 14;
            else if (ReminderCombo.SelectedIndex == 5) days = 30;

            if (days > 0)
                reminderTime = DateTime.Now.AddDays(days);

            TaskItem task = new TaskItem();
            task.Title = title;
            task.Description = desc;
            task.ReminderTime = reminderTime;
            task.Completed = false;

            bool saved = db.AddTask(task);

            if (saved)
            {
                string logMsg = "Task added: " + title;
                if (reminderTime != null)
                    logMsg += " (reminder in " + days + " days)";

                bot.LogAction(logMsg);

                TaskTitleBox.Clear();
                TaskDescBox.Clear();
                ReminderCombo.SelectedIndex = 0;

                LoadTaskList();
                LoadActivityLog();

                StatusText.Text = "Status: Task added";

                string chatReply = "Task added: \"" + title + "\". " + desc + ".";
                if (reminderTime != null)
                    chatReply += " Reminder set for " + reminderTime.Value.ToString("dd/MM/yyyy") + ".";

                ChatBox.Items.Add("CyberBot: " + chatReply);
            }
            else
            {
                MessageBox.Show("Could not save the task. Check your database connection.",
                    "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CompleteTask_Click(object sender, RoutedEventArgs e)
        {
            TaskViewModel selected = TaskListView.SelectedItem as TaskViewModel;

            if (selected == null)
            {
                MessageBox.Show("Please select a task first.", "No Task Selected",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            db.CompleteTask(selected.Title);
            bot.LogAction("Task completed: " + selected.Title);

            LoadTaskList();
            LoadActivityLog();

            StatusText.Text = "Status: Task marked complete";
            ChatBox.Items.Add("CyberBot: Task \"" + selected.Title + "\" has been marked as completed.");
        }

        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            TaskViewModel selected = TaskListView.SelectedItem as TaskViewModel;

            if (selected == null)
            {
                MessageBox.Show("Please select a task first.", "No Task Selected",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            MessageBoxResult confirm = MessageBox.Show(
                "Are you sure you want to delete \"" + selected.Title + "\"?",
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirm == MessageBoxResult.Yes)
            {
                db.DeleteTask(selected.Title);
                bot.LogAction("Task deleted: " + selected.Title);

                LoadTaskList();
                LoadActivityLog();

                StatusText.Text = "Status: Task deleted";
                ChatBox.Items.Add("CyberBot: Task \"" + selected.Title + "\" has been deleted.");
            }
        }

        private void LoadTaskList()
        {
            List<TaskItem> tasks = db.GetTasks();
            List<TaskViewModel> viewList = new List<TaskViewModel>();

            foreach (TaskItem t in tasks)
                viewList.Add(new TaskViewModel(t));

            TaskListView.ItemsSource = viewList;
        }

        private void StartQuiz_Click(object sender, RoutedEventArgs e)
        {
            SetupQuizQuestions();

            quizRunning = true;
            currentQuestionIndex = 0;
            quizScore = 0;
            waitingForNext = false;

            bot.LogAction("Quiz started");
            LoadActivityLog();

            StartQuizBtn.Content = "Restart Quiz";
            NextQuestionBtn.Visibility = Visibility.Collapsed;
            FeedbackBorder.Visibility = Visibility.Collapsed;

            ShowCurrentQuestion();
            StatusText.Text = "Status: Quiz in progress";
        }

        private void QuizOption_Click(object sender, RoutedEventArgs e)
        {
            if (!quizRunning || waitingForNext)
                return;

            Button btn = sender as Button;
            string chosen = btn.Content.ToString();
            string letter = chosen.Substring(0, 1).ToUpper();

            Question q = quizQuestions[currentQuestionIndex];
            bool correct = letter == q.CorrectAnswer.ToUpper();

            if (correct)
                quizScore++;

            if (correct)
            {
                FeedbackText.Text = "Correct! " + q.Explanation;
                FeedbackBorder.Background = new SolidColorBrush(Color.FromRgb(27, 94, 32));
                bot.LogAction("Quiz: answered correctly");
            }
            else
            {
                FeedbackText.Text = "Incorrect. The correct answer was " + q.CorrectAnswer + ". " + q.Explanation;
                FeedbackBorder.Background = new SolidColorBrush(Color.FromRgb(127, 0, 0));
                bot.LogAction("Quiz: answered incorrectly");
            }

            FeedbackBorder.Visibility = Visibility.Visible;
            QuizScoreText.Text = "Score: " + quizScore + " / " + quizQuestions.Count;

            waitingForNext = true;
            NextQuestionBtn.Visibility = Visibility.Visible;
        }

        private void NextQuestion_Click(object sender, RoutedEventArgs e)
        {
            currentQuestionIndex++;
            waitingForNext = false;
            FeedbackBorder.Visibility = Visibility.Collapsed;
            NextQuestionBtn.Visibility = Visibility.Collapsed;

            if (currentQuestionIndex >= quizQuestions.Count)
            {
                quizRunning = false;
                QuizStatusText.Text = "Quiz Complete";

                string result = "";
                if (quizScore >= 9)
                    result = "Excellent work! You have a strong understanding of cybersecurity.";
                else if (quizScore >= 7)
                    result = "Good job! Keep practising to improve further.";
                else if (quizScore >= 5)
                    result = "Not bad. Review the topics you got wrong and try again.";
                else
                    result = "Keep studying. Cybersecurity knowledge is important!";

                QuestionText.Text = "Final Score: " + quizScore + " / " + quizQuestions.Count + "\n\n" + result;
                OptionsPanel.ItemsSource = null;

                bot.LogAction("Quiz completed. Score: " + quizScore + "/" + quizQuestions.Count);
                LoadActivityLog();
                StatusText.Text = "Status: Quiz finished";

                return;
            }

            ShowCurrentQuestion();
        }

        private void ShowCurrentQuestion()
        {
            Question q = quizQuestions[currentQuestionIndex];

            QuizStatusText.Text = "Question " + (currentQuestionIndex + 1) + " of " + quizQuestions.Count;
            QuestionText.Text = q.Text;

            List<string> options = new List<string>();
            char label = 'A';
            foreach (string opt in q.Options)
            {
                options.Add(label + ") " + opt);
                label++;
            }

            OptionsPanel.ItemsSource = options;
        }

        private void SetupQuizQuestions()
        {
            quizQuestions = new List<Question>();

            quizQuestions.Add(new Question(
                "What is phishing?",
                new string[] { "Firewall software", "A scam to steal personal information", "A data backup tool", "A type of encryption" },
                "B",
                "Phishing is when attackers pretend to be trusted sources to steal information like passwords."));

            quizQuestions.Add(new Question(
                "True or False: The password '123456' is strong.",
                new string[] { "True", "False" },
                "B",
                "123456 is one of the most commonly used passwords and can be cracked in seconds."));

            quizQuestions.Add(new Question(
                "What is malware?",
                new string[] { "Useful software", "Software designed to cause harm", "A type of firewall", "A VPN service" },
                "B",
                "Malware is any software that is designed to damage systems or steal data."));

            quizQuestions.Add(new Question(
                "What does 2FA stand for?",
                new string[] { "Two-Factor Authentication", "Two File Access", "Transfer File Authentication", "Two Firewall Access" },
                "A",
                "Two-Factor Authentication adds a second step to logging in, making accounts much harder to hack."));

            quizQuestions.Add(new Question(
                "What should you do when you receive a suspicious email?",
                new string[] { "Click the links to check", "Reply asking who sent it", "Report it as phishing", "Forward it to friends" },
                "C",
                "Reporting phishing emails helps protect you and others from falling for the same scam."));

            quizQuestions.Add(new Question(
                "True or False: Public Wi-Fi is safe for online banking.",
                new string[] { "True", "False" },
                "B",
                "Public Wi-Fi is not encrypted so other people can intercept what you send. Use a VPN if you must connect."));

            quizQuestions.Add(new Question(
                "What is the main job of a firewall?",
                new string[] { "Monitor and filter network traffic", "Delete viruses from files", "Encrypt your hard drive", "Send automatic emails" },
                "A",
                "A firewall checks network traffic and blocks anything that looks unauthorised."));

            quizQuestions.Add(new Question(
                "What is social engineering?",
                new string[] { "Writing secure programs", "Tricking people into giving up information", "Building social media apps", "Encrypting messages" },
                "B",
                "Social engineering targets people rather than systems using tricks and manipulation."));

            quizQuestions.Add(new Question(
                "What does encryption do to data?",
                new string[] { "Deletes it permanently", "Makes it unreadable without the correct key", "Scans it for viruses", "Copies it to the cloud" },
                "B",
                "Encryption scrambles data so that only someone with the right key can read it."));

            quizQuestions.Add(new Question(
                "True or False: Using the same password for all your accounts is fine.",
                new string[] { "True", "False" },
                "B",
                "If one account is hacked, all your other accounts become vulnerable too."));

            quizQuestions.Add(new Question(
                "Which of the following is the strongest password?",
                new string[] { "password1", "12345678", "P@ssw0rd!2024#", "yourname99" },
                "C",
                "A strong password uses a mix of uppercase, lowercase, numbers and symbols."));

            quizQuestions.Add(new Question(
                "What is ransomware?",
                new string[] { "Software that speeds up your PC", "Malware that locks your files and demands payment", "A type of VPN", "An antivirus tool" },
                "B",
                "Ransomware encrypts your files and criminals demand money to unlock them. Always keep backups."));
        }

        private void RefreshLog_Click(object sender, RoutedEventArgs e)
        {
            LoadActivityLog();
            StatusText.Text = "Status: Log refreshed";
        }

        private void LoadActivityLog()
        {
            ActivityListBox.Items.Clear();

            List<ActivityItem> log = bot.GetActivityLog();

            int number = 1;
            foreach (ActivityItem item in log)
            {
                ActivityListBox.Items.Add(number + ". " + item.ToString());
                number++;
            }
        }
    }

    public class TaskViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ReminderDisplay { get; set; }
        public string StatusDisplay { get; set; }

        public TaskViewModel(TaskItem t)
        {
            Title = t.Title;
            Description = t.Description;

            if (t.ReminderTime != null)
                ReminderDisplay = t.ReminderTime.Value.ToString("dd/MM/yyyy HH:mm");
            else
                ReminderDisplay = "No reminder";

            if (t.Completed)
                StatusDisplay = "Completed";
            else
                StatusDisplay = "Pending";
        }
    }
}