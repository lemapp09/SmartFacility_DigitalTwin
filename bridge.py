import time
import json
import pandas as pd
import paho.mqtt.client as mqtt
from paho.mqtt.enums import CallbackAPIVersion

# --- HiveMQ Cloud Configuration ---
# Found in the 'Overview' tab of your HiveMQ console
BROKER = "42c5cfb619974a12bf9b24741db7e636.s1.eu.hivemq.cloud" 
PORT = 8883
USER = "facility_client"
PASS = "DY;6A@kmJmC&L^#"
TOPIC = "facility/office/sensors"

# --- Callback Functions ---
def on_connect(client, userdata, flags, rc, properties=None):
    if rc == 0:
        print("✅ Successfully connected to HiveMQ Cloud")
    else:
        print(f"❌ Connection failed with result code {rc}")

# --- Initialize Client ---
# Using VERSION2 is essential for compatibility with Python 3.13/Paho 2.x
client = mqtt.Client(CallbackAPIVersion.VERSION2)
client.username_pw_set(USER, PASS)
client.tls_set() # Mandatory for secure 8883 connection
client.on_connect = on_connect

print(f"Connecting to {BROKER}...")
client.connect(BROKER, PORT, keepalive=60)
client.loop_start()

# --- Data Streaming Logic ---
try:
    # Load the generated dataset from the 'outputs' directory
    df = pd.read_csv("outputs/dataset.csv")
    print(f"Loaded {len(df)} rows of telemetry. Starting stream...")

    for index, row in df.iterrows():
        # Convert the row to a JSON payload for the Unity C# parser
        payload = row.to_json()
        
        # Publish to the topic
        client.publish(TOPIC, payload, qos=1)
        print(f"Published Row {index}: {payload[:80]}...")
        
        # Simulate real-time sensor intervals (e.g., 1 second)
        time.sleep(1) 

except KeyboardInterrupt:
    print("\nStopping stream...")
except FileNotFoundError:
    print("❌ Error: outputs/dataset.csv not found. Did you run the generator?")
finally:
    client.loop_stop()
    client.disconnect()