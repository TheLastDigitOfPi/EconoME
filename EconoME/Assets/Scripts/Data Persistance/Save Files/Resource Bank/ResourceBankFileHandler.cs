using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

[Serializable]
public class ResourceBankFileHandler
{
    public ResourceBankHandler resourceBankHandler;
    public GameObject BankParent;
    private string dataDirPath;
    [SerializeField] private string dataFileName = "/Saves/ResourceBank/BankItems.gay";
    [SerializeField] private bool useEncryption = false;
    private string encryptionCodeWord = "Unlucky";

    public void Load()
    {
        dataDirPath = Application.dataPath;
        ResourceBankData loadedData = null;


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
                loadedData = JsonUtility.FromJson<ResourceBankData>(dataToLoad);
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
        bool BankWasOpen = false;
        if (BankParent.activeSelf)
        {
            BankWasOpen = true;
        }
        else
        {
            BankParent.SetActive(true);
        }
        resourceBankHandler.ResetSearch();

        for (int i = 0; i < loadedData.BankSlotNames.Count; i++)
        {
            if (resourceBankHandler.AllBankSlots[i].slotData.item.ItemName != loadedData.BankSlotNames[i])
            {
                Debug.LogWarning("JSON data does not sync with bank data, is the file old?");
                continue;
            }
            resourceBankHandler.AllBankSlots[i].slotData.item.Stacksize = loadedData.BankSlotsItemCount[i];
            resourceBankHandler.AllBankSlots[i].UpdateSlot();
        }

        if (!BankWasOpen)
        {
            BankParent.SetActive(false);
        }

        #endregion

    }

    public void Save()
    {

        dataDirPath = Application.dataPath;
        if (resourceBankHandler == null)
        {
            Debug.LogError("Attempted to save resource bank but bank handler was not referenced");
            return;
        }

        ResourceBankData data = new ResourceBankData();

        #region Get data from running game

        bool BankWasOpen = false;
        if (BankParent.activeSelf)
        {
            BankWasOpen = true;
        }
        else
        {
            BankParent.SetActive(true);
        }

        resourceBankHandler.TempResetSearch();

        for (int i = 0; i < resourceBankHandler.AllBankSlots.Length; i++)
        {
            if (resourceBankHandler.AllBankSlots[i].slotData.item == null) { continue; }
            if (resourceBankHandler.AllBankSlots[i].slotData.item.ItemName == "None" || resourceBankHandler.AllBankSlots[i].slotData.item.ItemName == "") { continue; }
            data.BankSlotNames.Add(resourceBankHandler.AllBankSlots[i].slotData.item.ItemName);
            data.BankSlotsItemCount.Add(resourceBankHandler.AllBankSlots[i].slotData.item.Stacksize);
        }

        resourceBankHandler.RefineSearch();

        if (!BankWasOpen)
        {
            BankParent.SetActive(false);
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
