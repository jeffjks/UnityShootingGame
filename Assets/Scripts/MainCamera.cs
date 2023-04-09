using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour {
    
    private PlayerManager m_PlayerManager = null;
    private Vector2Int m_PlayerPosition;
    private Vector2 m_ShakePosition;
    private const float POSITION_Y = -8f;
    private float m_CameraMoveRate, m_CameraMargin;
    private IEnumerator m_ShakeCamera;

    void Start()
    {
        m_PlayerManager = PlayerManager.instance_pm;
        //m_PlayerPosition = m_PlayerManager.m_PlayerController.m_Position;

        m_CameraMargin = m_PlayerManager.m_CameraMargin;
        m_CameraMoveRate = m_CameraMargin / Size.CAMERA_MOVE_LIMIT;
        
        InitMainCamera();
    }

    void LateUpdate()
    {
        m_PlayerPosition = m_PlayerManager.m_PlayerController.m_Position;
        
        float camera_x;
        try {
            camera_x = ((float) m_PlayerPosition.x) / 256  * m_CameraMoveRate;
        }
        catch {
            camera_x = transform.position.x;
        }

        camera_x = Mathf.Clamp(camera_x, - m_CameraMargin, m_CameraMargin);
        
        transform.position = new Vector3(camera_x, POSITION_Y, Depth.CAMERA) + (Vector3) m_ShakePosition;
    }

    public void InitMainCamera() {
        transform.position = new Vector3(0f, POSITION_Y, Depth.CAMERA);
    }

    public void ShakeCamera(float duration) {
        if (m_ShakeCamera != null)
            StopCoroutine(m_ShakeCamera);
        m_ShakeCamera = ShakeCameraProcess(duration);
        StartCoroutine(m_ShakeCamera);
    }

    private IEnumerator ShakeCameraProcess(float duration) {
        float timer = 0, radius, radius_init;
        radius = Mathf.Clamp01(duration) * 1.5f;
        radius_init = radius;

        while(timer < duration) {
            m_ShakePosition = Random.insideUnitCircle * radius;
    
            timer += Time.deltaTime;
            radius = Mathf.Lerp(radius_init, 0f, timer / duration);
            yield return null;
        }
        m_ShakePosition = Vector2.zero;
        yield break;
    }
}
