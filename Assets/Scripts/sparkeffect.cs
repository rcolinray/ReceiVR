using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class sparkeffect : MonoBehaviour
{
    public float opac;
    public virtual void UpdateColor()
    {
        Component[] renderers = this.transform.GetComponentsInChildren(typeof(MeshRenderer));
        Vector4 color = new Vector4(this.opac, this.opac, this.opac, this.opac);
        foreach (MeshRenderer renderer in renderers)
        {
            renderer.material.SetColor("_TintColor", color);
        }
        Component[] lights = this.transform.GetComponentsInChildren(typeof(Light));
        foreach (Light light in lights)
        {
            light.intensity = this.opac;
        }
    }

    public virtual void Start()
    {
        this.opac = Random.Range(0f, 1f);
        this.UpdateColor();

        {
            float _141 = Random.Range(0f, 360f);
            Quaternion _142 = this.transform.localRotation;
            Vector3 _143 = _142.eulerAngles;
            _143.z = _141;
            _142.eulerAngles = _143;
            this.transform.localRotation = _142;
        }

        {
            float _144 = Random.Range(0.8f, 2f);
            Vector3 _145 = this.transform.localScale;
            _145.x = _144;
            this.transform.localScale = _145;
        }

        {
            float _146 = Random.Range(0.8f, 2f);
            Vector3 _147 = this.transform.localScale;
            _147.y = _146;
            this.transform.localScale = _147;
        }

        {
            float _148 = Random.Range(0.8f, 2f);
            Vector3 _149 = this.transform.localScale;
            _149.z = _148;
            this.transform.localScale = _149;
        }
    }

    public virtual void Update()
    {
        this.UpdateColor();
        this.opac = this.opac - (Time.deltaTime * 10f);
        this.transform.localScale = this.transform.localScale + ((new Vector3(1, 1, 1) * Time.deltaTime) * 30f);
        if (this.opac <= 0f)
        {
            UnityEngine.Object.Destroy(this.gameObject);
        }
    }

}