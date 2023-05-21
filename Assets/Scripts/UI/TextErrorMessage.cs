using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Xml;
using TMPro;

public class TextErrorMessage : MonoBehaviour
{
    public TMP_Text m_Text;
    public float m_DisplayTime;

    private Color m_DefaultColor;
    private Color m_DefaultColor_0;
    private IEnumerator m_TextAnimation;
    private string m_XML = "textData";
    
    private GameManager m_GameManager = null;

    void Start()
    {
        m_DefaultColor = m_Text.color;
        m_DefaultColor_0 = new Color(m_DefaultColor.r, m_DefaultColor.g, m_DefaultColor.b, 0f);

        m_GameManager = GameManager.instance_gm;
    }

    private string GetErrorMessage(string errorCode)
    {
        string errorMessage = string.Empty;

        if (errorCode == "BlockedUserException" || errorCode == "BlockedPCException") {
            m_GameManager.SetAccountID(string.Empty);
            m_GameManager.m_IsOnline = false;
        }

        try {
            TextAsset textAsset = (TextAsset) Resources.Load(m_XML);
            XmlDocument xmlDoc = new XmlDocument();
            XmlNode xNode;
            xmlDoc.LoadXml(textAsset.text);

            if (GameSetting.m_Language == Language.English) {
                xNode = xmlDoc.SelectSingleNode("ErrorMessage").SelectSingleNode(errorCode).SelectSingleNode("Eng");
            }
            else {
                xNode = xmlDoc.SelectSingleNode("ErrorMessage").SelectSingleNode(errorCode).SelectSingleNode("Kor");
            }
            errorMessage = xNode.InnerText.ToString();

            Resources.UnloadAsset(textAsset);
        }
        catch {
            if (GameSetting.m_Language == Language.English) {
                errorMessage = "Unknown error has occured.";
            }
            else {
                errorMessage = "알 수 없는 오류가 발생하였습니다.";
            }
        }
        return errorMessage;
    }

    public void DisplayText(string errorCode, string errorDetails = "") {
        m_Text.text = GetErrorMessage(errorCode);
        if (errorDetails != "") {
            if (GameSetting.m_Language == Language.English)
                m_Text.text += "\nError Code : "+errorDetails;
            else
                m_Text.text += "\n에러 코드 : "+errorDetails;
        }

        if (m_TextAnimation != null) {
            StopCoroutine(m_TextAnimation);
        }
        m_TextAnimation = TextAnimation();
        StartCoroutine(m_TextAnimation);
    }

    private IEnumerator TextAnimation() {
        float alpha = 1f;
        float timer = 1f;
        m_Text.color = new Color(m_DefaultColor.r, m_DefaultColor.g, m_DefaultColor.b, alpha);
        yield return new WaitForSeconds(m_DisplayTime);
        while(alpha > 0f) {
            m_Text.color = Color.Lerp(m_DefaultColor_0, m_DefaultColor, alpha);
            alpha -= Time.deltaTime / timer;
            yield return null;
        }
        yield break;
    }
}
