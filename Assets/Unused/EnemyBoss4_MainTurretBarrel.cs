﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// UNUSED SCRIPT

public class EnemyBoss4_MainTurretBarrel : MonoBehaviour
{
    private float m_Pos_Z1;
    private float m_Pos_Z2 = -0.32f;

    void Start()
    {
        m_Pos_Z1 = transform.localPosition.z;
    }

    public IEnumerator ShootAnimation() {
        int frame;

        frame = 100 * Application.targetFrameRate / 1000;
        for (int i = 0; i < frame; ++i) {
            float t_posz = AC_Ease.ac_ease[(int)EaseType.Linear].Evaluate((float) (i+1) / frame);
            float localPosition_z = Mathf.Lerp(m_Pos_Z1, m_Pos_Z2, t_posz);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, localPosition_z);
            yield return new WaitForMillisecondFrames(0);
        }
        
        yield return new WaitForMillisecondFrames(100);

        frame = 300 * Application.targetFrameRate / 1000;
        for (int i = 0; i < frame; ++i) {
            float t_posz = AC_Ease.ac_ease[(int)EaseType.Linear].Evaluate((float) (i+1) / frame);
            float localPosition_z = Mathf.Lerp(m_Pos_Z2, m_Pos_Z1, t_posz);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, localPosition_z);
            yield return new WaitForMillisecondFrames(0);
        }
        yield break;
    }
}
