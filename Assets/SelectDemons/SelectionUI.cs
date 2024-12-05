using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectionUI : MonoBehaviour
{
    [SerializeField] FighterShowObject[] selectedFighterShowObjects;
    List<FighterShowObject> myFighterShowObjects = new List<FighterShowObject>();
    [SerializeField] FighterShowObject fighterShowPrefab;
    [SerializeField] Transform myDemonsParent;

    [SerializeField] string combatSceneName = "Combat";

    DemonSelectionManager demonSelectionManager;
    MultiplayerSelectioManager multiplayerSelectioManager;

    private void Awake()
    {
        demonSelectionManager = GetComponent<DemonSelectionManager>();
        multiplayerSelectioManager = GetComponent<MultiplayerSelectioManager>();

        demonSelectionManager.OnSelectedFightersChanged.AddListener(ShowSelectedFighters);
        demonSelectionManager.OnSelectedFightersChanged.AddListener(ShowMyFighters);

        multiplayerSelectioManager.onAllFightersSelected += StartCombat;

    }

    private void Start()
    {
        foreach (FighterShowObject fighterShowObject in selectedFighterShowObjects)
        {
            fighterShowObject.OnSelectFighter += OnUnselectFighter;
        }

        ShowSelectedFighters();
        ShowMyFighters();
    }

    private void Update() //TODO weg damit
    {

        if (Input.GetKeyDown(KeyCode.Return)) OnConfirmSelection();
    }


    void ShowSelectedFighters()
    {
        for (int i = 0; i < selectedFighterShowObjects.Length; i++)
        {
            if(i < demonSelectionManager.selectedFighters.Count)
            {
                selectedFighterShowObjects[i].SetFighter(demonSelectionManager.selectedFighters[i]);
                selectedFighterShowObjects[i].SetTargetHighlightActive(true);
            }
            else
            {
                selectedFighterShowObjects[i].SetFighter(null);
                selectedFighterShowObjects[i].SetTargetHighlightActive(false);
            }
        }
    }

    void ShowMyFighters()
    {
        List<Fighter> myFighters = demonSelectionManager.GetMyFighters(PhotonNetwork.LocalPlayer.NickName);
        
        SpawnFighterShowObjects(myFighters);

        for (int i = 0; i < myFighterShowObjects.Count; i++)
        {
            if (i >= myFighters.Count)
            {
                myFighterShowObjects[i].gameObject.SetActive(false);
            }
            else
            {
                myFighterShowObjects[i].gameObject.SetActive(true);
                myFighterShowObjects[i].SetFighter(myFighters[i]);
            }
        }
    }

    private void SpawnFighterShowObjects(List<Fighter> myFighters)
    {
        int diff = myFighters.Count - myFighterShowObjects.Count;

        if (diff > 0)
        {
            for (int i = 0; i < diff; i++)
            {
                FighterShowObject newFighterShowObject = Instantiate(fighterShowPrefab, myDemonsParent);
                myFighterShowObjects.Add(newFighterShowObject);
                newFighterShowObject.OnSelectFighter += OnSelectFighter;
                newFighterShowObject.SetClickable(true);
            }
        }
    }

    void OnSelectFighter(Fighter fighter)
    {
        demonSelectionManager.SelectFighter(fighter);
    }

    void OnUnselectFighter(Fighter fighter)
    {
        demonSelectionManager.UnselectFighter(fighter);
    }
    public void OnClickSummonDemon()
    {
        SceneManager.LoadScene("CreateDemons");
    }

    public void OnConfirmSelection()
    {
        demonSelectionManager.SaveSelection();

        if (!demonSelectionManager.isPvP)
        {
            StartCombat();
        }
    }

    public void StartCombat()
    {
        SceneManager.LoadScene(combatSceneName);
    }
}