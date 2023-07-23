using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShipLarge : EnemyUnit
{
    public EnemyShipLargeTurret0 m_Turret0;
    public EnemyShipLargeTurret1 m_Turret1;
    private int[] m_FireDelay = { 900, 360, 240 };
    private int m_Phase = 0;
    
    void Start()
    {
        RotateImmediately(m_MoveVector.direction);

        m_EnemyHealth.Action_OnHealthChanged += DestroyChildEnemy;
    }
    
    protected override void Update()
    {
        base.Update();
        
        RotateImmediately(m_MoveVector.direction);
    }

    private void DestroyChildEnemy() {
        if (m_Phase > 0) {
            return;
        }
        if (m_EnemyHealth.HealthPercent <= 0.33f) {
            m_Phase = 1;
            StartCoroutine(Pattern1());
            m_Turret0?.m_EnemyDeath.OnDying();
            m_Turret1?.m_EnemyDeath.OnDying();
        }
    }
    
    private IEnumerator Pattern1() {
        Vector3[] pos = new Vector3[2];
        BulletAccel accel1 = new BulletAccel(0.1f, 600);
        BulletAccel accel2 = new BulletAccel(0f, 0);

        while(true) {
            for (int i = 0; i < 2; i++) {
                pos[i] = BackgroundCamera.GetScreenPosition(m_FirePosition[i].position);
                CreateBullet(0, pos[i], 3.6f, Random.Range(0f, 360f), accel1, BulletSpawnType.EraseAndCreate, 600,
                4, 8f, BulletPivot.Player, Random.Range(-18f, 18f), accel2);
            }
            yield return new WaitForMillisecondFrames(m_FireDelay[(int) SystemManager.Difficulty]);
        }
    }
}
