using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ImageManager
{
	private static ImageManager _instance;

	private Dictionary<string, Texture2D> loaderDict = new Dictionary<string, Texture2D>();

	public static ImageManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new ImageManager();
			}
			return _instance;
		}
	}

	public void Add(string url, Texture2D image)
	{
		/*if (!string.IsNullOrEmpty(url) && !loaderDict.ContainsKey(url))
		{
			loaderDict.Add(url, image);
			Save(url, image, loaderDict.Count);
		}*/
	}

	private void Save(string url, Texture2D image, int number)
	{
		/*string text = Application.persistentDataPath + "/Images";
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		text = text + "/texture2D." + number;
		byte[] array = image.EncodeToPNG();
		using (FileStream fileStream = new FileStream(text, FileMode.Create))
		{
			BinaryWriter binaryWriter = new BinaryWriter(fileStream);
			binaryWriter.Write(url);
			binaryWriter.Write(array.Length);
			binaryWriter.Write(array);
			fileStream.Close();
		}*/
	}

	public void Load()
	{
		/*string path = Application.persistentDataPath + "/Images";
		if (!Directory.Exists(path))
		{
			return;
		}
		FileInfo[] files = new DirectoryInfo(path).GetFiles();
		if (files == null || files.Length <= 0)
		{
			return;
		}
		int i = 0;
		for (int num = files.Length; i < num; i++)
		{
			using (FileStream fileStream = files[i].OpenRead())
			{
				BinaryReader binaryReader = new BinaryReader(fileStream);
				string key = binaryReader.ReadString();
				int count = binaryReader.ReadInt32();
				byte[] data = binaryReader.ReadBytes(count);
				fileStream.Close();
				Texture2D texture2D = new Texture2D(1, 1);
				texture2D.LoadImage(data);
				loaderDict.Add(key, texture2D);
			}
		}*/
	}

	public Texture2D GetTexture(string url)
	{
		/*if (string.IsNullOrEmpty(url) || !loaderDict.ContainsKey(url))
		{
			return null;
		}
		return loaderDict[url];*/
		return null;
}

	public bool ContainsKey(string url)
	{
		/*if (string.IsNullOrEmpty(url))
		{
			return false;
		}
		return loaderDict.ContainsKey(url);*/
		return false; 
	}
}
