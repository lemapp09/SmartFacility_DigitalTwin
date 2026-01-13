using Unity.Properties;


public class FacilityTelemetry
{
    // Attributes to allow Unity 6 UI Toolkit to bind to these fields
    [CreateProperty] public string timestamp;
    [CreateProperty] public string zone;
    [CreateProperty] public string sensor;
    [CreateProperty] public float value;

    // Helper property for the UI to show a formatted string
    [CreateProperty] 
    public string DisplayValue => $"{sensor}: {value:F2}";
}