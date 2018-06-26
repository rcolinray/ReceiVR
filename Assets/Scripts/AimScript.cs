using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;

// Prefabs
// Shortcuts to components
// Instances
// Public parameters
// Private variables
[System.Serializable]
public class Spring : object
{
    public float state;
    public float target_state;
    public float vel;
    public float strength;
    public float damping;
    public Spring(float state, float target_state, float strength, float damping)
    {
        this.Set(state, target_state, strength, damping);
    }

    public void Set(float state, float target_state, float strength, float damping)
    {
        this.state = state;
        this.target_state = target_state;
        this.strength = strength;
        this.damping = damping;
        this.vel = 0f;
    }

    public void Update()
    {
        bool linear_springs = false;
        if (linear_springs)
        {
            this.state = Mathf.MoveTowards(this.state, this.target_state, (this.strength * Time.deltaTime) * 0.05f);
        }
        else
        {
            this.vel = this.vel + (((this.target_state - this.state) * this.strength) * Time.deltaTime);
            this.vel = this.vel * Mathf.Pow(this.damping, Time.deltaTime);
            this.state = this.state + (this.vel * Time.deltaTime);
        }
    }

}
public enum GunTilt
{
    LEFT = 0,
    CENTER = 1,
    RIGHT = 2
}

public enum HandMagStage
{
    HOLD = 0,
    HOLD_TO_INSERT = 1,
    EMPTY = 2
}

public enum WeaponSlotType
{
    GUN = 0,
    MAGAZINE = 1,
    FLASHLIGHT = 2,
    EMPTY = 3,
    EMPTYING = 4
}

[System.Serializable]
public class WeaponSlot : object
{
    public GameObject obj;
    public WeaponSlotType type;
    public Vector3 start_pos;
    public Quaternion start_rot;
    public Spring spring;
    public WeaponSlot()
    {
        this.type = WeaponSlotType.EMPTY;
        this.start_pos = new Vector3(0, 0, 0);
        this.start_rot = Quaternion.identity;
        this.spring = new Spring(1, 1, 100, 1E-06f);
    }

}
/*++count;
		if(count >= 2){
			break;
		}*/
// Put held mag in empty slot
// Take mag from inventory
// Put flashlight away
// Take flashlight from inventory
// Put gun away
//} else if(gun_script.IsAddingRounds()){
//	add_rounds_pose_spring.target_state = 1.0;
/* && aim_spring.state > 0.5*/
//audiosource_audio_content.pitch = 10.0;
//audiosource_audio_content.clip = holder.sound_scream[Random.Range(0,holder.sound_scream.length)];
//audiosource_audio_content.pitch = 10.0;
//audiosource_audio_content.volume = 0.1;
//round.rigidbody.position += round.rigidbody.velocity * Time.deltaTime;
[System.Serializable]
public class DisplayLine : object
{
    public string str;
    public bool bold;
    public DisplayLine(string _str, bool _bold)
    {
        this.bold = _bold;
        this.str = _str;
    }

}
[System.Serializable]
public partial class AimScript : MonoBehaviour
{
    private GameObject magazine_obj;
    private GameObject gun_obj;
    private GameObject casing_with_bullet;
    public Texture texture_death_screen;
    public AudioClip[] sound_bullet_grab;
    public AudioClip[] sound_body_fall;
    public AudioClip[] sound_electrocute;
    public AudioSource audiosource_tape_background;
    public AudioSource audiosource_audio_content;
    private GameObject main_camera;
    private CharacterController character_controller;
    private bool show_help;
    private bool show_advanced_help;
    private float help_hold_time;
    private bool help_ever_shown;
    private bool just_started_help;
    private GameObject gun_instance;
    private float sensitivity_x;
    private float sensitivity_y;
    private float min_angle_y;
    private float max_angle_y;
    private GUISkinHolder holder;
    private WeaponHolder weapon_holder;
    public bool disable_springs;
    public bool disable_recoil;
    private bool aim_toggle;
    private float kAimSpringStrength;
    private float kAimSpringDamping;
    private Spring aim_spring;
    private GameObject held_flashlight;
    private Vector3 flashlight_aim_pos;
    private Quaternion flashlight_aim_rot;
    private Spring flashlight_mouth_spring;
    private Spring flash_ground_pose_spring;
    private Vector3 flash_ground_pos;
    private Quaternion flash_ground_rot;
    private float rotation_x_leeway;
    private float rotation_y_min_leeway;
    private float rotation_y_max_leeway;
    private float kRotationXLeeway;
    private float kRotationYMinLeeway;
    private float kRotationYMaxLeeway;
    private float rotation_x;
    private float rotation_y;
    private float view_rotation_x;
    private float view_rotation_y;
    private float kRecoilSpringStrength;
    private float kRecoilSpringDamping;
    private Spring x_recoil_spring;
    private Spring y_recoil_spring;
    private Spring head_recoil_spring_x;
    private Spring head_recoil_spring_y;
    private Vector3 mag_pos;
    private Quaternion mag_rot;
    private GameObject magazine_instance_in_hand;
    private float kGunDistance;
    private Spring slide_pose_spring;
    private Spring reload_pose_spring;
    private Spring press_check_pose_spring;
    private Spring inspect_cylinder_pose_spring;
    private Spring add_rounds_pose_spring;
    private Spring eject_rounds_pose_spring;
    private GunTilt gun_tilt;
    private Spring hold_pose_spring;
    private Spring mag_ground_pose_spring;
    private bool left_hand_occupied;
    private int kMaxHeadRecoil;
    private float[] head_recoil_delay;
    private int next_head_recoil_delay;
    private Vector3 mag_ground_pos;
    private Quaternion mag_ground_rot;
    private HandMagStage mag_stage;
    private List<GameObject> collected_rounds;
    private int target_weapon_slot;
    private bool queue_drop;
    private List<GameObject> loose_bullets;
    private List<Spring> loose_bullet_spring;
    private Spring show_bullet_spring;
    private float picked_up_bullet_delay;
    private float head_fall;
    private float head_fall_vel;
    private float head_tilt;
    private float head_tilt_vel;
    private float head_tilt_x_vel;
    private float head_tilt_y_vel;
    private float dead_fade;
    private float win_fade;
    private float dead_volume_fade;
    private bool dead_body_fell;
    private float start_tape_delay;
    private float stop_tape_delay;
    private List<AudioClip> tapes_heard;
    private List<AudioClip> tapes_remaining;
    private List<AudioClip> total_tapes;
    private bool tape_in_progress;
    private int unplayed_tapes;
    private bool god_mode;
    private bool slomo_mode;
    private int iddqd_progress;
    private int idkfa_progress;
    private int slomo_progress;
    private float cheat_delay;
    private float level_reset_hold;
    private WeaponSlot[] weapon_slots;
    private float health;
    private bool dying;
    private bool dead;
    private bool won;
    public bool IsAiming()
    {
        return (this.gun_instance != null) && (this.aim_spring.target_state == 1f);
    }

    public bool IsDead()
    {
        return this.dead;
    }

    public void StepRecoil(float amount)
    {
        this.x_recoil_spring.vel = this.x_recoil_spring.vel + (Random.Range(100, 400) * amount);
        this.y_recoil_spring.vel = this.y_recoil_spring.vel + (Random.Range(-200, 200) * amount);
    }

    public void WasShot()
    {
        this.head_recoil_spring_x.vel = this.head_recoil_spring_x.vel + Random.Range(-400, 400);
        this.head_recoil_spring_y.vel = this.head_recoil_spring_y.vel + Random.Range(-400, 400);
        this.x_recoil_spring.vel = this.x_recoil_spring.vel + Random.Range(-400, 400);
        this.y_recoil_spring.vel = this.y_recoil_spring.vel + Random.Range(-400, 400);
        this.rotation_x = this.rotation_x + Random.Range(-4, 4);
        this.rotation_y = this.rotation_y + Random.Range(-4, 4);
        if (!this.god_mode && !this.won)
        {
            this.dying = true;
            if (Random.Range(0f, 1f) < 0.3f)
            {
                this.SetDead(true);
            }
            if (this.dead && (Random.Range(0f, 1f) < 0.3f))
            {
                this.dead_fade = this.dead_fade + 0.3f;
            }
        }
    }

    public void FallDeath(Vector3 vel)
    {
        if (!this.god_mode && !this.won)
        {
            this.SetDead(true);
            this.head_fall_vel = vel.y;
            this.dead_fade = Mathf.Max(this.dead_fade, 0.5f);
            this.head_recoil_spring_x.vel = this.head_recoil_spring_x.vel + Random.Range(-400, 400);
            this.head_recoil_spring_y.vel = this.head_recoil_spring_y.vel + Random.Range(-400, 400);
        }
    }

    public void InstaKill()
    {
        this.SetDead(true);
        this.dead_fade = 1f;
    }

    public void Shock()
    {
        if (!this.god_mode && !this.won)
        {
            if (!this.dead)
            {
                this.PlaySoundFromGroup(this.sound_electrocute, 1f);
            }
            this.SetDead(true);
        }
        this.head_recoil_spring_x.vel = this.head_recoil_spring_x.vel + Random.Range(-400, 400);
        this.head_recoil_spring_y.vel = this.head_recoil_spring_y.vel + Random.Range(-400, 400);
    }

    public void SetDead(bool new_dead)
    {
        if (new_dead == this.dead)
        {
            return;
        }
        this.dead = new_dead;
        if (!this.dead)
        {
            this.head_tilt_vel = 0f;
            this.head_tilt_x_vel = 0f;
            this.head_tilt_y_vel = 0f;
            this.head_tilt = 0f;
            this.head_fall = 0f;
        }
        else
        {
            ((MusicScript)this.GetComponent(typeof(MusicScript))).HandleEvent(MusicEvent.DEAD);
            this.head_tilt_vel = Random.Range(-100, 100);
            this.head_tilt_x_vel = Random.Range(-100, 100);
            this.head_tilt_y_vel = Random.Range(-100, 100);
            this.head_fall = 0f;
            this.head_fall_vel = 0f;
            this.dead_body_fell = false;
        }
    }

    public void PlaySoundFromGroup(AudioClip[] group, float volume)
    {
        int which_shot = Random.Range(0, group.Length);
        this.GetComponent<AudioSource>().PlayOneShot((AudioClip)group[which_shot], volume * PlayerPrefs.GetFloat("sound_volume", 1f));
    }

    public void AddLooseBullet(bool spring)
    {
        this.loose_bullets.Add(UnityEngine.Object.Instantiate(this.casing_with_bullet));
        Spring new_spring = new Spring(0.3f, 0.3f, this.kAimSpringStrength, this.kAimSpringDamping);
        this.loose_bullet_spring.Add(new_spring);
        if (spring)
        {
            new_spring.vel = 3f;
            this.picked_up_bullet_delay = 2f;
        }
    }

