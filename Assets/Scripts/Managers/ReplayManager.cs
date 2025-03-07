﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization;
using Random = UnityEngine.Random;

public class ReplayManager : MonoBehaviour
{
    public bool m_Activate;
    public ReplayVersionDatas m_ReplayVersionData;
    
    private static PlayerController _playerController;
    private PlayerManager _playerManager;
    
    private ReplayInfo _replayInfo;
    private static readonly ReplayMovementData _movementContext = new();
    private static readonly Queue<IReplayInput> _replayDataBuffer = new();
    private const int QueueMinCount = 16;
    private const int QueueMaxCount = 32;
    private static bool _eof;

    private const string ReplayWriteFile = "replayLog_Play.log";
    private const string ReplayReadFile = "replayLog_Replay.log";

#if UNITY_EDITOR || DEVELOPMENT_BUILD
    private StreamWriter _logFileStream;

    public bool m_KillLog;
    public bool m_RemoveLog;
    public bool m_PlayerMovementInputLog;
    public bool m_PlayerActionInputLog;
    public bool m_PlayerWeaponStartLog;
    public bool m_PlayerWeaponHitLog;
    public bool m_PlayerLaserStartLog;
    public bool m_ItemLog;
    public bool m_HitCountLog;
    public bool m_DebugLog;

    public static bool KillLog => Instance != null && Instance.m_KillLog;
    public static bool RemoveLog => Instance != null && Instance.m_RemoveLog;
    public static bool PlayerMovementInputLog => Instance != null && Instance.m_PlayerMovementInputLog;
    public static bool PlayerActionInputLog => Instance != null && Instance.m_PlayerActionInputLog;
    public static bool PlayerWeaponStartLog => Instance != null && Instance.m_PlayerWeaponStartLog;
    public static bool PlayerWeaponHitLog => Instance != null && Instance.m_PlayerWeaponHitLog;
    public static bool PlayerLaserStartLog => Instance != null && Instance.m_PlayerLaserStartLog;
    public static bool ItemLog => Instance != null && Instance.m_ItemLog;
    public static bool HitCountLog => Instance != null && Instance.m_HitCountLog;
    public static bool DebugLog => Instance != null && Instance.m_DebugLog;
#endif

    private int PlayerAttackLevel
    {
        get
        {
            if (SystemManager.GameMode != GameMode.Training)
                return 0;
            
            switch (SystemManager.Stage) {
                case 0:
                    return SystemManager.TrainingInfo.bossOnly ? 2 : 0;
                case 1:
                    return SystemManager.TrainingInfo.bossOnly ? 4 : 2;
                default:
                    return 4;
            }
        }
    }
    
    public static int CurrentReplaySlot { private get; set; }

    public static bool IsReplayAvailable => !PauseManager.IsGamePaused && SystemManager.IsInGame && !ErrorOccured;

    private static ReplayManager Instance { get; set; }
    
    public static int CurrentFrame;
    private static bool ErrorOccured;

    public enum KeyType : byte
    {
        Fire = 1,
        Bomb = 2
    }

    public enum ReplayDataType : short
    {
        PlayerMovement = 1,
        PlayerActionInput = 2,
        CollisionEnter = 3,
        CollisionExit = 4,
        EnemyHealth = 5
    }

    public interface IReplayInput
    {
        public bool IsActive { get; set; } // 동시에 여러 Input 방지
        public int Frame { get; set; }

        public void RunData();
    }

    [Serializable]
    public class ReplayMovementData : IReplayInput
    {
        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        public int Frame
        {
            get { return frame; }
            set { frame = value; }
        }

        [NonSerialized] private bool isActive;
        private int frame;

        private byte inputMovement;

        public void RunData()
        {
            var moveVectorInt = GetMoveVectorData();
            _playerController.OnMoveInvoked(moveVectorInt);
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (PlayerMovementInputLog)
                WriteReplayLogFile($"{moveVectorInt} Move {PlayerManager.GetPlayerPosition():N6}");
#endif
        }

        public Vector2Int GetMoveVectorData()
        {
            var moveLeft = (inputMovement >> 0) & 1;
            var moveRight = (inputMovement >> 1) & 1;
            var moveDown = (inputMovement >> 2) & 1;
            var moveUp = (inputMovement >> 3) & 1;
            return new Vector2Int(moveRight - moveLeft, moveUp - moveDown);
        }

        public void SetByteData(Vector2Int inputVector)
        {
            var moveLeft = (byte) (inputVector.x == -1 ? 1 : 0);
            var moveRight = (byte) (inputVector.x == 1 ? 1 : 0);
            var moveDown = (byte) (inputVector.y == -1 ? 1 : 0);
            var moveUp = (byte) (inputVector.y == 1 ? 1 : 0);
            
            inputMovement = (byte) ((moveLeft << 0) | (moveRight << 1) | (moveDown << 2)| (moveUp << 3));
            IsActive = true;
        }
    }
    
