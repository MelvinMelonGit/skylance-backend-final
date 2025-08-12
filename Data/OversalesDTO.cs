namespace skylance_backend.Data
{
    // drop down
    public record AvailableFlightDto(int Id, string Code, int SeatCapacity);

    // NEW: request for historical-average by flight number
    public record OversalesRequest(
        string FlightNumber,
        int? Capacity,          // optional (use if creating a new flight and you already know capacity)
        double? SafetyFactor    // 0..1; defaults to 0.8 if null/invalid
    );

    // NEW: response keyed by flight number
    public record OversalesResponse(
        string FlightNumber,
        double ShowPercentage,  // 0..100
        int RecommendOversale,  // integer tickets
        string Rationale
    );
}