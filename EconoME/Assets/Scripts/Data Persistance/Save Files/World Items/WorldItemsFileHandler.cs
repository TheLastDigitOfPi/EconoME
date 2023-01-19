using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

[Serializable]
public class WorldItemsFileHandler
{

    private string dataDirPath;
    [SerializeField] private string dataFileName = "/Saves/World/WorldItems.gay";
    [SerializeField] private bool useEncryption = false;
    private string encryptionCodeWord = "Unlucky";

    public WorldItemsData worldItems = new WorldItemsData();
    public void Load()
    {
        dataDirPath = Application.dataPath;

        #region Get data from file and convert to data object
        // use Path.Combine to account for different OS's having different path separators
        string fullPath = Path.Combine(dataDirPath, dataFileName);


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
                worldItems = JsonUtility.FromJson<WorldItemsData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Error occured when trying to load data from file: " + fullPath + "\n" + e);
            }
        }
        else
        {
            Debug.Log("Can't find file");
        }
        #endregion


        #region Set data to running game

        for (int i = 0; i < worldItems.ItemsData.Count; i++)
        {
            if (worldItems.ItemsData[i].item is not WorldTile)
            {
                worldItems.ItemsData[i].item.SetIcon(Resources.Load<Sprite>(worldItems.ItemsData[i].item.IconPath), worldItems.ItemsData[i].item.IconPath);

            }
            else
            {
                Texture2D texture = new Texture2D(2, 2);
                byte[] filedata = File.ReadAllBytes(worldItems.ItemsData[i].item.IconPath);
                texture.LoadImage(filedata);
                Rect rect = new Rect(0, 0, texture.width, texture.height);
                worldItems.ItemsData[i].item.SetIcon(Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f)), worldItems.ItemsData[i].item.IconPath);
            }
        }

        //Rest Done in DataPersistanceManager since needs to instantiate objects

        #endregion

    }

    public void Save()
    {
        dataDirPath = Application.dataPath;

        #region Get data from running game
        //Got Data in Persistance Manager since needed to search for objects
        #endregion

        #region Create JSON data and write to file
        // use Path.Combine to account for different OS's having different path separators
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        try
        {
            // create the directory the file will be written to if it doesn't already exist
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            // serialize the C# game data object into Json

            string dataToStore = JsonUtility.ToJson(worldItems, true);

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

    public void setAllItemData(List<WorldItemInstance> data)
    {
        worldItems.ItemsData = data;
    }
}
