using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public static class Size
{
    public const float MAIN_CAMERA_POS = -64f;
    public const float CAMERA_WIDTH = 12f; // 정확히는 카메라 콜라이더
    public const float CAMERA_HEIGHT = 16f;
    public const float GAME_WIDTH = 15.11f;
    public const float GAME_HEIGHT = 16f;

    public const float CAMERA_MOVE_LIMIT = 4.895f; // 카메라가 움직이는 플레이어의 최대/최소 x값

    public const float GAME_BOUNDARY_LEFT = - GAME_WIDTH / 2;
    public const float GAME_BOUNDARY_RIGHT = GAME_WIDTH / 2;
    public const float GAME_BOUNDARY_BOTTOM = -GAME_HEIGHT;
    public const float GAME_BOUNDARY_TOP = -GAME_HEIGHT;

    public const float BACKGROUND_CAMERA_ANGLE = 25f;
}


public class PlayerManager : MonoBehaviour
{
    public GameObject m_PlayerPrefab;
    public GameObject m_ItemPowerUp;
    public ReplayManager m_ReplayManager;

    [SerializeField] private BoxCollider2D m_CameraBoundary = null;
    [SerializeField] private BoxCollider2D m_CameraOuterBoundary = null;
    [SerializeField] private BoxCollider2D m_GameOuterBoundary = null;

    [SerializeField] private RectTransform m_CanvasUI = null;

    [HideInInspector] public bool m_PlayerIsAlive;
    [HideInInspector] public float m_CameraMargin;

    private GameManager m_GameManager = null;
    private SystemManager m_SystemManager = null;
    private GameObject m_Player = null;

    [HideInInspector] public PlayerShooter m_PlayerShooter;
    [HideInInspector] public PlayerController m_PlayerController;
    [HideInInspector] public ShipAttributes m_CurrentAttributes = new ShipAttributes(0, 0, 0, 0, 0, 0, 0);
    [HideInInspector] public bool m_PlayerControlable = false;
    [HideInInspector] public int m_RevivePositionY = -13*256; // -3328;
    private Vector3 m_SpawnPoint;
    
    private const int REVIVE_DELAY = 1200;

    public static PlayerManager instance_pm = null;
/*
    PlayerManager() {
        instance_pm = this;
    }*/

    public void Init()
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

        m_SpawnPoint = new Vector3(0, -20, Depth.PLAYER);
        m_CameraMargin = (Size.GAME_WIDTH - Size.CAMERA_WIDTH) / 2; // 1.555

        m_CanvasUI.gameObject.SetActive(true);
    }

    void OnEnable() // Start였음
    {
        //if (m_GameManager != null) {
        //    m_CurrentAttributes = m_GameManager.m_CurrentAttributes;
        //}
        
        m_ReplayManager.Init();
        m_PlayerControlable = false;

        m_SystemManager.SetPlayerController(m_PlayerController);
        m_SystemManager.m_BackgroundCamera.transform.rotation = Quaternion.AngleAxis(90f - Size.BACKGROUND_CAMERA_ANGLE, Vector3.right);
    }

    public void SpawnPlayer() {
        SpawnPlayer(m_CurrentAttributes);
    }

    public void SpawnPlayer(ShipAttributes attributes)
    {
        m_CurrentAttributes = attributes;
        m_PlayerIsAlive = true;
        if (m_SystemManager.SpawnAtSpawnPointCondition()) {
            m_Player = Instantiate(m_PlayerPrefab, m_SpawnPoint, Quaternion.identity);
        }
        else {
            m_Player = Instantiate(m_PlayerPrefab, new Vector3(0f, m_RevivePositionY/256, Depth.PLAYER), Quaternion.identity);
        }
        m_PlayerShooter = m_Player.GetComponentInChildren<PlayerShooter>();
        m_PlayerController = m_Player.GetComponentInChildren<PlayerController>();

        if (SystemManager.GameMode == GameMode.GAMEMODE_TRAINING) {
            int power;
            switch (m_SystemManager.GetCurrentStage()) {
                case 0:
                    if (SystemManager.TrainingInfo.m_BossOnly) {
                        power = 2;
                    }
                    else {
                        power = 0;
                    }
                    break;
                case 1:
                    if (SystemManager.TrainingInfo.m_BossOnly) {
                        power = 4;
                    }
                    else {
                        power = 2;
                    }
                    break;
                default:
                    power = 4;
                    break;
            }
            m_PlayerShooter.PowerSet(power);
        }
    }

    public void PlayerDead(Vector2Int dead_position) {
        m_PlayerIsAlive = false;
        StartCoroutine(RevivePlayer());
        Vector3 item_pos = new Vector3(dead_position.x / 256, dead_position.y / 256, Depth.ITEMS);
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
        m_SystemManager.AddMiss();

        for (int i = 0; i < item_num; i++) { // item_num 만큼 파워업 아이템 드랍
            GameObject item = Instantiate(m_ItemPowerUp, item_pos, Quaternion.identity);
            Item m_Item = item.GetComponent<Item>();
            m_Item.InitPosition(dead_position);
        }
        m_PlayerShooter.m_ShotLevel -= item_num;
    }
    
    private IEnumerator RevivePlayer() {
        yield return new WaitForMillisecondFrames(REVIVE_DELAY);
        m_PlayerIsAlive = true;
        m_Player.SetActive(true);
        yield break;
    }

    public Vector3 GetPlayerPosition() {
        if (m_Player == null) {
            return new Vector3(0f, m_RevivePositionY, Depth.PLAYER);
        }
        return m_PlayerController.transform.position;
    }

    public void DestroyPlayer() {
        Destroy(m_Player);
    }
}
