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
        FacilityTelemetry newData = JsonUtility.FromJson<FacilityTelemetry>(json);

        if (_displayLabel!= null)
        {
            // Procedural update: efficient, crash-proof, and easily debuggable
            _displayLabel.text = $"{newData.sensor.ToUpper()}: {newData.value:F2} (Zone: {newData.zone})";
        }

        // Trigger Phase 4: Spatial alerts
        // FindObjectOfType<FacilityZoneManager>().NotifyData(newData);
    }

    private async void OnApplicationQuit()
    {
        if (_mqttClient!= null && _mqttClient.IsConnected)
            await _mqttClient.DisconnectAsync();
    }
}