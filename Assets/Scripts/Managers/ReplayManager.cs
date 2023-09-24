using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Random = UnityEngine.Random;

public class ReplayManager : MonoBehaviour
{
    public bool m_Activate;
    
    private FileStream _fileStream;
    private PlayerMovement _playerMovement;
    private PlayerController _playerController;
    
    private PlayerManager _playerManager;
    
    private string _replayDirectory;
    private ReplayInfo _replayInfo;
    private BinaryReader _br;
    private BinaryWriter _bw;
    private int _context;
    private bool _eof;

    public static string ReplayFilePath;

    public static bool IsUsingReplay => !PauseManager.IsGamePaused && SystemManager.PlayState < PlayState.OnStageResult;

    public static ReplayManager Instance { get; private set; }

    public enum KeyType
    {
        Fire = 8,
        Bomb = 10
    }

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
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        GameManager.CurrentFrame = 0;
        
        _replayDirectory = $"{Application.dataPath}/";

        SystemManager.Action_OnQuitInGame += OnClose;
    }

    private void OnDestroy()
    {
        SystemManager.Action_OnQuitInGame -= OnClose;
    }

    public void Init()
    {
        _playerManager = PlayerManager.Instance;

        if (!m_Activate)
        {
            InitPlayer(GetPlayerAttackLevel());
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
            StartReadReplayFile();
        }
        else
        {
            StartWriteReplayFile();
        }
    }

    private void StartReadReplayFile()
    {
        try
        {
            _fileStream = new FileStream(ReplayFilePath, FileMode.Open);
            _br = new BinaryReader(_fileStream);
            _replayInfo = ReadBinaryHeader(_fileStream);
            
            InitPlayer(PlayerManager.CurrentAttributes, _replayInfo.m_PlayerAttackLevel);
            Debug.Log($"Start reading replay file: {ReplayFilePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error has occured while reading replay file: {e}");
        }
    }

    private void StartWriteReplayFile()
    {
        ReplayFilePath = $"{_replayDirectory}replayTemp.rep";
        var playerAttackLevel = GetPlayerAttackLevel();
        
        try {
            File.Delete(ReplayFilePath);
            _fileStream = new FileStream(ReplayFilePath, FileMode.Append, FileAccess.Write);
            _bw = new BinaryWriter(_fileStream);
            
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
    }

    private void InitPlayer(int playerAttackLevel)
    {
        Random.InitState(_replayInfo.m_Seed);
        var player = _playerManager.SpawnPlayer(playerAttackLevel);
        _playerMovement = player.GetComponentInChildren<PlayerMovement>();
        _playerController = player.GetComponentInChildren<PlayerController>();
    }

    private void InitPlayer(ShipAttributes shipAttributes, int playerAttackLevel)
    {
        Random.InitState(_replayInfo.m_Seed);
        var player = _playerManager.SpawnPlayer(shipAttributes, playerAttackLevel);
        _playerMovement = player.GetComponentInChildren<PlayerMovement>();
        _playerController = player.GetComponentInChildren<PlayerController>();
    }

    private int GetPlayerAttackLevel()
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

    public void ReadUserInput()
    {
        if (_fileStream.Position >= _fileStream.Length)
        {
            if (!_eof)
                Debug.Log($"Replay file reached end of file.");
            _eof = true;
            return;
        }
        
        var context = _br.ReadInt32();

        var inputMove = (context >> 0) & 1;
        if (inputMove == 1)
        {
            var moveLeft = (context >> 4) & 1;
            var moveRight = (context >> 5) & 1;
            var moveDown = (context >> 6) & 1;
            var moveUp = (context >> 7) & 1;
            var moveVectorInt = new Vector2Int(moveRight - moveLeft, moveUp - moveDown);
            _playerController.OnMoveInvoked(moveVectorInt);
        }

        var inputFire = (context >> (int) KeyType.Fire) & 0b11;
        var inputBomb = (context >> (int) KeyType.Bomb) & 0b11;
        
        if ((inputFire & 0b01) == 0b01)
            _playerController.OnFireInvoked((inputFire & 0b10) == 0b10);
        if ((inputBomb & 0b01) == 0b01)
            _playerController.OnBombInvoked((inputBomb & 0b10) == 0b10);
    }

    public void WriteUserMovementInput(Vector2Int rawInputVector)
    {
        if (SystemManager.GameMode == GameMode.Replay)
            return;
        
        var moveLeft = rawInputVector.x == -1 ? 1 : 0;
        var moveRight = rawInputVector.x == 1 ? 1 : 0;
        var moveDown = rawInputVector.y == -1 ? 1 : 0;
        var moveUp = rawInputVector.y == 1 ? 1 : 0;

        _context |= 1;
        _context |= (moveLeft << 4) | (moveRight << 5) | (moveDown << 6)| (moveUp << 7);
        // _context |= (_playerController.IsFirePressed ? 1 : 0) << 4;
        // _context |= (_playerController.IsBombPressed ? 1 : 0) << 5;
    }

    public void WriteUserPressInput(bool isPressed, KeyType keyType)
    {
        if (SystemManager.GameMode == GameMode.Replay)
            return;

        var offset = (int) keyType;
        
        var inputFire = isPressed ? 0b11 : 0b01;
        
        _context |= inputFire << offset;
    }

    public void WriteReplayData()
    {
        //if (!PlayerUnit.IsControllable)
        //    return;
        
        //Debug.Log($"{GameManager.CurrentFrame}: {_context}, {PlayerUnit.Instance.transform.position}");
        
        _bw?.Write(_context);
        _context = 0;
    }

    private void BinarySerialize(int seed) {
        try {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(_fileStream, seed);
        }
        catch {
            Debug.LogAssertion("File Error Has Occured");
        }
    }

    private void BinarySerialize(long seed) {
        try {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(_fileStream, seed);
        }
        catch {
            Debug.LogAssertion("File Error Has Occured");
        }
    }

    private void BinarySerialize(string version) { // Overload
        try {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(_fileStream, version);
        }
        catch {
            Debug.LogAssertion("File Error Has Occured");
        }
    }

    private void BinarySerialize(ShipAttributes attributes) { // Overload
        try {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(_fileStream, attributes);
        }
        catch {
            Debug.LogAssertion("File Error Has Occured");
        }
    }

    public static ReplayInfo ReadBinaryHeader(FileStream fileStream)
    {
        var replayInfo = new ReplayInfo(
            BinaryDeserializeInt(fileStream),
            BinaryDeserializeLong(fileStream),
            BinaryDeserializeString(fileStream),
            BinaryDeserializeAttributes(fileStream),
            BinaryDeserializeInt(fileStream),
            (GameMode)BinaryDeserializeInt(fileStream),
            BinaryDeserializeInt(fileStream),
            (GameDifficulty)BinaryDeserializeInt(fileStream)
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
    }

    private static int BinaryDeserializeInt(FileStream fileStream) {
        int context = -1;
        try {
            BinaryFormatter formatter = new BinaryFormatter();
            context = (int) formatter.Deserialize(fileStream);
        }
        catch {
            Debug.LogAssertion("File Error Has Occured");
        }
        return context;
    }

    private static long BinaryDeserializeLong(FileStream fileStream) {
        long context = -1;
        try {
            BinaryFormatter formatter = new BinaryFormatter();
            context = (long) formatter.Deserialize(fileStream);
        }
        catch {
            Debug.LogAssertion("File Error Has Occured");
        }
        return context;
    }

    private static string BinaryDeserializeString(FileStream fileStream) {
        string context = string.Empty;
        try {
            BinaryFormatter formatter = new BinaryFormatter();
            context = (string) formatter.Deserialize(fileStream);
        }
        catch {
            Debug.LogAssertion("File Error Has Occured");
        }
        return context;
    }

    private static ShipAttributes BinaryDeserializeAttributes(FileStream fileStream) {
        ShipAttributes context = null;
        try {
            BinaryFormatter formatter = new BinaryFormatter();
            context = (ShipAttributes) formatter.Deserialize(fileStream);
        }
        catch {
            Debug.LogAssertion("File Error Has Occured");
        }
        return context;
    }

    private void OnClose()
    {
        Debug.Log($"Replay file closed");
        _fileStream?.Close();
        _br?.Close();
        _bw?.Close();
    }

    private void OnApplicationQuit()
    {
        OnClose();
    }
}
