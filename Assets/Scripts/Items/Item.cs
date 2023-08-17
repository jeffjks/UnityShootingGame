using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class Item : UnitObject, IHasGroundCollider
{
    public ItemInfoDatas m_ItemData;
    public Collider2D m_Collider2D; // 지상 아이템 콜라이더 보정 및 충돌 체크
    public Transform m_Renderer;
    private const float BOUNDARY_PADDING = 0.5f;

    protected abstract void ItemEffect(Collider2D other);
    protected abstract void OnItemRemoved();
    
    protected virtual void Update()
    {
        CheckOutside();
        SetColliderPosition();
    }

    private void CheckOutside() { // 화면 바깥으로 나갈시 파괴
        Vector3 pos;
        if (m_IsAir) {
            pos = transform.position;
        }
        else {
            pos = m_Position2D;
        }
        if (Mathf.Abs(pos.x) > Size.GAME_WIDTH*0.5f + BOUNDARY_PADDING) {
            OnItemRemoved();
        }
        else if (Mathf.Abs(pos.y + Size.GAME_HEIGHT*0.5f) > Size.GAME_HEIGHT*0.5f + BOUNDARY_PADDING) {
            OnItemRemoved();
        }
    }

    private void SetColliderPosition() {
        if (m_IsAir)
        {
            return;
        }
        Quaternion screenRotation = Quaternion.AngleAxis(Size.BACKGROUND_CAMERA_ANGLE, Vector3.right);
        
        SetColliderPositionOnScreen(m_Position2D, screenRotation);
    }
    
    public void SetColliderPositionOnScreen(Vector2 screenPosition, Quaternion screenRotation) {
        m_Collider2D.transform.position = screenPosition;
        m_Collider2D.transform.rotation = screenRotation;
    }

    void OnTriggerEnter2D(Collider2D other) // 충돌 감지
    {
        if (other.gameObject.CompareTag("PlayerBody")) { // 대상이 플레이어 바디면 자신 파괴
            ItemEffect(other);
            OnItemRemoved();
        }
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
        MoveDirection(m_MoveVector);

        if (!m_Disappear && m_IsAir) {
            float angle;
            if (transform.position.x <= MainCamera.Instance.transform.position.x - (Size.MAIN_CAMERA_WIDTH / 2f - PADDING)) { // left
                angle = Random.Range(-45f, 45f);
                m_MoveVector.direction = 90f + angle;
            }
            else if (transform.position.x >= MainCamera.Instance.transform.position.x + (Size.MAIN_CAMERA_WIDTH / 2f - PADDING)) { // right
                angle = Random.Range(-45f, 45f);
                m_MoveVector.direction = -90f + angle;
            }
            else if (transform.position.y <= MainCamera.Instance.transform.position.y - (Size.MAIN_CAMERA_HEIGHT / 2f - PADDING)) { // bottom
                angle = Random.Range(-45f, 45f);
                m_MoveVector.direction = 180f + angle;
            }
            else if (transform.position.y >= MainCamera.Instance.transform.position.y + (Size.MAIN_CAMERA_HEIGHT / 2f - PADDING)) { // top
                angle = Random.Range(-45f, 45f);
                m_MoveVector.direction = 0f + angle;
            }
        }
    }

    private void MoveDirection(MoveVector moveVector) {
        if (m_IsAir) {
            Vector2 vector2 = Quaternion.AngleAxis(moveVector.direction, Vector3.forward) * Vector2.down;
            transform.Translate(vector2 * moveVector.speed / Application.targetFrameRate * Time.timeScale, Space.World);
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

    protected override void Update()
    {
        base.Update();
        if (SystemManager.PlayState == PlayState.OnStageTransition) {
            ReturnToPool();
        }

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
    }
}