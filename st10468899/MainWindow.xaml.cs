
using System;
using System.Media;
using System.Windows;
using System.Collections.Generic;

namespace ProgPart2
{
    public partial class MainWindow : Window
    {
        
        string userName = "";
        string currentTopic = "";
        List<string> tasks = new List<string>();

        
        private List<QuizQuestion> quizQuestions = new List<QuizQuestion>();
        private List<string> activityLog = new List<string>();
        private int currentQuestionIndex = 0;
        private int userScore = 0;
        private bool isGameActive = false;


        Random random = new Random();


        string[] passwordResponses = {
            "Use strong passwords with symbols and numbers.",
            "Avoid using your name in passwords.",
            "Use different passwords for different accounts.",
            "Enable 2-factor authentication for extra security"
        };

        string[] phishingResponses = {
            "Avoid suspicious emails and links.",
            "Always verify the sender before clicking links.",
            "Scammers often pretend to be trusted companies.",
            "Never share personal information through suspicious emails."
        };

        string[] browsingResponses = {
            "Always check for HTTPS before entering personal information on a website.",
            "Avoid downloading files from websites you do not trust.",
            "Be careful when using public Wi-Fi because hackers can sometimes access unsecured connections.",
            "Trusted websites usually have secure connections and clear contact information.",
            "Try not to click pop-up ads because some may contain malware."
        };

        
        public MainWindow()
        {
            InitializeComponent();
            DisplayHeader();
            DisplayLogo();
            PlayGreeting();
            InitializeQuizData();
        }

        
        private void DisplayHeader()
        {
            ChatDisplay.AppendText("=========================================\n");
            ChatDisplay.AppendText("     CYBERSECURITY AWARENESS BOT\n");
            ChatDisplay.AppendText("=========================================\n");
            ChatDisplay.AppendText("       !!! Stay Safe Online !!!\n");
            ChatDisplay.AppendText("=========================================\n\n");
        }

