public class DateService
{
    private DateTime _analyticsDate = DateTime.Today;
    private DateTime _overviewDate = DateTime.Today;

    public DateTime AnalyticsDate
    {
        get => _analyticsDate;
        set
        {
            if (_analyticsDate != value)
            {
                _analyticsDate = value;
                OnAnalyticsDateChanged?.Invoke();
            }
        }
    }

    public DateTime OverviewDate
    {
        get => _overviewDate;
        set
        {
            if (_overviewDate != value)
            {
                _overviewDate = value;
                OnOverviewDateChanged?.Invoke();
            }
        }
    }

    public event Action? OnAnalyticsDateChanged;
    public event Action? OnOverviewDateChanged;
}