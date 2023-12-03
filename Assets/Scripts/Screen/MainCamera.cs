using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [SerializeField] private Transform m_CameraShakingTransform;
    public Camera Camera;
    
    private bool _destroySingleton;
    private Vector2 _shakingPosition;
    private const float CAMERA_MOVE_RATE = CAMERA_MARGIN / Size.CAMERA_MOVE_LIMIT;
    private const float CAMERA_MARGIN = (Size.GAME_WIDTH - Size.MAIN_CAMERA_WIDTH) / 2; // 1.555
    
    private static IEnumerator _shakeCamera;
    
    public static MainCamera Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            _destroySingleton = true;
            Destroy(gameObject);
            return;
        }
        Instance = this;

        InitCamera();

        SystemManager.Action_OnNextStage += InitCamera;
    }

    private void OnDestroy()
    {
        if (_destroySingleton)
            return;
        SystemManager.Action_OnNextStage -= InitCamera;
    }

    private void LateUpdate()
    {
        var cameraPosX = PlayerManager.GetPlayerPosition().x * CAMERA_MOVE_RATE;

        cameraPosX = Mathf.Clamp(cameraPosX, - CAMERA_MARGIN, CAMERA_MARGIN);
        
        transform.position = new Vector3(cameraPosX, -Size.GAME_HEIGHT/2, Depth.CAMERA);
        m_CameraShakingTransform.transform.localPosition = new Vector3(_shakingPosition.x, _shakingPosition.y, 0f);
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

    private static IEnumerator ShakeCameraProcess(float duration)
    {
        var timer = 0f;
        var radius = Mathf.Clamp01(duration) * 1.5f;
        var radiusInit = radius;

        while(timer < duration) {
            if (PauseManager.IsGamePaused)
                continue;
            Instance._shakingPosition = Utility.GetRandomPositionInsideCircle(radius);
    
            timer += Time.deltaTime;
            radius = Mathf.Lerp(radiusInit, 0f, timer / duration);
            yield return new WaitForFrames(1);
        }
        Instance._shakingPosition = Vector2.zero;
    }

    public Vector3 GetCameraScreenPosition()
    {
        return transform.position;
    }
}
