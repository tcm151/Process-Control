using UnityEngine;
using UnityEngine.Serialization;


namespace ProcessControl.Procedural
{
    public static class Noise
    {
        //> NOISE TYPES
        public enum Type { Simple, Rigid, Brownian, Swiss };

        //> NOISE LAYER SETTINGS
        [System.Serializable] public class Layer
        {
            public bool enabled = true;
            public bool useMask = true;
            public bool subtract;

            public Type noiseType;
            public SimplexNoise generator = new SimplexNoise();

            [Range(01, 004)] public int octaves = 3;
            [Range(00, 001)] public float strength = 0.25f;
            [Range(00, .1f)] public float baseRoughness = 0.001f;
            [Range(00, 010)] public float roughness = 4f;
            [Range(00, 002)] public float persistence = 0.5f;
            [FormerlySerializedAs("localZero")][Range(00, 1)] public float threshold = 0f;
            public Vector3 offset = Vector3.zero;
        }

        //> GET A NOISE VALUE FROM A VECTOR3
        public static float GenerateValue(Layer noise, Vector3 vector3)
        {
            float generatedValue = 0f;
            float roughness = noise.baseRoughness;
            float amplitude = 1;
            // float frequency = 1;
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

                case Type.Brownian:
                {
                    float acceleration = 2f;

                    for (int i = 0; i < noise.octaves; i++)
                    {
                        generatedValue += noise.generator.Generate(vector3 * Mathf.Pow(acceleration, i)) / Mathf.Pow(acceleration, i);
                    } break;
                }
                
            }

            generatedValue = Mathf.Max(0, generatedValue - noise.threshold);
            return (noise.subtract) ? -generatedValue * noise.strength : generatedValue * noise.strength;
        }
    }
}