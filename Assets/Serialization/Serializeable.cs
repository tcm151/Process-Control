namespace ProcessControl.Serialization
{
	public interface Serializeable
	{
		public void Save(string name);
		public void Load(string name);
	}
}