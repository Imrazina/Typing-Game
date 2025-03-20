using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Timers;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using Game.ChangeColor;
using GameLibrary;
using Timer = System.Timers.Timer;

namespace Game
{
    public partial class MainWindow : Window
    {
        private readonly Random random = new();
        private readonly Stats stats = new();
        private readonly Timer gameTimer;
        public ObservableCollection<LetterItem> Letters { get; private set; } = new();

        private const int MaxLetters = 6;
        private int difficulty = 800;
        private readonly GameMode gameMode;
        private readonly DifficultyMode difficultyMode;
        private string currentInput = "";

        private const int LetEasyDifficulty = 1000;
        private const int LetMediumDifficulty = 600;
        private const int LetHardDifficulty = 300;
        private const int LetGradualyStartDifficulty = 1000;
        private const int LetDefaultDifficulty = 800;
        private const int LetMinDifficulty = 100;

        private Timer countdownTimer;
        private int remainingTime;
        private int elapsedTime;

        public MainWindow(GameMode mode, DifficultyMode difficultyMode)
        {
            gameMode = mode;
            this.difficultyMode = difficultyMode;
            InitializeComponent();
            DataContext = this;
            LettersListBox.ItemsSource = Letters;
            stats.UpdatedStats += Stats_UpdatedStats;

            InitializeDifficulty();

            gameTimer = new Timer(difficulty);
            gameTimer.Elapsed += GameTimer_Tick;
            gameTimer.Start();

            if (gameMode == GameMode.FullSentence)
            {
                InitializeCountdownTimer();
                TimeDisplayTextBlock.IsVisible = true;
            }

            this.KeyDown -= Window_KeyDown;
            this.KeyDown += Window_KeyDown;
        }
        
        private void ShowGameOverScreen()
        {
            GameOverGrid.IsVisible = true;

            GameOverGrid.Opacity = 1;
            GameOverGrid.RenderTransform = new TranslateTransform { Y = 0 };
            
            var statsEventArgs = new UpdatedStatsEventArgs
            {
                Correct = stats.Correct,
                Missed = stats.Missed,
                Accuracy = stats.Accuracy
            };

            Stats(this, statsEventArgs); 
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (gameMode == GameMode.FullSentence)
            {
                ProcessInput();
            }
        }

        private void ProcessInput()
        {
            var newSentence = "START";
            var lettersList = new ObservableCollection<LetterChar>();
            foreach (char c in newSentence)
            {
                lettersList.Add(new LetterChar
                {
                    Character = c,
                    Color = "LightGray"
                });
            }

            var letterItem = new LetterItem
            {
                Letters = lettersList
            };

            Letters.Add(letterItem);

            LettersListBox.Focus();
        }

        private void GameTimer_Tick(object? sender, ElapsedEventArgs e)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (gameMode == GameMode.OnlyLetters)
                {
                    // Режим "Only Letters": генерируем случайные буквы
                    char newLetter = (char)random.Next(65, 91);
                    var letterItem = new LetterItem();
                    letterItem.Letters.Add(new LetterChar { Character = newLetter, Color = "LightGray" });
                    Letters.Add(letterItem);
                    if (Letters.Count > MaxLetters)
                    {
                        gameTimer.Stop();
                        Letters.Clear();
                        ShowGameOver();
                    }
                }

