using System.Collections;
using System.Collections.Generic;
using UDEV.SPM;
using UnityEngine;

namespace UDEV.GhostDefense
{
    [System.Serializable]
    public class CollectableItem
    {
        [Range(0f, 1f)] public float spawnRate;
        public int amount;
        [PoolerKeys(target = PoolerTarget.NONE)]
        public string collectablePool;
    } 

    [CreateAssetMenu(fileName = "New Collectable Item Data", menuName = "Data/Collectable Item Data")]
    public class CollectableItemData : ScriptableObject
    {
        public CollectableItem[] items;
    }
}
