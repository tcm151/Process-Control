using UnityEngine;


namespace ProcessControl.Tools
{
    public class Range
    {
        public float min {get; private set;}
        public float max {get; private set;}

        //> EMPTY CONSTRUCTOR
        public Range()
        {
            min = 999999999;
            max = -999999999;
        }

        //> DEFINED RANGE CONSTRUCTOR
        public Range(float min, float max)
        {
            this.min = min;
            this.max = max;
        }

        //> ADD VALID VALUE TO RANGE
        public void Add(float value)
        {
            if (value > max) max = value;
            if (value < min) min = value;
        }

        //> CHECK IF VALUE WITHIN RANGE
        public bool Contains(float value) => (value >= min && value <= max);

        //> ADD RANGES TOGETHER
        public static Range operator +(Range rangeA, Range rangeB)
            => new Range
            {
                min = Mathf.Min(rangeA.min, rangeB.min),
                max = Mathf.Max(rangeA.max, rangeB.max),
            };
    }
}