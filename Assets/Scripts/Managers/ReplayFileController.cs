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
    
    private static CryptoStream _cryptoStream;
    private static FileStream _fileStream;
    private static BinaryFormatter _bf;
    private static ReplayFileMode _replayFileMode;
    private const string Key = "key";
    private const int AesKeySize = 128;
    private const int AesBlockSize = 128;

    private static int _currentReplaySlot = -1;

    public static string ReplayFilePath
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
        _bf = new BinaryFormatter();
        _replayFileMode = ReplayFileMode.None;
    }

    public static void InitWritingReplayFile(UnityAction onComplete, int slot = -1)
    {
        // Init Writing
        try
        {
            _replayFileMode = ReplayFileMode.Write;
            _currentReplaySlot = slot;
            _fileStream = new FileStream(ReplayFilePath, FileMode.Create);

            using var aesAlg = Aes.Create();
            aesAlg.KeySize = AesKeySize;
            aesAlg.BlockSize = AesBlockSize;
            aesAlg.Padding = PaddingMode.PKCS7;
            aesAlg.Mode = CipherMode.CBC;

            byte[] keyBytes = Encoding.UTF8.GetBytes(Key);
            Array.Resize(ref keyBytes, 16);
            aesAlg.Key = keyBytes;
            aesAlg.IV = keyBytes;

            ICryptoTransform encryptor = aesAlg.CreateEncryptor();

            _cryptoStream = new CryptoStream(_fileStream, encryptor, CryptoStreamMode.Write);

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
            _fileStream = new FileStream(ReplayFilePath, FileMode.Open);

            using var aesAlg = Aes.Create();
            aesAlg.KeySize = AesKeySize;
            aesAlg.BlockSize = AesBlockSize;
            aesAlg.Padding = PaddingMode.PKCS7;
            aesAlg.Mode = CipherMode.CBC;

            byte[] keyBytes = Encoding.UTF8.GetBytes(Key);
            Array.Resize(ref keyBytes, 16);
            aesAlg.Key = keyBytes;
            aesAlg.IV = keyBytes;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor();

            _cryptoStream = new CryptoStream(_fileStream, decryptor, CryptoStreamMode.Read);
            
            Debug.Log($"Start reading replay file: {ReplayFilePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error has occured while reading replay file: {e}");
        }
        onComplete?.Invoke();
    }

    public static void WriteReplayFile<T>(T obj)
    {
        _bf.Serialize(_cryptoStream, obj);
    }

    public static bool TryReadReplayFile<T>(out T obj)
    {
        if (_fileStream.Position < _fileStream.Length)
        {
            obj = (T)_bf.Deserialize(_cryptoStream);
            return true;
        }

        obj = default;
        return false;
    }

    public static ReplayManager.ReplayInfo ReadBinaryHeader(int slot)
    {
        if (_replayFileMode != ReplayFileMode.None)
        {
            Debug.LogWarning($"현재 ReplayFileController가 이미 사용중입니다.");
            return default;
        }

        InitReadingReplayFile(null, slot);
        //TryReadReplayFile(out ReplayManager.ReplayInfo replayInfo);
        //Debug.Log(new DateTime(replayInfo.m_DateTime).ToString("yyyy-MM-dd-HH:mm"));
        OnClose();
        return default;
        //return replayInfo;
    }

    public static void OnClose()
    {
        switch (_replayFileMode)
        {
            case ReplayFileMode.Write:
                _cryptoStream.FlushFinalBlock();
                _cryptoStream.Close();
                _fileStream.Close();
                break;
            case ReplayFileMode.Read:
                _cryptoStream.Flush();
                _cryptoStream.Close();
                _fileStream.Close();
                break;
        }
        _replayFileMode = ReplayFileMode.None;
    }
}
