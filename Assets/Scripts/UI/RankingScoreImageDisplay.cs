using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class RankingScoreImageDisplay : MonoBehaviour
{
    public Image[] m_AttributeSpeed = new Image[3];
    public Image[] m_AttributeShotForm = new Image[3];
    public Image[] m_AttributeShot = new Image[3];
    public Image[] m_AttributeLaser = new Image[3];
    public Image[] m_AttributeModule = new Image[4];

    private List<Image[]> m_Attributes = new List<Image[]>();
    private GameObject[] m_AttributeSlots = new GameObject[5];

    RankingScoreImageDisplay() {
        m_Attributes.Add(m_AttributeSpeed);
        m_Attributes.Add(m_AttributeShotForm);
        m_Attributes.Add(m_AttributeShot);
        m_Attributes.Add(m_AttributeLaser);
        m_Attributes.Add(m_AttributeModule);
    }

    void OnDisable()
    {
        DestroyAllAttributeSlots();
    }

    public void DisplayImages(string text) {
        int totalAttributes;
        DestroyAllAttributeSlots();
        try {
            totalAttributes = int.Parse(text);
        }
        catch (System.FormatException) {
            return;
        }

        bool[] display = { false, true, true, true, true, true, false };
        int check = 0, i = 4;

        while(i >= 0) {
            try {
                if (!display[check++]) {
                    totalAttributes /= 10;
                    continue;
                }
                m_AttributeSlots[i] = new GameObject("Slot ("+(i+1)+")"); // Color, "Speed", "ShotForm", "Shot", "Laser", "Module", Bomb
                
                RectTransform rectTransform = m_AttributeSlots[i].AddComponent<RectTransform>();
                rectTransform.position = new Vector3(i*40f, 0f, 0f);
                rectTransform.sizeDelta = new Vector2(40f, 40f);
                rectTransform.anchorMin = new Vector2(0f, 0.5f);
                rectTransform.anchorMax = new Vector2(0f, 0.5f);
                rectTransform.pivot = new Vector2(0f, 0.5f);
                rectTransform.localScale = new Vector2(1f, 1f);

                Image image = m_AttributeSlots[i].AddComponent<Image>();
                image.sprite = m_Attributes[i][totalAttributes % 10].sprite;
                image.color = Color.white;

                m_AttributeSlots[i].transform.SetParent(this.transform, false);
                totalAttributes /= 10;
                i--;
            }
            catch (System.IndexOutOfRangeException) {
                i--;
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
