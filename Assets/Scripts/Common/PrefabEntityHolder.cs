using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Common
{
    public class PrefabEntityHolder : MonoBehaviour, IConvertGameObjectToEntity
    {
        public static List<Entity> PrefabEntity = new List<Entity>();
        public List<GameObject> prefabs;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            using BlobAssetStore blobAssetStore = new BlobAssetStore();
            prefabs.ForEach(prefab =>
            {
                Entity prefabEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(prefab,
                    GameObjectConversionSettings.FromWorld(dstManager.World, blobAssetStore));
                
                PrefabEntity.Add(prefabEntity);
            });
        }
    }
}