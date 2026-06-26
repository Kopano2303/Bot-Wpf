using System;
using System.Collections.Generic;

namespace WpfBot
{

    internal class CbBot
    {

        public string Name;

        private string userName = "";
        private string favouriteTopic = "";
        private string lastTopic = "";

        private string mood = "";

        private Random random = new Random();

        private List<string> activityLog = new List<string>();

        public CbBot(string name)
        {
            Name = name;
            activityLog.Add("CyberBot started.");
        }

        private void AddActivity(string activity)
        {
            activityLog.Add(activity);

            if (activityLog.Count > 10)
            {
                activityLog.RemoveAt(0);
            }
        }

        private string ShowActivityLog()
        {
            string log = "Recent Activity\n\n";

            foreach (string item in activityLog)
            {
                log += "• " + item + "\n";
            }

            return log;
        }

        private string RandomResponse(string[] replies)
        {
            return replies[random.Next(replies.Length)];
        }

        public string GetResponse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return "I'm not sure I understand. Can you try rephrasing?";
            }

            input = input.ToLower().Trim();

            if (input.StartsWith("my name is "))
            {
                userName = input.Replace("my name is ", "");
                AddActivity("User name saved.");
                return "Nice to meet you " + userName + ". I'll remember your name.";
            }

            if (input == "hi" || input == "hello" || input == "hey")
            {
                AddActivity("Greeting");

                return userName == ""
                    ? "Hello! Welcome to CyberBot."
                    : "Welcome back " + userName + "! How can I help you today?";
            }

            if (input.Contains("worried") || input.Contains("scared"))
            {
                mood = "worried";
                AddActivity("Sentiment: Worried");

                string tip =
                    "Scammers often rely on urgency and fear.\n" +
                    "Never share personal info quickly, and always verify messages.";

                return "It's completely understandable to feel that way. Cyber threats can be overwhelming.\n\n" + tip;
            }

            if (input.Contains("frustrated") || input.Contains("angry"))
            {
                mood = "frustrated";
                AddActivity("Sentiment: Frustrated");

                return "I understand you're frustrated. Cybersecurity can be confusing at first.\n\nLet me help simplify it for you.";
            }

            if (input.Contains("curious"))
            {
                mood = "curious";
                AddActivity("Sentiment: Curious");

                return "Great mindset! Curiosity is important in cybersecurity. What topic would you like to explore?";
            }

            if (input.Contains("happy") || input.Contains("good") || input.Contains("great"))
            {
                mood = "happy";
                AddActivity("Sentiment: Happy");

                return "I'm glad you're feeling good! Let's keep building your cybersecurity knowledge.";
            }

            if (input.Contains("show activity log") || input.Contains("what have you done for me"))
            {
                AddActivity("Activity log viewed.");
                return ShowActivityLog();
            }

            if (input.Contains("password"))
            {
                lastTopic = "Password Security";
                favouriteTopic = "Password Security";
                AddActivity("Keyword: Password");

                return RandomResponse(new string[]
                {
                    "Use strong, unique passwords with letters, numbers, and symbols.",
                    "Never reuse passwords across different accounts.",
                    "A password manager can help store secure passwords safely."
                });
            }

            if (input.Contains("privacy"))
            {
                lastTopic = "Privacy";
                favouriteTopic = "Privacy";
                AddActivity("Keyword: Privacy");

                return RandomResponse(new string[]
                {
                    "Limit personal information shared online.",
                    "Adjust privacy settings on social media accounts.",
                    "Think before posting sensitive data."
                });
            }

            if (input.Contains("scam"))
            {
                AddActivity("Keyword: Scam");

                return RandomResponse(new string[]
                {
                    "Never trust messages asking for urgent money or data.",
                    "Always verify the sender before responding.",
                    "Scammers often pretend to be official organisations."
                });
            }

            if (input.Contains("phishing"))
            {
                lastTopic = "Phishing";
                favouriteTopic = "Phishing";
                AddActivity("Keyword: Phishing");

                return RandomResponse(new string[]
                {
                    "Check email addresses carefully before clicking links.",
                    "Do not enter passwords from email links.",
                    "Urgent messages are often phishing attempts."
                });
            }

            if (input.Contains("malware"))
            {
                lastTopic = "Malware";
                favouriteTopic = "Malware";
                AddActivity("Keyword: Malware");

                return RandomResponse(new string[]
                {
                    "Avoid downloading files from unknown sources.",
                    "Keep antivirus software updated.",
                    "Malware can steal or damage your data."
                });
            }

            if (input.Contains("continue") || input.Contains("tell me more") || input.Contains("explain more"))
            {
                if (lastTopic != "")
                {
                    AddActivity("Follow-up detected.");
                    return "Continuing " + lastTopic + ": Always stay alert and follow safe cybersecurity practices.";
                }

                return "Tell me a topic first so I can continue.";
            }

            if (input.Contains("show activity log"))
            {
                AddActivity("Activity log requested.");
                return ShowActivityLog();
            }

            if (input.Contains("help"))
            {
                AddActivity("Help opened.");

                return "Try asking about phishing, scams, passwords, privacy or malware.";
            }

            AddActivity("Unknown input");

            return "I'm not sure I understand. Can you try rephrasing?";
        }
    }
}

