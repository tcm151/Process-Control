using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using ProcessControl.Serialization;
using UnityEngine.TestTools;
using File = UnityEngine.Windows.File;


namespace Tests
{
	public class FileTests
	{
		[Serializable]
		public class Container
		{
			public string Name;
			public List<Data> DataList;
		}

		[Serializable]
		public class Data
		{
			public int Id;
			public string Description;
		}
		
		private readonly Container mockContainer = new Container
		{
			Name = "Mock Container", DataList = new List<Data>
			{
				new Data {Id = 0, Description = "This first one!"},
				new Data {Id = 1, Description = "This second one"},
				new Data {Id = 2, Description = "This middle one!"},
				new Data {Id = 3, Description = "I could be any one"},
				new Data {Id = 4, Description = "The end."},
			},
		};
		
		[Test]
		public void CanWriteFile()
		{
			FileManager.WriteFile("testFile", mockContainer);
			Assert.True(File.Exists(FileManager.GetFilePath("testFile")));
		}
		
		[Test]
		public void CanReadFile()
		{
			FileManager.WriteFile("testFile", mockContainer);
			
			var fileContents = FileManager.ReadFile<Container>("testFile");
			Assert.True(fileContents.Name == mockContainer.Name);
			Assert.True(fileContents.DataList.Count == mockContainer.DataList.Count);
			for (int i = 0; i < fileContents.DataList.Count; i++)
			{
				Assert.True(fileContents.DataList[i].Id == mockContainer.DataList[i].Id);
				Assert.True(fileContents.DataList[i].Description == mockContainer.DataList[i].Description);
			}
		}
	}
}