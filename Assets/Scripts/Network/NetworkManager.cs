using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static Dictionary<int, PlayerManager> Players = new Dictionary<int, PlayerManager>();
    public static Dictionary<int, NetworkGameObject> TrackedObjects = new Dictionary<int, NetworkGameObject>();

    public GameObject LocalPlayerPrefab;
    public GameObject PlayerPrefab;

    public void SpawnPlayer(int _id, string _username, Vector3 _position, Quaternion _rotation)
    {
        GameObject _player;
        if (_id == Client.Instance.MyID)
        {
            _player = GameObject.Instantiate(LocalPlayerPrefab, _position, _rotation);
        }
        else
        {
            _player = GameObject.Instantiate(PlayerPrefab, _position, _rotation);
        }
        if (Client.Instance.IsHost)
        {
            UpdateAllNetworkObjectPositions(_id);
        }
        _player.transform.name = _username + ":ID:" + _id;
        _player.GetComponent<PlayerManager>().ID = _id;
        _player.GetComponent<PlayerManager>().UserName = _username;

        Players.Add(_id, _player.GetComponent<PlayerManager>());
    }
    void OnApplicationQuit()
    {
        Debug.Log("Disconnecting " + Client.Instance.MyID);
        ClientSend.SendDisconnect(); // Tell the server we bailing
    }

    public void MovePlayer(int _id, Vector3 _pos, Quaternion _rot)
    {
        if (_id != Client.Instance.MyID && Players.ContainsKey(_id))
        {
            Players[_id].SetPositionAndRot(_pos, _rot);
        }
    }
    public void MoveObject(int _id,int _objectNetID, Vector3 _pos, Quaternion _rot)
    {
        if (_id != Client.Instance.MyID && Players.ContainsKey(_id))
        {
            TrackedObjects[_objectNetID].SetPositionAndRot(_pos,_rot);
        }
    }

    public void SetObjectLocation(int _id, int _objectNetID, Vector3 _pos, Quaternion _rot)
    {
        Debug.Log("Being told to move this fella");
        if (TrackedObjects.ContainsKey(_objectNetID))
        {
            TrackedObjects[_objectNetID].SetLocation(_pos, _rot);
        }
    }
    public void UpdateAllNetworkObjectPositions(int _clientID)
    {
        foreach(var trackedObject in TrackedObjects.Values)
        {
            ClientSend.SetObjectPosition(_clientID, trackedObject.NetworkID,trackedObject.transform.position,trackedObject.transform.rotation);
        }
    }

    public void RemovePlayer(int _id) // removes player from game
    {
        if (_id != Client.Instance.MyID)
        {
            GameObject playerObj = Players[_id].gameObject; // we will MURDER THE PLAYER
            Players.Remove(_id);
            GameObject.Destroy(playerObj);

            if (Players.Count == 1)
            {
                Client.Instance.IsHost = true; // we the first peeps so host
            }
            Debug.Log("Player with id of " + _id + " Has disconnected");
        }

    }

    #region singleton
    public static NetworkManager Instance;

    private void Awake()
    {
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
