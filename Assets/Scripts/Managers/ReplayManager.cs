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

    public static ReplayManager Instance { get; private set; }

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
    }

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
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
            
            var replayInfo = new ReplayInfo(
                SystemManager.CurrentSeed,
                DateTime.Now.Ticks,
                Application.version,
                PlayerManager.CurrentAttributes,
                playerAttackLevel,
                SystemManager.GameMode,
                SystemManager.Stage,
                SystemManager.Difficulty
            );

            WriteBinaryHeader(replayInfo);
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

    private void Update()
    {
        if (PauseManager.IsGamePaused)
            return;
        
        ReadUserInput();
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

    private void ReadUserInput()
    {
        if (SystemManager.GameMode != GameMode.Replay)
            return;
        if (!PlayerUnit.IsControllable)
            return;
        if (_fileStream.Position >= _fileStream.Length)
        {
            if (!_eof)
                Debug.Log($"Replay file reached end of file.");
            _eof = true;
            return;
        }
        
        var context = _br.ReadInt32();

        var moveLeft = (context >> 0) & 1;
        var moveRight = (context >> 1) & 1;
        var moveDown = (context >> 2) & 1;
        var moveUp = (context >> 3) & 1;
        var moveVector = new Vector2(moveRight - moveLeft, moveUp - moveDown);

        var inputFire = (context >> 4) & 1;
        var inputBomb = (context >> 5) & 1;
        
        _playerMovement.MovePlayer(moveVector);
        _playerController.ExecuteFire(inputFire == 1);
        if (inputBomb == 1)
            _playerController.ExecuteBomb();
    }

    public void WriteUserMoveInput(int rawHorizontal, int rawVertical)
    {
        if (SystemManager.GameMode == GameMode.Replay)
            return;
        if (!PlayerUnit.IsControllable)
            return;
        
        var moveLeft = rawHorizontal == -1 ? 1 : 0;
        var moveRight = rawHorizontal == 1 ? 1 : 0;
        var moveDown = rawVertical == -1 ? 1 : 0;
        var moveUp = rawVertical == 1 ? 1 : 0;

        _context |= moveLeft | (moveRight << 1) | (moveDown << 2)| (moveUp << 3);
        _context |= (_playerController.IsFirePressed ? 1 : 0) << 4;
        _context |= (_playerController.IsBombPressed ? 1 : 0) << 5;
    }

    public void WriteUserPressInput(bool isPressed, int offset)
    {
        if (SystemManager.GameMode == GameMode.Replay)
            return;
        
        var inputFire = isPressed ? 1 : 0;
        
        _context |= inputFire << offset;
    }

    public void LateUpdate()
    {
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
