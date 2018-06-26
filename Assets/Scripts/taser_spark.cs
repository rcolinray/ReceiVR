using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class taser_spark : MonoBehaviour
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
            light.intensity = this.opac * 2f;
        }
    }

    public virtual void Start()
    {
        this.opac = Random.Range(0.4f, 1f);
        this.UpdateColor();

        {
            float _150 = Random.Range(0f, 360f);
            Quaternion _151 = this.transform.localRotation;
            Vector3 _152 = _151.eulerAngles;
            _152.z = _150;
            _151.eulerAngles = _152;
            this.transform.localRotation = _151;
        }

        {
            float _153 = Random.Range(0.8f, 2f);
            Vector3 _154 = this.transform.localScale;
            _154.x = _153;
            this.transform.localScale = _154;
        }

        {
            float _155 = Random.Range(0.8f, 2f);
            Vector3 _156 = this.transform.localScale;
            _156.y = _155;
            this.transform.localScale = _156;
        }

        {
            float _157 = Random.Range(0.8f, 2f);
            Vector3 _158 = this.transform.localScale;
            _158.z = _157;
            this.transform.localScale = _158;
        }
    }

    public virtual void Update()
    {
        this.UpdateColor();
        this.opac = this.opac - (Time.deltaTime * 50f);
        if (this.opac <= 0f)
        {
            UnityEngine.Object.Destroy(this.gameObject);
        }
    }

}