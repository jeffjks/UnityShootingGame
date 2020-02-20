using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Stage5Manager : StageManager
{
    [Space(10)]
    public GameObject m_Test;
    public GameObject m_Helicopter, m_TankLarge_2, m_Gunship,
    m_PlaneSmall_1, m_ItemHeli_1, m_ItemHeli_2, m_PlaneMedium_3, m_PlaneMedium_4, m_PlaneLarge_2, m_PlaneLarge_3;
    public Transform[] m_BossTerrains = new Transform[3];

    private float m_BackgroundPos;

    void Awake()
    {
        m_Stage = 4;
    }

    protected override IEnumerator MainTimeLine()
    {
        yield return new WaitForSeconds(1f);
        InitEnemies();
        yield break;
    }

    protected override IEnumerator TestTimeLine()
    {
        yield break;
    }

    protected override IEnumerator BossOnlyTimeLine()
    {
        yield break;
    }

    protected override IEnumerator EnemyTimeLine()
    {
        yield break;
    }
}