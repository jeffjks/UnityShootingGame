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
    public BodyCircle m_BodyCircle = new();

    [DrawIf("m_BodyType", BodyType.Box, ComparisonType.Equals)]
    public BodyBox m_BodyBox = new();
    
    [DrawIf("m_BodyType", BodyType.Polygon, ComparisonType.Equals)]
    public BodyPolygon m_BodyPolygon = new();
    
    public UnityAction<TriggerBody> m_OnTriggerBodyEnter;
    public UnityAction<TriggerBody> m_OnTriggerBodyExit;
    public UnityAction<TriggerBody> m_OnTriggerBodyStay;

    public BodyCircle TransformedBodyCircle => GetTransformedBody(m_BodyCircle);
    public BodyPolygon TransformedBodyPolygon => GetTransformedBody(m_BodyPolygon);
    public BodyType BodyTypeForComparison { get; private set; }

    private readonly HashSet<TriggerBody> _triggerBodySet = new();

#if UNITY_EDITOR
    public List<TriggerBody> m_DebugTriggerBody = new();
    
    private void Update()
    {
        m_DebugTriggerBody = _triggerBodySet.ToList();
    }
    
    private void OnDrawGizmosSelected()
    {
        if (enabled == false)
            return;
        
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

    private void Start()
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
    }

    private BodyCircle GetTransformedBody(BodyCircle bodyCircle)
    {
        var newBodyCircle = new BodyCircle();
        var point = TransformPoint(bodyCircle.m_BodyCenter);
        newBodyCircle.m_BodyCenter = point;
        
        var radius = m_BodyCircle.m_BodyRadius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
        newBodyCircle.m_BodyRadius = radius;
        return newBodyCircle;
    }

    private BodyPolygon GetTransformedBody(BodyPolygon bodyPolygon)
    {
        var newBodyPolygon = new BodyPolygon(new List<BodyPolygonUnit>(bodyPolygon.m_BodyPolygonUnits.Count));
        foreach (var bodyPolygonUnit in bodyPolygon.m_BodyPolygonUnits)
        {
            var newBodyPolygonUnit = new BodyPolygonUnit(new List<Vector2>(bodyPolygonUnit.m_BodyPoints.Count));
            
            foreach (var point in bodyPolygonUnit.m_BodyPoints)
            {
                var transformedPoint = TransformPoint(point);
                newBodyPolygonUnit.m_BodyPoints.Add(transformedPoint);
            }

            newBodyPolygon.m_BodyPolygonUnits.Add(newBodyPolygonUnit);
        }
        
        return newBodyPolygon;
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
            var isAdded = _triggerBodySet.Add(other);
            if (isAdded == false)
                return;
            other._triggerBodySet.Add(this);
            m_OnTriggerBodyEnter?.Invoke(other);
            //Debug.Log($"{transform.parent.gameObject.name} OnTriggerBodyCollisionEnter: {other.transform.parent.gameObject.name}");
        }
        else
        {
            var isRemoved = _triggerBodySet.Remove(other);
            if (isRemoved == false)
                return;
            other._triggerBodySet.Remove(this);
            m_OnTriggerBodyExit?.Invoke(other);
            //Debug.Log($"{transform.parent.gameObject.name} OnTriggerBodyCollisionExit: {other.transform.parent.gameObject.name}");
        }
    }

    public void OnTriggerBodyCollisionStay()
    {
        foreach (var triggerBody in _triggerBodySet)
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
    
    private void OnDisable()
    {
        foreach (var triggerBody in _triggerBodySet)
        {
            if (gameObject.CompareTag("Enemy"))
            {
            }
            triggerBody._triggerBodySet.Remove(this);
            //Debug.LogError($"{gameObject.name} triggerBodySet removed {isRemoved}: {triggerBody}");
        }
        _triggerBodySet.Clear();
#if UNITY_EDITOR
        m_DebugTriggerBody.Clear();
#endif
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
}

