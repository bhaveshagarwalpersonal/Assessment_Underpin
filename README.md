# 2D Elevator Simulation

A realistic multi-elevator control system built in Unity 6, demonstrating smart dispatching, smooth movement, and modular game architecture.

**Unity Version:** `6000.3.20f1`

---

## Architecture & Design Overview

The project is structured using a **modular component-based architecture**, separating UI input, business logic, and data handling to ensure loose coupling and maintainability.

### Core Modules

1. **Input Layer (UI)**
   - `FloorButton`: Captures user clicks and generates `ElevatorRequest` commands.
   
2. **Logic Layer (Controllers)**
   - `ElevatorDispatcher`: The central decision-maker. It calculates a "score" for each elevator based on **distance** and **direction alignment** to assign the most suitable elevator.
   - `ElevatorController`: Manages a single elevator's state (Idle, Up, Down). It builds dynamic stop-lists and handles smooth movement using `Mathf.MoveTowards`.

3. **Data Layer (Models)**
   - `ElevatorRequest`: DTO (Data Transfer Object) for floor calls.
   - `ElevatorQueue`: Manages pending requests per elevator with built-in deduplication.
   - `FloorManager`: A centralized service providing Y-axis positions for any floor number.

---

### Software Design Patterns Used

- **Dispatcher Pattern**: Decouples the floor buttons from the elevator movement logic, allowing the system to scale easily to more elevators.
- **State Pattern**: `ElevatorController` uses directional states (Up/Down) to intelligently build stop-lists and avoid unnecessary backtracking.
- **Observer Pattern**: Utilizes C# `Action` events (e.g., `OnArrivedAtFloor`) to update UI elements and trigger door animations without tight coupling between scripts.
- **Command Pattern**: Encapsulates floor requests into reusable `ElevatorRequest` objects.
- **Service Locator**: `FloorManager` acts as a singleton-like service for global floor coordinate lookups.

---

### Logic Flow

1. **User Input** → Pressing a floor button creates an `ElevatorRequest`.
2. **Dispatching** → The `Dispatcher` scores all 3 elevators (favoring idle elevators and those already moving in the requested direction).
3. **Queue Assignment** → The best elevator adds the request to its `ElevatorQueue`.
4. **Movement** → The elevator builds a sorted stop-list, moves smoothly to each target floor via `Update()`, and fires an arrival event when it reaches the destination.
5. **UI Feedback** → The `ElevatorUI` component listens to arrival events to update the current floor display in real-time.

---

### Getting Started

1. Clone the repository.
2. Open the project in **Unity 6000.3.20f1**.
3. Open the main scene.
4. Play The game in playmode
