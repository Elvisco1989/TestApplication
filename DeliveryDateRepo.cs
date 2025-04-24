using TestApplication;

public class DeliveryDateRepo
{
    private List<DeliveryDates> _deliveryDates = new List<DeliveryDates>();
    private int _nextId = 1;

    public DeliveryDateRepo()
    {
        // Generate dates for the next 30 days
        var today = DateTime.Today;
        for (int i = 0; i < 30; i++)
        {
            var date = today.AddDays(i);
            _deliveryDates.Add(new DeliveryDates
            {
                DeliveryDateId = _nextId++,
                DeliveryDate = date
            });
        }
    }

    public List<DeliveryDates> GetDeliveryDatesForSegment(Segment segment, int count)
    {
        DayOfWeek targetDay = (DayOfWeek)segment;
        return _deliveryDates
            .Where(d => d.DeliveryDate.DayOfWeek == targetDay)
            .OrderBy(d => d.DeliveryDate)
            .Take(count)
            .ToList();
    }
}
