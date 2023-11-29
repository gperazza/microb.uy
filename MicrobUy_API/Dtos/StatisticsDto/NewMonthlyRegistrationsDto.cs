using Microsoft.Identity.Client;

namespace MicrobUy_API.Dtos.StatisticsDto
{
    public class NewMonthlyRegistrationsDto
    {
        public string Month {  get; set; }
        public int NewTotalUser { get; set; }
        public int NewTotalInstance { get; set; }
    }
}
