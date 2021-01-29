using Unity.Transforms;
using Unity.Entities;

using UnityEngine.InputSystem;
using MathematicsExtensions;

[UpdateAfter(typeof(MouseInputSystem))]
[UpdateBefore(typeof(ApplyCollisionsSystem))]
public class MouseDebugActionSystem : SystemBase {
    protected override void OnUpdate() {
        var mouse = Mouse.current;
        var mouseData = GetSingleton<MouseComponent>();
        var mousePosition = mouseData.mouseWorldPosition.xy0();

        if (mouse.rightButton.wasPressedThisFrame) {
            var prefabs = GetSingleton<ParticlePrefabsComponent>();
            Entity particle = EntityManager.Instantiate(prefabs.simpleParticle);
        
            EntityManager.SetComponentData(
                particle,
                new Translation{Value=mousePosition});
        }

        if (mouseData.selected != default(Entity)) {
            World.EntityManager.SetComponentData(
                mouseData.selected,
                new Translation{Value=mousePosition});
        }
    }
}
