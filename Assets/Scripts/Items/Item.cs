using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public bool m_IsAir;
    [SerializeField] private Collider2D m_Collider2D = null; // 지상 아이템 콜라이더 보정 및 충돌 체크
    public AudioClip m_AudioClip;

    protected abstract void ItemEffect(Collider2D other);
    protected Transform m_MainCameraTransform;
    protected Vector2 m_Position2D;

    private Vector2 m_BackgroundCameraSize;
    
    protected SystemManager m_SystemManager = null;
    protected PlayerManager m_PlayerManager = null;
    

    protected virtual void Awake()
    {
        m_SystemManager = SystemManager.instance_sm;
        m_PlayerManager = PlayerManager.instance_pm;

        m_BackgroundCameraSize = m_SystemManager.m_BackgroundCameraSize;
        m_MainCameraTransform = m_SystemManager.m_MainCamera.transform;
    }
    
    protected virtual void FixedUpdate()
    {
        GetCoordinates();
    }

    protected void GetCoordinates() {
        if (m_IsAir)
            m_Position2D = transform.position;
        else {
            m_Position2D = GetScreenPosition(transform.position);
            m_Collider2D.transform.position = m_Position2D;
        }
    }

    private Vector2 GetScreenPosition(Vector3 pos) {
        float main_camera_xpos = m_SystemManager.m_MainCamera.transform.position.x;
        Vector3 screen_pos = m_SystemManager.m_BackgroundCamera.WorldToScreenPoint(pos);
        Vector2 modified_pos = new Vector2(
            screen_pos[0]*m_BackgroundCameraSize.x/Screen.width - m_BackgroundCameraSize.x/2 + main_camera_xpos,
            screen_pos[1]*m_BackgroundCameraSize.y/Screen.height - m_BackgroundCameraSize.y);
        return modified_pos;
    }
}



public abstract class ItemBox : Item
{
    [SerializeField] private float m_DisappearTime = 0f;
    protected MoveVector m_MoveVector;
    
    private float m_MinX, m_MaxX, m_MinY, m_MaxY;
    private float m_Padding = 0.6f;
    private bool m_Disappear = false;
    private float m_Speed = 4f;

    protected override void Awake()
    {
        base.Awake();
        m_MoveVector.speed = m_Speed;
        float random_value = Random.Range(-45f, 45f);
        m_MoveVector.direction = random_value;

        if (!m_Disappear) {
            m_MinX = - (7.555f - m_Padding);
            m_MaxX = 7.555f - m_Padding;
            m_MinY = - (16f - m_Padding);
            m_MaxY = - m_Padding;
            transform.position = new Vector3 
                (
                    Mathf.Clamp(transform.position.x, m_MinX, m_MaxX), 
                    Mathf.Clamp(transform.position.y, m_MinY, m_MaxY),
                    Depth.ITEMS
                );
            Invoke("Disappear", m_DisappearTime);
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        MoveDirection(m_MoveVector);

        if (!m_Disappear && m_IsAir) {
            float angle;
            if (transform.position.x <= m_MainCameraTransform.position.x - (Size.CAMERA_WIDTH*0.5f - m_Padding)) { // left
                angle = Random.Range(-45f, 45f);
                m_MoveVector.direction = 90f + angle;
            }
            else if (transform.position.x >= m_MainCameraTransform.position.x + (Size.CAMERA_WIDTH*0.5f - m_Padding)) { // right
                angle = Random.Range(-45f, 45f);
                m_MoveVector.direction = -90f + angle;
            }
            else if (transform.position.y <= m_MainCameraTransform.position.y - (Size.CAMERA_HEIGHT*0.5f - m_Padding)) { // bottom
                angle = Random.Range(-45f, 45f);
                m_MoveVector.direction = 180f + angle;
            }
            else if (transform.position.y >= m_MainCameraTransform.position.y + (Size.CAMERA_HEIGHT*0.5f - m_Padding)) { // top
                angle = Random.Range(-45f, 45f);
                m_MoveVector.direction = 0f + angle;
            }
        }
        RotateBox();
    }

    private void MoveDirection(MoveVector moveVector) {
        if (m_IsAir) {
            Vector2 vector2 = Quaternion.AngleAxis(moveVector.direction, Vector3.forward) * Vector2.down;
            transform.Translate(vector2 * moveVector.speed * Time.fixedDeltaTime, Space.World);
        }
    }

    private void Disappear() {
        m_Disappear = true;
    }

    private void RotateBox() {
        transform.Rotate(Vector3.up * Time.fixedDeltaTime * 100f, Space.Self);
    }

    void OnTriggerEnter2D(Collider2D other) // 충돌 감지
    {
        if (other.gameObject.CompareTag("PlayerBody")) { // 대상이 플레이어 바디면 자신 파괴
            ItemEffect(other);
            Destroy(gameObject);
        }
    }
}


public abstract class ItemGem : Item
{
    public string m_ObjectName;

    private PoolingManager m_PoolingManager = null;


    protected override void Awake()
    {
        base.Awake();
        m_PoolingManager = PoolingManager.instance_op;
    }

    void OnEnable()
    {
        GetCoordinates();
    }

    

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (m_SystemManager.m_PlayState == 4) {
            OnDeath();
        }
    }

    void OnTriggerEnter2D(Collider2D other) // 충돌 감지
    {
        if (other.gameObject.CompareTag("PlayerBody")) { // 대상이 플레이어 바디면 자신 파괴
            ItemEffect(other);
            OnDeath();
        }
    }

    public void OnDeath() {
        m_PoolingManager.PushToPool(m_ObjectName, gameObject, PoolingParent.ITEM_GEM);
    }
}