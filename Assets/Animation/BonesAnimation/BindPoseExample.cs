using UnityEngine;

// 此示例从头开始创建四边形网格，创建骨骼并分配它们，
// 并根据简单的动画曲线设置骨骼动作的动画以使四边形网格生成动画。
public class BindPoseExample : MonoBehaviour
{
    void Start()
    {
        gameObject.AddComponent<Animation>();
        gameObject.AddComponent<SkinnedMeshRenderer>();
        SkinnedMeshRenderer rend = GetComponent<SkinnedMeshRenderer>();
        Animation anim = GetComponent<Animation>();

        // 构建基本网格
        Mesh mesh = new Mesh();
        mesh.vertices = new Vector3[] { new Vector3(-1, 0, 0), new Vector3(1, 0, 0), new Vector3(-1, 5, 0), new Vector3(1, 5, 0) };
        mesh.uv = new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1) };
        mesh.triangles = new int[] { 0, 1, 2, 1, 3, 2 };
        mesh.RecalculateNormals();
        rend.material = new Material(Shader.Find("Diffuse"));

        // 将骨骼权重指定给网格
        // 可以用一个，两个或四个骨骼对每个顶点进行修饰，所有骨骼的权重总和为1
        // 下面的例子，我们创建了两个骨骼，一个在Mesh的底部，另一个在Mesh的顶部，
        // 由于每个顶点只受到一个骨骼影响，所以对应weight0都是1
        // 同时，BoneWeight数组与顶点数组一一对应
        // 附着在0，1索引顶点是第0个骨骼, 所以boneIndex0为0 
        // 附着在2，3索引顶点是第1个骨骼, 所以boneIndex0为1
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

        // 创建 Bone的Transforms 和 Bind poses
        Transform[] bones = new Transform[2];
        Matrix4x4[] bindPoses = new Matrix4x4[2];
        bones[0] = new GameObject("Lower").transform;
        bones[0].parent = transform;
        // 设置相对于父级的位置
        bones[0].localRotation = Quaternion.identity;
        bones[0].localPosition = Vector3.zero;
        // 绑定姿势是骨骼的逆矩阵。在这种情况下，我们也相对于根生成这个矩阵。 这样我们就可以自由地移动根游戏对象
        bindPoses[0] = bones[0].worldToLocalMatrix * transform.localToWorldMatrix;

        bones[1] = new GameObject("Upper").transform;
        bones[1].parent = transform;
        // 设置相对于父级的位置
        bones[1].localRotation = Quaternion.identity;
        bones[1].localPosition = new Vector3(0, 5, 0);
        // 绑定姿势是骨骼的逆矩阵。在这种情况下，我们也相对于根生成这个矩阵。 这样我们就可以自由地移动根游戏对象
        bindPoses[1] = bones[1].worldToLocalMatrix * transform.localToWorldMatrix;

        // 之前创建了bindPoses，并使用所需的矩阵进行了更新。
        // 现在将bindPoses数组分配给Mesh中的bindposes。
        mesh.bindposes = bindPoses;

        // 分配骨骼和Mesh
        rend.bones = bones;
        rend.sharedMesh = mesh;

        // 将简单的波动动画分配给底部骨骼
        AnimationCurve curve = new AnimationCurve();
        curve.keys = new Keyframe[] { new Keyframe(0, 0, 0, 0), new Keyframe(1, 3, 0, 0), new Keyframe(2, 0.0F, 0, 0) };

        // 创建带Curve曲线的Clip
        AnimationClip clip = new AnimationClip();
        clip.SetCurve("Lower", typeof(Transform), "m_LocalPosition.z", curve);
        clip.legacy = true;

        // 添加并运行Clip
        clip.wrapMode = WrapMode.Loop;
        anim.AddClip(clip, "test");
        anim.Play("test");
    }
}