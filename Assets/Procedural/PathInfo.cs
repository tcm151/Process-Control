namespace ProcessControl.Procedural
{
    public class PathInfo
    {
        public float gCost = float.MaxValue;
        public float hCost;
        public float fCost;

        public Cell previousInPath;

        public void Set(float g, float h, Cell previousCell)
        {
            gCost = g;
            hCost = h;
            fCost = gCost + hCost;
            previousInPath = previousCell;
        }
        
        public void Reset()
        {
            hCost = 0;
            gCost = int.MaxValue;
            previousInPath = null;
        }
    }
}