using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public static class Size
{
    public const float CAMERA_WIDTH = 12; // 정확히는 카메라 콜라이더
    public const float CAMERA_HEIGHT = 16;
    public const float GAME_WIDTH = 15.11f;
    public const float GAME_HEIGHT = 16;

    public const float CAMERA_MOVE_LIMIT = 4.895f; // 카메라가 움직이는 플레이어의 최대/최소 x값

    public const float GAME_BOUNDARY_LEFT = - GAME_WIDTH / 2;
    public const float GAME_BOUNDARY_RIGHT = GAME_WIDTH / 2;
    public const float GAME_BOUNDARY_BOTTOM = -8f - GAME_HEIGHT / 2;
    public const float GAME_BOUNDARY_TOP = -8f + GAME_HEIGHT / 2;

    public const float BACKGROUND_CAMERA_ANGLE = 25f;
}


public class PlayerManager : MonoBehaviour
{
    public GameObject m_Player;
    public MainCamera m_MainCamera;
    public float m_RevivePointY = -13f;
    public float m_SafeLine = -12f;
    public GameObject m_ItemPowerUp;

    [SerializeField] private BoxCollider2D m_CameraBoundary = null;
    [SerializeField] private BoxCollider2D m_CameraOuterBoundary = null;
    [SerializeField] private BoxCollider2D m_GameOuterBoundary = null;

    [SerializeField] private RectTransform m_CanvasUI = null;

    [HideInInspector] public bool m_PlayerIsAlive;
    [HideInInspector] public int[] m_CurrentAttributes = {0, 0, 0, 0, 0, 0, 0}; // Color, Speed, ShotForm, ShotDamage, LaserDamage, Module, Bomb
    [HideInInspector] public float m_CameraMargin;

    private GameManager m_GameManager = null;
    private SystemManager m_SystemManager = null;

    private PlayerShooter m_PlayerShooter;
    private Vector3 m_SpawnPoint;
    private bool m_PlayerControlable = false;
    private float m_ReviveDelay = 2f;

    public static PlayerManager instance_pm = null;

    void Awake()
    {
        if (instance_pm != null) {
            Destroy(this.gameObject);
            return; 
        }
        instance_pm = this;

        m_GameManager = GameManager.instance_gm;
        m_SystemManager = SystemManager.instance_sm;

        m_CameraBoundary.size = new Vector2(Size.CAMERA_WIDTH, Size.CAMERA_HEIGHT);
        m_CameraOuterBoundary.size = new Vector2(Size.CAMERA_WIDTH, Size.CAMERA_HEIGHT);
        m_GameOuterBoundary.size = new Vector2(Size.GAME_WIDTH, Size.GAME_HEIGHT);
        m_CanvasUI.sizeDelta = new Vector2(Size.CAMERA_WIDTH, Size.CAMERA_HEIGHT);

        m_SpawnPoint = new Vector3(0, -20f, Depth.PLAYER);
        m_CameraMargin = (Size.GAME_WIDTH - Size.CAMERA_WIDTH) / 2; // 1.555
        
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        SpawnPlayer();
        m_SystemManager.SetPlayerManager();
        m_SystemManager.m_BackgroundCamera.transform.rotation = Quaternion.AngleAxis(90f - Size.BACKGROUND_CAMERA_ANGLE, Vector3.right);
    }

    private void SpawnPlayer()
    {
        try {
            for (int i=0; i<m_GameManager.m_CurrentAttributes.Length; i++) {
                m_CurrentAttributes[i] = m_GameManager.m_CurrentAttributes[i];
            }
        }
        catch {
            for (int i=0; i<6; i++) {
                m_CurrentAttributes[i] = 0;
            }
        }

        m_PlayerIsAlive = true;
        if (m_SystemManager.GetStage() == 0)
            m_Player = Instantiate(m_Player, m_SpawnPoint, Quaternion.identity);
        else
            m_Player = Instantiate(m_Player, new Vector3(0f, m_RevivePointY, Depth.PLAYER), Quaternion.identity);
        m_PlayerShooter = m_Player.GetComponent<PlayerShooter>();
        m_PlayerShooter.m_ShotLevel = m_SystemManager.m_ShotLevel;
    }

    public void PlayerDead(Vector3 dead_position) {
        m_PlayerIsAlive = false;
        Invoke("PlayerRevive", m_ReviveDelay);
        Vector3 item_pos = new Vector3(dead_position.x, dead_position.y, Depth.ITEMS);

        int item_num;

        if (m_SystemManager.GetTotalMiss() == 0) {
            item_num = Mathf.Min(m_PlayerShooter.m_ShotLevel, 2);
        }
        else if (m_SystemManager.GetTotalMiss() == 1) {
            item_num = Mathf.Min(m_PlayerShooter.m_ShotLevel, 1);
        }
        else {
            item_num = 0;
        }

        for (int i = 0; i < item_num; i++) { // item_num 만큼 파워업 아이템 드랍
            Instantiate(m_ItemPowerUp, item_pos, Quaternion.identity);
        }
        m_PlayerShooter.m_ShotLevel -= item_num;
    }
 
    private void PlayerRevive() {
        m_SystemManager.AddMiss();
        m_PlayerIsAlive = true;
        m_Player.SetActive(true);
    }

    public bool PlayerControlable {
        get { return m_PlayerControlable; }
        set { m_PlayerControlable = value; }
    }
}
