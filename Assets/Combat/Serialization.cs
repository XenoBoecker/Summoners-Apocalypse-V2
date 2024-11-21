using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

public class Serialization
{
    public static byte[] SerializeFighters(List<Fighter> fighters)
    {
        string json = JsonUtility.ToJson(new FighterListWrapper(fighters));
        return System.Text.Encoding.UTF8.GetBytes(json);
    }

    public static List<Fighter> DeserializeFighters(byte[] bytes)
    {
        string json = System.Text.Encoding.UTF8.GetString(bytes);
        FighterListWrapper wrapper = JsonUtility.FromJson<FighterListWrapper>(json);
        return wrapper.fighters;
    }

    [System.Serializable]
    private class FighterListWrapper
    {
        public List<Fighter> fighters;

        public FighterListWrapper(List<Fighter> fighters)
        {
            this.fighters = fighters;
        }
    }
}