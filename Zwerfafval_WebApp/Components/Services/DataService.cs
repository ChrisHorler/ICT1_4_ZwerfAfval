public class DateService
{
    private DateOnly _selectedDate = DateOnly.FromDateTime(DateTime.Now);

    public event Action? OnDateChanged;

    public DateOnly SelectedDate
    {
        get => _selectedDate;
        set
        {
            if (_selectedDate != value)
            {
                _selectedDate = value;
                OnDateChanged?.Invoke();
            }
        }
    }
}
