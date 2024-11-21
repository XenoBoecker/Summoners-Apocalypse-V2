using System;
using UnityEngine;

[System.Serializable]
public class Fighter
{
    public FighterData fighterData;

    public string ownerName;
    public string fighterName;

    public int team;

    [SerializeField]public int health;

    public bool isDead;

    public delegate void FighterDiedEventHandler(Fighter fighter);

    public event FighterDiedEventHandler OnDeath;

    public Fighter(FighterData fighterData)
    {
        this.fighterData = fighterData;
        health = fighterData.maxHealth;
    }

    public void GetHitBy(Ability selectedAbility)
    {
        TakeDamage(selectedAbility.damage);
    }

    private void TakeDamage(int dmg)
    {
        // Debug.Log(fighterName + " took " + dmg + " damage!");
        health -= dmg;

        if (health > fighterData.maxHealth) health = fighterData.maxHealth;

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log(fighterName + " has died!");
        OnDeath(this);
    }

    public static byte[] Serialize(object customType)
    {
        Fighter fighter = (Fighter)customType;

        // Convert fighterID, health, and isDead to bytes.
        byte[] idBytes = BitConverter.GetBytes(fighter.fighterData.fighterID);
        byte[] healthBytes = BitConverter.GetBytes(fighter.health);
        byte[] isDeadBytes = BitConverter.GetBytes(fighter.isDead);

        // Concatenate the bytes.
        byte[] serializedData = new byte[idBytes.Length + healthBytes.Length + isDeadBytes.Length];
        idBytes.CopyTo(serializedData, 0);
        healthBytes.CopyTo(serializedData, idBytes.Length);
        isDeadBytes.CopyTo(serializedData, idBytes.Length + healthBytes.Length);

        return serializedData;
    }

    public static object Deserialize(byte[] data)
    {
        if (data.Length != sizeof(int) * 3)
        {
            throw new ArgumentException("Invalid data length for deserialization.");
        }

        // Extract fighterID, health, and isDead bytes from the data.
        byte[] idBytes = new byte[sizeof(int)];
        byte[] healthBytes = new byte[sizeof(int)];
        byte[] isDeadBytes = new byte[sizeof(bool)];

        Array.Copy(data, 0, idBytes, 0, sizeof(int));
        Array.Copy(data, sizeof(int), healthBytes, 0, sizeof(int));
        Array.Copy(data, sizeof(int) * 2, isDeadBytes, 0, sizeof(bool));

        // Convert bytes back to their respective types.
        int fighterID = BitConverter.ToInt32(idBytes, 0);
        int health = BitConverter.ToInt32(healthBytes, 0);
        bool isDead = BitConverter.ToBoolean(isDeadBytes, 0);

        // Create a new Fighter instance and set its properties.
        Fighter fighter = new Fighter(new FighterData(fighterID));
        fighter.health = health;
        fighter.isDead = isDead;

        return fighter;
    }

}