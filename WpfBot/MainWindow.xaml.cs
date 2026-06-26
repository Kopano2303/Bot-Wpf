using System.Windows;


namespace WpfBot
{
    public partial class MainWindow : Window
    {
        private CbBot bot;

        public MainWindow()
        {
            InitializeComponent();

            bot = new CbBot("CyberBot");

            ChatBox.Items.Add("CyberBot: Welcome to CyberSecurity Awareness Chatbot!");
            ChatBox.Items.Add("CyberBot: Type 'help' to see what I can do.");

           // StatusText.Text = "Status: Ready";
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            string input = InputBox.Text;

            if (string.IsNullOrWhiteSpace(input))
                return;

            ChatBox.Items.Add("You: " + input);

            string response = bot.GetResponse(input);

            ChatBox.Items.Add("CyberBot: " + response);

            InputBox.Clear();

            //StatusText.Text = "Status: Message processed";
        }

    }
}