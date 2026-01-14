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

    private VisualElement _root;
    private Label _displayLabel;
    private IMqttClient _mqttClient;
    
    public SensorDisplayManager displayManager;
    public ZoneCameraManager cameraManager;

    void Awake()
    {
        if (uiDocument != null)
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
           .WithTlsOptions(o => o.WithCertificateValidationHandler(_ => true))
           .WithCleanSession()
           .Build();

        _mqttClient.ApplicationMessageReceivedAsync += OnMessageReceived;
        await _mqttClient.ConnectAsync(options);
        await _mqttClient.SubscribeAsync(topic);
        Debug.Log("✅ Digital Thread Live: Unity connected and subscribed.");
    }

    private async Task OnMessageReceived(MqttApplicationMessageReceivedEventArgs e)
    {
        string payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
        await Awaitable.MainThreadAsync();
        ProcessData(payload);
    }

    private void ProcessData(string json)
    {
        FacilityTelemetry newData = JsonUtility.FromJson<FacilityTelemetry>(json);

        // 1. Coordinate Mapping & String Parsing
        string zoneId = newData.zone.Replace("Zone_", ""); // Result: "1A"
        string floorNum = zoneId.Substring(0, 1);         // "1"
        string zoneLetter = zoneId.Substring(1, 1);        // "A"

        // 2. Identify Display Name for Sensor Type
        string sensorTypeDisplay = "Unknown";
        int typeIndex = -1;

        if (newData.sensor.Contains("temp")) { sensorTypeDisplay = "Temp"; typeIndex = 0; }
        else if (newData.sensor.Contains("energy")) { sensorTypeDisplay = "Power"; typeIndex = 1; }
        else if (newData.sensor.Contains("co2")) { sensorTypeDisplay = "CO2"; typeIndex = 2; }

        // 3. Format the Display Label: "Floor #/Zone (Letter)/Type - ###"
        if (_displayLabel != null)
        {
            // Convert float value to a whole number (rounded)
            int wholeValue = Mathf.RoundToInt(newData.value);
            string units = "";
            switch (sensorTypeDisplay)
            {
                case "Temp":
                    units = "C";
                    break;
                case "Power":
                    units = "kwh";
                    break;
                case "CO2":
                    units = "ppm";
                    break;
            }
            
            if (_displayLabel != null)
            {
                _displayLabel.text = $"Floor {floorNum}/Zone {zoneLetter}/{sensorTypeDisplay} - {wholeValue} {units}";
            }    }

        // 4. Update the Visual Grid and Automation
        if (typeIndex != -1 && displayManager != null)
        {
            int floorIndex = int.Parse(floorNum) - 1; // 0-indexed for matrix math
            int zoneIndex = zoneLetter[0] - 'A';      // 0-indexed for matrix math

            Color statusColor = displayManager.GetStatusColor(newData.sensor, newData.value);
            
            // floorIndex + 1 ensures we pass the 1-based floor number to the UI and Camera managers
            displayManager.UpdateSensorUI(floorIndex + 1, zoneIndex, typeIndex, statusColor);

            // Automatically switch camera if not locked
            if (cameraManager != null)
            {
                cameraManager.SwitchToZone(floorIndex + 1, zoneIndex, false);
            }
        }
    }

    private async void OnApplicationQuit()
    {
        if (_mqttClient != null && _mqttClient.IsConnected)
            await _mqttClient.DisconnectAsync();
    }
}