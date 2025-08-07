using skylance_backend.Models;

namespace skylance_backend.Data
{
    public class Web_FlightsDTO
    {
        public string? AirlineName { get; set; }
        public string? FlightName { get; set; }
        public string? DepartureDate { get; set; }
        public string? ArrivalTime { get; set; }
        public string? DepartureCity { get; set; }
        public string? ArrivalCity { get; set; }
        public string? DepartureCityCode { get; set; }
        public string? ArrivalCityCode { get; set; }
        public int? TotalNoOfPassengers { get; set; }
        public int? TotalNoOfCrew { get; set; }
        public string? FlightStatus { get; set; }
    }
}
