using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderCheckSecond : MonoBehaviour
{
    public GamePlaySecond player;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Box"))
        {
            player.startFly = false;
            player.playerRig.isKinematic = false;
            Destroy(player.part);
        }
    }
}
