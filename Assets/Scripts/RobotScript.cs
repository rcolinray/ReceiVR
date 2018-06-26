using UnityEngine;
using System.Collections;

public enum RobotType
{
    SHOCK_DRONE = 0,
    STATIONARY_TURRET = 1,
    MOBILE_TURRET = 2,
    GUN_DRONE = 3
}

public enum AIState
{
    IDLE = 0,
    ALERT = 1,
    ALERT_COOLDOWN = 2,
    AIMING = 3,
    FIRING = 4,
    DEACTIVATING = 5,
    DEAD = 6
}

public enum CameraPivotState
{
    DOWN = 0,
    WAIT_UP = 1,
    UP = 2,
    WAIT_DOWN = 3
}

[System.Serializable]
public partial class RobotScript : MonoBehaviour
{
    public AudioClip[] sound_gunshot;
    public AudioClip[] sound_damage_camera;
    public AudioClip[] sound_damage_gun;
    public AudioClip[] sound_damage_battery;
    public AudioClip[] sound_damage_ammo;
    public AudioClip[] sound_damage_motor;
    public AudioClip[] sound_bump;
    public AudioClip sound_alert;
    public AudioClip sound_unalert;
    public AudioClip sound_engine_loop;
    public AudioClip sound_damaged_engine_loop;
    private AudioSource audiosource_taser;
    private AudioSource audiosource_motor;
    private GameObject object_audiosource_motor;
    private AudioSource audiosource_effect;
    private AudioSource audiosource_foley;
    private float sound_line_of_sight;
    public GameObject electric_spark_obj;
    public GameObject muzzle_flash;
    public GameObject bullet_obj;
    public RobotType robot_type;
    private float gun_delay;
    private bool alive;
    private Spring rotation_x;
    private Spring rotation_y;
    private Quaternion initial_turret_orientation;
    private Vector3 initial_turret_position;
    private Transform gun_pivot;
    private AIState ai_state;
    private bool battery_alive;
    private bool motor_alive;
    private bool camera_alive;
    private bool trigger_alive;
    private bool barrel_alive;
    private bool ammo_alive;
    private bool trigger_down;
    private int bullets;
    private float kAlertDelay;
    private float kAlertCooldownDelay;
    private float alert_delay;
    private float alert_cooldown_delay;
    private float kMaxRange;
    private float rotor_speed;
    private float top_rotor_rotation;
    private float bottom_rotor_rotation;
    private Vector3 initial_pos;
    private bool stuck;
    private float stuck_delay;
    private Vector3 tilt_correction;
    private bool distance_sleep;
    private float kSleepDistance;
    public Vector3 target_pos;
    public CameraPivotState camera_pivot_state;
    public float camera_pivot_delay;
    public float camera_pivot_angle;
    public virtual void PlaySoundFromGroup(object[] group, float volume)
    {
        if (group.Length == 0)
        {
            return;
        }
        int which_shot = Random.Range(0, group.Length);
        this.audiosource_effect.PlayOneShot((AudioClip)group[which_shot], volume * PlayerPrefs.GetFloat("sound_volume", 1f));
    }

    public virtual GameObject GetTurretLightObject()
    {
        return this.transform.Find("gun pivot").Find("camera").Find("light").gameObject;
    }

    public virtual GameObject GetDroneLightObject()
    {
        return this.transform.Find("camera_pivot").Find("camera").Find("light").gameObject;
    }

    public virtual GameObject GetDroneLensFlareObject()
    {
        return this.transform.Find("camera_pivot").Find("camera").Find("lens flare").gameObject;
    }

