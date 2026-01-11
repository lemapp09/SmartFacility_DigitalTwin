
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

**Technical Documentation:** Available in the `/Docs` folder of this repository.
