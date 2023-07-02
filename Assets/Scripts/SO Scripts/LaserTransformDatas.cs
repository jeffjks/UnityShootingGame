using UnityEngine;

[CreateAssetMenu(fileName = "Laser Transform", menuName = "Scriptable Object/Laser Transform")]

public class LaserTransformDatas : ScriptableObject
{
    public Vector3[] fireLocalScale;
    public Vector3[] rushLocalScale;
    public Vector3[] hitLocalScale;
    public float[] laserWidth;
}