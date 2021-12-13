using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointMartixTest : MonoBehaviour
{
    public Transform Cubu;
    private Vector3 point;
    private Matrix4x4 pointMartix;

    private Matrix4x4 pointWorldMartix;

    private Matrix4x4 pointCameraMartix;

    private Matrix4x4 pointProjectionMartix;

    private Vector2 scenePoint;
    private Matrix4x4 pointSceneMartix;

    private GUIStyle textGUIStyle;

    private string inputX;
    private string inputY;
    private string inputZ;

    private string rotationX;
    private string rotationY;
    private string rotationZ;
    private Quaternion quaternion;
    private Vector3 rotation;

    private string positionX;
    private string positionY;
    private string positionZ;
    private Vector3 position;

    private bool showWorld;
    private bool showCamera;
    private bool showProjection;
    private bool showScene;

    void Start()
    {
        textGUIStyle = new GUIStyle();
        textGUIStyle.fontSize = 30;
        GUIStyleState normalStyleState = new GUIStyleState();
        normalStyleState.textColor = Color.white;
        textGUIStyle.normal = normalStyleState;

        inputX = "0";
        inputY = "0";
        inputZ = "0";

        quaternion = Cubu.rotation;
        Vector3 rotation = quaternion.eulerAngles;
        rotationX = rotation.x.ToString();
        rotationY = rotation.y.ToString();
        rotationZ = rotation.z.ToString();

        Vector3 position = Cubu.position;
        positionX = position.x.ToString();
        positionY = position.x.ToString();
        positionZ = position.x.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        pointMartix = new Matrix4x4(new Vector4(point.x, point.y, point.z, 1), Vector4.zero, Vector4.zero, Vector4.zero);
        computePointWorld();
        computePointCamera();
        computePointProjection();
        computePointScene();
    }

    private void OnGUI()
    {
        int firstHeight = 30;
        showInputPosition(firstHeight, "cube上的位置。", ref point);

        firstHeight += 50;
        showRotation(firstHeight, "cube旋转角度。", ref rotation);
        quaternion.eulerAngles = rotation;
        Cubu.rotation = quaternion;

        firstHeight += 50;
        showPosition(firstHeight, "cube位置。", ref position);
        Cubu.position = position;

        firstHeight += 50;
        if (GUI.Button(new Rect(20, firstHeight, 200, 40), "point -> world"))
        {
            showWorld = !showWorld;
            if (showWorld)
            {
                showCamera = false;
                showProjection = false;
                showScene = false;
            }
        }
        if (GUI.Button(new Rect(230, firstHeight, 200, 40), "world -> camera"))
        {
            showCamera = !showCamera;
            if (showCamera)
            {
                showWorld = false;
                showProjection = false;
                showScene = false;
            }
        }
        if (GUI.Button(new Rect(440, firstHeight, 200, 40), "camera -> projection"))
        {
            showProjection = !showProjection;
            if (showProjection)
            {
                showWorld = false;
                showCamera = false;
                showScene = false;
            }
        }
        if (GUI.Button(new Rect(650, firstHeight, 200, 40), "projection -> scene"))
        {
            showScene = !showScene;
            if (showScene)
            {
                showWorld = false;
                showCamera = false;
                showProjection = false;
            }
        }

        firstHeight += 50;
        if (showWorld)
        {
            showMatrix4x4(firstHeight, "M (world)", Cubu.localToWorldMatrix);
            showMatrix4x4(firstHeight + 150, "P (world)", pointWorldMartix);
        }
        if (showCamera)
        {
            showMatrix4x4(firstHeight, "M (camera)", Camera.main.worldToCameraMatrix);
            showMatrix4x4(firstHeight + 150, "P (camera)", pointCameraMartix);
        }
        if (showProjection)
        {
            showMatrix4x4(firstHeight, "M (projection)", Camera.main.projectionMatrix);
            showMatrix4x4(firstHeight + 150, "P (projection)", pointProjectionMartix);
        }
        if (showScene)
        {
            GUI.Label(new Rect(20, firstHeight, 300, 30), "Screen Width:" + Screen.width + " Screen Height:" + Screen.height, textGUIStyle);
            showPosition(firstHeight + 30, "P (scene)", scenePoint);
            GUI.Label(new Rect(scenePoint.x - 5, Screen.height - (scenePoint.y + 5), 10, 10), "*", textGUIStyle);
        }
    }
    private void showInputPosition(float height, string title, ref Vector3 point)
    {
        GUI.Label(new Rect(20, height, 250, 30), title, textGUIStyle);

        GUI.Label(new Rect(260, height, 20, 30), "x:", textGUIStyle);
        inputX = GUI.TextField(new Rect(290, height, 60, 30), inputX, textGUIStyle);
        if (!string.IsNullOrEmpty(inputX))
        {
            float px;
            if (float.TryParse(inputX, out px))
            {
                point.x = px;
            }
        }

        GUI.Label(new Rect(420, height, 20, 30), "y:", textGUIStyle);
        inputY = GUI.TextField(new Rect(450, height, 60, 30), inputY, textGUIStyle);
        if (!string.IsNullOrEmpty(inputY))
        {
            float py;
            if (float.TryParse(inputY, out py))
            {
                point.y = py;
            }
        }

        GUI.Label(new Rect(570, height, 20, 30), "z:", textGUIStyle);
        inputZ = GUI.TextField(new Rect(620, height, 60, 30), inputZ, textGUIStyle);
        if (!string.IsNullOrEmpty(inputZ))
        {
            float pz;
            if (float.TryParse(inputZ, out pz))
            {
                point.z = pz;
            }
        }
    }

    private void showRotation(float height, string title, ref Vector3 point)
    {
        GUI.Label(new Rect(20, height, 250, 30), title, textGUIStyle);

        GUI.Label(new Rect(260, height, 20, 30), "x:", textGUIStyle);
        rotationX = GUI.TextField(new Rect(290, height, 60, 30), rotationX, textGUIStyle);
        if (!string.IsNullOrEmpty(rotationX))
        {
            float px;
            if (float.TryParse(rotationX, out px))
            {
                if (px == 0)
                {
                    point.x = 0;
                }
                else
                {
                    point.x = px;
                }
            }
        }

        GUI.Label(new Rect(420, height, 20, 30), "y:", textGUIStyle);
        rotationY = GUI.TextField(new Rect(450, height, 60, 30), rotationY, textGUIStyle);
        if (!string.IsNullOrEmpty(rotationY))
        {
            float py;
            if (float.TryParse(rotationY, out py))
            {
                if (py == 0)
                {
                    point.y = 0;
                }
                else
                {
                    point.y = py;
                }
            }
        }

        GUI.Label(new Rect(570, height, 20, 30), "z:", textGUIStyle);
        rotationZ = GUI.TextField(new Rect(620, height, 60, 30), rotationZ, textGUIStyle);
        if (!string.IsNullOrEmpty(rotationZ))
        {
            float pz;
            if (float.TryParse(rotationZ, out pz))
            {
                if (pz == 0)
                {
                    point.z = 0;
                }
                else
                {
                    point.z = pz;
                }
            }
        }
    }

    private void showPosition(float height, string title, ref Vector3 point)
    {
        GUI.Label(new Rect(20, height, 250, 30), title, textGUIStyle);

        GUI.Label(new Rect(260, height, 20, 30), "x:", textGUIStyle);
        positionX = GUI.TextField(new Rect(290, height, 60, 30), positionX, textGUIStyle);
        if (!string.IsNullOrEmpty(positionX))
        {
            float px;
            if (float.TryParse(positionX, out px))
            {
                if (px == 0)
                {
                    point.x = 0;
                }
                else
                {
                    point.x = px;
                }
            }
        }

        GUI.Label(new Rect(420, height, 20, 30), "y:", textGUIStyle);
        positionY = GUI.TextField(new Rect(450, height, 60, 30), positionY, textGUIStyle);
        if (!string.IsNullOrEmpty(positionY))
        {
            float py;
            if (float.TryParse(positionY, out py))
            {
                if (py == 0)
                {
                    point.y = 0;
                }
                else
                {
                    point.y = py;
                }
            }
        }

        GUI.Label(new Rect(570, height, 20, 30), "z:", textGUIStyle);
        positionZ = GUI.TextField(new Rect(620, height, 60, 30), positionZ, textGUIStyle);
        if (!string.IsNullOrEmpty(positionZ))
        {
            float pz;
            if (float.TryParse(positionZ, out pz))
            {
                if (pz == 0)
                {
                    point.z = 0;
                }
                else
                {
                    point.z = pz;
                }
            }
        }
    }


    private void showMatrix4x4(float height, string title, Matrix4x4 matrix)
    {
        GUI.Label(new Rect(20, height, 250, height), title, textGUIStyle);

        GUI.Label(new Rect(260, height, 200, 200), matrix.ToString(), textGUIStyle);
    }

    private void showPosition(float height, string title, Vector3 point)
    {
        GUI.Label(new Rect(20, height, 250, 30), title, textGUIStyle);
        GUI.Label(new Rect(260, height, 200, 200), point.ToString(), textGUIStyle);
    }

    private void computePointWorld()
    {
        Matrix4x4 localToWorldMatrix = Cubu.localToWorldMatrix;
        pointWorldMartix = Cubu.localToWorldMatrix * pointMartix;

        Debug.Log("point -> world. \n " + "\n" + localToWorldMatrix + " \n*\n " + pointMartix + " \n=\n " + pointWorldMartix);
    }

    private void computePointCamera()
    {
        Matrix4x4 worldToCameraMatrix = Camera.main.worldToCameraMatrix;
        pointCameraMartix = worldToCameraMatrix * pointWorldMartix;

        Debug.Log("world -> camera. \n " + "\n" + worldToCameraMatrix + " \n*\n " + pointWorldMartix + " \n=\n " + pointCameraMartix);
    }

    private void computePointProjection()
    {
        Matrix4x4 projectionMatrix = Camera.main.projectionMatrix;
        pointProjectionMartix = projectionMatrix * pointCameraMartix;

        Debug.Log("camera -> projection. \n " + "\n" + projectionMatrix + " \n*\n " + pointCameraMartix + " \n=\n " + pointProjectionMartix);
    }

    private void computePointScene()
    {
        scenePoint = Vector3.zero;
        float width = Screen.width;
        float height = Screen.height;
        float x = pointProjectionMartix.m00;
        float y = pointProjectionMartix.m10;
        float w = pointProjectionMartix.m30;
        scenePoint.x = ((x * width) / (2 * w)) + width / 2;
        scenePoint.y = ((y * height) / (2 * w)) + height / 2;

        Debug.Log("projection -> scene . \n scenePoint:" + scenePoint + "\n width:" + width + " height:" + height);
    }
}

