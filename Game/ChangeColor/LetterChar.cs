using System.ComponentModel;

namespace Game.ChangeColor;

public class LetterChar : INotifyPropertyChanged
{
    private char _character;
    private string _color = "LightGray";

    public char Character
    {
        get => _character;
        set
        {
            if (_character != value)
            {
                _character = value;
                OnPropertyChanged(nameof(Character));
            }
        }
    }

    public string Color
    {
        get => _color;
        set
        {
            if (_color != value)
            {
                _color = value;
                OnPropertyChanged(nameof(Color));
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}