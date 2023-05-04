using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct JsonTest_Explosion {
    ExplosionEffect explosionEffect;
    ExplosionAudio explosionAudio;
    Vector3 position;
    float radius;
    float[] speed;
    float[] direction;
    int duration;
    int[] timer_add;
    int number;
}

public class TestScript : MonoBehaviour
{
    void Start()
    {
        List<JsonTest_Explosion> list = new List<JsonTest_Explosion>() {new JsonTest_Explosion(), new JsonTest_Explosion()};
        string jsonString = JsonUtility.ToJson(list, true);
        Save(jsonString);
    }

    public void Save(string jsonString)
    {
        FileStream fileStream = new FileStream(string.Format("{0}/{1}.json", Application.dataPath, "TestJsonData"), FileMode.Create);
        byte[] data = Encoding.UTF8.GetBytes(jsonString);
        fileStream.Write(data, 0, data.Length);
        fileStream.Close();
    }
}
