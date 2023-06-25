using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EndingCredit : MonoBehaviour
{
    public GameObject m_RegisterRecordMenu;
    public SpriteRenderer m_SpriteRenderer;
    public Text[] m_EndingScroll = new Text[2];
    [Space(10)]
    public string m_Date;
    [TextArea(10, 100)]
    public string[] m_EndingText = new string[2];

    private float m_ScrollSpeed, m_DefaultScrollSpeed = 0.8f;
    private int m_Language;
    private float m_Scale;
    private bool m_Quitting = false;
    
    private GameManager m_GameManager = null;

    void Start()
    {
        //m_SystemManager = SystemManager.instance_sm;
        m_GameManager = GameManager.instance_gm;
        //transform.position = new Vector3(transform.position.x, transform.position.y, Depth.CAMERA);
        //m_SystemManager.SetCurrentStage(5);
        //m_SystemManager.m_PlayState = 3;

        for (int i = 0; i < m_EndingScroll.Length; i++) {
            m_EndingScroll[i].text = m_EndingText[i] + "\nver " + Application.version + "\n" + m_Date;
        }

        m_Language = (int) GameSetting.m_Language;
        
        AudioService.LoadMusics("Main");
        AudioService.PlayMusic("Ending");
        
        m_Scale = m_EndingScroll[m_Language].rectTransform.localScale.x;
        m_EndingScroll[m_Language].gameObject.SetActive(true);
        m_ScrollSpeed = m_DefaultScrollSpeed;
    }
    
    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (!m_Quitting)
                StartCoroutine(QuitEnding());
        }
        
        if (m_EndingScroll[m_Language].rectTransform.anchoredPosition.y < Size.MAIN_CAMERA_HEIGHT*0.5f + m_EndingScroll[m_Language].preferredHeight*m_Scale) {
            if (Input.GetButton("Fire1")) {
                m_ScrollSpeed = m_DefaultScrollSpeed*8f;
            }
            else {
                m_ScrollSpeed = m_DefaultScrollSpeed;
            }
        }
        else {
            m_ScrollSpeed = 0f;
            if (!m_Quitting)
                StartCoroutine(QuitEnding());
        }
        
        Vector3 pos = m_EndingScroll[m_Language].rectTransform.anchoredPosition;
        m_EndingScroll[m_Language].rectTransform.anchoredPosition = new Vector3(pos.x, pos.y + m_ScrollSpeed*Time.deltaTime, pos.z);
    }

    private IEnumerator QuitEnding() {
        m_Quitting = true;
        m_SpriteRenderer.color = new Color(0f, 0f, 0f, 0f);
        Debug.Log("Quitting");

        m_SpriteRenderer.DOFade(1f, 2f);
        AudioService.FadeOutMusic(2f);
        yield return new WaitForSeconds(3f);
        
        m_SpriteRenderer.color = new Color(0f, 0f, 0f, 0f);
        AudioService.StopMusic();
        m_RegisterRecordMenu.SetActive(true);
        gameObject.SetActive(false);
    }
}

/*
<size=56><b>Producer</b></size>
강낭땅콩 (Bean Peanut)
jeff_jks@naver.com
https://blog.naver.com/jeff_jks

Made with Unity 2019.1.8f1



<size=56><b>Used Assets (Unity Asset Store)</b></size>

<size=56><b>Models</b></size>
Ballistic Turret
Chainlink Fences
EnergyShieldEffect
Futuristic Aircrafts
Free SF Fighter
Hyper Falcon Modular Spaceship
Low poly combat drone
Military Helicopter
Modular Space Platform Kit
PBR RPG/FPS Game Assets (Industrial Set v1.0)
Powerup Crate
Rock and Boulders 2
RPG/FPS Game Assets for PCbile (Industrial Set v2.0)
Scifi RTS Series Mega Pack I
Scifi RTS Series Mega Pack II
Simple Gems Ultimate Animated Customizable Pack
Simple Sci-Fi Turret
Spaceship+Fighter+Set of...+
Space Missiles
Spaceships Fleet III Mega Pack I
UXR Tetrahedron (by ENEA LE FONS, Sketchfab)

<size=56><b>Images / Textures</b></size>
50+ Progress Bars [Pack 4] - DANGEROUS PROGRESS
https://www.freepik.com/
Sci Fi UI Pack
Sci-Fi Skill Icon Pack
Sci-Fi Texture Pack

<size=56><b>Effects</b></size>
Effect textures and prefabs
Quality Explosion SpriteKit
Shockwave Effects
Waf FX

<size=56><b>Sounds / Musics</b></size>
CAVE Interactive CO. LTD (Ketsui Arrange)
Fantasy Menu SFX

<size=56><b>Terrains</b></size>
City Highways Construction Kit
City Street Props
Grass And Flowers Pack 1
Race Tracks
Seamless Textures
Standard Asset
Windridge City
World Space Trees (FREE)

<size=56><b>Other</b></size>
Asset Usage Detector
Auto Letterbox
DOTween








<size=56><b>Dead Planet 2</b></size>
================================================*/


/*================================================
<size=56><b>제작</b></size>
강낭땅콩 (Bean Peanut)
jeff_jks@naver.com
https://blog.naver.com/jeff_jks

Made with Unity 2019.1.8f1



<size=56><b>사용한 에셋 (유니티 에셋 스토어)</b></size>

<size=56><b>모델링</b></size>
Ballistic Turret
Chainlink Fences
EnergyShieldEffect
Futuristic Aircrafts
Free SF Fighter
Hyper Falcon Modular Spaceship
Low poly combat drone
Military Helicopter
Modular Space Platform Kit
PBR RPG/FPS Game Assets (Industrial Set v1.0)
Powerup Crate
Rock and Boulders 2
RPG/FPS Game Assets for PCbile (Industrial Set v2.0)
Scifi RTS Series Mega Pack I
Scifi RTS Series Mega Pack II
Simple Gems Ultimate Animated Customizable Pack
Simple Sci-Fi Turret
Spaceship+Fighter+Set of...+
Space Missiles
Spaceships Fleet III Mega Pack I
UXR Tetrahedron (by ENEA LE FONS, Sketchfab)

<size=56><b>이미지 / 텍스쳐</b></size>
50+ Progress Bars [Pack 4] - DANGEROUS PROGRESS
https://www.freepik.com/
Sci Fi UI Pack
Sci-Fi Skill Icon Pack
Sci-Fi Texture Pack

<size=56><b>Effects</b></size>
Effect textures and prefabs
Ground crack
Quality Explosion SpriteKit
Shockwave Effects
Waf FX

<size=56><b>효과음 / 음악</b></size>
CAVE Interactive CO. LTD (Ketsui Arrange)
Fantasy Menu SFX

<size=56><b>지형</b></size>
City Highways Construction Kit
City Street Props
Grass And Flowers Pack 1
Race Tracks
Seamless Textures
Standard Asset
Windridge City
World Space Trees (FREE)

<size=56><b>기타</b></size>
Asset Usage Detector
Auto Letterbox
DOTween








<size=56><b>Dead Planet 2</b></size>
*/