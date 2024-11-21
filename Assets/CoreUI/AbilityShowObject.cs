using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

internal class AbilityShowObject : MonoBehaviour
{
    [SerializeField] Ability ability;

    [SerializeField] Image image;
    [SerializeField] TMP_Text nameText;

    public delegate void AbilitySelectedEventHandler(Ability ability);
    
    public event AbilitySelectedEventHandler AbilitySelected;

    internal void SetAbility(Ability ability)
    {
        this.ability = ability;

        ShowAbility();
    }

    private void ShowAbility()
    {
        nameText.text = ability.abilityName + "\n" + ability.damage + "\n" + ability.targetTypes[0] + "\n" + ability.specialEffects[0];
    }

    public void OnClickSelectAbility()
    {
        AbilitySelected?.Invoke(ability);
    }
}
