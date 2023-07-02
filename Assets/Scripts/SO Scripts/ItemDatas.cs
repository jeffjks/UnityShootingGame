using UnityEngine;

[CreateAssetMenu(fileName = "Item Data", menuName = "Scriptable Object/Item Data")]

public class ItemDatas : ScriptableObject
{
    public ItemType itemType;
    public int itemScore;
    public int activeTimer;
}