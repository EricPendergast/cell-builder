using Unity.Transforms;
using Unity.Mathematics;
using Unity.Entities;

using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;

// This system populates the singleton MouseComponent with the
// proper values.
[UpdateAfter(typeof(ParticleGridSystem))]
[UpdateBefore(typeof(ApplyCollisionsSystem))]
public class MouseInputSystem : SystemBase {
    ParticleGridSystem myParticleSystem;

    protected override void OnCreate() {
        World.EntityManager.CreateEntity(typeof(MouseComponent));

        myParticleSystem = World.GetOrCreateSystem<ParticleGridSystem>();
    }

    protected override void OnUpdate() {
        myParticleSystem.GetFinalJobHandle().Complete();

        var mouse = Mouse.current;
        var mouseData = GetSingleton<MouseComponent>();
        mouseData.mouseWorldPosition = (Vector2)Camera.main.ScreenToWorldPoint(mouse.position.ReadValue());

        if (mouse.leftButton.wasPressedThisFrame) {
            Entity particle = GetParticleAtPoint(mouseData.mouseWorldPosition);
            mouseData.selected = particle;
        } else if (mouse.leftButton.wasReleasedThisFrame) {
            mouseData.selected = default(Entity);
        }

        SetSingleton(mouseData);
    }


    private Entity GetParticleAtPoint(float2 point) {
        var nearbyIterator = new NearbyGridCellIterator<RigidParticleInfo>(
            ParticleGridSystem.ToGrid(point),
            myParticleSystem.grid
        );

        foreach (var rpInfo in nearbyIterator) {
            if (math.length(rpInfo.pos.Value.xy - point) < rpInfo.body.radius) {
                return rpInfo.entity;
            }
        }

        return default(Entity);
    }
}
