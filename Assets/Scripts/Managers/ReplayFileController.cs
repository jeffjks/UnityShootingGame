using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.Events;

public class ReplayFileController : MonoBehaviour
{
    private enum ReplayFileMode
    {
        None,
        Read,
        Write
    }
    
    private static ReplayFileMode _replayFileMode;
    private static CryptoStream _cryptoStream;
    private static FileStream _fileStream;
    private static BinaryWriter _bw;
    private static BinaryReader _br;
    private static int _currentReplaySlot = -1;
    
    private const int AesKeySize = 128;
    private const int AesBlockSize = 128;
    
    private static readonly byte[] _aesKey =
    {
        0xF2, 0x26, 0x09, 0xBA, 0xC6, 0x4E, 0x81, 0xCD,
        0x0A, 0x3B, 0x61, 0x84, 0x7F, 0xDA, 0x50, 0xB9
    };

    private static string ReplayFilePath
    {
        get
        {
            if (_currentReplaySlot == -1)
                return $"{Application.dataPath}/replayTemp.rep";
            return $"{Application.dataPath}/replay{_currentReplaySlot}.rep";
        }
    }

    private void Awake()
    {
        _replayFileMode = ReplayFileMode.None;
    }

    public static void InitWritingReplayFile(UnityAction onComplete, int slot = -1)
    {
        // Init Writing
        try
        {
            _replayFileMode = ReplayFileMode.Write;
            _currentReplaySlot = slot;
            _fileStream = new FileStream(ReplayFilePath, FileMode.Create, FileAccess.Write);

            using var aesAlg = Aes.Create();
            aesAlg.KeySize = AesKeySize;
            aesAlg.BlockSize = AesBlockSize;
            aesAlg.Padding = PaddingMode.PKCS7;
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Key = _aesKey;
            
            aesAlg.GenerateIV();
            _fileStream.Write(aesAlg.IV, 0, aesAlg.IV.Length);

            ICryptoTransform encryptor = aesAlg.CreateEncryptor();

            _cryptoStream = new CryptoStream(_fileStream, encryptor, CryptoStreamMode.Write);
            _bw = new BinaryWriter(_cryptoStream);

            Debug.Log($"Start writing replay file: {ReplayFilePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error has occured while writing replay file: {e}");
        }
        onComplete?.Invoke();
    }

    public static void InitReadingReplayFile(UnityAction onComplete, int slot = -1)
    {
        try
        {
            _replayFileMode = ReplayFileMode.Read;
            _currentReplaySlot = slot;
            _fileStream = new FileStream(ReplayFilePath, FileMode.Open, FileAccess.Read);

            using var aesAlg = Aes.Create();
            aesAlg.KeySize = AesKeySize;
            aesAlg.BlockSize = AesBlockSize;
            aesAlg.Padding = PaddingMode.PKCS7;
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Key = _aesKey;
            
            if (_fileStream.Read(aesAlg.IV, 0, aesAlg.IV.Length) < aesAlg.IV.Length)
                throw new Exception("Failed to get initial vector");

            ICryptoTransform decryptor = aesAlg.CreateDecryptor();

            _cryptoStream = new CryptoStream(_fileStream, decryptor, CryptoStreamMode.Read);
            _br = new BinaryReader(_cryptoStream);
            
            Debug.Log($"Start reading replay file: {ReplayFilePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error has occured while reading replay file: {e}");
        }
        onComplete?.Invoke();
    }

    public static void WriteBinaryReplayInfo(ReplayManager.ReplayInfo data)
    {
        _bw.Write(data.m_Seed);
        _bw.Write(data.m_DateTime);
        _bw.Write(data.m_Version);
        _bw.Write(data.m_Attributes.GetAttributesCode());
        _bw.Write(data.m_PlayerAttackLevel);
        _bw.Write((int)data.m_GameMode);
        _bw.Write(data.m_Stage);
        _bw.Write((int)data.m_Difficulty);
    }

    public static void WriteBinaryReplayData(ReplayManager.ReplayData data)
    {
        _bw.Write(data.GetData());
    }

    public static ReplayManager.ReplayInfo ReadBinaryReplayInfo()
    {
        var data = new ReplayManager.ReplayInfo(
            _br.ReadInt32(),
            _br.ReadInt64(),
            _br.ReadString(),
            new ShipAttributes(_br.ReadString()),
            _br.ReadInt32(),
            (GameMode) _br.ReadInt32(),
            _br.ReadInt32(),
            (GameDifficulty)_br.ReadInt32()
            );
        return data;
    }

    public static ReplayManager.ReplayData ReadBinaryReplayData()
    {
        return new ReplayManager.ReplayData(_br.ReadInt64());
    }

    public static ReplayManager.ReplayInfo ReadReplayHeader(int slot)
    {
        if (_replayFileMode != ReplayFileMode.None)
        {
            Debug.LogWarning($"현재 ReplayFileController가 이미 사용중입니다.");
            return default;
        }

        if (!File.Exists(ReplayFilePath))
            return default;

        InitReadingReplayFile(null, slot);
        var replayInfo = ReadBinaryReplayInfo();
        
        DiscardRemainingCryptoStream();
        
        return replayInfo;
    }

    private static void DiscardRemainingCryptoStream()
    {
        const int bufferSize = 1024;
        var buffer = new byte[bufferSize];
        var cnt = 0;
        while (_cryptoStream.Read(buffer, 0, buffer.Length) > 0)
        {
            cnt++;
        }
        
#if UNITY_EDITOR
        Debug.Log($"RemainingSize: {cnt * bufferSize}");
#endif
        
        OnClose();
    }

    public static void OnClose()
    {
        switch (_replayFileMode)
        {
            case ReplayFileMode.Write:
                _cryptoStream.FlushFinalBlock();
                _cryptoStream.Close();
                _fileStream.Close();
                _bw.Close();
                break;
            case ReplayFileMode.Read:
                _cryptoStream.Flush();
                _cryptoStream.Close();
                _fileStream.Close();
                _br.Close();
                break;
        }
        _replayFileMode = ReplayFileMode.None;
        Debug.Log($"Replay file closed");
    }
}
