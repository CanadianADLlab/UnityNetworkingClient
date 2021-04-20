using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static Dictionary<int, PlayerManager> Players = new Dictionary<int, PlayerManager>();
    public static Dictionary<int, NetworkGameObject> TrackedObjects = new Dictionary<int, NetworkGameObject>();

    public GameObject LocalPlayerPrefab;
    public GameObject PlayerPrefab;

    public GameObject LocalVRPlayerPrefab;
    public GameObject VRPlayerPrefab;

    public void SpawnPlayer(int _id, string _username, Vector3 _position, Quaternion _rotation, bool _isVR)
    {
        GameObject _player;
        Debug.Log("this should be a vr guy is it  " + _isVR);
        if (!_isVR)
        {
            if (_id == Client.Instance.MyID)
            {
                _player = GameObject.Instantiate(LocalPlayerPrefab, _position, _rotation);
            }
            else
            {
                _player = GameObject.Instantiate(PlayerPrefab, _position, _rotation);
            }
        }
        else
        {
            Debug.Log("spawning vr player fagot");
            if (_id == Client.Instance.MyID)
            {
                _player = GameObject.Instantiate(LocalVRPlayerPrefab, _position, _rotation);
            }
            else
            {
                _player = GameObject.Instantiate(VRPlayerPrefab, _position, _rotation);
            }
        }
        if (Client.Instance.IsHost)
        {
            UpdateAllNetworkObjectPositions(_id);
        }
        _player.transform.name = _username + ":ID:" + _id;
        _player.GetComponent<PlayerManager>().ID = _id;
        _player.GetComponent<PlayerManager>().UserName = _username;

        Players.Add(_id, _player.GetComponent<PlayerManager>());

        if (Players.ContainsKey(Client.Instance.MyID)) // if I'm the new player there is a chance my id is not set so let's just double check 
        {
            if (!Client.Instance.IsVR)
            {
                ClientSend.SendPlayerValues(Players[Client.Instance.MyID].transform.position, Players[Client.Instance.MyID].transform.rotation, false); // Send my position value so the new player knows where we are (movement only happens when i move) and lerping is set to false so it just moves the player there
            }
            else
            {
                ClientSend.SendPlayerValues(Players[Client.Instance.MyID].transform.position, Players[Client.Instance.MyID].transform.rotation, Players[Client.Instance.MyID].LeftHand.position, Players[Client.Instance.MyID].LeftHand.rotation, Players[Client.Instance.MyID].RightHand.position, Players[Client.Instance.MyID].RightHand.rotation, false); // Send my position value so the new player knows where we are (movement only happens when i move) and lerping is set to false so it just moves the player there
            }
        }
    }
    void OnApplicationQuit()
    {
        Debug.Log("Disconnecting " + Client.Instance.MyID);
        ClientSend.SendDisconnect(); // Tell the server we bailing
        Client.Instance.Disconnect();
    }

    public void MovePlayer(int _id, Vector3 _pos, Quaternion _rot, bool _lerp)
    {
        if (_id != Client.Instance.MyID && Players.ContainsKey(_id))
        {
            Players[_id].SetPositionAndRot(_pos, _rot, _lerp);
        }
    }

    public void MovePlayer(int _id, Vector3 _pos, Quaternion _rot, Vector3 _leftHandPos, Quaternion _leftHandRot, Vector3 _rightHandPos, Quaternion _rightHandRot, bool _lerp)
    {
        if (_id != Client.Instance.MyID && Players.ContainsKey(_id))
        {
            Players[_id].SetPositionAndRot(_pos,_rot,_leftHandPos, _leftHandRot, _rightHandPos, _rightHandRot, _lerp);
        }
    }
    public void MoveObject(int _id, int _objectNetID, Vector3 _pos, Quaternion _rot)
    {
        if (_id != Client.Instance.MyID && Players.ContainsKey(_id))
        {
            TrackedObjects[_objectNetID].SetPositionAndRot(_pos, _rot);
        }
    }

    public void SetObjectLocation(int _id, int _objectNetID, Vector3 _pos, Quaternion _rot)
    {
        if (TrackedObjects.ContainsKey(_objectNetID))
        {
            TrackedObjects[_objectNetID].SetLocation(_pos, _rot);
        }
    }
    public void UpdateAllNetworkObjectPositions(int _clientID)
    {
        foreach (var trackedObject in TrackedObjects.Values)
        {
            ClientSend.SetObjectPosition(_clientID, trackedObject.NetworkID, trackedObject.transform.position, trackedObject.transform.rotation);
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
