using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_InputField nameInput;
    [SerializeField] TMP_Text connectButtonText;

    [SerializeField] bool testing;

    private void Start()
    {
        if (testing)
        {
            PhotonNetwork.NickName = "TestPlayer";
            connectButtonText.text = "Connecting...";
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            OnClickConnect();
        }
    }

    public void OnClickConnect()
    {
        PhotonNetwork.NickName = nameInput.text;
        connectButtonText.text = "Connecting...";
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene("Lobby");
    }
}
