using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class Item : UnitObject
{
    public ItemInfoDatas m_ItemData;
    public TriggerBody m_TriggerBody; // 지상 아이템 콜라이더 보정 및 충돌 체크
    public Transform m_Renderer;
    private const float BoundaryPadding = 0.5f;
    [SerializeField] protected float m_Radius;

    protected abstract void ItemEffect(PlayerUnit playerUnit);
    protected abstract void OnItemRemoved();

    public void OnEnable()
    {
        // SimulationManager.AddTriggerBody(m_TriggerBody);
    }

    public void OnDisable()
    {
        // SimulationManager.RemoveTriggerBody(m_TriggerBody);
    }
    
    protected virtual void Update()
    {
        if (Time.timeScale == 0)
            return;
        
        CheckOutside();
        SetTriggerBodyPosition();
    }

    private void CheckOutside() // 화면 바깥으로 나갈시 파괴
    {
        var pos = m_IsAir ? transform.position : (Vector3) Position2D;
        
        if (InnerGameBoundary.IsOutOfCamera(pos, BoundaryPadding))
            OnItemRemoved();
    }

    private void SetTriggerBodyPosition() {
        if (m_IsAir)
        {
            return;
        }
        Quaternion screenRotation = Quaternion.AngleAxis(Size.BACKGROUND_CAMERA_ANGLE, Vector3.right);
        
        SetColliderPositionOnScreen(Position2D, screenRotation);
    }
    
    public void SetColliderPositionOnScreen(Vector2 screenPosition, Quaternion screenRotation)
    {
        m_TriggerBody.transform.position = screenPosition;
        m_TriggerBody.transform.rotation = screenRotation;
    }

    public void GetItem()
    {
        ItemEffect(PlayerUnit.Instance);
        OnItemRemoved();
        
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (ReplayManager.ItemLog)
            ReplayManager.WriteReplayLogFile($"GetItem {gameObject.name}: {PlayerManager.GetPlayerPosition():N6}");
#endif
    }
}



public abstract class ItemBox : Item
{
    private float m_MinX, m_MaxX, m_MinY, m_MaxY;
    private bool m_Disappear;
    private const float SPEED = 4f;
    private const float PADDING = 0.6f;

    private void Awake()
    {
        m_MoveVector.speed = SPEED;
        float random_value = Random.Range(-45f, 45f);
        m_MoveVector.direction = random_value;

        if (!m_Disappear) {
            m_MinX = - (Size.GAME_WIDTH / 2f - PADDING);
            m_MaxX = Size.GAME_WIDTH / 2f - PADDING;
            m_MinY = - (Size.GAME_HEIGHT - PADDING);
            m_MaxY = - PADDING;
            Vector3 newPos = transform.position;
            newPos.x = Mathf.Clamp(newPos.x, m_MinX, m_MaxX);
            newPos.y = Mathf.Clamp(newPos.y, m_MinY, m_MaxY);
            newPos.z = Depth.ITEMS;
            transform.position = newPos;
            StartCoroutine(DisappearTimer());
        }
    }

    protected override void Update()
    {
        base.Update();
        
        if (Time.timeScale == 0)
            return;
        
        if (m_IsAir)
            MoveDirection(m_MoveVector.speed, m_MoveVector.direction);

        if (!m_Disappear && m_IsAir) {
            float angle;
            if (transform.position.x <= MainCamera.BOUNDARY_LEFT + PADDING) { // left
                angle = Random.Range(-45f, 45f);
                m_MoveVector.direction = 90f + angle;
            }
            else if (transform.position.x >= MainCamera.BOUNDARY_RIGHT - PADDING) { // right
                angle = Random.Range(-45f, 45f);
                m_MoveVector.direction = -90f + angle;
            }
            else if (transform.position.y <= MainCamera.BOUNDARY_BOTTOM + PADDING) { // bottom
                angle = Random.Range(-45f, 45f);
                m_MoveVector.direction = 180f + angle;
            }
            else if (transform.position.y >= MainCamera.BOUNDARY_TOP - PADDING) { // top
                angle = Random.Range(-45f, 45f);
                m_MoveVector.direction = 0f + angle;
            }
        }
    }

    private IEnumerator DisappearTimer() {
        yield return new WaitForMillisecondFrames(m_ItemData.activeTimer);
        m_Disappear = true;
    }

    private void RotateBox() {
        m_Renderer.Rotate(Vector3.up * (Time.deltaTime * 100f), Space.Self);
    }

    protected override void OnItemRemoved() {
        Destroy(gameObject);
    }
}


public abstract class ItemGem : Item, IObjectPooling
{
    public string m_ObjectName;
    public MeshRenderer m_MeshRenderer;

    private byte m_Brightness;
    private bool m_BrightnessState;

    public abstract void ReturnToPool();

    private void Awake()
    {
        SystemManager.Action_OnNextStage += RemoveItemOnNextStage;
    }

    private void OnDestroy()
    {
        SystemManager.Action_OnNextStage -= RemoveItemOnNextStage;
    }

    private void RemoveItemOnNextStage(bool hasNextStage)
    {
        ReturnToPool();
    }

    
    /*
    protected override void Update()
    {
        base.Update();

        SetBrightness();
    }

    private void SetBrightness() {
        Material material = m_MeshRenderer.material;
        material.SetColor("_EmissionColor", new Color32(m_Brightness, m_Brightness, m_Brightness, 255));
        material.EnableKeyword("_EMISSION");

        if (m_Brightness <= 16)
            m_BrightnessState = true;
        else if (m_Brightness >= 56)
            m_BrightnessState = false;
        
        if (m_BrightnessState)
            m_Brightness += 1;
        else
            m_Brightness -= 1;
    }*/
}