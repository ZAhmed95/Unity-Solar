using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveLoad {

	public static Body[] save = SaveData.body;

	public static void Save()
	{
		string saveFile = Application.persistentDataPath + "/savedSimulation.sim";
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.OpenWrite(saveFile);
		bf.Serialize(file, save);
		file.Close();
	}

	public static void Load()
	{
		string saveFile = Application.persistentDataPath + "/savedSimulation.sim";
		if(File.Exists(saveFile))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(saveFile, FileMode.Open);
			SaveLoad.save = (Body[])bf.Deserialize(file);
			file.Close();
		}
	}
}
