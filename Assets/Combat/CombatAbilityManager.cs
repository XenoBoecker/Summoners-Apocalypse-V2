using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class CombatAbilityManager : MonoBehaviour
{
    CombatFighterStorage fighterStorage;
    CombatTurnManager turnManager;

    public UnityEvent OnAbilityTargetsSelected = new UnityEvent();
    public UnityEvent OnAbilityUsed = new UnityEvent();
    public UnityEvent OnFightersChanged = new UnityEvent();


    Ability selectedAbility;
    List<Fighter> currentAbilityTargets = new List<Fighter>();


    internal void Setup(CombatFighterStorage fighterStorage, CombatTurnManager turnManager)
    {
        this.fighterStorage = fighterStorage;
        this.turnManager = turnManager;
    }

    internal void AddAbilityTarget(Fighter fighter) => currentAbilityTargets.Add(fighter);
    internal void SetSelectedTargets(List<Fighter> targets)
    {
        currentAbilityTargets.Clear();
        foreach (Fighter fighter in targets)
        {
            AddAbilityTarget(fighter);
        }
    }
    internal List<Fighter> GetCurrentAbilityTargets() => currentAbilityTargets;

    internal void SetSelectedAbility(int abilityIndex) => selectedAbility = turnManager.GetCurrentTurnFighter().fighterData.abilities[abilityIndex];
    internal void SetSelectedAbility(Ability ability) => selectedAbility = ability;
    internal int GetSelectedAbilityIndex() => turnManager.GetCurrentTurnFighter().fighterData.abilities.IndexOf(selectedAbility);

    internal List<Fighter> GetPossibleTargets(Fighter fighter, Ability ability)
    {
        List<Fighter> possibleTargets = new List<Fighter>();

        foreach (Fighter target in fighterStorage.GetAllFighters())
        {
            if (target.team != fighter.team)
            {
                if (ability.targetTypes.Contains(TargetType.Enemy))
                {
                    possibleTargets.Add(target);
                }
            }
            else
            {
                if (ability.targetTypes.Contains(TargetType.Ally) && target != fighter)
                {
                    possibleTargets.Add(target);
                }
                if (ability.targetTypes.Contains(TargetType.Self) && target == fighter)
                {
                    possibleTargets.Add(target);
                }
            }
        }

        return possibleTargets;
    }

    internal void SelectFighter(Fighter fighter) // do I do this here (EnemyAI just has its own list which gets passed to UseAbility)
    {
        Debug.Log("OnFighterSelected");
        if (selectedAbility != null)
        {
            Debug.Log("add target");
            currentAbilityTargets.Add(fighter);

            if (currentAbilityTargets.Count == selectedAbility.targetCount)
            {
                Debug.Log("now go");
                OnAbilityTargetsSelected.Invoke();

                UseAbility(selectedAbility, currentAbilityTargets);
            }
        }
    }

    internal void CastSelectedAbilityOnSelectedTargets()
    {
        UseAbility(selectedAbility, currentAbilityTargets);
    }


    public void UseAbility(Ability ability, List<Fighter> targets)
    {
        Debug.Log("Use Ability: " + ability.abilityName);
        if (targets == null)
        {
            Debug.Log("targets is null");
        }
        
        StartCoroutine(CastAbility(ability, targets));
    }

    IEnumerator CastAbility(Ability ability, List<Fighter> targets)
    {
        Debug.Log("wait now for CastAbility: " + ability.abilityName);
        yield return new WaitForSeconds(1f);
        Debug.Log("Now CastAbility: " + ability.abilityName);

        foreach (Fighter target in targets)
        {
            target.GetHitBy(ability);
        }

        selectedAbility = null;
        currentAbilityTargets.Clear();

        OnAbilityUsed.Invoke();
        OnFightersChanged.Invoke();

        turnManager.EndTurn();
    }
}
