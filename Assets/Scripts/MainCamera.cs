using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour {
    
    private PlayerManager m_PlayerManager = null;
    private Vector3 m_PlayerPosition;
    private Camera m_Camera;
    private Vector3 m_CameraPosition;
    private float m_CameraMoveRate;
    private float m_CameraMargin;

    void Start()
    {
        m_PlayerManager = PlayerManager.instance_pm;
        m_PlayerPosition = m_PlayerManager.m_Player.transform.position;

        m_CameraMargin = m_PlayerManager.m_CameraMargin;

        m_Camera = GetComponentInChildren<Camera>();
        m_CameraMoveRate = m_CameraMargin / Size.CAMERA_MOVE_LIMIT;

        transform.position = new Vector3(transform.position.x, transform.position.y, Depth.CAMERA);
    }

    void LateUpdate()
    {
        m_PlayerPosition = m_PlayerManager.m_Player.transform.position;
        
        float camera_x = m_PlayerPosition.x * m_CameraMoveRate;
        try {
            camera_x = m_PlayerPosition.x * m_CameraMoveRate;
        }
        catch {
            camera_x = transform.position.x;
        }

        camera_x = Mathf.Clamp(camera_x, - m_CameraMargin, m_CameraMargin);

        m_CameraPosition = new Vector3(camera_x, transform.position.y, Depth.CAMERA);
        transform.position = m_CameraPosition;
    }
}
