using UnityEngine;
using System.Collections;

public enum LightType
{
    AIRPLANE_BLINK = 0,
    NORMAL = 1,
    FLICKER = 2
}

[System.Serializable]
public partial class ShootableLight : MonoBehaviour
{
    public GameObject destroy_effect;
    public Color light_color;
    public bool destroyed;
    public LightType light_type;
    private float blink_delay;
    private float light_amount;
    public virtual void WasShot(GameObject obj, Vector3 pos, Vector3 vel)
    {
        if (!this.destroyed)
        {
            this.destroyed = true;
            this.light_amount = 0f;
            UnityEngine.Object.Instantiate(this.destroy_effect, this.transform.Find("bulb").position, Quaternion.identity);
        }
        if ((obj && obj.GetComponent<Collider>()) && (obj.GetComponent<Collider>().material.name == "glass (Instance)"))
        {
            GameObject.Destroy(obj);
        }
    }

    public virtual void Start()
    {
    }

    public virtual void Update()
    {
        if (!this.destroyed)
        {
            switch (this.light_type)
            {
                case LightType.AIRPLANE_BLINK:
                    if (this.blink_delay <= 0f)
                    {
                        this.blink_delay = 1f;
                        if (this.light_amount == 1f)
                        {
                            this.light_amount = 0f;
                        }
                        else
                        {
                            this.light_amount = 1f;
                        }
                    }
                    this.blink_delay = this.blink_delay - Time.deltaTime;
                    break;
            }
        }
        Color combined_color = new Color(this.light_color.r * this.light_amount, this.light_color.g * this.light_amount, this.light_color.b * this.light_amount);
        foreach (Light light in this.gameObject.GetComponentsInChildren(typeof(Light)))
        {
            light.color = combined_color;
        }
        foreach (MeshRenderer renderer in this.gameObject.GetComponentsInChildren(typeof(MeshRenderer)))
        {
            renderer.material.SetColor("_Illum", combined_color);
            if (renderer.gameObject.name == "shade")
            {
                renderer.material.SetColor("_Illum", combined_color * 0.5f);
            }
        }
    }

    public ShootableLight()
    {
        this.light_color = new Color(1, 1, 1);
        this.light_type = LightType.NORMAL;
        this.light_amount = 1f;
    }

}