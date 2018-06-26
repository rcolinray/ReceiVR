using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public enum GunType
{
    AUTOMATIC = 0,
    REVOLVER = 1
}

public enum ActionType
{
    DOUBLE = 0,
    SINGLE = 1
}

public enum PressureState
{
    NONE = 0,
    INITIAL = 1,
    CONTINUING = 2
}

public enum RoundState
{
    EMPTY = 0,
    READY = 1,
    FIRED = 2,
    LOADING = 3,
    JAMMED = 4
}

public enum SlideStage
{
    NOTHING = 0,
    PULLBACK = 1,
    HOLD = 2
}

public enum Thumb
{
    ON_HAMMER = 0,
    OFF_HAMMER = 1,
    SLOW_LOWERING = 2
}

public enum Safety
{
    OFF = 0,
    ON = 1
}

public enum MagStage
{
    OUT = 0,
    INSERTING = 1,
    IN = 2,
    REMOVING = 3
}

public enum AutoModStage
{
    ENABLED = 0,
    DISABLED = 1
}

public enum YolkStage
{
    CLOSED = 0,
    OPENING = 1,
    OPEN = 2,
    CLOSING = 3
}

public enum ExtractorRodStage
{
    CLOSED = 0,
    OPENING = 1,
    OPEN = 2,
    CLOSING = 3
}

