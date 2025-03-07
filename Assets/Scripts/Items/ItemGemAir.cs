﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGemAir : ItemGem
{
    [SerializeField] private float m_RandomScale;

    private float m_VerticalSpeed, m_HorizontalSpeed;
    private Vector3 m_RandomAxis;
    private float m_Angle, m_Scale;

    public void Init()
    {
        m_Renderer.rotation = Random.rotation;
        m_RandomAxis = Utility.GetRandomPositionInsideCircle().normalized;
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
        
        if (Time.timeScale == 0)
            return;

        RotateSelf();

        var moveX = m_HorizontalSpeed;
        var moveY = m_VerticalSpeed;
        m_MoveVector = new MoveVector(new Vector2(moveX, moveY));
        MoveDirection(m_MoveVector.speed, m_MoveVector.direction);

        if (m_VerticalSpeed > -7f)
            m_VerticalSpeed -= 0.1f;
        else
            m_VerticalSpeed = -7f;
    }

    protected override void ItemEffect(PlayerUnit playerUnit) {
        InGameDataManager.Instance.AddScore(m_ItemData.itemScore, true, m_ItemData.itemType);
        AudioService.PlaySound("ItemGem");
    }

    protected override void OnItemRemoved() {
        ReturnToPool();
    }

    public override void ReturnToPool() {
        PoolingManager.PushToPool(m_ObjectName, gameObject, PoolingParent.GemAir);
    }
}
