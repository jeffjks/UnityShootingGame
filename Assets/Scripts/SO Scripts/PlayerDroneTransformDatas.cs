using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Drone Transform Data", menuName = "Scriptable Object/Player Drone Transform Data")]

public class PlayerDroneTransformDatas : ScriptableObject
{
    [Serializable]
    public class TransformData
    {
        public Vector3 positionData;
        public Vector3 rotationData;
    }

    public TransformData[] shotTransformData;
    public TransformData[] laserTransformData;
}
