using Unity.Transforms;
using Unity.Entities;
using Unity.Mathematics;

using UnityEngine.InputSystem;
using MathematicsExtensions;

[UpdateAfter(typeof(MouseInputSystem))]
[UpdateInGroup(typeof(BeforeApplyVelocityGroup))]
public class MouseDebugActionSystem : SystemBase {
    protected override void OnUpdate() {
        var mouse = Mouse.current;
        var mouseData = GetSingleton<MouseComponent>();
        var mousePosition = mouseData.mouseWorldPosition.xy0();

        //if (mouse.rightButton.isPressed) {
        if (mouse.rightButton.wasPressedThisFrame) {
            var prefabs = GetSingleton<ParticlePrefabsComponent>();
            Entity particle = EntityManager.Instantiate(prefabs.simpleParticle);
        
            EntityManager.SetComponentData(
                particle,
                new Translation{Value=mousePosition});
            EntityManager.SetComponentData(
                particle,
                new PreviousPosition{Value=mousePosition.xy});
        }

        if (mouseData.selected != default(Entity)) {
            DragParticle(mouseData);
        }
    }

    private void DragParticle(MouseComponent mouseData) {
        var pos = EntityManager.GetComponentData<Translation>(mouseData.selected);
        var vel = EntityManager.GetComponentData<Velocity>(mouseData.selected);

        float2 force = ParticleMath.DampedSpringForce(
                pos.xy(), vel.xy(),
                mouseData.mouseWorldPosition, mouseData.mouseDelta/Time.DeltaTime,
                0, //Spring length
                100, 20);//Spring constant and damping

        vel.Value += force*Time.DeltaTime;

        EntityManager.SetComponentData(
            mouseData.selected,
            pos);
        EntityManager.SetComponentData(
            mouseData.selected,
            vel);

    }
}
