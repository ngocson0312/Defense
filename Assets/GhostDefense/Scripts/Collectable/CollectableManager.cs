using System.Collections;
using System.Collections.Generic;
using UDEV.SPM;
using UnityEngine;

namespace UDEV.GhostDefense {
    public class CollectableManager : Singleton<CollectableManager>
    {
        public CollectableItemData data;

        public void Spawn(Vector3 pos)
        {
            if (!data) return;

            var items = data.items;

            if (items == null || items.Length <= 0) return;

            float rateChecking = Random.Range(0f, 1f);

            for (int i = 0; i < items.Length; i++)
            {
                CollectableItem item = items[i];
                if (item == null || item.spawnRate < rateChecking) continue;

                for (int j = 0; j < item.amount; j++)
                {
                    GameObject collectableClone = PoolersManager.Ins.Spawn(PoolerTarget.NONE, item.collectablePool, pos, Quaternion.identity);
                    if(!collectableClone) continue;
                    Collectable collectable = collectableClone.GetComponent<Collectable>();
                    collectable?.Init();
                }
            }
        }
    }
}
