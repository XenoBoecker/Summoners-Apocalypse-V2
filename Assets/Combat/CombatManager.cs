using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CombatManager : MonoBehaviour
{
    CombatFighterStorage fighterStorage;
    CombatTurnManager turnManager;
    CombatAbilityManager abilityManager;

    public int maxPlayerCount = 2;
    public int maxFightersPerTeam = 4;

    public UnityEvent OnSetupFight = new UnityEvent();
    public UnityEvent OnStartTurn = new UnityEvent();
    public UnityEvent OnAbilityTargetsSelected = new UnityEvent();
    public UnityEvent OnAbilityUsed = new UnityEvent();
    public UnityEvent OnFightersChanged = new UnityEvent();
    public UnityEvent OnCombatEnd = new UnityEvent();

    private void Awake()
    {
        fighterStorage = new CombatFighterStorage(maxPlayerCount, maxFightersPerTeam);
        fighterStorage.OnCombatEnd.AddListener(OnCombatEnd.Invoke);
        fighterStorage.OnFightersChanged.AddListener(OnFightersChanged.Invoke);

        turnManager = new CombatTurnManager(fighterStorage);
        turnManager.OnStartTurn.AddListener(OnStartTurn.Invoke);

        abilityManager = gameObject.AddComponent<CombatAbilityManager>();
        abilityManager.Setup(fighterStorage, turnManager);
        abilityManager.OnAbilityTargetsSelected.AddListener(OnAbilityTargetsSelected.Invoke);
        abilityManager.OnAbilityUsed.AddListener(OnAbilityUsed.Invoke);
        abilityManager.OnFightersChanged.AddListener(OnFightersChanged.Invoke);
    }

    private void Start()
    {
        OnFightersChanged.Invoke();
        OnSetupFight.Invoke();

        turnManager.StartTurn();
    }

    internal List<Fighter> GetAllFighters() => fighterStorage.GetAllFighters();

    internal void AddAbilityTargets(Fighter fighter) => abilityManager.AddAbilityTarget(fighter);

    internal void SetSelectedAbility(int abilityIndex) => abilityManager.SetSelectedAbility(abilityIndex);
    internal void SetSelectedAbility(Ability ability) => abilityManager.SetSelectedAbility(ability);

    internal void CastSelectedAbilityOnSelectedTargets() => abilityManager.CastSelectedAbilityOnSelectedTargets();

    internal Fighter GetCurrentTurnFighter() => turnManager.GetCurrentTurnFighter();
    internal int GetSelectedAbilityIndex() => abilityManager.GetSelectedAbilityIndex();

    internal List<Fighter> GetCurrentAbilityTargets() => abilityManager.GetCurrentAbilityTargets();

    internal void SetSelectedTargets(List<Fighter> targets) => abilityManager.SetSelectedTargets(targets);

    internal List<Fighter> GetPossibleTargets(Fighter fighter, Ability selectedAbility) => abilityManager.GetPossibleTargets(fighter, selectedAbility);

    internal void SelectFighter(Fighter fighter) => abilityManager.SelectFighter(fighter);

    internal int GetCurrentTurnIndex() => turnManager.GetCurrentTurnIndex();
}
