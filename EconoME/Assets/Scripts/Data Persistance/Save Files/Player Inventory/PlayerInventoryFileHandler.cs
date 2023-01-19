using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Newtonsoft.Json;

[Serializable]
public class PlayerInventoryFileHandler
{
    private string dataDirPath;
    [SerializeField] private string dataFileName = "/Saves/Player/Inventory.gay";
    [SerializeField] private bool useEncryption = false;
    private string encryptionCodeWord = "Unlucky";

    [SerializeField] List<InventoryObject> inventoryObjects = new();
    List<SerializableInventory> InventoriesData = new();

    public void Load()
    {
        dataDirPath = Application.dataPath;
        List<SerializableInventory> loadedData = null;

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
                loadedData = JsonUtility.FromJson<List<SerializableInventory>>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Error occured when trying to load data from file: " + fullPath + "\n" + e);
                return;
            }
        }
        else
        {
            Debug.Log("Can't find file");
        }
        #endregion


        #region Set data to running game

        List<SerializableInventory> UnfoundInventories = new();
        List<bool> DataCorrectlyLoaded = new();

        for (int i = 0; i < inventoryObjects.Count; i++)
        {
            DataCorrectlyLoaded.Add(false);
        }

        for (int i = 0; i < loadedData.Count; i++)
        {
            var foundIndex = inventoryObjects.FindIndex(a => a.data.inventoryName == loadedData[i].inventoryName);

            if (foundIndex == -1)
            {
                UnfoundInventories.Add(loadedData[i]);
                continue;
            }
            inventoryObjects[foundIndex].data = loadedData[i];
            DataCorrectlyLoaded[foundIndex] = true;
        }

        int DataNotInFile = 0;
        string WarningString = "The following data was not found in the save file: \n";
        for (int i = 0; i < DataCorrectlyLoaded.Count; i++)
        {
            if (!DataCorrectlyLoaded[i])
            {
                DataNotInFile++;
                WarningString += inventoryObjects[i].data.inventoryName + "\n";
            }
        }
        if (DataNotInFile > 0)
        {
            Debug.LogWarning(WarningString);
        }
        if (UnfoundInventories.Count > 0)
        {
            WarningString = $"File has data for following inventory(s) that are not in the active game:\n";
            for (int i = 0; i < UnfoundInventories.Count; i++)
            {
                WarningString += UnfoundInventories[i].inventoryName + "\n";
            }
            Debug.Log(WarningString);
        }

        #endregion

    }

    public void Save()
    {
        dataDirPath = Application.dataPath;

        #region Get data from running game


        for (int i = 0; i < inventoryObjects.Count; i++)
        {
            InventoriesData.Add(inventoryObjects[i].data);
        }



        #endregion

        #region Create JSON data and write to file
        // use Path.Combine to account for different OS's having different path separators
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        try
        {
            // create the directory the file will be written to if it doesn't already exist
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            // serialize the C# game data object into Json

            string dataToStore = JsonUtility.ToJson(InventoriesData, true);

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
