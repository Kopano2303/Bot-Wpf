using System;

namespace WpfBot
{
    public class Question
    {
        public string Text { get; set; }
        public string[] Options { get; set; }
        public string CorrectAnswer { get; set; }
        public string Explanation { get; set; }

        public Question()
        {
        }

        public Question(string text, string[] options, string correctAnswer, string explanation)
        {
            Text = text;
            Options = options;
            CorrectAnswer = correctAnswer;
            Explanation = explanation;
        }

        public override string ToString()
        {
            string result = Text + "\n\n";
            char label = 'A';
            foreach (string option in Options)
            {
                result += label + ") " + option + "\n";
                label++;
            }
            return result;
        }
    }
}