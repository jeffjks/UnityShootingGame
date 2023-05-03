using UnityEngine;

[CreateAssetMenu(fileName = "Explosion Audio Data", menuName = "Scriptable Object/Explosion Audio Data")]

public class ExplosionAudioData : ScriptableObject
{
    [SerializeField]
    private AudioClip audio_explosionAirMedium_1;
    [SerializeField]
    private AudioClip audio_explosionAirMedium_2;
    [SerializeField]
    private AudioClip audio_explosionAirSmall;
    [SerializeField]
    private AudioClip audio_explosionMediumGround;
    [SerializeField]
    private AudioClip audio_explosionGroundSmall;
    [SerializeField]
    private AudioClip audio_explosionHuge_1;
    [SerializeField]
    private AudioClip audio_explosionHuge_2;
    [SerializeField]
    private AudioClip audio_explosionLarge;
}