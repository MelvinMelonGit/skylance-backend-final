namespace skylance_backend.Data
{
    public class TripDetailDTO
    {
        public string BookingReferenceNumber { get; set; }
        public string FlightNumber { get; set; }
        public string OriginAirportCode { get; set; }
        public string OriginAirportName { get; set; }
        public string DestinationAirportCode { get; set; }
        public string DestinationAirportName { get; set; }
        public string Airline { get; set; }
        public string AircraftModel { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public TimeSpan FlightDuration { get; set; }
        public bool HasCheckedIn { get; set; }
    }
}