using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UVTest : MonoBehaviour
{
    enum ShowUVPartType
    {
        None = 0,
        LeftTop,
        LeftBottom,
        Center,
        RightTop,
        RightBottom
    };

    List<Vector2> uvs;
    Vector2[] backUv;
    Mesh mesh;
    [SerializeField] ShowUVPartType showUVPartType;

    [SerializeField] bool isPlayRandomUvPart;

    ShowUVPartType currentUvPartType = ShowUVPartType.None;
    private void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        uvs = mesh.uv.ToList();
        backUv = mesh.uv;
    }

    float totalTime;
    private void LateUpdate()
    {
        if (isPlayRandomUvPart)
        {

            totalTime += Time.deltaTime;
            if (totalTime >= 0.7f)
            {
                totalTime = 0;
                int random = Random.Range(1, 6);
                showUVPartType = (ShowUVPartType)random;
            }

        }

        if (currentUvPartType != showUVPartType)
        {
            currentUvPartType = showUVPartType;
            switch (showUVPartType)
            {
                case ShowUVPartType.Center:
                    mesh.SetUVs(0, backUv);
                    break;
                case ShowUVPartType.LeftBottom:
                    ShowLeftBottomUvPart();
                    break;
                case ShowUVPartType.LeftTop:
                    ShowLeftTopUvPart();
                    break;
                case ShowUVPartType.RightBottom:
                    ShowRightBottomUvPart();
                    break;
                case ShowUVPartType.RightTop:
                    ShowRightTopUvPart();
                    break;
            }
        }
    }




    void ShowLeftTopUvPart()
    {
        //вС-сроб-вСио-срио
        uvs[0] = new Vector2(0, 0.75f);
        uvs[1] = new Vector2(0.25f, 0.75f);
        uvs[2] = new Vector2(0, 1);
        uvs[3] = new Vector2(0.25f, 1);

        mesh.SetUVs(0, uvs);
    }


    void ShowRightTopUvPart()
    {

        uvs[0] = new Vector2(0.75f, 0.75f);
        uvs[1] = new Vector2(1, 0.75f);
        uvs[2] = new Vector2(0.75f, 1);
        uvs[3] = new Vector2(1, 1);

        mesh.SetUVs(0, uvs);
    }


    void ShowLeftBottomUvPart()
    {

        uvs[0] = new Vector2(0, 0);
        uvs[1] = new Vector2(0.25f, 0);
        uvs[2] = new Vector2(0, 0.25f);
        uvs[3] = new Vector2(0.25f, 0.25f);

        mesh.SetUVs(0, uvs);
    }


    void ShowRightBottomUvPart()
    {

        uvs[0] = new Vector2(0.75f, 0);
        uvs[1] = new Vector2(1, 0);
        uvs[2] = new Vector2(0.75f, 0.25f);
        uvs[3] = new Vector2(1, 0.25f);

        mesh.SetUVs(0, uvs);
    }


}

