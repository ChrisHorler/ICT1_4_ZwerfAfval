public class DateService
{
    public event Action? OnDateChanged;

    private DateTime _selectedDate = DateTime.Today;

    public DateTime SelectedDate
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
