namespace SharedModels;

public class CalculationRequestDTO
{
    public double NumberOne { get; set; }
    public double NumberTwo { get; set; }
    public CalculationType CalculationType { get; set; }
    public Dictionary<string, object>? Headers { get; } = new();
}