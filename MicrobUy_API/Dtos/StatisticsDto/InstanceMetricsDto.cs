namespace MicrobUy_API.Dtos.StatisticsDto
{
    public class InstanceMetricsDto
    {
        public int PositionTop { get; set; }
        public string InstanceName { get; set; }
        public int TotalPost {  get; set; }
        public int TotalUsers { get; set; }
        public float PercentUserPlatform {  get; set; }
        public DateTime CreationDate { get; set; }
    }
}
