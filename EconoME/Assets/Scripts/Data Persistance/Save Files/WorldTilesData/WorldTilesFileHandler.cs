using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;
[Serializable]
public class WorldTilesFileHandler
{

    public TilePlacerObject MapData;

    [SerializeField] private string dataFileName = "/Saves/World/PlacedTiles.gay";
    [SerializeField] private bool useEncryption = false;
    private string encryptionCodeWord = "Unlucky";

    public WorldTilesFileHandler(bool useEncryption)
    {
        this.useEncryption = useEncryption;
    }

    public void Load()
    {
        WorldTilesData loadedData = null;
        #region Get data from file and convert to data object
        // use Path.Combine to account for different OS's having different path separators
        string fullPath = Path.Combine(Application.dataPath, dataFileName);

        if (File.Exists(fullPath))
        {
            try
            {
                // load the serialized data from the file
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                // optionally decrypt the data
                if (useEncryption)
                {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }

                // deserialize the data from Json back into the C# object
                loadedData = JsonUtility.FromJson<WorldTilesData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Error occured when trying to load data from file: " + fullPath + "\n" + e);
            }
        }
        #endregion

        #region Set data to running game

        MapData.ClearWorld();
        foreach (Vector2Int key in loadedData.WorldTiles.Keys)
        {
            if (!MapData.WorldTiles.ContainsKey(key))
            {
                MapData.WorldTiles.Add(key, loadedData.WorldTiles[key]);
            }

        }

        #endregion
    }

    public void Save()
    {
        WorldTilesData data = new WorldTilesData();

        #region Get data from running game
        foreach (Vector2Int key in MapData.WorldTiles.Keys)
        {
            if (MapData.WorldTiles[key].TileName != "Spawn")
            {
                data.WorldTiles.Add(key, MapData.WorldTiles[key]);
            }
        }

        #endregion

        #region Create JSON data and write to file

        // use Path.Combine to account for different OS's having different path separators
        string fullPath = Path.Combine(Application.dataPath, dataFileName);
        try
        {
            // create the directory the file will be written to if it doesn't already exist
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            // serialize the C# game data object into Json
            string dataToStore = JsonUtility.ToJson(data, true);

            // optionally encrypt the data
            if (useEncryption)
            {
                dataToStore = EncryptDecrypt(dataToStore);
            }

            // write the serialized data to the file
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                    writer.Close();
                    stream.Close();
                }
            }

        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + e);
        }
        #endregion

    }

    // the below is a simple implementation of XOR encryption
    private string EncryptDecrypt(string data)
    {
        string modifiedData = "";
        encryptionCodeWord = "Unlucky";
        for (int i = 0; i < data.Length; i++)
        {
            modifiedData += (char)(data[i] ^ encryptionCodeWord[i % encryptionCodeWord.Length]);
        }
        return modifiedData;
    }
}
