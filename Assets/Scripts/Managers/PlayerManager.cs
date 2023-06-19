using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject m_PlayerPrefab;
    public GameObject m_ItemPowerUp;
    public ReplayManager m_ReplayManager;
    
    public GameObject Player { get; private set; }
    private PlayerShooter _playerShooter;
    
    public const float REVIVE_POSITION_Y = -13f;

    private readonly Vector3 m_SpawnPoint = new (0, -20, Depth.PLAYER);
    
    public static ShipAttributes CurrentAttributes = new (0, 0, 0, 0, 0, 0, 0);
    public static bool IsPlayerAlive;
    
    private const int REVIVE_DELAY = 1200;

    public static Action Action_OnStartStartNewGame;

    public static PlayerManager Instance { get; set; }
    
    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        DontDestroyOnLoad(gameObject);

        SystemManager.Action_OnQuitInGame += DestroyPlayerManager;
        Action_OnStartStartNewGame?.Invoke();
        
        m_ReplayManager.Init();
        BackgroundCamera.Camera.transform.rotation = Quaternion.AngleAxis(90f - Size.BACKGROUND_CAMERA_ANGLE, Vector3.right);
    }

    public void SpawnPlayer() {
        SpawnPlayer(CurrentAttributes);
    }

    public void SpawnPlayer(ShipAttributes attributes)
    {
        CurrentAttributes = attributes;
        IsPlayerAlive = true;
        if (SystemManager.Instance.SpawnAtSpawnPointCondition()) {
            Player = Instantiate(m_PlayerPrefab, m_SpawnPoint, Quaternion.identity);
        }
        else {
            Player = Instantiate(m_PlayerPrefab, new Vector3(0f, REVIVE_POSITION_Y, Depth.PLAYER), Quaternion.identity);
        }
        _playerShooter = Player.GetComponentInChildren<PlayerShooter>();

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
            _playerShooter.PowerSet(power);
        }
    }

    public void PlayerDead(Vector2Int dead_position) {
        IsPlayerAlive = false;
        StartCoroutine(RevivePlayer());
        Vector3 item_pos = new Vector3(dead_position.x / 256, dead_position.y / 256, Depth.ITEMS);
        int item_num;

        if (InGameDataManager.Instance.TotalMiss == 0) {
            item_num = Mathf.Min(_playerShooter.m_ShotLevel, 2);
        }
        else if (InGameDataManager.Instance.TotalMiss == 1) {
            item_num = Mathf.Min(_playerShooter.m_ShotLevel, 1);
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
        _playerShooter.m_ShotLevel -= item_num;
    }
    
    private IEnumerator RevivePlayer() {
        yield return new WaitForMillisecondFrames(REVIVE_DELAY);
        IsPlayerAlive = true;
        Player.SetActive(true);
        PlayerInvincibility.SetInvincibility(PlayerInvincibility.REVIVE_TIME);
    }

    public static Vector3 GetPlayerPosition() {
        if (Instance.Player == null || !IsPlayerAlive) {
            return new Vector3(0f, REVIVE_POSITION_Y, Depth.PLAYER);
        }
        return Instance.Player.transform.position;
    }

    private void DestroyPlayerManager()
    {
        Destroy(gameObject);
    }
}
