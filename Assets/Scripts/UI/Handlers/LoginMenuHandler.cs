using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginMenuHandler : MenuHandler
{
    public TMP_InputField m_InputFieldID;
    public TMP_InputField m_InputFieldPW;
    public NetworkAccount m_NetworkAccount;
    public TextErrorMessage m_TextErrorMessage;

    protected override void Init()
    {
        m_InputFieldID.SetTextWithoutNotify(PlayerPrefs.GetString("LastLoginID", string.Empty));
    }

    public void Login()
    {
        EventSystem.current.sendNavigationEvents = false;
        StartCoroutine(m_NetworkAccount.Login(this, m_InputFieldID.text, m_InputFieldPW.text));
    }

    public void Register()
    {
        EventSystem.current.sendNavigationEvents = false;
        StartCoroutine(m_NetworkAccount.SignUp(this, m_InputFieldID.text, m_InputFieldPW.text));
    }

    public void PlayOffline() {
        AudioService.PlaySound("ConfirmUI");
        SceneManager.LoadScene("MainMenu");
    }

    public void ExitGame() {
        AudioService.PlaySound("CancelUI");
        Utility.QuitGame();
    }

    public void TryLogin(string id, string code) {
        EventSystem.current.sendNavigationEvents = true;
        if (code == "UserLoginSucceed" || code == "UserRegisterSucceed") {
            AudioService.PlaySound("ConfirmUI");
            PlayerPrefs.SetString("LastLoginID", id);
            GameManager.SetAccountID(id);
            SceneManager.LoadScene("MainMenu");
        }
        else {
            AudioService.PlaySound("CancelUI");
            m_TextErrorMessage.DisplayText(code);
        }
    }

    public void NetworkError(string errorDetails) {
        EventSystem.current.sendNavigationEvents = true;
        m_TextErrorMessage.DisplayText("NetworkError", errorDetails);
        AudioService.PlaySound("CancelUI");
    }
}