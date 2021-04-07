using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();

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
        _player.transform.name = _username + ":ID:"+_id;

        _player.GetComponent<PlayerManager>().ID = _id;
        _player.GetComponent<PlayerManager>().UserName = _username;

        players.Add(_id, _player.GetComponent<PlayerManager>());
     
    }
    #region singleton
    public static GameManager Instance;

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
