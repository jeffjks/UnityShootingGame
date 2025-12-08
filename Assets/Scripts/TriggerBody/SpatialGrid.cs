using System.Collections.Generic;
using UnityEngine;

public class SpatialGrid
{
    private const float CellSize = 2; // 2
    private readonly Dictionary<Vector2Int, HashSet<TriggerBody>> _cells = new();
    private readonly Vector2 _worldMin;
    private readonly Vector2 _worldMax;

    private readonly HashSet<TriggerBody> _nearByResult = new();
    private readonly List<Vector2Int> _cellsInBounds = new();
    //private readonly TriggerBodyType _type;

    public SpatialGrid()
    {
        _worldMin = new Vector2(-12, -4);
        _worldMax = new Vector2(12, 20);
        _cellsInBounds.Add(new Vector2Int(0, 0));
        //_type = type;
    }

    public void Clear()
    {
        foreach (var cell in _cells)
        {
            cell.Value.Clear();
        }
    }

    public override string ToString()
    {
        var str = string.Empty;

        foreach (var cell in _cells)
        {
            str += $"[{cell.Key}]\n";

            foreach (var body in cell.Value)
            {
                if (body.m_TriggerBodyType == TriggerBodyType.GameBoundary || body.m_TriggerBodyType == TriggerBodyType.CameraBoundary)
                    continue;
                str += $"{body.m_TriggerBodyType} ({body})\n";
            }
        }

        return str;
    }

    public void Add(TriggerBody body)
    {
        var bounds = GetBounds(body);
        var target_cells = GetCellsInBounds(bounds);

        foreach (var cellCoord in target_cells)
        {
            if (!_cells.ContainsKey(cellCoord))
            {
                _cells[cellCoord] = new HashSet<TriggerBody>();
            }
            _cells[cellCoord].Add(body);
        }
    }

    public HashSet<TriggerBody> GetNearbyTriggerBodies(TriggerBody body)
    {
        var bounds = GetBounds(body);
        var target_cells = GetCellsInBounds(bounds);
        _nearByResult.Clear();
        
        foreach (var cellCoord in target_cells)
        {
            if (_cells.TryGetValue(cellCoord, out var cellBodies))
            {
                foreach (var otherBody in cellBodies)
                {
                    if (otherBody != body)
                    {
                        _nearByResult.Add(otherBody);
                    }
                }
            }
        }

        return _nearByResult;
    }

    private Bounds GetBounds(TriggerBody body)
    {
        var bodyType = body.m_BodyType;
        
        if (bodyType == TriggerBody.BodyType.Circle)
        {
            var circle = body.TransformedBodyCircle;
            var radius = circle.m_BodyRadius;
            var center = circle.m_BodyCenter;
            var size = new Vector2(radius * 2, radius * 2);

            return new Bounds(center, size);
        }
        else if (bodyType == TriggerBody.BodyType.Polygon || bodyType == TriggerBody.BodyType.Box)
        {
            var polygon = body.TransformedBodyPolygon;

            var min = new Vector2(float.MaxValue, float.MaxValue);
            var max = new Vector2(float.MinValue, float.MinValue);
            bool modified = false;

            foreach (var bodyPolygonUnit in polygon.m_BodyPolygonUnits)
            {
                foreach (var bodyPoint in bodyPolygonUnit.m_BodyPoints)
                {
                    min.x = Mathf.Min(min.x, bodyPoint.x);
                    min.y = Mathf.Min(min.y, bodyPoint.y);
                    max.x = Mathf.Max(max.x, bodyPoint.x);
                    max.y = Mathf.Max(max.y, bodyPoint.y);
                    modified = true;
                }
            }
            
            if (modified == false)
                return new Bounds(Vector2.zero, Vector2.zero);

            var center = (min + max) / 2f;
            var size = max - min;

            return new Bounds(new Vector2(center.x, center.y), new Vector2(size.x, size.y));
        }

        return new Bounds(Vector2.zero, Vector2.zero);
    }

    private List<Vector2Int> GetCellsInBounds(Bounds bounds)
    {
        _cellsInBounds.Clear();

        // Debug.DrawLine(bounds.min, bounds.max);

        var minCell = WorldToCell(bounds.min);
        var maxCell = WorldToCell(bounds.max);
        
        for (var x = minCell.x; x <= maxCell.x; x++)
        {
            for (var y = minCell.y; y <= maxCell.y; y++)
            {
                _cellsInBounds.Add(new Vector2Int(x, y));
            }
        }

        return _cellsInBounds;
    }

    private Vector2Int WorldToCell(Vector2 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x / CellSize);
        int y = Mathf.FloorToInt(worldPos.y / CellSize);

        return new Vector2Int(x, y);
    }
}