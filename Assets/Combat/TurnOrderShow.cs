using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOrderShow : MonoBehaviour
{
    [SerializeField] GameObject fighterIconShowPrefab;
    [SerializeField] GameObject fighterIconShowContainer;

    [SerializeField] int maxShowObjectCount = 3;

    List<FighterIconShowObject> fighterIconShowObjects = new List<FighterIconShowObject>();

    public void ShowTurnOrder(List<Fighter> fighters, int currentTurnIndex)
    {
        if (fighterIconShowObjects.Count == 0) SpawnFighterIconShowObjects();
        
        for (int i = 0; i < maxShowObjectCount; i++)
        {
            if (i >= fighters.Count)
            {
                fighterIconShowObjects[i].gameObject.SetActive(false);
                return;
            }

            fighterIconShowObjects[i].gameObject.SetActive(true);

            int fighterIndex = (i + currentTurnIndex) % fighters.Count;

            fighterIconShowObjects[i].SetFighter(fighters[fighterIndex]);
        }
    }

    private void SpawnFighterIconShowObjects()
    {
        for (int i = 0; i < maxShowObjectCount; i++)
        {
            GameObject temp = (GameObject)Instantiate(fighterIconShowPrefab, fighterIconShowContainer.transform);
            fighterIconShowObjects.Add(temp.GetComponent<FighterIconShowObject>());
        }
    }
}
