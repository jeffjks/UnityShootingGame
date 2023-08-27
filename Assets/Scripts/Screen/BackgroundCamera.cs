using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundCamera : MonoBehaviour
{
    public static Camera Camera;
    
    public static BackgroundCamera Instance { get; private set; }

    private Vector3 _backgroundCameraDefaultLocalPos;
    private Vector3 _backgroundMoveVector;

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        Camera = GetComponent<Camera>();
        _backgroundCameraDefaultLocalPos = transform.localPosition;

        InitCamera();

        SystemManager.Action_OnNextStage += InitCamera;
        SystemManager.Action_OnShowOverview += StopBackground;
    }

    private void Update()
    {
        MoveBackgroundCamera();
    }

    private void MoveBackgroundCamera() {
        transform.position += _backgroundMoveVector / Application.targetFrameRate * Time.timeScale;
    }

    private void InitCamera(bool hasNextStage = true)
    {
        if (!hasNextStage)
        {
            return;
        }
        Instance.transform.localPosition = _backgroundCameraDefaultLocalPos;
    }

    private void StopBackground()
    {
        SetBackgroundSpeed(0f);
    }

    public static Vector3 GetBackgroundVector()
    {
        return Instance._backgroundMoveVector;
    }

    public static void SetBackgroundSpeed(float target, int millisecond = 0) {
        Instance.StartCoroutine(Instance.BackgroundSpeedCoroutine(target, millisecond));
    }

    public static void SetBackgroundSpeed(Vector3 target, int millisecond = 0) { // Overloading
        Instance.StartCoroutine(Instance.BackgroundSpeedCoroutine(target, millisecond));
    }

    public static void MoveBackgroundCamera(bool relative, float position_z, int millisecond = 0) { // Overloading
        Instance.StartCoroutine(Instance.MoveBackgroundCameraCoroutine(relative, position_z, millisecond));
    }

    private IEnumerator BackgroundSpeedCoroutine(float target, int millisecond = 0) {
        int frame = millisecond * Application.targetFrameRate / 1000;
        if (frame == 0) { // 즉시 종료
            _backgroundMoveVector.z = target;
            yield break;
        }

        float init_vector_z = _backgroundMoveVector.z;
        
        for (int i = 0; i < frame; ++i) {
            _backgroundMoveVector.z = init_vector_z + (target - init_vector_z)*(i+1) / frame;
            yield return new WaitForFrames(0);
        }
    }

    private IEnumerator BackgroundSpeedCoroutine(Vector3 target_vector, int millisecond = 0) { // Overloading
        int frame = millisecond * Application.targetFrameRate / 1000;
        if (frame == 0) { // 즉시 종료
            _backgroundMoveVector = target_vector;
            yield break;
        }

        Vector3 init_vector = _backgroundMoveVector;

        for (int i = 0; i < frame; ++i) {
            _backgroundMoveVector = init_vector + (target_vector - init_vector)*(i+1) / frame;
            yield return new WaitForFrames(0);
        }
    }

    private IEnumerator MoveBackgroundCameraCoroutine(bool relative, float position_z, int millisecond) {
        int frame = millisecond * Application.targetFrameRate / 1000;
        float init_position_z = Instance.transform.position.z;
        float target_position_z;

        if (relative) {
            target_position_z = init_position_z + position_z;
        }
        else {
            target_position_z = position_z;
        }

        for (int i = 0; i < frame; ++i) {
            float t_pos_z = AC_Ease.ac_ease[(int)EaseType.InOutQuad].Evaluate((float) (i+1) / frame);
            
            position_z = Mathf.Lerp(init_position_z, target_position_z, t_pos_z);
            Vector3 temp = transform.position;
            temp.z = position_z;
            transform.position = temp;
            yield return new WaitForMillisecondFrames(0);
        }
    }

    public static Vector2 GetScreenPosition(Vector3 pos)
    {
        var mainCameraX = MainCamera.Instance.GetCameraScreenPosition().x;
        Vector3 screenPos = Camera.WorldToScreenPoint(pos);
        Vector2 newPos = new Vector2(
            screenPos[0]*Size.MAIN_CAMERA_WIDTH/Screen.width - Size.MAIN_CAMERA_WIDTH/2 + mainCameraX,
            screenPos[1]*Size.MAIN_CAMERA_HEIGHT/Screen.height - Size.MAIN_CAMERA_HEIGHT
        );
        return newPos;
    }
}
