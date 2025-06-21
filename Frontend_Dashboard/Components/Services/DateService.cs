using System.ComponentModel;

namespace Frontend_Dashboard.Components.Services;

public sealed class DateService : INotifyPropertyChanged
{
    private DateOnly _selectedDate = DateOnly.FromDateTime(DateTime.Today);

    public DateOnly SelectedDate
    {
        get => _selectedDate;
        set
        {
            if (value != _selectedDate)
            {
                _selectedDate = value;
                PropertyChanged?.Invoke(this, new(nameof(SelectedDate)));
            }
        }
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
}