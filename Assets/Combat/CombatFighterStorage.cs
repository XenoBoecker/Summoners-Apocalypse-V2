using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class CombatFighterStorage
{
    FighterLoader fighterLoader;

    public UnityEvent OnFightersChanged = new UnityEvent();
    public UnityEvent OnCombatEnd = new UnityEvent();

    List<Fighter> allFighters = new List<Fighter>();
    List<Fighter> team0Fighters = new List<Fighter>();
    List<Fighter> team1Fighters = new List<Fighter>();

    public CombatFighterStorage(int maxPlayerCount, int maxFightersPerTeam)
    {
        fighterLoader = new FighterLoader(maxPlayerCount, maxFightersPerTeam);

        Setup();
    }

    void Setup()
    {
        List<Fighter> combatants = fighterLoader.GetAllFighters();

        allFighters.Clear();
        team0Fighters.Clear();
        team1Fighters.Clear();

        foreach (Fighter fighter in combatants)
        {
            allFighters.Add(fighter);
            fighter.OnDeath += OnFighterDeath;

            if (fighter.team == 0)
            {
                team0Fighters.Add(fighter);
            }
            else
            {
                team1Fighters.Add(fighter);
            }
        }

        OrderFightersByInitiative();
    }

    private void OrderFightersByInitiative()
    {
        allFighters = allFighters.OrderBy(o => o.fighterData.initiative).ToList();

        for (int i = 0; i < allFighters.Count; i++)
        {
            Debug.Log(i + ": " + allFighters[i].fighterName);
        }
    }

    internal List<Fighter> GetAllFighters()
    {
        return allFighters;
    }

    internal Fighter GetNextFighter(Fighter currentTurnFighter)
    {
        int nextTurnFighterIndex = allFighters.IndexOf(currentTurnFighter);

        int security = 0;
        do
        {
            security++;
            if(security > allFighters.Count * 2) 
            {
                Debug.LogError("All fighters dead");
            }

            nextTurnFighterIndex = (nextTurnFighterIndex + 1) % allFighters.Count;
        } while (allFighters[nextTurnFighterIndex].isDead);

        return allFighters[nextTurnFighterIndex];
    }

    public bool FightOver()
    {
        if (team0Fighters.Count == 0)
        {
            Debug.Log("You Loose");
            OnCombatEnd.Invoke();
            return true;
        }
        else if (team1Fighters.Count == 0)
        {
            Debug.Log("You Won");
            OnCombatEnd.Invoke();
            return true;
        }
        return false;
    }

    private void OnFighterDeath(Fighter fighter)
    {
        if (FightOver()) return;

        OnFightersChanged.Invoke(); // already gets called after ability is used, but well doppelt gemoppelt hält besser
    }
}
