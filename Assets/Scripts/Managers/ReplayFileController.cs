using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Events;

public class ReplayFileController : MonoBehaviour
{
    private enum ReplayFileMode
    {
        None,
        Read,
        Write,
        Error
    }

    public enum ErrorCode
    {
        None,
        NoFile,
        Error
    }
    
    private static ReplayFileMode _replayFileMode;
    private static CryptoStream _cryptoStream;
    private static FileStream _fileStream;
    private static readonly BinaryFormatter _formatter = new();
    
    private const int AesKeySize = 128;
    private const int AesBlockSize = 128;
    
    private static readonly byte[] _aesKey =
    {
        0xF2, 0x26, 0x09, 0xBA, 0xC6, 0x4E, 0x81, 0xCD,
        0x0A, 0x3B, 0x61, 0x84, 0x7F, 0xDA, 0x50, 0xB9
    };

    private void Awake()
    {
        _replayFileMode = ReplayFileMode.None;
    }

    public static string GetReplayFilePath(int slot = -1)
    {
        if (slot == -1)
            return $"{GameManager.ReplayFilePath}replayTemp.rep";
        return $"{GameManager.ReplayFilePath}replay{slot}.rep";
    }

    public static bool InitWritingReplayFile(UnityAction onComplete, int slot = -1)
    {
        if (_replayFileMode != ReplayFileMode.None)
        {
            Debug.LogError($"Replay file is already in use!");
            return false;
        }
        
        // Init Writing
        try
        {
            var filePath = GetReplayFilePath(slot);
            _fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);

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
            //_bw = new BinaryWriter(_cryptoStream);

            Debug.Log($"Start writing replay file: {filePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error has occured while writing replay file: {e}");
            return false;
        }
        
        _replayFileMode = ReplayFileMode.Write;
        onComplete?.Invoke();
        return true;
    }

    public static bool InitReadingReplayFile(UnityAction onComplete, int slot)
    {
        if (_replayFileMode != ReplayFileMode.None)
        {
            Debug.LogError($"Replay file is already in use!");
            return false;
        }
        
        try
        {
            var filePath = GetReplayFilePath(slot);
            _fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            using var aesAlg = Aes.Create();
            aesAlg.KeySize = AesKeySize;
            aesAlg.BlockSize = AesBlockSize;
            aesAlg.Padding = PaddingMode.PKCS7;
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Key = _aesKey;

            var initialVector = new byte[16];
            var initialVectorLength = _fileStream.Read(initialVector, 0, 16);
            if (initialVectorLength < 16)
                throw new CryptographicException();
            aesAlg.IV = initialVector;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor();

            _cryptoStream = new CryptoStream(_fileStream, decryptor, CryptoStreamMode.Read);
            //_br = new BinaryReader(_cryptoStream);
            
            Debug.Log($"Start reading replay file: {filePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error has occured while reading replay file: {e}");
            return false;
        }
        
        _replayFileMode = ReplayFileMode.Read;
        onComplete?.Invoke();
        return true;
    }

    public static void WriteBinaryReplayInfo(ReplayManager.ReplayInfo data)
    {
        _formatter.Serialize(_cryptoStream, data);
    }

    public static void WriteBinaryReplayData(ReplayManager.ReplayDataType dataType, ReplayManager.ReplayData data)
    {
        _formatter.Serialize(_cryptoStream, dataType);
        _formatter.Serialize(_cryptoStream, data);
        //_bw.Write(data.GetData());
    }

    public static ReplayManager.ReplayInfo ReadBinaryReplayInfo()
    {
        var data = (ReplayManager.ReplayInfo) _formatter.Deserialize(_cryptoStream);
        return data;
    }

    public static ReplayManager.ReplayDataType ReadBinaryReplayDataType()
    {
        var dataType = (ReplayManager.ReplayDataType) _formatter.Deserialize(_cryptoStream);
        return dataType;
    }

    public static T ReadBinaryReplayData<T>() where T : ReplayManager.ReplayData
    {
        return (T) _formatter.Deserialize(_cryptoStream);
    }

    public static ReplayManager.ReplayInfo ReadReplayHeader(int slot, out ErrorCode result)
    {
        var filePath = GetReplayFilePath(slot);
        if (!File.Exists(filePath))
        {
            result = ErrorCode.NoFile;
            return null;
        }

        if (!InitReadingReplayFile(null, slot))
        {
            result = ErrorCode.Error;
            return null;
        }

        try
        {
            var replayInfo = ReadBinaryReplayInfo();
            result = ErrorCode.None;
            OnClose();
            return replayInfo;
        }
        catch (Exception e)
        {
            _replayFileMode = ReplayFileMode.Error;
            result = ErrorCode.Error;
            Debug.LogError($"Error has occurred while reading replay header:\n{e}");
            OnClose();
            return null;
        }
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
    }

    public static void OnClose()
    {
        switch (_replayFileMode)
        {
            case ReplayFileMode.Write:
                _cryptoStream.FlushFinalBlock();
                _cryptoStream.Close();
                _fileStream.Close();
                //_bw.Close();
                break;
            case ReplayFileMode.Read:
                DiscardRemainingCryptoStream();
                _cryptoStream.Flush();
                _cryptoStream.Close();
                _fileStream.Close();
                //_br.Close();
                break;
            case ReplayFileMode.Error:
                _fileStream.Close();
                break;
            default:
                return;
        }
        _replayFileMode = ReplayFileMode.None;
        Debug.Log($"Replay file closed");
    }
}
