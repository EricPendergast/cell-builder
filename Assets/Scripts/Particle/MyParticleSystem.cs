using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Burst;
using Unity.Jobs;


[UpdateBefore(typeof(TransformSystemGroup))]
public class MyParticleSystem : SystemBase {

    protected override void OnUpdate() {
        float deltaTime = Time.DeltaTime;
        Entities
            .WithName("ParticleSystem")
            .ForEach((ref Translation position, in Velocity velocity) => {
                    position.Value += new float3(velocity.Value, 0)*deltaTime;
            })
            .ScheduleParallel();
    }
}