[System.Serializable]
public class CylinderState : object
{
    public GameObject obj;
    public bool canFire;
    public float seated;
    public bool falling;
}
[System.Serializable]
public partial class Gun : MonoBehaviour
{
    public GunType gun_type;
    public ActionType action_type;
    public AudioClip[] sound_gunshot_bigroom;
    public AudioClip[] sound_gunshot_smallroom;
    public AudioClip[] sound_gunshot_open;
    public AudioClip[] sound_mag_eject_button;
    public AudioClip[] sound_mag_ejection;
    public AudioClip[] sound_mag_insertion;
    public AudioClip[] sound_slide_back;
    public AudioClip[] sound_slide_front;
    public AudioClip[] sound_safety;
    public AudioClip[] sound_bullet_eject;
    public AudioClip[] sound_cylinder_open;
    public AudioClip[] sound_cylinder_close;
    public AudioClip[] sound_extractor_rod_open;
    public AudioClip[] sound_extractor_rod_close;
    public AudioClip[] sound_cylinder_rotate;
    public AudioClip[] sound_hammer_cock;
    public AudioClip[] sound_hammer_decock;
    private float kGunMechanicVolume;
    public bool add_head_recoil;
    public float recoil_transfer_x;
    public float recoil_transfer_y;
    public float rotation_transfer_x;
    public float rotation_transfer_y;
    public Vector3 old_pos;
    public Vector3 velocity;
    public GameObject magazine_obj;
    public GameObject bullet_obj;
    public GameObject muzzle_flash;
    public GameObject shell_casing;
    public GameObject casing_with_bullet;
    public bool ready_to_remove_mag;
    public PressureState pressure_on_trigger;
    public float trigger_pressed;
    private GameObject round_in_chamber;
    private RoundState round_in_chamber_state;
    private GameObject magazine_instance_in_gun;
    private float mag_offset;
    private bool slide_pressure;
    private Vector3 slide_rel_pos;
    private float slide_amount;
    private bool slide_lock;
    private SlideStage slide_stage;
    private Thumb thumb_on_hammer;
    private Vector3 hammer_rel_pos;
    private Quaternion hammer_rel_rot;
    private float hammer_cocked;
    private Vector3 safety_rel_pos;
    private Quaternion safety_rel_rot;
    private float safety_off;
    private Safety safety;
    private float kSlideLockPosition;
    private float kPressCheckPosition;
    private float kSlideLockSpeed;
    private MagStage mag_stage;
    private float mag_seated;
    private AutoModStage auto_mod_stage;
    private float auto_mod_amount;
    private Vector3 auto_mod_rel_pos;
    private bool fired_once_this_pull;
    private bool has_slide;
    private bool has_safety;
    private bool has_hammer;
    private bool has_auto_mod;
    private Quaternion yolk_pivot_rel_rot;
    private float yolk_open;
    private YolkStage yolk_stage;
    private float cylinder_rotation;
    private float cylinder_rotation_vel;
    private int active_cylinder;
    private int target_cylinder_offset;
    private ExtractorRodStage extractor_rod_stage;
    private float extractor_rod_amount;
    private bool extracted;
    private Vector3 extractor_rod_rel_pos;
    public bool disable_springs;
    private int cylinder_capacity;
    public CylinderState[] cylinders;
    public bool IsAddingRounds()
    {
        if (this.yolk_stage == YolkStage.OPEN)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsEjectingRounds()
    {
        if (this.extractor_rod_stage != ExtractorRodStage.CLOSED)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public Transform GetHammer()
    {
        Transform hammer = this.transform.Find("hammer");
        if (!hammer)
        {
            hammer = this.transform.Find("hammer_pivot");
        }
        return hammer;
    }

    public Transform GetHammerCocked()
    {
        Transform hammer = this.transform.Find("point_hammer_cocked");
        if (!hammer)
        {
            hammer = this.transform.Find("hammer_pivot");
        }
        return hammer;
    }

    public void Start()
    {
        this.disable_springs = false;
        if (this.transform.Find("slide"))
        {
            Transform slide = this.transform.Find("slide");
            this.has_slide = true;
            this.slide_rel_pos = slide.localPosition;
            if (slide.Find("auto mod toggle"))
            {
                this.has_auto_mod = true;
                this.auto_mod_rel_pos = slide.Find("auto mod toggle").localPosition;
                if (Random.Range(0, 2) == 0)
                {
                    this.auto_mod_amount = 1f;
                    this.auto_mod_stage = AutoModStage.ENABLED;
                }
            }
        }
        Transform hammer = this.GetHammer();
        if (hammer)
        {
            this.has_hammer = true;
            this.hammer_rel_pos = hammer.localPosition;
            this.hammer_rel_rot = hammer.localRotation;
        }
        Transform extractor_rod = null;
        Transform yolk_pivot = this.transform.Find("yolk_pivot");
        if (yolk_pivot)
        {
            this.yolk_pivot_rel_rot = yolk_pivot.localRotation;
            Transform yolk = yolk_pivot.Find("yolk");
            if (yolk)
            {
                Transform cylinder_assembly = yolk.Find("cylinder_assembly");
                if (cylinder_assembly)
                {
                    extractor_rod = cylinder_assembly.Find("extractor_rod");
                    if (extractor_rod)
                    {
                        this.extractor_rod_rel_pos = extractor_rod.localPosition;
                    }
                }
            }
        }
        if (this.gun_type == GunType.AUTOMATIC)
        {
            this.magazine_instance_in_gun = UnityEngine.Object.Instantiate(this.magazine_obj);
            this.magazine_instance_in_gun.transform.parent = this.transform;
            Component[] renderers = this.magazine_instance_in_gun.GetComponentsInChildren(typeof(Renderer));
            foreach (Renderer renderer in renderers)
            {
                renderer.shadowCastingMode = ShadowCastingMode.Off;
            }
            if (Random.Range(0, 2) == 0)
            {
                this.round_in_chamber = UnityEngine.Object.Instantiate(this.casing_with_bullet, this.transform.Find("point_chambered_round").position, this.transform.Find("point_chambered_round").rotation);
                this.round_in_chamber.transform.parent = this.transform;
                this.round_in_chamber.transform.localScale = new Vector3(1f, 1f, 1f);
                renderers = this.round_in_chamber.GetComponentsInChildren(typeof(Renderer));
                foreach (Renderer renderer in renderers)
                {
                    renderer.shadowCastingMode = ShadowCastingMode.Off;
                }
            }
            if (Random.Range(0, 2) == 0)
            {
                this.slide_amount = this.kSlideLockPosition;
                this.slide_lock = true;
            }
        }
        if (this.gun_type == GunType.REVOLVER)
        {
            this.cylinders = new CylinderState[this.cylinder_capacity];
            int i = 0;
            while (i < this.cylinder_capacity)
            {
                this.cylinders[i] = new CylinderState();
                if (Random.Range(0, 2) == 0)
                {
                    goto Label_for_19;
                }
                string name = "point_chamber_" + (i + 1);
                this.cylinders[i].obj = UnityEngine.Object.Instantiate(this.casing_with_bullet, extractor_rod.Find(name).position, extractor_rod.Find(name).rotation);
                this.cylinders[i].obj.transform.localScale = new Vector3(1f, 1f, 1f);
                this.cylinders[i].canFire = true;
                this.cylinders[i].seated = Random.Range(0f, 0.5f);
                var renderers = this.cylinders[i].obj.GetComponentsInChildren(typeof(Renderer));
                foreach (Renderer renderer in renderers)
                {
                    renderer.shadowCastingMode = ShadowCastingMode.Off;
                }
            Label_for_19:
                ++i;
            }
        }
        if ((Random.Range(0, 2) == 0) && this.has_hammer)
        {
            this.hammer_cocked = 0f;
        }
        if (this.transform.Find("safety"))
        {
            this.has_safety = true;
            this.safety_rel_pos = this.transform.Find("safety").localPosition;
            this.safety_rel_rot = this.transform.Find("safety").localRotation;
            if (Random.Range(0, 4) == 0)
            {
                this.safety_off = 0f;
                this.safety = Safety.ON;
                this.slide_amount = 0f;
                this.slide_lock = false;
            }
        }
    }

    public Mag MagScript()
    {
        return this.magazine_instance_in_gun.GetComponent("mag_script") as Mag;
    }

    public bool ShouldPullSlide()
    {
        if (this.gun_type != GunType.AUTOMATIC)
        {
            return false;
        }
        return (!this.round_in_chamber && (this.magazine_instance_in_gun != null)) && (((Mag)this.magazine_instance_in_gun.GetComponent(typeof(Mag))).NumRounds() > 0);
    }

    public bool ShouldReleaseSlideLock()
    {
        return this.round_in_chamber ? this.slide_lock : false;
    }

    public bool ShouldEjectMag()
    {
        if (this.gun_type != GunType.AUTOMATIC)
        {
            return false;
        }
        return this.magazine_instance_in_gun ? ((Mag)this.magazine_instance_in_gun.GetComponent(typeof(Mag))).NumRounds() == 0 : false;
    }

    public bool ChamberRoundFromMag()
    {
        if (this.gun_type != GunType.AUTOMATIC)
        {
            return false;
        }
        if ((this.magazine_instance_in_gun && (this.MagScript().NumRounds() > 0)) && (this.mag_stage == MagStage.IN))
        {
            if (!this.round_in_chamber)
            {
                this.MagScript().RemoveRound();
                this.round_in_chamber = UnityEngine.Object.Instantiate(this.casing_with_bullet, this.transform.Find("point_load_round").position, this.transform.Find("point_load_round").rotation);
                Component[] renderers = this.round_in_chamber.GetComponentsInChildren(typeof(Renderer));
                foreach (Renderer renderer in renderers)
                {
                    renderer.shadowCastingMode = ShadowCastingMode.Off;
                }
                this.round_in_chamber.transform.parent = this.transform;
                this.round_in_chamber.transform.localScale = new Vector3(1f, 1f, 1f);
                this.round_in_chamber_state = RoundState.LOADING;
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    public void PullSlideBack()
    {
        if (this.gun_type != GunType.AUTOMATIC)
        {
            return;
        }
        this.slide_amount = 1f;
        if ((this.slide_lock && (this.mag_stage == MagStage.IN)) && (!this.magazine_instance_in_gun || (this.MagScript().NumRounds() == 0)))
        {
            return;
        }
        this.slide_lock = false;
        if (this.round_in_chamber && ((this.round_in_chamber_state == RoundState.FIRED) || (this.round_in_chamber_state == RoundState.READY)))
        {
            this.round_in_chamber.AddComponent(typeof(Rigidbody));
            this.PlaySoundFromGroup(this.sound_bullet_eject, this.kGunMechanicVolume);
            this.round_in_chamber.transform.parent = null;
            this.round_in_chamber.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
            this.round_in_chamber.GetComponent<Rigidbody>().velocity = this.velocity;
            this.round_in_chamber.GetComponent<Rigidbody>().velocity = this.round_in_chamber.GetComponent<Rigidbody>().velocity + (this.transform.rotation * new Vector3(Random.Range(2f, 4f), Random.Range(1f, 2f), Random.Range(-1f, -3f)));
            this.round_in_chamber.GetComponent<Rigidbody>().angularVelocity = new Vector3(Random.Range(-40f, 40f), Random.Range(-40f, 40f), Random.Range(-40f, 40f));
            this.round_in_chamber = null;
        }
        if (!this.ChamberRoundFromMag() && (this.mag_stage == MagStage.IN))
        {
            this.slide_lock = true;
        }
    }

    public void ReleaseSlideLock()
    {
        if (this.gun_type != GunType.AUTOMATIC)
        {
            return;
        }
        this.slide_lock = false;
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
        return a * Quaternion.AngleAxis(angle * -val, axis);
    }

    public void PlaySoundFromGroup(object[] group, float volume)
    {
        if (group.Length == 0)
        {
            return;
        }
        int which_shot = Random.Range(0, group.Length);
        this.GetComponent<AudioSource>().PlayOneShot((AudioClip)group[which_shot], volume * PlayerPrefs.GetFloat("sound_volume", 1f));
    }

    public void ApplyPressureToTrigger()
    {
        if (this.pressure_on_trigger == PressureState.NONE)
        {
            this.pressure_on_trigger = PressureState.INITIAL;
            this.fired_once_this_pull = false;
        }
        else
        {
            this.pressure_on_trigger = PressureState.CONTINUING;
        }
        if (this.yolk_stage != YolkStage.CLOSED)
        {
            return;
        }
        if (((((((this.pressure_on_trigger == PressureState.INITIAL) || (this.action_type == ActionType.DOUBLE)) && !this.slide_lock) && (this.thumb_on_hammer == Thumb.OFF_HAMMER)) && (this.hammer_cocked == 1f)) && (this.safety_off == 1f)) && ((this.auto_mod_stage == AutoModStage.ENABLED) || !this.fired_once_this_pull))
        {
            this.trigger_pressed = 1f;
            if ((this.gun_type == GunType.AUTOMATIC) && (this.slide_amount == 0f))
            {
                this.hammer_cocked = 0f;
                if (this.round_in_chamber && (this.round_in_chamber_state == RoundState.READY))
                {
                    this.fired_once_this_pull = true;
                    this.PlaySoundFromGroup(this.sound_gunshot_smallroom, 1f);
                    this.round_in_chamber_state = RoundState.FIRED;
                    GameObject.Destroy(this.round_in_chamber);
                    this.round_in_chamber = UnityEngine.Object.Instantiate(this.shell_casing, this.transform.Find("point_chambered_round").position, this.transform.rotation);
                    this.round_in_chamber.transform.parent = this.transform;
                    Component[] renderers = this.round_in_chamber.GetComponentsInChildren(typeof(Renderer));
                    foreach (Renderer renderer in renderers)
                    {
                        renderer.shadowCastingMode = ShadowCastingMode.Off;
                    }
                    UnityEngine.Object.Instantiate(this.muzzle_flash, this.transform.Find("point_muzzleflash").position, this.transform.Find("point_muzzleflash").rotation);
                    GameObject bullet = UnityEngine.Object.Instantiate(this.bullet_obj, this.transform.Find("point_muzzle").position, this.transform.Find("point_muzzle").rotation);
                    ((BulletScript)bullet.GetComponent(typeof(BulletScript))).SetVelocity(this.transform.forward * 251f);
                    this.PullSlideBack();
                    this.rotation_transfer_y = this.rotation_transfer_y + Random.Range(1f, 2f);
                    this.rotation_transfer_x = this.rotation_transfer_x + Random.Range(-1f, 1f);
                    this.recoil_transfer_x = this.recoil_transfer_x - Random.Range(150f, 300f);
                    this.recoil_transfer_y = this.recoil_transfer_y + Random.Range(-200f, 200f);
                    this.add_head_recoil = true;
                    return;
                }
                else
                {
                    this.PlaySoundFromGroup(this.sound_mag_eject_button, 0.5f);
                }
            }
            else
            {
                if (this.gun_type == GunType.REVOLVER)
                {
                    this.hammer_cocked = 0f;
                    int which_chamber = this.active_cylinder % this.cylinder_capacity;
                    if (which_chamber < 0)
                    {
                        which_chamber = which_chamber + this.cylinder_capacity;
                    }
                    GameObject round = this.cylinders[which_chamber].obj;
                    if (round && this.cylinders[which_chamber].canFire)
                    {
                        this.PlaySoundFromGroup(this.sound_gunshot_smallroom, 1f);
                        this.round_in_chamber_state = RoundState.FIRED;
                        this.cylinders[which_chamber].canFire = false;
                        this.cylinders[which_chamber].seated = this.cylinders[which_chamber].seated + Random.Range(0f, 0.5f);
                        this.cylinders[which_chamber].obj = UnityEngine.Object.Instantiate(this.shell_casing, round.transform.position, round.transform.rotation);
                        GameObject.Destroy(round);
                        var renderers = this.cylinders[which_chamber].obj.GetComponentsInChildren(typeof(Renderer));
                        foreach (Renderer renderer in renderers)
                        {
                            renderer.shadowCastingMode = ShadowCastingMode.Off;
                        }
                        UnityEngine.Object.Instantiate(this.muzzle_flash, this.transform.Find("point_muzzleflash").position, this.transform.Find("point_muzzleflash").rotation);
                        var bullet = UnityEngine.Object.Instantiate(this.bullet_obj, this.transform.Find("point_muzzle").position, this.transform.Find("point_muzzle").rotation);
                        ((BulletScript)bullet.GetComponent(typeof(BulletScript))).SetVelocity(this.transform.forward * 251f);
                        this.rotation_transfer_y = this.rotation_transfer_y + Random.Range(1f, 2f);
                        this.rotation_transfer_x = this.rotation_transfer_x + Random.Range(-1f, 1f);
                        this.recoil_transfer_x = this.recoil_transfer_x - Random.Range(150f, 300f);
                        this.recoil_transfer_y = this.recoil_transfer_y + Random.Range(-200f, 200f);
                        this.add_head_recoil = true;
                        return;
                    }
                    else
                    {
                        this.PlaySoundFromGroup(this.sound_mag_eject_button, 0.5f);
                    }
                }
            }
        }
        if (((this.action_type == ActionType.DOUBLE) && (this.trigger_pressed < 1f)) && (this.thumb_on_hammer == Thumb.OFF_HAMMER))
        {
            this.CockHammer();
            this.CockHammer();
        }
        return;
    }

    public void ReleasePressureFromTrigger()
    {
        this.pressure_on_trigger = PressureState.NONE;
        this.trigger_pressed = 0f;
    }

    public void MagEject()
    {
        if (this.gun_type != GunType.AUTOMATIC)
        {
            return;
        }
        this.PlaySoundFromGroup(this.sound_mag_eject_button, this.kGunMechanicVolume);
        if (this.mag_stage != MagStage.OUT)
        {
            this.mag_stage = MagStage.REMOVING;
            this.PlaySoundFromGroup(this.sound_mag_ejection, this.kGunMechanicVolume);
            return;
        }
        return;
    }

    public void TryToReleaseSlideLock()
    {
        if (this.gun_type != GunType.AUTOMATIC)
        {
            return;
        }
        if (this.slide_amount == this.kSlideLockPosition)
        {
            this.ReleaseSlideLock();
        }
    }

    public void PressureOnSlideLock()
    {
        if (this.gun_type != GunType.AUTOMATIC)
        {
            return;
        }
        if ((this.slide_amount > this.kPressCheckPosition) && (this.slide_stage == SlideStage.PULLBACK))
        {
            this.slide_lock = true;
        }
        else
        {
            if (this.slide_amount > this.kSlideLockPosition)// && slide_stage == SlideStage.NOTHING){
            {
                this.slide_lock = true;
            }
        }
    }

    public void ReleasePressureOnSlideLock()
    {
        if (this.slide_amount == this.kPressCheckPosition)
        {
            this.slide_lock = false;
            if (this.slide_pressure)
            {
                this.slide_stage = SlideStage.PULLBACK;
            }
        }
        else
        {
            if (this.slide_amount == 1f)
            {
                this.slide_lock = false;
            }
        }
    }

    public void ToggleSafety()
    {
        if (!this.has_safety)
        {
            return;
        }
        if (this.safety == Safety.OFF)
        {
            if ((this.slide_amount == 0f) && (this.hammer_cocked == 1f))
            {
                this.safety = Safety.ON;
                this.PlaySoundFromGroup(this.sound_safety, this.kGunMechanicVolume);
            }
        }
        else
        {
            if (this.safety == Safety.ON)
            {
                this.safety = Safety.OFF;
                this.PlaySoundFromGroup(this.sound_safety, this.kGunMechanicVolume);
            }
        }
    }

    public void ToggleAutoMod()
    {
        if (!this.has_auto_mod)
        {
            return;
        }
        this.PlaySoundFromGroup(this.sound_safety, this.kGunMechanicVolume);
        if (this.auto_mod_stage == AutoModStage.DISABLED)
        {
            this.auto_mod_stage = AutoModStage.ENABLED;
        }
        else
        {
            if (this.auto_mod_stage == AutoModStage.ENABLED)
            {
                this.auto_mod_stage = AutoModStage.DISABLED;
            }
        }
    }

    public void PullBackSlide()
    {
        if (this.gun_type != GunType.AUTOMATIC)
        {
            return;
        }
        if ((this.slide_stage != SlideStage.PULLBACK) && (this.safety == Safety.OFF))
        {
            this.slide_stage = SlideStage.PULLBACK;
            this.slide_lock = false;
        }
        this.slide_pressure = true;
    }

    public void ReleaseSlide()
    {
        if (this.gun_type != GunType.AUTOMATIC)
        {
            return;
        }
        this.slide_stage = SlideStage.NOTHING;
        this.slide_pressure = false;
    }

    public void CockHammer()
    {
        float old_hammer_cocked = this.hammer_cocked;
        this.hammer_cocked = Mathf.Min(1f, this.hammer_cocked + (Time.deltaTime * 10f));
        if ((this.hammer_cocked == 1f) && (old_hammer_cocked != 1f))
        {
            if (this.thumb_on_hammer == Thumb.ON_HAMMER)
            {
                this.PlaySoundFromGroup(this.sound_hammer_cock, this.kGunMechanicVolume);
            }
            ++this.active_cylinder;
            this.cylinder_rotation = (this.active_cylinder * 360f) / this.cylinder_capacity;
        }
        if (this.hammer_cocked < 1f)
        {
            this.cylinder_rotation = ((this.active_cylinder + this.hammer_cocked) * 360f) / this.cylinder_capacity;
            this.target_cylinder_offset = (int)0f;
        }
    }

    public void PressureOnHammer()
    {
        if (!this.has_hammer)
        {
            return;
        }
        this.thumb_on_hammer = Thumb.ON_HAMMER;
        if ((this.gun_type == GunType.REVOLVER) && (this.yolk_stage != YolkStage.CLOSED))
        {
            return;
        }
        this.CockHammer();
    }

    public void ReleaseHammer()
    {
        if (!this.has_hammer)
        {
            return;
        }
        if (((this.pressure_on_trigger != PressureState.NONE) && (this.safety_off == 1f)) || (this.hammer_cocked != 1f))
        {
            this.thumb_on_hammer = Thumb.SLOW_LOWERING;
            this.trigger_pressed = 1f;
        }
        else
        {
            this.thumb_on_hammer = Thumb.OFF_HAMMER;
        }
    }

    public bool IsSafetyOn()
    {
        return this.safety == Safety.ON;
    }

    public bool IsSlideLocked()
    {
        if (this.gun_type != GunType.AUTOMATIC)
        {
            return false;
        }
        return this.slide_lock;
    }

    public bool IsSlidePulledBack()
    {
        if (this.gun_type != GunType.AUTOMATIC)
        {
            return false;
        }
        return this.slide_stage != SlideStage.NOTHING;
    }

    public GameObject RemoveMag()
    {
        if (this.gun_type != GunType.AUTOMATIC)
        {
            return null;
        }
        GameObject mag = this.magazine_instance_in_gun;
        this.magazine_instance_in_gun = null;
        mag.transform.parent = null;
        this.ready_to_remove_mag = false;
        return mag;
    }

    public bool IsThereAMagInGun()
    {
        if (this.gun_type != GunType.AUTOMATIC)
        {
            return false;
        }
        return this.magazine_instance_in_gun;
    }

    public bool IsMagCurrentlyEjecting()
    {
        if (this.gun_type != GunType.AUTOMATIC)
        {
            return false;
        }
        return this.mag_stage == MagStage.REMOVING;
    }

    public void InsertMag(GameObject mag)
    {
        if (this.gun_type != GunType.AUTOMATIC)
        {
            return;
        }
        if (this.magazine_instance_in_gun)
        {
            return;
        }
        this.magazine_instance_in_gun = mag;
        mag.transform.parent = this.transform;
        this.mag_stage = MagStage.INSERTING;
        this.PlaySoundFromGroup(this.sound_mag_insertion, this.kGunMechanicVolume);
        this.mag_seated = 0f;
    }

    public bool IsCylinderOpen()
    {
        return (this.yolk_stage == YolkStage.OPEN) || (this.yolk_stage == YolkStage.OPENING);
    }

    public bool AddRoundToCylinder()
    {
        if ((this.gun_type != GunType.REVOLVER) || (this.yolk_stage != YolkStage.OPEN))
        {
            return false;
        }
        int best_chamber = -1;
        int next_shot = this.active_cylinder;
        if (!this.IsHammerCocked())
        {
            next_shot = (next_shot + 1) % this.cylinder_capacity;
        }
        int i = 0;
        while (i < this.cylinder_capacity)
        {
            int check = (next_shot + i) % this.cylinder_capacity;
            if (check < 0)
            {
                check = check + this.cylinder_capacity;
            }
            if (!this.cylinders[check].obj)
            {
                best_chamber = check;
                break;
            }
            ++i;
        }
        if (best_chamber == -1)
        {
            return false;
        }
        Transform yolk_pivot = this.transform.Find("yolk_pivot");
        if (yolk_pivot)
        {
            Transform yolk = yolk_pivot.Find("yolk");
            if (yolk)
            {
                Transform cylinder_assembly = yolk.Find("cylinder_assembly");
                if (cylinder_assembly)
                {
                    Transform extractor_rod = cylinder_assembly.Find("extractor_rod");
                    if (extractor_rod)
                    {
                        string name = "point_chamber_" + (best_chamber + 1);
                        this.cylinders[best_chamber].obj = UnityEngine.Object.Instantiate(this.casing_with_bullet, extractor_rod.Find(name).position, extractor_rod.Find(name).rotation);
                        this.cylinders[best_chamber].obj.transform.localScale = new Vector3(1f, 1f, 1f);
                        this.cylinders[best_chamber].canFire = true;
                        this.cylinders[best_chamber].seated = Random.Range(0f, 1f);
                        Component[] renderers = this.cylinders[best_chamber].obj.GetComponentsInChildren(typeof(Renderer));
                        foreach (Renderer renderer in renderers)
                        {
                            renderer.shadowCastingMode = ShadowCastingMode.Off;
                        }
                        this.PlaySoundFromGroup(this.sound_bullet_eject, this.kGunMechanicVolume);
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public bool ShouldOpenCylinder()
    {
        int num_firable_bullets = 0;
        int i = 0;
        while (i < this.cylinder_capacity)
        {
            if (this.cylinders[i].canFire)
            {
                ++num_firable_bullets;
            }
            ++i;
        }
        return num_firable_bullets != this.cylinder_capacity;
    }

    public bool ShouldCloseCylinder()
    {
        int num_firable_bullets = 0;
        int i = 0;
        while (i < this.cylinder_capacity)
        {
            if (this.cylinders[i].canFire)
            {
                ++num_firable_bullets;
            }
            ++i;
        }
        return num_firable_bullets == this.cylinder_capacity;
    }

    public bool ShouldExtractCasings()
    {
        int num_fired_bullets = 0;
        int i = 0;
        while (i < this.cylinder_capacity)
        {
            if (this.cylinders[i].obj && !this.cylinders[i].canFire)
            {
                ++num_fired_bullets;
            }
            ++i;
        }
        return num_fired_bullets > 0;
    }

    public bool ShouldInsertBullet()
    {
        int num_empty_chambers = 0;
        int i = 0;
        while (i < this.cylinder_capacity)
        {
            if (!this.cylinders[i].obj)
            {
                ++num_empty_chambers;
            }
            ++i;
        }
        return num_empty_chambers > 0;
    }

    public bool HasSlide()
    {
        return this.has_slide;
    }

    public bool HasSafety()
    {
        return this.has_safety;
    }

    public bool HasHammer()
    {
        return this.has_hammer;
    }

    public bool HasAutoMod()
    {
        return this.has_auto_mod;
    }

    public bool ShouldToggleAutoMod()
    {
        return this.auto_mod_stage == AutoModStage.ENABLED;
    }

    public bool IsHammerCocked()
    {
        return this.hammer_cocked == 1f;
    }

    public bool ShouldPullBackHammer()
    {
        return ((this.hammer_cocked != 1f) && this.has_hammer) && (this.action_type == ActionType.SINGLE);
    }

    public bool SwingOutCylinder()
    {
        if ((this.gun_type == GunType.REVOLVER) && ((this.yolk_stage == YolkStage.CLOSED) || (this.yolk_stage == YolkStage.CLOSING)))
        {
            this.yolk_stage = YolkStage.OPENING;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CloseCylinder()
    {
        if ((this.gun_type == GunType.REVOLVER) && (((this.extractor_rod_stage == ExtractorRodStage.CLOSED) && (this.yolk_stage == YolkStage.OPEN)) || (this.yolk_stage == YolkStage.OPENING)))
        {
            this.yolk_stage = YolkStage.CLOSING;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool ExtractorRod()
    {
        if ((this.gun_type == GunType.REVOLVER) && (((this.yolk_stage == YolkStage.OPEN) && (this.extractor_rod_stage == ExtractorRodStage.CLOSED)) || (this.extractor_rod_stage == ExtractorRodStage.CLOSING)))
        {
            this.extractor_rod_stage = ExtractorRodStage.OPENING;
            if (this.extractor_rod_amount < 1f)
            {
                this.extracted = false;
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    public void RotateCylinder(int how_many)
    {
        /*while(how_many != 0){
		if(how_many > 0){
			active_cylinder = (active_cylinder + 1)%cylinder_capacity;
			--how_many;
		}
		if(how_many < 0){
			active_cylinder = (cylinder_capacity + active_cylinder - 1)%cylinder_capacity;
			++how_many;
		}
	}*/
        this.target_cylinder_offset = this.target_cylinder_offset + (how_many * Mathf.Max(1, Mathf.Abs(this.target_cylinder_offset)));
        this.target_cylinder_offset = Mathf.Max(-12, Mathf.Min(12, this.target_cylinder_offset));
    }

    public bool IsPressCheck()
    {
        if (this.gun_type != GunType.AUTOMATIC)
        {
            return false;
        }
        return (this.slide_amount <= this.kPressCheckPosition) && (((this.slide_stage == SlideStage.PULLBACK) && this.slide_lock) || (this.slide_stage == SlideStage.HOLD));
    }

    public void Update()
    {
        if (this.gun_type == GunType.AUTOMATIC)
        {
            if (this.magazine_instance_in_gun)
            {
                Vector3 mag_pos = this.transform.Find("point_mag_inserted").position;
                Quaternion mag_rot = this.transform.rotation;
                float mag_seated_display = this.mag_seated;
                if (this.disable_springs)
                {
                    mag_seated_display = Mathf.Floor(mag_seated_display + 0.5f);
                }
                mag_pos = mag_pos + ((this.transform.Find("point_mag_to_insert").position - this.transform.Find("point_mag_inserted").position) * (1f - mag_seated_display));
                this.magazine_instance_in_gun.transform.position = mag_pos;
                this.magazine_instance_in_gun.transform.rotation = mag_rot;
            }
            if (this.mag_stage == MagStage.INSERTING)
            {
                this.mag_seated = this.mag_seated + (Time.deltaTime * 5f);
                if (this.mag_seated >= 1f)
                {
                    this.mag_seated = 1f;
                    this.mag_stage = MagStage.IN;
                    if (this.slide_amount > 0.7f)
                    {
                        this.ChamberRoundFromMag();
                    }
                    this.recoil_transfer_y = this.recoil_transfer_y + Random.Range(-40f, 40f);
                    this.recoil_transfer_x = this.recoil_transfer_x + Random.Range(50f, 300f);
                    this.rotation_transfer_x = this.rotation_transfer_x + Random.Range(-0.4f, 0.4f);
                    this.rotation_transfer_y = this.rotation_transfer_y + Random.Range(0f, 1f);
                }
            }
            if (this.mag_stage == MagStage.REMOVING)
            {
                this.mag_seated = this.mag_seated - (Time.deltaTime * 5f);
                if (this.mag_seated <= 0f)
                {
                    this.mag_seated = 0f;
                    this.ready_to_remove_mag = true;
                    this.mag_stage = MagStage.OUT;
                }
            }
        }
        if (this.has_safety)
        {
            if (this.safety == Safety.OFF)
            {
                this.safety_off = Mathf.Min(1f, this.safety_off + (Time.deltaTime * 10f));
            }
            else
            {
                if (this.safety == Safety.ON)
                {
                    this.safety_off = Mathf.Max(0f, this.safety_off - (Time.deltaTime * 10f));
                }
            }
        }
        if (this.has_auto_mod)
        {
            if (this.auto_mod_stage == AutoModStage.ENABLED)
            {
                this.auto_mod_amount = Mathf.Min(1f, this.auto_mod_amount + (Time.deltaTime * 10f));
            }
            else
            {
                if (this.auto_mod_stage == AutoModStage.DISABLED)
                {
                    this.auto_mod_amount = Mathf.Max(0f, this.auto_mod_amount - (Time.deltaTime * 10f));
                }
            }
        }
        if (this.thumb_on_hammer == Thumb.SLOW_LOWERING)
        {
            this.hammer_cocked = this.hammer_cocked - (Time.deltaTime * 10f);
            if (this.hammer_cocked <= 0f)
            {
                this.hammer_cocked = 0f;
                this.thumb_on_hammer = Thumb.OFF_HAMMER;
                this.PlaySoundFromGroup(this.sound_hammer_decock, this.kGunMechanicVolume);
            }
        }
        //PlaySoundFromGroup(sound_mag_eject_button, kGunMechanicVolume);
        if (this.has_slide)
        {
            if ((this.slide_stage == SlideStage.PULLBACK) || (this.slide_stage == SlideStage.HOLD))
            {
                if (this.slide_stage == SlideStage.PULLBACK)
                {
                    this.slide_amount = this.slide_amount + (Time.deltaTime * 10f);
                    if ((this.slide_amount >= this.kSlideLockPosition) && this.slide_lock)
                    {
                        this.slide_amount = this.kSlideLockPosition;
                        this.slide_stage = SlideStage.HOLD;
                        this.PlaySoundFromGroup(this.sound_slide_back, this.kGunMechanicVolume);
                    }
                    if ((this.slide_amount >= this.kPressCheckPosition) && this.slide_lock)
                    {
                        this.slide_amount = this.kPressCheckPosition;
                        this.slide_stage = SlideStage.HOLD;
                        this.slide_lock = false;
                        this.PlaySoundFromGroup(this.sound_slide_back, this.kGunMechanicVolume);
                    }
                    if (this.slide_amount >= 1f)
                    {
                        this.PullSlideBack();
                        this.slide_amount = 1f;
                        this.slide_stage = SlideStage.HOLD;
                        this.PlaySoundFromGroup(this.sound_slide_back, this.kGunMechanicVolume);
                    }
                }
            }
            float slide_amount_display = this.slide_amount;
            if (this.disable_springs)
            {
                slide_amount_display = Mathf.Floor(slide_amount_display + 0.5f);
                if (this.slide_amount == this.kPressCheckPosition)
                {
                    slide_amount_display = this.kPressCheckPosition;
                }
            }
            this.transform.Find("slide").localPosition = this.slide_rel_pos + ((this.transform.Find("point_slide_end").localPosition - this.transform.Find("point_slide_start").localPosition) * slide_amount_display);
        }
        if (this.has_hammer)
        {
            Transform hammer = this.GetHammer();
            Transform point_hammer_cocked = this.transform.Find("point_hammer_cocked");
            float hammer_cocked_display = this.hammer_cocked;
            if (this.disable_springs)
            {
                hammer_cocked_display = Mathf.Floor(hammer_cocked_display + 0.5f);
            }
            hammer.localPosition = Vector3.Lerp(this.hammer_rel_pos, point_hammer_cocked.localPosition, hammer_cocked_display);
            hammer.localRotation = Quaternion.Slerp(this.hammer_rel_rot, point_hammer_cocked.localRotation, hammer_cocked_display);
        }
        if (this.has_safety)
        {
            float safety_off_display = this.safety_off;
            if (this.disable_springs)
            {
                safety_off_display = Mathf.Floor(safety_off_display + 0.5f);
            }
            this.transform.Find("safety").localPosition = Vector3.Lerp(this.safety_rel_pos, this.transform.Find("point_safety_off").localPosition, safety_off_display);
            this.transform.Find("safety").localRotation = Quaternion.Slerp(this.safety_rel_rot, this.transform.Find("point_safety_off").localRotation, safety_off_display);
        }
        if (this.has_auto_mod)
        {
            float auto_mod_amount_display = this.auto_mod_amount;
            if (this.disable_springs)
            {
                auto_mod_amount_display = Mathf.Floor(auto_mod_amount_display + 0.5f);
            }
            Transform slide = this.transform.Find("slide");
            slide.Find("auto mod toggle").localPosition = Vector3.Lerp(this.auto_mod_rel_pos, slide.Find("point_auto_mod_enabled").localPosition, auto_mod_amount_display);
        }
        if (this.gun_type == GunType.AUTOMATIC)
        {
            this.hammer_cocked = Mathf.Max(this.hammer_cocked, this.slide_amount);
            if (((this.hammer_cocked != 1f) && (this.thumb_on_hammer == Thumb.OFF_HAMMER)) && ((this.pressure_on_trigger == PressureState.NONE) || (this.action_type == ActionType.SINGLE)))
            {
                this.hammer_cocked = Mathf.Min(this.hammer_cocked, this.slide_amount);
            }
        }
        else
        {
            if (((this.hammer_cocked != 1f) && (this.thumb_on_hammer == Thumb.OFF_HAMMER)) && ((this.pressure_on_trigger == PressureState.NONE) || (this.action_type == ActionType.SINGLE)))
            {
                this.hammer_cocked = 0f;
            }
        }
        if (this.has_slide)
        {
            if (this.slide_stage == SlideStage.NOTHING)
            {
                float old_slide_amount = this.slide_amount;
                this.slide_amount = Mathf.Max(0f, this.slide_amount - (Time.deltaTime * this.kSlideLockSpeed));
                if ((!this.slide_lock && (this.slide_amount == 0f)) && (old_slide_amount != 0f))
                {
                    this.PlaySoundFromGroup(this.sound_slide_front, this.kGunMechanicVolume * 1.5f);
                    if (this.round_in_chamber)
                    {
                        this.round_in_chamber.transform.position = this.transform.Find("point_chambered_round").position;
                        this.round_in_chamber.transform.rotation = this.transform.Find("point_chambered_round").rotation;
                    }
                }
                if ((this.slide_amount == 0f) && (this.round_in_chamber_state == RoundState.LOADING))
                {
                    this.round_in_chamber_state = RoundState.READY;
                }
                if (this.slide_lock && (old_slide_amount >= this.kSlideLockPosition))
                {
                    this.slide_amount = Mathf.Max(this.kSlideLockPosition, this.slide_amount);
                    if ((old_slide_amount != this.kSlideLockPosition) && (this.slide_amount == this.kSlideLockPosition))
                    {
                        this.PlaySoundFromGroup(this.sound_slide_front, this.kGunMechanicVolume);
                    }
                }
            }
        }
        if (this.gun_type == GunType.REVOLVER)
        {
            if ((this.yolk_stage == YolkStage.CLOSED) && (this.hammer_cocked == 1f))
            {
                this.target_cylinder_offset = 0;
            }
            if (this.target_cylinder_offset != 0f)
            {
                float target_cylinder_rotation = ((this.active_cylinder + this.target_cylinder_offset) * 360f) / this.cylinder_capacity;
                this.cylinder_rotation = Mathf.Lerp(target_cylinder_rotation, this.cylinder_rotation, Mathf.Pow(0.2f, Time.deltaTime));
                if (this.cylinder_rotation > (((this.active_cylinder + 0.5f) * 360f) / this.cylinder_capacity))
                {
                    ++this.active_cylinder;
                    --this.target_cylinder_offset;
                    if (this.yolk_stage == YolkStage.CLOSED)
                    {
                        this.PlaySoundFromGroup(this.sound_cylinder_rotate, this.kGunMechanicVolume);
                    }
                }
                if (this.cylinder_rotation < (((this.active_cylinder - 0.5f) * 360f) / this.cylinder_capacity))
                {
                    --this.active_cylinder;
                    ++this.target_cylinder_offset;
                    if (this.yolk_stage == YolkStage.CLOSED)
                    {
                        this.PlaySoundFromGroup(this.sound_cylinder_rotate, this.kGunMechanicVolume);
                    }
                }
            }
            if (this.yolk_stage == YolkStage.CLOSING)
            {
                this.yolk_open = this.yolk_open - (Time.deltaTime * 5f);
                if (this.yolk_open <= 0f)
                {
                    this.yolk_open = 0f;
                    this.yolk_stage = YolkStage.CLOSED;
                    this.PlaySoundFromGroup(this.sound_cylinder_close, this.kGunMechanicVolume * 2f);
                    this.target_cylinder_offset = 0;
                }
            }
            if (this.yolk_stage == YolkStage.OPENING)
            {
                this.yolk_open = this.yolk_open + (Time.deltaTime * 5f);
                if (this.yolk_open >= 1f)
                {
                    this.yolk_open = 1f;
                    this.yolk_stage = YolkStage.OPEN;
                    this.PlaySoundFromGroup(this.sound_cylinder_open, this.kGunMechanicVolume * 2f);
                }
            }
            if (this.extractor_rod_stage == ExtractorRodStage.CLOSING)
            {
                this.extractor_rod_amount = this.extractor_rod_amount - (Time.deltaTime * 10f);
                if (this.extractor_rod_amount <= 0f)
                {
                    this.extractor_rod_amount = 0f;
                    this.extractor_rod_stage = ExtractorRodStage.CLOSED;
                    this.PlaySoundFromGroup(this.sound_extractor_rod_close, this.kGunMechanicVolume);
                }
                int j = 0;
                while (j < this.cylinder_capacity)
                {
                    if (this.cylinders[j].obj)
                    {
                        this.cylinders[j].falling = false;
                    }
                    ++j;
                }
            }
            if (this.extractor_rod_stage == ExtractorRodStage.OPENING)
            {
                float old_extractor_rod_amount = this.extractor_rod_amount;
                this.extractor_rod_amount = this.extractor_rod_amount + (Time.deltaTime * 10f);
                if (this.extractor_rod_amount >= 1f)
                {
                    if (!this.extracted)
                    {
                        int k = 0;
                        while (k < this.cylinder_capacity)
                        {
                            if (this.cylinders[k].obj)
                            {
                                if (Random.Range(0f, 3f) > this.cylinders[k].seated)
                                {
                                    this.cylinders[k].falling = true;
                                    this.cylinders[k].seated = this.cylinders[k].seated - Random.Range(0f, 0.5f);
                                }
                                else
                                {
                                    this.cylinders[k].falling = false;
                                }
                            }
                            ++k;
                        }
                        this.extracted = true;
                    }
                    int l = 0;
                    while (l < this.cylinder_capacity)
                    {
                        if (this.cylinders[l].obj && this.cylinders[l].falling)
                        {
                            this.cylinders[l].seated = this.cylinders[l].seated - (Time.deltaTime * 5f);
                            if (this.cylinders[l].seated <= 0f)
                            {
                                GameObject bullet = this.cylinders[l].obj;
                                bullet.AddComponent(typeof(Rigidbody));
                                bullet.transform.parent = null;
                                bullet.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
                                bullet.GetComponent<Rigidbody>().velocity = this.velocity;
                                bullet.GetComponent<Rigidbody>().angularVelocity = new Vector3(Random.Range(-40f, 40f), Random.Range(-40f, 40f), Random.Range(-40f, 40f));
                                this.cylinders[l].obj = null;
                                this.cylinders[l].canFire = false;
                            }
                        }
                        ++l;
                    }
                    this.extractor_rod_amount = 1f;
                    this.extractor_rod_stage = ExtractorRodStage.OPEN;
                    if (old_extractor_rod_amount < 1f)
                    {
                        this.PlaySoundFromGroup(this.sound_extractor_rod_open, this.kGunMechanicVolume);
                    }
                }
            }
            if ((this.extractor_rod_stage == ExtractorRodStage.OPENING) || (this.extractor_rod_stage == ExtractorRodStage.OPEN))
            {
                this.extractor_rod_stage = ExtractorRodStage.CLOSING;
            }
            float yolk_open_display = this.yolk_open;
            float extractor_rod_amount_display = this.extractor_rod_amount;
            if (this.disable_springs)
            {
                yolk_open_display = Mathf.Floor(yolk_open_display + 0.5f);
                extractor_rod_amount_display = Mathf.Floor(extractor_rod_amount_display + 0.5f);
            }
            Transform yolk_pivot = this.transform.Find("yolk_pivot");
            yolk_pivot.localRotation = Quaternion.Slerp(this.yolk_pivot_rel_rot, this.transform.Find("point_yolk_pivot_open").localRotation, yolk_open_display);
            Transform cylinder_assembly = yolk_pivot.Find("yolk").Find("cylinder_assembly");

            {
                float _114 = this.cylinder_rotation;
                Quaternion _115 = cylinder_assembly.localRotation;
                Vector3 _116 = _115.eulerAngles;
                _116.z = _114;
                _115.eulerAngles = _116;
                cylinder_assembly.localRotation = _115;
            }
            Transform extractor_rod = cylinder_assembly.Find("extractor_rod");
            extractor_rod.localPosition = Vector3.Lerp(this.extractor_rod_rel_pos, cylinder_assembly.Find("point_extractor_rod_extended").localPosition, extractor_rod_amount_display);
            int i = 0;
            while (i < this.cylinder_capacity)
            {
                if (this.cylinders[i].obj)
                {
                    string name = "point_chamber_" + (i + 1);
                    Transform bullet_chamber = extractor_rod.Find(name);
                    this.cylinders[i].obj.transform.position = bullet_chamber.position;
                    this.cylinders[i].obj.transform.rotation = bullet_chamber.rotation;
                    this.cylinders[i].obj.transform.localScale = this.transform.localScale;
                }
                ++i;
            }
        }
    }

    public void FixedUpdate()
    {
        this.velocity = (this.transform.position - this.old_pos) / Time.deltaTime;
        this.old_pos = this.transform.position;
    }

    public Gun()
    {
        this.kGunMechanicVolume = 0.2f;
        this.pressure_on_trigger = PressureState.NONE;
        this.round_in_chamber_state = RoundState.READY;
        this.slide_stage = SlideStage.NOTHING;
        this.thumb_on_hammer = Thumb.OFF_HAMMER;
        this.hammer_cocked = 1f;
        this.safety_off = 1f;
        this.safety = Safety.OFF;
        this.kSlideLockPosition = 0.9f;
        this.kPressCheckPosition = 0.4f;
        this.kSlideLockSpeed = 20f;
        this.mag_stage = MagStage.IN;
        this.mag_seated = 1f;
        this.auto_mod_stage = AutoModStage.DISABLED;
        this.yolk_stage = YolkStage.CLOSED;
        this.extractor_rod_stage = ExtractorRodStage.CLOSED;
        this.cylinder_capacity = 6;
    }

}