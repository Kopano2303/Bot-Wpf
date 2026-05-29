namespace WpfBot
{
    internal class CbBot
    {
        public string Name;

        public CbBot(string name)
        {
            Name = name;
        }

        public string GetResponse(string input)
        {
            input = input.ToLower().Trim();

            if (input.Contains("phishing"))
            {
                return
                    "Definition:\n" +
                    "Phishing is when attackers trick you into giving sensitive information like passwords or bank details.\n\n" +

                    "Example:\n" +
                    "You receive an email that looks like your bank asking you to verify your account but it is fake.\n\n" +

                    "How to stay safe:\n" +
                    "- Never click suspicious links\n" +
                    "- Check the sender email carefully\n" +
                    "- Always log in directly from the official website";
            }

            if (input.Contains("password"))
            {
                return
                    "Definition:\n" +
                    "A password is a secret code used to protect your accounts.\n\n" +

                    "Example:\n" +
                    "Weak: 123456 | Strong: T!g3r#2026$\n\n" +

                    "How to stay safe:\n" +
                    "- Use long passwords (12+ characters)\n" +
                    "- Mix letters, numbers, symbols\n" +
                    "- Never reuse passwords across accounts";
            }

            if (input.Contains("malware"))
            {
                return
                    "Definition:\n" +
                    "Malware is harmful software designed to damage or steal data.\n\n" +

                    "Example:\n" +
                    "Downloading a free cracked game that secretly installs a virus.\n\n" +

                    "How to stay safe:\n" +
                    "- Use antivirus software\n" +
                    "- Avoid pirated downloads\n" +
                    "- Only install trusted apps";
            }

            if (input.Contains("privacy"))
            {
                return
                    "Definition:\n" +
                    "Online privacy is protecting your personal information from being exposed.\n\n" +

                    "Example:\n" +
                    "Not sharing your home address or ID number on social media.\n\n" +

                    "How to stay safe:\n" +
                    "- Limit personal information online\n" +
                    "- Use privacy settings on apps\n" +
                    "- Be careful what you post";
            }

            if (input.Contains("safe browsing") || input.Contains("browse") || input.Contains("internet"))
            {
                return
                    "Definition:\n" +
                    "Safe browsing means using the internet without exposing yourself to threats.\n\n" +

                    "Example:\n" +
                    "Avoiding websites that look suspicious or have too many ads or pop-ups.\n\n" +

                    "How to stay safe:\n" +
                    "- Only use HTTPS websites\n" +
                    "- Do not download unknown files\n" +
                    "- Avoid clicking random ads";
            }

            if (input.Contains("help"))
            {
                return
                    "I can teach you about:\n" +
                    "- Phishing\n" +
                    "- Password security\n" +
                    "- Malware\n" +
                    "- Privacy\n" +
                    "- Safe browsing\n\n" +
                    "Just type a topic.";
            }

            return
                "I did not fully understand that.\n\n" +
                "Try asking about:\n" +
                "- phishing\n" +
                "- password safety\n" +
                "- malware\n" +
                "- privacy\n" +
                "- safe browsing";
        }
    }
}