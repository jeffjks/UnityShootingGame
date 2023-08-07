using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneLarge3Turret : EnemyUnit
{
    private int[] m_FireDelay = { 600, 600, 500 };
    private IEnumerator m_CurrentPattern;

    void Start()
    {
        CurrentAngle = AngleToPlayer;
        //m_CurrentPattern = Pattern1();
        //StartCoroutine(m_CurrentPattern);
    }

    public void StopPattern1() {
        if (m_CurrentPattern != null)
            StopCoroutine(m_CurrentPattern);
    }

    protected override void Update()
    {
        base.Update();
        
        if (PlayerManager.IsPlayerAlive)
            RotateUnit(AngleToPlayer);
        else
            RotateUnit(AngleToPlayer, 180f);
    }
    
    private IEnumerator Pattern1() {
        BulletAccel accel1 = new BulletAccel(0.1f, 800);
        BulletAccel accel2 = new BulletAccel(0f, 0);
        Vector3 pos;
        yield return new WaitForMillisecondFrames(2300);

        while(true) {
            for (int i = 0; i < 3; i++) {
                if (SystemManager.Difficulty == GameDifficulty.Normal) {
                    pos = m_FirePosition[0].position;
                    CreateBullet(3, pos, 8.3f, CurrentAngle, accel1, BulletSpawnType.EraseAndCreate, 600,
                    5, 4.3f, BulletPivot.Player, 0f, accel2);
                }
                else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                    pos = m_FirePosition[0].position;
                    CreateBulletsSector(3, pos, 8.3f, CurrentAngle, accel1, 2, 100f, BulletSpawnType.EraseAndCreate, 600,
                    5, 4.3f, BulletPivot.Player, 0f, accel2, 3, 16f);
                }
                else {
                    pos = m_FirePosition[0].position;
                    CreateBulletsSector(3, pos, 8.3f, CurrentAngle, accel1, 2, 100f, BulletSpawnType.EraseAndCreate, 600,
                    5, 4.3f, BulletPivot.Player, 0f, accel2, 3, 16f);
                }
                yield return new WaitForMillisecondFrames(280);
            }
            yield return new WaitForMillisecondFrames(m_FireDelay[(int) SystemManager.Difficulty]);
        }
    }
}
