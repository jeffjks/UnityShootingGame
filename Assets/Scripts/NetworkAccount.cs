using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class NetworkAccount : MonoBehaviour
{
    public GameObject m_LoginPanel;

    private GameManager m_GameManager = null;

    void Start()
    {
        InitServer();

        m_GameManager = GameManager.instance_gm;
        
        if (!m_LoginPanel.activeInHierarchy) {
            SceneManager.LoadScene("MainMenu");
            return;
        }

        m_LoginPanel.SetActive(true);
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

        UnityWebRequest webRequest = UnityWebRequest.Post(url, form);
        yield return webRequest.SendWebRequest();
        if(webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError) {
            handler.NetworkError(webRequest.error);
        }
        else {
            handler.TryLogin(id, webRequest.downloadHandler.text);
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

        UnityWebRequest webRequeset = UnityWebRequest.Post(url, form);
        yield return webRequeset.SendWebRequest();
        if(webRequeset.result == UnityWebRequest.Result.ConnectionError || webRequeset.result == UnityWebRequest.Result.ProtocolError) {
            handler.NetworkError(webRequeset.error);
        }
        else {
            handler.TryLogin(id, webRequeset.downloadHandler.text);
        }
        yield return null;
    }
}
