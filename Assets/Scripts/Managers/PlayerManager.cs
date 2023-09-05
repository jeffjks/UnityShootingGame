using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject m_PlayerPrefab;
    public Item m_ItemPowerUp;
    public ReplayManager m_ReplayManager;
    
    private bool _destroySingleton;
    private PlayerUnit _playerUnit;

    public const float REVIVE_POSITION_Y = -13f;
    private static Vector3 RevivePosition = new (0f, REVIVE_POSITION_Y, Depth.PLAYER);

    private readonly Vector3 m_SpawnPoint = new (0, -20f, Depth.PLAYER);
    
    public static ShipAttributes CurrentAttributes = new (0, 0, 0, 0, 0, 0, 0);
    public static bool IsPlayerAlive;
    
    private const int REVIVE_DELAY = 1500;

    public static Action Action_OnStartStartNewGame;

    public static PlayerManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null)
        {
            _destroySingleton = true;
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        DontDestroyOnLoad(gameObject);
        
        m_ReplayManager.Init();

        SystemManager.Action_OnQuitInGame += DestroySelf;
        Action_OnStartStartNewGame?.Invoke();
        
        BackgroundCamera.Instance.m_BackgroundOffsetTransform.rotation = Quaternion.AngleAxis(90f - Size.BACKGROUND_CAMERA_ANGLE, Vector3.right);
    }
    
    private void OnDestroy()
    {
        if (_destroySingleton)
            return;
        SystemManager.Action_OnQuitInGame -= DestroySelf;
    }

    public GameObject SpawnPlayer(int playerAttackLevel) {
        return SpawnPlayer(CurrentAttributes, playerAttackLevel);
    }

    public GameObject SpawnPlayer(ShipAttributes attributes, int playerAttackLevel)
    {
        CurrentAttributes = attributes;
        IsPlayerAlive = true;
        GameObject spawnedPlayer;
        
        if (SystemManager.Instance.SpawnAtSpawnPointCondition()) {
            spawnedPlayer = Instantiate(m_PlayerPrefab, m_SpawnPoint, Quaternion.identity);
        }
        else {
            spawnedPlayer = Instantiate(m_PlayerPrefab, new Vector3(0f, REVIVE_POSITION_Y, Depth.PLAYER), Quaternion.identity);
        }
        _playerUnit = spawnedPlayer.GetComponentInChildren<PlayerUnit>();

        _playerUnit.PlayerAttackLevel = playerAttackLevel;

        return spawnedPlayer;
    }

    public Vector3 PlayerDead(Vector3 deadPosition)
    {
        var playerReviveX = Mathf.Clamp(deadPosition.x, -Size.CAMERA_MOVE_LIMIT, Size.CAMERA_MOVE_LIMIT);
        RevivePosition = new Vector3(playerReviveX, REVIVE_POSITION_Y, Depth.PLAYER);
        
        IsPlayerAlive = false;
        StartCoroutine(RevivePlayer());
        var itemPos = new Vector3(deadPosition.x, deadPosition.y, Depth.ITEMS);
        int itemNumber;

        if (InGameDataManager.Instance.TotalMiss == 0) {
            itemNumber = Mathf.Min(_playerUnit.PlayerAttackLevel, 2);
        }
        else if (InGameDataManager.Instance.TotalMiss == 1) {
            itemNumber = Mathf.Min(_playerUnit.PlayerAttackLevel, 1);
        }
        else {
            itemNumber = 0;
        }
        InGameDataManager.Instance.AddMiss();

        for (var i = 0; i < itemNumber; i++) { // itemNumber 만큼 파워업 아이템 드랍
            var item = Instantiate(m_ItemPowerUp, itemPos, Quaternion.identity);
        }
        _playerUnit.PlayerAttackLevel -= itemNumber;
        return RevivePosition;
    }
    
    private IEnumerator RevivePlayer()
    {
        yield return new WaitForMillisecondFrames(REVIVE_DELAY);
        IsPlayerAlive = true;
        _playerUnit.gameObject.SetActive(true);
        PlayerInvincibility.SetInvincibility(PlayerInvincibility.REVIVE_TIME);
    }

    public static Vector3 GetPlayerPosition()
    {
        if (IsPlayerAlive)
            return Instance._playerUnit.transform.position;
        return RevivePosition;
    }

    private void DestroySelf()
    {
        Instance = null;
        IsPlayerAlive = false;
        Destroy(gameObject);
    }
}
