using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]

public class WaterFog : MonoBehaviour
{
	[HideInInspector]
	public Material waterFogMat;
	float ray = 50;
    GameObject fogObj;
	public GameObject dirtness;
	Transform cached;

	public Transform followTarget;

	public bool enableFollow, follow;

	public Color  FogColor;
	public Vector3 FogSize = new Vector3(50, 50, 50);
	public float FogDensity = 5;
	public Light Sun = null;
    public Color SunLightColor = Color.white;
	public float sunIntensity = 1.3f;
	float  sunRadius = 0.77f;
	float sunDistance = 1f;
	float sunDistanceFactor = 20;
	public bool changeSortingOrder;
	public int FogSortingIndex = 0;
    MeshFilter filter;
    Mesh mesh;
    Vector3 curFog = Vector3.one;
	

    public float GetDensity()
    {
		return FogDensity;
    }

    void CreateMaterial()
    {
        waterFogMat = new Material(Shader.Find("Hidden/DCG/Water Shader/Water Fog"));
        waterFogMat.name = "Water Fog Material";
        waterFogMat.hideFlags = HideFlags.HideAndDontSave;
	
    }
    void Awake()
    {
		cached = this.transform;
        CreateMaterial();
        fogObj = this.gameObject;
        fogObj.GetComponent<Renderer>().sharedMaterial = waterFogMat;
		Keys();
        filter = gameObject.GetComponent<MeshFilter>();
        if (filter == null)
        {
            CreateMesh(transform.localScale);
            transform.localScale = Vector3.one;
        }
        UpdateBoxMesh();
    }

    static public void Wireframe(GameObject obj, bool Enable)
    {
#if UNITY_EDITOR
        EditorUtility.SetSelectedWireframeHidden(obj.GetComponent<Renderer>(), Enable);
#endif
    }

    void Update(){
#if UNITY_EDITOR
        UpdateBoxMesh();
		FogDensity = Mathf.Max(0.01f, FogDensity);
		ray = Mathf.Max(1, ray);
		sunIntensity = Mathf.Max(0, sunIntensity);
		sunDistanceFactor = Mathf.Max(1, sunDistanceFactor);
		sunDistance = Mathf.Max(0, sunDistance);
#endif
    }

    void OnWillRenderObject()
    {
#if UNITY_EDITOR
		Keys();
        Wireframe(fogObj, false);
#endif

        if (Sun)
        {
			waterFogMat.SetFloat("_sunIntensity", sunIntensity);
            waterFogMat.SetVector("LightTransform", -Sun.transform.forward);
			waterFogMat.SetFloat("sunRadius", sunRadius);
			waterFogMat.SetFloat("sunDistanceFactor", sunDistanceFactor);
            waterFogMat.SetColor("_LightColor", Sun.color * Sun.intensity);

        }

        waterFogMat.SetVector("VolumeSize", new Vector4(FogSize.x, FogSize.y, FogSize.z, 0));    

        waterFogMat.SetFloat("Exposure", Mathf.Max(0, 1f));

		waterFogMat.SetFloat("sunDistance", sunDistance);
		Vector3 VolumeSize = curFog;
        waterFogMat.SetVector("_BoxMin", VolumeSize * -.5f);
        waterFogMat.SetVector("_BoxMax", VolumeSize * .5f);
        waterFogMat.SetColor("_FogColor", FogColor);
		waterFogMat.SetColor("_SunLightColor", SunLightColor);
		waterFogMat.SetFloat("_SceneThresold", 1000);
		waterFogMat.SetFloat("_Ray", ray * .001f);
		waterFogMat.SetFloat("_FogDensity", FogDensity);
		GetComponent<Renderer>().sortingOrder = FogSortingIndex;
    }

    void Keys()
    {
        if (waterFogMat) {
			if (Sun)
				waterFogMat.EnableKeyword ("_FOG_SUNLIGHT");
			else
				waterFogMat.DisableKeyword ("_FOG_SUNLIGHT");
		}
    }

    void UpdateBoxMesh()
    {
		if (curFog != FogSize || filter == null)
            CreateMesh(FogSize);

        transform.localScale = Vector3.one;
    }

    void CreateMesh(Vector3 scale)
    {
		curFog = scale;

        if (filter == null)
            filter = gameObject.AddComponent<MeshFilter>();

        mesh = filter.sharedMesh;

        if (mesh == null)
        {
            mesh = new Mesh();
            mesh.name = gameObject.name;
            filter.sharedMesh = mesh;
        }
        mesh.Clear();

        float width = scale.y;
        float height = scale.z;
        float length = scale.x;

        Vector3 p0 = new Vector3(-length * .5f, -width * .5f, height * .5f);
        Vector3 p1 = new Vector3(length * .5f, -width * .5f, height * .5f);
        Vector3 p2 = new Vector3(length * .5f, -width * .5f, -height * .5f);
        Vector3 p3 = new Vector3(-length * .5f, -width * .5f, -height * .5f);

        Vector3 p4 = new Vector3(-length * .5f, width * .5f, height * .5f);
        Vector3 p5 = new Vector3(length * .5f, width * .5f, height * .5f);
        Vector3 p6 = new Vector3(length * .5f, width * .5f, -height * .5f);
        Vector3 p7 = new Vector3(-length * .5f, width * .5f, -height * .5f);

        Vector3[] vertices = new Vector3[]
			{
		// Bottom
				p0, p1, p2, p3,
				
		// Left
				p7, p4, p0, p3,
				
		// Front
				p4, p5, p1, p0,
				
		// Back
				p6, p7, p3, p2,
				
		// Right
				p5, p6, p2, p1,
				
		// Top
				p7, p6, p5, p4
			};

        int[] triangles = new int[]
			{
		// Bottom
				3, 1, 0,
				3, 2, 1,			
				
		// Left
				3 + 4 * 1, 1 + 4 * 1, 0 + 4 * 1,
				3 + 4 * 1, 2 + 4 * 1, 1 + 4 * 1,
				
		// Front
				3 + 4 * 2, 1 + 4 * 2, 0 + 4 * 2,
				3 + 4 * 2, 2 + 4 * 2, 1 + 4 * 2,
				
		// Back
				3 + 4 * 3, 1 + 4 * 3, 0 + 4 * 3,
				3 + 4 * 3, 2 + 4 * 3, 1 + 4 * 3,
				
		// Right
				3 + 4 * 4, 1 + 4 * 4, 0 + 4 * 4,
				3 + 4 * 4, 2 + 4 * 4, 1 + 4 * 4,
				
		// Top
				3 + 4 * 5, 1 + 4 * 5, 0 + 4 * 5,
				3 + 4 * 5, 2 + 4 * 5, 1 + 4 * 5,
				
			};

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
        ;
    }
	void LateUpdate()
	{
		if (playerTransform.IsUnderWater) {
			if(changeSortingOrder)
			{
				FogSortingIndex = 0;
			}
				dirtness.SetActive (true);
		} else {

			if(changeSortingOrder)
			{
					FogSortingIndex = -100;
			}
				dirtness.SetActive (false);
			}
		if (enableFollow) {

			if (followTarget) {

				follow = playerTransform.IsTouchingWater;

				if (follow) {
					if (cached)
						cached.position = new Vector3 (followTarget.position.x, cached.position.y, followTarget.position.z);
				}
			}
		}
	}
}

