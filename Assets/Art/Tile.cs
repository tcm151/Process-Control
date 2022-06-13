using UnityEngine;
using UnityEngine.Tilemaps;


namespace ProcessControl.Art
{
	[CreateAssetMenu(fileName = "Tile", menuName = "Tile")]
	public class Tile : ScriptableObject
	{
		new public string name;
		public TileBase tile;
	}
}