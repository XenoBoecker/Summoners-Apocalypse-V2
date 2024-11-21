using UnityEngine;
using UnityEngine.Events;

public class CombatTurnManager
{
    CombatFighterStorage fighterStorage;

    Fighter currentTurnFighter;

    public UnityEvent OnStartTurn = new UnityEvent();

    internal Fighter GetCurrentTurnFighter() => currentTurnFighter;

    public CombatTurnManager(CombatFighterStorage fighterStorage)
    {
        this.fighterStorage = fighterStorage;

        currentTurnFighter = fighterStorage.GetAllFighters()[0];
    }

    private void StartNextTurn()
    {
        currentTurnFighter = fighterStorage.GetNextFighter(currentTurnFighter);

        Debug.Log("Start turn of: " + GetCurrentTurnFighter().fighterName);

        StartTurn();
    }

    public void StartTurn()
    {
        OnStartTurn.Invoke();
    }

    public void EndTurn()
    {
        // Debug.Log("EndTurn");
        if (fighterStorage.FightOver())
        {

            return;
        }

        StartNextTurn();
    }

    internal int GetCurrentTurnIndex()
    {
        Debug.Log("fighter count: " + fighterStorage.GetAllFighters().Count);
        Debug.Log("CurrentTurnFighter: " + GetCurrentTurnFighter().fighterName);


        return fighterStorage.GetAllFighters().IndexOf(GetCurrentTurnFighter());
    }
}
