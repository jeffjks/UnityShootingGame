using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Text;
using UnityEditor.UIElements;
using Random = UnityEngine.Random;

public class ReplayManager : MonoBehaviour
{
    public bool m_Activate;
    
    private FileStream _fileStream;
    private PlayerMovement _playerMovement;
    private PlayerController _playerController;
    private bool m_StageOverview = false;
    
    private PlayerManager _playerManager;
    
    private const string REPLAY_DIRECTORY = "";
    private ReplayInfo _replayInfo;
    private BinaryReader _br = null;
    private BinaryWriter _bw = null;

    public static ReplayManager Instance { get; private set; }

    private struct ReplayInfo
    {
        public int m_Seed;
        public string m_Version;
        public ShipAttributes m_Attributes;
        public GameDifficulty m_Difficulty;

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
            ReadReplayFile();
        }
        else
        {
            WriteReplayFile();
        }
    }

    private void WriteReplayFile()
    {
        var filePath = $"{REPLAY_DIRECTORY}replayTemp.rep";
        
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
        }
        catch (Exception e)
        {
            Debug.Log($"Error has occured while reading replay file: {e}");
        }
        finally {
            InitPlayer();
        }
    }

    private void ReadReplayFile()
    {
        var replayNum = 0; // TODO. have to decide replay num from existing files
        var filePath = $"{REPLAY_DIRECTORY}replay{replayNum}.rep";

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
        }
        catch (Exception e)
        {
            Debug.Log($"Error has occured while reading replay file: {e}");
        }
        finally {
            InitPlayer(_replayInfo.m_Attributes);
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

    void Update()
    {
        if (Time.timeScale == 0)
            return;

        if (_playerController.gameObject.activeInHierarchy) {
            if (SystemManager.GameMode == GameMode.Replay) { // 리플레이
                ReadUserInput();
                _playerController.PlayerControllerBehaviour();
            }
            else {
                _playerController.PlayerControllerBehaviour();
                WriteUserInput();
            }
            //_playerController.ResetKeyPress();
        }
    }

    private void ReadUserInput()
    {
        if (!PlayerUnit.IsControllable)
            return;

        var context = _br.ReadInt32();
        
        //var context = _fileStream.ReadByte();
        
        _playerMovement.MoveRawHorizontal = (context & 0b0000_0011) - 1;
        _playerMovement.MoveRawVertical = ((context & 0b0000_1100) >> 2) - 1;
        //_playerController.ShotKeyPress = (context & 0b0001_0000) >> 4;
        //_playerController.BombKeyPress = (context & 0b0010_0000) >> 5;
    }

    private void WriteUserInput()
    {
        if (!PlayerUnit.IsControllable)
            return;
        
        var context = 0;

        var moveLeft = _playerMovement.MoveRawHorizontal == -1 ? 1 : 0;
        var moveRight = _playerMovement.MoveRawHorizontal == 1 ? 1 : 0;
        var moveDown = _playerMovement.MoveRawVertical == -1 ? 1 : 0;
        var moveUp = _playerMovement.MoveRawVertical == 1 ? 1 : 0;

        context |= moveLeft | (moveRight << 1) | (moveDown << 2)| (moveUp << 3);
        context |= (_playerController.IsFirePressed ? 1 : 0) << 4;
        context |= (_playerController.IsBombPressed ? 1 : 0) << 5;

        _bw.Write(context);
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

    private void OnApplicationQuit()
    {
        _fileStream.Close();
    }
}
