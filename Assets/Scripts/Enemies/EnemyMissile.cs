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

        DisableInteractableAll();
    }

    void OnEnable()
    {
        transform.SetParent(null);
        m_MoveVector = new MoveVector(1f, 0f);
        m_Renderer.rotation = m_Rotation;
        StartCoroutine(AppearanceSequence());

        EnableInteractableAll();
    }

    private IEnumerator AppearanceSequence() {
        yield return new WaitForMillisecondFrames(1000);
        m_Engine.SetActive(true);

        yield return new WaitForMillisecondFrames(1200);

        float init_speed = transform.position.z;
        int frame = 1000 * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) { // 1초간 1->6
            float t_spd = AC_Ease.ac_ease[(int)EaseType.Linear].Evaluate((float) (i+1) / frame);
            
            float speed = Mathf.Lerp(init_speed, 6f, t_spd);
            m_MoveVector.speed = speed;
            yield return new WaitForMillisecondFrames(0);
        }
        yield break;
    }

    protected override IEnumerator DyingEffect() { // 파괴 과정
        Vector3 pos = transform.position;
        
        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            CreateBulletsSector(0, pos, 0.5f, -15f, new BulletAccel(4.5f, 500), 12, 30f);
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert) {
            CreateBulletsSector(3, pos, 0.25f, -12f, new BulletAccel(4f, 500), 15, 24f);
            CreateBulletsSector(0, pos, 0.75f, 0f, new BulletAccel(5f, 500), 15, 24f);
        }
        else {
            CreateBulletsSector(3, pos, 0.25f, -10f, new BulletAccel(4f, 500), 18, 20f);
            CreateBulletsSector(0, pos, 0.75f, 0f, new BulletAccel(5f, 500), 18, 20f);
        }
        
        yield break;
    }
}
