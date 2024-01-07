using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Random = UnityEngine.Random;

public struct ReplayInput<T>
{
    public int timeStamp;
    public T input;
    
    public ReplayInput(int timeStamp, T input)
    {
        this.timeStamp = timeStamp;
        this.input = input;
    }
}

public class ReplayManager : MonoBehaviour
{
    public bool m_Activate;
    public ReplayVersionDatas m_ReplayVersionData;
    
    private PlayerController _playerController;
    private PlayerManager _playerManager;
    
    private ReplayInfo _replayInfo;
    private ReplayData _context;
    private bool _eof;

    private const string ReplayWriteFile = "replayLog_Play.log";
    private const string ReplayReadFile = "replayLog_Replay.log";

#if UNITY_EDITOR
    private StreamWriter _logFileStream;

    public bool m_KillLog;
    public bool m_RemoveLog;
    public bool m_PlayerMovementInputLog;
    public bool m_PlayerAttackInputLog;
    public bool m_PlayerWeaponStartLog;
    public bool m_PlayerWeaponHitLog;
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

    public static bool IsReplayAvailable => !PauseManager.IsGamePaused && SystemManager.IsInGame && LoadSucceed;

    public static ReplayManager Instance { get; private set; }
    public static int CurrentFrame;
    private static bool LoadSucceed;

    public enum KeyType
    {
        Fire = 0,
        Bomb = 2
    }

    // int = 0bAAAA_BBBB_CCCC_DDDD_EEEE_FFFF_GGGG_HHHH // GGGG_HHHH: movementInputFlag, EEEE_FFFF: movementInput, CCCC_DDDD: fire/bomb
    [Serializable]
    public struct ReplayData
    {
        public byte movementInputFlag;
        public byte inputMovement;
        public byte inputPress;
        public byte unused;
        public int frame;

        public ReplayData(long context)
        {
            frame = (int)(context >> 32);
            var inputData = (int) (context & 0xFFFFFFFF);

            movementInputFlag = (byte) ((inputData >> 0) & 0xFF);
            inputMovement = (byte) ((inputData >> 8) & 0xFF);
            inputPress = (byte) ((inputData >> 16) & 0xFF);
            unused = (byte) ((inputData >> 24) & 0xFF);
        }

        public void SetMoveVectorData(Vector2Int inputVector)
        {
            var moveLeft = (byte) (inputVector.x == -1 ? 1 : 0);
            var moveRight = (byte) (inputVector.x == 1 ? 1 : 0);
            var moveDown = (byte) (inputVector.y == -1 ? 1 : 0);
            var moveUp = (byte) (inputVector.y == 1 ? 1 : 0);

            movementInputFlag = 1;
            inputMovement = (byte) ((moveLeft << 0) | (moveRight << 1) | (moveDown << 2)| (moveUp << 3));
        }

        public long GetData()
        {
            long code = 0;
            code |= (long) movementInputFlag << 0;
            code |= (long) inputMovement << 8;
            code |= (long) inputPress << 16;
            code |= (long) unused << 24;
            code |= (long) frame << 32;
            return code;
        }

        public bool TryGetMoveVectorData(out Vector2Int moveVectorInt)
        {
            if (movementInputFlag == 0)
            {
                moveVectorInt = default;
                return false;
            }
            var moveLeft = (inputMovement >> 0) & 1;
            var moveRight = (inputMovement >> 1) & 1;
            var moveDown = (inputMovement >> 2) & 1;
            var moveUp = (inputMovement >> 3) & 1;
            
            moveVectorInt = new Vector2Int(moveRight - moveLeft, moveUp - moveDown);
            return true;
        }

        public bool TryGetFirePressed(out bool isFirePressed)
        {
            var inputFire = (inputPress >> (byte) KeyType.Fire) & 0b11;

            if ((inputFire & 0b01) != 0b01)
            {
                isFirePressed = default;
                return false;
            }
            
            isFirePressed = (inputFire & 0b10) == 0b10;
            return true;
        }

        public bool TryGetBombPressed(out bool isBombPressed)
        {
            var inputBomb = (inputPress >> (byte) KeyType.Bomb) & 0b11;

            if ((inputBomb & 0b01) != 0b01)
            {
                isBombPressed = default;
                return false;
            }
            
            isBombPressed = (inputBomb & 0b10) == 0b10;
            return true;
        }
    }

    [Serializable]
    public struct ReplayInfo
    {
        public readonly int m_Seed;
        public readonly long m_DateTime;
        public readonly string m_Version;
        public readonly ShipAttributes m_Attributes;
        public readonly int m_PlayerAttackLevel;
        public readonly GameMode m_GameMode;
        public readonly int m_Stage;
        public readonly GameDifficulty m_Difficulty;

