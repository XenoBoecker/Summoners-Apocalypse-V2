using UnityEngine;


public enum TargetType
{
    Self,
    Enemy,
    Ally,
    All
}

public enum StatusEffectType
{
    None,
    Stun,
    Poison,
    Burn,
    Buff,
    Debuff
}

public class StatusEffect // TODO not used yet, cause fighters were generater before
{
    StatusEffectType type;

    int effectValue;
}

[System.Serializable]
public class Ability
{
    public string abilityName;

    public TargetType[] targetTypes;
    public StatusEffectType[] specialEffects;
    public int damage;

    public int targetCount;

    public Ability(TargetType[] targetTypes, StatusEffectType[] specialEffects, int damage, int targetCount, string abilityName)
    {
        this.targetTypes = targetTypes;
        this.specialEffects = specialEffects;
        this.damage = damage;
        this.targetCount = targetCount;

        this.abilityName = abilityName;
    }
}