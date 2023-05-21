using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ServerMenuHandler : GameUI
{
    public NetworkAccount m_NetworkAccount;

    void Update()
	{
        int moveRawVertical = (int) Input.GetAxisRaw("Vertical");

        if (Input.GetButtonDown("Fire1")) {
            switch(m_Selection) {
                case 0:
                    Reconnect();
                    break;
                case 1:
                    PlayOffline();
                    break;
                case 2:
                    ExitGame();
                    break;
                default:
                    break;
            }
        }

        MoveCursorVertical(moveRawVertical);
        m_Selection = EndToStart(m_Selection, m_Total);
        SetColor();
	}

    private void Reconnect() {
        AudioService.PlaySound("ConfirmUI");
        m_NetworkAccount.InitServer();
    }

    private void PlayOffline() {
        AudioService.PlaySound("ConfirmUI");
        SceneManager.LoadScene("MainMenu");
    }

    private void ExitGame() {
        AudioService.PlaySound("CancelUI");
        Application.Quit(); // 에디터에서는 무시됨
    }
}