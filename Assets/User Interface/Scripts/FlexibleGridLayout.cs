using UnityEngine;
using UnityEngine.UI;


namespace ProcessControl.UI
{
    public class FlexibleGridLayout : LayoutGroup
    {
        public enum Alignment { Horizontal, Vertical }
        public enum FitType { Uniform, Width, Height, FixedRows, FixedColumns, FixedBoth }

        public Alignment alignment;
        
        [Header("Fit")]
        public FitType fitType;
        public bool fitX;
        public bool fitY;
        
        [Header("Size")]
        [Min(1)] public int rows;
        [Min(1)] public int columns;
        [Min(0)] public Vector2 spacing;
        public Vector2 cellSize;


        public bool NudgeLastItemsOver;

        override public void CalculateLayoutInputVertical()
        {
            base.CalculateLayoutInputHorizontal();

            float sqrRt;
            var childCount = transform.childCount;
            switch (fitType)
            {
                case FitType.Uniform:
                {
                    fitX = fitY = true;
                    sqrRt = Mathf.Sqrt(childCount);
                    rows = Mathf.CeilToInt(sqrRt);
                    columns = Mathf.CeilToInt(sqrRt);
                    rows = Mathf.CeilToInt(childCount / (float)columns);
                    columns = Mathf.CeilToInt(childCount / (float)rows);
                    break;
                }
                case FitType.Width:
                {
                    fitX = fitY = true;
                    sqrRt = Mathf.Sqrt(transform.childCount);
                    rows = Mathf.CeilToInt(sqrRt);
                    columns = Mathf.CeilToInt(sqrRt);
                    rows = Mathf.CeilToInt(childCount / (float)columns);
                    break;
                }
                case FitType.Height:
                {
                    fitX = fitY = true;
                    sqrRt = Mathf.Sqrt(transform.childCount);
                    rows = Mathf.CeilToInt(sqrRt);
                    columns = Mathf.CeilToInt(sqrRt);
                    columns = Mathf.CeilToInt(childCount / (float)rows);
                    break;
                }
                case FitType.FixedRows:
                {
                    fitX = fitY = false;
                    columns = Mathf.CeilToInt(childCount / (float)rows);
                    break;
                }
                case FitType.FixedColumns:
                {
                    fitX = fitY = false;
                    rows = Mathf.CeilToInt(childCount / (float)columns);
                    break;
                }
                case FitType.FixedBoth:
                {
                    fitX = fitY = false;
                    break;
                }
            }


            float cellWidth = 0, cellHeight = 0;
            var rect = rectTransform.rect;
            switch (alignment)
            {
                case Alignment.Horizontal:
                {
                    cellWidth = (rect.width / columns) - ((spacing.x / columns) * (columns - 1)) - (padding.left / (float)columns) - (padding.right / (float)columns);
                    cellHeight = (rect.height / rows) - ((spacing.y / rows) * (rows - 1)) - (padding.top / (float)rows) - (padding.bottom / (float)rows);
                    break;
                }
                case Alignment.Vertical:
                {
                    cellHeight = (rect.width / columns) - ((spacing.x / columns) * (columns - 1)) - (padding.left / (float)columns) - (padding.right / (float)columns);
                    cellWidth = (rect.height / rows) - ((spacing.y / rows) * (rows - 1)) - (padding.top / (float)rows) - (padding.bottom / (float)rows);
                    break;
                }
            }

            cellSize.x = fitX ? (cellWidth <= 0 ? cellSize.x : cellWidth) : cellSize.x;
            cellSize.y = fitY ? (cellHeight <= 0 ? cellSize.y : cellHeight) : cellSize.y;

            for (int i = 0; i < rectChildren.Count; i++)
            {
                var item = rectChildren[i];
                float xLastItemOffset = 0;

                int rowCount = 0;
                int columnCount = 0;
                switch (alignment)
                {
                    case Alignment.Horizontal:
                    {
                        rowCount = i / columns;
                        columnCount = i % columns;
                        if (NudgeLastItemsOver && rowCount == (rectChildren.Count / columns))
                            xLastItemOffset = (cellSize.x + padding.left) / 2;
                        break;
                    }
                    case Alignment.Vertical:
                    {
                        rowCount = i / rows;
                        columnCount = i % rows;
                        if (NudgeLastItemsOver && rowCount == (rectChildren.Count / rows)) 
                            xLastItemOffset = (cellSize.x + padding.left) / 2;
                        break;
                    }
                }

                float xPos = (cellSize.x * columnCount) + (spacing.x * columnCount) + padding.left + xLastItemOffset;
                float yPos = (cellSize.y * rowCount) + (spacing.y * rowCount) + padding.top;

                switch (m_ChildAlignment)
                {
                    case TextAnchor.UpperLeft:
                        break;
                    
                    case TextAnchor.UpperCenter:
                        xPos += (0.5f * (rectTransform.sizeDelta.x + (spacing.x + padding.left + padding.left) - (columns * (cellSize.x + spacing.x + padding.left))));
                        break;
                    case TextAnchor.UpperRight:
                        xPos = -xPos + rectTransform.sizeDelta.x - cellSize.x;
                        //No need to change yPos;
                        break;
                    case TextAnchor.MiddleLeft:
                        //No need to change xPos;
                        yPos += (0.5f * (rectTransform.sizeDelta.y + (spacing.y + padding.top + padding.top) - (rows * (cellSize.y + spacing.y + padding.top)))); //Center yPos
                        break;
                    case TextAnchor.MiddleCenter:
                        xPos += (0.5f * (rectTransform.sizeDelta.x + (spacing.x + padding.left + padding.left) - (columns * (cellSize.x + spacing.x + padding.left)))); //Center xPos
                        yPos += (0.5f * (rectTransform.sizeDelta.y + (spacing.y + padding.top + padding.top) - (rows * (cellSize.y + spacing.y + padding.top))));       //Center yPos
                        break;
                    case TextAnchor.MiddleRight:
                        xPos = -xPos + rectTransform.sizeDelta.x - cellSize.x;                                                                                    //Flip xPos to go bottom-up
                        yPos += (0.5f * (rectTransform.sizeDelta.y + (spacing.y + padding.top + padding.top) - (rows * (cellSize.y + spacing.y + padding.top)))); //Center yPos
                        break;
                    case TextAnchor.LowerLeft:
                        //No need to change xPos;
                        yPos = -yPos + rectTransform.sizeDelta.y - cellSize.y; //Flip yPos to go Right to Left
                        break;
                    case TextAnchor.LowerCenter:
                        xPos += (0.5f * (rectTransform.sizeDelta.x + (spacing.x + padding.left + padding.left) - (columns * (cellSize.x + spacing.x + padding.left)))); //Center xPos
                        yPos = -yPos + rectTransform.sizeDelta.y - cellSize.y;                                                                                          //Flip yPos to go Right to Left
                        break;
                    case TextAnchor.LowerRight:
                        xPos = -xPos + rectTransform.sizeDelta.x - cellSize.x; //Flip xPos to go bottom-up
                        yPos = -yPos + rectTransform.sizeDelta.y - cellSize.y; //Flip yPos to go Right to Left
                        break;
                }

                SetChildAlongAxis(item, 0, xPos, cellSize.x);
                SetChildAlongAxis(item, 1, yPos, cellSize.y);
            }
        }

        override public void SetLayoutHorizontal() { }

        override public void SetLayoutVertical() { }
    }
}