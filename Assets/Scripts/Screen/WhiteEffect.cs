using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteEffect : MonoBehaviour
{
    public void OnAnimationEnd()
    {
        gameObject.SetActive(false);
    }
}
