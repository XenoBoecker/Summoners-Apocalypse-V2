using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using UnityEngine;

public static class Serialization
{
    public static byte[] SerializeFighters(List<Fighter> fighters)
    {
        List<string> serializedSprites = new List<string>();

        // Serialize each fighter's sprite
        foreach (var fighter in fighters)
        {
            byte[] spriteData = SerializeSprite(fighter.fighterData.sprite);
            if (spriteData != null)
            {
                serializedSprites.Add(Convert.ToBase64String(spriteData)); // Convert byte[] to Base64
            }
            else
            {
                serializedSprites.Add(null);
            }
        }

        // Wrap the fighters and their sprites into the wrapper
        FighterListWrapper wrapper = new FighterListWrapper(fighters, serializedSprites);

        // Convert the wrapper to JSON and then to a byte array
        string json = JsonUtility.ToJson(wrapper);
        return System.Text.Encoding.UTF8.GetBytes(json);
    }


    private static byte[] SerializeSprite(Sprite sprite)
    {
        if (sprite == null)
        {
            Debug.Log("No sprite do serialize");
            return null;
        }

        Texture2D texture = sprite.texture;
        return texture.EncodeToJPG(); // Convert texture to JPG
    }
    public static List<Fighter> DeserializeFighters(byte[] bytes)
    {
        // Decode the byte array into a JSON string
        string json = System.Text.Encoding.UTF8.GetString(bytes);

        // Deserialize the wrapper
        FighterListWrapper wrapper = JsonUtility.FromJson<FighterListWrapper>(json);

        if (wrapper.serializedSprites == null) Debug.Log("Serialized sprites are null");

        Debug.Log("Deserialize Wrapper sprite count " + wrapper.serializedSprites.Count);

        // Reconstruct sprites and assign them back to fighters
        for (int i = 0; i < wrapper.fighters.Count; i++)
        {
            string base64Sprite = wrapper.serializedSprites[i];
            byte[] spriteData = base64Sprite != null ? Convert.FromBase64String(base64Sprite) : null;
            wrapper.fighters[i].fighterData.sprite = DeserializeSprite(spriteData);
        }

        return wrapper.fighters;
    }


    private static Sprite DeserializeSprite(byte[] data)
    {
        if (data == null || data.Length == 0) return null;

        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(data); // Load the image into the texture
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }


    [System.Serializable]
    private class FighterListWrapper
    {
        public List<Fighter> fighters;
        public List<string> serializedSprites; // Store as Base64 strings

        public FighterListWrapper(List<Fighter> fighters, List<string> sprites)
        {
            if (sprites == null) Debug.Log("no sprites");
            else Debug.Log("Wrapping " + sprites.Count + " sprites");

            this.fighters = fighters;
            this.serializedSprites = sprites;
        }
    }


}