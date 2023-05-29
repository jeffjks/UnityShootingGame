using UnityEngine;

[CreateAssetMenu(fileName = "Attributes Details Window Data", menuName = "Scriptable Object/Attributes Details Window Data")]

public class AttributesDetailsWindowDatas : ScriptableObject
{
    public AttributeType AttributeType;
    public string AttributeName;
    public string NativeAttributeName;
    public DetailsWindowElement[] DetailsWindowElements;
}