    public void Start()
    {
        this.disable_springs = false;
        this.disable_recoil = true;
        this.holder = (GUISkinHolder)GameObject.Find("gui_skin_holder").GetComponent(typeof(GUISkinHolder));
        this.weapon_holder = (WeaponHolder)this.holder.weapon.GetComponent(typeof(WeaponHolder));
        this.magazine_obj = this.weapon_holder.mag_object;
        this.gun_obj = this.weapon_holder.gun_object;
        this.casing_with_bullet = this.weapon_holder.bullet_object;
        if (Random.Range(0f, 1f) < 0.35f)
        {
            this.held_flashlight = UnityEngine.Object.Instantiate(this.holder.flashlight_object);
            UnityEngine.Object.Destroy(this.held_flashlight.GetComponent<Rigidbody>());
            ((FlashlightScript)this.held_flashlight.GetComponent(typeof(FlashlightScript))).TurnOn();
            this.holder.has_flashlight = true;
        }
        this.rotation_x = this.transform.rotation.eulerAngles.y;
        this.view_rotation_x = this.transform.rotation.eulerAngles.y;
        this.gun_instance = UnityEngine.Object.Instantiate(this.gun_obj);
        Component[] renderers = this.gun_instance.GetComponentsInChildren(typeof(Renderer));
        foreach (Renderer renderer in renderers)
        {
            renderer.castShadows = false;
        }
        this.main_camera = GameObject.Find("Main Camera").gameObject;
        this.character_controller = (CharacterController)this.GetComponent(typeof(CharacterController));
        int i = 0;
        while (i < this.kMaxHeadRecoil)
        {
            this.head_recoil_delay[i] = -1f;
            ++i;
        }
        i = 0;
        while (i < 10)
        {
            this.weapon_slots[i] = new WeaponSlot();
            ++i;
        }
        int num_start_bullets = Random.Range(0, 10);
        if (this.GetGunScript().gun_type == GunType.AUTOMATIC)
        {
            int num_start_mags = Random.Range(0, 3);
            i = 1;
            while (i < (num_start_mags + 1))
            {
                this.weapon_slots[i].type = WeaponSlotType.MAGAZINE;
                this.weapon_slots[i].obj = UnityEngine.Object.Instantiate(this.magazine_obj);
                ++i;
            }
        }
        else
        {
            num_start_bullets = num_start_bullets + Random.Range(0, 20);
        }
        this.loose_bullets = new List<GameObject>();
        this.loose_bullet_spring = new List<Spring>();
        i = 0;
        while (i < num_start_bullets)
        {
            this.AddLooseBullet(false);
            ++i;
        }
        this.audiosource_tape_background = (AudioSource)this.gameObject.AddComponent(typeof(AudioSource));
        this.audiosource_tape_background.loop = true;
        this.audiosource_tape_background.clip = this.holder.sound_tape_background;
        this.audiosource_audio_content = (AudioSource)this.gameObject.AddComponent(typeof(AudioSource));
        this.audiosource_audio_content.loop = false;
        int count = 0;
        foreach (AudioClip tape in this.holder.sound_tape_content)
        {
            this.total_tapes.Add(tape);
        }
        List<AudioClip> temp_total_tapes = this.total_tapes;
        while (temp_total_tapes.Count > 0)
        {
            int rand_tape_id = Random.Range(0, temp_total_tapes.Count);
            this.tapes_remaining.Add(temp_total_tapes[rand_tape_id]);
            temp_total_tapes.RemoveAt(rand_tape_id);
        }
    }

    public float GunDist()
    {
        return this.kGunDistance * (0.5f + (PlayerPrefs.GetFloat("gun_distance", 1f) * 0.5f));
    }

    public Vector3 AimPos()
    {
        Vector3 aim_dir = this.AimDir();
        return this.main_camera.transform.position + (aim_dir * this.GunDist());
    }

    public Vector3 AimDir()
    {
        Quaternion aim_rot = Quaternion.Euler((-this.rotation_y * Mathf.PI) / 180f, (this.rotation_x * Mathf.PI) / 180f, 0f);
        return aim_rot * new Vector3(0f, 0f, 1f);
    }

    public Gun GetGunScript()
    {
        return (Gun)this.gun_instance.GetComponent(typeof(Gun));
    }

    public Vector3 mix(Vector3 a, Vector3 b, float val)
    {
        return a + ((b - a) * val);
    }

    public Quaternion mix(Quaternion a, Quaternion b, float val)
    {
        float angle = 0f;
        Vector3 axis = new Vector3();
        (Quaternion.Inverse(b) * a).ToAngleAxis(out angle, out axis);
        if (angle > 180)
        {
            angle = angle - 360;
        }
        if (angle < -180)
        {
            angle = angle + 360;
        }
        if (angle == 0)
        {
            return a;
        }
        return a * Quaternion.AngleAxis(angle * -val, axis);
    }

