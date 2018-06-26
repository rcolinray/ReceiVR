using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class ShellCasingScript : MonoBehaviour
{
    public AudioClip[] sound_shell_bounce;
    public bool collided;
    public Vector3 old_pos;
    public float life_time;
    public float glint_delay;
    public float glint_progress;
    private Light glint_light;
    public virtual void PlaySoundFromGroup(object[] group, float volume)
    {
        int which_shot = Random.Range(0, group.Length);
        this.GetComponent<AudioSource>().PlayOneShot((AudioClip) group[which_shot], volume * PlayerPrefs.GetFloat("sound_volume", 1f));
    }

    public virtual void Start()
    {
        this.old_pos = this.transform.position;
        if (this.transform.Find("light_pos"))
        {
            this.glint_light = this.transform.Find("light_pos").GetComponent<Light>();
            this.glint_light.enabled = false;
        }
    }

    public virtual void CollisionSound()
    {
        if (!this.collided)
        {
            this.collided = true;
            this.PlaySoundFromGroup(this.sound_shell_bounce, 0.3f);
        }
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
        if ((this.GetComponent<Rigidbody>() && this.GetComponent<Rigidbody>().IsSleeping()) && this.glint_light)
        {
            if (this.glint_delay == 0f)
            {
                this.glint_delay = Random.Range(1f, 5f);
            }
            this.glint_delay = Mathf.Max(0f, this.glint_delay - Time.deltaTime);
            if (this.glint_delay == 0f)
            {
                this.glint_progress = 1f;
            }
            if (this.glint_progress > 0f)
            {
                this.glint_light.enabled = true;
                this.glint_light.intensity = Mathf.Sin(this.glint_progress * Mathf.PI);
                this.glint_progress = Mathf.Max(0f, this.glint_progress - (Time.deltaTime * 2f));
            }
            else
            {
                this.glint_light.enabled = false;
            }
        }
        this.old_pos = this.transform.position;
    }

    public virtual void OnCollisionEnter(Collision collision)
    {
        this.CollisionSound();
    }

}