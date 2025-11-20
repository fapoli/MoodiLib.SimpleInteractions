# MoodyLib.Interactions

An extensible and lightweight interaction framework for Unity that cleanly **separates interaction logic from visual presentation**.  
Detection (raycasts, distance checks, conditions, triggers) is handled independently from how interactions are displayed, enabling you to plug in your own UI, IMGUI, crosshair logic, or any other indicator strategy without modifying gameplay code.

## Contents

- **Interactor MonoBehaviour**  
  Core interaction manager. Performs raycasts from a given origin (usually the camera) to:
    - Determine the currently selected interactable.
    - Collect nearby interactables within a configurable radius and layer mask.
    - Invoke registered indicator renderer strategies (e.g. IMGUI icon renderer).

- **IInteractive / AbstractInteractive**  
  Contract and base class for interactable objects. Includes:
    - Single‑use logic.
    - Condition handling using `ICondition` components.
    - Interaction flow (`TryInteract` → `OnInteract`).
    - Optional post‑interaction triggers (`AbstractTrigger`).

- **ICondition**  
  Interface for components that define dynamic interaction conditions.  
  Used by `AbstractInteractive` to determine whether interaction is currently allowed. Can be used for example for locked doors (by defining a HasItemCondition)

- **ITriggerable / AbstractTrigger**  
  Base for all triggerable actions. Can be:
    - Triggered manually,
    - Triggered automatically when the player enters a trigger collider,
    - Used as post‑actions on interactables.

- **Indicator System (optional)**  
  Easily extendable, includes:
    - `AbstractIndicatorRendererStrategy` (base class)
    - `IconRenderStrategy` (default IMGUI icon renderer)
    - `InteractionIndicatorPivot` (visual‑only placement helper)

- **Prefabs Provided**
    - **Interactor.prefab** — a ready‑to‑use interaction manager with `IconRenderStrategy` included.
    - **AudioTrigger.prefab** — trigger that plays an audio clip using a local AudioSource.
    - **SetActiveTrigger.prefab** — trigger that enables or disables a target GameObject.


## Install via Git URL

1. Open **Window > Package Manager** in Unity.
2. Click the **+** button.
3. Select **“Add package from Git URL…”**.
4. Paste:

   ```
   https://github.com/fapoli/MoodiLib.Interactions.git
   ```

Unity will download the package and display it under **Packages**.


## How to use

### 1. Add the interactor

You can either:

- Drag **Interactor.prefab** into your scene (**recommended**),  
  **OR**
- Add the `Interactor` component manually to a GameObject. Optionally, for visual indicators, add at least one indicator strategy (e.g., `IconRenderStrategy`).

Remember to drag the object that will serve as the raycast origin (usually your main camera if it's an FPS) into the `Raycast Origin Transform` field of the `Interactor` component.

### 2. Create an interactor (player / camera)

Call `CheckInteractions()` every frame:

```csharp
void Update() {
    var selected = _interactor.CheckInteractions();

    if (selected != null && Input.GetKeyDown(KeyCode.E)) {
        selected.TryInteract(gameObject);
    }
}
```

### 3. Create interactable objects

Add colliders on an interaction layer and derive from `AbstractInteractive`:

```csharp
public class DoorInteractive : AbstractInteractive {
    protected override void OnInteract(GameObject caller) {
        // Open the door
    }
}
```

### 4. Add conditions (optional)

Implement `ICondition` to dynamically allow or block interaction.

### 5. Use triggers / post actions (optional)

Included prefabs:

- `AudioTrigger.prefab`
- `SetActiveTrigger.prefab`

Or create custom triggers by deriving from `AbstractTrigger`.

### 6. Indicator rendering (optional)

If you use **Interactor.prefab**, the default IMGUI icon strategy is already attached.  
Otherwise, add an indicator strategy manually.


## About

MoodyLib.Interactions is designed to keep gameplay and UI concerns separate.  
Interaction logic remains clean, while presentation layers (icons, UI, crosshairs, outlines) can be swapped freely using strategy components.

Check the documentation inside the corresponding classes to learn about the different parameters you can send to the methods described above.

