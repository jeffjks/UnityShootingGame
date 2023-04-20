using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public MoveVector m_MoveVector;
    
    [HideInInspector] public Vector2 m_Position2D;
}
