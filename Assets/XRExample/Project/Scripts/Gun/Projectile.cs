using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Attatch to projectile bullet for hit detection
/// </summary>
namespace EpicXRCrossPlatformInput
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public class Projectile : MonoBehaviour
    {
        public UnityEvent ProjectileCollided;
        public LayerMask ProjectileLayerMask;

        public float BulletLifeTime = 5.0f;
        public void OnCollisionEnter(Collision collision)
        {
            int mask = 1 << collision.gameObject.layer;
            int tmp = ProjectileLayerMask & mask;
            bool result = tmp != 0;
            if (result)
            {
                ProjectileCollided.Invoke(); // Calls a unity event when it hits do whatever here
            }
        }

        private void Start()
        {
            StartCoroutine(WaitToDestroy());
        }
        private IEnumerator WaitToDestroy()
        {
            yield return new WaitForSeconds(BulletLifeTime);
            GameObject.Destroy(this.gameObject);
        }
        public void OnHitDebug()
        {
            Destroy(this.gameObject);
        }
    }
}

