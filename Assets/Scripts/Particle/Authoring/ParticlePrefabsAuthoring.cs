using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class ParticlePrefabsAuthoring : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity {
    public GameObject simpleParticle;

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs) {
        referencedPrefabs.Add(simpleParticle);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var particlePrefabs = new ParticlePrefabsComponent {
            simpleParticle = conversionSystem.GetPrimaryEntity(simpleParticle),
        };
        dstManager.AddComponentData(entity, particlePrefabs);
    }
}
