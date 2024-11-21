using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MultiplayerSelectioManager;

public class MultiplayerSelectioManager : MonoBehaviourPunCallbacks
{
    int maxPlayers = 2;
    int maxFighters = 4;

    Fighter[][] allSelectedFighters = new Fighter[2][];

    int doneWithSelectionPlayerCount;

    public delegate void OnAllFightersSelected();
    public OnAllFightersSelected onAllFightersSelected;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("PlayerSelectedDemons"))
        {
            OnPlayerDemonSelectionChanged(targetPlayer, changedProps);
        }
    }

    private void OnPlayerDemonSelectionChanged(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        Fighter[] selectedFighters = (Fighter[])changedProps["PlayerSelectedDemons"];

        int playerIndex = 0;
        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            if (player.Value != targetPlayer)
            {
                playerIndex++;
                continue;
            }

            allSelectedFighters[playerIndex] = (Fighter[])changedProps["PlayerSelectedDemons"];

            break;
        }

        doneWithSelectionPlayerCount++;

        if (doneWithSelectionPlayerCount >= PhotonNetwork.CurrentRoom.Players.Count)
        {
            onAllFightersSelected.Invoke();

            SaveSelectedFightersInPlayerPrefs();

            doneWithSelectionPlayerCount = 0;
        }
    }

    private void SaveSelectedFightersInPlayerPrefs()
    {
        for (int i = 0; i < allSelectedFighters.Length; i++)
        {
            PlayerPrefs.SetInt("selectedFighterCount_" + i, allSelectedFighters[i].Length);

            for (int j = 0; j < allSelectedFighters[i].Length; j++)
            {
                Fighter fighter = allSelectedFighters[i][j];

                if (fighter.ownerName != PhotonNetwork.LocalPlayer.NickName)
                {
                    SaveLoad.SaveToJson(fighter);
                }

                PlayerPrefs.SetInt("player_" + i + "_selectedFighterID_" + j, fighter.fighterData.fighterID);
            }
        }
    }
}
