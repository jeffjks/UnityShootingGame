using UnityEngine;
using UnityEngine.EventSystems;

public class RankingDifficultyPageController : MonoBehaviour, IMoveHandler
{
    public RankingDataLoader m_RankingDataLoader;

    public void OnMove(AxisEventData axisEventData)
    {
        if (EventSystem.current.currentSelectedGameObject != gameObject)
        {
            return;
        }

        var moveInputX = (int) axisEventData.moveVector.x;

        if (moveInputX != 0)
        {
            m_RankingDataLoader.MovePage(moveInputX);
        }
    }
}
