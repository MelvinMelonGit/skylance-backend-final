namespace skylance_backend.Data
{
    // For dropdown
    public record AvailableFlightDto(int Id, string Code, int SeatCapacity);

    // Request body for calculation
    public record OversalesRequest(int FlightId, double? SafetyFactor); // set default 0.8 later to cushion from the no-show passenger number

    // Response body (kept the human-friendly text)
    public record OversalesResponse(
        int FlightId,
        double ShowPercentage,     // 0..100
        int RecommendOversale,     // integer tickets
        string Rationale           // return human-friendly text
    );
}