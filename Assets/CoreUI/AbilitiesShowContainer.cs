using System;
using System.Collections.Generic;
using UnityEngine;

public class AbilitiesShowContainer : MonoBehaviour
{
    [SerializeField] GameObject abilityShowPrefab;
    [SerializeField] int maxAbilitiesToShow = 4;

    List<AbilityShowObject> abilityShowObjects = new List<AbilityShowObject>();

    public delegate void AbilitySelectedEventHandler(Ability ability);

    public event AbilitySelectedEventHandler AbilitySelected;

    public void SetFighter(Fighter fighter)
    {
        if (abilityShowObjects.Count == 0) SpawnAbilityShowObjects();

        for (int i = 0; i < maxAbilitiesToShow; i++)
        {
            if (i >= fighter.fighterData.abilities.Count) abilityShowObjects[i].gameObject.SetActive(false);
            else
            {
                abilityShowObjects[i].gameObject.SetActive(true);
                abilityShowObjects[i].SetAbility(fighter.fighterData.abilities[i]);
            }
        }
    }

    private void SpawnAbilityShowObjects()
    {
        for (int i = 0; i < maxAbilitiesToShow; i++)
        {
            GameObject temp = (GameObject)Instantiate(abilityShowPrefab, gameObject.transform);
            abilityShowObjects.Add(temp.GetComponent<AbilityShowObject>());
            temp.GetComponent<AbilityShowObject>().AbilitySelected += OnAbilitySelected;
        }
    }

    private void OnAbilitySelected(Ability ability)
    {
        // TODO: temporarily unable all ability select buttons
        AbilitySelected?.Invoke(ability);
    }
}