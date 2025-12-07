using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class TriggerBody : MonoBehaviour
{
    public enum BodyType
    {
        Circle,
        Box,
        Polygon,
    }

    public TriggerBodyType m_TriggerBodyType;
    public BodyType m_BodyType;

    [DrawIf("m_BodyType", BodyType.Circle, ComparisonType.Equals)]
    public BodyCircle m_BodyCircle;

    [DrawIf("m_BodyType", BodyType.Box, ComparisonType.Equals)]
    public BodyBox m_BodyBox;
    
    [DrawIf("m_BodyType", BodyType.Polygon, ComparisonType.Equals)]
    public BodyPolygon m_BodyPolygon;
    
    public UnityAction<TriggerBody> m_OnTriggerBodyEnter;
    public UnityAction<TriggerBody> m_OnTriggerBodyExit;
    public UnityAction<TriggerBody> m_OnTriggerBodyStay;

    public BodyType BodyTypeForComparison { get; private set; }

    public HashSet<TriggerBody> TriggerBodySet { get; } = new();

    private BodyCircle _cachedTransformedBodyCircle;
    private bool _isTransformedBodyCircleDirty = true;
    private BodyPolygon _cachedTransformedBodyPolygon;
    private bool _isTransformedBodyPolygonDirty = true;

    public BodyCircle TransformedBodyCircle
    {
        get
        {
            if (_isTransformedBodyCircleDirty)
            {
                _cachedTransformedBodyCircle = GetTransformedBody(m_BodyCircle);
                _isTransformedBodyCircleDirty = false;
            }
            return _cachedTransformedBodyCircle;
        }
        private set
        {
            _cachedTransformedBodyCircle = value;
        }
    }
    public BodyPolygon TransformedBodyPolygon
    {
        get
        {
            if (_isTransformedBodyPolygonDirty)
            {
                _cachedTransformedBodyPolygon = GetTransformedBody(m_BodyPolygon);
                _isTransformedBodyPolygonDirty = false;
            }
            return _cachedTransformedBodyPolygon;
        }
        private set
        {
            _cachedTransformedBodyPolygon = value;
        }
    }

#if UNITY_EDITOR
    public List<TriggerBody> m_DebugTriggerBody = new();
    
    private void Update()
    {
        m_DebugTriggerBody = TriggerBodySet.ToList();
    }
    
    private void OnDrawGizmosSelected()
    {
        if (enabled == false)
            return;
        
        InitCachedTransformedBody();
        
        switch (m_BodyType)
        {
            case BodyType.Circle:
                DrawCircleGizmos();
                break;
            case BodyType.Box:
                m_BodyPolygon = new BodyPolygon(m_BodyBox);
                DrawPolygonGizmos();
                break;
            case BodyType.Polygon:
                DrawPolygonGizmos();
                break;
        }
    }

    private void DrawCircleGizmos()
    {
        Handles.color = Color.green;
        var gizmoCircle = TransformedBodyCircle;
        Handles.DrawWireDisc(gizmoCircle.m_BodyCenter, Vector3.forward, gizmoCircle.m_BodyRadius);
    }

    private void DrawPolygonGizmos()
    {
        var isConcaveList = new List<bool>();
        var transformedBodyPolygon = TransformedBodyPolygon;
        
        foreach (var bodyPolygonUnit in transformedBodyPolygon.m_BodyPolygonUnits)
        {
            var count = bodyPolygonUnit.m_BodyPoints.Count;

            if (count <= 3)
            {
                isConcaveList.Add(false);
                continue;
            }

            var currentConcave = false;
            var prevCross = 0f;
            
            for (var i = 0; i < count; ++i)
            {
                var prevPoint = bodyPolygonUnit.m_BodyPoints[((i - 1) % count + count) % count];
                var currentPoint = bodyPolygonUnit.m_BodyPoints[i];
                var nextPoint = bodyPolygonUnit.m_BodyPoints[(i + 1) % count];
                var prevEdge = currentPoint - prevPoint;
                var nextEdge = nextPoint - currentPoint;
            
                var currentCross = Vector3.Cross(prevEdge, nextEdge);
                
                if (prevCross != 0f && currentCross.z * prevCross < 0)
                {
                    currentConcave = true;
                    break;
                }

                prevCross = currentCross.z;
            }
            isConcaveList.Add(currentConcave);
        }

        var index = 0;
        foreach (var bodyPolygonUnit in transformedBodyPolygon.m_BodyPolygonUnits)
        {
            Gizmos.color = isConcaveList[index] ? Color.red : Color.green;
            index++;
            
            var count = bodyPolygonUnit.m_BodyPoints.Count;
            for (var i = 0; i < count; ++i)
            {
                var currentPoint = bodyPolygonUnit.m_BodyPoints[i];
                var nextPoint = bodyPolygonUnit.m_BodyPoints[(i + 1) % count];
                Gizmos.DrawLine(currentPoint, nextPoint);
            }
        }
    }
#endif

    private void Awake()
    {
        if (m_BodyType == BodyType.Box)
        {
            m_BodyPolygon = new BodyPolygon(m_BodyBox);
            BodyTypeForComparison = BodyType.Polygon;
        }
        else
        {
            BodyTypeForComparison = m_BodyType;
        }

        InitCachedTransformedBody();
    }

    private void OnEnable()
    {
        SimulationManager.AddTriggerBody(this);
    }
    
    private void OnDisable()
    {
        SimulationManager.RemoveTriggerBody(this);

        foreach (var triggerBody in TriggerBodySet)
        {
            triggerBody.TriggerBodySet.Remove(this);
            //Debug.LogError($"{gameObject.name} triggerBodySet removed {isRemoved}: {triggerBody}");
        }
        TriggerBodySet.Clear();
#if UNITY_EDITOR
        m_DebugTriggerBody.Clear();
#endif
    }

    private void InitCachedTransformedBody()
    {
        _isTransformedBodyCircleDirty = true;
        _isTransformedBodyPolygonDirty = true;

        switch (m_BodyType)
        {
            case BodyType.Circle:
                _cachedTransformedBodyCircle = new BodyCircle();
                break;
            case BodyType.Polygon:
            case BodyType.Box:
                _cachedTransformedBodyPolygon = new BodyPolygon(m_BodyPolygon);
                break;
        }
    }

    private void LateUpdate()
    {
        _isTransformedBodyCircleDirty = true;
        _isTransformedBodyPolygonDirty = true;
    }

    private BodyCircle GetTransformedBody(BodyCircle bodyCircle)
    {
        var point = TransformPoint(bodyCircle.m_BodyCenter);
        _cachedTransformedBodyCircle.m_BodyCenter = point;
        
        var radius = m_BodyCircle.m_BodyRadius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
        _cachedTransformedBodyCircle.m_BodyRadius = radius;
        return _cachedTransformedBodyCircle;
    }

    private BodyPolygon GetTransformedBody(BodyPolygon bodyPolygon)
    {
        for (var i = 0; i < bodyPolygon.m_BodyPolygonUnits.Count; ++i)
        {
            for (var j = 0; j < bodyPolygon.m_BodyPolygonUnits[i].m_BodyPoints.Count; ++j)
            {
                var transformedPoint = TransformPoint(bodyPolygon.m_BodyPolygonUnits[i].m_BodyPoints[j]);
                _cachedTransformedBodyPolygon.m_BodyPolygonUnits[i].m_BodyPoints[j] = transformedPoint;
            }
        }
        
        return _cachedTransformedBodyPolygon;
    }

    private Vector2 TransformPoint(Vector2 point)
    {
        point *= transform.lossyScale;
        var rotationMatrix = Matrix4x4.Rotate(transform.rotation); 
        point = rotationMatrix.MultiplyPoint(point);
        point += (Vector2)transform.position;
        return point;
    }

    public void OnTriggerBodyCollision(TriggerBody other, bool result)
    {
        if (isActiveAndEnabled == false || other.isActiveAndEnabled == false)
            return;
        if (result)
        {
            var isAdded = TriggerBodySet.Add(other);
            if (isAdded == false)
                return;
            other.TriggerBodySet.Add(this);
            m_OnTriggerBodyEnter?.Invoke(other);
            //Debug.Log($"{transform.parent.gameObject.name} OnTriggerBodyCollisionEnter: {other.transform.parent.gameObject.name}");
        }
        else
        {
            var isRemoved = TriggerBodySet.Remove(other);
            if (isRemoved == false)
                return;
            other.TriggerBodySet.Remove(this);
            m_OnTriggerBodyExit?.Invoke(other);
            //Debug.Log($"{transform.parent.gameObject.name} OnTriggerBodyCollisionExit: {other.transform.parent.gameObject.name}");
        }
    }

    public void OnTriggerBodyCollisionStay()
    {
        foreach (var triggerBody in TriggerBodySet)
        {
            m_OnTriggerBodyStay?.Invoke(triggerBody);
            //Debug.Log($"{this} OnTriggerBodyCollisionStay: {triggerBody}");
        }
    }

    public void SetCircleSize(float radius)
    {
        if (m_BodyType != BodyType.Circle)
        {
            Debug.LogError($"Trying to set body box size to wrong body type!");
            return;
        }

        m_BodyCircle.m_BodyRadius = radius;
    }

    public void SetBoxSize(Vector2 bodySize)
    {
        if (m_BodyType != BodyType.Box)
        {
            Debug.LogError($"Trying to set body box size to wrong body type!");
            return;
        }
        m_BodyBox.m_BodySize = bodySize;
        var bodyCenter = m_BodyBox.m_BodyCenter;
        var bodyPoints = m_BodyPolygon.m_BodyPolygonUnits[0].m_BodyPoints;
        
        var halfWidth = bodySize.x / 2f;
        var halfHeight = bodySize.y / 2f;
        bodyPoints[0] = new Vector2(bodyCenter.x - halfWidth, bodyCenter.y - halfHeight);
        bodyPoints[1] = new Vector2(bodyCenter.x + halfWidth, bodyCenter.y - halfHeight);
        bodyPoints[2] = new Vector2(bodyCenter.x + halfWidth, bodyCenter.y + halfHeight);
        bodyPoints[3] = new Vector2(bodyCenter.x - halfWidth, bodyCenter.y + halfHeight);
    }

    public void SetBoxSize(Vector2 bodyCenter, Vector2 bodySize)
    {
        if (m_BodyType != BodyType.Box)
        {
            Debug.LogError($"Trying to set body box size to wrong body type!");
            return;
        }
        m_BodyBox.m_BodySize = bodySize;
        m_BodyBox.m_BodyCenter = bodyCenter;
        var bodyPoints = m_BodyPolygon.m_BodyPolygonUnits[0].m_BodyPoints;
        
        var halfWidth = bodySize.x / 2f;
        var halfHeight = bodySize.y / 2f;
        bodyPoints[0] = new Vector2(bodyCenter.x - halfWidth, bodyCenter.y - halfHeight);
        bodyPoints[1] = new Vector2(bodyCenter.x + halfWidth, bodyCenter.y - halfHeight);
        bodyPoints[2] = new Vector2(bodyCenter.x + halfWidth, bodyCenter.y + halfHeight);
        bodyPoints[3] = new Vector2(bodyCenter.x - halfWidth, bodyCenter.y + halfHeight);
    }
}

