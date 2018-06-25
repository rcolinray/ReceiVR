using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootableLight : MonoBehaviour
{

    public enum LightType
    {
        AirplaneBlink,
        Normal,
        Flicker,
    }

    public LightType lightType = LightType.Normal;

    public GameObject destroyEffect;
    public Color lightColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    public bool destroyed = false;

    private float blinkDelay = 0.0f;
    private float lightAmount = 1.0f;

    // Update is called once per frame
    void Update()
    {
        if (!destroyed)
        {
            switch (lightType)
            {
                case LightType.AirplaneBlink:
                    if (blinkDelay <= 0.0f)
                    {
                        blinkDelay = 1.0f;
                        if (lightAmount == 1.0f)
                        {
                            lightAmount = 0.0f;
                        }
                        else
                        {
                            lightAmount = 1.0f;
                        }
                    }
                    blinkDelay -= Time.deltaTime;
                    break;
            }
        }

        Color combinedColor = lightColor * lightAmount;
        Light[] lights = gameObject.GetComponentsInChildren<Light>();
        foreach (Light light in lights)
        {
            light.color = combinedColor;
        }

        MeshRenderer[] renderers = gameObject.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in renderers)
        {
            renderer.material.SetColor("_Illum", combinedColor);
            if (renderer.gameObject.name == "shade")
            {
                renderer.material.SetColor("_Illum", combinedColor * 0.5f);
            }
        }
    }

    void WasShot(GameObject obj, Vector3 pos, Vector3 vel)
    {
        if (!destroyed)
        {
            destroyed = true;
            lightAmount = 0.0f;
            Instantiate(destroyEffect, transform.Find("bulb").position, Quaternion.identity);
        }

        if (obj != null)
        {
            Collider collider = obj.GetComponent<Collider>();
            if (collider != null && collider.material.name == "glass (Instance)")
            {
                Destroy(obj);
            }
        }
    }
}
