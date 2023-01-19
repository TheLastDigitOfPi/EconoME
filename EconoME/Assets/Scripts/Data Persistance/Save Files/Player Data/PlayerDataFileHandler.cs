using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;

[Serializable]
public class PlayerDataFileHandler
{
    private string dataDirPath;
    [SerializeField] private string dataFileName = "/Saves/Player/GlobalModifiers.gay";
    [SerializeField] private bool useEncryption = false;
    private string encryptionCodeWord = "Unlucky";

    [SerializeField] PlayerDataObject AllSaveData;


    public void Load()
    {
        dataDirPath = Application.dataPath;
        SerializableSavableData loadedData = null;

        loadedData = GetDataFromFile();

        LoadDataToGame();

        void LoadDataToGame()
        {
            if (loadedData == null) { return; }

            var OriginalIntData = AllSaveData.data.IntVariables.Cast<ISaveableObject>().ToList();
            var OriginalFloatData = AllSaveData.data.FloatVariables.Cast<ISaveableObject>().ToList();
            var OriginalBoolData = AllSaveData.data.BoolValues.Cast<ISaveableObject>().ToList();
            var OriginalVector3Data = AllSaveData.data.Vector3Variables.Cast<ISaveableObject>().ToList();

            FindData(loadedData.IntData, OriginalIntData);
            FindData(loadedData.FloatData, OriginalFloatData);
            FindData(loadedData.BoolData, OriginalBoolData);
            FindData(loadedData.Vector3Data, OriginalVector3Data);

            void FindData<T>(List<DataContainer<T>> LoadData, List<ISaveableObject> SaveData)
            {
                List<bool> FoundSaveSpot = new();
                for (int i = 0; i < SaveData.Count; i++)
                {
                    FoundSaveSpot.Add(false);
                }
                List<DataContainer<T>> UnfoundData = new();

                //Attempt to save 1 to 1 file to save data
                for (int i = 0; i < SaveData.Count; i++)
                {
                    if (i > LoadData.Count - 1) { Debug.LogWarning($"Loaded data had count of {LoadData.Count} while expecting {SaveData.Count}"); break; }
                    if (!SaveData[i].Load(LoadData[i]))
                    {
                        UnfoundData.Add(LoadData[i]);
                        continue;
                    }
                    FoundSaveSpot[i] = true;
                }

                //Attempt to find misplaced data
                for (int i = 0; i < UnfoundData.Count; i++)
                {
                    //See if data is in our list
                    int foundDataIndex = SaveData.FindIndex(a => a.ValueName == UnfoundData[i].ValueName);
                    if(foundDataIndex == -1){continue;}
                    ISaveableObject foundData = SaveData[foundDataIndex];
                    //If it isn't continue
                    if (foundData == null) { continue; }
                    //Otherwise load it to the data
                    if (!foundData.Load(UnfoundData[i])) { Debug.LogWarning("Lol it found the correct data and it still didn't work, nice"); continue; }
                    //Update our list of unfound data
                    FoundSaveSpot[foundDataIndex] = true;
                    UnfoundData.RemoveAt(i);
                    i--;
                }

                if (UnfoundData.Count > 0)
                {
                    string WarningString = "Unable to place the following data from file: \n";
                    for (int i = 0; i < UnfoundData.Count; i++)
                    {
                        WarningString += $"Value Name- {UnfoundData[i].ValueName} \n";
                    }
                    Debug.LogWarning(WarningString);
                }
                int failedToFindItemsCount = 0;
                string UnfoundDataWarning = "The following data was not found in the file: \n";
                for (int i = 0; i < FoundSaveSpot.Count; i++)
                {
                    if (!FoundSaveSpot[i])
                    {
                        failedToFindItemsCount++;
                        UnfoundDataWarning += $"Value Name- {SaveData[i].ValueName} \n";
                    }
                }

                if(failedToFindItemsCount > 0)
                {
                    Debug.LogWarning(UnfoundDataWarning);
                }
            }

        }

        SerializableSavableData GetDataFromFile()
        {
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
                    loadedData = JsonUtility.FromJson<SerializableSavableData>(dataToLoad);

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

            return loadedData;
        }

    }
    public void Save()
    {
        dataDirPath = Application.dataPath;

        // use Path.Combine to account for different OS's having different path separators
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        try
        {
            // create the directory the file will be written to if it doesn't already exist
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            // serialize the C# game data object into Json

            var SerializableData = new SerializableSavableData(AllSaveData.data);
            string dataToStore = JsonUtility.ToJson(SerializableData, true);

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
