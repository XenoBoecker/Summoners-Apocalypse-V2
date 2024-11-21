using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DemonSelectionManager : MonoBehaviour
{
    public bool isPvP { get; private set; }

    public int maxFighters = 4;

    public List<Fighter> selectedFighters = new List<Fighter>();

    public UnityEvent OnSelectedFightersChanged;

    ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();

    private void Start()
    {
        isPvP = PhotonNetwork.IsConnected && PhotonNetwork.PlayerList.Length > 1;
    }

    public void SelectFighter(Fighter fighter)
    {
        if (selectedFighters.Count >= maxFighters) return;

        foreach (Fighter selectedFighter in selectedFighters)
        {
            if(selectedFighter.fighterData.fighterID == fighter.fighterData.fighterID)
            {
                return;
            }
        }

        selectedFighters.Add(fighter);

        OnSelectedFightersChanged.Invoke();
    }

    public void UnselectFighter(Fighter fighter)
    {
        selectedFighters.Remove(fighter);

        OnSelectedFightersChanged.Invoke();
    }

    public List<Fighter> GetMyFighters(string ownerName)
    {
        return SaveLoad.LoadMyFighters(ownerName);
    }

    internal void SaveSelection()
    {
        if (isPvP)
        {

            playerProperties["PlayerSelectedDemons"] = selectedFighters.ToArray();

            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
        }
        else
        {
            PlayerPrefs.SetInt("selectedFighterCount_0", selectedFighters.Count);
            for (int i = 0; i < selectedFighters.Count; i++)
            {
                PlayerPrefs.SetInt("player_0_selectedFighterID_" + i, selectedFighters[i].fighterData.fighterID);
            }
        }
    }
}
