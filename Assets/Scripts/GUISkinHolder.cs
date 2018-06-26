using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class GUISkinHolder : MonoBehaviour
{
    public GUISkin gui_skin;
    public AudioClip[] sound_scream;
    public AudioClip[] sound_tape_content;
    public AudioClip sound_tape_start;
    public AudioClip sound_tape_end;
    public AudioClip sound_tape_background;
    public GameObject tape_object;
    public AudioClip win_sting;
    public GameObject[] weapons;
    public GameObject weapon;
    public GameObject flashlight_object;
    public bool has_flashlight;
    public void Awake()
    {
        //weapon = weapons[2];
        this.weapon = this.weapons[Random.Range(0, this.weapons.Length)];
    }

    public void Start()
    {
    }

    public void Update()
    {
    }

}