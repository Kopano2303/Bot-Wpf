using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace WpfBot
{
    public class CbBot
    {
        public string Name { get; }
        public TaskItem PendingTask { get; set; }

        private List<TaskItem> tasks = new List<TaskItem>();
        private List<ActivityItem> activityLog = new List<ActivityItem>();

        private List<Question> quizQuestions = new List<Question>();
        private int currentQuestionIndex = 0;
        private int score = 0;
        private bool quizActive = false;

        private string userName = "";

        public CbBot(string name)
        {
            Name = name;
            LogAction("CyberBot started");
        }

        public void LogAction(string message)
        {
            activityLog.Add(new ActivityItem(message));
            if (activityLog.Count > 10)
                activityLog.RemoveAt(0);
        }

        public List<ActivityItem> GetActivityLog()
        {
            return new List<ActivityItem>(activityLog);
        }

        private string ShowActivityLog()
        {
            if (activityLog.Count == 0)
                return "No activity recorded yet.";

            string log = "Recent Activity (last 10):\n\n";
            int n = 1;
            foreach (ActivityItem item in activityLog)
            {
                log += "  " + n + ". " + item.ToString() + "\n";
                n++;
            }
            return log;
        }

        private bool IsGreeting(string s)
        {
            return s.Contains("hello") || s.Contains("hi") || s.Contains("hey")
                || s.Contains("good morning") || s.Contains("good afternoon");
        }

        private bool IsTaskIntent(string s)
        {
            return s.Contains("add task") || s.Contains("create task") || s.Contains("new task")
                || (s.Contains("task") && (s.Contains("add") || s.Contains("create") || s.Contains("make")));
        }

        private bool IsReminderIntent(string s)
        {
            return s.Contains("remind") || s.Contains("reminder") || s.Contains("set reminder")
                || s.Contains("dont forget") || s.Contains("don't forget");
        }

        private bool IsShowTasksIntent(string s)
        {
            return s.Contains("show task") || s.Contains("list task") || s.Contains("view task")
                || s.Contains("my tasks") || s.Contains("all tasks");
        }

        private bool IsActivityLogIntent(string s)
        {
            return s.Contains("activity log") || s.Contains("what have you done")
                || s.Contains("recent actions") || s.Contains("show log")
                || s.Contains("history");
        }

        private bool IsQuizIntent(string s)
        {
            return s.Contains("quiz") || s.Contains("test me") || s.Contains("test my knowledge")
                || s.Contains("start game") || s.Contains("mini game") || s.Contains("play game");
        }

        private bool IsPasswordTopic(string s)
        {
            return s.Contains("password") || s.Contains("passphrase") || s.Contains("credentials");
        }

        private bool IsPhishingTopic(string s)
        {
            return s.Contains("phishing") || s.Contains("phish") || s.Contains("scam email")
                || s.Contains("suspicious email") || s.Contains("fake email");
        }

        private bool IsMalwareTopic(string s)
        {
            return s.Contains("malware") || s.Contains("virus") || s.Contains("ransomware")
                || s.Contains("trojan") || s.Contains("spyware");
        }

        private bool Is2FATopic(string s)
        {
            return s.Contains("2fa") || s.Contains("two factor") || s.Contains("two-factor")
                || s.Contains("multi factor") || s.Contains("mfa");
        }

        private bool IsVPNTopic(string s)
        {
            return s.Contains("vpn") || s.Contains("virtual private network") || s.Contains("public wifi")
                || s.Contains("public wi-fi");
        }

        private bool IsFirewallTopic(string s)
        {
            return s.Contains("firewall");
        }

        private bool IsSocialEngineeringTopic(string s)
        {
            return s.Contains("social engineering") || s.Contains("pretexting") || s.Contains("vishing");
        }

        private bool IsEncryptionTopic(string s)
        {
            return s.Contains("encrypt") || s.Contains("decryption") || s.Contains("cipher");
        }

        private bool IsHelpIntent(string s)
        {
            return s.Contains("help") || s.Contains("what can you do") || s.Contains("commands");
        }

        private bool IsNameSetIntent(string s)
        {
            return s.Contains("my name is") || s.Contains("call me") || s.Contains("i am ");
        }

        private string ExtractTaskTitle(string raw)
        {
            string[] strips = new string[]
            {
                "add task to", "add task", "create task to", "create task",
                "new task to", "new task", "remind me to", "remind me",
                "set a reminder to", "set reminder to", "add a reminder to",
                "add reminder to", "i need to", "i want to",
                "can you remind me to", "please remind me to"
            };

            string text = raw.ToLower();
            foreach (string s in strips)
                text = text.Replace(s, "");

            text = Regex.Replace(text, @"\b(tomorrow|today|tonight|in \d+ days?|next week|in a week)\b", "", RegexOptions.IgnoreCase);
            text = text.Trim(' ', ',', '.', '-');

            if (text.Length > 0)
                text = char.ToUpper(text[0]) + text.Substring(1);

            if (string.IsNullOrWhiteSpace(text))
                return "New cybersecurity task";

            return text;
        }

        private DateTime? ExtractReminderTime(string s)
        {
            if (s.Contains("tomorrow") || s.Contains("1 day") || s.Contains("one day"))
                return DateTime.Now.AddDays(1);
            if (s.Contains("3 days") || s.Contains("three days"))
                return DateTime.Now.AddDays(3);
            if (s.Contains("7 days") || s.Contains("week") || s.Contains("seven days"))
                return DateTime.Now.AddDays(7);
            if (s.Contains("14 days") || s.Contains("two weeks"))
                return DateTime.Now.AddDays(14);
            if (s.Contains("30 days") || s.Contains("month"))
                return DateTime.Now.AddDays(30);
            if (s.Contains("today") || s.Contains("tonight"))
                return DateTime.Now.AddHours(6);

            Match m = Regex.Match(s, @"in (\d+)\s*(day|hour|minute|min)s?");
            if (m.Success)
            {
                int val = int.Parse(m.Groups[1].Value);
                string unit = m.Groups[2].Value;
                if (unit.StartsWith("day")) return DateTime.Now.AddDays(val);
                if (unit.StartsWith("hour")) return DateTime.Now.AddHours(val);
                return DateTime.Now.AddMinutes(val);
            }

            return null;
        }

        private string ExtractName(string s)
        {
            string[] patterns = new string[]
            {
                @"my name is ([a-zA-Z]+)",
                @"call me ([a-zA-Z]+)",
                @"i am ([a-zA-Z]+)"
            };

            foreach (string pat in patterns)
            {
                Match m = Regex.Match(s, pat, RegexOptions.IgnoreCase);
                if (m.Success)
                    return m.Groups[1].Value;
            }

            return "";
        }

        private void LoadQuiz()
        {
            quizQuestions = new List<Question>();

            quizQuestions.Add(new Question("What is phishing?",
                new string[] { "A) Firewall", "B) Scam to steal data", "C) Backup", "D) Encryption" },
                "B", "Phishing tricks users into giving sensitive info via fake messages."));

            quizQuestions.Add(new Question("True or False: '123456' is a strong password.",
                new string[] { "A) True", "B) False" },
                "B", "It is extremely weak and commonly cracked within seconds."));

            quizQuestions.Add(new Question("What is malware?",
                new string[] { "A) Good software", "B) Malicious software", "C) Firewall", "D) VPN" },
                "B", "Malware is harmful software designed to damage or gain access."));

            quizQuestions.Add(new Question("What does 2FA mean?",
                new string[] { "A) Two-Factor Authentication", "B) Fast Access", "C) File Access", "D) Firewall" },
                "A", "An extra security layer beyond just a password."));

            quizQuestions.Add(new Question("What should you do with phishing emails?",
                new string[] { "A) Click links", "B) Reply", "C) Report it", "D) Ignore forever" },
                "C", "Reporting helps protect all users from the threat."));

            quizQuestions.Add(new Question("True or False: Public Wi-Fi is safe for banking.",
                new string[] { "A) True", "B) False" },
                "B", "Public Wi-Fi is unencrypted. Use a VPN for sensitive tasks."));

            quizQuestions.Add(new Question("What does a firewall do?",
                new string[] { "A) Filters network traffic", "B) Deletes files", "C) Encrypts data", "D) Sends emails" },
                "A", "It monitors and controls incoming and outgoing network traffic."));

            quizQuestions.Add(new Question("What is social engineering?",
                new string[] { "A) Hacking code", "B) Tricking people into revealing info", "C) Writing apps", "D) Encrypting data" },
                "B", "It exploits human trust rather than technical vulnerabilities."));

            quizQuestions.Add(new Question("What does encryption do?",
                new string[] { "A) Deletes data", "B) Makes data unreadable to others", "C) Scans for viruses", "D) Backs up files" },
                "B", "Encryption scrambles data using a key, protecting it in transit and at rest."));

            quizQuestions.Add(new Question("True or False: Reusing passwords across sites is safe.",
                new string[] { "A) True", "B) False" },
                "B", "One breached site can compromise all accounts sharing that password."));
        }

        private string GetQuizQuestion()
        {
            if (currentQuestionIndex >= quizQuestions.Count)
            {
                quizActive = false;
                LogAction("Quiz completed. Score: " + score + "/" + quizQuestions.Count);

                string grade = "";
                if (score >= 9)
                    grade = "Excellent work! You have a strong understanding of cybersecurity.";
                else if (score >= 7)
                    grade = "Good job! Keep practising to improve further.";
                else
                    grade = "Keep studying. Cybersecurity knowledge is important!";

                return "Quiz Complete!\nScore: " + score + "/" + quizQuestions.Count + "\n\n" + grade;
            }

            Question q = quizQuestions[currentQuestionIndex];
            string opts = "";

            foreach (string o in q.Options)
                opts += "  " + o + "\n";

            return "Q" + (currentQuestionIndex + 1) + ": " + q.Text + "\n\n" + opts + "(Reply A / B / C / D)";
        }

        public string GetResponse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "I did not catch that. Could you try again?";

            string lower = input.ToLower().Trim();
            PendingTask = null;

            if (quizActive)
            {
                Question q = quizQuestions[currentQuestionIndex];
                string letter = lower.ToUpper().Trim();

                if (letter.Length == 0)
                    return "Please reply with A, B, C, or D.";

                string answer = letter.Substring(0, 1);
                bool ok = answer == q.CorrectAnswer.ToUpper();

                if (ok)
                    score++;

                LogAction(ok ? "Quiz: correct answer" : "Quiz: wrong answer");

                string feedback = "";
                if (ok)
                    feedback = "Correct! " + q.Explanation;
                else
                    feedback = "Wrong. The answer was " + q.CorrectAnswer + ". " + q.Explanation;

                currentQuestionIndex++;
                return feedback + "\n\n" + GetQuizQuestion();
            }

            if (IsGreeting(lower))
            {
                string greet = "";
                if (!string.IsNullOrEmpty(userName))
                    greet = ", " + userName;
                LogAction("Greeted user");
                return "Hello" + greet + "! I am CyberBot. How can I help you with cybersecurity today?";
            }

            if (IsNameSetIntent(lower))
            {
                string name = ExtractName(lower);
                if (!string.IsNullOrEmpty(name))
                {
                    userName = name;
                    LogAction("User identified as: " + name);
                    return "Nice to meet you, " + name + "! How can I help you with cybersecurity today?";
                }
            }

            if (IsHelpIntent(lower))
            {
                LogAction("Help requested");
                return "Here is what I can do:\n\n" +
                       "  - Chat: Ask me about phishing, passwords, malware, VPNs, firewalls, encryption, 2FA, or social engineering.\n" +
                       "  - Tasks: Use the Tasks tab to add, complete, or delete cybersecurity tasks with optional reminders.\n" +
                       "  - Quiz: Use the Quiz tab or type 'start quiz' to test your cybersecurity knowledge.\n" +
                       "  - Activity Log: Type 'show activity log' or visit the Activity Log tab.\n\n" +
                       "You can also type things like:\n" +
                       "  - 'Remind me to enable 2FA tomorrow'\n" +
                       "  - 'Add task: review privacy settings'\n" +
                       "  - 'What have you done for me?'";
            }

            if (IsActivityLogIntent(lower))
            {
                LogAction("Activity log viewed");
                return ShowActivityLog();
            }

            if (IsQuizIntent(lower))
            {
                LoadQuiz();
                quizActive = true;
                currentQuestionIndex = 0;
                score = 0;
                LogAction("Quiz started via chat");
                return "Starting the quiz! Answer each question with A, B, C, or D.\n\n" + GetQuizQuestion();
            }

            if (IsReminderIntent(lower))
            {
                string title = ExtractTaskTitle(lower);
                DateTime? reminder = ExtractReminderTime(lower);

                TaskItem task = new TaskItem();
                task.Title = title;
                task.Description = "Reminder task";
                task.ReminderTime = reminder;
                task.Completed = false;

                PendingTask = task;

                string when = "";
                if (reminder != null)
                    when = "on " + reminder.Value.ToString("dd/MM/yyyy") + " at " + reminder.Value.ToString("HH:mm");
                else
                    when = "without a specific date. You can set one in the Tasks tab";

                LogAction("Reminder set: " + title);
                return "Reminder set for '" + title + "' " + when + ". It has been saved to your tasks.";
            }

            if (IsTaskIntent(lower))
            {
                string title = ExtractTaskTitle(lower);
                DateTime? reminder = ExtractReminderTime(lower);

                TaskItem task = new TaskItem();
                task.Title = title;
                task.Description = "Added via chat";
                task.ReminderTime = reminder;
                task.Completed = false;

                PendingTask = task;

                string extra = "";
                if (reminder != null)
                    extra = " Reminder set for " + reminder.Value.ToString("dd/MM/yyyy") + ".";
                else
                    extra = " No reminder set. Would you like one? Say 'remind me in X days'.";

                LogAction("Task added via chat: " + title);
                return "Task added: '" + title + "'." + extra;
            }

            if (IsShowTasksIntent(lower))
            {
                LogAction("Tasks listed via chat");
                if (tasks.Count == 0)
                    return "You have no tasks yet. Head to the Tasks tab or say 'add task: name' to create one.";

                string result = "Your current tasks:\n\n";
                int n = 1;
                foreach (TaskItem t in tasks)
                {
                    string status = t.Completed ? "Completed" : "Pending";
                    result += "  " + n + ". " + t.Title + " - " + status + "\n";
                    n++;
                }
                return result;
            }

            if (IsPasswordTopic(lower))
            {
                LogAction("Password topic discussed");
                return "Password Tips:\n" +
                       "  - Use at least 12 characters with a mix of letters, numbers and symbols.\n" +
                       "  - Never reuse passwords across different sites.\n" +
                       "  - Use a password manager like Bitwarden or 1Password.\n" +
                       "  - Enable 2FA wherever possible.";
            }

            if (IsPhishingTopic(lower))
            {
                LogAction("Phishing topic discussed");
                return "Phishing Awareness:\n" +
                       "  - Phishing emails impersonate trusted sources to steal credentials or install malware.\n" +
                       "  - Check the sender's actual email address, not just the display name.\n" +
                       "  - Hover over links before clicking to verify the URL.\n" +
                       "  - Report suspicious emails to your IT department or email provider.";
            }

            if (IsMalwareTopic(lower))
            {
                LogAction("Malware topic discussed");
                return "Malware Protection:\n" +
                       "  - Keep your operating system and applications fully updated.\n" +
                       "  - Use reputable antivirus software.\n" +
                       "  - Avoid downloading software from untrusted sources.\n" +
                       "  - Ransomware encrypts your files and demands payment. Back up your data regularly.";
            }

            if (Is2FATopic(lower))
            {
                LogAction("2FA topic discussed");
                return "Two-Factor Authentication (2FA):\n" +
                       "  - 2FA requires a second proof of identity beyond your password.\n" +
                       "  - Common methods include an authenticator app, SMS, or email code.\n" +
                       "  - Enable 2FA on email, banking and social media accounts.\n" +
                       "  - Authenticator apps like Google Authenticator are more secure than SMS.";
            }

            if (IsVPNTopic(lower))
            {
                LogAction("VPN topic discussed");
                return "VPN and Public Wi-Fi:\n" +
                       "  - Public Wi-Fi networks are unencrypted and anyone nearby can intercept traffic.\n" +
                       "  - A VPN encrypts your connection and hides your activity.\n" +
                       "  - Avoid banking or logging into sensitive sites on public Wi-Fi without a VPN.\n" +
                       "  - Choose a reputable no-log VPN provider.";
            }

            if (IsFirewallTopic(lower))
            {
                LogAction("Firewall topic discussed");
                return "Firewalls:\n" +
                       "  - A firewall monitors and filters incoming and outgoing network traffic.\n" +
                       "  - It blocks unauthorised access while allowing legitimate connections.\n" +
                       "  - Both hardware firewalls in routers and software firewalls in your OS are important.\n" +
                       "  - Make sure your Windows Firewall or equivalent is always switched on.";
            }

            if (IsSocialEngineeringTopic(lower))
            {
                LogAction("Social engineering topic discussed");
                return "Social Engineering:\n" +
                       "  - Social engineering manipulates people rather than exploiting software.\n" +
                       "  - Common tactics include pretexting, baiting and voice phishing.\n" +
                       "  - Always verify the identity of anyone requesting sensitive information.\n" +
                       "  - Legitimate companies will never ask for your password by email or phone.";
            }

            if (IsEncryptionTopic(lower))
            {
                LogAction("Encryption topic discussed");
                return "Encryption:\n" +
                       "  - Encryption converts data into an unreadable format using a key.\n" +
                       "  - HTTPS encrypts web traffic. Always look for the padlock in your browser.\n" +
                       "  - Full-disk encryption like BitLocker protects your data if your device is stolen.\n" +
                       "  - Use end-to-end encrypted messaging apps like Signal for sensitive conversations.";
            }

            LogAction("Unrecognised input");
            return "I am not sure I understood that. Here are some things you can try:\n" +
                   "  - 'Add task: Enable 2FA'\n" +
                   "  - 'Remind me to update my password in 3 days'\n" +
                   "  - 'Tell me about phishing'\n" +
                   "  - 'Start quiz'\n" +
                   "  - 'Show activity log'\n" +
                   "  - 'Help'";
        }
    }
}