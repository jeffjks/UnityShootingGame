using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Speed", menuName = "Scriptable Object/Player Speed Data")]

public class PlayerSpeedDatas : ScriptableObject
{
    [Serializable]
    public struct PlayerSpeed
    {
        public int defaultSpeed;
        public int slowSpeed;
    }

    public PlayerSpeed[] playerSpeed;
}