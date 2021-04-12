using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public GameObject StartMenu;
    public InputField UserNameField;

    public GameObject CreateJoinRoomCanvas;

    public InputField RoomNameInputField;
    public Text ErrorText;


    public void CreateRoom()
    {
        string roomName = RoomNameInputField.text;

        if(String.IsNullOrEmpty(roomName))
        {
            ErrorText.text = "Please enter a room name";
        }
        else
        {
            ClientSend.CreateRoom(roomName);
        }
    }


    public void ConnectToServer()
    {   
        StartMenu.SetActive(false);
        UserNameField.interactable = false;
        Client.Instance.Name = UserNameField.text;
        Client.Instance.ConnectToServer();
        CreateJoinRoomCanvas.SetActive(true);
    }

    #region singleton
    public static UIManager Instance;

    private void Awake()
    {
        CreateJoinRoomCanvas.SetActive(false);
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Client script already exist destorying the one attatched to " + transform.name);
            Destroy(this);
        }
    }

    #endregion
}
