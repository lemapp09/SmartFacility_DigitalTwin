using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using MQTTnet;
using MQTTnet.Client;

public class FacilityMqttClient : MonoBehaviour
{
    public UIDocument uiDocument;
    public string brokerUrl = "42c5cfb619974a12bf9b24741db7e636.s1.eu.hivemq.cloud";
    public string username = "facility_client";
    public string password = "DY;6A@kmJmC&L^#";
    public string topic = "facility/office/sensors";

    // Performance Caching
    private VisualElement _root;
    private Label _displayLabel;
    private IMqttClient _mqttClient;
    
    public SensorDisplayManager displayManager; 

    void Awake()
    {
        // Cache UI elements once at initialization to avoid per-update overhead
        if (uiDocument!= null)
        {
            _root = uiDocument.rootVisualElement;
            _displayLabel = _root.Q<Label>("FacilityDisplay");
        }
    }

    async void Start()
    {
        await ConnectToBroker();
    }

    private async Task ConnectToBroker()
    {
        var factory = new MqttFactory();
        _mqttClient = factory.CreateMqttClient();

        var options = new MqttClientOptionsBuilder()
           .WithTcpServer(brokerUrl, 8883)
           .WithCredentials(username, password)
           .WithTlsOptions(o => 
            {
                // Correct Fluent API for MQTTnet 4.x validation bypass
                o.WithCertificateValidationHandler(_ => true);
            })
           .WithCleanSession()
           .Build();

        _mqttClient.ApplicationMessageReceivedAsync += OnMessageReceived;
        await _mqttClient.ConnectAsync(options);
        await _mqttClient.SubscribeAsync(topic);
        Debug.Log("✅ Digital Thread Live: Unity connected and subscribed.");
    }

    private async Task OnMessageReceived(MqttApplicationMessageReceivedEventArgs e)
    {
        // Use PayloadSegment to minimize memory allocations and GC pressure
        string payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
        
        // Sync with Unity Main Thread using new Unity 6 Awaitables
        await Awaitable.MainThreadAsync();
        
        ProcessData(payload);
    }

    private void ProcessData(string json)
    {
        // 1. Deserialize using your existing FacilityTelemetry model
        FacilityTelemetry newData = JsonUtility.FromJson<FacilityTelemetry>(json);

        // 2. Update the Label (FacilityDisplay) if it exists
        if (_displayLabel != null)
        {
            _displayLabel.text = $"{newData.zone} | {newData.DisplayValue}";
        }

        // 3. Coordinate Mapping (String Parsing)
        // Extract Floor and Zone from newData.zone (e.g., "Zone_1A")
        string zoneId = newData.zone.Replace("Zone_", ""); // Result: "1A"
        int floorIndex = int.Parse(zoneId.Substring(0, 1)) - 1; // "1" -> 0
        int zoneIndex = zoneId[1] - 'A'; // 'A' -> 0, 'B' -> 1...

        // 4. Identify Sensor Type from the sensor string
        // e.g., "zone_1A_temp" or "zone_1A_energy"
        int typeIndex = -1;
        string sensorType = "";

        if (newData.sensor.Contains("temp")) { typeIndex = 0; sensorType = "temperature"; }
        else if (newData.sensor.Contains("energy")) { typeIndex = 1; sensorType = "energy"; }
        else if (newData.sensor.Contains("co2")) { typeIndex = 2; sensorType = "co2"; }

        // 5. Update Visuals
        if (typeIndex != -1 && displayManager != null)
        {
            // Update the 3D Matrix [Floor, Zone, Type]
            Color statusColor = displayManager.GetStatusColor(newData.sensor, newData.value);
            displayManager.UpdateSensorUI(floorIndex + 1, zoneIndex, typeIndex, statusColor);
        }
    }

    private async void OnApplicationQuit()
    {
        if (_mqttClient!= null && _mqttClient.IsConnected)
            await _mqttClient.DisconnectAsync();
    }
}