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

/*
TODO. use this code when save replay file
var key = "Key";
EncryptData(ReplayFilePath, _memoryStream?.ToArray(), key);
*/

/*
public class ReplayData
{
    public Queue<ReplayInput<Vector2Int>> movementInput = new();
    public Queue<ReplayInput<bool>> fireInput = new();
    public Queue<ReplayInput<bool>> bombInput = new();

    public void RecordMovementInput(ReplayInput<Vector2Int> input)
    {
        movementInput.Enqueue(input);
    }

    public void RecordPressInput(ReplayInput<bool> input, ReplayManager.KeyType keyType)
    {
        switch (keyType)
        {
            case ReplayManager.KeyType.Fire:
                fireInput.Enqueue(input);
                break;
            case ReplayManager.KeyType.Bomb:
                bombInput.Enqueue(input);
                break;
        }
    }
}
*/

public class ReplayManager : MonoBehaviour
{
    public bool m_Activate;
    
    //private FileStream _fileStream;
    private PlayerController _playerController;
    
    private PlayerManager _playerManager;
    
    private string _replayDirectory;
    private ReplayInfo _replayInfo;
    //private BinaryReader _br;
    //private BinaryWriter _bw;
    //private MemoryStream _memoryStream;
    private ReplayData _context;
    private bool _eof;

    private StreamWriter _logFileStream;

    private int PlayerAttackLevel
    {
        get
        {
            var power = 0;
            if (SystemManager.GameMode == GameMode.Training) {
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
            }

            return power;
        }
    }

    public static bool IsUsingReplay => !PauseManager.IsGamePaused && PlayerUnit.IsControllable;

    public static ReplayManager Instance { get; private set; }
    public static int CurrentFrame;
    //public static ReplayData CurrentReplayData;

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

        public void SetData(long context)
        {
            frame = (int)(context >> 32);
            var inputData = (int) (context & 0xFFFFFFFF);

            movementInputFlag = (byte) ((inputData >> 0) & 0xFF);
            inputMovement = (byte) ((inputData >> 8) & 0xFF);
            inputPress = (byte) ((inputData >> 16) & 0xFF);
            unused = (byte) ((inputData >> 24) & 0xFF);
        }

        public void SetMoveVectorData(Vector2Int rawInputVector)
        {
            var moveLeft = (byte) (rawInputVector.x == -1 ? 1 : 0);
            var moveRight = (byte) (rawInputVector.x == 1 ? 1 : 0);
            var moveDown = (byte) (rawInputVector.y == -1 ? 1 : 0);
            var moveUp = (byte) (rawInputVector.y == 1 ? 1 : 0);

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
    public class ReplayInfo
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
        //CurrentReplayData = new ReplayData();

        _replayDirectory = $"{Application.dataPath}/";

        SystemManager.Action_OnQuitInGame += OnClose;
        SystemManager.Action_OnNextStage += InitCurrentFrame;

#if UNITY_EDITOR
        if (SystemManager.GameMode == GameMode.Replay)
        {
            if (!File.Exists($"{_replayDirectory}replayLog_Replay.txt"))
            {
                var file = File.CreateText($"{_replayDirectory}replayLog_Replay.txt");
                file.Flush();
                file.Close();
            }
            _logFileStream = new StreamWriter($"{_replayDirectory}replayLog_Replay.txt");
        }
        else
        {
            if (!File.Exists($"{_replayDirectory}replayLog_Play.txt"))
            {
                var file = File.CreateText($"{_replayDirectory}replayLog_Play.txt");
                file.Flush();
                file.Close();
            }
            _logFileStream = new StreamWriter($"{_replayDirectory}replayLog_Play.txt");
        }
#endif
    }

    private void OnDestroy()
    {
        SystemManager.Action_OnQuitInGame -= OnClose;
        SystemManager.Action_OnNextStage -= InitCurrentFrame;
    }

    private void InitCurrentFrame(bool hasNextStage)
    {
        if (!hasNextStage)
            return;
        CurrentFrame = 0;
        //CurrentReplayData = new ReplayData();
    }

