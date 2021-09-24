using UnityEngine;
using TCM.NoiseGeneration;
using System.Collections.Generic;

namespace ProcessControl.Procedural
{
    public class Chunk
    {
        public class Data
        {
            public List<ProceduralGrid.Cell> cells = new List<ProceduralGrid.Cell>();
        }

        public Chunk(ref ProceduralGrid.Data grid)
        {
            this.grid = grid;
        }

        private ProceduralGrid.Data grid;

        public void Generate()
        {
            grid.cells.ForEach(c =>
            {
                var noiseValue = GenerateNoise(c);
                grid.noiseRange.Add(noiseValue);
                c.value = noiseValue;
            });

            // Debug.Log($"Noise Range: {noiseRange.min}-{noiseRange.max}");
        }

        public float GenerateNoise(ProceduralGrid.Cell cell)
        {
            float noiseValue = 0f;
            float firstLayerElevation = 0f;

            if (grid.noiseLayers.Count > 0)
            {
                firstLayerElevation = Noise.GenerateValue(grid.noiseLayers[0], cell.center);
                if (grid.noiseLayers[0].enabled) noiseValue = firstLayerElevation;
            }

            for (int i = 1; i < grid.noiseLayers.Count; i++)
            {
                // ignore if not enabled
                if (!grid.noiseLayers[i].enabled) continue;

                float firstLayerMask = (grid.noiseLayers[i].useMask) ? firstLayerElevation : 1;
                noiseValue += Noise.GenerateValue(grid.noiseLayers[i], cell.center) * firstLayerMask;
            }

            grid.noiseRange.Add(noiseValue);
            return noiseValue;
        }

        public void Apply()
        {
            grid.cells.ForEach(c =>
            {
                var tile = (c.value >= grid.noiseLayers[0].localZero) ? grid.tiles[0] : grid.tiles[1];
                grid.tilemap.SetTile(new Vector3Int(c.coordinates.x, c.coordinates.y, 0), tile);
            });
        }
    }
}