using skylance_backend.Models;

namespace skylance_backend.Data
{
    public class BoardingPassDTO
    {
        public string FlightNumber { get; set; }
        public string OriginAirportCode { get; set; }
        public string OriginAirportName { get; set; }
        public string DestinationAirportCode { get; set; }
        public string DestinationAirportName { get; set; }
        public string Airline { get; set; }
        public string AircraftModel { get; set; }
        public DateTime BoardingTime { get; set; }
        public string Gate { get; set; }
        public Seat Seat { get; set; }
        public string Terminal { get; set; }
    }
}
