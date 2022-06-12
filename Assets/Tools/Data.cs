using UnityEngine;


namespace ProcessControl.Tools
{
	public interface Data<T> where T : MonoBehaviour
	{
		public void Save();
		public void Load();	
	}
}