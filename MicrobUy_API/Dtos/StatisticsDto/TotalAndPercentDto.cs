namespace MicrobUy_API.Dtos.StatisticsDto
{
    public class TotalAndPercentDto
    {
        public int Total { get; set; }
        public int Increase { get; set; }

        public TotalAndPercentDto(int Total, int Increase)
        {
            this.Total = Total;
            this.Increase = Increase;
        }

    }
}