[Serializable]
public class BodyCircle
{
    public Vector2 m_BodyCenter;
    public float m_BodyRadius;
    
    public BodyCircle() {}

    public BodyCircle(Vector2 bodyCenter, float bodyRadius)
    {
        m_BodyCenter = bodyCenter;
        m_BodyRadius = bodyRadius;
    }

    public BodyCircle(BodyCircle bodyCircle)
    {
        m_BodyCenter = bodyCircle.m_BodyCenter;
        m_BodyRadius = bodyCircle.m_BodyRadius;
    }
}

[Serializable]
public class BodyBox
{
    public Vector2 m_BodyCenter;
    public Vector2 m_BodySize;
    
    public BodyBox() {}

    public BodyBox(Vector2 bodyCenter, Vector2 bodySize)
    {
        m_BodyCenter = bodyCenter;
        m_BodySize = bodySize;
    }

    public BodyBox(BodyBox bodyBox)
    {
        m_BodyCenter = bodyBox.m_BodyCenter;
        m_BodySize = bodyBox.m_BodySize;
    }
}

[Serializable]
public class BodyPolygon
{
    public List<BodyPolygonUnit> m_BodyPolygonUnits = new();
    
    public BodyPolygon() {}

    public BodyPolygon(List<BodyPolygonUnit> bodyPolygonUnits)
    {
        m_BodyPolygonUnits = bodyPolygonUnits;
    }

