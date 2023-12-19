using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using Random = System.Random;

public class IntegrityTestFailedException : Exception
{
    public IntegrityTestFailedException(string message) : base(message)
    {
    }
}

public static class Utility
{
    public static AnimationCurve[] AnimationCurves = new AnimationCurve[4];

    static Utility()
    {
        /*
        AnimationCurves[0] = new AnimationCurve();
        AnimationCurves[0].AddKey(0f, 0f);
        AnimationCurves[0].AddKey(1f, 1f);
        AnimationCurves[0].keys[0].*/

        /*
        for (int i = 0; i < m_AnimationCurve.Length; ++i) {
            AC_Ease.ac_ease[i] = m_AnimationCurve[i];
        }
        
        Debug.Log(m_AnimationCurve[4].keys[0].inWeight);
        Debug.Log(m_AnimationCurve[4].keys[0].outWeight);
        Debug.Log(m_AnimationCurve[4].keys[0].inTangent);
        Debug.Log(m_AnimationCurve[4].keys[0].outTangent);
        Debug.Log(m_AnimationCurve[4].keys[0].weightedMode);
        AnimationUtility.SetKeyRightTangentMode(m_AnimationCurve[4], 0, AnimationUtility.TangentMode.Linear);
        AnimationUtility.SetKeyLeftTangentMode(m_AnimationCurve[4], 1, AnimationUtility.TangentMode.Linear);*/
    }
    
    // <summary>
    // Enum 다음값 가져오기
    // </summary>
    // <typeparam name="T"></typeparam>
    // <param name="source"></param>
    // <returns></returns>
    
    public static T GetEnumNext<T>(this T source, bool wrapAround) where T : Enum
    {
        var array = Enum.GetValues(typeof(T));
        for (int i = 0; i < array.Length - 1; ++i)
        {
            if (source.Equals(array.GetValue(i)))
                return (T) array.GetValue(i + 1);
        }

        if (wrapAround)
            return (T)array.GetValue(0);
        return source;
    }
    
    public static T GetEnumPrev<T>(this T source, bool wrapAround) where T : Enum
    {
        var array = Enum.GetValues(typeof(T));
        for (int i = 0; i < array.Length - 1; ++i)
        {
            if (source.Equals(array.GetValue(array.Length - i - 1)))
                return (T) array.GetValue(array.Length - i - 2);
        }

        if (wrapAround)
            return (T)array.GetValue(array.Length - 1);
        return source;
    }

    public static int GetEnumCount<T>(this T source) where T : Enum
    {
        var array = Enum.GetValues(typeof(T));
        return array.Length;
    }
    

    public static bool CheckLayer(this GameObject obj, int layerValue)
    {
        if ((1 << obj.layer & layerValue) != 0)
        {
            return true;
        }

        return false;
    }

    public static string Md5Sum(string strToEncrypt) {
        UTF8Encoding ue = new UTF8Encoding();
        byte[] bytes = ue.GetBytes(strToEncrypt);
    
        // encrypt bytes
        System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] hashBytes = md5.ComputeHash(bytes);
    
        // Convert the encrypted bytes back to a string (base 16)
        string hashString = String.Empty;
    
        for (int i = 0; i < hashBytes.Length; i++) {
            hashString += Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }
    
        return hashString.PadLeft(32, '0');
    }

    public static void SaveDataFile<T>(string filePath, string fileName, T gameData)
    {
        string str = JsonConvert.SerializeObject(gameData, Formatting.None);
        string md5 = Md5Sum(str);
        str += md5;
        string encryptedStr = AESEncrypter.AESEncrypt128(str);
        
        FileStream fileStream = new FileStream($"{filePath}{fileName}", FileMode.Create);
        byte[] data = Encoding.UTF8.GetBytes(encryptedStr);
        
        #if UNITY_EDITOR
        Debug.Log($"파일 생성이 완료되었습니다: {fileName}, {data.Length} Bytes, 해쉬값 : {md5}");
        #endif
        
        fileStream.Write(data, 0, data.Length);
        fileStream.Close();
    }

    public static (T jsonData, string hash) LoadDataFile<T>(string filePath, string fileName)
    {
        #if UNITY_EDITOR
        //Debug.Log($"파일 열기를 시도합니다: {fileName}");
        #endif
        try
        {
            var (jsonData, hash) = LoadDataFileString(filePath, fileName);
            if (Md5Sum(jsonData) != hash)
            {
                throw new IntegrityTestFailedException($"무결성 검사 실패: {fileName}");
            }
            var deserializedData = JsonConvert.DeserializeObject<T>(jsonData);
            Debug.Log($"파일 열기에 성공했습니다: {fileName}");
            return (deserializedData, hash);
        }
        catch (Exception e)
        {
            Debug.LogError($"파일 열기에 실패했습니다: {fileName}\n{e}");
            return (default, string.Empty);
        }
    }

    public static (string, string) LoadDataFileString(string filePath, string fileName) {
        FileStream fileStream = new FileStream($"{filePath}{fileName}", FileMode.Open);
        byte[] data = new byte[fileStream.Length];
        fileStream.Read(data, 0, data.Length);
        fileStream.Close();
        string encryptedStr = Encoding.UTF8.GetString(data);

        if (encryptedStr == string.Empty)
        {
            return (null, null);
        }
        
        
        string decryptedStr = AESEncrypter.AESDecrypt128(encryptedStr);
        var jsonData = decryptedStr.Substring(0, decryptedStr.Length - 32);
        var hash = decryptedStr.Substring(decryptedStr.Length - 32);
        return (jsonData, hash);
    }
        
    public static Texture2D ToTexture2D(this RenderTexture rTex, Vector2 imageSize)
    {
        Texture2D tex = new Texture2D((int)imageSize.x, (int)imageSize.y, TextureFormat.ARGB4444, false);
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }
    
    public static Vector2 GetRandomPositionInsideCircle(float radius = 1f)
    {
        var random = new Random();
        var azimuthalAngle = random.NextDouble() * 2d * Math.PI;
        //var polarAngle = Math.Acos(2d * random.NextDouble() - 1d);
        var distance = radius * Math.Cbrt(random.NextDouble());
        //var sinPolarAngle = Math.Sin(polarAngle);
        var x = (float)(distance * /*sinPolarAngle * */Math.Cos(azimuthalAngle));
        var y = (float)(distance * /*sinPolarAngle * */Math.Sin(azimuthalAngle));
        //var z = (float)(distance * Math.Cos(polarAngle));
        return new Vector2(x, y);
    }

    public static void QuitGame()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }
}
