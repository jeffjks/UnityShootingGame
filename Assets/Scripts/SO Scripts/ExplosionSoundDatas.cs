using UnityEngine;

[CreateAssetMenu(fileName = "Explosion Sound Data", menuName = "Scriptable Object/Explosion Sound Data")]

public class ExplosionSoundDatas : ScriptableObject
{
    public SoundExplosionInfo[] soundInfos;
}