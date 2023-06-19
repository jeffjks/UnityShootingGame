using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Text;

public class ReplayManager : MonoBehaviour
{
    public bool m_Activate;

    private int m_RandomSeed;
    private string m_Version;
    private ShipAttributes m_Attributes;
    private int m_Difficulty;

    private string m_FilePath = string.Empty;
    private FileStream m_FileStream;
    private PlayerController m_PlayerController;
    private PlayerShooter m_PlayerShooter;
    private bool m_StageOverview = false;
    
    private GameManager m_GameManager = null;
    private PlayerManager m_PlayerManager = null;
    
    private StringBuilder m_TotalContext = new StringBuilder();

    public void Init() {
        m_GameManager = GameManager.instance_gm;
        m_PlayerManager = PlayerManager.Instance;

        m_RandomSeed = System.Environment.TickCount; // Generate Random Seed

        if (m_Activate) {
            try {
                if (SystemManager.GameMode == GameMode.Replay) { // 리플레이 (파일 읽기)
                    m_FilePath = m_GameManager.m_ReplayDirectory + "replay" + m_GameManager.m_ReplayNum + ".rep";

                    if (System.IO.File.Exists(m_FilePath)) {
                        m_FileStream = new FileStream(m_FilePath, FileMode.Open);
                        m_RandomSeed = BinaryDeserializeInt(); // 시드 읽기
                        m_Version = BinaryDeserializeString(); // 버전 읽기
                        m_Attributes = BinaryDeserializeAttributes(); // 기체 스펙 읽기
                        m_Difficulty = BinaryDeserializeInt(); // 난이도 읽기

                        if (m_Version.Equals(Application.version)) {
                            m_PlayerManager.SpawnPlayer(m_Attributes);
                        }
                        else {
                            // ToDo 버전이 다름
                        }
                        SystemManager.SetDifficulty((GameDifficulty) m_Difficulty);
                    }
                }
                else { // 비 리플레이 (파일 쓰기)
                    m_FilePath = m_GameManager.m_ReplayDirectory + "replayTemp.rep";
                    m_PlayerManager.SpawnPlayer();

                    m_FileStream = new FileStream(m_FilePath, FileMode.Create);
                    m_Attributes = PlayerManager.CurrentAttributes;
                    m_Difficulty = (int) SystemManager.Difficulty;
                    BinarySerialize(m_RandomSeed); // 시드 쓰기
                    BinarySerialize(Application.version); // 버전 쓰기
                    BinarySerialize(m_Attributes); // 기체 스펙 쓰기
                    BinarySerialize((int) m_Difficulty); // 난이도 쓰기
                    m_FileStream.Close();
                    m_FileStream = new FileStream(m_FilePath, FileMode.Append);
                }
            }
            catch (System.NullReferenceException) {
                m_PlayerManager.SpawnPlayer();
            }
            finally {
                InitPlayerState();
            }
        }
        else {
            m_PlayerManager.SpawnPlayer();
            InitPlayerState();
        }
    }

    private void InitPlayerState()
    {
        Random.InitState(m_RandomSeed);
        GameObject player = m_PlayerManager.Player;
        m_PlayerController = player.GetComponent<PlayerController>();
        m_PlayerShooter = player.GetComponent<PlayerShooter>();
    }

    void Update()
    {
        if (Time.timeScale == 0)
            return;
        
        
        if (m_PlayerShooter.gameObject.activeInHierarchy) {
            if (SystemManager.GameMode == GameMode.Replay) { // 리플레이
                ReadUserInput();
                m_PlayerShooter.PlayerShooterBehaviour();
            }
            else {
                m_PlayerShooter.PlayerShooterBehaviour();
                WriteUserInput();
            }
            m_PlayerShooter.ResetKeyPress();
        }
    }

    private void ReadUserInput() {
        int context = m_FileStream.ReadByte();

        if (m_StageOverview) {
            if (SystemManager.PlayState == PlayState.OnField) {
                m_StageOverview = false;
            }
        }
        int overview = (context & 0b0010_0000) >> 7;

        if (context == -1) {
            // Debug.Log(m_TotalContext);
            return;
        }
        else if (overview == 1) {
            m_StageOverview = true;
            return;
        }
        else {
            m_TotalContext.Append(" ");
            m_TotalContext.Append(context.ToString());
        }

        m_PlayerController.MoveRawHorizontal = (context & 0b0000_0011) - 1;
        m_PlayerController.MoveRawVertical = ((context & 0b0000_1100) >> 2) - 1;
        m_PlayerShooter.ShotKeyPress = (context & 0b0001_0000) >> 4;
        m_PlayerShooter.BombKeyPress = (context & 0b0010_0000) >> 5;
    }

    private void WriteUserInput() {
        byte context;
        int movement, shot, bomb;

        if (SystemManager.PlayState == PlayState.OnStageResult || SystemManager.PlayState == PlayState.OnStageTransition) {
            if (!m_StageOverview) { // 최초 Overview 진입시
                m_StageOverview = true;
                context = 0b0000_0001 << 7;
            }
            else // 그 이후
                return;
        }
        else {
            m_StageOverview = false;
            movement = (m_PlayerController.MoveRawHorizontal + 1) + 4*(m_PlayerController.MoveRawVertical + 1);
            shot = m_PlayerShooter.ShotKeyPress;
            bomb = m_PlayerShooter.BombKeyPress;

            context = (byte) (movement + (shot << 4) + (bomb << 5));

            m_TotalContext.Append(" ");
            m_TotalContext.Append(context.ToString());
        }
        // Debug.Log(m_TotalContext);
        
        try {
            m_FileStream.WriteByte(context);
        }
        catch (System.NullReferenceException) {
            return;
        }
    }

    private void BinarySerialize(int seed) {
        try {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(m_FileStream, seed);
        }
        catch {
            Debug.LogAssertion("File Error Has Occured");
        }
    }

    private void BinarySerialize(string version) { // Overload
        try {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(m_FileStream, version);
        }
        catch {
            Debug.LogAssertion("File Error Has Occured");
        }
    }

    private void BinarySerialize(ShipAttributes attributes) { // Overload
        try {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(m_FileStream, attributes);
        }
        catch {
            Debug.LogAssertion("File Error Has Occured");
        }
    }

    private int BinaryDeserializeInt() {
        int context = -1;
        try {
            BinaryFormatter formatter = new BinaryFormatter();
            context = (int) formatter.Deserialize(m_FileStream);
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
            context = (string) formatter.Deserialize(m_FileStream);
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
            context = (ShipAttributes) formatter.Deserialize(m_FileStream);
        }
        catch {
            Debug.LogAssertion("File Error Has Occured");
        }
        return context;
    }

    void OnApplicationQuit()
    {
        try {
            m_FileStream.Close();
        }
        catch {
        }
    }
}