    public BodyPolygon(BodyPolygon bodyPolygon)
    {
        m_BodyPolygonUnits = new List<BodyPolygonUnit>(bodyPolygon.m_BodyPolygonUnits.Count);
        
        foreach (var bodyPolygonUnit in bodyPolygon.m_BodyPolygonUnits)
        {
            m_BodyPolygonUnits.Add(new BodyPolygonUnit(bodyPolygonUnit));
        }
    }

    public BodyPolygon(BodyBox bodyBox)
    {
        var halfWidth = bodyBox.m_BodySize.x / 2f;
        var halfHeight = bodyBox.m_BodySize.y / 2f;
        List<Vector2> bodyPoints = new()
        {
            new Vector2(bodyBox.m_BodyCenter.x - halfWidth, bodyBox.m_BodyCenter.y - halfHeight),
            new Vector2(bodyBox.m_BodyCenter.x + halfWidth, bodyBox.m_BodyCenter.y - halfHeight),
            new Vector2(bodyBox.m_BodyCenter.x + halfWidth, bodyBox.m_BodyCenter.y + halfHeight),
            new Vector2(bodyBox.m_BodyCenter.x - halfWidth, bodyBox.m_BodyCenter.y + halfHeight),
        };
        var bodyPolygonUnit = new BodyPolygonUnit(bodyPoints);
        m_BodyPolygonUnits = new()
        {
            bodyPolygonUnit
        };
    }
}

[Serializable]
public class BodyPolygonUnit
{
    public List<Vector2> m_BodyPoints;

    public BodyPolygonUnit(List<Vector2> bodyPoints)
    {
        m_BodyPoints = bodyPoints;
    }

    public BodyPolygonUnit(BodyPolygonUnit bodyPolygonUnit)
    {
        m_BodyPoints = new List<Vector2>(bodyPolygonUnit.m_BodyPoints.Count);
        
        foreach (var bodyPoint in bodyPolygonUnit.m_BodyPoints)
        {
            m_BodyPoints.Add(bodyPoint);
        }
    }
}

