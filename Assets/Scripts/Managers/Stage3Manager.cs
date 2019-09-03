using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Stage3Manager : StageManager
{
    [Space(10)]
    public GameObject m_Test;
    public GameObject m_TankSmall_1, m_TankSmall_2, m_ShipSmall_1, m_ShipSmall_2, m_Helicopter, m_PlaneSmall_1, m_ItemHeli_1, m_ItemHeli_2, m_Gunship, m_PlaneMedium_2, m_PlaneMedium_3;

    private const float WATER_HEIGHT = 2.32f;
    private IEnumerator m_CurrentSpawn;

    void Awake()
    {
        m_Stage = 2;
    }

    protected override IEnumerator MainTimeLine()
    {
        yield return new WaitForSeconds(1f);
        //InitEnemies();
        SetBackgroundSpeed(0.016f);

        yield return new WaitForSeconds(55f);
        SetBackgroundSpeed(-0.016f, 2f);
        //StartCoroutine(MiddleBossStart(new Vector3(-12.5f, 4.2f, 29f), 1f)); // Middle Boss (56s)

        yield return new WaitForSeconds(26f);
        SetBackgroundSpeed(new Vector3(-0.032f, 0f, -0.012f), 0.75f);
        
        yield return new WaitForSeconds(13f);
        SetBackgroundSpeed(new Vector3(0f, 0f, 0.016f), 0.75f);

        yield return new WaitForSeconds(40f);
        //StartCoroutine(BossStart(new Vector3(16f, WATER_HEIGHT, 115f), 9f)); // Boss
        StartCoroutine(FadeOutMusic());
        yield return new WaitForSeconds(3f);
        m_SystemManager.StartCoroutine("WarningText");
        yield return new WaitForSeconds(4f);
        SetBackgroundSpeed(0f);
        PlayBossMusic();
        yield break;
    }

    protected override IEnumerator TestTimeLine()
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(BossStart(new Vector3(0f, 5.6f, 27f), 3f)); // Boss
        yield return null;
    }

    protected override IEnumerator EnemyTimeLine()
    {
        yield break;
        yield return new WaitForSeconds(3f);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(-3.5f, 3f), new Vector2(-3f, -3f), Random.Range(1.2f, 1.5f));
        CreateEnemyWithTarget(m_Helicopter, new Vector2(3.5f, 3f), new Vector2(3f, -3f), Random.Range(1.2f, 1.5f));
        yield return new WaitForSeconds(4f);
        CreateEnemy(m_ItemHeli_1, new Vector2(-2f, 3f)); // Item Heli 1
        yield return new WaitForSeconds(2f);
        CreateEnemy(m_ItemHeli_2, new Vector2(3.5f, 3f)); // Item Heli 2
        yield break;
    }

    private IEnumerator SpawnPlane1()
    {
        while (true) {
            for (int i = 0; i < 2; i++) {
                if (m_SystemManager.m_Difficulty == Difficulty.NORMAL) {
                    CreateEnemy(m_PlaneSmall_1, new Vector2(Random.Range(-2f, -1f), Random.Range(2f, 4f)));
                    CreateEnemy(m_PlaneSmall_1, new Vector2(Random.Range(1f, 2f), Random.Range(2f, 4f)));
                }
                else {
                    CreateEnemy(m_PlaneSmall_1, new Vector2(Random.Range(-3f, -2f), Random.Range(2f, 4f)));
                    CreateEnemy(m_PlaneSmall_1, new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(2f, 4f)));
                    CreateEnemy(m_PlaneSmall_1, new Vector2(Random.Range(2f, 3f), Random.Range(2f, 4f)));
                }
                yield return new WaitForSeconds(0.6f);
            }
            yield return new WaitForSeconds(5f);
        }
    }
}