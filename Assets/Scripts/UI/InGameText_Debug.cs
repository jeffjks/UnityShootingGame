using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class InGameText_Debug : MonoBehaviour
{
    public TextMeshProUGUI m_DebugText;

#if UNITY_EDITOR
    private void Update()
    {
        var count = BulletManager.EnemyBulletList.Count;
        m_DebugText.SetText($"Bullet: {count}");
    }
#else
    private void Start()
    {
        m_DebugText.enabled = false;
    }
#endif
}
