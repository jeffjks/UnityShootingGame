using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject m_PlayerPrefab;
    public Item m_ItemPowerUp;
    public ReplayManager m_ReplayManager;
    
    public GameObject Player { get; private set; }
    private PlayerUnit _playerUnit;
    
    public const float REVIVE_POSITION_Y = -13f;

    private readonly Vector3 m_SpawnPoint = new (0, -20, Depth.PLAYER);
    
    public static ShipAttributes CurrentAttributes = new (0, 0, 0, 0, 0, 0, 0);
    public static bool IsPlayerAlive;
    
    private const int REVIVE_DELAY = 1500;

    public static Action Action_OnStartStartNewGame;

    public static PlayerManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        DontDestroyOnLoad(gameObject);
        
        m_ReplayManager.Init();

        SystemManager.Action_OnQuitInGame += DestroySelf;
        Action_OnStartStartNewGame?.Invoke();
        
        BackgroundCamera.Camera.transform.rotation = Quaternion.AngleAxis(90f - Size.BACKGROUND_CAMERA_ANGLE, Vector3.right);
    }
    
    private void OnDestroy()
    {
        SystemManager.Action_OnQuitInGame -= DestroySelf;
    }

    public GameObject SpawnPlayer(int playerAttackLevel) {
        return SpawnPlayer(CurrentAttributes, playerAttackLevel);
    }

    public GameObject SpawnPlayer(ShipAttributes attributes, int playerAttackLevel)
    {
        CurrentAttributes = attributes;
        IsPlayerAlive = true;
        if (SystemManager.Instance.SpawnAtSpawnPointCondition()) {
            Player = Instantiate(m_PlayerPrefab, m_SpawnPoint, Quaternion.identity);
        }
        else {
            Player = Instantiate(m_PlayerPrefab, new Vector3(0f, REVIVE_POSITION_Y, Depth.PLAYER), Quaternion.identity);
        }
        _playerUnit = Player.GetComponentInChildren<PlayerUnit>();

        _playerUnit.PlayerAttackLevel = playerAttackLevel;

        return Player;
    }

    public void PlayerDead(Vector2Int dead_position) {
        IsPlayerAlive = false;
        StartCoroutine(RevivePlayer());
        Vector3 item_pos = new Vector3(dead_position.x / 256, dead_position.y / 256, Depth.ITEMS);
        int item_num;

        if (InGameDataManager.Instance.TotalMiss == 0) {
            item_num = Mathf.Min(_playerUnit.PlayerAttackLevel, 2);
        }
        else if (InGameDataManager.Instance.TotalMiss == 1) {
            item_num = Mathf.Min(_playerUnit.PlayerAttackLevel, 1);
        }
        else {
            item_num = 0;
        }
        InGameDataManager.Instance.AddMiss();

        for (var i = 0; i < item_num; i++) { // item_num 만큼 파워업 아이템 드랍
            var item = Instantiate(m_ItemPowerUp, item_pos, Quaternion.identity);
        }
        _playerUnit.PlayerAttackLevel -= item_num;
    }
    
    private IEnumerator RevivePlayer() {
        yield return new WaitForMillisecondFrames(REVIVE_DELAY);
        IsPlayerAlive = true;
        Player.SetActive(true);
        PlayerInvincibility.SetInvincibility(PlayerInvincibility.REVIVE_TIME);
    }

    public static Vector3 GetPlayerPosition() {
        if (IsPlayerAlive)
            return Instance._playerUnit.transform.position;
        return new Vector3(0f, REVIVE_POSITION_Y, Depth.PLAYER);
    }

    private void DestroySelf()
    {
        Instance = null;
        IsPlayerAlive = false;
        Destroy(gameObject);
    }
}
