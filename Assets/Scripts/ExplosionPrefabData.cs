using UnityEngine;

[CreateAssetMenu(fileName = "Explosion Prefab Data", menuName = "Scriptable Object/Explosion Prefab Data")]

public class ExplosionPrefabData : ScriptableObject
{
    [SerializeField]
    private GameObject explosionGroundLarge;
    [SerializeField]
    private GameObject explosionGroundMedium;
    [SerializeField]
    private GameObject explosionGroundSmall;
    [SerializeField]
    private GameObject explosionLandMine;
    [SerializeField]
    private GameObject explosionLarge;
    [SerializeField]
    private GameObject explosionMedium;
    [SerializeField]
    private GameObject explosionSimpleMedium;
    [SerializeField]
    private GameObject explosionSmall;
    [SerializeField]
    private GameObject explosionSimpleSmall;
    [SerializeField]
    private GameObject explosionStarSmoke;
}