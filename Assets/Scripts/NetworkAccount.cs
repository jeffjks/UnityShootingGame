using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkAccount : MonoBehaviour
{
    public GameObject[] m_ServerMenus;
    public GameObject[] m_LoginMenus;

    void Start()
    {
        InitServer();
        for (int i = 0; i < m_LoginMenus.Length; i++) {
            m_LoginMenus[i].SetActive(true);
        }
    }

    public void InitServer() {

    }

    public IEnumerator Login(LoginMenuHandler handler, string id, string pw) {
        string url = "http://jeffjks.cafe24.com/DeadPlanet2php/userLogin.php";

        if (id.Length < 4 || pw.Length < 6) {
            handler.TryLogin(id, "BadUnauthorizedException");
            yield break;
        }

        WWWForm form = new WWWForm();
        form.AddField("userID", id);
        form.AddField("userPW", pw);
        form.AddField("pcID", SystemInfo.deviceUniqueIdentifier);

        UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();
        if(www.isNetworkError || www.isHttpError) {
            handler.NetworkError(www.error);
        }
        else {
            handler.TryLogin(id, www.downloadHandler.text);
        }
        yield return null;
    }

    public IEnumerator SignUp(LoginMenuHandler handler, string id, string pw) {
        string url = "http://jeffjks.cafe24.com/DeadPlanet2php/userRegister.php";

        if (id.Length < 4 || pw.Length < 6) {
            handler.TryLogin(id, "InvalidSignUpException");
            yield break;
        }

        WWWForm form = new WWWForm();
        form.AddField("userID", id);
        form.AddField("userPW", pw);
        form.AddField("pcID", SystemInfo.deviceUniqueIdentifier);

        UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();
        if(www.isNetworkError || www.isHttpError) {
            handler.NetworkError(www.error);
        }
        else {
            handler.TryLogin(id, www.downloadHandler.text);
        }
        yield return null;
    }
}
