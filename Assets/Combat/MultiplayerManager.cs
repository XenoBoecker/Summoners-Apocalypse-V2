using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.XR;

public class MultiplayerManager : MonoBehaviourPun
{
    CombatManager combatManager;
    CombatUI combatUI;
    EnemyAI enemyAI;

    int localPlayerTeam;

    bool isPvP;

    private void Awake()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.PlayerList.Length > 1) isPvP = true;
        if (!isPvP) return;

        print("We are connected!");
        
        combatManager = GameObject.FindObjectOfType<CombatManager>();
        combatUI = GameObject.FindObjectOfType<CombatUI>();
        enemyAI = GameObject.FindObjectOfType<EnemyAI>();
        
        enemyAI.gameObject.SetActive(false);
        SetLocalPlayerTeam();

        combatManager.OnAbilityTargetsSelected.AddListener(OnAbilityTargetsSelected);
        
        combatUI.localPlayerTeam = localPlayerTeam;
        print("localPlayerTeam: " + localPlayerTeam);
    }

    private void OnAbilityTargetsSelected()
    {
        print("Multiplayer: OnAbilityTargetsSelected");
        List<Fighter> targets = combatManager.GetCurrentAbilityTargets();
    
        int[] targetIndices = new int[targets.Count];
    
        for (int i = 0; i < targets.Count; i++)
        {
            targetIndices[i] = combatManager.GetAllFighters().IndexOf(targets[i]);
            print("TargetIndex: " + targetIndices[i]);
        }
    
        byte[] targetBytes = new byte[targetIndices.Length * sizeof(int)];
    
        for (int i = 0; i < targetIndices.Length; i++)
        {
            byte[] intBytes = BitConverter.GetBytes(targetIndices[i]);
            Buffer.BlockCopy(intBytes, 0, targetBytes, i * sizeof(int), sizeof(int));
        }

        int[] targetIndicesNew = new int[targetBytes.Length / sizeof(int)];
        for (int i = 0; i < targetIndices.Length; i++)
        {
            targetIndicesNew[i] = BitConverter.ToInt32(targetBytes, i * sizeof(int));
            print("Converted TargetIndex: " + targetIndicesNew[i]);
        }

        photonView.RPC("SetTargets", RpcTarget.Others, targetBytes);
    
        byte[] abilityBytes = BitConverter.GetBytes(combatManager.GetSelectedAbilityIndex());
    
        photonView.RPC("SetAbility", RpcTarget.Others, abilityBytes);
    
        photonView.RPC("UseAbility", RpcTarget.Others);
    }
    
    [PunRPC]
    void SetTargets(byte[] bytes)
    {
        int[] targetIndices = new int[bytes.Length / sizeof(int)];
        for (int i = 0; i < targetIndices.Length; i++)
        {
            targetIndices[i] = BitConverter.ToInt32(bytes, i * sizeof(int));
            print("Received Converted TargetIndex: " + targetIndices[i]);
        }
    
        for (int i = 0; i < targetIndices.Length; i++)
        {
            combatManager.
                AddAbilityTargets(
                combatManager.
                GetAllFighters()
                [targetIndices[i]]);
        }
    }

    [PunRPC]
    void SetAbility(byte[] bytes)
    {
        int abilityIndex = BitConverter.ToInt32(bytes, 0);
        combatManager.SetSelectedAbility(abilityIndex);
    }

    [PunRPC]
    void UseAbility()
    {
        print("Send UseAbility");
        combatManager.CastSelectedAbilityOnSelectedTargets();
    }

    bool IsMyTurn()
    {
        if (combatManager.GetCurrentTurnFighter().team == localPlayerTeam) return true;
        else return false;
    }

    void SetLocalPlayerTeam()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (PhotonNetwork.PlayerList[i] == PhotonNetwork.LocalPlayer)
            {
                localPlayerTeam = i;
            }
        }
    }
}
