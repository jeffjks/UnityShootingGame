using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class RankingScoreImageDisplay : MonoBehaviour
{
    public Image[] m_AttributeSpeed = new Image[3];
    public Image[] m_AttributeShot = new Image[3];
    public Image[] m_AttributeLaser = new Image[3];
    public Image[] m_AttributeModule = new Image[4];

    private List<Image[]> m_Attributes = new List<Image[]>();
    private GameObject[] m_AttributeSlots = new GameObject[5];

    RankingScoreImageDisplay() {
        m_Attributes.Add(m_AttributeSpeed);
        m_Attributes.Add(m_AttributeShot);
        m_Attributes.Add(m_AttributeLaser);
        m_Attributes.Add(m_AttributeModule);
    }

    void OnDisable()
    {
        DestroyAllAttributeSlots();
    }

    public void DisplayImages(int shipAttributesCode) {
        DestroyAllAttributeSlots();

        bool[] display = { false, true, true, true, true, false };
        int num = -1;
        for (int i = 0; i < display.Length; ++i) {
            if (display[i]) {
                num++;
            }
        }
        int check = 0;

        while(num >= 0) {
            try {
                if (!display[check++]) {
                    shipAttributesCode /= 10;
                    continue;
                }
                m_AttributeSlots[num] = new GameObject("Slot ("+(num+1)+")"); // Color, "Speed", "Shot", "Laser", "Module", Bomb
                
                RectTransform rectTransform = m_AttributeSlots[num].AddComponent<RectTransform>();
                rectTransform.position = new Vector3(num*40f, 0f, 0f);
                rectTransform.sizeDelta = new Vector2(40f, 40f);
                rectTransform.anchorMin = new Vector2(0f, 0.5f);
                rectTransform.anchorMax = new Vector2(0f, 0.5f);
                rectTransform.pivot = new Vector2(0f, 0.5f);
                rectTransform.localScale = new Vector2(1f, 1f);

                Image image = m_AttributeSlots[num].AddComponent<Image>();
                image.sprite = m_Attributes[num][shipAttributesCode % 10].sprite;
                image.color = Color.white;

                m_AttributeSlots[num].transform.SetParent(this.transform, false);
                shipAttributesCode /= 10;
                num--;
            }
            catch (System.IndexOutOfRangeException) {
                num--;
                continue;
            }
        }
    }

    private void DestroyAllAttributeSlots() {
        for (int i = 0; i < m_AttributeSlots.Length; i++) {
            Destroy(m_AttributeSlots[i]);
        }
    }
}
