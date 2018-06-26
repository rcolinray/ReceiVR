using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class tapescript : MonoBehaviour
{
    private float life_time;
    private Vector3 old_pos;
    public virtual void Start()
    {
        this.old_pos = this.transform.position;
    }

    public virtual void Update()
    {
        this.transform.Find("light_obj").GetComponent<Light>().intensity = 1f + Mathf.Sin(Time.time * 2f);
    }

    public virtual void FixedUpdate()
    {
        RaycastHit hit = default(RaycastHit);
        if (((this.GetComponent<Rigidbody>() && !this.GetComponent<Rigidbody>().IsSleeping()) && this.GetComponent<Collider>()) && this.GetComponent<Collider>().enabled)
        {
            this.life_time = this.life_time + Time.deltaTime;
            if (Physics.Linecast(this.old_pos, this.transform.position, out hit, 1))
            {
                this.transform.position = hit.point;
                this.transform.GetComponent<Rigidbody>().velocity = this.transform.GetComponent<Rigidbody>().velocity * -0.3f;
            }
            if (this.life_time > 2f)
            {
                this.GetComponent<Rigidbody>().Sleep();
            }
        }
        this.old_pos = this.transform.position;
    }

}