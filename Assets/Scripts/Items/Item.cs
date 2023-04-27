using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public bool m_IsAir;
    public Collider2D m_Collider2D; // 지상 아이템 콜라이더 보정 및 충돌 체크
    public AudioClip m_AudioClip;
    public Transform m_Renderer;

    protected abstract void ItemEffect(Collider2D other);
    public abstract void OnItemRemoved();
    protected Transform m_MainCameraTransform;
    protected Vector2 m_Position2D;
    protected Vector2Int m_Position;

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
    
    protected virtual void Update()
    {
        GetCoordinates();
        CheckOutside();
    }

    protected void GetCoordinates() {
        if (m_IsAir)
            m_Position2D = transform.position;
        else {
            m_Position2D = GetScreenPosition(transform.position);
            m_Collider2D.transform.position = m_Position2D;
        }
    }

    private void CheckOutside() { // 화면 바깥으로 나갈시 파괴
        float gap = 0.5f;
        Vector3 pos;
        if (m_IsAir) {
            pos = transform.position;
        }
        else {
            pos = m_Position2D;
        }
        if (Mathf.Abs(pos.x) > Size.GAME_WIDTH*0.5f + gap) {
            OnItemRemoved();
        }
        else if (Mathf.Abs(pos.y + Size.GAME_HEIGHT*0.5f) > Size.GAME_HEIGHT*0.5f + gap) {
            OnItemRemoved();
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

    protected void SetPosition() {
        transform.position = new Vector3((float) m_Position.x / 256, (float) m_Position.y / 256, transform.position.z);
    }

    public void InitPosition(Vector2Int pos) {
        m_Position = pos;
        SetPosition();
    }
}



public abstract class ItemBox : Item
{
    public int m_DisappearTime;
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
            StartCoroutine(Disappear());
        }
    }

    protected override void Update()
    {
        base.Update();
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
            transform.Translate(vector2 * moveVector.speed / Application.targetFrameRate * Time.timeScale, Space.World);
        }
    }

    private IEnumerator Disappear() {
        yield return new WaitForMillisecondFrames(m_DisappearTime);
        m_Disappear = true;
        yield break;
    }

    private void RotateBox() {
        m_Renderer.Rotate(Vector3.up * Time.deltaTime * 100f, Space.Self);
    }

    void OnTriggerEnter2D(Collider2D other) // 충돌 감지
    {
        if (other.gameObject.CompareTag("PlayerBody")) { // 대상이 플레이어 바디면 자신 파괴
            ItemEffect(other);
            OnItemRemoved();
        }
    }

    public override void OnItemRemoved() {
        Destroy(gameObject);
    }
}


public abstract class ItemGem : Item, UseObjectPool
{
    public string m_ObjectName;
    public MeshRenderer m_MeshRenderer;

    protected PoolingManager m_PoolingManager = null;

    private byte m_Shiness;
    private bool m_ShinessState;

    public abstract void ReturnToPool();

    protected override void Awake()
    {
        base.Awake();
        m_PoolingManager = PoolingManager.instance_op;
    }

    void OnEnable()
    {
        GetCoordinates();
    }

    protected override void Update()
    {
        base.Update();
        if (m_SystemManager.m_PlayState == 4) {
            ReturnToPool();
        }

        SetShiness();
    }

    private void SetShiness() {
        Material material = m_MeshRenderer.material;
        material.SetColor("_EmissionColor", new Color32(m_Shiness, m_Shiness, m_Shiness, 255));
        material.EnableKeyword("_EMISSION");

        if (m_Shiness <= 16)
            m_ShinessState = true;
        else if (m_Shiness >= 56)
            m_ShinessState = false;
        
        if (m_ShinessState)
            m_Shiness += 1;
        else
            m_Shiness -= 1;
    }

    void OnTriggerEnter2D(Collider2D other) // 충돌 감지
    {
        if (other.gameObject.CompareTag("PlayerBody")) { // 대상이 플레이어 바디면 자신 파괴
            ItemEffect(other);
            ReturnToPool();
        }
    }
}