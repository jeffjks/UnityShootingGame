using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Pooling Data", menuName = "Scriptable Object/Pooling Data")]

public class PoolingDatas : ScriptableObject
{
    [Serializable]
    public class PoolingInfo
    {
        public string objectName;
        public GameObject poolingObject;
        public int defaultNumber;
        public PoolingParent poolingParent;
    }
    
    public PoolingInfo[] poolingOutGameInfos;
    public PoolingInfo[] poolingInfos;
}