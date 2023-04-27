using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMissile : EnemyUnit
{
    public Transform m_Renderer;
    public GameObject m_Engine;

    private Quaternion m_Rotation;

    protected override void Awake()
    {
        base.Awake();
        m_Rotation = m_Renderer.rotation;
    }

    void OnEnable()
    {
        transform.parent = null;
        m_Collider2D[0].enabled = true;
        m_MoveVector = new MoveVector(1f, 0f);
        UpdateTransform();
        m_Renderer.rotation = m_Rotation;
        StartCoroutine(AppearanceSequence());
    }

    private IEnumerator AppearanceSequence() {
        yield return new WaitForMillisecondFrames(1000);
        m_Engine.SetActive(true);

        yield return new WaitForMillisecondFrames(1200);

        float init_speed = transform.position.z;
        int frame = 1000 * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) { // 1초간 1->6
            float t_spd = AC_Ease.ac_ease[EaseType.Linear].Evaluate((float) (i+1) / frame);
            
            float speed = Mathf.Lerp(init_speed, 6f, t_spd);
            m_MoveVector.speed = speed;
            yield return new WaitForMillisecondFrames(0);
        }
        yield break;
    }

    protected override IEnumerator DyingEffect() { // 파괴 과정
        Vector3 pos = transform.position;
        
        if (m_SystemManager.GetDifficulty() == 0) {
            CreateBulletsSector(0, pos, 0.5f, -15f, new EnemyBulletAccel(4.5f, 500), 12, 30f);
        }
        else if (m_SystemManager.GetDifficulty() == 1) {
            CreateBulletsSector(3, pos, 0.25f, -12f, new EnemyBulletAccel(4f, 500), 15, 24f);
            CreateBulletsSector(0, pos, 0.75f, 0f, new EnemyBulletAccel(5f, 500), 15, 24f);
        }
        else {
            CreateBulletsSector(3, pos, 0.25f, -10f, new EnemyBulletAccel(4f, 500), 18, 20f);
            CreateBulletsSector(0, pos, 0.75f, 0f, new EnemyBulletAccel(5f, 500), 18, 20f);
        }
        ExplosionEffect(0, -1, new Vector2(0f, -1f));
        ExplosionEffect(0, -1, new Vector2(0f, 1f));
        
        CreateItems();
        Destroy(gameObject);
        yield break;
    }
}
