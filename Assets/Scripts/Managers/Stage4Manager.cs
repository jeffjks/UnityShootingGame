using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Stage4Manager : StageManager
{
    [Space(10)]
    public GameObject m_Test;
    public GameObject m_TankSmall3, m_TankLarge_2,
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

        yield return new WaitForSeconds(74f);
        StartCoroutine(MiddleBossStart(new Vector3(0f, 2.5f, Depth.ENEMY), 1f)); // Middle Boss

        yield return new WaitForSeconds(22f);
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
        yield return new WaitForSeconds(47f);
        CreateEnemyWithMoveVector(m_TankLarge_2, new Vector3(2.61f, 3.18f, 56.17f), new MoveVector(0.7f, -32f), new MovePattern[] {new MovePattern(2f, 8739f, 0f, 0.7f)});
        yield return new WaitForSeconds(75f);
        CreateEnemyWithMoveVector(m_TankLarge_2, new Vector3(-24.35f, 3.21f, 84.5f), new MoveVector(-4f, -90f), new MovePattern[] {new MovePattern(1.4f, 8739f, 0f, 1.4f)});
        yield return new WaitForSeconds(35f);
        CreateEnemyWithMoveVector(m_TankLarge_2, new Vector3(12.7f, 3.03f, 106f), new MoveVector(2.5f, 14.785f),
            new MovePattern[] {new MovePattern(1f, 8739f, 0f, 1f), new MovePattern(2f, 8739f, -0.8f, 1f), new MovePattern(6f, 8739f, 0f, 0.5f)});
    }
}