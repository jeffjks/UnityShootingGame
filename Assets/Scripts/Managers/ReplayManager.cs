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

    public static ReplayManager Instance { get; private set; }

    private struct ReplayInfo
    {
        public readonly int m_Seed;
        public readonly string m_Version;
        public readonly ShipAttributes m_Attributes;
        public readonly GameDifficulty m_Difficulty;

        public ReplayInfo(int seed, string version, ShipAttributes attributes, GameDifficulty difficulty)
        {
            m_Seed = seed;
            m_Version = version;
            m_Attributes = attributes;
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

    public void Init()
    {
        _playerManager = PlayerManager.Instance;

        if (!m_Activate)
        {
            InitPlayer();
            return;
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
        var replayNum = 0; // TODO. have to decide replay num from existing files
        var filePath = $"{_replayDirectory}replay{replayNum}.rep";

        try
        {
            _fileStream = new FileStream(filePath, FileMode.Open);
            _br = new BinaryReader(_fileStream);
            _replayInfo = ReadBinaryHeader();

            if (_replayInfo.m_Version != Application.version)
            {
                // TODO. 버전이 다름
            }

            SystemManager.SetDifficulty(_replayInfo.m_Difficulty);
            Debug.Log($"Start reading replay file.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error has occured while reading replay file: {e}");
        }
        finally {
            InitPlayer(_replayInfo.m_Attributes);
        }
    }

    private void StartWriteReplayFile()
    {
        var filePath = $"{_replayDirectory}replayTemp.rep";
        
        try {
            _fileStream = new FileStream(filePath, FileMode.Append);
            _bw = new BinaryWriter(_fileStream);
            
            var replayInfo = new ReplayInfo(
                Environment.TickCount,
                Application.version,
                PlayerManager.CurrentAttributes,
                SystemManager.Difficulty
            );

            WriteBinaryHeader(replayInfo);
            Debug.Log($"Start writing replay file.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error has occured while reading replay file: {e}");
        }
        finally {
            InitPlayer();
        }
    }

    private void InitPlayer()
    {
        Random.InitState(_replayInfo.m_Seed);
        var player = _playerManager.SpawnPlayer();
        _playerMovement = player.GetComponentInChildren<PlayerMovement>();
        _playerController = player.GetComponentInChildren<PlayerController>();
    }

    private void InitPlayer(ShipAttributes shipAttributes)
    {
        Random.InitState(_replayInfo.m_Seed);
        var player = _playerManager.SpawnPlayer(shipAttributes);
        _playerMovement = player.GetComponentInChildren<PlayerMovement>();
        _playerController = player.GetComponentInChildren<PlayerController>();
    }

    private void Update()
    {
        if (Time.timeScale == 0)
            return;

        if (_playerController.gameObject.activeInHierarchy) {
            if (SystemManager.GameMode == GameMode.Replay) { // 리플레이
                ReadUserInput();
            }
        }
    }

    private void ReadUserInput()
    {
        if (SystemManager.GameMode != GameMode.Replay)
            return;
        
        var context = _br.ReadInt32();

        var moveLeft = context & (1 << 0);
        var moveRight = context & (1 << 1);
        var moveDown = context & (1 << 2);
        var moveUp = context & (1 << 3);
        var moveVector = new Vector2(moveRight - moveLeft, moveUp - moveDown);

        var inputFire = context & (1 << 4);
        var inputBomb = context & (1 << 5);
        
        _playerMovement.MovePlayer(moveVector);
        _playerController.ExecuteFire(inputFire == 1);
        if (inputBomb == 1)
            _playerController.ExecuteBomb();
    }

    public void WriteUserMoveInput(int rawHorizontal, int rawVertical)
    {
        if (SystemManager.GameMode == GameMode.Replay)
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

    private ReplayInfo ReadBinaryHeader()
    {
        var replayInfo = new ReplayInfo(
            BinaryDeserializeInt(),
            BinaryDeserializeString(),
            BinaryDeserializeAttributes(),
            (GameDifficulty)BinaryDeserializeInt()
            );
        return replayInfo;
    }

    private void WriteBinaryHeader(ReplayInfo replayInfo)
    {
        BinarySerialize(replayInfo.m_Seed); // 시드 쓰기
        BinarySerialize(replayInfo.m_Version); // 버전 쓰기
        BinarySerialize(replayInfo.m_Attributes); // 기체 스펙 쓰기
        BinarySerialize((int) replayInfo.m_Difficulty); // 난이도 쓰기
    }

    private int BinaryDeserializeInt() {
        int context = -1;
        try {
            BinaryFormatter formatter = new BinaryFormatter();
            context = (int) formatter.Deserialize(_fileStream);
        }
        catch {
            Debug.LogAssertion("File Error Has Occured");
        }
        return context;
    }

    private string BinaryDeserializeString() {
        string context = string.Empty;
        try {
            BinaryFormatter formatter = new BinaryFormatter();
            context = (string) formatter.Deserialize(_fileStream);
        }
        catch {
            Debug.LogAssertion("File Error Has Occured");
        }
        return context;
    }

    private ShipAttributes BinaryDeserializeAttributes() {
        ShipAttributes context = null;
        try {
            BinaryFormatter formatter = new BinaryFormatter();
            context = (ShipAttributes) formatter.Deserialize(_fileStream);
        }
        catch {
            Debug.LogAssertion("File Error Has Occured");
        }
        return context;
    }

    private void OnClose()
    {
        Debug.Log($"File closed");
        _fileStream?.Close();
        _br?.Close();
        _bw?.Close();
    }

    private void OnApplicationQuit()
    {
        OnClose();
    }
}
