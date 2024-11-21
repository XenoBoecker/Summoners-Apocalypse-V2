using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;

public static class SaveLoad
{

    static string savePath = Application.persistentDataPath + "/SavedFiles";

    public static void SaveToJson(Fighter fighter)
    {
        CreateSaveFolder(savePath + "/" + fighter.ownerName);

        string json = JsonUtility.ToJson(fighter, true);
        File.WriteAllText(savePath + "/" + fighter.ownerName + "/FighterFile" + fighter.fighterData.fighterID + ".json", json);
    }

    public static Fighter LoadFromJson(string ownerName, int fighterID)
    {
        string json = File.ReadAllText(savePath + "/" + ownerName + "/FighterFile" + fighterID + ".json");

        Fighter fighter = JsonUtility.FromJson<Fighter>(json);

        fighter.fighterData.sprite = LoadImageAsSprite(ownerName, fighter.fighterData.fighterID);
        fighter.ownerName = ownerName; // if you move the fighters in the folders

        return fighter;
    }

    public static List<Fighter> LoadMyFighters(string ownerName)
    {
        List<Fighter> fighters = new List<Fighter>();

        string[] fighterFiles = Directory.GetFiles(savePath + "/" + ownerName, "FighterFile*.json");

        foreach (string fighterFile in fighterFiles)
        {
            fighters.Add(LoadFromJson(ownerName, ParseFighterID(fighterFile)));
        }

        return fighters;
        
        // int currentID = 0;
        // while (true)
        // {
        //     if (File.Exists(savePath + "/" + ownerName + "/FighterFile" + currentID + ".json"))
        //     {
        //         fighters.Add(LoadFromJson(ownerName, currentID));
        //         currentID++;
        //     }
        //     else
        //     {
        //         return fighters;
        //     }
        // }
    }
    private static int ParseFighterID(string fileName)
    {
        // Extract the ID from the file name
        string idString = Path.GetFileNameWithoutExtension(fileName).Replace("FighterFile", "");

        // Parse the ID as an integer
        if (int.TryParse(idString, out int fighterID))
        {
            return fighterID;
        }
        else
        {
            // Handle parsing error (you might want to throw an exception or handle it appropriately)
            Console.WriteLine($"Error parsing fighter ID from file: {fileName}");
            return -1; // or throw an exception
        }
    }

    public static int GetNextFighterID(string ownerName)
    {
        int highestID = 0;
        while (true)
        {
            string filePath = savePath + "/" + ownerName + "/FighterFile" + highestID + ".json";
            if (File.Exists(filePath))
            {
                highestID++;
            }
            else
            {
                return highestID;
            }
        }
    }

    public static void SaveImage(Texture2D texture, string ownerName, int fighterID)
    {
        CreateSaveFolder(savePath + "/" + ownerName);
        
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(savePath + "/" + ownerName + "/FighterImage" + fighterID + ".png", bytes);
    }

    public static Sprite LoadImageAsSprite(string ownerName, int fighterID)
    {
        // Load the image as a Texture2D.
        Texture2D texture = LoadTexture2D(ownerName, fighterID);

        if (texture != null)
        {
            // Create a new Sprite using the loaded texture.
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

            // Assign the created Sprite to a SpriteRenderer or an Image component.
            return sprite;
        }
        else
        {
            Debug.Log("Failed to load the image.");
            return null;
        }
    }

    private static Texture2D LoadTexture2D(string ownerName, int fighterID)
    {
        string filePath = savePath + "/" + ownerName + "/FighterImage" + fighterID + ".png";
        if (File.Exists(filePath))
        {
            byte[] bytes = File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(2, 2); // You can use any initial dimensions.

            if (texture.LoadImage(bytes))
            {
                return texture;
            }
            else
            {
                Debug.Log("Failed to load the image as a Texture2D.");
                return null;
            }
        }
        else
        {
            Debug.Log("File does not exist at the specified path: " + filePath);
            return null;
        }
    }
    private static void CreateSaveFolder(string saveFolder)
    {

        // Check if the folder exists, and if not, create it
        if (!Directory.Exists(saveFolder)) Directory.CreateDirectory(saveFolder);
    }

    //public void SaveFighterDataToJson(string filePath)
    //{
    //    FighterWrapper wrapper = new FighterWrapper(fighters);
    //    string json = JsonUtility.ToJson(wrapper);
    //
    //    File.WriteAllText(filePath, json);
    //}
    //
    //public List<Fighter> LoadFighterDataFromJson(string filePath)
    //{
    //    if (File.Exists(filePath))
    //    {
    //        string json = File.ReadAllText(filePath);
    //        FighterWrapper wrapper = JsonUtility.FromJson<FighterWrapper>(json);
    //        return wrapper.fighterList;
    //    }
    //    else
    //    {
    //        Debug.LogError("JSON file does not exist at the specified path.");
    //        return new List<Fighter>();
    //    }
    //}
}   //

[System.Serializable]
public class FighterWrapper
{
    public List<Fighter> fighterList;

    public FighterWrapper(List<Fighter> fighters)
    {
        fighterList = fighters;
    }
}
