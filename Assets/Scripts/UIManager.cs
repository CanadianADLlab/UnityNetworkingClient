﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public GameObject StartMenu;
    public InputField UserNameField;

    public GameObject CreateJoinRoomCanvas;

    public InputField RoomNameInputField;
    public Text ErrorText;

    public Dropdown RoomsDropdown;
    public Dictionary<string, int> Rooms = new Dictionary<string, int>(); //Normally it would be int,string but because the roomname is what gets selected when the dropdown is clicked we do it backwards :)

    public int RoomToJoinID = -99;


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


    public void JoinServer()
    {
        if(RoomToJoinID == -99) // default value is -99 because why not
        {
            ErrorText.text = "Please Select a room";
        }
        else
        {
            Debug.Log("Selected room is ! " + RoomToJoinID);
        }
    }

    public void OnRoomValueChanged()
    {
        RoomToJoinID = Rooms[RoomsDropdown.options[RoomsDropdown.value].text];
    }

    public void AddRoomToDropDown(int _roomID,string _roomName,int _roomSize,int _roomSpacesOccupied)
    {
        var optionData = new Dropdown.OptionData(_roomName);
        Rooms.Add(_roomName, _roomID);
        RoomsDropdown.options.Add(optionData);
        if (RoomsDropdown.options.Count == 1) // first option
        {
            var listAvailableStrings = RoomsDropdown.options.Select(option => option.text).ToList();
            RoomsDropdown.value = listAvailableStrings.IndexOf(_roomName); ; // setting the first value to be selectedidk why its 1 or why my select thing doesn't work
            RoomToJoinID = _roomID;
            Debug.Log("Selecting " + RoomsDropdown.value);
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
