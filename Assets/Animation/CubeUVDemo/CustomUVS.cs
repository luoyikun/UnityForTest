﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class CustomUVS : MonoBehaviour
{
    MeshFilter msf;
    Mesh mesh;
    [SerializeField]
    Vector2[] uvs;
    // Use this for initialization
    void Start()
    {
        msf = GetComponent<MeshFilter>();
        Mesh meshCopy = Mesh.Instantiate(msf.sharedMesh) as Mesh;    // Make a deep copy
        meshCopy.name = "Cube2";
        mesh = msf.mesh = meshCopy;                                // Assign the copy to the meshes
        if (mesh == null || mesh.uv.Length != 24)
        {
            Debug.Log("Script needs to be attached to built-in cube");
            return;
        }

        uvs = mesh.uv;

        // Front
        uvs[0] = new Vector2(0.0f, 0.0f);
        uvs[1] = new Vector2(0.333f, 0.0f);
        uvs[2] = new Vector2(0.0f, 0.333f);
        uvs[3] = new Vector2(0.333f, 0.333f);

        // Top
        uvs[8] = new Vector2(0.334f, 0.0f);
        uvs[9] = new Vector2(0.666f, 0.0f);
        uvs[4] = new Vector2(0.334f, 0.333f);
        uvs[5] = new Vector2(0.666f, 0.333f);

        // Back
        uvs[10] = new Vector2(0.667f, 0.0f);
        uvs[11] = new Vector2(1.0f, 0.0f);
        uvs[6] = new Vector2(0.667f, 0.333f);
        uvs[7] = new Vector2(1.0f, 0.333f);

        // Bottom
        uvs[15] = new Vector2(0.0f, 0.333f);
        uvs[14] = new Vector2(0.333f, 0.334f);
        uvs[12] = new Vector2(0.0f, 0.666f);
        uvs[13] = new Vector2(0.333f, 0.666f);

        // Left
        uvs[19] = new Vector2(0.334f, 0.334f);
        uvs[18] = new Vector2(0.666f, 0.334f);
        uvs[16] = new Vector2(0.334f, 0.666f);
        uvs[17] = new Vector2(0.666f, 0.666f);

        // Right        
        uvs[23] = new Vector2(0.667f, 0.334f);
        uvs[22] = new Vector2(1.00f, 0.334f);
        uvs[20] = new Vector2(0.667f, 0.666f);
        uvs[21] = new Vector2(1.0f, 0.666f);

        mesh.uv = uvs;
    }

}