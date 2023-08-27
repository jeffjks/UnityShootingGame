using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridLayoutAutoExpand : MonoBehaviour
{
	public GridLayoutGroup gridLayout;
	public RectTransform rectTransform;
	public int amountPerRow;
	
	void Start ()
	{
		RecalculateGridLayout();
	}

	private void OnRectTransformDimensionsChange()
	{
		RecalculateGridLayout();
	}
		
	void RecalculateGridLayout ()
	{
		if (gridLayout != null)
		{
			gridLayout.constraint = GridLayoutGroup.Constraint.FixedRowCount;

			int count = gridLayout.transform.childCount;

			Vector2 scale = rectTransform.rect.size;

			Vector3 cellSize = gridLayout.cellSize;
			Vector3 spacing = gridLayout.spacing;

			int amountPerColumn = count / amountPerRow;

			float childWidth = (scale.x - spacing.x * (amountPerRow-1)) / amountPerRow;
			float childHeight = (scale.y - spacing.y * (amountPerColumn-1)) / amountPerColumn;

			cellSize.x = childWidth;
			cellSize.y = childHeight;

			gridLayout.cellSize = cellSize;	
		}
	}
}