using ProcessControl.Tools;
using UnityEngine;
using UnityEngine.UI;


namespace ProcessControl.UI
{
    // public class FlexItem : LayoutElement
    // {
    //     
    // }
    
    public class FlexGridLayout : LayoutGroup
    {
        public enum FitType { Uniform, Width, Height, FixedRows, FixedColumns }

        [Min(1)] public int rows;
        [Min(1)] public int columns;
        public Vector2 cellSize;
        public Vector2 spacing;

        public bool fitX, fitY;
        public FitType fit;

        private Rect rect;

        override public void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();
            
            switch (fit)
            {
                case FitType.Width:
                case FitType.Height:
                case FitType.Uniform:
                {
                    fitX = fitY = true;
                    rows = columns = Mathf.Sqrt(transform.childCount).CeilToInt();
                    break;
                }
            }


            switch (fit)
            {
                case FitType.Width:
                case FitType.FixedColumns:
                {
                    rows = (transform.childCount / (float)columns).CeilToInt();
                    break;
                }

                case FitType.Height:
                case FitType.FixedRows:
                {
                    columns = (transform.childCount / (float)rows).CeilToInt();
                    break;
                }
            }

            rect = rectTransform.rect;
            cellSize.x = (!fitX) ? cellSize.x : rect.width / columns - ((spacing.x / columns) * (columns - 1)) - (padding.left / (float)columns) - (padding.right / (float)columns);
            cellSize.y = (!fitY) ? cellSize.y : rect.height / rows - ((spacing.y / rows) * (columns - 1)) - (padding.top / (float)rows - (padding.bottom / (float)rows));

            
            for (int i = 0; i < rectChildren.Count; i++)
            {
                int rowCount = i / columns;
                int columnCount = i % columns;

                var child = rectChildren[i];
                var x = (cellSize.x * columnCount) + (spacing.x * columnCount) + padding.left;
                var y = (cellSize.y * rowCount) + (spacing.y * rowCount) + padding.top;
                
                switch (childAlignment)
                {
                    case TextAnchor.UpperLeft:
                        break;
                    
                    // case TextAnchor.UpperCenter:
                    //     x += (0.5f * (rectTransform.sizeDelta.x + (spacing.x + padding.left + padding.left) - (columns * (cellSize.x + spacing.x + padding.left))));
                    //     break;
                    // case TextAnchor.UpperRight:
                    //     x = -x + rectTransform.sizeDelta.x - cellSize.x;
                    //     //No need to change yPos;
                    //     break;
                    // case TextAnchor.MiddleLeft:
                    //     //No need to change xPos;
                    //     y += (0.5f * (rectTransform.sizeDelta.y + (spacing.y + padding.top + padding.top) - (rows * (cellSize.y + spacing.y + padding.top)))); //Center yPos
                    //     break;
                    // case TextAnchor.MiddleCenter:
                    //     Debug.Log($"I1:{i} X:{x} Y:{y}");
                    //     var center = new Vector2(rect.width / 2, rect.height / 2);
                    //     Debug.Log(center);
                    //     x += center.x / 2;
                    //     // y += 
                    //     Debug.Log($"I2:{i} X:{x} Y:{y}");
                    //
                    //     break;
                    // case TextAnchor.MiddleRight:
                    //     x = -x + rectTransform.sizeDelta.x - cellSize.x;                                                                                    //Flip xPos to go bottom-up
                    //     y += (0.5f * (rectTransform.sizeDelta.y + (spacing.y + padding.top + padding.top) - (rows * (cellSize.y + spacing.y + padding.top)))); //Center yPos
                    //     break;
                    // case TextAnchor.LowerLeft:
                    //     //No need to change xPos;
                    //     y = -y + rectTransform.sizeDelta.y - cellSize.y; //Flip yPos to go Right to Left
                    //     break;
                    // case TextAnchor.LowerCenter:
                    //     x += (0.5f * (rectTransform.sizeDelta.x + (spacing.x + padding.left + padding.left) - (columns * (cellSize.x + spacing.x + padding.left)))); //Center xPos
                    //     y = -y + rectTransform.sizeDelta.y - cellSize.y;                                                                                          //Flip yPos to go Right to Left
                    //     break;
                    // case TextAnchor.LowerRight:
                    //     x = -x + rectTransform.sizeDelta.x - cellSize.x; //Flip xPos to go bottom-up
                    //     y = -y + rectTransform.sizeDelta.y - cellSize.y; //Flip yPos to go Right to Left
                    //     break;
                }

                SetChildAlongAxis(child, 0, x, cellSize.x);
                SetChildAlongAxis(child, 1, y, cellSize.y);
            }
        }

        override public void CalculateLayoutInputVertical() { }

        override public void SetLayoutHorizontal() { }

        override public void SetLayoutVertical() { }
    }
}