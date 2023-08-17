using UnityEngine;

[CreateAssetMenu(fileName = "Item Info Data", menuName = "Scriptable Object/Item Info Data")]

public class ItemInfoDatas : ScriptableObject
{
    public ItemType itemType;
    public int itemScore;
    public int activeTimer;
}