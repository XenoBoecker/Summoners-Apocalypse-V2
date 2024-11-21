using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class FighterData
{
    public int fighterID;

    [SerializeField]public Sprite sprite;

    [SerializeField]public int initiative;

    [SerializeField]public int maxHealth;
    
    [SerializeField]public List<Ability> abilities = new List<Ability>();
    
    public FighterData(int maxHealth, List<Ability> abilities, int initiative, int id)
    {
        this.fighterID = id;

        this.maxHealth = maxHealth;

        this.abilities = abilities;
        this.initiative = initiative;
    }

    public FighterData(int id)
    {
        this.fighterID = id;
    }
}