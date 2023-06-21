using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour {
    
    public static Camera Camera;
    
    private Vector2 _shakePosition;
    private float m_CameraMoveRate;
    private const float CAMERA_MARGIN = (Size.GAME_WIDTH - Size.MAIN_CAMERA_WIDTH) / 2; // 1.555
    
    private static IEnumerator _shakeCamera;
    
    public static MainCamera Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        Camera = GetComponent<Camera>();

        InitCamera();

        SystemManager.Action_OnNextStage += InitCamera;
    }

    void Start()
    {
        //m_PlayerPosition = m_PlayerManager.m_PlayerController.m_Position;
        
        m_CameraMoveRate = CAMERA_MARGIN / Size.CAMERA_MOVE_LIMIT;
    }

    void LateUpdate()
    {
        var camera_x = PlayerManager.IsPlayerAlive ? PlayerManager.GetPlayerPosition().x : transform.position.x;

        camera_x = Mathf.Clamp(camera_x, - CAMERA_MARGIN, CAMERA_MARGIN);
        
        transform.position = new Vector3(camera_x, -Size.GAME_HEIGHT/2, Depth.CAMERA) + (Vector3) _shakePosition;
    }

    private void InitCamera(bool hasNextStage = true)
    {
        if (!hasNextStage)
        {
            return;
        }
        transform.position = new Vector3(0f, -Size.GAME_HEIGHT/2, Depth.CAMERA);
    }

    public static void ShakeCamera(float duration) {
        if (_shakeCamera != null)
            Instance.StopCoroutine(_shakeCamera);
        _shakeCamera = ShakeCameraProcess(duration);
        Instance.StartCoroutine(_shakeCamera);
    }

    private static IEnumerator ShakeCameraProcess(float duration) {
        float timer = 0, radius, radius_init;
        radius = Mathf.Clamp01(duration) * 1.5f;
        radius_init = radius;

        while(timer < duration) {
            Instance._shakePosition = Random.insideUnitCircle * radius;
    
            timer += Time.deltaTime;
            radius = Mathf.Lerp(radius_init, 0f, timer / duration);
            yield return null;
        }
        Instance._shakePosition = Vector2.zero;
    }
}
