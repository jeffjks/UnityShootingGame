using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public struct HitCountBonus
{
    public int hitCount;
    public int bonusPercent;
}

[CreateAssetMenu(fileName = "Hit Count Const Data", menuName = "Scriptable Object/Hit Count Const Data")]

public class HitCountConstData : ScriptableObject
{
    public int HitCountDecreasingFrame;
    public int HitCountDecreasingPeriodFrame;
    public int HitCountLaserMaxCount;
    public int MaxHitCount;
    public int MinHitCount;

    public List<HitCountBonus> hitCountBonus;
}