using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class BulletScript : MonoBehaviour
{
    public AudioClip[] sound_hit_concrete;
    public AudioClip[] sound_hit_metal;
    public AudioClip[] sound_hit_glass;
    public AudioClip[] sound_hit_body;
    public AudioClip[] sound_hit_ricochet;
    public AudioClip[] sound_glass_break;
    public AudioClip[] sound_flyby;
    public GameObject bullet_obj;
    public GameObject bullet_hole_obj;
    public GameObject glass_bullet_hole_obj;
    public GameObject metal_bullet_hole_obj;
    public GameObject spark_effect;
    public GameObject puff_effect;
    private object old_pos;
    private bool hit_something;
    private LineRenderer line_renderer;
    private Vector3 velocity;
    private float life_time;
    private float death_time;
    private int segment;
    private bool hostile;
    public void SetVelocity(Vector3 vel)
    {
        this.velocity = vel;
    }

    public void SetHostile()
    {
        this.GetComponent<AudioSource>().rolloffMode = AudioRolloffMode.Logarithmic;
        this.PlaySoundFromGroup(this.sound_flyby, 0.4f);
        this.hostile = true;
    }

    public void Start()
    {
        this.line_renderer = (LineRenderer)this.GetComponent(typeof(LineRenderer));
        this.line_renderer.SetPosition(0, this.transform.position);
        this.line_renderer.SetPosition(1, this.transform.position);
        this.old_pos = this.transform.position;
    }

    public Component RecursiveHasScript(GameObject obj, string script, int depth)
    {
        if (obj.GetComponent(script))
        {
            return obj.GetComponent(script);
        }
        else
        {
            if ((depth > 0) && obj.transform.parent)
            {
                return this.RecursiveHasScript(obj.transform.parent.gameObject, script, depth - 1);
            }
            else
            {
                return null;
            }
        }
    }

    public static Quaternion RandomOrientation()
    {
        return Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
    }

    public void PlaySoundFromGroup(object[] group, float volume)
    {
        int which_shot = Random.Range(0, group.Length);
        this.GetComponent<AudioSource>().PlayOneShot((AudioClip)group[which_shot], volume * PlayerPrefs.GetFloat("sound_volume", 1f));
    }

    public void Update()
    {
        RaycastHit hit = default(RaycastHit);
        RaycastHit new_hit = default(RaycastHit);
        if (!this.hit_something)
        {
            this.life_time = this.life_time + Time.deltaTime;
            if (this.life_time > 1.5f)
            {
                this.hit_something = true;
            }
            this.transform.position = this.transform.position + (this.velocity * Time.deltaTime);
            this.velocity = this.velocity + (Physics.gravity * Time.deltaTime);
            if (Physics.Linecast((Vector3)this.old_pos, this.transform.position, out hit, ((1 << 0) | (1 << 9)) | (1 << 11)))
            {
                GameObject hit_obj = hit.collider.gameObject;
                GameObject hit_transform_obj = hit.transform.gameObject;
                ShootableLight light_script = this.RecursiveHasScript(hit_obj, "ShootableLight", 1) as ShootableLight;
                AimScript aim_script = this.RecursiveHasScript(hit_obj, "AimScript", 1) as AimScript;
                RobotScript turret_script = this.RecursiveHasScript(hit_obj, "RobotScript", 3) as RobotScript;
                this.transform.position = hit.point;
                float ricochet_amount = Vector3.Dot(this.velocity.normalized, hit.normal) * -1f;
                if ((Random.Range(0f, 1f) > ricochet_amount) && ((Vector3.Magnitude(this.velocity) * (1f - ricochet_amount)) > 10f))
                {
                    GameObject ricochet = Instantiate(this.bullet_obj, hit.point, this.transform.rotation);
                    Vector3 ricochet_vel = (this.velocity * 0.3f) * (1f - ricochet_amount);
                    this.velocity = this.velocity - ricochet_vel;
                    ricochet_vel = Vector3.Reflect(ricochet_vel, hit.normal);
                    ((BulletScript)ricochet.GetComponent(typeof(BulletScript))).SetVelocity(ricochet_vel);
                    this.PlaySoundFromGroup(this.sound_hit_ricochet, this.hostile ? 1f : 0.6f);
                }
                else
                {
                    if (turret_script && (this.velocity.magnitude > 100f))
                    {
                        if (Physics.Linecast(hit.point + (this.velocity.normalized * 0.001f), hit.point + this.velocity.normalized, out new_hit, (1 << 11) | (1 << 12)))
                        {
                            if (new_hit.collider.gameObject.layer == 12)
                            {
                                turret_script.WasShotInternal(new_hit.collider.gameObject);
                            }
                        }
                    }
                }
                if (hit_transform_obj.GetComponent<Rigidbody>())
                {
                    hit_transform_obj.GetComponent<Rigidbody>().AddForceAtPosition(this.velocity * 0.01f, hit.point, ForceMode.Impulse);
                }
                if (light_script)
                {
                    light_script.WasShot(hit_obj, hit.point, this.velocity);
                    if (hit.collider.material.name == "glass (Instance)")
                    {
                        this.PlaySoundFromGroup(this.sound_glass_break, 1f);
                    }
                }
                if (Vector3.Magnitude(this.velocity) > 50)
                {
                    GameObject hole = null;
                    GameObject effect = null;
                    if (turret_script)
                    {
                        this.PlaySoundFromGroup(this.sound_hit_metal, this.hostile ? 1f : 0.8f);
                        hole = Instantiate(this.metal_bullet_hole_obj, hit.point, BulletScript.RandomOrientation());
                        effect = Instantiate(this.spark_effect, hit.point, BulletScript.RandomOrientation());
                        turret_script.WasShot(hit_obj, hit.point, this.velocity);
                    }
                    else
                    {
                        if (aim_script)
                        {
                            hole = Instantiate(this.bullet_hole_obj, hit.point, BulletScript.RandomOrientation());
                            effect = Instantiate(this.puff_effect, hit.point, BulletScript.RandomOrientation());
                            this.PlaySoundFromGroup(this.sound_hit_body, 1f);
                            aim_script.WasShot();
                        }
                        else
                        {
                            if (hit.collider.material.name == "metal (Instance)")
                            {
                                this.PlaySoundFromGroup(this.sound_hit_metal, this.hostile ? 1f : 0.4f);
                                hole = Instantiate(this.metal_bullet_hole_obj, hit.point, BulletScript.RandomOrientation());
                                effect = Instantiate(this.spark_effect, hit.point, BulletScript.RandomOrientation());
                            }
                            else
                            {
                                if (hit.collider.material.name == "glass (Instance)")
                                {
                                    this.PlaySoundFromGroup(this.sound_hit_glass, this.hostile ? 1f : 0.4f);
                                    hole = Instantiate(this.glass_bullet_hole_obj, hit.point, BulletScript.RandomOrientation());
                                    effect = Instantiate(this.spark_effect, hit.point, BulletScript.RandomOrientation());
                                }
                                else
                                {
                                    this.PlaySoundFromGroup(this.sound_hit_concrete, this.hostile ? 1f : 0.4f);
                                    hole = Instantiate(this.bullet_hole_obj, hit.point, BulletScript.RandomOrientation());
                                    effect = Instantiate(this.puff_effect, hit.point, BulletScript.RandomOrientation());
                                }
                            }
                        }
                    }
                    effect.transform.position = effect.transform.position + (hit.normal * 0.05f);
                    hole.transform.position = hole.transform.position + (hit.normal * 0.01f);
                    if (!aim_script)
                    {
                        hole.transform.parent = hit_obj.transform;
                    }
                    else
                    {
                        hole.transform.parent = GameObject.Find("Main Camera").transform;
                    }
                }
                this.hit_something = true;
            }
            this.line_renderer.positionCount = this.segment + 1;
            this.line_renderer.SetPosition(this.segment, this.transform.position);
            ++this.segment;
        }
        else
        {
            this.life_time = this.life_time + Time.deltaTime;
            this.death_time = this.death_time + Time.deltaTime;
        }
        //Destroy(this.gameObject);
        int i = 0;
        while (i < this.segment)
        {
            Color start_color = new Color(1, 1, 1, (1f - (this.life_time * 5f)) * 0.05f);
            Color end_color = new Color(1, 1, 1, (1f - (this.death_time * 5f)) * 0.05f);
            this.line_renderer.startColor = start_color;
            this.line_renderer.endColor = end_color;
            if (this.death_time > 1f)
            {
                Destroy(this.gameObject);
            }
            ++i;
        }
    }

    public BulletScript()
    {
        this.segment = 1;
    }

}