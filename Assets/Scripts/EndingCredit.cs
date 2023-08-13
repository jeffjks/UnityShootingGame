using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.InputSystem;

public class EndingCredit : MonoBehaviour
{
    public MenuHandler m_RegisterLocalRankingMenuHandler;
    public TextMeshProUGUI m_CreditText;
    public RectTransform m_ParentRectTransform;

    private Dictionary<Language, string> _creditJsonData = new();

    private const float DEFAULT_SCROLL_SPEED = 0.8f;
    private const float FAST_SCROLL_SPEED = 6.4f;
    private float _currentScrollSpeed;
    private bool m_Quitting = false;
    private bool _isFirePress;
    private InGameInputController _inGameInputController;

    private void Awake()
    {
        _creditJsonData = Utility.LoadDataFile<Dictionary<Language, string>>(Application.dataPath, "resources1").jsonData;
        m_CreditText.SetText(_creditJsonData[GameSetting.CurrentLanguage]);
        
        _inGameInputController = InGameInputController.Instance;
        _inGameInputController.Action_OnFireInput += OnFireInvoked;
        _inGameInputController.Action_OnBombInput += QuitEndingCredit;
        _inGameInputController.Action_OnEscapeInput += QuitEndingCredit;
    }

    private void OnEnable()
    {
        AudioService.LoadMusics("Main");
        AudioService.PlayMusic("Ending");
    }

    private void OnFireInvoked(InputValue inputValue)
    {
        _isFirePress = inputValue.isPressed;
    }

    private void Update ()
    {
        if (transform.localPosition.y >= m_CreditText.flexibleHeight + m_ParentRectTransform.rect.height / 2)
        {
            _currentScrollSpeed = 0f;
            QuitEndingCredit(3f);
            return;
        }
        
        _currentScrollSpeed = _isFirePress ? FAST_SCROLL_SPEED : DEFAULT_SCROLL_SPEED;

        Vector3 newLocalPos = transform.localPosition;
        newLocalPos.y += _currentScrollSpeed*Time.deltaTime;
        transform.localPosition = newLocalPos;
    }

    private void QuitEndingCredit()
    {
        QuitEndingCredit(0f);
    }

    private void QuitEndingCredit(float delay)
    {
        if (m_Quitting)
            return;
        m_Quitting = true;
        StartCoroutine(QuitEnding(delay));
    }

    private IEnumerator QuitEnding(float delay) {
        yield return new WaitForSeconds(delay);
        Debug.Log("Quitting");
        
        FadeScreenService.ScreenFadeOut(2f);
        AudioService.FadeOutMusic(2f);
        yield return new WaitForSeconds(3f);
        
        FadeScreenService.ScreenFadeIn(0f);
        AudioService.StopMusic();
        m_RegisterLocalRankingMenuHandler.gameObject.SetActive(true);
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