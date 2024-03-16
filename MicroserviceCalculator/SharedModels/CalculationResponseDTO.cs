namespace SharedModels;

public class CalculationResponseDTO
{
    
    public double NumberOne { get; set; }
    public double NumberTwo { get; set; }
    public double CalculationResult {  get; set; }
    public CalculationType CalculationType { get; set; }
    public Dictionary<string, object>? Headers { get; } = new();

}