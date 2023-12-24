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

    private void Update()
    {
#if UNITY_EDITOR
        var count = BulletManager.EnemyBulletList.Count;
        m_DebugText.SetText($"Bullet: {count}");
#endif
    }
}
