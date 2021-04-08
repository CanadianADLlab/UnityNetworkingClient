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
        _player.transform.name = _username + ":ID:" + _id;

        _player.GetComponent<PlayerManager>().ID = _id;
        _player.GetComponent<PlayerManager>().UserName = _username;

        Players.Add(_id, _player.GetComponent<PlayerManager>());

    }


    public void MovePlayer(int _id, Vector3 _pos, Quaternion _rot)
    {
        Debug.Log("Moving player with the id of " + _id);
        if (_id != Client.Instance.MyID)
        {
            Players[_id].transform.position = _pos;
            Players[_id].transform.rotation = _rot;
            Debug.Log("Moving player " + _id + " THe pos is  " + _pos);
        }
    }
    public void MoveObject(int _id,int _objectNetID, Vector3 _pos, Quaternion _rot)
    {
        Debug.Log("Moving player with the id of " + _id);
        if (_id != Client.Instance.MyID)
        {
            TrackedObjects[_objectNetID].SetPositionAndRot(_pos,_rot);
            Debug.Log("Moving player " + _id + " THe pos is  " + _pos);
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
