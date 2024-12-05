using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

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
        Debug.Log("OnPlayerPropertiesUpdate");
        if (changedProps.ContainsKey("PlayerSelectedDemons"))
        {
            OnPlayerDemonSelectionChanged(targetPlayer, changedProps);
        }
    }

    private void OnPlayerDemonSelectionChanged(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        Debug.Log("OnPlayerDemonSelectionChanged");

        Fighter[] selectedFighters = Serialization.DeserializeFighters((byte[])changedProps["PlayerSelectedDemons"]).ToArray();

        int playerIndex = 0;
        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            if (player.Value != targetPlayer)
            {
                playerIndex++;
                continue;
            }

            allSelectedFighters[playerIndex] = selectedFighters;

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
