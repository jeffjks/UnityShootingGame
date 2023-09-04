using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class BackgroundCamera : MonoBehaviour
{
    public Camera m_BackgroundCamera;
    public Transform m_BackgroundOffsetTransform;

    public static BackgroundCamera Instance { get; private set; }

    private static bool _isRepeating;
    private Vector3 _backgroundCameraDefaultLocalPos;
    private Vector3 _backgroundMoveVector;
    private readonly HashSet<EnemyUnit> _repeatingEnemies = new();

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

    public static void MoveBackgroundCameraOffset(float offsetZ, int millisecond = 0)
    {
        Instance.StartCoroutine(Instance.MoveBackgroundCameraOffsetCoroutine(offsetZ, millisecond));
    }

    public static bool AddRepeatingEnemy(EnemyUnit enemyUnit)
    {
        return Instance._repeatingEnemies.Add(enemyUnit);
    }

    public static bool RemoveRepeatingEnemy(EnemyUnit enemyUnit)
    {
        return Instance._repeatingEnemies.Remove(enemyUnit);
    }

    public static void RepeatBackground(float repeatLength)
    {
        if (repeatLength > 0f)
        {
            _isRepeating = true;
            Instance.StartCoroutine(Instance.RepeatBackgroundCoroutine(repeatLength));
        }
        else
        {
            _isRepeating = false;
        }
    }

    private IEnumerator RepeatBackgroundCoroutine(float repeatLength)
    {
        var pivot = transform.position.z;

        while (_isRepeating)
        {
            if (transform.position.z >= pivot + repeatLength)
            {
                var pos = transform.position;
                pos.z -= repeatLength;
                transform.position = pos;

                foreach (var enemyUnit in _repeatingEnemies)
                {
                    var tempPosition = enemyUnit.transform.position;
                    tempPosition.z -= repeatLength;
                    enemyUnit.transform.position = tempPosition;
                }
            }

            yield return null;
        }
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

    private IEnumerator MoveBackgroundCameraOffsetCoroutine(float offsetZ, int millisecond)
    {
        var frame = millisecond * Application.targetFrameRate / 1000;
        var initPositionZ = m_BackgroundOffsetTransform.localPosition.z;
        var targetPositionZ = initPositionZ + offsetZ;

        for (int i = 0; i < frame; ++i)
        {
            float t_pos_z = AC_Ease.ac_ease[(int)EaseType.InOutQuad].Evaluate((float) (i+1) / frame);
            
            var backgroundOffsetZ = Mathf.Lerp(initPositionZ, targetPositionZ, t_pos_z);
            
            var backgroundOffset = m_BackgroundOffsetTransform.localPosition;
            backgroundOffset.z = backgroundOffsetZ;
            m_BackgroundOffsetTransform.localPosition = backgroundOffset;
            
            yield return new WaitForMillisecondFrames(0);
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
