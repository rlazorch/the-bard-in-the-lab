using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupBehavior : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player") {
            //print("Pickup Collected");
            GameObject.Find("/SFXManager/Pickup").GetComponent<PitchVariedSFX>().PlaySFX();
            Destroy(gameObject);
        }
    }
}
