using System;

namespace WpfBot
{
    internal class Question
    {
        public string QuestionText { get; set; }

        public string[] Options { get; set; }

        public string CorrectAnswer { get; set; }

        public string Explanation { get; set; }

        public Question()
        {
        }

        public Question(string questionText, string[] options, string correctAnswer, string explanation)
        {
            QuestionText = questionText;
            Options = options;
            CorrectAnswer = correctAnswer;
            Explanation = explanation;
        }

        public override string ToString()
        {
            string text = QuestionText + "\n\n";

            char optionLetter = 'A';

            foreach (string option in Options)
            {
                text += optionLetter + ") " + option + "\n";
                optionLetter++;
            }

            return text;
        }
    }
}