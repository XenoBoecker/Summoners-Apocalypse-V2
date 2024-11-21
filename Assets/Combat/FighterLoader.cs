using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FighterLoader
{
    bool spawnTestPlayers = true;

    int maxPlayerCount = 0;
    int maxFightersPerTeam = 0;
    internal void SetMaxPlayerCount(int maxPlayerCount) => this.maxPlayerCount = maxPlayerCount;
    internal void SetMaxFightersPerTeam(int maxFightersPerTeam) => this.maxFightersPerTeam = maxFightersPerTeam;

    public FighterLoader(int maxPlayerCount, int maxFightersPerTeam)
    {
        SetMaxPlayerCount(maxPlayerCount);
        SetMaxFightersPerTeam(maxFightersPerTeam);
    }
    internal List<Fighter> GetAllFighters()
    {
        List<Fighter> fighters = GetPlayerSelectedFighters();

        if (spawnTestPlayers)
        {
            List<Fighter> testFighters = GetTestFighters(maxFightersPerTeam, 1);

            foreach (Fighter fighter in testFighters)
            {
                fighters.Add(fighter);
            }
        }

        return fighters;
    }

    List<Fighter> GetTestFighters(int count, int teamNbr)
    {
        List<Fighter> fighters = new List<Fighter>();

        FighterData testFighter = CreateTestFighter();
        for (int i = 0; i < count; i++)
        {
            fighters.Add(new Fighter(testFighter));
            fighters[i].team = teamNbr;
            fighters[i].fighterName = "Fighter " + i;
        }

        return fighters;
    }


    private List<Fighter> GetPlayerSelectedFighters()
    {

        List<Fighter> playerFighters = new List<Fighter>();


        for (int i = 0; i < maxPlayerCount; i++)
        {

            if (PhotonNetwork.IsConnected)
            {
                if (PhotonNetwork.PlayerList.Length > 1) spawnTestPlayers = false;

                List<Fighter> fighters = SaveLoad.LoadMyFighters(PhotonNetwork.PlayerList[i].NickName);
                foreach (Fighter fighter in fighters)
                {
                    fighter.team = i;
                }
            
                playerFighters.AddRange(fighters);
            }
            else playerFighters.AddRange(SaveLoad.LoadMyFighters("TestPlayer"));
            
            continue;
            for (int j = 0; j < PlayerPrefs.GetInt("selectedFighterCount_" + i); j++)
            {
                int index = PlayerPrefs.GetInt("player_" + i + "_selectedFighterID_" + j);

                if (PhotonNetwork.IsConnected)
                {
                    if(PhotonNetwork.PlayerList.Length > 1) spawnTestPlayers = false;

                    Debug.Log("Load fighter: " + index + " of player " + i);

                    Debug.Log("Fighter Count: " + SaveLoad.LoadMyFighters(PhotonNetwork.
                        PlayerList[i].NickName).Count);
                    
                    Fighter fighter = SaveLoad.LoadMyFighters(PhotonNetwork.
                        PlayerList[i].NickName)
                        [index];
                    fighter.team = i;

                    playerFighters.Add(fighter);
                }
                else playerFighters.Add(SaveLoad.LoadMyFighters("TestPlayer")[index]);
            }
        }

        return playerFighters;
    }

    private FighterData CreateTestFighter()
    {
        List<Ability> abilities = new List<Ability>();
        TargetType[] fireballTargetTypes = new TargetType[] { TargetType.Enemy };
        SpecialEffect[] fireBallSpecialEffects = new SpecialEffect[] { SpecialEffect.None };
        abilities.Add(new Ability(fireballTargetTypes, fireBallSpecialEffects, 20, 1, "Fireball"));

        TargetType[] healTargetTypes = new TargetType[] { TargetType.Self };
        SpecialEffect[] healSpecialEffects = new SpecialEffect[] { SpecialEffect.None };
        abilities.Add(new Ability(healTargetTypes, healSpecialEffects, -10, 1, "HealSelf"));

        FighterData testFighter = new FighterData(50, abilities, 50, 0);
        return testFighter;
    }
}