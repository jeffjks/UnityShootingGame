using UnityEngine;
using UnityEngine.EventSystems;

public class ArrowController : MonoBehaviour, IMoveHandler
{
    [SerializeField] private ArrowHighlighter[] m_ArrowHighlighters;

    public void OnMove(AxisEventData axisEventData)
    {
        foreach (var arrowHighlighter in m_ArrowHighlighters)
        {
            arrowHighlighter.SetHighlightSprite(axisEventData.moveDir);
        }
    }
}
