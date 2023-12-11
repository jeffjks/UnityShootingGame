using System;
using UnityEngine;

[Serializable]
public struct AssetList
{
    public string assetCategory;
    [TextArea(0, 10)]
    public string assetNames;
}

[CreateAssetMenu(fileName = "Ending Credit Message Data", menuName = "Scriptable Object/Ending Credit Message Data")]

public class EndingCreditMessageDatas : ScriptableObject
{
    public string categoryCredit;
    [TextArea(0, 10)]
    public string credit;
    
    [Space(10)]
    public string categoryAssetList;
    public AssetList[] assetLists;

    public string creditDate;
    public string unityVersion;
}