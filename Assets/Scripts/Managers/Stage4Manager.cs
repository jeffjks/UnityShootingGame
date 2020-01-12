using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Stage4Manager : StageManager
{
    [Space(10)]
    public GameObject m_Test;
    public GameObject m_ShipSmall_1, m_ShipSmall_2, m_ShipMedium, m_ShipLarge, m_ShipCarrier_1, m_TankLarge_3,
    m_PlaneSmall_1, m_PlaneSmall_2, m_PlaneSmall_3, m_ItemHeli_1, m_ItemHeli_2, m_PlaneMedium_5;

    private float m_BackgroundPos;

    void Awake()
    {
        m_Stage = 3;
    }

    protected override IEnumerator MainTimeLine()
    {
        yield return new WaitForSeconds(1f);
        InitEnemies();
        SetBackgroundSpeed(1.12f);

        yield return new WaitForSeconds(96f);
        SetBackgroundSpeed(0f, 1.2f);
        yield return new WaitForSeconds(1.2f);

        while(m_BackgroundPos < 270f) {
            float c_size = 1f;
            SetBackgroundSpeed(new Vector3(c_size*Mathf.Cos(Mathf.Deg2Rad * (180f + m_BackgroundPos)), 0f, c_size*Mathf.Sin(Mathf.Deg2Rad * (180f + m_BackgroundPos))));
            m_BackgroundPos += 4f * Time.deltaTime;
            yield return null;
        }
        yield break;
    }

    protected override IEnumerator TestTimeLine()
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(MiddleBossStart(new Vector3(0f, 2.5f, Depth.ENEMY), 1f)); // Middle Boss
        yield break;
    }

    protected override IEnumerator EnemyTimeLine()
    {
        yield break;
    }
}