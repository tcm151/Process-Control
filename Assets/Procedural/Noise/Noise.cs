using UnityEngine;


namespace TCM.NoiseGeneration
{
    public static class Noise
    {
        //> NOISE TYPES
        public enum Type { Simple, Rigid };

        //> NOISE LAYER SETTINGS
        [System.Serializable] public class Layer
        {
            public bool enabled = true;
            public bool useMask = true;

            public Type noiseType;
            public SimplexNoise generator = new SimplexNoise();

            [Range( 1,  8)] public int octaves = 1;
            [Range( 0, 10)] public float strength = 1f;
            [Range( 0,  5)] public float baseRoughness = 1f;
            [Range( 0, 15)] public float roughness = 2f;
            [Range( 0,  5)] public float persistence = 0.5f;
            [Range(-5,  5)] public float localZero = 0f;
            public Vector3 offset = Vector3.zero;
        }

        //> GET A NOISE VALUE FROM A VECTOR3
        public static float GenerateValue(Layer noise, Vector3 vector3)
        {
            float generatedValue = 0f;
            float roughness = noise.baseRoughness;
            float amplitude = 1;
            float weight = 1;

            switch (noise.noiseType)
            {
                //> SIMPLE NOISE FILTER
                case Type.Simple:
                {
                    for (int i = 0; i < noise.octaves; i++)
                    {
                        float value = noise.generator.Generate(vector3 * roughness + noise.offset);
                        generatedValue += (value + 1) / 2 * amplitude;
                        roughness *= noise.roughness;
                        amplitude *= noise.persistence;
                    } break;
                }

                //> MOUNTAINOUS NOISE FILTER
                case Type.Rigid:
                {
                    for (int i = 0; i < noise.octaves; i++)
                    {
                        float value = 1 - Mathf.Abs(noise.generator.Generate(vector3 * roughness + noise.offset));
                        value *= value * weight;
                        weight = value;
                        generatedValue += value * amplitude;
                        roughness *= noise.roughness;
                        amplitude *= noise.persistence;
                    } break;
                }
            }

            generatedValue = Mathf.Max(0, generatedValue - noise.localZero);
            return generatedValue * noise.strength;
        }
    }
}