    private void LateUpdate()
    {
        if (!IsUsingReplay)
            return;
        CurrentFrame++;
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
            ReplayFileController.InitReadingReplayFile(OnCompleteInitReading);
        }
        else
        {
            ReplayFileController.InitWritingReplayFile(OnCompleteInitWriting);
        }
    }

    private void OnCompleteInitReading()
    {
        InitPlayer(PlayerManager.CurrentAttributes, _replayInfo.m_PlayerAttackLevel);
    }

    private void OnCompleteInitWriting()
    {
        var playerAttackLevel = PlayerAttackLevel;
        
        _replayInfo = new ReplayInfo(
            SystemManager.CurrentSeed,
            DateTime.Now.Ticks,
            Application.version,
            PlayerManager.CurrentAttributes,
            playerAttackLevel,
            SystemManager.GameMode,
            SystemManager.Stage,
            SystemManager.Difficulty
        );
        ReplayFileController.WriteReplayFile(_replayInfo);
        
        InitPlayer(playerAttackLevel);
    }

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

    #region ReadReplayFile
    /*private void StartReadReplayFile()
    {
        try
        {
            byte[] encryptedBytes = File.ReadAllBytes(ReplayFilePath);

            byte[] decryptedBytes = Utility.DecryptData(encryptedBytes);

            _memoryStream = new MemoryStream(decryptedBytes);
            _br = new BinaryReader(_memoryStream);
            
            
            //_fileStream = new FileStream(ReplayFilePath, FileMode.Open);
            //_br = new BinaryReader(_fileStream);
            _replayInfo = ReadBinaryHeader(_memoryStream);
            
            InitPlayer(PlayerManager.CurrentAttributes, _replayInfo.m_PlayerAttackLevel);
            Debug.Log($"Start reading replay file: {ReplayFilePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error has occured while reading replay file: {e}");
        }
    }*/
    
    public void ReadUserInput()
    {
        if (_eof)
            return;
        
        if (_context.GetData() == 0)
        {
            if (!ReplayFileController.TryReadReplayFile(out _context))
            {
                Debug.Log($"Replay file reached end of file.");
                _eof = true;
                return;
            }
            
            //_context.SetData(_br.ReadInt64());
        }
        
        if (CurrentFrame != _context.frame)
            return;

        if (_context.TryGetMoveVectorData(out var moveVectorInt))
        {
            _playerController.OnMoveInvoked(moveVectorInt);
            WriteReplayLogFile($"{moveVectorInt} Move {PlayerManager.GetPlayerPosition().ToString("N6")}");
        }

        if (_context.TryGetFirePressed(out var isFirePressed))
        {
            _playerController.OnFireInvoked(isFirePressed);
            WriteReplayLogFile($"{isFirePressed} Fire {PlayerManager.GetPlayerPosition().ToString("N6")}");
        }

        if (_context.TryGetBombPressed(out var isBombPressed))
        {
            _playerController.OnBombInvoked(isBombPressed);
            WriteReplayLogFile($"{isBombPressed} Bomb {PlayerManager.GetPlayerPosition().ToString("N6")}");
        }

        _context.SetData(0L);
    }

    private static T BinaryDeserialize<T>(MemoryStream memoryStream)
    {
        T context = default;
        try {
            BinaryFormatter formatter = new BinaryFormatter();
            context = (T) formatter.Deserialize(memoryStream);
        }
        catch {
            Debug.LogAssertion("File Error Has Occured");
        }
        return context;
    }
    #endregion

    #region WriteReplayFile
    /*private void StartWriteReplayFile()
    {
        // TODO. Need to change replay directory
        ReplayFilePath = $"{_replayDirectory}replay0.rep";
        var playerAttackLevel = PlayerAttackLevel;
        
        try {
            //File.Delete(ReplayFilePath);
            //_fileStream = new FileStream(ReplayFilePath, FileMode.Append, FileAccess.Write);
            
            _memoryStream = new MemoryStream();
            _bw = new BinaryWriter(_memoryStream);
            
            _replayInfo = new ReplayInfo(
                SystemManager.CurrentSeed,
                DateTime.Now.Ticks,
                Application.version,
                PlayerManager.CurrentAttributes,
                playerAttackLevel,
                SystemManager.GameMode,
                SystemManager.Stage,
                SystemManager.Difficulty
            );

            WriteBinaryHeader(_replayInfo);
            Debug.Log($"Start writing replay file: {ReplayFilePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error has occured while reading replay file: {e}");
        }
        finally {
            InitPlayer(playerAttackLevel);
        }
    }*/
    
    public void WriteUserMovementInput(Vector2Int rawInputVector)
    {
        if (SystemManager.GameMode == GameMode.Replay)
            return;
        
        _context.SetMoveVectorData(rawInputVector);
        //WriteReplayLogFile($"WritePressInput {rawInputVector} Move {PlayerManager.GetPlayerPosition().ToString("N6")}");
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
        
        ReplayFileController.WriteReplayFile(_context);
        
        //_memoryStream?.Write(BitConverter.GetBytes(_context.GetData()));

        if (_context.TryGetMoveVectorData(out var moveVectorInt))
        {
            WriteReplayLogFile($"{moveVectorInt} Move {PlayerManager.GetPlayerPosition().ToString("N6")}");
        }

        if (_context.TryGetFirePressed(out var isFirePressed))
        {
            WriteReplayLogFile($"{isFirePressed} Fire {PlayerManager.GetPlayerPosition().ToString("N6")}");
        }

        if (_context.TryGetBombPressed(out var isBombPressed))
        {
            WriteReplayLogFile($"{isBombPressed} Bomb {PlayerManager.GetPlayerPosition().ToString("N6")}");
        }
        
        _context.SetData(0L);
    }

    /*private void BinarySerialize<T>(T seed)
    {
        try {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(_memoryStream, seed);
        }
        catch {
            Debug.LogAssertion("File Error Has Occured");
        }
    }*/
    #endregion

    /*
    public static ReplayInfo ReadBinaryHeader(MemoryStream memoryStream)
    {
        var replayInfo = new ReplayInfo(
            BinaryDeserialize<int>(memoryStream),
            BinaryDeserialize<long>(memoryStream),
            BinaryDeserialize<string>(memoryStream),
            BinaryDeserialize<ShipAttributes>(memoryStream),
            BinaryDeserialize<int>(memoryStream),
            BinaryDeserialize<GameMode>(memoryStream),
            BinaryDeserialize<int>(memoryStream),
            BinaryDeserialize<GameDifficulty>(memoryStream)
            );
        return replayInfo;
    }

    private void WriteBinaryHeader(ReplayInfo replayInfo)
    {
        BinarySerialize(replayInfo.m_Seed); // 시드 쓰기
        BinarySerialize(replayInfo.m_DateTime); // 날짜 쓰기
        BinarySerialize(replayInfo.m_Version); // 버전 쓰기
        BinarySerialize(replayInfo.m_Attributes); // 기체 스펙 쓰기
        BinarySerialize(replayInfo.m_PlayerAttackLevel); // 공격 레벨 쓰기
        BinarySerialize((int) replayInfo.m_GameMode); // 게임 모드 쓰기
        BinarySerialize(replayInfo.m_Stage); // 시작 스테이지 쓰기
        BinarySerialize((int) replayInfo.m_Difficulty); // 난이도 쓰기
    }*/

    private void OnClose()
    {
        Debug.Log($"Replay file closed");
        //_memoryStream?.Close();
        ReplayFileController.OnClose();
        _logFileStream?.Close();
        //_br?.Close();
        //_bw?.Close();
        //_memoryStream = null;
        _logFileStream = null;
    }

    private void OnApplicationQuit()
    {
        OnClose();
    }

    public void WriteReplayLogFile(string str)
    {
#if UNITY_EDITOR
        if (!SystemManager.IsInGame)
            return;
        _logFileStream?.WriteLine($"{CurrentFrame}: {str}");
#endif
    }
}
