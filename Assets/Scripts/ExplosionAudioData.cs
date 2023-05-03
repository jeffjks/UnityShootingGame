using UnityEngine;

[CreateAssetMenu(fileName = "Explosion Audio Data", menuName = "Scriptable Object/Explosion Audio Data")]

public class ExplosionAudioData : ScriptableObject
{
    public AudioClip audio_explosionAirMedium_1;
    public AudioClip audio_explosionAirMedium_2;
    public AudioClip audio_explosionAirSmall;
    public AudioClip audio_explosionMediumGround;
    public AudioClip audio_explosionGroundSmall;
    public AudioClip audio_explosionHuge_1;
    public AudioClip audio_explosionHuge_2;
    public AudioClip audio_explosionLarge;
}