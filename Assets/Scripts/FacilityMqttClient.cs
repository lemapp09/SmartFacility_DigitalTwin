using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MQTTnet;
using MQTTnet.Client;

public class FacilityMqttClient : MonoBehaviour
{
    private IMqttClient _mqttClient;
    private MqttClientOptions _options;

    // These credentials must match your HiveMQ Cloud Access Management settings
   
    public string brokerUrl = "your-cluster-id.s1.eu.hivemq.cloud";
    public int port = 8883;
    public string username = "facility_client";
    public string password = "your_password";
    public string topic = "facility/office/sensors";

    async void Start()
    {
        await ConnectToBroker();
    }

    private async Task ConnectToBroker()
    {
        Debug.Log("🔍 Attempting to initialize MQTT Client...");
        var factory = new MqttFactory();
        _mqttClient = factory.CreateMqttClient();

        _options = new MqttClientOptionsBuilder()
            .WithTcpServer(brokerUrl, port)
            .WithCredentials(username, password)
            .WithTls() 
            .WithCleanSession()
            .Build();

        _mqttClient.ApplicationMessageReceivedAsync += OnMessageReceived;

        try {
            Debug.Log($"📡 Connecting to {brokerUrl}...");
            var result = await _mqttClient.ConnectAsync(_options);
        
            // Log the specific ReasonCode from HiveMQ
            Debug.Log($"✅ Connection Result: {result.ResultCode}"); 
        
            if (_mqttClient.IsConnected) {
                await _mqttClient.SubscribeAsync(topic);
                Debug.Log($"📝 Subscribed to topic: {topic}");
            }
        }
        catch (System.Exception ex) {
            // This will now capture TLS or credential errors
            Debug.LogError($"❌ Connection Exception: {ex.GetType().Name} - {ex.Message}");
        }
    }

    private async Task OnMessageReceived(MqttApplicationMessageReceivedEventArgs e)
    {
        string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
        
        // CRITICAL: Move execution to the Main Thread for Unity 6
        await Awaitable.MainThreadAsync();
        
        ProcessData(payload);
    }

    private void ProcessData(string json)
    {
        // This is where you will parse the JSON and update the Digital Twin logic
        Debug.Log($"Telemetry Received on Main Thread: {json}");
    }

    private async void OnApplicationQuit()
    {
        if (_mqttClient!= null && _mqttClient.IsConnected)
        {
            await _mqttClient.DisconnectAsync();
            Debug.Log("Gracefully disconnected from MQTT Broker.");
        }
    }
}