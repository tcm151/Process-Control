using UnityEngine;
using UnityEngine.UI;


namespace ProcessControl.UI
{
    public class LineRendererUI : Graphic
    {
        public float width;
        public float length;
        public Vector3 start, end;

        override protected void OnValidate()
        {
            rectTransform.rect.Set(0, 0, width, length);
        }

        override protected void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            var rect = rectTransform.rect;
            
            UIVertex vertex = UIVertex.simpleVert;

            vertex.position = new Vector3(rect.x, rect.y);
            vh.AddVert(vertex);
            
            vertex.position = new Vector3(rect.x, rect.height/2);
            vh.AddVert(vertex);
            
            vertex.position = new Vector3(width, rect.height/2);
            vh.AddVert(vertex);
            
            vertex.position = new Vector3(width, rect.y);
            vh.AddVert(vertex);
            
            vh.AddTriangle(0, 1, 2);   
            vh.AddTriangle(2, 3, 0);   
        }
    }
}

