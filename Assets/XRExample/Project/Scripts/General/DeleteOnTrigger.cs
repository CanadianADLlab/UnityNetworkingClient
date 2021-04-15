using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Deletes self when layer enters into trigger
/// </summary>
public class DeleteOnTrigger : MonoBehaviour
{
    public int LayerToTriggerDelete;

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerToTriggerDelete)
        {
            Destroy(this.gameObject);
        }
    }
}