                else if (gameMode == GameMode.FullSentence)
                {
                    // Генерация случайного предложения
                    string[] sentences = new[]
                    {
                        "THE SUN SHINES BRIGHTLY OVER THE CALM BLUE OCEAN",
                        "SHE QUICKLY GRABBED HER COAT AND RAN OUTSIDE",
                        "A SMALL CAT SLEPT PEACEFULLY ON THE WARM COUCH",
                        "HE ALWAYS WAKES UP EARLY TO GO FOR A RUN",
                        "THE OLD LIBRARY SMELLED OF DUST AND ANCIENT BOOKS",
                        "BIRDS SANG HAPPILY IN THE TREES NEAR OUR HOUSE",
                        "SHE CAREFULLY PLACED THE VASE ON THE WOODEN TABLE",
                        "THE BOY LAUGHED LOUDLY AT HIS FRIEND'S FUNNY JOKE",
                        "OUR TEAM WORKED HARD TO FINISH THE BIG PROJECT",
                        "THE TRAIN ARRIVED EXACTLY ON TIME AT THE STATION",
                        "A GENTLE BREEZE MOVED THROUGH THE GREEN FOREST",
                        "HE PLAYED THE GUITAR WHILE SHE SANG A SWEET SONG",
                        "THE BAKER MADE FRESH BREAD EVERY MORNING AT DAWN",
                        "SHE FOUND A HIDDEN LETTER INSIDE THE OLD DRAWER",
                        "THE LITTLE GIRL PAINTED A PICTURE OF THE SKY",
                        "THEY WALKED THROUGH THE PARK, ENJOYING THE FRESH AIR",
                        "A DELICIOUS CAKE WAS BAKING IN THE HOT OVEN",
                        "HE FIXED HIS BIKE AND RODE IT DOWN THE STREET",
                        "THE MOUNTAIN PEAK WAS COVERED IN THICK WHITE SNOW",
                        "THE DOG WAGGED ITS TAIL AND BARKED EXCITEDLY"
                    };

                    string newSentence = sentences[random.Next(sentences.Length)];

                    var lettersList = new ObservableCollection<LetterChar>();
                    foreach (char c in newSentence)
                    {
                        lettersList.Add(new LetterChar
                        {
                            Character = c,
                            Color = "LightGray"
                        });
                        Console.WriteLine($"Added character: '{c}'");
                    }

                    var letterItem = new LetterItem
                    {
                        Letters = lettersList
                    };

                    Letters.Add(letterItem);
                }
            });
        }

        private void Window_KeyDown(object? sender, KeyEventArgs e)
        {
            if (Letters.Any(item => item.Letters.Select(l => l.Character).SequenceEqual("Game Over!")))
            {
                RestartGame();
                stats.Reset();
                return;
            }

            if (gameMode == GameMode.OnlyLetters)
            {
                string key = e.Key.ToString().ToUpper();
                var letterItem = Letters.FirstOrDefault(item =>
                    item.Letters.Count == 1 && item.Letters[0].Character.ToString() == key);
                if (letterItem != null)
                {
                    letterItem.Letters[0].Color = "Black";
                    Letters.Remove(letterItem);
                    stats.Update(true);
                    IncreaseDifficulty();
                }
                else
                {
                    var firstLetter = Letters.FirstOrDefault();
                    if (firstLetter != null)
                    {
                        firstLetter.Letters[0].Color = "Red";
                    }

                    stats.Update(false);
                }
            }
            else if (gameMode == GameMode.FullSentence)
            {
                if (e.Key == Key.Back)
                {
                    if (currentInput.Length > 0)
                    {
                        currentInput = currentInput.Remove(currentInput.Length - 1);
                        UpdateWordColors();
                    }
                }
                else if (e.Key == Key.Enter)
                {
                    if (Letters.Count > 0)
                    {
                        var currentSentenceItem = Letters[0];

                        string currentSentence = new string(currentSentenceItem.Letters
                            .Select(l => l.Character == '\u00A0' ? ' ' : l.Character)
                            .ToArray());

                        if (currentInput.TrimEnd() == currentSentence)
                        {
                            Letters.RemoveAt(0);
                            stats.Update(true);
                            IncreaseDifficulty();
                        }
                        else
                        {
                            Letters.RemoveAt(0);
                            stats.Update(false);
                        }

                        currentInput = "";
                        UpdateWordColors();
                    }
                }
                else if (e.Key >= Key.A && e.Key <= Key.Z)
                {
                    currentInput += e.Key.ToString().ToUpper();
                    UpdateWordColors();
                }
                else if (e.Key == Key.Space)
                {
                    currentInput += " ";
                    UpdateWordColors();
                }
            }
        }

        private void UpdateWordColors()
        {
            if (Letters.Count == 0) return; 

            var currentSentenceItem = Letters[0];
            for (int i = 0; i < currentSentenceItem.Letters.Count; i++)
            {
                if (i < currentInput.Length)
                {
                    if (currentSentenceItem.Letters[i].Character == currentInput[i])
                    {
                        currentSentenceItem.Letters[i].Color = "Black";
                    }
                    else
                    {
                        currentSentenceItem.Letters[i].Color = "Red";
                    }
                }
                else
                {
                    currentSentenceItem.Letters[i].Color = "LightGray";
                }
            }
        }

        private void Stats_UpdatedStats(object sender, UpdatedStatsEventArgs e)
        {
            CorrectLabel.Text = $"{e.Correct}";
            MissedLabel.Text = $"{e.Missed}";
            int NewAccuracy = e.Accuracy;
            int totalAccuracy = NewAccuracy / 21;
            if (gameMode == GameMode.FullSentence)
            {
                AccuracyLabel.Text = $"{totalAccuracy}";
            }
            else
            {
                AccuracyLabel.Text = $"{e.Accuracy}%";
            }
        }
        
        private void Stats(object sender, UpdatedStatsEventArgs e)
        {
            CorrectLabelText.Text = $"{e.Correct}";
            MissedLabelText.Text = $"{e.Missed}";
            AccuracyLabelText.Text = $"{e.Accuracy}%";
        }

        private void InitializeDifficulty()
        {
            switch (difficultyMode)
            {
                case DifficultyMode.Easy:
                    difficulty = LetEasyDifficulty;
                    UpdateDifficultyProgressBar();
                    break;
                case DifficultyMode.Medium:
                    difficulty = LetMediumDifficulty;
                    UpdateDifficultyProgressBar();
                    break;
                case DifficultyMode.Hard:
                    difficulty = LetHardDifficulty;
                    UpdateDifficultyProgressBar();
                    break;
                case DifficultyMode.Gradualy:
                    difficulty = LetGradualyStartDifficulty;
                    break;
                default:
                    difficulty = LetDefaultDifficulty;
                    break;
            }
            
            if (gameTimer != null)
            {
                gameTimer.Interval = difficulty;
            }
        }

        private void IncreaseDifficulty()
        {
            int newDifficulty = difficulty;

            if (difficultyMode == DifficultyMode.Gradualy)
            {
                if (newDifficulty > 400)
                    newDifficulty -= 60;
                else if (newDifficulty > 250)
                    newDifficulty -= 15;
                else if (newDifficulty > 150)
                    newDifficulty -= 8;
                newDifficulty = Math.Max(LetMinDifficulty, newDifficulty);
            }

            if (newDifficulty != difficulty && gameTimer != null)
            {
                difficulty = newDifficulty;
                gameTimer.Stop();
                gameTimer.Interval = difficulty;
                gameTimer.Start();
                UpdateDifficultyProgressBar();
            }
        }

        private void InitializeCountdownTimer()
        {
            switch (difficultyMode)
            {
                case DifficultyMode.Easy:
                    remainingTime = 120; 
                    break;
                case DifficultyMode.Medium:
                    remainingTime = 90; 
                    break;
                case DifficultyMode.Hard:
                    remainingTime = 60; 
                    break;
                case DifficultyMode.Gradualy:
                    elapsedTime = 0; 
                    break;
                default:
                    remainingTime = 120; 
                    break;
            }

            countdownTimer = new Timer(1000); 
            countdownTimer.Elapsed += CountdownTimer_Tick;
            countdownTimer.Start();
        }

        private void CountdownTimer_Tick(object? sender, ElapsedEventArgs e)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (difficultyMode == DifficultyMode.Gradualy)
                {
                    elapsedTime++;
                }
                else
                {
                    remainingTime--;
                }
                
                UpdateTimeDisplay();
                
                if (difficultyMode == DifficultyMode.Gradualy)
                {
                    if (Letters.Count == 0)
                    {
                        countdownTimer.Stop();
                        ShowGameOver();
                    }
                }
                else
                {
                    if (remainingTime <= 0)
                    {
                        ShowGameOver();
                        countdownTimer.Stop();
                        ShowGameOver();
                    }
                }
            });
        }

        private void UpdateTimeDisplay()
        {
            string timeString;

            if (difficultyMode == DifficultyMode.Gradualy)
            {
                TimeSpan time = TimeSpan.FromSeconds(elapsedTime);
                timeString = time.ToString(@"mm\:ss");
            }
            else
            {
                TimeSpan time = TimeSpan.FromSeconds(remainingTime);
                timeString = time.ToString(@"mm\:ss");
            }
            
            TimeDisplayTextBlock.Text = timeString;
        }

        private void ShowGameOver()
        {
            gameTimer.Stop();
            
            var gameOverItem = new LetterItem();
            gameOverItem.Letters.Add(new LetterChar { Character = 'G', Color = "Black" });
            gameOverItem.Letters.Add(new LetterChar { Character = 'a', Color = "Black" });
            gameOverItem.Letters.Add(new LetterChar { Character = 'm', Color = "Black" });
            gameOverItem.Letters.Add(new LetterChar { Character = 'e', Color = "Black" });
            gameOverItem.Letters.Add(new LetterChar { Character = ' ', Color = "Black" });
            gameOverItem.Letters.Add(new LetterChar { Character = 'O', Color = "Black" });
            gameOverItem.Letters.Add(new LetterChar { Character = 'v', Color = "Black" });
            gameOverItem.Letters.Add(new LetterChar { Character = 'e', Color = "Black" });
            gameOverItem.Letters.Add(new LetterChar { Character = 'r', Color = "Black" });
            gameOverItem.Letters.Add(new LetterChar { Character = '!', Color = "Black" });
            Letters.Add(gameOverItem);
            
            this.KeyDown -= Window_KeyDown;
            ShowGameOverScreen();
        }

        private void UpdateDifficultyProgressBar()
        {
            DifficultyProgressBar.Value = difficulty;

            if (difficulty > 400)
            {
                DifficultyProgressBar.Foreground = new SolidColorBrush(Color.Parse("#4CAF50"));
            }
            else if (difficulty > 250)
            {
                DifficultyProgressBar.Foreground = new SolidColorBrush(Color.Parse("#FFC107"));
            }
            else
            {
                DifficultyProgressBar.Foreground = new SolidColorBrush(Color.Parse("#F44336"));
            }
        }

        private void RestartGame()
        {
            stats.Reset();
            
            Letters.Clear();
            
            InitializeDifficulty();
            
            UpdateDifficultyProgressBar();
            
            currentInput = "";
            
            gameTimer.Stop();
            gameTimer.Start();
            if (countdownTimer != null)
            {
                countdownTimer.Stop();
                if (difficultyMode == DifficultyMode.Gradualy)
                {
                    elapsedTime = 0;
                }
                else
                {
                    InitializeCountdownTimer();
                }
            }
            
            if (gameMode == GameMode.FullSentence)
            {
                ProcessInput();
            }
            this.KeyDown -= Window_KeyDown;
            this.KeyDown += Window_KeyDown;

            // Показываем основной интерфейс и скрываем экран завершения
            MainGameGrid.IsVisible = true;
            GameOverGrid.IsVisible = false;
        }
        
        private void RestartButton_Click(object? sender, RoutedEventArgs e)
        {
            GameOverGrid.IsVisible = false;

            // Показываем основной интерфейс
            MainGameGrid.IsVisible = true;

            // Перезапускаем игру
            RestartGame();
            CorrectLabel.Text = $"0";
            MissedLabel.Text = $"0";
            AccuracyLabel.Text = $"0%";
        }
    }
}