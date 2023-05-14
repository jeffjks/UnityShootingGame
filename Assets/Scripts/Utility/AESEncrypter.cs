using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class AESEncrypter
{
    //키로 사용하기 위한 암호. 16글자 이상이어야 한다.
    private static string PASSWORD = "DeadPlanet2_JsonEncrypter";
    //인증키 정의
    private static readonly string KEY = PASSWORD.Substring(0, 128 / 8); //8bit단위로 나눔

    //암호화
    public static string AESEncrypt128(string plain)
    {
        byte[] plainBytes = Encoding.UTF8.GetBytes(plain);
        
        RijndaelManaged rijndael = new RijndaelManaged();

        // CipherMode : 암호화 모드 지정
        rijndael.Mode = CipherMode.CBC;

        // PaddingMode : 메시지 데이터 블록이 암호화 작업에 필요한 전체 길이보다 짧을 때 뭘로 채울 것인지 결정
        rijndael.Padding = PaddingMode.PKCS7;

        // KeySize : 패스워드 키 사이즈
        rijndael.KeySize = 128;

        MemoryStream memoryStream = new MemoryStream();

        ICryptoTransform encryptor = rijndael.CreateEncryptor(Encoding.UTF8.GetBytes(KEY), Encoding.UTF8.GetBytes(KEY));

        CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
        cryptoStream.Write(plainBytes, 0, plainBytes.Length);
        cryptoStream.FlushFinalBlock();

        byte[] encryptBytes = memoryStream.ToArray();
        string encryptString = Convert.ToBase64String(encryptBytes);

        cryptoStream.Close();
        memoryStream.Close();

        return encryptString;
    }
	
    //복호화
    public static string AESDecrypt128(string encrypt)
    {
        byte[] encryptBytes = Convert.FromBase64String(encrypt);

        RijndaelManaged rijndael = new RijndaelManaged();
        rijndael.Mode = CipherMode.CBC;
        rijndael.Padding = PaddingMode.PKCS7;
        rijndael.KeySize = 128;

        MemoryStream memoryStream = new MemoryStream(encryptBytes);
        ICryptoTransform decryptor = rijndael.CreateDecryptor(Encoding.UTF8.GetBytes(KEY), Encoding.UTF8.GetBytes(KEY));
        CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

        byte[] plainBytes = new byte[encryptBytes.Length];

        int plainCount = cryptoStream.Read(plainBytes, 0, plainBytes.Length);

        string plainString = Encoding.UTF8.GetString(plainBytes, 0, plainCount);

        cryptoStream.Close();
        memoryStream.Close();

        return plainString;
    }
}