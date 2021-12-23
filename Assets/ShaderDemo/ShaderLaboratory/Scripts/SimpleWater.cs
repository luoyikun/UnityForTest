using System.Collections;
using UnityEngine;

public class SimpleWater : MonoBehaviour
{
    [Range(0,0.5f)]
    public float waveWidth = 0.1f;

    [Range(0.01f, 0.1f)]
    public float timeMince = 0.02f;

    [Range(0,1)]
    public float waveDissolveRatioTime = 0.5f;

    private Vector3 point = new Vector3(0, 0, 0);

    private Material material;

    private float panelWidth;
    private float panelHeight;

    private bool isCollision = false;

    private void Awake()
    {
        material = GetComponent<MeshRenderer>().material;

        if (material == null)
            Debug.Log("material is null!");

        panelWidth = transform.localScale.x * 5;
        panelHeight = transform.localScale.z * 5;

        material.SetVector("_wavePos", new Vector4(0, 0, 0, 0));
        material.SetFloat("_StartWaveWidth", 0);
        material.SetFloat("_EndWaveWidth", 0);
    } 

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        //碰撞点坐标 
        Vector3 pos = contact.point; 

        pos -= transform.position;

        pos.x /= panelWidth;
        pos.z /= panelHeight;

        pos.x = pos.x / 2 + 0.5f;
        pos.z = pos.z / 2 + 0.5f;

        isCollision = true;

        StartCoroutine(ChangeWaveWidth("_StartWaveWidth", 0.8f,true));

        //把panel的长宽也传递过去，以消除缩放影响
        material.SetVector("_wavePos", new Vector4(1 - pos.x, 1 - pos.z, panelWidth, panelHeight));
    }

    private IEnumerator ChangeWaveWidth(string name, float time, bool isStartWaveWidth)
    {
        //把设定的waveWidth每timeMince传给shader一次，每传一次加一点，从0逐渐接近设定的waveWidth值
        for (int i = 0; i < time / timeMince; i++)
        {
            material.SetFloat(name, waveWidth * i / (time * 50));

            //在设定的wave消失时间(比例)时，开始给shader的EndWaveWidth，这样就可以模拟波从内圈开始消失了
            if (Mathf.Abs(i - waveDissolveRatioTime * time / timeMince) <= 0.05f)
            {
                if (isStartWaveWidth)
                    StartCoroutine(ChangeWaveWidth("_EndWaveWidth", 1f, false));
            }
            yield return new WaitForSeconds(timeMince);
        }

        StopCoroutine(ChangeWaveWidth(name, time, isStartWaveWidth));
    }
}
