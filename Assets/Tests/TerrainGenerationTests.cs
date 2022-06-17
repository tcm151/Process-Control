using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using ProcessControl.Art;
using ProcessControl.Industry;
using ProcessControl.Procedural;
using ProcessControl.Serialization;
using ProcessControl.Tools;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.Tilemaps;


namespace Tests
{
	public class TerrainGenerationTests
	{
		[UnityTest]
		public IEnumerator CanBuildTerrain()
		{
			var serviceManager = ServiceManager.Create();
			serviceManager.CreateService<TileFactory>();
			serviceManager.CreateService<ItemFactory>();

			var grid = new GameObject("cellGrid").AddComponent<CellGrid>();
			var data = FileManager.ReadFile<CellGridData>("greenEggs");
			grid.data = data;
			Assert.True(grid.data == data);

			grid.Initialize();
			Assert.NotNull(grid.tilemaps);
			Assert.True(grid.data.chunks.GetLength(0) == grid.data.size);
			Assert.True(grid.data.chunks.GetLength(1) == grid.data.size);
			grid.data.chunks.ForEach(chunk =>
			{
				Assert.NotNull(chunk);
				Assert.NotNull(chunk.cells);
				Assert.NotNull(chunk.chunkCenter);
				Assert.NotNull(chunk.chunkOffset);
				Assert.NotNull(chunk.neighbours);
				Assert.True(chunk.cells.GetLength(0) == grid.data.chunkSize);
				Assert.True(chunk.cells.GetLength(1) == grid.data.chunkSize);

				chunk.cells.ForEach(cell =>
				{
					Assert.NotNull(cell);
					Assert.NotNull(cell.parentChunk);
					Assert.NotNull(cell.position);
					Assert.NotNull(cell.coords);
					Assert.NotNull(cell.neighbours);
				});
			});

			grid.GenerateChunks(grid.data.chunks.ToList());
			grid.data.chunks.ForEach(chunk =>
			{
				chunk.cells.ForEach(cell =>
				{
					Assert.NotZero(cell.terrainValue);
					Assert.NotNull(cell.biome);
					Assert.NotNull(cell.resourceDeposits);
				});
			});


			// }
			// catch (Exception exception)
			// {
			// }

			yield return null;
			// yield return new WaitForSeconds(20f);
		}
	}
}