using System;
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
    public InputField RoomSizeInputField;
    public Text ErrorText;

    public Dropdown RoomsDropdown;
    public Dictionary<string, int> Rooms = new Dictionary<string, int>(); //Normally it would be int,string but because the roomname is what gets selected when the dropdown is clicked we do it backwards :)

    public int MaxRoomSize = 50;
    public int RoomToJoinID = -99;


    public void CreateRoom()
    {
        string roomName = RoomNameInputField.text;
        int roomSize;

        try
        {
            roomSize = int.Parse(RoomSizeInputField.text);
        }
        catch
        {
            ErrorText.text = "Please a valid amount of users";
            return;
        }

        if (String.IsNullOrEmpty(roomName))
        {
            ErrorText.text = "Please enter a room name";
        }
        else if (roomSize > MaxRoomSize)
        {
            ErrorText.text = "Please enter a room size less than  " + MaxRoomSize;
        }
        else
        {
            ClientSend.CreateRoom(roomName, roomSize);
        }
    }

    public void Refresh()
    {
        if (Client.Instance.IsConnected)
        {
            // re init the room list before we fetch more to avoid dups
            RoomsDropdown.options.Clear();
            Rooms = new Dictionary<string, int>(); 

            ClientSend.GetRooms();
        }
    }

    public void JoinRoom()
    {
        if (RoomToJoinID == -99) // default value is -99 because why not
        {
            ErrorText.text = "Please Select a room";
        }
        else
        {
            Debug.Log("Selected room is ! " + RoomToJoinID);
            ClientSend.JoinRoom(RoomToJoinID);
        }
    }

    public void OnRoomValueChanged()
    {
        RoomToJoinID = Rooms[RoomsDropdown.options[RoomsDropdown.value].text];
    }

    public void AddRoomToDropDown(int _roomID, string _roomName, int _roomSize, int _roomSpacesOccupied)
    {

        var roomValue = _roomName + " : " + _roomSpacesOccupied + "/" + _roomSize;
        var optionData = new Dropdown.OptionData(roomValue);
        Rooms.Add(roomValue, _roomID);
        RoomsDropdown.options.Add(optionData);
        if (RoomsDropdown.options.Count == 1) // first option
        {
            var listAvailableStrings = RoomsDropdown.options.Select(option => option.text).ToList();
            RoomsDropdown.value = listAvailableStrings.IndexOf(roomValue); ; // setting the first value to be selectedidk why its 1 or why my select thing doesn't work
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
