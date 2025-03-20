using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Game.ChangeColor;


    public class LetterItem : INotifyPropertyChanged
    {
        public ObservableCollection<LetterChar> Letters { get; init; } = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
