using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class FlashlightScript : MonoBehaviour
{
    public AnimationCurve battery_curve;
    public AudioClip sound_turn_on;
    public AudioClip sound_turn_off;
    private float kSoundVolume;
    private bool switch_on;
    private float max_battery_life;
    private float battery_life_remaining;
    private float initial_pointlight_intensity;
    private float initial_spotlight_intensity;
    public void Awake()
    {
        this.switch_on = false;// Random.Range(0.0,1.0) < 0.5;
    }

    public void Start()
    {
        this.initial_pointlight_intensity = ((Light)this.transform.Find("Pointlight").gameObject.GetComponent(typeof(Light))).intensity;
        this.initial_spotlight_intensity = ((Light)this.transform.Find("Spotlight").gameObject.GetComponent(typeof(Light))).intensity;
        this.battery_life_remaining = Random.Range(this.max_battery_life * 0.2f, this.max_battery_life);
    }

    public void TurnOn()
    {
        if (!this.switch_on)
        {
            this.switch_on = true;
            this.GetComponent<AudioSource>().PlayOneShot(this.sound_turn_on, this.kSoundVolume * PlayerPrefs.GetFloat("sound_volume", 1f));
        }
    }

    public void TurnOff()
    {
        if (this.switch_on)
        {
            this.switch_on = false;
            this.GetComponent<AudioSource>().PlayOneShot(this.sound_turn_off, this.kSoundVolume * PlayerPrefs.GetFloat("sound_volume", 1f));
        }
    }

    public void Update()
    {
        if (this.switch_on)
        {
            this.battery_life_remaining = this.battery_life_remaining - Time.deltaTime;
            if (this.battery_life_remaining <= 0f)
            {
                this.battery_life_remaining = 0f;
            }
            float battery_curve_eval = this.battery_curve.Evaluate(1f - (this.battery_life_remaining / this.max_battery_life));
            ((Light)this.transform.Find("Pointlight").gameObject.GetComponent(typeof(Light))).intensity = (this.initial_pointlight_intensity * battery_curve_eval) * 8f;
            ((Light)this.transform.Find("Spotlight").gameObject.GetComponent(typeof(Light))).intensity = (this.initial_spotlight_intensity * battery_curve_eval) * 3f;
            ((Light)this.transform.Find("Pointlight").gameObject.GetComponent(typeof(Light))).enabled = true;
            ((Light)this.transform.Find("Spotlight").gameObject.GetComponent(typeof(Light))).enabled = true;
        }
        else
        {
            ((Light)this.transform.Find("Pointlight").gameObject.GetComponent(typeof(Light))).enabled = false;
            ((Light)this.transform.Find("Spotlight").gameObject.GetComponent(typeof(Light))).enabled = false;
        }
        if (this.GetComponent<Rigidbody>())
        {
            this.transform.Find("Pointlight").GetComponent<Light>().enabled = true;
            this.transform.Find("Pointlight").GetComponent<Light>().intensity = 1f + Mathf.Sin(Time.time * 2f);
            this.transform.Find("Pointlight").GetComponent<Light>().range = 1f;
        }
        else
        {
            this.transform.Find("Pointlight").GetComponent<Light>().range = 10f;
        }
    }

    public FlashlightScript()
    {
        this.kSoundVolume = 0.3f;
        this.max_battery_life = (60 * 60) * 5.5f;
        this.battery_life_remaining = this.max_battery_life;
    }

}