using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour {

    // Use this for initialization
    void Start () {
        Transform enemies = transform.Find("enemies");
        if (enemies != null) {
            foreach (Transform child in enemies) {
                if (Random.Range(0.0f, 1.0f) < 0.9f) {
                    Destroy(child.gameObject);
                }
            }
        }

        Transform players = transform.Find("player_spawn");
        if (players != null) {
            int saveIndex = Random.Range(0, players.childCount);
            int index = 0;

            foreach (Transform child in players) {
                if (index != saveIndex) {
                    Destroy(child.gameObject);
                }
                index++;
            }
        }

        Transform items = transform.Find("items");
        if (items != null) {
            foreach (Transform child in items) {
                if (Random.Range(0.0f, 1.0f) < 0.9f) {
                    Destroy(child.gameObject);
                }
            }
        }
    }
}
