using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;  
using System.IO;  

public class ReplayManager : MonoBehaviour
{
    private int m_RandomSeed;
    private string m_FilePath = "D:" + "/replay.rep";
    private FileStream m_FileStream;
    private PlayerController m_PlayerController;
    private PlayerShooter m_PlayerShooter;
    
    private SystemManager m_SystemManager = null;
    private PlayerManager m_PlayerManager = null;

    public void Init() {
        m_SystemManager = SystemManager.instance_sm;
        m_PlayerManager = PlayerManager.instance_pm;
        m_PlayerController = m_PlayerManager.m_PlayerController;
        m_PlayerShooter = m_PlayerManager.m_PlayerShooter;

        if (m_SystemManager.m_ReplayState) { // 리플레이 (파일 읽기)
            if (System.IO.File.Exists(m_FilePath)) {
                m_FileStream = new FileStream(m_FilePath, FileMode.Open);
                m_RandomSeed = BinaryDeserialize(); // 시드 기록
                // ToDo 버전 기록
                // ToDo 기체 스펙 기록
            }
        }
        else { // 노 리플레이 (파일 쓰기)
            m_FileStream = new FileStream(m_FilePath, FileMode.Create);
            m_RandomSeed = System.Environment.TickCount; // Generate Random Seed
            BinarySerialize(m_RandomSeed);
            m_FileStream = new FileStream(m_FilePath, FileMode.Append);
        }
        Random.InitState(m_RandomSeed);
    }

    void FixedUpdate()
    {
        if (m_SystemManager.m_ReplayState) {
            BinaryReadByte();
            m_PlayerShooter.PlayerShooterBehaviour();
        }
        else {
            m_PlayerShooter.PlayerShooterBehaviour();
            RecordUserInput();
        }
    }

    private void BinaryReadByte() {
        int context = m_FileStream.ReadByte();
        if (context == -1)
            return;
        if (!m_PlayerManager.PlayerControlable) {
            return;
        }
        m_PlayerController.m_MoveRawHorizontal = (context & 0b0000_0011) - 1;
        m_PlayerController.m_MoveRawVertical = ((context & 0b0000_1100) >> 2) - 1;
        m_PlayerShooter.m_ShotKeyPress = (context & 0b0001_0000) >> 4;
        m_PlayerShooter.m_BombKeyPress = (context & 0b0010_0000) >> 5;
    }

    private void RecordUserInput() {
        byte write;
        int movement, shot, bomb;
        
        movement = (m_PlayerController.m_MoveRawHorizontal + 1) + 4*(m_PlayerController.m_MoveRawVertical + 1);
        shot = m_PlayerShooter.m_ShotKeyPress;
        bomb = m_PlayerShooter.m_BombKeyPress;

        write = (byte) (movement + (shot << 4) + (bomb << 5));
        
        m_FileStream.WriteByte(write);
    }

    private void BinarySerialize(int seed) {
        try {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(m_FileStream, seed);
            m_FileStream.Close();
        }
        catch {
            Debug.LogAssertion("File Error Has Occured");
        }
    }

    private int BinaryDeserialize() {
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
}