    [Serializable]
    public class ReplayActionData : IReplayInput
    {
        public bool IsActive { get; set; }
        public int Frame { get; set; }

        private readonly KeyType keyType;
        private readonly bool isPressed;
        
        public ReplayActionData(int frame, KeyType keyType, bool isPressed)
        {
            Frame = frame;
            this.keyType = keyType;
            this.isPressed = isPressed;
        }

        public void RunData()
        {
            switch (keyType)
            {
                case KeyType.Fire:
                    _playerController.OnFireInvoked(isPressed);
                    break;
                case KeyType.Bomb:
                    _playerController.OnBombInvoked(isPressed);
                    break;
                default:
                    Debug.LogError($"Unknown key type: {keyType}");
                    break;
            }
        }
    }

    [Serializable]
    public class ReplayInfo
    {
        public readonly int m_Seed;
        public readonly long m_DateTime;
        public readonly string m_Version;
        public readonly ShipAttributes m_Attributes;
        public readonly int m_PlayerAttackLevel;
        public readonly GameMode m_GameMode;
        public readonly TrainingInfo m_TrainingInfo;
        public readonly GameDifficulty m_Difficulty;

        public ReplayInfo(int seed, long dateTime, string version, ShipAttributes attributes, int playerAttackLevel, GameMode gameMode, TrainingInfo trainingInfo, GameDifficulty difficulty)
        {
            m_Seed = seed;
            m_DateTime = dateTime;
            m_Version = version;
            m_Attributes = attributes;
            m_PlayerAttackLevel = playerAttackLevel;
            m_GameMode = gameMode;
            m_TrainingInfo = trainingInfo;
            m_Difficulty = difficulty;
        }

        public bool IsDefault()
        {
            return m_Version == null;
        }
    }
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        CurrentFrame = 0;
        _movementContext.IsActive = false;
        _replayDataBuffer.Clear();

        SystemManager.Action_OnQuitInGame += OnClose;
        SystemManager.Action_OnNextStage += OnNextStage;
        SystemManager.Action_OnStageClear += OnStageClear;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (!SystemManager.IsInGame)
            return;
        
        var directoryInfo = new DirectoryInfo(GameManager.ReplayLogFilePath);
        if (!directoryInfo.Exists)
            directoryInfo.Create();
        
