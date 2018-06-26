using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCreator : MonoBehaviour
{

    public GameObject[] levelTiles;

    private List<Light> shadowedLights = new List<Light>();
    private List<int> tiles = new List<int>();

    // Use this for initialization
    void Start()
    {
        // Create the player position
        SpawnTile(0, 0.0f, true);

        // Create tiles around the player
        // for (int i = -3; i <= 3; i++)
        // {
        //     CreateTileIfNeeded(i);
        // }
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: VR camera
        Transform mainCamera = GameObject.Find("Camera (eye)").transform;
        // float tileX = mainCamera.position.z / 20.0f + 0.5f;
        // for (int i = -2; i <= 2; i++)
        // {
        //     // TODO: confirm that int is the correct type here
        //     CreateTileIfNeeded((int)tileX + i);
        // }
        foreach (Light light in shadowedLights)
        {
            if (light == null)
            {
                Debug.Log("LIGHT IS MISSING");
            }
            else
            {
                float shadowedAmount = Vector3.Distance(mainCamera.position, light.gameObject.transform.position);
                float shadowThreshold = Mathf.Min(30, light.range * 2.0f);
                float fadeThreshold = shadowThreshold * 0.75f;
                if (shadowedAmount < shadowThreshold)
                {
                    light.shadows = LightShadows.Hard;
                    light.shadowStrength = Mathf.Min(1.0f, 1.0f - (fadeThreshold - shadowedAmount) / (fadeThreshold - shadowThreshold));
                }
                else
                {
                    light.shadows = LightShadows.None;
                }
            }
        }
    }

    void SpawnTile(int where, float challenge, bool player)
    {
        GameObject levelObj = levelTiles[Random.Range(0, levelTiles.Length)];
        GameObject level = new GameObject(levelObj.name + " (Clone)");

        foreach (Transform child in levelObj.transform)
        {
            string childName = child.gameObject.name;
            if (childName != "enemies" && childName != "player_spawn" && childName != "items")
            {
                Vector3 pos = new Vector3(0, 0, where * 20) + child.localPosition;
                GameObject childObj = Instantiate(child.gameObject, pos, child.localRotation);
                childObj.transform.parent = level.transform;
            }
        }

        Transform enemies = levelObj.transform.Find("enemies");
        if (enemies != null)
        {
            foreach (Transform child in enemies)
            {
                if (Random.Range(0.0f, 1.0f) <= challenge)
                {
                    Vector3 pos = new Vector3(0, 0, where * 20) + child.localPosition + enemies.position;
                    GameObject childObj = Instantiate(child.gameObject, pos, child.localRotation);
                    childObj.transform.parent = level.transform;
                }
            }
        }

        Transform items = levelObj.transform.Find("items");
        float itemChallenge = player ? challenge + 0.3f : challenge;
        if (items != null)
        {
            foreach (Transform child in items)
            {
                if (Random.Range(0.0f, 1.0f) <= itemChallenge)
                {
                    Vector3 pos = new Vector3(0, 0, where * 20) + child.localPosition + items.position;
                    GameObject childObj = Instantiate(child.gameObject, pos, child.localRotation);
                    childObj.transform.parent = level.transform;
                }
            }
        }

        if (player)
        {
            Transform players = levelObj.transform.Find("player_spawn");
            if (players != null)
            {
                int saveIndex = Random.Range(0, players.childCount);
                Transform child = players.GetChild(saveIndex);
                Vector3 pos = new Vector3(0, 0, where * 20) + child.localPosition + players.localPosition;
                GameObject childObj = Instantiate(child.gameObject, pos, child.localRotation);
                childObj.transform.parent = level.transform;
                childObj.name = "Player";
            }
        }

        level.transform.parent = gameObject.transform;

        Light[] lights = GetComponentsInChildren<Light>();
        foreach (Light light in lights)
        {
            if (light.enabled && light.shadows == LightShadows.Hard)
            {
                shadowedLights.Add(light);
            }
        }
        tiles.Add(where);
    }

    void CreateTileIfNeeded(int which)
    {
        if (!tiles.Contains(which))
        {
            SpawnTile(which, Mathf.Min(0.5f, 0.1f * Mathf.Abs(which)), false);
        }
    }
}