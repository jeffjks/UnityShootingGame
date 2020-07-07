using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginMenuHandler : GameUI
{
    public InputField m_InputFieldID, m_InputFieldPW;
    public NetworkAccount m_NetworkAccount;
    public TextUI_ErrorMessage m_ErrorMessage;

    private bool m_Active = true;

    void Start()
    {
        m_InputFieldID.text = PlayerPrefs.GetString("LastLoginID", string.Empty);

        m_GameManager = GameManager.instance_gm;
    }

    void Update()
	{
        int moveRawVertical = (int) Input.GetAxisRaw("Vertical");

        if (m_Active) {
            if (Input.GetButtonDown("Fire1")) {
                switch(m_Selection) {
                    case 2:
                        Login();
                        break;
                    case 3:
                        Register();
                        break;
                    case 4:
                        PlayOffline();
                        break;
                    case 5:
                        ExitGame();
                        break;
                    default:
                        break;
                }
            }

            MoveCursorVertical(moveRawVertical);
        }
        m_Selection = EndToStart(m_Selection, m_Total);
        SetColor();

        switch(m_Selection) {
            case 0:
                if (!m_InputFieldID.isFocused)
                    m_InputFieldID.ActivateInputField();
                m_InputFieldPW.DeactivateInputField();
                break;
            case 1:
                if (!m_InputFieldPW.isFocused)
                    m_InputFieldPW.ActivateInputField();
                m_InputFieldID.DeactivateInputField();
                break;
            default:
                m_InputFieldID.DeactivateInputField();
                m_InputFieldPW.DeactivateInputField();
                break;
        }
	}

    private void Login() {
        m_Active = false;
        StartCoroutine(m_NetworkAccount.Login(this, m_InputFieldID.text, m_InputFieldPW.text));
    }

    private void Register() {
        m_Active = false;
        StartCoroutine(m_NetworkAccount.SignUp(this, m_InputFieldID.text, m_InputFieldPW.text));
    }

    private void PlayOffline() {
        ConfirmSound();
        SceneManager.LoadScene("MainMenu");
    }

    private void ExitGame() {
        CancelSound();
        Application.Quit(); // 에디터에서는 무시됨
        m_InputFieldID.DeactivateInputField();
        m_InputFieldPW.DeactivateInputField();
    }

    public void TryLogin(string id, string code) {
        m_Active = true;
        if (code == "UserLoginSucceed" || code == "UserRegisterSucceed") {
            ConfirmSound();
            PlayerPrefs.SetString("LastLoginID", id);
            m_GameManager.SetAccountID(id);
            SceneManager.LoadScene("MainMenu");
            m_InputFieldID.DeactivateInputField();
            m_InputFieldPW.DeactivateInputField();
        }
        else {
            CancelSound();
            m_ErrorMessage.DisplayText(code);
        }
    }

    public void NetworkError(string errorDetails) {
        m_Active = true;
        m_ErrorMessage.DisplayText("NetworkError", errorDetails);
        CancelSound();
    }
}