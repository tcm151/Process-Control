﻿using System.IO;
using UnityEngine;


namespace ProcessControl.Tools
{
	public static class FileManager
	{
		public static string GetFilePath(string fileName) => $"{Application.dataPath}/Files/{fileName}";

		public static void WriteFile<T>(string fileName, T contents)
		{
			var pathToFile = GetFilePath(fileName);
			var json = JsonUtility.ToJson(contents);
			Debug.Log(json);
			File.WriteAllText(pathToFile, json);
			Debug.Log($"Wrote {typeof(T)} to {pathToFile}");
		}

		public static T ReadFile<T>(string fileName) where T : class
		{
			var pathToFile = GetFilePath(fileName);
			string json = File.ReadAllText(pathToFile);
			var contents = JsonUtility.FromJson(json, typeof(T)) as T;
			Debug.Log($"Read {typeof(T)} from {pathToFile}");
			return contents;
		}
	}
}