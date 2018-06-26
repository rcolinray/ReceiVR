using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class plasterpuffscript : MonoBehaviour
{
    public float opac;
    public void UpdateColor()
    {
        Component[] renderers = this.transform.GetComponentsInChildren(typeof(MeshRenderer));
        Vector4 color = new Vector4(0, 0, 0, this.opac * 0.2f);
        foreach (MeshRenderer renderer in renderers)
        {
            renderer.material.SetColor("_TintColor", color);
        }
    }

    public void Start()
    {
        this.opac = Random.Range(0f, 1f);
        this.UpdateColor();

        {
            float _126 = Random.Range(0f, 360f);
            Quaternion _127 = this.transform.localRotation;
            Vector3 _128 = _127.eulerAngles;
            _128.z = _126;
            _127.eulerAngles = _128;
            this.transform.localRotation = _127;
        }

        {
            float _129 = Random.Range(0.8f, 2f);
            Vector3 _130 = this.transform.localScale;
            _130.x = _129;
            this.transform.localScale = _130;
        }

        {
            float _131 = Random.Range(0.8f, 2f);
            Vector3 _132 = this.transform.localScale;
            _132.y = _131;
            this.transform.localScale = _132;
        }

        {
            float _133 = Random.Range(0.8f, 2f);
            Vector3 _134 = this.transform.localScale;
            _134.z = _133;
            this.transform.localScale = _134;
        }
    }

    public void Update()
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