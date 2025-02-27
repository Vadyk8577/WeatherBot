namespace WeatherHistory.Models
{
    public class WeatherHistory
    {
        public int Id { get; set; }
        public long UserID { get; set; }
        public string City { get; set; }
        public decimal Temperature { get; set; }
        public decimal Humidity { get; set; }
        public decimal WindSpeed { get; set; }
        public string Condition { get; set; }
    }
}
