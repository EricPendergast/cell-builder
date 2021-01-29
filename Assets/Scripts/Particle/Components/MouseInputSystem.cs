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
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(mouse.position.ReadValue());
        mousePosition.z = 0;
        var mouseData = GetSingleton<MouseComponent>();

        if (mouse.leftButton.wasPressedThisFrame) {
            Entity particle = GetParticleAtPoint((Vector2)mousePosition);
            mouseData.selected = particle;
        } else if (mouse.leftButton.wasReleasedThisFrame) {
            mouseData.selected = default(Entity);
        }

        if (mouse.rightButton.wasPressedThisFrame) {
            var prefabs = GetSingleton<ParticlePrefabsComponent>();
            Entity particle = EntityManager.Instantiate(prefabs.simpleParticle);

            EntityManager.SetComponentData(particle, new Translation{Value=mousePosition});
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
