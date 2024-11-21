using System;
using System.Collections.Generic;
using UnityEngine;

public class CombatUI : MonoBehaviour
{
    [SerializeField] GameObject fighterShowPrefab;
    [SerializeField] GameObject[] fighterShowContainers;
    [SerializeField] AbilitiesShowContainer abilitiesShowContainer;
    [SerializeField] TurnOrderShow turnOrderShow;

    CombatManager combatManager;

    List<FighterShowObject> fighterShowObjects = new List<FighterShowObject>();

    public int localPlayerTeam = 0;


    private void Awake()
    {
        combatManager = GetComponent<CombatManager>();

        combatManager.OnSetupFight.AddListener(UpdateFighterUI);

        combatManager.OnStartTurn.AddListener(ShowCurrentFighterAbilities);
        combatManager.OnStartTurn.AddListener(ShowTurnOrder);
        combatManager.OnStartTurn.AddListener(HighlightCurrentTurnFighter);

        combatManager.OnAbilityUsed.AddListener(UnHighlightAllFighters);

        combatManager.OnFightersChanged.AddListener(UpdateFighterUI);
        combatManager.OnFightersChanged.AddListener(ShowTurnOrder);


        abilitiesShowContainer.AbilitySelected += OnAbilitySelected;
    }

    private void UpdateFighterUI()
    {
        if (fighterShowObjects.Count == 0) SpawnFighterShowObjects();

        List<Fighter> allFighters = combatManager.GetAllFighters();

        ShowCurrentFighters(allFighters);

        for (int i = 0; i < allFighters.Count; i++)
        {
            fighterShowObjects[i].SetFighter(allFighters[i]);
        }
    }

    void ShowCurrentFighters(List<Fighter> allFighters)
    {
        for (int i = 0; i < combatManager.maxFightersPerTeam * 2; i++)
        {
            if (i >= allFighters.Count)
            {
                fighterShowObjects[i].gameObject.SetActive(false);
            }
            else
            {
                fighterShowObjects[i].gameObject.SetActive(true);

                Fighter fighter = allFighters[i];

                fighterShowObjects[i].SetFighter(fighter);
                fighterShowObjects[i].transform.SetParent(fighterShowContainers[(fighter.team + localPlayerTeam) % 2].transform); // if localPlayerTeam == 1: team 1 ist links und team 0 rechts
            }
        }
    }

    private void SpawnFighterShowObjects()
    {
        for (int i = 0; i < combatManager.maxFightersPerTeam * 2; i++)
        {
            SpawnFighterShowObject();
        }
    }

    private void SpawnFighterShowObject()
    {
        GameObject temp = (GameObject)Instantiate(fighterShowPrefab, fighterShowContainers[0].transform);

        fighterShowObjects.Add(temp.GetComponent<FighterShowObject>());
        temp.GetComponent<FighterShowObject>().OnSelectFighter += OnSelectFighter;
    }

    private void OnSelectFighter(Fighter fighter)
    {
        combatManager.SelectFighter(fighter);
    }

    void ShowCurrentFighterAbilities()
    {
        if (IsMyTurn())
        {
            ShowAbilities(combatManager.GetCurrentTurnFighter());
        }
        else
        {
            abilitiesShowContainer.gameObject.SetActive(false);
        }
    }

    public void ShowAbilities(Fighter fighter)
    {
        abilitiesShowContainer.SetFighter(fighter);

        abilitiesShowContainer.gameObject.SetActive(true);
    }

    private void ShowTurnOrder()
    {
        
        // print("ShowTurnOrder: " + combatManager.allFighters.Count);
        turnOrderShow.ShowTurnOrder(combatManager.GetAllFighters(), combatManager.GetCurrentTurnIndex());
    }

    public void OnAbilitySelected(Ability ability)
    {
        UnHighlightAllFighters();

        List<Fighter> possibleTargets = combatManager.GetPossibleTargets(combatManager.GetCurrentTurnFighter(), ability);

        foreach (Fighter target in possibleTargets)
        {
            HighlightFighter(target);
        }

        combatManager.SetSelectedAbility(ability);
    }


    private void HighlightFighter(Fighter fighter)
    {
        fighterShowObjects[combatManager.GetAllFighters().IndexOf(fighter)].SetTargetHighlightActive(true);
    }

    void UnHighlightAllFighters()
    {
        foreach (FighterShowObject fighterShowObject in fighterShowObjects)
        {
            fighterShowObject.SetTargetHighlightActive(false);
        }
    }

    void HighlightCurrentTurnFighter()
    {
        for (int i = 0; i < fighterShowObjects.Count; i++)
        {
            if (fighterShowObjects[i].GetFighter == combatManager.GetCurrentTurnFighter()) fighterShowObjects[i].SetThisFightersTurnHighlightActive(true);
            else fighterShowObjects[i].SetThisFightersTurnHighlightActive(false);
        }
    }

    private bool IsMyTurn()
    {
        return combatManager.GetCurrentTurnFighter().team == localPlayerTeam;
    }
}
