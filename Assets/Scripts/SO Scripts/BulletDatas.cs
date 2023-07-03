using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bullet Data", menuName = "Scriptable Object/Bullet Data")]

public class BulletDatas : ScriptableObject
{
    public List<Bullet> bullets;
}