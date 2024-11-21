using UnityEngine;


public enum TargetType
{
    Self,
    Enemy,
    Ally,
    All
}

public enum SpecialEffect
{
    None,
    Stun,
    Poison,
    Burn,
    Buff,
    Debuff
}

[System.Serializable]
public class Ability
{
    public string abilityName;

    public TargetType[] targetTypes;
    public SpecialEffect[] specialEffects;
    public int damage;

    public int targetCount;

    public Ability(TargetType[] targetTypes, SpecialEffect[] specialEffects, int damage, int targetCount, string abilityName)
    {
        this.targetTypes = targetTypes;
        this.specialEffects = specialEffects;
        this.damage = damage;
        this.targetCount = targetCount;

        this.abilityName = abilityName;
    }
}