        if (SystemManager.IsReplayMode)
        {
            if (!File.Exists($"{GameManager.ReplayLogFilePath}{ReplayReadFile}"))
            {
                var file = File.CreateText($"{GameManager.ReplayLogFilePath}{ReplayReadFile}");
                file.Flush();
                file.Close();
            }
            _logFileStream = new StreamWriter($"{GameManager.ReplayLogFilePath}{ReplayReadFile}");
        }
        else
        {
            if (!File.Exists($"{GameManager.ReplayLogFilePath}{ReplayWriteFile}"))
            {
                var file = File.CreateText($"{GameManager.ReplayLogFilePath}{ReplayWriteFile}");
                file.Flush();
                file.Close();
            }
            _logFileStream = new StreamWriter($"{GameManager.ReplayLogFilePath}{ReplayWriteFile}");
        }
#endif
    }

    private void OnDestroy()
    {
        SystemManager.Action_OnQuitInGame -= OnClose;
        SystemManager.Action_OnNextStage -= OnNextStage;
    }

    private void LateUpdate()
    {
        if (!IsReplayAvailable)
        {
            if (ErrorOccured && !PauseManager.IsGamePaused)
                OpenPopupMenu();
            return;
        }
        if (CurrentFrame != -1)
            CurrentFrame++;
    }

    private static void OpenPopupMenu()
    {
        if (SystemManager.IsReplayMode)
            PauseManager.Instance.OpenPopupMenu("리플레이 파일을 읽는 중 오류가 발생하였습니다.",
                "Error has occured while reading replay file.");
        else
            PauseManager.Instance.OpenPopupMenu("리플레이 파일을 쓰는 중 오류가 발생하였습니다.",
                "Error has occured while writing replay file.");
    }

    public void Init()
    {
        _playerManager = PlayerManager.Instance;

        if (!m_Activate)
        {
            InitPlayer(PlayerAttackLevel);
            return;
        }
        
        Random.InitState(SystemManager.CurrentSeed);
        Debug.Log($"Current Seed: {SystemManager.CurrentSeed}");

        if (SystemManager.IsReplayMode)
        {
            ErrorOccured = !ReplayFileController.InitReadingReplayFile(OnCompleteInitReading, CurrentReplaySlot);
        }
        else
        {
            ErrorOccured = !ReplayFileController.InitWritingReplayFile(OnCompleteInitWriting);
        }
    }

    #region ReadReplayFile

    private void OnCompleteInitReading()
    {
        _replayInfo = ReplayFileController.ReadBinaryReplayInfo();
        InitPlayer(PlayerManager.CurrentAttributes, _replayInfo.m_PlayerAttackLevel);
    }
    
    public static void ReadUserInput()
    {
        if (_replayDataBuffer.Count < QueueMinCount)
        {
            ReadReplayDataToBuffer();
        }
        
        while (_replayDataBuffer.Count > 0 && _replayDataBuffer.Peek().Frame == CurrentFrame)
        {
            var replayData = _replayDataBuffer.Dequeue();
            replayData.RunData();
        }
    }

    private static void ReadReplayDataToBuffer()
    {
        if (_eof)
            return;
        
        while (_replayDataBuffer.Count < QueueMaxCount)
        {
            try
            {
                var dataType = ReplayFileController.ReadBinaryReplayDataType();
                switch (dataType)
                {
                    case ReplayDataType.PlayerMovement:
                        _replayDataBuffer.Enqueue(ReplayFileController.ReadBinaryReplayData<ReplayMovementData>());
                        break;
                    case ReplayDataType.PlayerActionInput:
                        _replayDataBuffer.Enqueue(ReplayFileController.ReadBinaryReplayData<ReplayActionData>());
                        break;
                }
            }
            catch (EndOfStreamException)
            {
                Debug.Log($"Replay file reached end of file.");
                _eof = true;
                break;
            }
            catch (SerializationException)
            {
                Debug.Log($"Replay file reached end of file.");
                _eof = true;
                break;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error has occured while reading replay file: {e}");
                OpenPopupMenu();
                ErrorOccured = true;
                break;
            }
        }
    }
    
    #endregion

    #region WriteReplayFile
    private void OnCompleteInitWriting()
    {
        var playerAttackLevel = PlayerAttackLevel;
        
        _replayInfo = new ReplayInfo(
            SystemManager.CurrentSeed,
            DateTime.Now.Ticks,
            m_ReplayVersionData.replayVersion,
            PlayerManager.CurrentAttributes,
            playerAttackLevel,
            SystemManager.GameMode,
            SystemManager.TrainingInfo,
            SystemManager.Difficulty
        );
        ReplayFileController.WriteBinaryReplayInfo(_replayInfo);
        
        InitPlayer(playerAttackLevel);
    }
    
    public static void WriteUserMovementInput(Vector2Int inputVector)
    {
        if (SystemManager.IsReplayMode)
            return;
        
        _movementContext.SetByteData(inputVector);
    }

    public static void WriteUserActionInput(KeyType keyType, bool isPressed)
    {
        if (SystemManager.IsReplayMode)
            return;

        var actionContext = new ReplayActionData(CurrentFrame, keyType, isPressed);
        
        ReplayFileController.WriteBinaryReplayData(ReplayDataType.PlayerActionInput, actionContext);
        
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (PlayerActionInputLog)
            WriteReplayLogFile($"{isPressed} {keyType} {PlayerManager.GetPlayerPosition():N6}");
#endif
    }

    public static void WriteReplayMovementData()
    {
        if (_movementContext.IsActive == false)
            return;
        _movementContext.Frame = CurrentFrame;
        
        ReplayFileController.WriteBinaryReplayData(ReplayDataType.PlayerMovement, _movementContext);

        var moveVectorInt = _movementContext.GetMoveVectorData();
        
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (PlayerMovementInputLog)
            WriteReplayLogFile($"{moveVectorInt} Move {PlayerManager.GetPlayerPosition():N6}");
#endif
        
        _playerController.OnMoveInvoked(moveVectorInt);
        _movementContext.IsActive = false;
    }
    #endregion
    
    private void InitPlayer(int playerAttackLevel)
    {
        Random.InitState(_replayInfo.m_Seed);
        var player = _playerManager.SpawnPlayer(playerAttackLevel);
        _playerController = player.GetComponentInChildren<PlayerController>();
    }

    private void InitPlayer(ShipAttributes shipAttributes, int playerAttackLevel)
    {
        Random.InitState(_replayInfo.m_Seed);
        var player = _playerManager.SpawnPlayer(shipAttributes, playerAttackLevel);
        _playerController = player.GetComponentInChildren<PlayerController>();
    }

    private void OnNextStage(bool hasNextStage)
    {
        if (!hasNextStage)
        {
            OnClose();
            return;
        }
        
        CurrentFrame = 0;
    }

    private void OnStageClear()
    {
        CurrentFrame = -1;
    }

    private void OnClose()
    {
        ReplayFileController.OnClose();
        _replayDataBuffer.Clear();
        
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (_logFileStream == null)
            return;
        
        _logFileStream.Flush();
        _logFileStream.BaseStream.Flush();
        _logFileStream.Close();
        _logFileStream = null;
        Debug.Log("LogFileStream has closed.");
#endif
    }

    private void OnApplicationQuit()
    {
        OnClose();
    }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
    public static void WriteReplayLogFile(string str)
    {
        if (!IsReplayAvailable)
            return;
        Instance._logFileStream?.WriteLine($"{CurrentFrame}: {str}");
    }
#endif
}
