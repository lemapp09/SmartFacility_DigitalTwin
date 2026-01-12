
# Smart Facility Digital Twin: Operational Management Framework

**Status:** Phase 1 - Architectural Environment & Data Architecture Design

## 1. Executive Summary

This project is an expert-level **Informative Digital Twin** (Level 2+) of a multi-story office facility, developed using the **Unity 6 Engine**. It bridges the gap between architectural design and engineering logic by integrating real-time IoT telemetry with a high-performance 3D visualization layer.

The goal is to move beyond a static 3D model to create a "Single Pane of Glass" operations view, enabling facility managers to monitor energy consumption, occupancy patterns, and HVAC performance in a unified, immersive environment.

## 2. Technical Stack & "Polyglot" Logic

As a Digital Twin Engineer with a background in both engineering and architecture, I have architected this system to be both visually accurate and computationally robust.

* **Engine:** Unity 6 (6000.0.x) utilizing the **Universal Render Pipeline (URP)** for optimized WebGL performance.
* **Primary Logic:** **C#** leveraging the new **Unity 6 Runtime Data Binding** system (`[UXMLElement]`, `[UXMLAttribute]`) to eliminate UI boilerplate and maximize responsiveness.
* **Connectivity Middleware:** Custom scripts (utilizing my background in **Python** and **C#**) designed to interface with **MQTT** brokers for real-time sensor ingestion.
* **Environment & Assets:** Modular architectural shell constructed via **Synty POLYGON Office** assets, optimized for sub-2-second load times on web-based portfolios.
* **Graphics & Textures:** **Adobe Photoshop** for custom UI sprites and data-overlay materials.

## 3. The Digital Thread (System Architecture)

The project implements a full "Digital Thread"—a continuous flow of data from the physical (or simulated) asset to the end-user dashboard:

1. **Data Generation:** Simulated IoT sensor streams (Temperature, Humidity, Occupancy) via JSON/MQTT.
2. **Transport Layer:** Data transmission through a centralized MQTT Broker.
3. **Engine Integration:** Unity 6 C# Client subscribes to the broker and parses telemetry.
4. **Visual Interaction:** Runtime binding triggers visual changes (e.g., room color shifts, gauge updates) based on incoming data thresholds.

## 4. Initial Phase Objectives (In Progress)

* [x] **Repository Architecture:** Established modular folder structure for clean CI/CD iteration.
* [x] **Environment Layout:** Modular office floorplan designed for system-wide asset tagging.
* [ ] **Data Pipeline:** Integration of the MQTTnet library for C# telemetry polling.
* [ ] **UI Dashboard:** Layout of the operational HUD using the Unity 6 **UI Toolkit**.

## 5. Professional Background

I am a published author on the **Unity 6 Engine** with a deep foundation in engineering and architecture. My expertise lies in translating complex industrial datasets into interactive, actionable 3D experiences.

**Live Portfolio:** [lemapperson.com](https://lemapperson.com)

## 6. Technical Implementation Notes:

1. **Technical Adaptability: Lighting & Rendering Optimization**

  * **The Challenge:** Encountered Unity 6 URP "Reduced additional punctual light shadows" errors due to high point-light density in the modular office prefab.
  * **The Engineering Logic:** Managed the shadow atlas memory budget by converting point lights (which require 6 shadow maps per light) to spot lights (1 map) to reduce atlas load by over 80%.
  * **Outcome:** Implemented a hybrid lighting strategy using Baked Lightmaps for static architectural components and Forward+ Rendering for dynamic assets, ensuring a stable 60 FPS for WebGL deployment.

2. **System Interoperability:** Connectivity Layer Stability

  * **The Challenge:** Resolved assembly reference errors (System.Security.Cryptography) caused by a version mismatch between MQTTnet 5.0 (targeting.NET 8) and the Unity 6 scripting runtime (.NET Standard 2.1).
  * **The Logic:** Performed a strategic downgrade to MQTTnet 4.3.7, which is the most stable version for.NET Standard 2.1 projects, ensuring full compatibility with Unity's IL2CPP and Mono backends.
  * **DevOps Proficiency:** Restored the project from a nested Git repository state on a Mac mini M4, establishing a clean "Digital Thread" baseline for iterative deployment.

3. **Technical Adaptability:** Multi-Platform Data Layer Management

  * **The Challenge:** Encountered "externally-managed-environment" protections and shell pathing conflicts (`zsh: command not found: pip`) while configuring the data generation layer on macOS Sequoia (M4 architecture).
  * **The Engineering Logic:** Established a Python Virtual Environment (venv) to isolate simulation dependencies from the system interpreter, ensuring project reproducibility and portability. Proactively upgraded `pip` to v25.3 to leverage PEP 517/660 build standards and PEP 658 metadata efficiency, reducing dependency resolution overhead for the simulation stack.
  * **Outcome:** Created a robust, decoupled **Data Generation Layer** using `smartbuildsim` that produces deterministic JSON telemetry for the MQTT "Digital Thread," ensuring the Unity 6 engine remains performant and isolated from backend scripting conflicts.

4. **Engineering Methodology:** Configuration-as-Code for Data Reproducibility

  * **The Challenge:** Resolved a CLI `No such option` conflict by identifying a paradigm shift between initialization (flag-based) and generation (config-based) subcommands in `smartbuildsim`.
  * **The Engineering Logic:** Migrated generation parameters from ephemeral shell flags to a persistent `config.yaml` architecture. This ensures that the deterministic "Digital Thread" can be exactly replicated by any team member or recruiter using the same seed and schema settings.
  * **Outcome:** Established a version-controlled data generation pipeline that produces 14 days of multi-variant building telemetry (HVAC, occupancy, energy), providing the necessary complexity for the Unity 6 operational dashboard.

5. **System Architecture:** TLS-Secured Cloud Data Bridge

  * **The Challenge:** Establishing a secure "Digital Thread" between a local Python 3.13 simulation and a cloud-based MQTT broker while adhering to modern Paho-MQTT 2.x callback architectures.
  * **The Engineering Logic:** Implemented a decoupled bridge using `paho-mqtt` with TLS 1.2 encryption on port 8883. Utilized `CallbackAPIVersion.VERSION2` to ensure stability on the Python 3.13 runtime and mapped tabular CSV data to long-format JSON payloads for downstream C# ingestion.
  * **Outcome:** Successfully synchronized the physical-layer simulation with a secure cloud backbone, enabling bidirectional data flow with 1-second reporting latency—the foundation for a Level 3 Predictive Twin.

6. **Engineering Methodology:** Main-Thread Synchronization via Unity 6 Awaitables

  * **The Challenge:** MQTT telemetry ingestion occurs on an asynchronous background thread, preventing direct interaction with Unity’s non-thread-safe engine APIs (UI, Transforms, Materials).
  * **The Logic:** Leveraged the new Unity 6 `Awaitable.MainThreadAsync()` pattern within the `ApplicationMessageReceivedAsync` handler. This eliminates the need for legacy "Thread Dispatcher" queues and reduces context-switching overhead.
  * **Outcome:** Established a high-performance, low-latency data bridge that allows live IoT JSON payloads to safely update the Digital Twin’s visual state without causing engine deadlocks or frame-rate stutter.

7. **Mac mini M4 & Apple Silicon Optimization**

  * **Environment Management:** Optimized the asset pipeline for Apple Silicon (M4) by resolving Unity 6/Rosetta 2 package initialization quirks and ensuring NuGet.config files were correctly identified by the asset importer.

**Technical Documentation:** Available in the `/Docs` folder of this repository.
