using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class electricsparkscript : MonoBehaviour
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
            light.intensity = this.opac * 2f;
        }
    }

    public void Start()
    {
        this.opac = Random.Range(0.4f, 1f);
        this.UpdateColor();

        {
            float _105 = Random.Range(0f, 360f);
            Quaternion _106 = this.transform.localRotation;
            Vector3 _107 = _106.eulerAngles;
            _107.z = _105;
            _106.eulerAngles = _107;
            this.transform.localRotation = _106;
        }

        {
            float _108 = Random.Range(0.8f, 2f);
            Vector3 _109 = this.transform.localScale;
            _109.x = _108;
            this.transform.localScale = _109;
        }

        {
            float _110 = Random.Range(0.8f, 2f);
            Vector3 _111 = this.transform.localScale;
            _111.y = _110;
            this.transform.localScale = _111;
        }

        {
            float _112 = Random.Range(0.8f, 2f);
            Vector3 _113 = this.transform.localScale;
            _113.z = _112;
            this.transform.localScale = _113;
        }
    }

    public void Update()
    {
        this.UpdateColor();
        this.opac = this.opac - (Time.deltaTime * 5f);
        this.transform.localScale = this.transform.localScale + ((new Vector3(1, 1, 1) * Time.deltaTime) * 30f);
        if (this.opac <= 0f)
        {
            UnityEngine.Object.Destroy(this.gameObject);
        }
    }

}