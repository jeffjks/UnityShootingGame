using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGemAir : ItemGem
{
    [SerializeField] private float m_RandomScale = 0f;

    private float m_VerticalSpeed, m_HorizontalSpeed;
    private Vector3 m_RandomAxis;
    private float m_Angle, m_Scale;

    void OnEnable()
    {
        m_Renderer.rotation = Random.rotation;
        m_RandomAxis = Random.insideUnitCircle.normalized;
        m_VerticalSpeed = 0f;
        m_HorizontalSpeed = Random.Range(-0.8f, 0.8f);
        m_Scale = Random.Range(-m_RandomScale, m_RandomScale);
        //Debug.Log(m_Scale);
        transform.localScale = new Vector3(1f, 1f, 1f) * (0.3f + m_Scale);
    }

    private void RotateSelf() {
        m_Renderer.rotation = Quaternion.AngleAxis(m_Angle, m_RandomAxis);
        m_Angle += 300f * Time.deltaTime;
    }
    
	protected override void Update()
    {
        base.Update();

        RotateSelf();

        transform.Translate(m_HorizontalSpeed / Application.targetFrameRate * Time.timeScale, m_VerticalSpeed / Application.targetFrameRate * Time.timeScale, 0f, Space.World);

        if (m_VerticalSpeed > -7f)
            m_VerticalSpeed -= 0.1f;
        else
            m_VerticalSpeed = -7f;
    }

    protected override void ItemEffect(Collider2D other) {
        m_SystemManager.AddScoreEffect(ItemScore.GEM_AIR, false);
        m_SystemManager.m_SoundManager.PlayAudio(m_AudioClip);
    }

    public override void OnDeath() {
        m_PoolingManager.PushToPool(m_ObjectName, gameObject, PoolingParent.ITEM_GEM_AIR);
    }
}
