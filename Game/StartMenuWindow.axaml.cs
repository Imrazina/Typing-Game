using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using System;
using Avalonia;

namespace Game
{
    public partial class StartMenuWindow : Window
    {
        private GameMode gameMode;
        private DifficultyMode difficultyMode;
        public StartMenuWindow()
        {
            InitializeComponent();
        }

        private void OnGameModeClick(object? sender, RoutedEventArgs e)
        {
            if (MainMenuPanel.RenderTransform is TranslateTransform mainMenuTranslate)
            {
                mainMenuTranslate.X = -700;
                MainMenuPanel.Opacity = 0.5;
                MainMenuPanel.IsEnabled = false;
            }
            
            if (GameModePanel.RenderTransform is TranslateTransform gameModeTranslate)
            {
                GameModePanel.IsVisible = true;
                GameModePanel.Opacity = 1;
                gameModeTranslate.X = 0;
            }
        }

        private void OnBackClick(object? sender, RoutedEventArgs e)
        {
            if (MainMenuPanel.RenderTransform is TranslateTransform mainMenuTranslate)
            {
                mainMenuTranslate.X = 0;
                MainMenuPanel.Opacity = 1;
                MainMenuPanel.IsEnabled = true;
            }
            
            if (GameModePanel.RenderTransform is TranslateTransform gameModeTranslate)
            {
                GameModePanel.Opacity = 0;
                GameModePanel.IsVisible = false;
                gameModeTranslate.X = 200; 
            }
            
            if (GameModePanel.RenderTransform is TranslateTransform difficultyModeTranslate)
            {
                DifficultyModePanel.Opacity = 0;
                DifficultyModePanel.IsVisible = false;
                difficultyModeTranslate.X = 200;
            }
        } 

        private void OnFirstLetterClick(object? sender, RoutedEventArgs e)
        {
            gameMode = GameMode.OnlyLetters;
            Console.WriteLine("Selected mode: First Letter");
            OnBackClick(sender, e);
        }

        private void OnAnyLetterClick(object? sender, RoutedEventArgs e)
        {
            gameMode = GameMode.FullSentence;
            Console.WriteLine("Selected mode: Any Letter");
            OnBackClick(sender, e); 
        }
        
        private void OnNewGameClick(object? sender, RoutedEventArgs e)
        {
            var gameWindow = new MainWindow(gameMode, difficultyMode);
            gameWindow.Show();
        }

       private void Difficulty_Mode(object? sender, RoutedEventArgs e)
        {
            if (MainMenuPanel.RenderTransform is TranslateTransform mainMenuTranslate)
            {
                mainMenuTranslate.X = -700;
                MainMenuPanel.Opacity = 0.5;
                MainMenuPanel.IsEnabled = false;
            }
            
            if (DifficultyModePanel.RenderTransform is TranslateTransform difficultyModeTranslate)
            {
                DifficultyModePanel.IsVisible = true;
                DifficultyModePanel.Opacity = 1;
                difficultyModeTranslate.X = 0;
            }
        }

        private void OnEasyMode(object? sender, RoutedEventArgs e)
        {
            difficultyMode = DifficultyMode.Easy;
            OnBackClick(sender, e); 
        }

        private void OnMediumMode(object? sender, RoutedEventArgs e)
        {
            difficultyMode = DifficultyMode.Medium;
            OnBackClick(sender, e); 
        }

        private void OnHardMode(object? sender, RoutedEventArgs e)
        {
            difficultyMode = DifficultyMode.Hard;
            OnBackClick(sender, e); 
        }

        private void OnGradualyMode(object? sender, RoutedEventArgs e)
        {
            difficultyMode = DifficultyMode.Gradualy;
            OnBackClick(sender, e); 
        }

        private void OnExitClick(object? sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}