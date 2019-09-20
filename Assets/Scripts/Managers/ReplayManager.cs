using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Text;

public class ReplayManager : MonoBehaviour
{
    private int m_RandomSeed;
    private string m_Version;
    private Attributes m_Attributes;
    private byte m_Difficulty;

    private string m_FilePath = string.Empty;
    private FileStream m_FileStream;
    private PlayerController m_PlayerController;
    private PlayerShooter m_PlayerShooter;
    private bool m_StageOverview = false;
    
    private GameManager m_GameManager = null;
    private SystemManager m_SystemManager = null;
    private PlayerManager m_PlayerManager = null;
    
    private StringBuilder m_TotalContext = new StringBuilder();

    public void Init() {
        m_GameManager = GameManager.instance_gm;
        m_SystemManager = SystemManager.instance_sm;
        m_PlayerManager = PlayerManager.instance_pm;

        if (m_SystemManager.m_ReplayState) { // 리플레이 (파일 읽기)
            m_FilePath = m_GameManager.m_ReplayDirectory + "replay" + m_GameManager.m_ReplayNum + ".rep";

            if (System.IO.File.Exists(m_FilePath)) {
                m_FileStream = new FileStream(m_FilePath, FileMode.Open);
                m_RandomSeed = BinaryDeserializeInt(); // 시드 읽기
                m_Version = BinaryDeserializeString(); // 버전 읽기
                m_Attributes = BinaryDeserializeAttributes(); // 기체 스펙 읽기
                m_Difficulty = (byte) BinaryDeserializeInt(); // 난이도 읽기

                if (m_Version.Equals(Application.version)) {
                    m_PlayerManager.SpawnPlayer(m_Attributes);
                }
                else {
                    // ToDo 버전이 다름
                }
                m_SystemManager.SetDifficulty(m_Difficulty);
            }
        }
        else { // 노 리플레이 (파일 쓰기)
            m_FilePath = m_GameManager.m_ReplayDirectory + "replayTemp.rep";
            m_PlayerManager.SpawnPlayer();

            m_FileStream = new FileStream(m_FilePath, FileMode.Create);
            m_RandomSeed = System.Environment.TickCount; // Generate Random Seed
            m_Attributes = m_PlayerManager.m_CurrentAttributes;
            m_Difficulty = m_SystemManager.m_Difficulty;
            BinarySerialize(m_RandomSeed); // 시드 쓰기
            BinarySerialize(Application.version); // 버전 쓰기
            BinarySerialize(m_Attributes); // 기체 스펙 쓰기
            BinarySerialize((int) m_Difficulty); // 난이도 쓰기
            m_FileStream.Close();
            m_FileStream = new FileStream(m_FilePath, FileMode.Append);
        }
        Random.InitState(m_RandomSeed);

        m_PlayerController = m_PlayerManager.m_PlayerController;
        m_PlayerShooter = m_PlayerManager.m_PlayerShooter;
    }

    void FixedUpdate()
    {
        if (Time.timeScale == 0)
            return;
        
        if (m_SystemManager.m_ReplayState) { // 리플레이
            ReadUserInput();
            m_PlayerShooter.PlayerShooterBehaviour();
        }
        else {
            m_PlayerShooter.PlayerShooterBehaviour();
            WriteUserInput();
        }
        m_PlayerShooter.ResetKeyPress();
    }

    private void ReadUserInput() {
        int context = m_FileStream.ReadByte();

        if (m_StageOverview) {
            if (m_SystemManager.m_PlayState == 0) {
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

        if (m_SystemManager.m_PlayState >= 3) {
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
        
        m_FileStream.WriteByte(context);
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

    private void BinarySerialize(Attributes attributes) { // Overload
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

    private Attributes BinaryDeserializeAttributes() {
        Attributes context = null;
        try {
            BinaryFormatter formatter = new BinaryFormatter();
            context = (Attributes) formatter.Deserialize(m_FileStream);
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
