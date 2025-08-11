namespace skylance_backend.Data
{
    public class PassengerDetailsDTO
    {
        public string PassengerId { get; set; }
        public string FlightNumber { get; set; }
        public string Airline { get; set; }
        public string PassengerName { get; set; }
        public string Class { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime? CheckinTime { get; set; }
        public string MembershipTier { get; set; }
        public DateTime DateOfTravel { get; set; }
        public string BookingStatus { get; set; }
        public string BaggageAllowance { get; set; }
        public string BaggageChecked { get; set; }
        public string SeatNumber { get; set; }
        public string CheckinStatus { get; set; }
        public string BookingReferenceNumber { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PassportNumber { get; set; }
        public string DepartureCity { get; set; }
        public string ArrivalCity { get; set; }
        public string SpecialRequests { get; set; }
        public string BoardingStatus { get; set; }
        public string Prediction {get; set;}
    }
}