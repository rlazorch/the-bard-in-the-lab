using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicZoneConrol : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player") {
            GameObject.Find("/MusicManager").GetComponent<MusicPlayerSync>().swapToSub(0);
            GameObject.Find("/AmbienceManager").GetComponent<MusicPlayerSync>().swapToSub(0);
        }
    }
    void OnTriggerExit2D(Collider2D  other) {
        if (other.gameObject.tag == "Player") {
            GameObject.Find("/MusicManager").GetComponent<MusicPlayerSync>().swapToSuper();
            GameObject.Find("/AmbienceManager").GetComponent<MusicPlayerSync>().swapToSuper();
        }
    }
}