    public bool ShouldPickUpNearby()
    {
        object nearest_mag = null;
        float nearest_mag_dist = 0f;
        Collider[] colliders = Physics.OverlapSphere(this.main_camera.transform.position, 2f, 1 << 8);
        foreach (Collider collider in colliders)
        {
            if ((this.magazine_obj && (collider.gameObject.name == (this.magazine_obj.name + "(Clone)"))) && collider.gameObject.GetComponent<Rigidbody>())
            {
                if (this.mag_stage == HandMagStage.EMPTY)
                {
                    return true;
                }
            }
            else
            {
                if (((collider.gameObject.name == this.casing_with_bullet.name) || (collider.gameObject.name == (this.casing_with_bullet.name + "(Clone)"))) && collider.gameObject.GetComponent<Rigidbody>())
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void HandleGetControl()
    {
        object nearest_mag = null;
        float nearest_mag_dist = 0f;
        Collider[] colliders = Physics.OverlapSphere(this.main_camera.transform.position, 2f, 1 << 8);
        foreach (Collider collider in colliders)
        {
            if ((this.magazine_obj && (collider.gameObject.name == (this.magazine_obj.name + "(Clone)"))) && collider.gameObject.GetComponent<Rigidbody>())
            {
                float dist = Vector3.Distance(collider.transform.position, this.main_camera.transform.position);
                if ((nearest_mag == null) || (dist < nearest_mag_dist))
                {
                    nearest_mag_dist = dist;
                    nearest_mag = collider.gameObject;
                }
            }
            else
            {
                if (((collider.gameObject.name == this.casing_with_bullet.name) || (collider.gameObject.name == (this.casing_with_bullet.name + "(Clone)"))) && collider.gameObject.GetComponent<Rigidbody>())
                {
                    this.collected_rounds.Add(collider.gameObject);
                    collider.gameObject.GetComponent<Rigidbody>().useGravity = false;
                    collider.gameObject.GetComponent<Rigidbody>().WakeUp();
                    collider.enabled = false;
                }
                else
                {
                    if ((collider.gameObject.name == "cassette_tape(Clone)") && collider.gameObject.GetComponent<Rigidbody>())
                    {
                        this.collected_rounds.Add(collider.gameObject);
                        collider.gameObject.GetComponent<Rigidbody>().useGravity = false;
                        collider.gameObject.GetComponent<Rigidbody>().WakeUp();
                        collider.enabled = false;
                    }
                    else
                    {
                        if (((collider.gameObject.name == "flashlight_object(Clone)") && collider.gameObject.GetComponent<Rigidbody>()) && !this.held_flashlight)
                        {
                            this.held_flashlight = collider.gameObject;
                            UnityEngine.Object.Destroy(this.held_flashlight.GetComponent<Rigidbody>());
                            ((FlashlightScript)this.held_flashlight.GetComponent(typeof(FlashlightScript))).TurnOn();
                            this.holder.has_flashlight = true;
                            this.flash_ground_pos = this.held_flashlight.transform.position;
                            this.flash_ground_rot = this.held_flashlight.transform.rotation;
                            this.flash_ground_pose_spring.state = 1f;
                            this.flash_ground_pose_spring.vel = 1f;
                        }
                    }
                }
            }
        }
        if ((nearest_mag != null) && (this.mag_stage == HandMagStage.EMPTY))
        {
            this.magazine_instance_in_hand = (GameObject)nearest_mag;
            UnityEngine.Object.Destroy(this.magazine_instance_in_hand.GetComponent<Rigidbody>());
            this.mag_ground_pos = this.magazine_instance_in_hand.transform.position;
            this.mag_ground_rot = this.magazine_instance_in_hand.transform.rotation;
            this.mag_ground_pose_spring.state = 1f;
            this.mag_ground_pose_spring.vel = 1f;
            this.hold_pose_spring.state = 1f;
            this.hold_pose_spring.vel = 0f;
            this.hold_pose_spring.target_state = 1f;
            this.mag_stage = HandMagStage.HOLD;
        }
    }

    public bool HandleInventoryControls()
    {
        if (Input.GetButtonDown("Holster"))
        {
            this.target_weapon_slot = -1;
        }
        if (Input.GetButtonDown("Inventory 1"))
        {
            this.target_weapon_slot = 0;
        }
        if (Input.GetButtonDown("Inventory 2"))
        {
            this.target_weapon_slot = 1;
        }
        if (Input.GetButtonDown("Inventory 3"))
        {
            this.target_weapon_slot = 2;
        }
        if (Input.GetButtonDown("Inventory 4"))
        {
            this.target_weapon_slot = 3;
        }
        if (Input.GetButtonDown("Inventory 5"))
        {
            this.target_weapon_slot = 4;
        }
        if (Input.GetButtonDown("Inventory 6"))
        {
            this.target_weapon_slot = 5;
        }
        if (Input.GetButtonDown("Inventory 7"))
        {
            this.target_weapon_slot = 6;
        }
        if (Input.GetButtonDown("Inventory 8"))
        {
            this.target_weapon_slot = 7;
        }
        if (Input.GetButtonDown("Inventory 9"))
        {
            this.target_weapon_slot = 8;
        }
        if (Input.GetButtonDown("Inventory 10"))
        {
            this.target_weapon_slot = 9;
        }
        bool mag_ejecting = false;
        if (this.gun_instance && (((Gun)this.gun_instance.GetComponent(typeof(Gun))).IsMagCurrentlyEjecting() || ((Gun)this.gun_instance.GetComponent(typeof(Gun))).ready_to_remove_mag))
        {
            mag_ejecting = true;
        }
        bool insert_mag_with_number_key = false;
        if (((this.target_weapon_slot != -2) && !mag_ejecting) && ((this.mag_stage == HandMagStage.EMPTY) || (this.mag_stage == HandMagStage.HOLD)))
        {
            if ((this.target_weapon_slot == -1) && !this.gun_instance)
            {
                int i = 0;
                while (i < 10)
                {
                    if (this.weapon_slots[i].type == WeaponSlotType.GUN)
                    {
                        this.target_weapon_slot = i;
                        break;
                    }
                    ++i;
                }
            }
            if (((this.mag_stage == HandMagStage.HOLD) && (this.target_weapon_slot != -1)) && (this.weapon_slots[this.target_weapon_slot].type == WeaponSlotType.EMPTY))
            {
                int i = 0;
                while (i < 10)
                {
                    if ((this.weapon_slots[this.target_weapon_slot].type != WeaponSlotType.EMPTY) && (this.weapon_slots[this.target_weapon_slot].obj == this.magazine_instance_in_hand))
                    {
                        this.weapon_slots[this.target_weapon_slot].type = WeaponSlotType.EMPTY;
                    }
                    ++i;
                }
                this.weapon_slots[this.target_weapon_slot].type = WeaponSlotType.MAGAZINE;
                this.weapon_slots[this.target_weapon_slot].obj = this.magazine_instance_in_hand;
                this.weapon_slots[this.target_weapon_slot].spring.state = 0f;
                this.weapon_slots[this.target_weapon_slot].spring.target_state = 1f;
                this.weapon_slots[this.target_weapon_slot].start_pos = this.magazine_instance_in_hand.transform.position - this.main_camera.transform.position;
                this.weapon_slots[this.target_weapon_slot].start_rot = Quaternion.Inverse(this.main_camera.transform.rotation) * this.magazine_instance_in_hand.transform.rotation;
                this.magazine_instance_in_hand = null;
                this.mag_stage = HandMagStage.EMPTY;
                this.target_weapon_slot = -2;
            }
            else
            {
                if ((((((this.mag_stage == HandMagStage.HOLD) && (this.target_weapon_slot != -1)) && (this.weapon_slots[this.target_weapon_slot].type == WeaponSlotType.EMPTYING)) && (this.weapon_slots[this.target_weapon_slot].obj == this.magazine_instance_in_hand)) && this.gun_instance) && !((Gun)this.gun_instance.GetComponent(typeof(Gun))).IsThereAMagInGun())
                {
                    insert_mag_with_number_key = true;
                    this.target_weapon_slot = -2;
                }
                else
                {
                    if (((this.target_weapon_slot != -1) && (this.mag_stage == HandMagStage.EMPTY)) && (this.weapon_slots[this.target_weapon_slot].type == WeaponSlotType.MAGAZINE))
                    {
                        this.magazine_instance_in_hand = this.weapon_slots[this.target_weapon_slot].obj;
                        this.mag_stage = HandMagStage.HOLD;
                        this.hold_pose_spring.state = 1f;
                        this.hold_pose_spring.target_state = 1f;
                        this.weapon_slots[this.target_weapon_slot].type = WeaponSlotType.EMPTYING;
                        this.weapon_slots[this.target_weapon_slot].spring.target_state = 0f;
                        this.weapon_slots[this.target_weapon_slot].spring.state = 1f;
                        this.target_weapon_slot = -2;
                    }
                    else
                    {
                        if ((((this.target_weapon_slot != -1) && (this.mag_stage == HandMagStage.EMPTY)) && (this.weapon_slots[this.target_weapon_slot].type == WeaponSlotType.EMPTY)) && this.held_flashlight)
                        {
                            ((FlashlightScript)this.held_flashlight.GetComponent(typeof(FlashlightScript))).TurnOff();
                            this.weapon_slots[this.target_weapon_slot].type = WeaponSlotType.FLASHLIGHT;
                            this.weapon_slots[this.target_weapon_slot].obj = this.held_flashlight;
                            this.weapon_slots[this.target_weapon_slot].spring.state = 0f;
                            this.weapon_slots[this.target_weapon_slot].spring.target_state = 1f;
                            this.weapon_slots[this.target_weapon_slot].start_pos = this.held_flashlight.transform.position - this.main_camera.transform.position;
                            this.weapon_slots[this.target_weapon_slot].start_rot = Quaternion.Inverse(this.main_camera.transform.rotation) * this.held_flashlight.transform.rotation;
                            this.held_flashlight = null;
                            this.target_weapon_slot = -2;
                        }
                        else
                        {
                            if (((this.target_weapon_slot != -1) && !this.held_flashlight) && (this.weapon_slots[this.target_weapon_slot].type == WeaponSlotType.FLASHLIGHT))
                            {
                                this.held_flashlight = this.weapon_slots[this.target_weapon_slot].obj;
                                ((FlashlightScript)this.held_flashlight.GetComponent(typeof(FlashlightScript))).TurnOn();
                                this.weapon_slots[this.target_weapon_slot].type = WeaponSlotType.EMPTYING;
                                this.weapon_slots[this.target_weapon_slot].spring.target_state = 0f;
                                this.weapon_slots[this.target_weapon_slot].spring.state = 1f;
                                this.target_weapon_slot = -2;
                            }
                            else
                            {
                                if (this.gun_instance && (this.target_weapon_slot == -1))
                                {
                                    if (this.target_weapon_slot == -1)
                                    {
                                        int i = 0;
                                        while (i < 10)
                                        {
                                            if (this.weapon_slots[i].type == WeaponSlotType.EMPTY)
                                            {
                                                this.target_weapon_slot = i;
                                                break;
                                            }
                                            ++i;
                                        }
                                    }
                                    if ((this.target_weapon_slot != -1) && (this.weapon_slots[this.target_weapon_slot].type == WeaponSlotType.EMPTY))
                                    {
                                        int i = 0;
                                        while (i < 10)
                                        {
                                            if ((this.weapon_slots[this.target_weapon_slot].type != WeaponSlotType.EMPTY) && (this.weapon_slots[this.target_weapon_slot].obj == this.gun_instance))
                                            {
                                                this.weapon_slots[this.target_weapon_slot].type = WeaponSlotType.EMPTY;
                                            }
                                            ++i;
                                        }
                                        this.weapon_slots[this.target_weapon_slot].type = WeaponSlotType.GUN;
                                        this.weapon_slots[this.target_weapon_slot].obj = this.gun_instance;
                                        this.weapon_slots[this.target_weapon_slot].spring.state = 0f;
                                        this.weapon_slots[this.target_weapon_slot].spring.target_state = 1f;
                                        this.weapon_slots[this.target_weapon_slot].start_pos = this.gun_instance.transform.position - this.main_camera.transform.position;
                                        this.weapon_slots[this.target_weapon_slot].start_rot = Quaternion.Inverse(this.main_camera.transform.rotation) * this.gun_instance.transform.rotation;
                                        this.gun_instance = null;
                                        this.target_weapon_slot = -2;
                                    }
                                }
                                else
                                {
                                    if ((this.target_weapon_slot >= 0) && !this.gun_instance)
                                    {
                                        if (this.weapon_slots[this.target_weapon_slot].type == WeaponSlotType.EMPTY)
                                        {
                                            this.target_weapon_slot = -2;
                                        }
                                        else
                                        {
                                            if (this.weapon_slots[this.target_weapon_slot].type == WeaponSlotType.GUN)
                                            {
                                                this.gun_instance = this.weapon_slots[this.target_weapon_slot].obj;
                                                this.weapon_slots[this.target_weapon_slot].type = WeaponSlotType.EMPTYING;
                                                this.weapon_slots[this.target_weapon_slot].spring.target_state = 0f;
                                                this.weapon_slots[this.target_weapon_slot].spring.state = 1f;
                                                this.target_weapon_slot = -2;
                                            }
                                            else
                                            {
                                                if ((this.weapon_slots[this.target_weapon_slot].type == WeaponSlotType.MAGAZINE) && (this.mag_stage == HandMagStage.EMPTY))
                                                {
                                                    this.magazine_instance_in_hand = this.weapon_slots[this.target_weapon_slot].obj;
                                                    this.mag_stage = HandMagStage.HOLD;
                                                    this.weapon_slots[this.target_weapon_slot].type = WeaponSlotType.EMPTYING;
                                                    this.weapon_slots[this.target_weapon_slot].spring.target_state = 0f;
                                                    this.weapon_slots[this.target_weapon_slot].spring.state = 1f;
                                                    this.target_weapon_slot = -2;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        return insert_mag_with_number_key;
    }

    public void HandleGunControls(bool insert_mag_with_number_key)
    {
        Gun gun_script = this.GetGunScript();
        if (Input.GetButton("Trigger"))
        {
            gun_script.ApplyPressureToTrigger();
        }
        else
        {
            gun_script.ReleasePressureFromTrigger();
        }
        if (Input.GetButtonDown("Slide Lock"))
        {
            gun_script.ReleaseSlideLock();
        }
        if (Input.GetButtonUp("Slide Lock"))
        {
            gun_script.ReleasePressureOnSlideLock();
        }
        if (Input.GetButton("Slide Lock"))
        {
            gun_script.PressureOnSlideLock();
        }
        if (Input.GetButtonDown("Safety"))
        {
            gun_script.ToggleSafety();
        }
        if (Input.GetButtonDown("Auto Mod Toggle"))
        {
            gun_script.ToggleAutoMod();
        }
        if (Input.GetButtonDown("Pull Back Slide"))
        {
            gun_script.PullBackSlide();
        }
        if (Input.GetButtonUp("Pull Back Slide"))
        {
            gun_script.ReleaseSlide();
        }
        if (Input.GetButtonDown("Swing Out Cylinder"))
        {
            gun_script.SwingOutCylinder();
        }
        if (Input.GetButtonDown("Close Cylinder"))
        {
            gun_script.CloseCylinder();
        }
        if (Input.GetButton("Extractor Rod"))
        {
            gun_script.ExtractorRod();
        }
        if (Input.GetButton("Hammer"))
        {
            gun_script.PressureOnHammer();
        }
        if (Input.GetButtonUp("Hammer"))
        {
            gun_script.ReleaseHammer();
        }
        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            gun_script.RotateCylinder((int)Input.GetAxis("Mouse ScrollWheel"));
        }
        if (Input.GetButtonDown("Insert"))
        {
            if (this.loose_bullets.Count > 0)
            {
                if (this.GetGunScript().AddRoundToCylinder())
                {
                    GameObject.Destroy((UnityEngine.Object)this.loose_bullets[0]);
                    this.loose_bullets.RemoveAt(0);
                    this.loose_bullet_spring.RemoveAt(0);
                }
            }
        }
        if ((this.slide_pose_spring.target_state < 0.1f) && (this.reload_pose_spring.target_state < 0.1f))
        {
            this.gun_tilt = GunTilt.CENTER;
        }
        else
        {
            if (this.slide_pose_spring.target_state > this.reload_pose_spring.target_state)
            {
                this.gun_tilt = GunTilt.LEFT;
            }
            else
            {
                this.gun_tilt = GunTilt.RIGHT;
            }
        }
        this.slide_pose_spring.target_state = 0f;
        this.reload_pose_spring.target_state = 0f;
        this.press_check_pose_spring.target_state = 0f;
        if (gun_script.IsSafetyOn())
        {
            this.reload_pose_spring.target_state = 0.2f;
            this.slide_pose_spring.target_state = 0f;
            this.gun_tilt = GunTilt.RIGHT;
        }
        if (gun_script.IsSlideLocked())
        {
            if (this.gun_tilt != GunTilt.LEFT)
            {
                this.reload_pose_spring.target_state = 0.7f;
            }
            else
            {
                this.slide_pose_spring.target_state = 0.7f;
            }
        }
        if (gun_script.IsSlidePulledBack())
        {
            if (this.gun_tilt != GunTilt.RIGHT)
            {
                this.slide_pose_spring.target_state = 1f;
            }
            else
            {
                this.reload_pose_spring.target_state = 1f;
            }
        }
        if (gun_script.IsPressCheck())
        {
            this.slide_pose_spring.target_state = 0f;
            this.reload_pose_spring.target_state = 0f;
            this.press_check_pose_spring.target_state = 0.6f;
        }
        this.add_rounds_pose_spring.target_state = 0f;
        this.eject_rounds_pose_spring.target_state = 0f;
        this.inspect_cylinder_pose_spring.target_state = 0f;
        if (gun_script.IsEjectingRounds())
        {
            this.eject_rounds_pose_spring.target_state = 1f;
        }
        else
        {
            if (gun_script.IsCylinderOpen())
            {
                this.inspect_cylinder_pose_spring.target_state = 1f;
            }
        }
        this.x_recoil_spring.vel = this.x_recoil_spring.vel + gun_script.recoil_transfer_x;
        this.y_recoil_spring.vel = this.y_recoil_spring.vel + gun_script.recoil_transfer_y;
        this.rotation_x = this.rotation_x + gun_script.rotation_transfer_x;
        this.rotation_y = this.rotation_y + gun_script.rotation_transfer_y;
        gun_script.recoil_transfer_x = 0f;
        gun_script.recoil_transfer_y = 0f;
        gun_script.rotation_transfer_x = 0f;
        gun_script.rotation_transfer_y = 0f;
        if (gun_script.add_head_recoil)
        {
            this.head_recoil_delay[this.next_head_recoil_delay] = 0.1f;
            this.next_head_recoil_delay = (this.next_head_recoil_delay + 1) % this.kMaxHeadRecoil;
            gun_script.add_head_recoil = false;
        }
        if (gun_script.ready_to_remove_mag && !this.magazine_instance_in_hand)
        {
            this.magazine_instance_in_hand = gun_script.RemoveMag();
            this.mag_stage = HandMagStage.HOLD;
            this.hold_pose_spring.state = 0f;
            this.hold_pose_spring.vel = 0f;
            this.hold_pose_spring.target_state = 1f;
        }
        if (Input.GetButtonDown("Insert") || insert_mag_with_number_key)
        {
            if (((this.mag_stage == HandMagStage.HOLD) && !gun_script.IsThereAMagInGun()) || insert_mag_with_number_key)
            {
                this.hold_pose_spring.target_state = 0f;
                this.mag_stage = HandMagStage.HOLD_TO_INSERT;
            }
        }
        if (this.mag_stage == HandMagStage.HOLD_TO_INSERT)
        {
            if (this.hold_pose_spring.state < 0.01f)
            {
                gun_script.InsertMag(this.magazine_instance_in_hand);
                this.magazine_instance_in_hand = null;
                this.mag_stage = HandMagStage.EMPTY;
            }
        }
    }

    public void HandleControls()
    {
        if (Input.GetButton("Get"))
        {
            this.HandleGetControl();
        }
        int i = 0;
        while (i < this.kMaxHeadRecoil)
        {
            if (this.head_recoil_delay[i] != -1f)
            {
                this.head_recoil_delay[i] = this.head_recoil_delay[i] - Time.deltaTime;
                if (this.head_recoil_delay[i] <= 0f)
                {
                    this.head_recoil_spring_x.vel = this.head_recoil_spring_x.vel + Random.Range(-30f, 30f);
                    this.head_recoil_spring_y.vel = this.head_recoil_spring_y.vel + Random.Range(-30f, 30f);
                    this.head_recoil_delay[i] = -1f;
                }
            }
            ++i;
        }
        bool insert_mag_with_number_key = this.HandleInventoryControls();
        if (Input.GetButtonDown("Eject/Drop") || this.queue_drop)
        {
            if (this.mag_stage == HandMagStage.HOLD)
            {
                this.mag_stage = HandMagStage.EMPTY;
                this.magazine_instance_in_hand.AddComponent(typeof(Rigidbody));
                this.magazine_instance_in_hand.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
                this.magazine_instance_in_hand.GetComponent<Rigidbody>().velocity = this.character_controller.velocity;
                this.magazine_instance_in_hand = null;
                this.queue_drop = false;
            }
        }
        if (Input.GetButtonDown("Eject/Drop"))
        {
            if ((this.mag_stage == HandMagStage.EMPTY) && this.gun_instance)
            {
                if (((Gun)this.gun_instance.GetComponent(typeof(Gun))).IsMagCurrentlyEjecting())
                {
                    this.queue_drop = true;
                }
                else
                {
                    ((Gun)this.gun_instance.GetComponent(typeof(Gun))).MagEject();
                }
            }
            else
            {
                if (this.mag_stage == HandMagStage.HOLD_TO_INSERT)
                {
                    this.mag_stage = HandMagStage.HOLD;
                    this.hold_pose_spring.target_state = 1f;
                }
            }
        }
        if (this.gun_instance)
        {
            this.HandleGunControls(insert_mag_with_number_key);
        }
        else
        {
            if (this.mag_stage == HandMagStage.HOLD)
            {
                if (Input.GetButtonDown("Insert"))
                {
                    if (this.loose_bullets.Count > 0)
                    {
                        if (((Mag)this.magazine_instance_in_hand.GetComponent(typeof(Mag))).AddRound())
                        {
                            GameObject.Destroy((UnityEngine.Object)this.loose_bullets[0]);
                            this.loose_bullet_spring.RemoveAt(0);
                        }
                    }
                }
                if (Input.GetButtonDown("Pull Back Slide"))
                {
                    if (((Mag)this.magazine_instance_in_hand.GetComponent(typeof(Mag))).RemoveRoundAnimated())
                    {
                        this.AddLooseBullet(true);
                        this.PlaySoundFromGroup(this.sound_bullet_grab, 0.2f);
                    }
                }
            }
        }
        if (Input.GetButtonDown("Aim Toggle"))
        {
            this.aim_toggle = !this.aim_toggle;
        }
        if (Input.GetButtonDown("Slow Motion Toggle") && this.slomo_mode)
        {
            if (Time.timeScale == 1f)
            {
                Time.timeScale = 0.1f;
            }
            else
            {
                Time.timeScale = 1f;
            }
        }
    }

    public void StartTapePlay()
    {
        this.GetComponent<AudioSource>().PlayOneShot(this.holder.sound_tape_start, 1f * PlayerPrefs.GetFloat("voice_volume", 1f));
        this.audiosource_tape_background.Play();
        if (this.tape_in_progress && (this.start_tape_delay == 0f))
        {
            this.audiosource_audio_content.Play();
        }
        if (!this.tape_in_progress && (this.tapes_remaining.Count > 0))
        {
            this.audiosource_audio_content.clip = (AudioClip)this.tapes_remaining[0];
            this.tapes_remaining.RemoveAt(0);
            this.start_tape_delay = Random.Range(0.5f, 3f);
            this.stop_tape_delay = 0f;
            this.tape_in_progress = true;
        }
        this.audiosource_tape_background.pitch = 0.1f;
        this.audiosource_audio_content.pitch = 0.1f;
    }

    public void StopTapePlay()
    {
        this.GetComponent<AudioSource>().PlayOneShot(this.holder.sound_tape_end, 1f * PlayerPrefs.GetFloat("voice_volume", 1f));
        if (this.tape_in_progress)
        {
            this.audiosource_tape_background.Pause();
            this.audiosource_audio_content.Pause();
        }
        else
        {
            this.audiosource_tape_background.Stop();
            this.audiosource_audio_content.Stop();
        }
    }

    public void StartWin()
    {
        ((MusicScript)this.GetComponent(typeof(MusicScript))).HandleEvent(MusicEvent.WON);
        this.won = true;
    }

    public void ApplyPose(string name, float amount)
    {
        Transform pose = this.gun_instance.transform.Find(name);
        if ((amount == 0f) || !pose)
        {
            return;
        }
        this.gun_instance.transform.position = this.mix(this.gun_instance.transform.position, pose.position, amount);
        this.gun_instance.transform.rotation = this.mix(this.gun_instance.transform.rotation, pose.rotation, amount);
    }

    public void UpdateCheats()
    {
        if ((this.iddqd_progress == 0) && Input.GetKeyDown("i"))
        {
            ++this.iddqd_progress;
            this.cheat_delay = 1f;
        }
        else
        {
            if ((this.iddqd_progress == 1) && Input.GetKeyDown("d"))
            {
                ++this.iddqd_progress;
                this.cheat_delay = 1f;
            }
            else
            {
                if ((this.iddqd_progress == 2) && Input.GetKeyDown("d"))
                {
                    ++this.iddqd_progress;
                    this.cheat_delay = 1f;
                }
                else
                {
                    if ((this.iddqd_progress == 3) && Input.GetKeyDown("q"))
                    {
                        ++this.iddqd_progress;
                        this.cheat_delay = 1f;
                    }
                    else
                    {
                        if ((this.iddqd_progress == 4) && Input.GetKeyDown("d"))
                        {
                            this.iddqd_progress = 0;
                            this.god_mode = !this.god_mode;
                            this.PlaySoundFromGroup(this.holder.sound_scream, 1f);
                        }
                    }
                }
            }
        }
        if ((this.idkfa_progress == 0) && Input.GetKeyDown("i"))
        {
            ++this.idkfa_progress;
            this.cheat_delay = 1f;
        }
        else
        {
            if ((this.idkfa_progress == 1) && Input.GetKeyDown("d"))
            {
                ++this.idkfa_progress;
                this.cheat_delay = 1f;
            }
            else
            {
                if ((this.idkfa_progress == 2) && Input.GetKeyDown("k"))
                {
                    ++this.idkfa_progress;
                    this.cheat_delay = 1f;
                }
                else
                {
                    if ((this.idkfa_progress == 3) && Input.GetKeyDown("f"))
                    {
                        ++this.idkfa_progress;
                        this.cheat_delay = 1f;
                    }
                    else
                    {
                        if ((this.idkfa_progress == 4) && Input.GetKeyDown("a"))
                        {
                            this.idkfa_progress = 0;
                            if (this.loose_bullets.Count < 30)
                            {
                                this.PlaySoundFromGroup(this.sound_bullet_grab, 0.2f);
                            }
                            while (this.loose_bullets.Count < 30)
                            {
                                this.AddLooseBullet(true);
                            }
                            this.PlaySoundFromGroup(this.holder.sound_scream, 1f);
                        }
                    }
                }
            }
        }
        if ((this.slomo_progress == 0) && Input.GetKeyDown("s"))
        {
            ++this.slomo_progress;
            this.cheat_delay = 1f;
        }
        else
        {
            if ((this.slomo_progress == 1) && Input.GetKeyDown("l"))
            {
                ++this.slomo_progress;
                this.cheat_delay = 1f;
            }
            else
            {
                if ((this.slomo_progress == 2) && Input.GetKeyDown("o"))
                {
                    ++this.slomo_progress;
                    this.cheat_delay = 1f;
                }
                else
                {
                    if ((this.slomo_progress == 3) && Input.GetKeyDown("m"))
                    {
                        ++this.slomo_progress;
                        this.cheat_delay = 1f;
                    }
                    else
                    {
                        if ((this.slomo_progress == 4) && Input.GetKeyDown("o"))
                        {
                            this.slomo_progress = 0;
                            this.slomo_mode = true;
                            if (Time.timeScale == 1f)
                            {
                                Time.timeScale = 0.1f;
                            }
                            else
                            {
                                Time.timeScale = 1f;
                            }
                            this.PlaySoundFromGroup(this.holder.sound_scream, 1f);
                        }
                    }
                }
            }
        }
        if (this.cheat_delay > 0f)
        {
            this.cheat_delay = this.cheat_delay - Time.deltaTime;
            if (this.cheat_delay <= 0f)
            {
                this.cheat_delay = 0f;
                this.iddqd_progress = 0;
                this.idkfa_progress = 0;
                this.slomo_progress = 0;
            }
        }
    }

    public void UpdateTape()
    {
        if (!this.tape_in_progress && (this.unplayed_tapes > 0))
        {
            --this.unplayed_tapes;
            this.StartTapePlay();
        }
        if (Input.GetButtonDown("Tape Player") && this.tape_in_progress)
        {
            if (!this.audiosource_tape_background.isPlaying)
            {
                this.StartTapePlay();
            }
            else
            {
                this.StopTapePlay();
            }
        }
        if (this.tape_in_progress && this.audiosource_tape_background.isPlaying)
        {
            ((MusicScript)this.GetComponent(typeof(MusicScript))).SetMystical((this.tapes_heard.Count + 1f) / this.total_tapes.Count);
            this.audiosource_tape_background.volume = PlayerPrefs.GetFloat("voice_volume", 1f);
            this.audiosource_tape_background.pitch = Mathf.Min(1f, this.audiosource_audio_content.pitch + (Time.deltaTime * 3f));
            this.audiosource_audio_content.volume = PlayerPrefs.GetFloat("voice_volume", 1f);
            this.audiosource_audio_content.pitch = Mathf.Min(1f, this.audiosource_audio_content.pitch + (Time.deltaTime * 3f));
            if (this.start_tape_delay > 0f)
            {
                if (!this.audiosource_audio_content.isPlaying)
                {
                    this.start_tape_delay = Mathf.Max(0f, this.start_tape_delay - Time.deltaTime);
                    if (this.start_tape_delay == 0f)
                    {
                        this.audiosource_audio_content.Play();
                    }
                }
            }
            else
            {
                if (this.stop_tape_delay > 0f)
                {
                    this.stop_tape_delay = Mathf.Max(0f, this.stop_tape_delay - Time.deltaTime);
                    if (this.stop_tape_delay == 0f)
                    {
                        this.tape_in_progress = false;
                        this.tapes_heard.Add(this.audiosource_audio_content.clip);
                        this.StopTapePlay();
                        if (this.tapes_heard.Count == this.total_tapes.Count)
                        {
                            this.StartWin();
                        }
                    }
                }
                else
                {
                    if (!this.audiosource_audio_content.isPlaying)
                    {
                        this.stop_tape_delay = Random.Range(0.5f, 3f);
                    }
                }
            }
        }
    }

    public void UpdateHealth()
    {
        if (this.dying)
        {
            this.health = this.health - Time.deltaTime;
        }
        if (this.health <= 0f)
        {
            this.health = 0f;
            this.SetDead(true);
            this.dying = false;
        }
    }

    public void UpdateHelpToggle()
    {
        if (Input.GetButton("Help Toggle"))
        {
            this.help_hold_time = this.help_hold_time + Time.deltaTime;
            if (this.show_help && (this.help_hold_time >= 1f))
            {
                this.show_advanced_help = true;
            }
        }
        if (Input.GetButtonDown("Help Toggle"))
        {
            if (!this.show_help)
            {
                this.show_help = true;
                this.help_ever_shown = true;
                this.just_started_help = true;
            }
            this.help_hold_time = 0f;
        }
        if (Input.GetButtonUp("Help Toggle"))
        {
            if ((this.show_help && (this.help_hold_time < 1f)) && !this.just_started_help)
            {
                this.show_help = false;
                this.show_advanced_help = false;
            }
            this.just_started_help = false;
        }
    }

    public void UpdateLevelResetButton()
    {
        if (Input.GetButtonDown("Level Reset"))
        {
            this.level_reset_hold = 0.01f;
        }
        if ((this.level_reset_hold != 0f) && Input.GetButton("Level Reset"))
        {
            this.level_reset_hold = this.level_reset_hold + Time.deltaTime;
            this.dead_volume_fade = Mathf.Min(1f - (this.level_reset_hold * 0.5f), this.dead_volume_fade);
            this.dead_fade = this.level_reset_hold * 0.5f;
            if (this.level_reset_hold >= 2f)
            {
                Application.LoadLevel(Application.loadedLevel);
                this.level_reset_hold = 0f;
            }
        }
        else
        {
            this.level_reset_hold = 0f;
        }
    }

    public void UpdateLevelEndEffects()
    {
        if (this.won)
        {
            this.win_fade = Mathf.Min(1f, this.win_fade + (Time.deltaTime * 0.1f));
            this.dead_volume_fade = Mathf.Max(0f, this.dead_volume_fade - (Time.deltaTime * 0.1f));
        }
        else
        {
            if (this.dead)
            {
                this.dead_fade = Mathf.Min(1f, this.dead_fade + (Time.deltaTime * 0.3f));
                this.dead_volume_fade = Mathf.Max(0f, this.dead_volume_fade - (Time.deltaTime * 0.23f));
                this.head_fall_vel = this.head_fall_vel - (9.8f * Time.deltaTime);
                this.head_fall = this.head_fall + (this.head_fall_vel * Time.deltaTime);
                this.head_tilt = this.head_tilt + (this.head_tilt_vel * Time.deltaTime);
                this.view_rotation_x = this.view_rotation_x + (this.head_tilt_x_vel * Time.deltaTime);
                this.view_rotation_y = this.view_rotation_y + (this.head_tilt_y_vel * Time.deltaTime);
                float min_fall = (this.character_controller.height * this.character_controller.transform.localScale.y) * -1f;
                if ((this.head_fall < min_fall) && (this.head_fall_vel < 0f))
                {
                    if (Mathf.Abs(this.head_fall_vel) > 0.5f)
                    {
                        this.head_recoil_spring_x.vel = this.head_recoil_spring_x.vel + (Random.Range(-10, 10) * Mathf.Abs(this.head_fall_vel));
                        this.head_recoil_spring_y.vel = this.head_recoil_spring_y.vel + (Random.Range(-10, 10) * Mathf.Abs(this.head_fall_vel));
                        this.head_tilt_vel = 0f;
                        this.head_tilt_x_vel = 0f;
                        this.head_tilt_y_vel = 0f;
                        if (!this.dead_body_fell)
                        {
                            this.PlaySoundFromGroup(this.sound_body_fall, 1f);
                            this.dead_body_fell = true;
                        }
                    }
                    this.head_fall_vel = this.head_fall_vel * -0.3f;
                }
                this.head_fall = Mathf.Max(min_fall, this.head_fall);
            }
            else
            {
                this.dead_fade = Mathf.Max(0f, this.dead_fade - (Time.deltaTime * 1.5f));
                this.dead_volume_fade = Mathf.Min(1f, this.dead_volume_fade + (Time.deltaTime * 1.5f));
            }
        }
    }

    public void UpdateLevelChange()
    {
        if (this.dead && (this.dead_volume_fade <= 0f))
        {
            Application.LoadLevel(Application.loadedLevel);
        }
        if (this.won && (this.dead_volume_fade <= 0f))
        {
            Application.LoadLevel("winscene");
        }
    }

    public void UpdateFallOffMapDeath()
    {
        if (this.transform.position.y < -1)
        {
            this.InstaKill();
        }
    }

    public void UpdateAimSpring()
    {
        RaycastHit hit = default(RaycastHit);
        bool offset_aim_target = false;
        if (((Input.GetButton("Hold To Aim") || this.aim_toggle) && !this.dead) && this.gun_instance)
        {
            this.aim_spring.target_state = 1f;
            if (Physics.Linecast(this.main_camera.transform.position, this.AimPos() + (this.AimDir() * 0.2f), out hit, 1 << 0))
            {
                this.aim_spring.target_state = Mathf.Clamp(1f - (Vector3.Distance(hit.point, this.main_camera.transform.position) / (this.GunDist() + 0.2f)), 0f, 1f);
                offset_aim_target = true;
            }
        }
        else
        {
            this.aim_spring.target_state = 0f;
        }
        this.aim_spring.Update();
        if (offset_aim_target)
        {
            this.aim_spring.target_state = 1f;
        }
    }

    public void UpdateCameraRotationControls()
    {
        this.rotation_y_min_leeway = Mathf.Lerp(0f, this.kRotationYMinLeeway, this.aim_spring.state);
        this.rotation_y_max_leeway = Mathf.Lerp(0f, this.kRotationYMaxLeeway, this.aim_spring.state);
        this.rotation_x_leeway = Mathf.Lerp(0f, this.kRotationXLeeway, this.aim_spring.state);
        if (PlayerPrefs.GetInt("lock_gun_to_center", 0) == 1)
        {
            this.rotation_y_min_leeway = 0;
            this.rotation_y_max_leeway = 0;
            this.rotation_x_leeway = 0;
        }
        this.sensitivity_x = PlayerPrefs.GetFloat("mouse_sensitivity", 1f) * 10f;
        this.sensitivity_y = PlayerPrefs.GetFloat("mouse_sensitivity", 1f) * 10f;
        if (PlayerPrefs.GetInt("mouse_invert", 0) == 1)
        {
            this.sensitivity_y = -Mathf.Abs(this.sensitivity_y);
        }
        else
        {
            this.sensitivity_y = Mathf.Abs(this.sensitivity_y);
        }
        bool in_menu = ((optionsmenuscript)GameObject.Find("gui_skin_holder").GetComponent(typeof(optionsmenuscript))).IsMenuShown();
        if (!this.dead && !in_menu)
        {
            this.rotation_x = this.rotation_x + (Input.GetAxis("Mouse X") * this.sensitivity_x);
            this.rotation_y = this.rotation_y + (Input.GetAxis("Mouse Y") * this.sensitivity_y);
            this.rotation_y = Mathf.Clamp(this.rotation_y, this.min_angle_y, this.max_angle_y);
            if ((Input.GetButton("Hold To Aim") || this.aim_toggle) && this.gun_instance)
            {
                this.view_rotation_y = Mathf.Clamp(this.view_rotation_y, this.rotation_y - this.rotation_y_min_leeway, this.rotation_y + this.rotation_y_max_leeway);
                this.view_rotation_x = Mathf.Clamp(this.view_rotation_x, this.rotation_x - this.rotation_x_leeway, this.rotation_x + this.rotation_x_leeway);
            }
            else
            {
                this.view_rotation_x = this.view_rotation_x + (Input.GetAxis("Mouse X") * this.sensitivity_x);
                this.view_rotation_y = this.view_rotation_y + (Input.GetAxis("Mouse Y") * this.sensitivity_y);
                this.view_rotation_y = Mathf.Clamp(this.view_rotation_y, this.min_angle_y, this.max_angle_y);
                this.rotation_y = Mathf.Clamp(this.rotation_y, this.view_rotation_y - this.rotation_y_max_leeway, this.view_rotation_y + this.rotation_y_min_leeway);
                this.rotation_x = Mathf.Clamp(this.rotation_x, this.view_rotation_x - this.rotation_x_leeway, this.view_rotation_x + this.rotation_x_leeway);
            }
        }
    }

    public void UpdateCameraAndPlayerTransformation()
    {
        this.main_camera.transform.localEulerAngles = new Vector3(-this.view_rotation_y, this.view_rotation_x, this.head_tilt);
        if (!this.disable_recoil)
        {
            this.main_camera.transform.localEulerAngles = this.main_camera.transform.localEulerAngles + new Vector3(this.head_recoil_spring_y.state, this.head_recoil_spring_x.state, 0);
        }

        {
            float _77 = this.view_rotation_x;
            Vector3 _78 = this.character_controller.transform.localEulerAngles;
            _78.y = _77;
            this.character_controller.transform.localEulerAngles = _78;
        }
        this.main_camera.transform.position = this.transform.position;

        {
            float _79 = this.main_camera.transform.position.y + ((this.character_controller.height * this.character_controller.transform.localScale.y) - 0.1f);
            Vector3 _80 = this.main_camera.transform.position;
            _80.y = _79;
            this.main_camera.transform.position = _80;
        }

        {
            float _81 = this.main_camera.transform.position.y + this.head_fall;
            Vector3 _82 = this.main_camera.transform.position;
            _82.y = _81;
            this.main_camera.transform.position = _82;
        }
    }

    public void UpdateGunTransformation()
    {
        Vector3 aim_dir = this.AimDir();
        Vector3 aim_pos = this.AimPos();
        Vector3 unaimed_dir = (this.transform.forward + new Vector3(0, -1, 0)).normalized;
        Vector3 unaimed_pos = this.main_camera.transform.position + (unaimed_dir * this.GunDist());
        if (this.disable_springs)
        {
            this.gun_instance.transform.position = this.mix(unaimed_pos, aim_pos, this.aim_spring.target_state);
            this.gun_instance.transform.forward = this.mix(unaimed_dir, aim_dir, this.aim_spring.target_state);
        }
        else
        {
            this.gun_instance.transform.position = this.mix(unaimed_pos, aim_pos, this.aim_spring.state);
            this.gun_instance.transform.forward = this.mix(unaimed_dir, aim_dir, this.aim_spring.state);
        }
        if (this.disable_springs)
        {
            this.ApplyPose("pose_slide_pull", this.slide_pose_spring.target_state);
            this.ApplyPose("pose_reload", this.reload_pose_spring.target_state);
            this.ApplyPose("pose_press_check", this.press_check_pose_spring.target_state);
            this.ApplyPose("pose_inspect_cylinder", this.inspect_cylinder_pose_spring.target_state);
            this.ApplyPose("pose_add_rounds", this.add_rounds_pose_spring.target_state);
            this.ApplyPose("pose_eject_rounds", this.eject_rounds_pose_spring.target_state);
        }
        else
        {
            this.ApplyPose("pose_slide_pull", this.slide_pose_spring.state);
            this.ApplyPose("pose_reload", this.reload_pose_spring.state);
            this.ApplyPose("pose_press_check", this.press_check_pose_spring.state);
            this.ApplyPose("pose_inspect_cylinder", this.inspect_cylinder_pose_spring.state);
            this.ApplyPose("pose_add_rounds", this.add_rounds_pose_spring.state);
            this.ApplyPose("pose_eject_rounds", this.eject_rounds_pose_spring.state);
        }
        if (!this.disable_recoil)
        {
            this.gun_instance.transform.RotateAround(this.gun_instance.transform.Find("point_recoil_rotate").position, this.gun_instance.transform.rotation * new Vector3(1, 0, 0), this.x_recoil_spring.state);
            this.gun_instance.transform.RotateAround(this.gun_instance.transform.Find("point_recoil_rotate").position, new Vector3(0, 1, 0), this.y_recoil_spring.state);
        }
    }

    public void UpdateFlashlightTransformation()
    {
        Vector3 flashlight_hold_pos = this.main_camera.transform.position + (this.main_camera.transform.rotation * new Vector3(-0.15f, -0.01f, 0.15f));
        Quaternion flashlight_hold_rot = this.main_camera.transform.rotation;
        Vector3 flashlight_pos = flashlight_hold_pos;
        Quaternion flashlight_rot = flashlight_hold_rot;
        this.held_flashlight.transform.position = flashlight_pos;
        this.held_flashlight.transform.rotation = flashlight_rot;
        this.held_flashlight.transform.RotateAround(this.held_flashlight.transform.Find("point_recoil_rotate").position, this.held_flashlight.transform.rotation * new Vector3(1, 0, 0), this.x_recoil_spring.state * 0.3f);
        this.held_flashlight.transform.RotateAround(this.held_flashlight.transform.Find("point_recoil_rotate").position, new Vector3(0, 1, 0), this.y_recoil_spring.state * 0.3f);
        flashlight_pos = this.held_flashlight.transform.position;
        flashlight_rot = this.held_flashlight.transform.rotation;
        if (this.gun_instance)
        {
            this.flashlight_aim_pos = this.gun_instance.transform.position + (this.gun_instance.transform.rotation * new Vector3(0.07f, -0.03f, 0f));
            this.flashlight_aim_rot = this.gun_instance.transform.rotation;
            this.flashlight_aim_pos = this.flashlight_aim_pos - this.main_camera.transform.position;
            this.flashlight_aim_pos = Quaternion.Inverse(this.main_camera.transform.rotation) * this.flashlight_aim_pos;
            this.flashlight_aim_rot = Quaternion.Inverse(this.main_camera.transform.rotation) * this.flashlight_aim_rot;
        }
        if (this.disable_springs)
        {
            flashlight_pos = this.mix(flashlight_pos, (this.main_camera.transform.rotation * this.flashlight_aim_pos) + this.main_camera.transform.position, this.aim_spring.target_state);
            flashlight_rot = this.mix(flashlight_rot, this.main_camera.transform.rotation * this.flashlight_aim_rot, this.aim_spring.target_state);
        }
        else
        {
            flashlight_pos = this.mix(flashlight_pos, (this.main_camera.transform.rotation * this.flashlight_aim_pos) + this.main_camera.transform.position, this.aim_spring.state);
            flashlight_rot = this.mix(flashlight_rot, this.main_camera.transform.rotation * this.flashlight_aim_rot, this.aim_spring.state);
        }
        Vector3 flashlight_mouth_pos = this.main_camera.transform.position + (this.main_camera.transform.rotation * new Vector3(0f, -0.08f, 0.05f));
        Quaternion flashlight_mouth_rot = this.main_camera.transform.rotation;
        this.flashlight_mouth_spring.target_state = 0f;
        if (this.magazine_instance_in_hand)
        {
            this.flashlight_mouth_spring.target_state = 1f;
        }
        this.flashlight_mouth_spring.target_state = Mathf.Max(this.flashlight_mouth_spring.target_state, ((((this.inspect_cylinder_pose_spring.state + this.eject_rounds_pose_spring.state) + (this.press_check_pose_spring.state / 0.6f)) + (this.reload_pose_spring.state / 0.7f)) + this.slide_pose_spring.state) * this.aim_spring.state);
        this.flashlight_mouth_spring.Update();
        if (this.disable_springs)
        {
            flashlight_pos = this.mix(flashlight_pos, flashlight_mouth_pos, this.flashlight_mouth_spring.target_state);
            flashlight_rot = this.mix(flashlight_rot, flashlight_mouth_rot, this.flashlight_mouth_spring.target_state);
            flashlight_pos = this.mix(flashlight_pos, this.flash_ground_pos, this.flash_ground_pose_spring.target_state);
            flashlight_rot = this.mix(flashlight_rot, this.flash_ground_rot, this.flash_ground_pose_spring.target_state);
        }
        else
        {
            flashlight_pos = this.mix(flashlight_pos, flashlight_mouth_pos, this.flashlight_mouth_spring.state);
            flashlight_rot = this.mix(flashlight_rot, flashlight_mouth_rot, this.flashlight_mouth_spring.state);
            flashlight_pos = this.mix(flashlight_pos, this.flash_ground_pos, this.flash_ground_pose_spring.state);
            flashlight_rot = this.mix(flashlight_rot, this.flash_ground_rot, this.flash_ground_pose_spring.state);
        }
        this.held_flashlight.transform.position = flashlight_pos;
        this.held_flashlight.transform.rotation = flashlight_rot;
    }

    public void UpdateMagazineTransformation()
    {
        if (this.gun_instance)
        {
            this.mag_pos = this.gun_instance.transform.position;
            this.mag_rot = this.gun_instance.transform.rotation;
            this.mag_pos = this.mag_pos + (this.gun_instance.transform.Find("point_mag_to_insert").position - this.gun_instance.transform.Find("point_mag_inserted").position);
        }
        if ((this.mag_stage == HandMagStage.HOLD) || (this.mag_stage == HandMagStage.HOLD_TO_INSERT))
        {
            Mag mag_script = (Mag)this.magazine_instance_in_hand.GetComponent(typeof(Mag));
            Vector3 hold_pos = this.main_camera.transform.position + (this.main_camera.transform.rotation * mag_script.holdOffset);
            Quaternion hold_rot = (this.main_camera.transform.rotation * Quaternion.AngleAxis(mag_script.holdRotation.x, new Vector3(0, 1, 0))) * Quaternion.AngleAxis(mag_script.holdRotation.y, new Vector3(1, 0, 0));
            if (this.disable_springs)
            {
                hold_pos = this.mix(hold_pos, this.mag_ground_pos, this.mag_ground_pose_spring.target_state);
                hold_rot = this.mix(hold_rot, this.mag_ground_rot, this.mag_ground_pose_spring.target_state);
            }
            else
            {
                hold_pos = this.mix(hold_pos, this.mag_ground_pos, this.mag_ground_pose_spring.state);
                hold_rot = this.mix(hold_rot, this.mag_ground_rot, this.mag_ground_pose_spring.state);
            }
            if (this.hold_pose_spring.state != 1f)
            {
                float amount = this.hold_pose_spring.state;
                if (this.disable_springs)
                {
                    amount = this.hold_pose_spring.target_state;
                }
                this.magazine_instance_in_hand.transform.position = this.mix(this.mag_pos, hold_pos, amount);
                this.magazine_instance_in_hand.transform.rotation = this.mix(this.mag_rot, hold_rot, amount);
            }
            else
            {
                this.magazine_instance_in_hand.transform.position = hold_pos;
                this.magazine_instance_in_hand.transform.rotation = hold_rot;
            }
        }
        else
        {
            this.magazine_instance_in_hand.transform.position = this.mag_pos;
            this.magazine_instance_in_hand.transform.rotation = this.mag_rot;
        }
    }

    public void UpdateInventoryTransformation()
    {
        WeaponSlot slot;
        var i = 0;
        for (i = 0; i < 10; ++i)
        {
            slot = weapon_slots[i];
            if (slot.type == WeaponSlotType.EMPTY)
            {
                continue;
            }
            slot.obj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
        for (i = 0; i < 10; ++i)
        {
            slot = weapon_slots[i];
            if (slot.type == WeaponSlotType.EMPTY)
            {
                continue;
            }
            var start_pos = main_camera.transform.position + slot.start_pos;
            var start_rot = main_camera.transform.rotation * slot.start_rot;
            if (slot.type == WeaponSlotType.EMPTYING)
            {
                start_pos = slot.obj.transform.position;
                start_rot = slot.obj.transform.rotation;
                if (Mathf.Abs(slot.spring.vel) <= 0.01 && slot.spring.state <= 0.01)
                {
                    slot.type = WeaponSlotType.EMPTY;
                    slot.spring.state = 0.0f;
                }
            }
            var scale = 0.0f;
            if (disable_springs)
            {
                var camera = main_camera.GetComponent<Camera>();
                slot.obj.transform.position = mix(
                    start_pos,
                    main_camera.transform.position + camera.ScreenPointToRay(new Vector3(camera.pixelWidth * (0.05f + i * 0.15f), camera.pixelHeight * 0.17f, 0)).direction * 0.3f,
                    slot.spring.target_state);
                scale = 0.3f * slot.spring.target_state + (1.0f - slot.spring.target_state);
                var localScale = slot.obj.transform.localScale * scale;
                slot.obj.transform.localScale = localScale;
                slot.obj.transform.rotation = mix(
                    start_rot,
                    main_camera.transform.rotation * Quaternion.AngleAxis(90, new Vector3(0, 1, 0)),
                    slot.spring.target_state);
            }
            else
            {
                var camera = main_camera.GetComponent<Camera>();
                slot.obj.transform.position = mix(
                    start_pos,
                    main_camera.transform.position + camera.ScreenPointToRay(new Vector3(camera.pixelWidth * (0.05f + i * 0.15f), camera.pixelHeight * 0.17f, 0)).direction * 0.3f,
                    slot.spring.state);
                scale = 0.3f * slot.spring.state + (1.0f - slot.spring.state);
                var localScale = slot.obj.transform.localScale * scale;
                slot.obj.transform.localScale = localScale;
                slot.obj.transform.rotation = mix(
                    start_rot,
                    main_camera.transform.rotation * Quaternion.AngleAxis(90, new Vector3(0, 1, 0)),
                    slot.spring.state);
            }
            var renderers = slot.obj.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                // renderer.castShadows = false;
                renderer.shadowCastingMode = ShadowCastingMode.Off;
            }
            slot.spring.Update();
        }
    }

    public void UpdateLooseBulletDisplay()
    {
        object revolver_open = this.gun_instance ? ((Gun)this.gun_instance.GetComponent(typeof(Gun))).IsCylinderOpen() : false;
        if ((((this.mag_stage == HandMagStage.HOLD) && !this.gun_instance) || (this.picked_up_bullet_delay > 0f)) || (revolver_open != null))
        {
            this.show_bullet_spring.target_state = 1f;
            this.picked_up_bullet_delay = Mathf.Max(0f, this.picked_up_bullet_delay - Time.deltaTime);
        }
        else
        {
            this.show_bullet_spring.target_state = 0f;
        }
        this.show_bullet_spring.Update();
        int i = 0;
        while (i < this.loose_bullets.Count)
        {
            Spring spring = (Spring)this.loose_bullet_spring[i];
            spring.Update();
            GameObject bullet = (GameObject)this.loose_bullets[i];
            bullet.transform.position = this.main_camera.transform.position + (this.main_camera.GetComponent<Camera>().ScreenPointToRay(new Vector3(0f, this.main_camera.GetComponent<Camera>().pixelHeight, 0)).direction * 0.3f);
            bullet.transform.position = bullet.transform.position + (this.main_camera.transform.rotation * new Vector3(0.02f, -0.01f, 0));
            bullet.transform.position = bullet.transform.position + (this.main_camera.transform.rotation * new Vector3(0.006f * i, 0f, 0));
            bullet.transform.position = bullet.transform.position + ((this.main_camera.transform.rotation * new Vector3(-0.03f, 0.03f, 0)) * (1f - this.show_bullet_spring.state));

            {
                float _95 = spring.state;
                Vector3 _96 = bullet.transform.localScale;
                _96.x = _95;
                bullet.transform.localScale = _96;
            }

            {
                float _97 = spring.state;
                Vector3 _98 = bullet.transform.localScale;
                _98.y = _97;
                bullet.transform.localScale = _98;
            }

            {
                float _99 = spring.state;
                Vector3 _100 = bullet.transform.localScale;
                _100.z = _99;
                bullet.transform.localScale = _100;
            }
            bullet.transform.rotation = this.main_camera.transform.rotation * Quaternion.AngleAxis(90, new Vector3(-1, 0, 0));
            Component[] renderers = bullet.GetComponentsInChildren(typeof(Renderer));
            foreach (Renderer renderer in renderers)
            {
                renderer.castShadows = false;
            }
            ++i;
        }
    }

    public void UpdateSprings()
    {
        this.slide_pose_spring.Update();
        this.reload_pose_spring.Update();
        this.press_check_pose_spring.Update();
        this.inspect_cylinder_pose_spring.Update();
        this.add_rounds_pose_spring.Update();
        this.eject_rounds_pose_spring.Update();
        this.x_recoil_spring.Update();
        this.y_recoil_spring.Update();
        this.head_recoil_spring_x.Update();
        this.head_recoil_spring_y.Update();
        if ((this.mag_stage == HandMagStage.HOLD) || (this.mag_stage == HandMagStage.HOLD_TO_INSERT))
        {
            this.hold_pose_spring.Update();
            this.mag_ground_pose_spring.Update();
        }
        this.flash_ground_pose_spring.Update();
    }

    public void UpdatePickupMagnet()
    {
        Vector3 attract_pos = this.transform.position - new Vector3(0, this.character_controller.height * 0.2f, 0);
        int i = 0;
        while (i < this.collected_rounds.Count)
        {
            GameObject round = this.collected_rounds[i] as GameObject;
            if (!round)
            {
                goto Label_for_13;
            }
            round.GetComponent<Rigidbody>().velocity = round.GetComponent<Rigidbody>().velocity + (((attract_pos - round.transform.position) * Time.deltaTime) * 20f);
            round.GetComponent<Rigidbody>().velocity = round.GetComponent<Rigidbody>().velocity * Mathf.Pow(0.1f, Time.deltaTime);
            if (Vector3.Distance(round.transform.position, attract_pos) < 0.5f)
            {
                if (round.gameObject.name == "cassette_tape(Clone)")
                {
                    ++this.unplayed_tapes;
                }
                else
                {
                    this.AddLooseBullet(true);
                    this.collected_rounds.RemoveAt(i);
                    this.PlaySoundFromGroup(this.sound_bullet_grab, 0.2f);
                }
                GameObject.Destroy(round);
            }
        Label_for_13:
            ++i;
        }
        this.collected_rounds.Remove(null);
    }

    public void Update()
    {
        this.UpdateTape();
        this.UpdateCheats();
        this.UpdateFallOffMapDeath();
        this.UpdateHealth();
        this.UpdateHelpToggle();
        this.UpdateLevelResetButton();
        this.UpdateLevelChange();
        this.UpdateLevelEndEffects();
        AudioListener.volume = this.dead_volume_fade * PlayerPrefs.GetFloat("master_volume", 1f);
        this.UpdateAimSpring();
        this.UpdateCameraRotationControls();
        this.UpdateCameraAndPlayerTransformation();
        if (this.gun_instance)
        {
            this.UpdateGunTransformation();
        }
        if (this.held_flashlight)
        {
            this.UpdateFlashlightTransformation();
        }
        if (this.magazine_instance_in_hand)
        {
            this.UpdateMagazineTransformation();
        }
        this.UpdateInventoryTransformation();
        this.UpdateLooseBulletDisplay();
        bool in_menu = ((optionsmenuscript)GameObject.Find("gui_skin_holder").GetComponent(typeof(optionsmenuscript))).IsMenuShown();
        if (!this.dead && !in_menu)
        {
            this.HandleControls();
        }
        this.UpdateSprings();
        this.UpdatePickupMagnet();
    }

    public void FixedUpdate()
    {
    }

    public bool ShouldHolsterGun()
    {
        if (this.loose_bullets == null)
        {
            return false;
        }
        if (this.loose_bullets.Count > 0)
        {
        }
        else
        {
            return false;
        }
        if (this.magazine_instance_in_hand)
        {
        }
        else
        {
            return false;
        }
        if (((Mag)this.magazine_instance_in_hand.GetComponent(typeof(Mag))).NumRounds() == 0)
        {
        }
        else
        {
            return false;
        }
        return true;
    }

    public bool CanLoadBulletsInMag()
    {
        return ((!this.gun_instance && (this.mag_stage == HandMagStage.HOLD)) && (this.loose_bullets.Count > 0)) && !((Mag)this.magazine_instance_in_hand.GetComponent(typeof(Mag))).IsFull();
    }

    public bool CanRemoveBulletFromMag()
    {
        return (!this.gun_instance && (this.mag_stage == HandMagStage.HOLD)) && (((Mag)this.magazine_instance_in_hand.GetComponent(typeof(Mag))).NumRounds() > 0);
    }

    public bool ShouldDrawWeapon()
    {
        return !this.gun_instance && !this.CanLoadBulletsInMag();
    }

    public int GetMostLoadedMag()
    {
        int max_rounds = 0;
        int max_rounds_slot = -1;
        int i = 0;
        while (i < 10)
        {
            if (this.weapon_slots[i].type == WeaponSlotType.MAGAZINE)
            {
                int rounds = ((Mag)this.weapon_slots[i].obj.GetComponent(typeof(Mag))).NumRounds();
                if (rounds > max_rounds)
                {
                    max_rounds_slot = i + 1;
                    max_rounds = rounds;
                }
            }
            ++i;
        }
        return max_rounds_slot;
    }

    public bool ShouldPutMagInInventory()
    {
        int rounds = ((Mag)this.magazine_instance_in_hand.GetComponent(typeof(Mag))).NumRounds();
        int most_loaded = this.GetMostLoadedMag();
        if (most_loaded == -1)
        {
            return false;
        }
        if (((Mag)this.weapon_slots[most_loaded - 1].obj.GetComponent(typeof(Mag))).NumRounds() > rounds)
        {
            return true;
        }
        return false;
    }

    public int GetEmptySlot()
    {
        int empty_slot = -1;
        int i = 0;
        while (i < 10)
        {
            if (this.weapon_slots[i].type == WeaponSlotType.EMPTY)
            {
                empty_slot = i + 1;
                break;
            }
            ++i;
        }
        return empty_slot;
    }

    public int GetFlashlightSlot()
    {
        int flashlight_slot = -1;
        int i = 0;
        while (i < 10)
        {
            if (this.weapon_slots[i].type == WeaponSlotType.FLASHLIGHT)
            {
                flashlight_slot = i + 1;
                break;
            }
            ++i;
        }
        return flashlight_slot;
    }

    public void OnGUI()
    {
        List<DisplayLine> display_text = new List<DisplayLine>();
        Gun gun_script = null;
        if (this.gun_instance)
        {
            gun_script = (Gun)this.gun_instance.GetComponent(typeof(Gun));
        }
        display_text.Add(new DisplayLine((this.tapes_heard.Count + " tapes absorbed out of ") + this.total_tapes.Count, true));
        if (!this.show_help)
        {
            display_text.Add(new DisplayLine("View help: Press [ ? ]", !this.help_ever_shown));
        }
        else
        {
            display_text.Add(new DisplayLine("Hide help: Press [ ? ]", false));
            display_text.Add(new DisplayLine("", false));
            if (this.tape_in_progress)
            {
                display_text.Add(new DisplayLine("Pause/Resume tape player: [ x ]", false));
            }
            display_text.Add(new DisplayLine("Look: [ move mouse ]", false));
            display_text.Add(new DisplayLine("Move: [ WASD ]", false));
            display_text.Add(new DisplayLine("Jump: [ space ]", false));
            display_text.Add(new DisplayLine("Pick up nearby: hold [ g ]", this.ShouldPickUpNearby()));
            if (this.held_flashlight)
            {
                int empty_slot = this.GetEmptySlot();
                if (empty_slot != -1)
                {
                    string str = "Put flashlight in inventory: tap [ ";
                    str = str + empty_slot;
                    str = str + " ]";
                    display_text.Add(new DisplayLine(str, false));
                }
            }
            else
            {
                int flashlight_slot = this.GetFlashlightSlot();
                if (flashlight_slot != -1)
                {
                    string str = "Equip flashlight: tap [ ";
                    str = str + flashlight_slot;
                    str = str + " ]";
                    display_text.Add(new DisplayLine(str, true));
                }
            }
            if (this.gun_instance)
            {
                display_text.Add(new DisplayLine("Fire weapon: tap [ left mouse button ]", false));
                bool should_aim = this.aim_spring.state < 0.5f;
                display_text.Add(new DisplayLine("Aim weapon: hold [ right mouse button ]", should_aim));
                display_text.Add(new DisplayLine("Aim weapon: tap [ q ]", should_aim));
                display_text.Add(new DisplayLine("Holster weapon: tap [ ~ ]", this.ShouldHolsterGun()));
            }
            else
            {
                display_text.Add(new DisplayLine("Draw weapon: tap [ ~ ]", this.ShouldDrawWeapon()));
            }
            if (this.gun_instance)
            {
                if (gun_script.HasSlide())
                {
                    display_text.Add(new DisplayLine("Pull back slide: hold [ r ]", gun_script.ShouldPullSlide() ? true : false));
                    display_text.Add(new DisplayLine("Release slide lock: tap [ t ]", gun_script.ShouldReleaseSlideLock() ? true : false));
                }
                if (gun_script.HasSafety())
                {
                    display_text.Add(new DisplayLine("Toggle safety: tap [ v ]", gun_script.IsSafetyOn() ? true : false));
                }
                if (gun_script.HasAutoMod())
                {
                    display_text.Add(new DisplayLine("Toggle full-automatic: tap [ v ]", gun_script.ShouldToggleAutoMod() ? true : false));
                }
                if (gun_script.HasHammer())
                {
                    display_text.Add(new DisplayLine("Pull back hammer: hold [ f ]", gun_script.ShouldPullBackHammer() ? true : false));
                }
                if (gun_script.gun_type == GunType.REVOLVER)
                {
                    if (!gun_script.IsCylinderOpen())
                    {
                        display_text.Add(new DisplayLine("Open cylinder: tap [ e ]", gun_script.ShouldOpenCylinder() && (this.loose_bullets.Count != 0) ? true : false));
                    }
                    else
                    {
                        display_text.Add(new DisplayLine("Close cylinder: tap [ r ]", gun_script.ShouldCloseCylinder() || (this.loose_bullets.Count == 0) ? true : false));
                        display_text.Add(new DisplayLine("Extract casings: hold [ v ]", gun_script.ShouldExtractCasings() ? true : false));
                        display_text.Add(new DisplayLine("Insert bullet: tap [ z ]", gun_script.ShouldInsertBullet() && (this.loose_bullets.Count != 0) ? true : false));
                    }
                    display_text.Add(new DisplayLine("Spin cylinder: [ mousewheel ]", false));
                }
                if ((this.mag_stage == HandMagStage.HOLD) && !gun_script.IsThereAMagInGun())
                {
                    bool should_insert_mag = ((Mag)this.magazine_instance_in_hand.GetComponent(typeof(Mag))).NumRounds() >= 1;
                    display_text.Add(new DisplayLine("Insert magazine: tap [ z ]", should_insert_mag));
                }
                else
                {
                    if ((this.mag_stage == HandMagStage.EMPTY) && gun_script.IsThereAMagInGun())
                    {
                        display_text.Add(new DisplayLine("Eject magazine: tap [ e ]", gun_script.ShouldEjectMag() ? true : false));
                    }
                    else
                    {
                        if ((this.mag_stage == HandMagStage.EMPTY) && !gun_script.IsThereAMagInGun())
                        {
                            int max_rounds_slot = this.GetMostLoadedMag();
                            if (max_rounds_slot != -1)
                            {
                                display_text.Add(new DisplayLine(("Equip magazine: tap [ " + max_rounds_slot) + " ]", true));
                            }
                        }
                    }
                }
            }
            else
            {
                if (this.CanLoadBulletsInMag())
                {
                    display_text.Add(new DisplayLine("Insert bullet in magazine: tap [ z ]", true));
                }
                if (this.CanRemoveBulletFromMag())
                {
                    display_text.Add(new DisplayLine("Remove bullet from magazine: tap [ r ]", false));
                }
            }
            if (this.mag_stage == HandMagStage.HOLD)
            {
                int empty_slot = this.GetEmptySlot();
                if (empty_slot != -1)
                {
                    string str = "Put magazine in inventory: tap [ ";
                    str = str + empty_slot;
                    str = str + " ]";
                    display_text.Add(new DisplayLine(str, this.ShouldPutMagInInventory()));
                }
                display_text.Add(new DisplayLine("Drop magazine: tap [ e ]", false));
            }
            display_text.Add(new DisplayLine("", false));
            if (this.show_advanced_help)
            {
                display_text.Add(new DisplayLine("Advanced help:", false));
                display_text.Add(new DisplayLine("Toggle crouch: [ c ]", false));
                if (this.aim_spring.state < 0.5f)
                {
                    display_text.Add(new DisplayLine("Run: tap repeatedly [ w ]", false));
                }
                if (this.gun_instance)
                {
                    if (!gun_script.IsSafetyOn() && gun_script.IsHammerCocked())
                    {
                        display_text.Add(new DisplayLine("Decock: Hold [f], hold [LMB], release [f]", this.ShouldPickUpNearby()));
                    }
                    if (!gun_script.IsSlideLocked() && !gun_script.IsSafetyOn())
                    {
                        display_text.Add(new DisplayLine("Inspect chamber: hold [ t ] and then [ r ]", false));
                    }
                    if ((this.mag_stage == HandMagStage.EMPTY) && !gun_script.IsThereAMagInGun())
                    {
                        int max_rounds_slot = this.GetMostLoadedMag();
                        if (max_rounds_slot != -1)
                        {
                            display_text.Add(new DisplayLine(("Quick load magazine: double tap [ " + max_rounds_slot) + " ]", false));
                        }
                    }
                }
                display_text.Add(new DisplayLine("Reset game: hold [ l ]", false));
            }
            else
            {
                display_text.Add(new DisplayLine("Advanced help: Hold [ ? ]", false));
            }
        }
        GUIStyle style = this.holder.gui_skin.label;
        float width = Screen.width * 0.5f;
        int offset = 0;
        foreach (DisplayLine line in display_text)
        {
            if (line.bold)
            {
                style.fontStyle = FontStyle.Bold;
            }
            else
            {
                style.fontStyle = FontStyle.Normal;
            }
            style.fontSize = 18;
            style.normal.textColor = new Color(0, 0, 0);
            GUI.Label(new Rect(width + 0.5f, offset + 0.5f, width + 0.5f, (offset + 20) + 0.5f), line.str, style);
            if (line.bold)
            {
                style.normal.textColor = new Color(1, 1, 1);
            }
            else
            {
                style.normal.textColor = new Color(0.7f, 0.7f, 0.7f);
            }
            GUI.Label(new Rect(width, offset, width, offset + 30), line.str, style);
            offset = offset + 20;
        }
        if (this.dead_fade > 0f)
        {
            if (!this.texture_death_screen)
            {
                Debug.LogError("Assign a Texture in the inspector.");
                return;
            }
            GUI.color = new Color(0, 0, 0, this.dead_fade);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), this.texture_death_screen, ScaleMode.StretchToFill, true);
        }
        if (this.win_fade > 0f)
        {
            GUI.color = new Color(1, 1, 1, this.win_fade);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), this.texture_death_screen, ScaleMode.StretchToFill, true);
        }
    }

    public AimScript()
    {
        this.sensitivity_x = 2f;
        this.sensitivity_y = 2f;
        this.min_angle_y = -89f;
        this.max_angle_y = 89f;
        this.disable_recoil = true;
        this.kAimSpringStrength = 100f;
        this.kAimSpringDamping = 1E-05f;
        this.aim_spring = new Spring(0, 0, this.kAimSpringStrength, this.kAimSpringDamping);
        this.flashlight_mouth_spring = new Spring(0, 0, this.kAimSpringStrength, this.kAimSpringDamping);
        this.flash_ground_pose_spring = new Spring(0, 0, this.kAimSpringStrength, this.kAimSpringDamping);
        this.kRotationXLeeway = 5f;
        this.kRotationYMinLeeway = 20f;
        this.kRotationYMaxLeeway = 10f;
        this.kRecoilSpringStrength = 800f;
        this.kRecoilSpringDamping = 1E-06f;
        this.x_recoil_spring = new Spring(0, 0, this.kRecoilSpringStrength, this.kRecoilSpringDamping);
        this.y_recoil_spring = new Spring(0, 0, this.kRecoilSpringStrength, this.kRecoilSpringDamping);
        this.head_recoil_spring_x = new Spring(0, 0, this.kRecoilSpringStrength, this.kRecoilSpringDamping);
        this.head_recoil_spring_y = new Spring(0, 0, this.kRecoilSpringStrength, this.kRecoilSpringDamping);
        this.kGunDistance = 0.3f;
        this.slide_pose_spring = new Spring(0, 0, this.kAimSpringStrength, this.kAimSpringDamping);
        this.reload_pose_spring = new Spring(0, 0, this.kAimSpringStrength, this.kAimSpringDamping);
        this.press_check_pose_spring = new Spring(0, 0, this.kAimSpringStrength, this.kAimSpringDamping);
        this.inspect_cylinder_pose_spring = new Spring(0, 0, this.kAimSpringStrength, this.kAimSpringDamping);
        this.add_rounds_pose_spring = new Spring(0, 0, this.kAimSpringStrength, this.kAimSpringDamping);
        this.eject_rounds_pose_spring = new Spring(0, 0, this.kAimSpringStrength, this.kAimSpringDamping);
        this.gun_tilt = GunTilt.CENTER;
        this.hold_pose_spring = new Spring(0, 0, this.kAimSpringStrength, this.kAimSpringDamping);
        this.mag_ground_pose_spring = new Spring(0, 0, this.kAimSpringStrength, this.kAimSpringDamping);
        this.kMaxHeadRecoil = 10;
        this.head_recoil_delay = new float[this.kMaxHeadRecoil];
        this.mag_stage = HandMagStage.EMPTY;
        this.collected_rounds = new List<GameObject>();
        this.target_weapon_slot = -2;
        this.show_bullet_spring = new Spring(0, 0, this.kAimSpringStrength, this.kAimSpringDamping);
        this.dead_fade = 1f;
        this.tapes_heard = new List<AudioClip>();
        this.tapes_remaining = new List<AudioClip>();
        this.total_tapes = new List<AudioClip>();
        this.weapon_slots = new WeaponSlot[10];
        this.health = 1f;
    }

}