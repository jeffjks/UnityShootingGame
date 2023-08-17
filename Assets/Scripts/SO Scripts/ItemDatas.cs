using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item Data", menuName = "Scriptable Object/Item Data")]

public class ItemDatas : ScriptableObject
{
    [Serializable]
    public class ItemElement
    {
        public ItemType ItemType;
        public Item Item;
    }

    public List<ItemElement> itemElementList;
}