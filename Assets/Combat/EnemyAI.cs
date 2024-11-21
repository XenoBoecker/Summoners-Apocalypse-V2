using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    CombatManager combatManager;

    // Start is called before the first frame update
    void Start()
    {
        combatManager = GameObject.FindObjectOfType<CombatManager>();
        combatManager.OnStartTurn.AddListener(ExecuteTurn);
    }

    void ExecuteTurn()
    {
        print("Enemy AI is executing turn");
        if (combatManager.GetCurrentTurnFighter().team == 0) return;

        print("AI execute turn");

        int selectedAbilityIndex = SelectAbilityIndex();

        List<Fighter> targets = SelectTargets(combatManager.GetCurrentTurnFighter().fighterData.abilities[selectedAbilityIndex]);

        combatManager.SetSelectedAbility(selectedAbilityIndex);
        combatManager.SetSelectedTargets(targets);

        combatManager.CastSelectedAbilityOnSelectedTargets();
    }

    private int SelectAbilityIndex()
    {
        List<Ability> abilities = combatManager.GetCurrentTurnFighter().fighterData.abilities;

        int rand = UnityEngine.Random.Range(0, abilities.Count);

        print("AI selected ability: " + abilities[rand].abilityName);

        return rand;
    }

    private List<Fighter> SelectTargets(Ability selectedAbility)
    {
        List<Fighter> targets = new List<Fighter>();
        List<Fighter> possibleTargets = combatManager.GetPossibleTargets(combatManager.GetCurrentTurnFighter(), selectedAbility);

        while (targets.Count < selectedAbility.targetCount && possibleTargets.Count != 0)
        {
            Fighter target = possibleTargets[UnityEngine.Random.Range(0, possibleTargets.Count)];
            targets.Add(target);
            possibleTargets.Remove(target);
        }

        print("AI selected targets:");
        for (int i = 0; i < targets.Count; i++)
        {
            print("target " + i + ": " + targets[i].fighterName);
        }

        return targets;
    }
}
