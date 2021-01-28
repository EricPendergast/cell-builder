using Unity.Transforms;
using Unity.Mathematics;
using Unity.Entities;

using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;

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
        var mousePosition = (Vector2)Camera.main.ScreenToWorldPoint(mouse.position.ReadValue());
        var mouseData = GetSingleton<MouseComponent>();

        if (mouse.leftButton.wasPressedThisFrame) {
            Entity particle = GetParticleAtPoint(mousePosition);
            mouseData.selected = particle;
        } else if (mouse.leftButton.wasReleasedThisFrame) {
            mouseData.selected = default(Entity);
        }


        if (mouseData.selected != default(Entity)) {
            World.EntityManager.SetComponentData(mouseData.selected, new Translation{Value=(Vector3)mousePosition});
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
