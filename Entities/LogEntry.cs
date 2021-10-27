namespace LogAnalyzer.Entities
{
    public class LogEntry
    {
        public int Id { get; set; }
        public string ClientIP { get; set; }
        public string ClientFQDN { get; set; }
        public int Visits { get; set; }
    }
}
