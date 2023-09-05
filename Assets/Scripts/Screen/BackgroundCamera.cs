using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class BackgroundCamera : MonoBehaviour
{
    public Camera m_BackgroundCamera;
    public Transform m_BackgroundOffsetTransform;

    public static BackgroundCamera Instance { get; private set; }

    private static bool _isRepeatingCamera;
    private static bool _isRepeatingBackground;
    private Vector3 _backgroundCameraDefaultLocalPos;
    private Vector3 _backgroundCameraMoveVector;
    private readonly HashSet<Transform> _repeatingBackgrounds = new();

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        _backgroundCameraDefaultLocalPos = transform.localPosition;

        InitCamera();

        SystemManager.Action_OnNextStage += InitCamera;
        SystemManager.Action_OnShowOverview += StopBackgroundCamera;
    }

    private void Update()
    {
        MoveBackgroundCamera();
    }

    private void MoveBackgroundCamera() {
        transform.position += _backgroundCameraMoveVector / Application.targetFrameRate * Time.timeScale;
    }

    private void InitCamera(bool hasNextStage = true)
    {
        if (!hasNextStage)
        {
            return;
        }
        StopAllCoroutines();
        _repeatingBackgrounds.Clear();
        _isRepeatingBackground = false;
        _isRepeatingCamera = false;
        Instance.transform.localPosition = _backgroundCameraDefaultLocalPos;
        Instance.m_BackgroundOffsetTransform.localPosition = Vector3.zero;
    }

    private void StopBackgroundCamera()
    {
        SetBackgroundCameraSpeed(0f);
    }

    public static Vector3 GetBackgroundCameraMoveVector()
    {
        return Instance._backgroundCameraMoveVector;
    }

    public static void SetBackgroundCameraSpeed(float target, int millisecond = 0) {
        Instance.StartCoroutine(Instance.BackgroundSpeedCoroutine(target, millisecond));
    }

    public static void SetBackgroundCameraSpeed(Vector3 target, int millisecond = 0) { // Overloading
        Instance.StartCoroutine(Instance.BackgroundSpeedCoroutine(target, millisecond));
    }

    public static void MoveBackgroundCameraOffset(bool relative, float positionZ, int millisecond = 0)
    {
        Instance.StartCoroutine(Instance.MoveBackgroundCameraOffsetCoroutine(relative, positionZ, millisecond));
    }

    private IEnumerator MoveBackgroundCameraOffsetCoroutine(bool relative, float positionZ, int millisecond)
    {
        var frame = millisecond * Application.targetFrameRate / 1000;
        var initPositionZ = Instance.m_BackgroundOffsetTransform.localPosition.z;
        var targetPositionZ = relative ? initPositionZ + positionZ : positionZ;
        
        for (var i = 0; i < frame; ++i)
        {
            var t_pos_z = AC_Ease.ac_ease[(int)EaseType.InOutQuad].Evaluate((float) (i+1) / frame);
            
            positionZ = Mathf.Lerp(initPositionZ, targetPositionZ, t_pos_z);
            Vector3 temp = m_BackgroundOffsetTransform.localPosition;
            temp.z = positionZ;
            m_BackgroundOffsetTransform.localPosition = temp;
            yield return null;
        }
    }

    public static void RepeatBackground(float repeatLength, float speed)
    {
        if (repeatLength > 0f)
        {
            _isRepeatingBackground = true;
            Instance.StartCoroutine(Instance.RepeatBackgroundCoroutine(repeatLength, speed));
        }
        else
        {
            _isRepeatingBackground = false;
        }
    }

    private IEnumerator RepeatBackgroundCoroutine(float repeatLength, float speed)
    {
        var current = 0f;

        while (_isRepeatingBackground)
        {
            var prevCurrent = current;
            current = Mathf.Repeat(current + speed / Application.targetFrameRate * Time.timeScale, repeatLength);
            
            foreach (var background in _repeatingBackgrounds)
            {
                var tempPosition = background.position;
                tempPosition.z -= current - prevCurrent;
                background.position = tempPosition;
            }

            yield return null;
        }
    }

    public static bool AddRepeatingBackground(Transform backgroundTransform)
    {
        return Instance._repeatingBackgrounds.Add(backgroundTransform);
    }

    public static bool RemoveRepeatingBackground(Transform backgroundTransform)
    {
        return Instance._repeatingBackgrounds.Remove(backgroundTransform);
    }

    public static void RepeatBackgroundCamera(float repeatLength)
    {
        if (repeatLength > 0f)
        {
            _isRepeatingCamera = true;
            Instance.StartCoroutine(Instance.RepeatBackgroundCameraCoroutine(repeatLength));
        }
        else
        {
            _isRepeatingCamera = false;
        }
    }

    private IEnumerator RepeatBackgroundCameraCoroutine(float repeatLength)
    {
        var pivot = transform.position.z;

        while (_isRepeatingCamera)
        {
            if (transform.position.z >= pivot + repeatLength)
            {
                var pos = transform.position;
                pos.z -= repeatLength;
                transform.position = pos;
            }

            yield return null;
        }
    }

    private IEnumerator BackgroundSpeedCoroutine(float target, int millisecond = 0) {
        int frame = millisecond * Application.targetFrameRate / 1000;
        if (frame == 0) { // 즉시 종료
            _backgroundCameraMoveVector.z = target;
            yield break;
        }

        float init_vector_z = _backgroundCameraMoveVector.z;
        
        for (int i = 0; i < frame; ++i) {
            _backgroundCameraMoveVector.z = init_vector_z + (target - init_vector_z)*(i+1) / frame;
            yield return new WaitForFrames(0);
        }
    }

    private IEnumerator BackgroundSpeedCoroutine(Vector3 target_vector, int millisecond = 0) { // Overloading
        int frame = millisecond * Application.targetFrameRate / 1000;
        if (frame == 0) { // 즉시 종료
            _backgroundCameraMoveVector = target_vector;
            yield break;
        }

        Vector3 init_vector = _backgroundCameraMoveVector;

        for (int i = 0; i < frame; ++i) {
            _backgroundCameraMoveVector = init_vector + (target_vector - init_vector)*(i+1) / frame;
            yield return new WaitForFrames(0);
        }
    }

    public static Vector2 GetScreenPosition(Vector3 pos)
    {
        var viewportPosition = Instance.m_BackgroundCamera.WorldToViewportPoint(pos);
        var mainCameraX = MainCamera.Instance.GetCameraScreenPosition().x;
        var screenPosition = new Vector2(
            (viewportPosition.x - 0.5f) * Size.MAIN_CAMERA_WIDTH + mainCameraX,
            (viewportPosition.y - 1f) * Size.MAIN_CAMERA_HEIGHT
        );
        return screenPosition;
    }
}
