using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDEV.GhostDefense
{
    [System.Serializable]
    public class ShopItem
    {
        public int price;
        public string heroName;
        public Sprite preview;
        public Sprite avatar;
        public Player heroPb;
    }

    [CreateAssetMenu(fileName = "New Shop Item Data", menuName = "Data/Shop Item Data")]
    public class ShopItemData : ScriptableObject
    {
        public ShopItem[] items;
    }
}
