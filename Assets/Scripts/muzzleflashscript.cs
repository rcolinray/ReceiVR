using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class muzzleflashscript : MonoBehaviour
{
    public float opac;
    public void UpdateColor()
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

    public void Start()
    {
        this.opac = Random.Range(0f, 1f);
        this.UpdateColor();

        {
            float _117 = Random.Range(0f, 360f);
            Quaternion _118 = this.transform.localRotation;
            Vector3 _119 = _118.eulerAngles;
            _119.z = _117;
            _118.eulerAngles = _119;
            this.transform.localRotation = _118;
        }

        {
            float _120 = Random.Range(0.8f, 2f);
            Vector3 _121 = this.transform.localScale;
            _121.x = _120;
            this.transform.localScale = _121;
        }

        {
            float _122 = Random.Range(0.8f, 2f);
            Vector3 _123 = this.transform.localScale;
            _123.y = _122;
            this.transform.localScale = _123;
        }

        {
            float _124 = Random.Range(0.8f, 2f);
            Vector3 _125 = this.transform.localScale;
            _125.z = _124;
            this.transform.localScale = _125;
        }
    }

    public void Update()
    {
        this.UpdateColor();
        this.opac = this.opac - (Time.deltaTime * 50f);
        if (this.opac <= 0f)
        {
            UnityEngine.Object.Destroy(this.gameObject);
        }
    }

}