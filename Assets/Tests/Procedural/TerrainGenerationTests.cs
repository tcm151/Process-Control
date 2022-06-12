using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using ProcessControl.Procedural;
using ProcessControl.Tools;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Procedural
{
	public class TerrainGenerationTests
	{
		[UnityTest]
		public IEnumerator IndustryTestsWithEnumerator()
		{
			var tileGrid = new GameObject("tileGrid").AddComponent<CellGrid>();
			var gridData = FileManager.ReadFile<CellGrid.Data>("foray.json");
			tileGrid.SetGridData(gridData);
			tileGrid.Initialize();
			// Assert.True(tileGrid);
			
			yield return null;
		}
	}
}