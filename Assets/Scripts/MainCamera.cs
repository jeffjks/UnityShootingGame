using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour {
    
    public static Camera Camera;
    
    private PlayerManager m_PlayerManager = null;
    private Vector2Int m_PlayerPosition;
    private Vector2 _shakePosition;
    private float m_CameraMoveRate, m_CameraMargin;
    private const float POSITION_Y = -Size.GAME_HEIGHT/2;
    
    private static IEnumerator _shakeCamera;
    
    public static MainCamera Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        Camera = GetComponent<Camera>();
        
        DontDestroyOnLoad(gameObject);

        InitCamera();

        SystemManager.instance_sm.Action_OnNextStage += InitCamera;
    }

    void Start()
    {
        m_PlayerManager = PlayerManager.instance_pm;
        //m_PlayerPosition = m_PlayerManager.m_PlayerController.m_Position;

        m_CameraMargin = m_PlayerManager.m_CameraMargin;
        m_CameraMoveRate = m_CameraMargin / Size.CAMERA_MOVE_LIMIT;
    }

    void LateUpdate()
    {
        m_PlayerPosition = m_PlayerManager.m_PlayerController.m_PositionInt2D;
        
        float camera_x;
        try {
            camera_x = ((float) m_PlayerPosition.x) / 256  * m_CameraMoveRate;
        }
        catch {
            camera_x = transform.position.x;
        }

        camera_x = Mathf.Clamp(camera_x, - m_CameraMargin, m_CameraMargin);
        
        transform.position = new Vector3(camera_x, POSITION_Y, Depth.CAMERA) + (Vector3) _shakePosition;
    }

    private void InitCamera() {
        transform.position = new Vector3(0f, POSITION_Y, Depth.CAMERA);
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