        public ReplayInfo(int seed, long dateTime, string version, ShipAttributes attributes, int playerAttackLevel, GameMode gameMode, int stage, GameDifficulty difficulty)
        {
            m_Seed = seed;
            m_DateTime = dateTime;
            m_Version = version;
            m_Attributes = attributes;
            m_PlayerAttackLevel = playerAttackLevel;
            m_GameMode = gameMode;
            m_Stage = stage;
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
        _context = new ReplayData();

        SystemManager.Action_OnQuitInGame += OnClose;
        SystemManager.Action_OnNextStage += OnNextStage;
        SystemManager.Action_OnNextStage += InitCurrentFrame;

#if UNITY_EDITOR
        if (!SystemManager.IsInGame)
            return;
        
        if (SystemManager.GameMode == GameMode.Replay)
        {
            if (!File.Exists($"{GameManager.ReplayLogFilePath}{ReplayReadFile}"))
            {
                Directory.CreateDirectory(GameManager.ReplayLogFilePath);
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
                Directory.CreateDirectory(GameManager.ReplayLogFilePath);
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
        SystemManager.Action_OnNextStage -= InitCurrentFrame;
    }

    private void InitCurrentFrame(bool hasNextStage)
    {
        if (!hasNextStage)
            return;
        CurrentFrame = 0;
    }

    private void LateUpdate()
    {
        if (!IsReplayAvailable)
        {
            if (!LoadSucceed && !PauseManager.IsGamePaused)
                OpenPopupMenu();
            return;
        }
        CurrentFrame++;
    }

    private void OpenPopupMenu()
    {
        if (SystemManager.GameMode == GameMode.Replay)
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
        
        if (SystemManager.CurrentSeed == -1)
        {
            SystemManager.CurrentSeed = Environment.TickCount;
            Random.InitState(SystemManager.CurrentSeed);
        }
        else
        {
            Random.InitState(SystemManager.CurrentSeed);
        }
        Debug.Log($"Current Seed: {SystemManager.CurrentSeed}");

        if (SystemManager.GameMode == GameMode.Replay)
        {
            LoadSucceed = ReplayFileController.InitReadingReplayFile(OnCompleteInitReading, CurrentReplaySlot);
        }
        else
        {
            LoadSucceed = ReplayFileController.InitWritingReplayFile(OnCompleteInitWriting);
        }
    }

    #region ReadReplayFile

    private void OnCompleteInitReading()
    {
        _replayInfo = ReplayFileController.ReadBinaryReplayInfo();
        InitPlayer(PlayerManager.CurrentAttributes, _replayInfo.m_PlayerAttackLevel);
    }
    
    public void ReadUserInput()
    {
        if (_eof)
            return;
        
        if (_context.GetData() == 0)
        {
            try
            {
                _context = ReplayFileController.ReadBinaryReplayData();
            }
            catch (EndOfStreamException)
            {
                Debug.Log($"Replay file reached end of file.");
                _eof = true;
                return;
            }
        }
        
        if (CurrentFrame != _context.frame)
            return;

        if (_context.TryGetMoveVectorData(out var moveVectorInt))
        {
            _playerController.OnMoveInvoked(moveVectorInt);
            if (m_PlayerMovementInputLog)
                WriteReplayLogFile($"{moveVectorInt} Move {PlayerManager.GetPlayerPosition().ToString("N6")}");
        }

        if (_context.TryGetFirePressed(out var isFirePressed))
        {
            _playerController.OnFireInvoked(isFirePressed);
            if (m_PlayerAttackInputLog)
                WriteReplayLogFile($"{isFirePressed} Fire {PlayerManager.GetPlayerPosition().ToString("N6")}");
        }

        if (_context.TryGetBombPressed(out var isBombPressed))
        {
            _playerController.OnBombInvoked(isBombPressed);
            if (m_PlayerAttackInputLog)
                WriteReplayLogFile($"{isBombPressed} Bomb {PlayerManager.GetPlayerPosition().ToString("N6")}");
        }

        _context = new ReplayData();
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
            SystemManager.Stage,
            SystemManager.Difficulty
        );
        ReplayFileController.WriteBinaryReplayInfo(_replayInfo);
        
        InitPlayer(playerAttackLevel);
    }
    
    public void WriteUserMovementInput(Vector2Int inputVector)
    {
        if (SystemManager.GameMode == GameMode.Replay)
            return;
        
        _context.SetMoveVectorData(inputVector);
        //WriteReplayLogFile($"WritePressInput {inputVector} Move {PlayerManager.GetPlayerPosition().ToString("N6")}");
    }

    public void WriteUserPressInput(bool isPressed, KeyType keyType)
    {
        if (SystemManager.GameMode == GameMode.Replay)
            return;

        var offset = (int) keyType;
        
        var inputFire = isPressed ? 0b11 : 0b01;

        _context.inputPress |= (byte) (0b11 << offset);
        _context.inputPress &= (byte) (inputFire << offset);
        //WriteReplayLogFile($"WritePressInput {isPressed} {keyType.ToString()} {PlayerManager.GetPlayerPosition().ToString("N6")}");
    }

    public void WriteReplayData()
    {
        var data = _context.GetData();
        
        if (data == 0L)
            return;
        _context.frame = CurrentFrame;
        
        ReplayFileController.WriteBinaryReplayData(_context);

        if (_context.TryGetMoveVectorData(out var moveVectorInt))
        {
            if (m_PlayerMovementInputLog)
                WriteReplayLogFile($"{moveVectorInt} Move {PlayerManager.GetPlayerPosition().ToString("N6")}");
        }

        if (_context.TryGetFirePressed(out var isFirePressed))
        {
            if (m_PlayerAttackInputLog)
                WriteReplayLogFile($"{isFirePressed} Fire {PlayerManager.GetPlayerPosition().ToString("N6")}");
        }

        if (_context.TryGetBombPressed(out var isBombPressed))
        {
            if (m_PlayerAttackInputLog)
                WriteReplayLogFile($"{isBombPressed} Bomb {PlayerManager.GetPlayerPosition().ToString("N6")}");
        }
        
        _context = new ReplayData();
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
            OnClose();
    }

    private void OnClose()
    {
        ReplayFileController.OnClose();
        
#if UNITY_EDITOR
        if (_logFileStream == null)
            return;
        if (!IsReplayAvailable)
            return;
        _logFileStream.Flush();
        _logFileStream.Close();
        _logFileStream = null;
#endif
    }

    private void OnApplicationQuit()
    {
        OnClose();
    }

    public static void WriteReplayLogFile(string str)
    {
#if UNITY_EDITOR
        if (!IsReplayAvailable)
            return;
        Instance._logFileStream?.WriteLine($"{CurrentFrame}: {str}");
#endif
    }
}
