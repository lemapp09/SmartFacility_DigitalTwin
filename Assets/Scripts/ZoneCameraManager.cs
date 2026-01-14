using UnityEngine;
using UnityEngine.UIElements;

public class ZoneCameraManager : MonoBehaviour
{
    public UIDocument uiDocument;
    public Camera[] zoneCameras; // Array of 42 cameras
    public RenderTexture displayTexture;
    
    private Image _uiMonitor;
    private Button _unlockButton;
    public bool IsLocked { get; private set; } = false;

    void Start()
    {
        if (uiDocument == null)
        {
            uiDocument = GetComponent<UIDocument>();
        }

        var root = uiDocument.rootVisualElement;
        
        // Find the Unlock Button and hide it initially
        _unlockButton = root.Q<Button>("UnlockCameraButton");
        if (_unlockButton != null)
        {
            _unlockButton.clicked += UnlockDisplay;
            _unlockButton.style.display = DisplayStyle.None;
        }
        
        // Target your 900x500 Image element
        _uiMonitor = root.Q<Image>("ZoneCameraDisplay");
        
        if (_uiMonitor != null)
        {
            // In UI Toolkit, an Image element can accept a RenderTexture directly
            _uiMonitor.image = displayTexture;
            
            // Optional: Ensure the image aspect ratio matches your 900x500 display
            _uiMonitor.scaleMode = ScaleMode.ScaleAndCrop;
        }
        else
        {
            Debug.LogError("ZoneCameraDisplay not found in UI Document.");
        }
            
        // Disable all cameras initially to save performance
        foreach (var cam in zoneCameras) 
        {
            cam.enabled = false;
        }
    }

    /// <summary>
    /// Switches the active camera to the specified floor and zone.
    /// </summary>
    public void SwitchToZone(int floor, int zoneIndex, bool isManualClick)
    {
        // If the display is locked by a user, ignore automated (MQTT) requests
        if (IsLocked && !isManualClick) return;
        
        // Calculate the 1D index from floor (1-6) and zone (0-6)
        int index = ((floor - 1) * 7) + zoneIndex;
        
        if (index >= 0 && index < zoneCameras.Length)
        {
            // 1. Disable all cameras
            foreach (var cam in zoneCameras) 
            {
                cam.enabled = false;
            }

            // 2. Enable and route the target camera
            Camera targetCam = zoneCameras[index];
            targetCam.targetTexture = displayTexture;
            targetCam.enabled = true;
            
            // If this was a manual click, activate the lock
            if (isManualClick)
            {
                IsLocked = true;
                if (_unlockButton != null) _unlockButton.style.display = DisplayStyle.Flex;
            }
        }
    }
    
    private void UnlockDisplay()
    {
        IsLocked = false;
        if (_unlockButton != null) _unlockButton.style.display = DisplayStyle.None;
    }
}