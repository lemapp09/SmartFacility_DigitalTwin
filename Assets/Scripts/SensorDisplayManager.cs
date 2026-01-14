using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Threading.Tasks; // Add this for the delay

public class SensorDisplayManager : MonoBehaviour
{
    private VisualElement[,,] sensorMatrix = new VisualElement[6, 7, 3];
    private string[] zoneLetters = { "A", "B", "C", "D", "E", "F", "G" };
    private string[] typeNames = { "Temp", "Power", "CO2" };
    public ZoneCameraManager cameraManager; 

    // Colors
    private Color colorHealthy = new Color(0.2f, 0.8f, 0.2f); // Green
    private Color colorWarning = new Color(1f, 0.8f, 0f);    // Yellow
    private Color colorDanger = new Color(0.9f, 0.1f, 0.1f);  // Red

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        CacheUIElements(root);
        InitializeGreen();
        
        // Start the stress test
        // StartCoroutine(TestUIFlow());
    }

    private void CacheUIElements(VisualElement root)
    {
        for (int f = 0; f < 6; f++)
        {
            for (int z = 0; z < 7; z++)
            {
                int floorNum = f + 1;
                int zoneIdx = z;
                bool containerClickRegistered = false; // Prevent multiple registrations

                for (int t = 0; t < 3; t++)
                {
                    string elementName = $"{floorNum}{zoneLetters[z]}_{typeNames[t]}";
                    VisualElement element = root.Q<VisualElement>(elementName);
            
                    if (element != null)
                    {
                        sensorMatrix[f, z, t] = element;
                    
                        if (!containerClickRegistered)
                        {
                            VisualElement cellContainer = element.parent;
                            if (cellContainer != null && cellContainer.ClassListContains("data-cell"))
                            {
                                cellContainer.RegisterCallback<ClickEvent>(evt => {
                                    // Add a null check here to prevent the crash if assignment is missing
                                    if (cameraManager != null)
                                        cameraManager.SwitchToZone(floorNum, zoneIdx, true);
                                    else
                                        Debug.LogError("CameraManager is not assigned on SensorDisplayManager!");
                                });
                                containerClickRegistered = true; 
                            }
                        }
                    }
                }
            }
        }
    }

    private void InitializeGreen()
    {
        foreach (var element in sensorMatrix)
        {
            if (element != null) ApplyColor(element, colorHealthy);
        }
    }

    private IEnumerator TestUIFlow()
    {
        // 7 elements per minute = roughly one change every 8.5 seconds
        float interval = 60f / 7f; 

        while (true)
        {
            yield return new WaitForSeconds(interval);

            // 1. Pick random indices
            int f = Random.Range(0, 6);
            int z = Random.Range(0, 7);
            int t = Random.Range(0, 3);
            
            VisualElement target = sensorMatrix[f, z, t];
            if (target == null) continue;

            // 2. Change to random alert color (Red or Yellow)
            Color alertColor = (Random.value > 0.5f) ? colorDanger : colorWarning;
            ApplyColor(target, alertColor);

            // 3. Return to Green after 3 seconds (nested coroutine)
            StartCoroutine(ResetAfterDelay(target, 3f));
        }
    }

    private IEnumerator ResetAfterDelay(VisualElement element, float delay)
    {
        yield return new WaitForSeconds(delay);
        ApplyColor(element, colorHealthy);
    }

    private void ApplyColor(VisualElement element, Color color)
    {
        // Handle Triangle icon tint vs Square/Circle background
        if (element.ClassListContains("shape-triangle"))
        {
            element.style.unityBackgroundImageTintColor = color;
            element.style.backgroundColor = Color.clear;
        }
        else
        {
            element.style.backgroundColor = color;
        }
    }
    
    /// <summary>
    /// Updates a specific sensor icon in the UI grid.
    /// </summary>
    public void UpdateSensorUI(int floor, int zoneIndex, int typeIndex, Color statusColor)
    {
        if (floor < 1 || floor > 6 || zoneIndex < 0 || zoneIndex > 6 || typeIndex < 0 || typeIndex > 2)
            return;

        VisualElement target = sensorMatrix[floor - 1, zoneIndex, typeIndex];
        if (target != null)
        {
            ApplyColor(target, statusColor);
        
            // Trigger the Pulse Animation
            TriggerPulse(target, statusColor);

        }
    }

    private async void TriggerPulse(VisualElement element,  Color statusColor)
    {
        if (element == null) return;

        // 2. Execute the Pulse
        element.AddToClassList("sensor-pulse");

        // Wait for the pulse duration (matches your USS 0.1s + buffer)
        await Awaitable.WaitForSecondsAsync(0.15f);

        // 3. Remove the class
        element.RemoveFromClassList("sensor-pulse");
        
        ApplyColor(element, statusColor);
    }
    
    public Color GetStatusColor(string type, float value)
    {
        // Split the incoming string (e.g., "zone_1B_temp") by the underscore
        string[] parts = type.ToLower().Split('_');
    
        // Grab the last part of the string (e.g., "temp", "energy", or "co2")
        string sensorType = parts[parts.Length - 1];

        switch (sensorType)
        {
            case "temp": // Updated from 'temperature' to match your sensor data
                if (value < 18f || value > 27f) return colorDanger;
                if (value > 24f) return colorWarning;
                return colorHealthy;

            case "energy":
                if (value > 450f) return colorDanger;
                if (value > 300f) return colorWarning;
                return colorHealthy;

            case "co2":
                if (value > 1200f) return colorDanger;
                if (value > 800f) return colorWarning;
                return colorHealthy;

            default:
                // Return gray if the split or the type doesn't match expected values
                return Color.gray;
        }
    }
    
}