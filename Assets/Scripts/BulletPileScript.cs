using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class BulletPileScript : MonoBehaviour
{
    public void Start()
    {
        GUISkinHolder holder = (GUISkinHolder)GameObject.Find("gui_skin_holder").GetComponent(typeof(GUISkinHolder));
        WeaponHolder weapon_holder = (WeaponHolder)holder.weapon.GetComponent(typeof(WeaponHolder));
        int num_bullets = Random.Range(1, 6);
        int i = 0;
        while (i < num_bullets)
        {
            GameObject bullet = UnityEngine.Object.Instantiate(weapon_holder.bullet_object);
            bullet.transform.position = this.transform.position + new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(0f, 0.2f), Random.Range(-0.1f, 0.1f));
            bullet.transform.rotation = BulletScript.RandomOrientation();
            bullet.AddComponent(typeof(Rigidbody));
            ((ShellCasingScript)bullet.GetComponent(typeof(ShellCasingScript))).collided = true;
            ++i;
        }
        if (Random.Range(0, 4) == 0)
        {
            GameObject tape = UnityEngine.Object.Instantiate(holder.tape_object);
            tape.transform.position = this.transform.position + new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(0f, 0.2f), Random.Range(-0.1f, 0.1f));
            tape.transform.rotation = BulletScript.RandomOrientation();
        }
        if ((Random.Range(0, 4) == 0) && !holder.has_flashlight)
        {
            GameObject flashlight = UnityEngine.Object.Instantiate(holder.flashlight_object);
            flashlight.transform.position = this.transform.position + new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(0.2f, 0.4f), Random.Range(-0.1f, 0.1f));
            flashlight.transform.rotation = BulletScript.RandomOrientation();
        }
    }

    public void Update()
    {
    }

}