    public virtual Quaternion RandomOrientation()
    {
        return Quaternion.EulerAngles(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
    }

    public virtual void Damage(GameObject obj)
    {
        bool damage_done = false;
        if ((obj.name == "battery") && this.battery_alive)
        {
            this.battery_alive = false;
            this.motor_alive = false;
            this.camera_alive = false;
            this.trigger_alive = false;
            if (this.robot_type == RobotType.SHOCK_DRONE)
            {
                this.barrel_alive = false;
            }
            this.PlaySoundFromGroup(this.sound_damage_battery, 1f);
            this.rotation_x.target_state = 40f;
            damage_done = true;
        }
        else
        {
            if (((obj.name == "pivot motor") || (obj.name == "motor")) && this.motor_alive)
            {
                this.motor_alive = false;
                this.PlaySoundFromGroup(this.sound_damage_motor, 1f);
                damage_done = true;
            }
            else
            {
                if ((obj.name == "power cable") && (this.camera_alive || this.trigger_alive))
                {
                    this.camera_alive = false;
                    damage_done = true;
                    this.PlaySoundFromGroup(this.sound_damage_battery, 1f);
                    this.trigger_alive = false;
                }
                else
                {
                    if ((obj.name == "ammo box") && this.ammo_alive)
                    {
                        this.ammo_alive = false;
                        this.PlaySoundFromGroup(this.sound_damage_ammo, 1f);
                        damage_done = true;
                    }
                    else
                    {
                        if (((obj.name == "gun") || (obj.name == "shock prod")) && this.barrel_alive)
                        {
                            this.barrel_alive = false;
                            this.PlaySoundFromGroup(this.sound_damage_gun, 1f);
                            damage_done = true;
                        }
                        else
                        {
                            if ((obj.name == "camera") && this.camera_alive)
                            {
                                this.camera_alive = false;
                                this.PlaySoundFromGroup(this.sound_damage_camera, 1f);
                                damage_done = true;
                            }
                            else
                            {
                                if ((obj.name == "camera armor") && this.camera_alive)
                                {
                                    this.camera_alive = false;
                                    this.PlaySoundFromGroup(this.sound_damage_camera, 1f);
                                    damage_done = true;
                                }
                            }
                        }
                    }
                }
            }
        }
        if (damage_done)
        {
            UnityEngine.Object.Instantiate(this.electric_spark_obj, obj.transform.position, this.RandomOrientation());
        }
    }

    public virtual void WasShotInternal(GameObject obj)
    {
        this.Damage(obj);
    }

    public virtual void WasShot(GameObject obj, Vector3 pos, Vector3 vel)
    {
        if (this.transform.parent && (this.transform.parent.gameObject.name == "gun pivot"))
        {
            Vector3 x_axis = this.transform.Find("point_pivot").rotation * new Vector3(1, 0, 0);
            Vector3 y_axis = this.transform.Find("point_pivot").rotation * new Vector3(0, 1, 0);
            Vector3 z_axis = this.transform.Find("point_pivot").rotation * new Vector3(0, 0, 1);
            Vector3 y_plane_vel = new Vector3(Vector3.Dot(vel, x_axis), 0f, Vector3.Dot(vel, z_axis));
            Vector3 rel_pos = pos - this.transform.Find("point_pivot").position;
            Vector3 y_plane_pos = new Vector3(Vector3.Dot(rel_pos, z_axis), 0f, -Vector3.Dot(rel_pos, x_axis));
            this.rotation_y.vel = this.rotation_y.vel + (Vector3.Dot(y_plane_vel, y_plane_pos) * 10f);
            Vector3 x_plane_vel = new Vector3(Vector3.Dot(vel, y_axis), 0f, Vector3.Dot(vel, z_axis));
            rel_pos = pos - this.transform.Find("point_pivot").position;
            Vector3 x_plane_pos = new Vector3(-Vector3.Dot(rel_pos, z_axis), 0f, Vector3.Dot(rel_pos, y_axis));
            this.rotation_x.vel = this.rotation_x.vel + (Vector3.Dot(x_plane_vel, x_plane_pos) * 10f);
        }
        if (this.robot_type == RobotType.SHOCK_DRONE)
        {
            if (Random.Range(0f, 1f) < 0.5f)
            {
                this.Damage(this.transform.Find("battery").gameObject);
            }
        }
        else
        {
            if (Random.Range(0f, 1f) < 0.25f)
            {
                this.Damage(this.transform.Find("battery").gameObject);
            }
        }
        this.Damage(obj);
    }

    public virtual void Start()
    {
        this.audiosource_effect = (AudioSource)this.gameObject.AddComponent(typeof(AudioSource));
        this.audiosource_effect.rolloffMode = AudioRolloffMode.Linear;
        this.audiosource_effect.maxDistance = 30;
        this.object_audiosource_motor = new GameObject("motor audiosource object");
        this.object_audiosource_motor.transform.parent = this.transform;
        this.object_audiosource_motor.transform.localPosition = new Vector3(0, 0, 0);
        this.audiosource_motor = (AudioSource)this.object_audiosource_motor.AddComponent(typeof(AudioSource));
        this.object_audiosource_motor.AddComponent(typeof(AudioLowPassFilter));
        this.audiosource_motor.loop = true;
        this.audiosource_motor.volume = 0.4f * PlayerPrefs.GetFloat("sound_volume", 1f);
        this.audiosource_motor.clip = this.sound_engine_loop;
        switch (this.robot_type)
        {
            case RobotType.STATIONARY_TURRET:
                this.gun_pivot = this.transform.Find("gun pivot");
                this.initial_turret_orientation = this.gun_pivot.transform.localRotation;
                this.initial_turret_position = this.gun_pivot.transform.localPosition;
                this.audiosource_motor.rolloffMode = AudioRolloffMode.Linear;
                this.audiosource_motor.maxDistance = 4;
                break;
            case RobotType.SHOCK_DRONE:
                this.audiosource_motor.maxDistance = 8;
                this.audiosource_foley = (AudioSource)this.gameObject.AddComponent(typeof(AudioSource));
                this.audiosource_taser = (AudioSource)this.gameObject.AddComponent(typeof(AudioSource));
                this.audiosource_taser.rolloffMode = AudioRolloffMode.Linear;
                this.audiosource_taser.loop = true;
                this.audiosource_taser.clip = this.sound_gunshot[0];
                break;
        }
        this.initial_pos = this.transform.position;
        this.target_pos = this.initial_pos;
    }

    public virtual void UpdateStationaryTurret()
    {
        RaycastHit hit = default(RaycastHit);
        if (Vector3.Distance(GameObject.Find("Player").transform.position, this.transform.position) > this.kSleepDistance)
        {
            this.GetTurretLightObject().GetComponent<Light>().shadows = LightShadows.None;
            if (this.audiosource_motor.isPlaying)
            {
                this.audiosource_motor.Stop();
            }
            return;
        }
        else
        {
            if (!this.audiosource_motor.isPlaying)
            {
                this.audiosource_motor.volume = PlayerPrefs.GetFloat("sound_volume", 1f);
                this.audiosource_motor.Play();
            }
            this.audiosource_motor.volume = 0.4f * PlayerPrefs.GetFloat("sound_volume", 1f);
            if (this.GetTurretLightObject().GetComponent<Light>().intensity > 0f)
            {
                this.GetTurretLightObject().GetComponent<Light>().shadows = LightShadows.Hard;
            }
            else
            {
                this.GetTurretLightObject().GetComponent<Light>().shadows = LightShadows.None;
            }
        }
        if (this.motor_alive)
        {
            switch (this.ai_state)
            {
                case AIState.IDLE:
                    this.rotation_y.target_state = this.rotation_y.target_state + (Time.deltaTime * 100f);
                    break;
                case AIState.AIMING:
                case AIState.ALERT:
                case AIState.ALERT_COOLDOWN:
                case AIState.FIRING:
                    Vector3 rel_pos = this.target_pos - this.transform.Find("point_pivot").position;
                    Vector3 x_axis = this.transform.Find("point_pivot").rotation * new Vector3(1, 0, 0);
                    Vector3 y_axis = this.transform.Find("point_pivot").rotation * new Vector3(0, 1, 0);
                    Vector3 z_axis = this.transform.Find("point_pivot").rotation * new Vector3(0, 0, 1);
                    Vector3 y_plane_pos = new Vector3(Vector3.Dot(rel_pos, z_axis), 0f, -Vector3.Dot(rel_pos, x_axis)).normalized;
                    float target_y = ((Mathf.Atan2(y_plane_pos.x, y_plane_pos.z) / Mathf.PI) * 180) - 90;
                    while (target_y > (this.rotation_y.state + 180))
                    {
                        this.rotation_y.state = this.rotation_y.state + 360f;
                    }
                    while (target_y < (this.rotation_y.state - 180))
                    {
                        this.rotation_y.state = this.rotation_y.state - 360f;
                    }
                    this.rotation_y.target_state = target_y;
                    float y_height = Vector3.Dot(y_axis, rel_pos.normalized);
                    float target_x = (-Mathf.Asin(y_height) / Mathf.PI) * 180;
                    this.rotation_x.target_state = target_x;
                    this.rotation_x.target_state = Mathf.Min(40, Mathf.Max(-40, target_x));
                    break;
            }
        }
        if (this.battery_alive)
        {
            switch (this.ai_state)
            {
                case AIState.FIRING:
                    this.trigger_down = true;
                    break;
                default:
                    this.trigger_down = false;
                    break;
            }
        }
        if (this.barrel_alive)
        {
            if (this.trigger_down)
            {
                if (this.gun_delay <= 0f)
                {
                    this.gun_delay = this.gun_delay + 0.1f;
                    Transform point_muzzle_flash = this.gun_pivot.Find("gun").Find("point_muzzleflash");
                    UnityEngine.Object.Instantiate(this.muzzle_flash, point_muzzle_flash.position, point_muzzle_flash.rotation);
                    this.PlaySoundFromGroup(this.sound_gunshot, 1f);
                    GameObject bullet = UnityEngine.Object.Instantiate(this.bullet_obj, point_muzzle_flash.position, point_muzzle_flash.rotation);
                    ((BulletScript)bullet.GetComponent(typeof(BulletScript))).SetVelocity(point_muzzle_flash.forward * 300f);
                    ((BulletScript)bullet.GetComponent(typeof(BulletScript))).SetHostile();
                    this.rotation_x.vel = this.rotation_x.vel + Random.Range(-50, 50);
                    this.rotation_y.vel = this.rotation_y.vel + Random.Range(-50, 50);
                    --this.bullets;
                }
            }
            if (this.ammo_alive && (this.bullets > 0))
            {
                this.gun_delay = Mathf.Max(0f, this.gun_delay - Time.deltaTime);
            }
        }
        float danger = 0f;
        GameObject player = GameObject.Find("Player");
        float dist = Vector3.Distance(player.transform.position, this.transform.position);
        if (this.battery_alive)
        {
            danger = danger + Mathf.Max(0f, 1f - (dist / this.kMaxRange));
        }
        if (this.camera_alive)
        {
            if (danger > 0f)
            {
                danger = Mathf.Min(0.2f, danger);
            }
            if ((this.ai_state == AIState.AIMING) || (this.ai_state == AIState.FIRING))
            {
                danger = 1f;
            }
            if ((this.ai_state == AIState.ALERT) || (this.ai_state == AIState.ALERT_COOLDOWN))
            {
                danger = danger + 0.5f;
            }
            Transform camera = this.transform.Find("gun pivot").Find("camera");
            var rel_pos = player.transform.position - camera.position;
            bool sees_target = false;
            if ((dist < this.kMaxRange) && (Vector3.Dot(camera.rotation * new Vector3(0, -1, 0), rel_pos.normalized) > 0.7f))
            {
                if (!Physics.Linecast(camera.position, player.transform.position, out hit, 1 << 0))
                {
                    sees_target = true;
                }
            }
            if (sees_target)
            {
                switch (this.ai_state)
                {
                    case AIState.IDLE:
                        this.ai_state = AIState.ALERT;
                        this.audiosource_effect.PlayOneShot(this.sound_alert, 0.3f * PlayerPrefs.GetFloat("sound_volume", 1f));
                        this.alert_delay = this.kAlertDelay;
                        break;
                    case AIState.AIMING:
                        if (Vector3.Dot(camera.rotation * new Vector3(0, -1, 0), rel_pos.normalized) > 0.9f)
                        {
                            this.ai_state = AIState.FIRING;
                        }
                        this.target_pos = player.transform.position;
                        break;
                    case AIState.FIRING:
                        this.target_pos = player.transform.position;
                        break;
                    case AIState.ALERT:
                        this.alert_delay = this.alert_delay - Time.deltaTime;
                        if (this.alert_delay <= 0f)
                        {
                            this.ai_state = AIState.AIMING;
                        }
                        this.target_pos = player.transform.position;
                        break;
                    case AIState.ALERT_COOLDOWN:
                        this.ai_state = AIState.ALERT;
                        this.alert_delay = this.kAlertDelay;
                        break;
                }
            }
            else
            {
                switch (this.ai_state)
                {
                    case AIState.AIMING:
                    case AIState.FIRING:
                    case AIState.ALERT:
                        this.ai_state = AIState.ALERT_COOLDOWN;
                        this.alert_cooldown_delay = this.kAlertCooldownDelay;
                        break;
                    case AIState.ALERT_COOLDOWN:
                        this.alert_cooldown_delay = this.alert_cooldown_delay - Time.deltaTime;
                        if (this.alert_cooldown_delay <= 0f)
                        {
                            this.ai_state = AIState.IDLE;
                            this.audiosource_effect.PlayOneShot(this.sound_unalert, 0.3f * PlayerPrefs.GetFloat("sound_volume", 1f));
                        }
                        break;
                }
            }
            switch (this.ai_state)
            {
                case AIState.IDLE:
                    this.GetTurretLightObject().GetComponent<Light>().color = new Color(0, 0, 1);
                    break;
                case AIState.AIMING:
                    this.GetTurretLightObject().GetComponent<Light>().color = new Color(1, 0, 0);
                    break;
                case AIState.ALERT:
                case AIState.ALERT_COOLDOWN:
                    this.GetTurretLightObject().GetComponent<Light>().color = new Color(1, 1, 0);
                    break;
            }
        }
        ((MusicScript)player.GetComponent(typeof(MusicScript))).AddDangerLevel(danger);
        if (!this.camera_alive)
        {
            this.GetTurretLightObject().GetComponent<Light>().intensity = this.GetTurretLightObject().GetComponent<Light>().intensity * Mathf.Pow(0.01f, Time.deltaTime);
        }
        float target_pitch = (Mathf.Abs(this.rotation_y.vel) + Mathf.Abs(this.rotation_x.vel)) * 0.01f;
        target_pitch = Mathf.Clamp(target_pitch, 0.2f, 2f);
        this.audiosource_motor.pitch = Mathf.Lerp(this.audiosource_motor.pitch, target_pitch, Mathf.Pow(0.0001f, Time.deltaTime));
        this.rotation_x.Update();
        this.rotation_y.Update();
        this.gun_pivot.localRotation = this.initial_turret_orientation;
        this.gun_pivot.localPosition = this.initial_turret_position;
        this.gun_pivot.RotateAround(this.transform.Find("point_pivot").position, this.transform.Find("point_pivot").rotation * new Vector3(1, 0, 0), this.rotation_x.state);
        this.gun_pivot.RotateAround(this.transform.Find("point_pivot").position, this.transform.Find("point_pivot").rotation * new Vector3(0, 1, 0), this.rotation_y.state);
    }

    public virtual void UpdateDrone()
    {
        Quaternion correction = default(Quaternion);
        Vector3 correction_vec = default(Vector3);
        float correction_angle = 0.0f;
        RaycastHit hit = default(RaycastHit);
        if (Vector3.Distance(GameObject.Find("Player").transform.position, this.transform.position) > this.kSleepDistance)
        {
            this.GetDroneLightObject().GetComponent<Light>().shadows = LightShadows.None;
            if (this.motor_alive)
            {
                this.distance_sleep = true;
                this.GetComponent<Rigidbody>().Sleep();
            }
            if (this.audiosource_motor.isPlaying)
            {
                this.audiosource_motor.Stop();
            }
            return;
        }
        else
        {
            if (this.GetDroneLightObject().GetComponent<Light>().intensity > 0f)
            {
                this.GetDroneLightObject().GetComponent<Light>().shadows = LightShadows.Hard;
            }
            else
            {
                this.GetDroneLightObject().GetComponent<Light>().shadows = LightShadows.None;
            }
            if (this.motor_alive && this.distance_sleep)
            {
                this.GetComponent<Rigidbody>().WakeUp();
                this.distance_sleep = false;
            }
            if (!this.audiosource_motor.isPlaying)
            {
                this.audiosource_motor.volume = PlayerPrefs.GetFloat("sound_volume", 1f);
                this.audiosource_motor.Play();
            }
        }
        Vector3 rel_pos = this.target_pos - this.transform.position;
        if (this.motor_alive)
        {
            float kFlyDeadZone = 0.2f;
            float kFlySpeed = 10f;
            Vector3 target_vel = (this.target_pos - this.transform.position) / kFlyDeadZone;
            if (target_vel.magnitude > 1f)
            {
                target_vel = target_vel.normalized;
            }
            target_vel = target_vel * kFlySpeed;
            Vector3 target_accel = target_vel - this.GetComponent<Rigidbody>().velocity;
            if (this.ai_state == AIState.IDLE)
            {
                target_accel = target_accel * 0.1f;
            }
            target_accel.y = target_accel.y + 9.81f;
            this.rotor_speed = target_accel.magnitude;
            this.rotor_speed = Mathf.Clamp(this.rotor_speed, 0f, 14f);
            Vector3 up = this.transform.rotation * new Vector3(0, 1, 0);
            correction.SetFromToRotation(up, target_accel.normalized);
            correction.ToAngleAxis(out correction_angle, out correction_vec);
            this.tilt_correction = correction_vec * correction_angle;
            this.tilt_correction = this.tilt_correction - this.GetComponent<Rigidbody>().angularVelocity;
            Vector3 x_axis = this.transform.rotation * new Vector3(1, 0, 0);
            Vector3 y_axis = this.transform.rotation * new Vector3(0, 1, 0);
            Vector3 z_axis = this.transform.rotation * new Vector3(0, 0, 1);
            if (this.ai_state != AIState.IDLE)
            {
                Vector3 y_plane_pos = new Vector3(Vector3.Dot(rel_pos, z_axis), 0f, -Vector3.Dot(rel_pos, x_axis)).normalized;
                float target_y = ((Mathf.Atan2(y_plane_pos.x, y_plane_pos.z) / Mathf.PI) * 180) - 90;
                while (target_y > 180)
                {
                    target_y = target_y - 360f;
                }
                while (target_y < -180)
                {
                    target_y = target_y + 360f;
                }
                this.tilt_correction = this.tilt_correction + (y_axis * target_y);
                this.tilt_correction = this.tilt_correction * 5f;
            }
            else
            {
                this.tilt_correction = this.tilt_correction + y_axis;
            }
            if (this.ai_state == AIState.IDLE)
            {
                this.tilt_correction = this.tilt_correction * 0.1f;
            }
            if (this.GetComponent<Rigidbody>().velocity.magnitude < 0.2f)
            {
                this.stuck_delay = this.stuck_delay + Time.deltaTime;
                if (this.stuck_delay > 1f)
                {
                    this.target_pos = this.transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                    this.stuck_delay = 0f;
                }
            }
            else
            {
                this.stuck_delay = 0f;
            }
        }
        else
        {
            this.rotor_speed = Mathf.Max(0f, this.rotor_speed - (Time.deltaTime * 5f));
            this.GetComponent<Rigidbody>().angularDrag = 0.05f;
        }
        if (this.barrel_alive && (this.ai_state == AIState.FIRING))
        {
            if (!this.audiosource_taser.isPlaying)
            {
                this.audiosource_taser.volume = PlayerPrefs.GetFloat("sound_volume", 1f);
                this.audiosource_taser.Play();
            }
            else
            {
                this.audiosource_taser.volume = PlayerPrefs.GetFloat("sound_volume", 1f);
            }
            if (this.gun_delay <= 0f)
            {
                this.gun_delay = 0.1f;
                UnityEngine.Object.Instantiate(this.muzzle_flash, this.transform.Find("point_spark").position, this.RandomOrientation());
                if (Vector3.Distance(this.transform.Find("point_spark").position, GameObject.Find("Player").transform.position) < 1)
                {
                    ((AimScript)GameObject.Find("Player").GetComponent(typeof(AimScript))).Shock();
                }
            }
        }
        else
        {
            this.audiosource_taser.Stop();
        }
        this.gun_delay = Mathf.Max(0f, this.gun_delay - Time.deltaTime);
        this.top_rotor_rotation = this.top_rotor_rotation + ((this.rotor_speed * Time.deltaTime) * 1000f);
        this.bottom_rotor_rotation = this.bottom_rotor_rotation - ((this.rotor_speed * Time.deltaTime) * 1000f);
        if ((this.rotor_speed * Time.timeScale) > 7f)
        {
            this.transform.Find("bottom rotor").gameObject.GetComponent<Renderer>().enabled = false;
            this.transform.Find("top rotor").gameObject.GetComponent<Renderer>().enabled = false;
        }
        else
        {
            this.transform.Find("bottom rotor").gameObject.GetComponent<Renderer>().enabled = true;
            this.transform.Find("top rotor").gameObject.GetComponent<Renderer>().enabled = true;
        }

        {
            float _135 = this.bottom_rotor_rotation;
            Vector3 _136 = this.transform.Find("bottom rotor").localEulerAngles;
            _136.y = _135;
            this.transform.Find("bottom rotor").localEulerAngles = _136;
        }

        {
            float _137 = this.top_rotor_rotation;
            Vector3 _138 = this.transform.Find("top rotor").localEulerAngles;
            _138.y = _137;
            this.transform.Find("top rotor").localEulerAngles = _138;
        }
        //rigidbody.velocity += transform.rotation * Vector3(0,1,0) * rotor_speed * Time.deltaTime;
        if (this.camera_alive)
        {
            if (this.ai_state == AIState.IDLE)
            {
                switch (this.camera_pivot_state)
                {
                    case CameraPivotState.DOWN:
                        this.camera_pivot_angle = this.camera_pivot_angle + (Time.deltaTime * 25f);
                        if (this.camera_pivot_angle > 50)
                        {
                            this.camera_pivot_angle = 50;
                            this.camera_pivot_state = CameraPivotState.WAIT_UP;
                            this.camera_pivot_delay = 0.2f;
                        }
                        break;
                    case CameraPivotState.UP:
                        this.camera_pivot_angle = this.camera_pivot_angle - (Time.deltaTime * 25f);
                        if (this.camera_pivot_angle < 0)
                        {
                            this.camera_pivot_angle = 0;
                            this.camera_pivot_state = CameraPivotState.WAIT_DOWN;
                            this.camera_pivot_delay = 0.2f;
                        }
                        break;
                    case CameraPivotState.WAIT_DOWN:
                        this.camera_pivot_delay = this.camera_pivot_delay - Time.deltaTime;
                        if (this.camera_pivot_delay < 0)
                        {
                            this.camera_pivot_state = CameraPivotState.DOWN;
                        }
                        break;
                    case CameraPivotState.WAIT_UP:
                        this.camera_pivot_delay = this.camera_pivot_delay - Time.deltaTime;
                        if (this.camera_pivot_delay < 0)
                        {
                            this.camera_pivot_state = CameraPivotState.UP;
                        }
                        break;
                }
            }
            else
            {
                this.camera_pivot_angle = this.camera_pivot_angle - (Time.deltaTime * 25f);
                if (this.camera_pivot_angle < 0)
                {
                    this.camera_pivot_angle = 0;
                }
            }
            Transform cam_pivot = this.transform.Find("camera_pivot");

            {
                float _139 = this.camera_pivot_angle;
                Vector3 _140 = cam_pivot.localEulerAngles;
                _140.x = _139;
                cam_pivot.localEulerAngles = _140;
            }
            GameObject player = GameObject.Find("Player");
            float dist = Vector3.Distance(player.transform.position, this.transform.position);
            float danger = Mathf.Max(0f, 1f - (dist / this.kMaxRange));
            if (danger > 0f)
            {
                danger = Mathf.Min(0.2f, danger);
            }
            if ((this.ai_state == AIState.AIMING) || (this.ai_state == AIState.FIRING))
            {
                danger = 1f;
            }
            if ((this.ai_state == AIState.ALERT) || (this.ai_state == AIState.ALERT_COOLDOWN))
            {
                danger = danger + 0.5f;
            }
            ((MusicScript)player.GetComponent(typeof(MusicScript))).AddDangerLevel(danger);
            Transform camera = this.transform.Find("camera_pivot").Find("camera");
            rel_pos = player.transform.position - camera.position;
            bool sees_target = false;
            if ((dist < this.kMaxRange) && (Vector3.Dot(camera.rotation * new Vector3(0, -1, 0), rel_pos.normalized) > 0.7f))
            {
                if (!Physics.Linecast(camera.position, player.transform.position, out hit, 1 << 0))
                {
                    sees_target = true;
                }
            }
            if (sees_target)
            {
                Vector3 new_target = player.transform.position + (((CharacterMotor)player.GetComponent(typeof(CharacterMotor))).GetVelocity() * Mathf.Clamp(Vector3.Distance(player.transform.position, this.transform.position) * 0.1f, 0.5f, 1f));
                switch (this.ai_state)
                {
                    case AIState.IDLE:
                        this.ai_state = AIState.ALERT;
                        this.alert_delay = this.kAlertDelay;
                        this.audiosource_effect.PlayOneShot(this.sound_alert, 0.3f * PlayerPrefs.GetFloat("sound_volume", 1f));
                        break;
                    case AIState.AIMING:
                        this.target_pos = new_target;
                        if (Vector3.Distance(this.transform.position, this.target_pos) < 4)
                        {
                            this.ai_state = AIState.FIRING;
                        }
                        this.target_pos.y = this.target_pos.y + 1f;
                        break;
                    case AIState.FIRING:
                        this.target_pos = new_target;
                        if (Vector3.Distance(this.transform.position, this.target_pos) > 4)
                        {
                            this.ai_state = AIState.AIMING;
                        }
                        break;
                    case AIState.ALERT:
                        this.alert_delay = this.alert_delay - Time.deltaTime;
                        this.target_pos = new_target;
                        this.target_pos.y = this.target_pos.y + 1f;
                        if (this.alert_delay <= 0f)
                        {
                            this.ai_state = AIState.AIMING;
                        }
                        break;
                    case AIState.ALERT_COOLDOWN:
                        this.ai_state = AIState.ALERT;
                        this.alert_delay = this.kAlertDelay;
                        break;
                }
            }
            else
            {
                switch (this.ai_state)
                {
                    case AIState.AIMING:
                    case AIState.FIRING:
                    case AIState.ALERT:
                        this.ai_state = AIState.ALERT_COOLDOWN;
                        this.alert_cooldown_delay = this.kAlertCooldownDelay;
                        break;
                    case AIState.ALERT_COOLDOWN:
                        this.alert_cooldown_delay = this.alert_cooldown_delay - Time.deltaTime;
                        if (this.alert_cooldown_delay <= 0f)
                        {
                            this.ai_state = AIState.IDLE;
                            this.audiosource_effect.PlayOneShot(this.sound_unalert, 0.3f * PlayerPrefs.GetFloat("sound_volume", 1f));
                        }
                        break;
                }
            }
            switch (this.ai_state)
            {
                case AIState.IDLE:
                    this.GetDroneLightObject().GetComponent<Light>().color = new Color(0, 0, 1);
                    break;
                case AIState.AIMING:
                    this.GetDroneLightObject().GetComponent<Light>().color = new Color(1, 0, 0);
                    break;
                case AIState.ALERT:
                case AIState.ALERT_COOLDOWN:
                    this.GetDroneLightObject().GetComponent<Light>().color = new Color(1, 1, 0);
                    break;
            }
        }
        if (!this.camera_alive)
        {
            this.GetDroneLightObject().GetComponent<Light>().intensity = this.GetDroneLightObject().GetComponent<Light>().intensity * Mathf.Pow(0.01f, Time.deltaTime);
        }
        (((LensFlare)this.GetDroneLensFlareObject().GetComponent(typeof(LensFlare))) as LensFlare).color = this.GetDroneLightObject().GetComponent<Light>().color;
        (((LensFlare)this.GetDroneLensFlareObject().GetComponent(typeof(LensFlare))) as LensFlare).brightness = this.GetDroneLightObject().GetComponent<Light>().intensity;
        float target_pitch = this.rotor_speed * 0.2f;
        target_pitch = Mathf.Clamp(target_pitch, 0.2f, 3f);
        this.audiosource_motor.pitch = Mathf.Lerp(this.audiosource_motor.pitch, target_pitch, Mathf.Pow(0.0001f, Time.deltaTime));
        this.audiosource_motor.volume = (this.rotor_speed * 0.1f) * PlayerPrefs.GetFloat("sound_volume", 1f);
        this.audiosource_motor.volume = this.audiosource_motor.volume - ((Vector3.Distance(GameObject.Find("Main Camera").transform.position, this.transform.position) * 0.0125f) * PlayerPrefs.GetFloat("sound_volume", 1f));
        bool line_of_sight = true;
        if (Physics.Linecast(this.transform.position, GameObject.Find("Main Camera").transform.position, out hit, 1 << 0))
        {
            line_of_sight = false;
        }
        if (line_of_sight)
        {
            this.sound_line_of_sight = this.sound_line_of_sight + (Time.deltaTime * 3f);
        }
        else
        {
            this.sound_line_of_sight = this.sound_line_of_sight - (Time.deltaTime * 3f);
        }
        this.sound_line_of_sight = Mathf.Clamp(this.sound_line_of_sight, 0, 1);
        this.audiosource_motor.volume = this.audiosource_motor.volume * (0.5f + (this.sound_line_of_sight * 0.5f));
        ((AudioLowPassFilter)this.object_audiosource_motor.GetComponent(typeof(AudioLowPassFilter))).cutoffFrequency = Mathf.Lerp(5000, 44000, this.sound_line_of_sight);
    }

    public virtual void Update()
    {
        switch (this.robot_type)
        {
            case RobotType.STATIONARY_TURRET:
                this.UpdateStationaryTurret();
                break;
            case RobotType.SHOCK_DRONE:
                this.UpdateDrone();
                break;
        }
    }

    public virtual void OnCollisionEnter(Collision collision)
    {
        if (this.robot_type == RobotType.SHOCK_DRONE)
        {
            if (collision.impactForceSum.magnitude > 10)
            {
                if ((Random.Range(0f, 1f) < 0.5f) && this.motor_alive)
                {
                    this.Damage(this.transform.Find("motor").gameObject);
                }
                else
                {
                    if ((Random.Range(0f, 1f) < 0.5f) && this.camera_alive)
                    {
                        this.Damage(this.transform.Find("camera_pivot").Find("camera").gameObject);
                    }
                    else
                    {
                        if ((Random.Range(0f, 1f) < 0.5f) && this.battery_alive)
                        {
                            this.Damage(this.transform.Find("battery").gameObject);
                        }
                        else
                        {
                            this.motor_alive = true;
                            this.Damage(this.transform.Find("motor").gameObject);
                        }
                    }
                }
            }
            else
            {
                int which_shot = Random.Range(0, this.sound_bump.Length);
                this.audiosource_foley.PlayOneShot(this.sound_bump[which_shot], (collision.impactForceSum.magnitude * 0.15f) * PlayerPrefs.GetFloat("sound_volume", 1f));
            }
        }
    }

    public virtual void FixedUpdate()
    {
        if ((this.robot_type == RobotType.SHOCK_DRONE) && !this.distance_sleep)
        {
            this.GetComponent<Rigidbody>().AddForce((this.transform.rotation * new Vector3(0, 1, 0)) * this.rotor_speed, ForceMode.Force);
            if (this.motor_alive)
            {
                this.GetComponent<Rigidbody>().AddTorque(this.tilt_correction, ForceMode.Force);
            }
        }
    }

    public RobotScript()
    {
        this.alive = true;
        this.rotation_x = new Spring(0f, 0f, 100f, 0.0001f);
        this.rotation_y = new Spring(0f, 0f, 100f, 0.0001f);
        this.ai_state = AIState.IDLE;
        this.battery_alive = true;
        this.motor_alive = true;
        this.camera_alive = true;
        this.trigger_alive = true;
        this.barrel_alive = true;
        this.ammo_alive = true;
        this.bullets = 15;
        this.kAlertDelay = 0.6f;
        this.kAlertCooldownDelay = 2f;
        this.kMaxRange = 20f;
        this.kSleepDistance = 20f;
        this.camera_pivot_state = CameraPivotState.WAIT_DOWN;
    }

}