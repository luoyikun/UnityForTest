using UnityEngine;

// ��ʾ����ͷ��ʼ�����ı������񣬴����������������ǣ�
// �����ݼ򵥵Ķ����������ù��������Ķ�����ʹ�ı����������ɶ�����
public class BindPoseExample : MonoBehaviour
{
    void Start()
    {
        gameObject.AddComponent<Animation>();
        gameObject.AddComponent<SkinnedMeshRenderer>();
        SkinnedMeshRenderer rend = GetComponent<SkinnedMeshRenderer>();
        Animation anim = GetComponent<Animation>();

        // ������������
        Mesh mesh = new Mesh();
        mesh.vertices = new Vector3[] { new Vector3(-1, 0, 0), new Vector3(1, 0, 0), new Vector3(-1, 5, 0), new Vector3(1, 5, 0) };
        mesh.uv = new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1) };
        mesh.triangles = new int[] { 0, 1, 2, 1, 3, 2 };
        mesh.RecalculateNormals();
        rend.material = new Material(Shader.Find("Diffuse"));

        // ������Ȩ��ָ��������
        // ������һ�����������ĸ�������ÿ������������Σ����й�����Ȩ���ܺ�Ϊ1
        // ��������ӣ����Ǵ���������������һ����Mesh�ĵײ�����һ����Mesh�Ķ�����
        // ����ÿ������ֻ�ܵ�һ������Ӱ�죬���Զ�Ӧweight0����1
        // ͬʱ��BoneWeight�����붥������һһ��Ӧ
        // ������0��1���������ǵ�0������, ����boneIndex0Ϊ0 
        // ������2��3���������ǵ�1������, ����boneIndex0Ϊ1
        BoneWeight[] weights = new BoneWeight[4];
        weights[0].boneIndex0 = 0;
        weights[0].weight0 = 1;
        weights[1].boneIndex0 = 0;
        weights[1].weight0 = 1;
        weights[2].boneIndex0 = 1;
        weights[2].weight0 = 1;
        weights[3].boneIndex0 = 1;
        weights[3].weight0 = 1;
        mesh.boneWeights = weights;

        // ���� Bone��Transforms �� Bind poses
        Transform[] bones = new Transform[2];
        Matrix4x4[] bindPoses = new Matrix4x4[2];
        bones[0] = new GameObject("Lower").transform;
        bones[0].parent = transform;
        // ��������ڸ�����λ��
        bones[0].localRotation = Quaternion.identity;
        bones[0].localPosition = Vector3.zero;
        // �������ǹ��������������������£�����Ҳ����ڸ������������ �������ǾͿ������ɵ��ƶ�����Ϸ����
        bindPoses[0] = bones[0].worldToLocalMatrix * transform.localToWorldMatrix;

        bones[1] = new GameObject("Upper").transform;
        bones[1].parent = transform;
        // ��������ڸ�����λ��
        bones[1].localRotation = Quaternion.identity;
        bones[1].localPosition = new Vector3(0, 5, 0);
        // �������ǹ��������������������£�����Ҳ����ڸ������������ �������ǾͿ������ɵ��ƶ�����Ϸ����
        bindPoses[1] = bones[1].worldToLocalMatrix * transform.localToWorldMatrix;

        // ֮ǰ������bindPoses����ʹ������ľ�������˸��¡�
        // ���ڽ�bindPoses��������Mesh�е�bindposes��
        mesh.bindposes = bindPoses;

        // ���������Mesh
        rend.bones = bones;
        rend.sharedMesh = mesh;

        // ���򵥵Ĳ�������������ײ�����
        AnimationCurve curve = new AnimationCurve();
        curve.keys = new Keyframe[] { new Keyframe(0, 0, 0, 0), new Keyframe(1, 3, 0, 0), new Keyframe(2, 0.0F, 0, 0) };

        // ������Curve���ߵ�Clip
        AnimationClip clip = new AnimationClip();
        clip.SetCurve("Lower", typeof(Transform), "m_LocalPosition.z", curve);
        clip.legacy = true;

        // ��Ӳ�����Clip
        clip.wrapMode = WrapMode.Loop;
        anim.AddClip(clip, "test");
        anim.Play("test");
    }
}