        private void DisplayLogo()
        {
            ChatDisplay.AppendText(@"
   .-------.
  /   ___   \
 |   /   \   |
 |  |     |  |
  \___/   |
 \___________/
 |           |
 |  [ SAFE ] |
 |___________|

");
        }

        private void PlayGreeting()
        {
            try
            {
                SoundPlayer player = new SoundPlayer("greetings.wav");
                player.Play();
            }
            catch
            {
                ChatDisplay.AppendText("Audio could not be played.\n");
            }
        }
        private void LogActivity(string description)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            activityLog.Add($"[{timestamp}] {description}");

            // Maintain only the last 10 actions to keep the log concise
            if (activityLog.Count > 10)
            {
                activityLog.RemoveAt(0);
            }
        }
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UserInput.Text)) return;

            string cleanInput = UserInput.Text.Trim().ToUpper();
            ChatDisplay.AppendText("You: " + UserInput.Text + "\n");

            //game logic 
            if (isGameActive)
            {
                QuizQuestion currentQ = quizQuestions[currentQuestionIndex];
                int selectedIndex = -1;

                if (cleanInput == "A") selectedIndex = 0;
                else if (cleanInput == "B") selectedIndex = 1;
                else if (cleanInput == "C") selectedIndex = 2;
                else if (cleanInput == "D") selectedIndex = 3;

                if (selectedIndex >= 0 && selectedIndex < currentQ.Options.Count)
                {
                    if (selectedIndex == currentQ.CorrectOptionIndex)
                    {
                        ChatDisplay.AppendText("Chatbot: Correct! \n");
                        userScore++;
                    }
                    else
                    {
                        char correctLetter = (char)('A' + currentQ.CorrectOptionIndex);
                        ChatDisplay.AppendText($"Chatbot: Incorrect. The correct choice was {correctLetter}.\n");
                    }

                    ChatDisplay.AppendText($"Explanation: {currentQ.Explanation}\n");
                    ChatDisplay.AppendText("-------------------------------------------------\n\n");

                    currentQuestionIndex++;
                    UserInput.Clear();
                    ShowNextQuestion();
                    return;
                }
                else
                {
                    ChatDisplay.AppendText("Bot: Invalid choice. Please type a valid option letter (A, B, C, or D).\n\n");
                    UserInput.Clear();
                    ChatDisplay.ScrollToEnd();
                    return;
                }
            }

            
            string input = UserInput.Text.ToLower();

            if (input.Contains("exit") || input.Contains("bye"))
            {
                ChatDisplay.AppendText("Bot: Goodbye, have a lovely day!\n");
                UserInput.Clear();
                return;
            }

            // Mini-game command trigger
            if (input.Contains("game") || input.Contains("play") || input.Contains("quiz"))
            {
                UserInput.Clear();
                StartCyberGame();
                return;
            }

            string response = GetBotResponse(input);
            ChatDisplay.AppendText("Bot: " + response + "\n\n");

            UserInput.Clear();
            ChatDisplay.ScrollToEnd();


        }
        private void StartCyberGame()
        {
            LogActivity("Quiz started - test initiated.");
            isGameActive = true;
            currentQuestionIndex = 0;
            userScore = 0;

            ChatDisplay.AppendText("=========================================\n");
            ChatDisplay.AppendText("    CYBERSECURITY MINI-GAME STARTED! \n");
            ChatDisplay.AppendText("=========================================\n");
            ChatDisplay.AppendText("Answer by typing option letters (e.g., A, B, C, or D).\n\n");

            ShowNextQuestion();
        }

        private void ShowNextQuestion()
        {
            if (currentQuestionIndex < quizQuestions.Count)
            {
                QuizQuestion q = quizQuestions[currentQuestionIndex];
                ChatDisplay.AppendText($"Question {currentQuestionIndex + 1}/{quizQuestions.Count}: {q.QuestionText}\n");

                char optionLetter = 'A';
                for (int i = 0; i < q.Options.Count; i++)
                {
                    ChatDisplay.AppendText($"{optionLetter}) {q.Options[i]}\n");
                    optionLetter++;
                }
                ChatDisplay.AppendText("\nType your answer selection below and press Send.\n\n");
            }
            else
            {
                LogActivity($"Quiz completed - User scored {userScore} out of {quizQuestions.Count}.");

                ChatDisplay.AppendText("==================== GAME OVER ====================\n");
                ChatDisplay.AppendText($"Your Final Score: {userScore} out of {quizQuestions.Count} correct.\n");

                if (userScore >= 9)
                {
                    ChatDisplay.AppendText("Bot Feedback: Great job! You're a cybersecurity pro! 🛡️✨\n\n");
                }
                else
                {
                    ChatDisplay.AppendText("Bot Feedback: Keep learning to stay safe online! 📚🔒\n\n");
                }

                isGameActive = false;
            }
            ChatDisplay.ScrollToEnd();
        }




        private string GetBotResponse(string input)
        {
            
            if (input.Contains("what have you done") || input.Contains("summary") || input.Contains("activity log"))
            {
                if (activityLog.Count == 0)
                {
                    return "Here's a summary of recent actions:\nNo actions recorded yet. Try playing a game or adding a task!";
                }

                string logSummary = "Here's a summary of recent actions:\n";
                for (int i = 0; i < activityLog.Count; i++)
                {
                    logSummary += $"{i + 1}. {activityLog[i]}\n";
                }
                return logSummary;
            }

            
            
            bool wantsToCreate = input.Contains("add") || input.Contains("remind") || input.Contains("set") || input.Contains("track");

            if (wantsToCreate)
            {
                string extractedTask = "";

                if (input.Contains("password"))
                {
                    extractedTask = "Update my password security settings";
                }
                else if (input.Contains("two-factor") || input.Contains("2fa") || input.Contains("authentication"))
                {
                    extractedTask = "Enable two-factor authentication (2FA)";
                }
                else if (input.Contains("privacy") || input.Contains("settings"))
                {
                    extractedTask = "Review account privacy settings";
                }
                else if (input.Contains("phishing") || input.Contains("email"))
                {
                    extractedTask = "Check suspicious emails for phishing hooks";
                }
                else if (input.Contains("browse") || input.Contains("website"))
                {
                    extractedTask = "Run an anti-malware scan on downloaded web files";
                }

                if (!string.IsNullOrEmpty(extractedTask))
                {
                    if (input.Contains("tomorrow"))
                    {
                        string targetDate = DateTime.Now.AddDays(1).ToShortDateString();
                        string fullTask = $"{extractedTask} | Due: {targetDate}";

                        tasks.Add(fullTask);
                        TaskListBox.Items.Add(fullTask);

                        LogActivity($"Task added via NLP: '{extractedTask}' (Reminder set for tomorrow)");

                        return $"Reminder set for '{extractedTask}' on tomorrow's date.";
                    }
                    else
                    {
                        string fullTask = $"{extractedTask} | No Reminder Set";

                        tasks.Add(fullTask);
                        TaskListBox.Items.Add(fullTask);

                     
                        LogActivity($"Task added via NLP: '{extractedTask}' (No reminder set)");

                        return $"Task added: '{extractedTask}' (no reminder set).";
                    }
                }
                }
                // MEMORY FEATURE
                if (input.Contains("my name is"))
                {
                    userName = input.Replace("my name is", "").Trim();
                    return "Welcome, " + userName + "!";
                }

                // SENTIMENT DETECTION
                if (input.Contains("sad") || input.Contains("upset"))
                {
                    return "I'm sorry you're feeling sad. Stay safe online and take breaks when needed.";
                }

                if (input.Contains("happy") || input.Contains("excited"))
                {
                    return "That's wonderful to hear!";
                }

                // GREETINGS
                if (input == "hi" || input == "hello" || input == "hey")
                {
                    return "Hello! How can I help you?";
                }

                // HOW ARE YOU
                if (input.Contains("how are you"))
                {
                    return "I'm doing great!,thank you for asking.How are you doing today";
                }
                if (input.Contains("good") || input.Contains("fine"))
                {
                    return "I'm glad to hear that";
                }

                if (input.Contains("tired") || input.Contains("bad") || input.Contains("sad"))
                {
                    return "I'm sorry to hear that. Make sure to rest and take care of yourself and i am here to give you tips on how to stay safe online,whenever you're ready! ";
                }

                // BOT PURPOSE
                if (input.Contains("what can") || input.Contains("assist"))
                {
                    return "I can help you with passwords, phishing, scams and safe browsing.";
                }

                // PASSWORDS
                if (input.Contains("password") || input.Contains("safety"))
                {
                    currentTopic = "password";
                    return passwordResponses[random.Next(passwordResponses.Length)];
                }

                // PHISHING
                if (input.Contains("phishing") || input.Contains("scam") || input.Contains("email"))
                {
                    currentTopic = "phishing";
                    return phishingResponses[random.Next(phishingResponses.Length)];
                }

                // SAFE BROWSING
                if (input.Contains("browse") || input.Contains("website") || input.Contains("safe browsing") || input.Contains("internet"))
                {
                    currentTopic = "browsing";
                    return browsingResponses[random.Next(browsingResponses.Length)];
                }

                // MEMORY CALLBACK
                if (input.Contains("who am i"))
                {
                    return "Your name is " + userName;
                }

                if (input.Contains("tell me more") || input.Contains("another tip"))
                {
                    if (currentTopic == "password")
                    {
                        return "Another good tip is to never reuse the same password across multiple websites.";
                    }

                    if (currentTopic == "phishing")
                    {
                        return "Phishing emails often pretend to be banks, Netflix or trusted companies.";
                    }

                    if (currentTopic == "browsing")
                    {
                        return "Another safe browsing tip is to keep your browser antivirus software updated";
                    }
                }

                return "I didn't understand that. Try asking something else.";
            }

        // Shortcut listener so users can hit the "Enter" key to send messages
        private void UserInput_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                SendButton_Click(this, new RoutedEventArgs());
            }
        }


        private void InitializeQuizData()
        {
            quizQuestions.Clear();

            // 1. Multiple Choice
            quizQuestions.Add(new QuizQuestion(
                "What should you do if you receive an email asking for your password?",
                new List<string> { "Reply with your password", "Delete the email", "Report the email as phishing", "Ignore it" },
                2, "Reporting phishing emails helps security teams block future scams. Never share passwords via email!"));

            // 2. True / False
            quizQuestions.Add(new QuizQuestion(
                "True or False: Public Wi-Fi networks are generally secure as long as you need a password to connect.",
                new List<string> { "True", "False" },
                1, "Public Wi-Fi networks can easily be intercepted by threat actors. Use a VPN to encrypt your connection."));

            // 3. Multiple Choice
            quizQuestions.Add(new QuizQuestion(
                "Which of the following characters should be included to create a highly secure password?",
                new List<string> { "Only lowercase letters", "Your birth year", "A mix of uppercase, lowercase, numbers, and symbols", "Sequential numbers like 1234" },
                2, "Complex mixtures prevent brute-force software tool automated guessing programs from cracking your account."));

            // 4. True / False
            quizQuestions.Add(new QuizQuestion(
                "True or False: Phishing scams only occur through written emails.",
                new List<string> { "True", "False" },
                1, "Phishing happens through SMS texts (Smishing), phone calls (Vishing), and malicious websites."));

            // 5. Multiple Choice
            quizQuestions.Add(new QuizQuestion(
                "What does the 'S' stand for in HTTPS when browsing a secure website URL?",
                new List<string> { "Speed", "System", "Secure", "Server" },
                2, "HTTPS ensures data passed between your browser and the web server is safely encrypted."));

            // 6. Multiple Choice
            quizQuestions.Add(new QuizQuestion(
                "What type of social engineering attack involves an attacker tracking you closely into a restricted office doorway?",
                new List<string> { "Tailgating", "Spear Phishing", "Whaling", "Baiting" },
                0, "Tailgating is a physical security breach where unauthorized individuals sneak past security checkpoints behind real employees."));

            // 7. True / False
            quizQuestions.Add(new QuizQuestion(
                "True or False: Using the exact same secure password across multiple personal accounts is safe.",
                new List<string> { "True", "False" },
                1, "If one database breaches, hackers will use credential stuffing to unlock all your other matching profiles."));

            // 8. Multiple Choice
            quizQuestions.Add(new QuizQuestion(
                "What is the safest way to deal with a computer pop-up warning stating your device is heavily infected?",
                new List<string> { "Click 'Fix Now'", "Download their recommended software", "Close the browser completely and run your local trusted antivirus", "Ignore it entirely" },
                2, "Fake alert pop-ups are malicious ads trying to trick you into downloading ransomware or malware tools."));

            // 9. True / False
            quizQuestions.Add(new QuizQuestion(
                "True or False: Software updates shouldn't be installed immediately because they are mostly for visual changes.",
                new List<string> { "True", "False" },
                1, "Updates patch critical zero-day security vulnerabilities that active hackers are currently exploiting."));

            // 10. Multiple Choice
            quizQuestions.Add(new QuizQuestion(
                "What is the term used for malicious software that locks your personal files and demands payment to restore access?",
                new List<string> { "Spyware", "Ransomware", "Adware", "Trojan Horse" },
                1, "Ransomware holds your computer files hostage using heavy cryptographic digital locking systems."));

            // 11. Multiple Choice
            quizQuestions.Add(new QuizQuestion(
                "If a close friend suddenly texts you an unusual link asking for emergency money, what should you do first?",
                new List<string> { "Click it immediately to help out", "Send money right away", "Call or contact them on a completely different platform to verify it is really them", "Block them forever" },
                2, "Account hijacking is common; always check directly with friends out-of-band before trusting weird links or requests."));
        }

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TaskTextBox.Text))
            {
                MessageBox.Show("Please enter a cybersecurity task (e.g., 'Enable 2FA').", "Input Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string task = TaskTextBox.Text.Trim();

            if (ReminderDatePicker.SelectedDate != null)
            {
                task += " | Due: " + ReminderDatePicker.SelectedDate.Value.ToShortDateString();
            }
            else
            {
                task += " | No Reminder Set";
            }

            tasks.Add(task);
            TaskListBox.Items.Add(task);

            ChatDisplay.AppendText("Bot: Added task: \"" + TaskTextBox.Text.Trim() + "\" to your tracking dashboard.\n\n");

            // Clear input fields for next entry
            TaskTextBox.Clear();
            ReminderDatePicker.SelectedDate = null;
            ChatDisplay.ScrollToEnd();
        }

        private void RemoveTask_Click(object sender, RoutedEventArgs e)
        {
            if (TaskListBox.SelectedIndex != -1)
            {
                int selectedIndex = TaskListBox.SelectedIndex;

                
                TaskListBox.Items.RemoveAt(selectedIndex);
                tasks.RemoveAt(selectedIndex);

                ChatDisplay.AppendText("Bot: I have removed that security task from your tracking dashboard.\n\n");
                ChatDisplay.ScrollToEnd();
            }
            else
            {
                MessageBox.Show("Please click on an active task in the checklist below to select and remove it.", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
    public class QuizQuestion
    {
        public string QuestionText { get; set; }
        public List<string> Options { get; set; }
        public int CorrectOptionIndex { get; set; }
        public string Explanation { get; set; }

        public QuizQuestion(string text, List<string> options, int correctIndex, string explanation)
        {
            QuestionText = text;
            Options = options;
            CorrectOptionIndex = correctIndex;
            Explanation = explanation;
        }
    }
}
