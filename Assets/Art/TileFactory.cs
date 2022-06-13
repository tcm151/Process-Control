using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProcessControl.Tools;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace ProcessControl.Art
{
	public class TileFactory : Factory
	{
		public List<Tile> tiles;

		public override T Get<T>(string name) where T : class
		{
			var item = tiles.FirstOrDefault(i => i.name == name);
			if (item is { }) return item as T;

			Debug.Log($"Tile \"{name}\" was not found.");
			return default;
		}

		public override void Initialize()
		{
			var assets = Resources.LoadAll("Tiles/", typeof(Tile));
			tiles = assets.Cast<Tile>().ToList();
		}
	}
}