namespace WpfBot
{
    internal class CbBot
    {
        public string Name;

        private string userName = "";
        private Random random = new Random();

        private List<TaskItem> tasks = new List<TaskItem>();

        private List<Question> quizQuestions = new List<Question>();
        private int currentQuestionIndex = 0;
        private int score = 0;
        private bool quizActive = false;

        private List<ActivityItem> activityLog = new List<ActivityItem>();

        public CbBot(string name)
        {
            Name = name;
            AddActivity("CyberBot started");
        }

        private void AddActivity(string message)
        {
            activityLog.Add(new ActivityItem
            {
                Message = message,
                Time = DateTime.Now
            });

            if (activityLog.Count > 10)
                activityLog.RemoveAt(0);
        }

        private string ShowActivityLog()
        {
            string log = "Recent Activity (Last 10)\n\n";

            foreach (var item in activityLog)
                log += "• " + item + "\n";

            return log;
        }

        private void LoadQuiz()
        {
            quizQuestions = new List<Question>
            {
                new Question
                {
                    Text = "What is phishing?",
                    Options = new[] { "Firewall", "Scam to steal data", "Backup", "Encryption" },
                    CorrectAnswer = "B",
                    Explanation = "Phishing tricks users into giving sensitive info."
                },
                new Question
                {
                    Text = "True or False: 123456 is strong password.",
                    Options = new[] { "True", "False" },
                    CorrectAnswer = "B",
                    Explanation = "It is extremely weak."
                },
                new Question
                {
                    Text = "What is malware?",
                    Options = new[] { "Good software", "Malicious software", "Firewall", "VPN" },
                    CorrectAnswer = "B",
                    Explanation = "Malware is harmful software."
                },
                new Question
                {
                    Text = "What does 2FA mean?",
                    Options = new[] { "Two-Factor Authentication", "Fast Access", "File Access", "Firewall" },
                    CorrectAnswer = "A",
                    Explanation = "Extra security layer."
                },
                new Question
                {
                    Text = "What should you do with phishing emails?",
                    Options = new[] { "Click", "Reply", "Report", "Ignore forever" },
                    CorrectAnswer = "C",
                    Explanation = "Reporting helps protect users."
                },
                new Question
                {
                    Text = "True or False: Public Wi-Fi is safe.",
                    Options = new[] { "True", "False" },
                    CorrectAnswer = "B",
                    Explanation = "Public Wi-Fi is risky."
                },
                new Question
                {
                    Text = "Firewall purpose?",
                    Options = new[] { "Blocks traffic", "Deletes files", "Encrypts", "Emails" },
                    CorrectAnswer = "A",
                    Explanation = "Filters network traffic."
                },
                new Question
                {
                    Text = "Social engineering?",
                    Options = new[] { "Hacking", "Tricking people", "Coding", "Encrypting" },
                    CorrectAnswer = "B",
                    Explanation = "Manipulating humans."
                },
                new Question
                {
                    Text = "Encryption?",
                    Options = new[] { "Delete", "Hide data", "Scan", "Backup" },
                    CorrectAnswer = "B",
                    Explanation = "Makes data unreadable."
                },
                new Question
                {
                    Text = "Reusing passwords safe?",
                    Options = new[] { "True", "False" },
                    CorrectAnswer = "B",
                    Explanation = "It is unsafe."
                }
            };
        }

        private string GetQuestion()
        {
            if (currentQuestionIndex >= quizQuestions.Count)
            {
                quizActive = false;

                AddActivity("Quiz completed");

                return $"Quiz Complete!\nScore: {score}/{quizQuestions.Count}\n" +
                       (score >= 8 ? "Great job!" : "Keep learning.");
            }

            var q = quizQuestions[currentQuestionIndex];

            string options = "";
            char label = 'A';

            foreach (var opt in q.Options)
                options += label++ + ") " + opt + "\n";

            return $"Q{currentQuestionIndex + 1}: {q.Text}\n\n{options}";
        }

        private bool IsTaskRequest(string input)
        {
            return input.Contains("task") || input.Contains("add task");
        }

        private bool IsReminderRequest(string input)
        {
            return input.Contains("remind");
        }

        private (string title, int? minutes) ParseInput(string input)
        {
            string text = input.ToLower();

            text = text.Replace("remind me to", "")
                       .Replace("add task to", "")
                       .Replace("create task", "")
                       .Trim();

            int? minutes = null;

            if (text.Contains("tomorrow"))
                minutes = 1440;
            else if (text.Contains("today"))
                minutes = 0;

            text = text.Replace("tomorrow", "")
                       .Replace("today", "")
                       .Trim();

            return (text, minutes);
        }

        public string GetResponse(string input)
        {
            input = input.ToLower().Trim();

            if (quizActive)
            {
                var q = quizQuestions[currentQuestionIndex];

                bool correct = input.ToUpper() == q.CorrectAnswer;

                if (correct)
                    score++;

                AddActivity(correct ? "Quiz correct" : "Quiz wrong");

                string feedback = correct
                    ? "Correct! " + q.Explanation
                    : "Wrong. " + q.Explanation;

                currentQuestionIndex++;

                return feedback + "\n\n" + GetQuestion();
            }

            if (IsReminderRequest(input))
            {
                var p = ParseInput(input);

                tasks.Add(new TaskItem
                {
                    Title = p.title,
                    Description = "Reminder task",
                    ReminderTime = DateTime.Now.AddMinutes(p.minutes ?? 10),
                    Completed = false
                });

                AddActivity("Reminder created: " + p.title);

                return $"Reminder set for: {p.title}";
            }

            if (IsTaskRequest(input))
            {
                var p = ParseInput(input);

                tasks.Add(new TaskItem
                {
                    Title = p.title,
                    Description = "Auto task",
                    ReminderTime = null,
                    Completed = false
                });

                AddActivity("Task created: " + p.title);

                return $"Task added: {p.title}";
            }

            if (input.Contains("quiz"))
            {
                LoadQuiz();
                quizActive = true;
                currentQuestionIndex = 0;
                score = 0;

                AddActivity("Quiz started");

                return GetQuestion();
            }

            if (input.Contains("show tasks"))
            {
                string result = "Tasks:\n\n";

                foreach (var t in tasks)
                    result += "• " + t + "\n";

                return result;
            }

            if (input.Contains("activity log") ||
                input.Contains("what have you done"))
            {
                AddActivity("Activity log viewed");
                return ShowActivityLog();
            }

            if (input.Contains("hello") || input.Contains("hi"))
                return "Hello! I am CyberBot.";

            if (input.Contains("password"))
                return "Use strong passwords.";

            if (input.Contains("phishing"))
                return "Phishing is a scam attack.";

            AddActivity("Unknown input");

            return "Try: quiz, task, remind, activity log";
        }
    }
}

