using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public GameObject StartMenu;
    public InputField UserNameField;

    public void ConnectToServer()
    {   
        StartMenu.SetActive(false);
        UserNameField.interactable = false;
        Client.Instance.ConnectToServer();
    }

    #region singleton
    public static UIManager Instance;

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
