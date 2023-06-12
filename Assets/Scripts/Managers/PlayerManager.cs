using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject m_PlayerPrefab;
    public GameObject m_ItemPowerUp;
    public ReplayManager m_ReplayManager;

    [HideInInspector] public bool m_PlayerIsAlive;

    private GameManager m_GameManager = null;
    private SystemManager m_SystemManager = null;
    private GameObject m_Player = null;

    [HideInInspector] public PlayerShooter m_PlayerShooter;
    [HideInInspector] public PlayerController m_PlayerController;
    public const float REVIVE_POSITION_Y = -13f;

    private readonly Vector3 m_SpawnPoint = new (0, -20, Depth.PLAYER);
    
    public static ShipAttributes CurrentAttributes = new (0, 0, 0, 0, 0, 0, 0);
    
    private const int REVIVE_DELAY = 1200;

    public static PlayerManager instance_pm = null;
    
    private void Awake()
    {
        if (instance_pm != null) {
            Destroy(gameObject);
            return;
        }
        instance_pm = this;
        
        DontDestroyOnLoad(gameObject);
        
        m_GameManager = GameManager.instance_gm;
        m_SystemManager = SystemManager.instance_sm;
        
        m_ReplayManager.Init();
        BackgroundCamera.Camera.transform.rotation = Quaternion.AngleAxis(90f - Size.BACKGROUND_CAMERA_ANGLE, Vector3.right);
    }

    public void SpawnPlayer() {
        SpawnPlayer(CurrentAttributes);
    }

    public void SpawnPlayer(ShipAttributes attributes)
    {
        CurrentAttributes = attributes;
        m_PlayerIsAlive = true;
        if (m_SystemManager.SpawnAtSpawnPointCondition()) {
            m_Player = Instantiate(m_PlayerPrefab, m_SpawnPoint, Quaternion.identity);
        }
        else {
            m_Player = Instantiate(m_PlayerPrefab, new Vector3(0f, REVIVE_POSITION_Y, Depth.PLAYER), Quaternion.identity);
        }
        m_PlayerShooter = m_Player.GetComponentInChildren<PlayerShooter>();
        m_PlayerController = m_Player.GetComponentInChildren<PlayerController>();

        if (SystemManager.GameMode == GameMode.Training) {
            int power;
            switch (SystemManager.Stage) {
                case 0:
                    if (SystemManager.TrainingInfo.bossOnly) {
                        power = 2;
                    }
                    else {
                        power = 0;
                    }
                    break;
                case 1:
                    if (SystemManager.TrainingInfo.bossOnly) {
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

        if (InGameDataManager.Instance.TotalMiss == 0) {
            item_num = Mathf.Min(m_PlayerShooter.m_ShotLevel, 2);
        }
        else if (InGameDataManager.Instance.TotalMiss == 1) {
            item_num = Mathf.Min(m_PlayerShooter.m_ShotLevel, 1);
        }
        else {
            item_num = 0;
        }
        InGameDataManager.Instance.AddMiss();

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
        if (!m_PlayerIsAlive || m_PlayerController == null) {
            return new Vector3(0f, REVIVE_POSITION_Y, Depth.PLAYER);
        }
        return m_PlayerController.transform.position;
    }

    public void DestroyPlayer() {
        Destroy(m_Player);
    }
}
