using UnityEngine;
using System.Collections;

public enum MusicEvent
{
    DEAD = 0,
    WON = 1
}

[System.Serializable]
public partial class MusicScript : MonoBehaviour
{
    public AudioClip[] music_layers;
    private AudioSource[] music_sources;
    private float[] music_volume;
    private AudioSource sting_source;
    public AudioClip death_sting;
    public AudioClip win_sting;
    private float[] target_gain;
    private float danger;
    private float global_gain;
    private float target_global_gain;
    private float gain_recover_delay;
    private float danger_level_accumulate;
    private float mystical;
    public void HandleEvent(MusicEvent ev)
    {
        switch (ev)
        {
            case MusicEvent.DEAD:
                this.target_global_gain = 0f;
                this.gain_recover_delay = 1f;
                this.sting_source.PlayOneShot(this.death_sting);
                break;
            case MusicEvent.WON:
                this.target_global_gain = 0f;
                this.gain_recover_delay = 4f;
                this.sting_source.PlayOneShot(((GUISkinHolder)GameObject.Find("gui_skin_holder").GetComponent(typeof(GUISkinHolder))).win_sting);
                break;
        }
    }

    public void AddDangerLevel(float val)
    {
        this.danger_level_accumulate = this.danger_level_accumulate + val;
    }

    public void SetMystical(float val)
    {
        this.mystical = val;
    }

    public void Start()
    {
        this.music_sources = new AudioSource[this.music_layers.Length];
        this.music_volume = new float[this.music_layers.Length];
        this.target_gain = new float[this.music_layers.Length];
        int i = 0;
        while (i < this.music_layers.Length)
        {
            AudioSource source = (AudioSource)this.gameObject.AddComponent(typeof(AudioSource));
            source.clip = this.music_layers[i];
            this.music_sources[i] = source;
            this.music_sources[i].loop = true;
            this.music_sources[i].volume = 0f;
            this.music_volume[i] = 0f;
            this.target_gain[i] = 0f;
            ++i;
        }
        this.sting_source = (AudioSource)this.gameObject.AddComponent(typeof(AudioSource));
        this.music_sources[0].Play();
        this.music_sources[1].Play();
        this.music_sources[2].Play();
        this.music_sources[3].Play();
        this.music_sources[4].Play();
        this.target_gain[0] = 1f;
    }

    public void Update()
    {
        this.danger = Mathf.Max(this.danger_level_accumulate, this.danger);
        this.danger_level_accumulate = 0f;
        int i = 0;
        while (i < this.music_layers.Length)
        {
            this.music_sources[i].volume = this.music_volume[i] * PlayerPrefs.GetFloat("music_volume");
            ++i;
        }
        this.sting_source.volume = PlayerPrefs.GetFloat("music_volume", 1f);
    }

    public void FixedUpdate()
    {
        this.target_gain[1] = this.danger;
        this.target_gain[2] = this.danger;
        this.target_gain[3] = Mathf.Max(0f, this.danger - 0.5f);
        this.target_gain[4] = this.mystical;
        this.danger = this.danger * 0.99f;
        this.mystical = this.mystical * 0.99f;
        this.global_gain = Mathf.Lerp(this.global_gain, this.target_global_gain, 0.01f);
        if (this.gain_recover_delay > 0f)
        {
            this.gain_recover_delay = this.gain_recover_delay - Time.deltaTime;
            if (this.gain_recover_delay <= 0f)
            {
                this.target_global_gain = 1f;
            }
        }
        int i = 0;
        while (i < this.music_layers.Length)
        {
            this.music_volume[i] = Mathf.Lerp(this.target_gain[i], this.music_volume[i], 0.99f) * this.global_gain;
            ++i;
        }
    }

    public MusicScript()
    {
        this.global_gain = 1f;
        this.target_global_gain = 1f;
    }

}