using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//模型弯曲变形修改器
public class MeshModify_Bend : MeshModify

{
    [Tooltip("弯曲角度")]
    public float angle;
    [Tooltip("弯曲方向")]
    public float dirction;

    [Tooltip("轴向选择")]
    public Axis axis;
    // [Tooltip("弯曲半径")]
    float bendradiu;
    //  [Tooltip("弯曲中心")]
    Vector3 bendcenter;

    float angle_min, angle_max;


    //判断是否为旧数据所需的变量
    float old_angle, old_dirction;
    Axis old_axis;

    void Start()
    {
        old_angle = angle + 1f;//强制初始值不一样
    }

    public override void GetBoolOldData()
    {
        if (old_angle != angle || old_dirction != dirction || old_axis != axis)
        {
            isolddata = false;
            old_angle = angle;
            old_dirction = dirction;
            old_axis = axis;
        }
        else { isolddata = true; }

    }


    public override Vector3[] ModifyEffect(Vector3[] vectors, Transform m_transform)
    {
        // print("bend!");

        if (angle == 0) { return vectors; }
        MeshSize meshSize = GetMeshSize(vectors);
        GetBendBaseData(meshSize, axis);
        if (dirction % 360 != 0) { vectors = RotationVector(vectors, dirction, axis); }

        Vector3[] temp = new Vector3[vectors.Length];
        float t, angletemp;

        for (int i = 0; i < vectors.Length; i++)
        {
            switch (axis)
            {
                case Axis.x:
                    t = Mathf.InverseLerp(meshSize.min.x, meshSize.max.x, vectors[i].x);
                    angletemp = Mathf.Lerp(angle_min, angle_max, t);
                    temp[i].x = Mathf.Sin(angletemp) * (bendradiu + vectors[i].y);
                    temp[i].y = Mathf.Cos(angletemp) * (bendradiu + vectors[i].y);
                    temp[i].z = vectors[i].z;

                    break;

                case Axis.y:
                    t = Mathf.InverseLerp(meshSize.min.y, meshSize.max.y, vectors[i].y);
                    angletemp = Mathf.Lerp(angle_min, angle_max, t);
                    temp[i].z = Mathf.Cos(angletemp) * (bendradiu + vectors[i].z);
                    temp[i].y = Mathf.Sin(angletemp) * (bendradiu + vectors[i].z);
                    temp[i].x = vectors[i].x;

                    break;

                case Axis.z:
                    t = Mathf.InverseLerp(meshSize.min.z, meshSize.max.z, vectors[i].z);
                    angletemp = Mathf.Lerp(angle_min, angle_max, t);
                    temp[i].z = Mathf.Sin(angletemp) * (bendradiu + vectors[i].x);
                    temp[i].y = vectors[i].y;
                    temp[i].x = Mathf.Cos(angletemp) * (bendradiu + vectors[i].x);


                    break;
            }
            temp[i] += bendcenter;

        }

        if (dirction % 360 != 0) { temp = RotationVector(temp, -dirction, axis); }

        return temp;
    }


    /// <summary>
    /// 计算弯曲变形所需的基础信息：弯曲半径、中心，弯曲角度最大最小值
    /// </summary>
    /// <param name="meshSize"></param>
    /// <param name="axis"></param>
    void GetBendBaseData(MeshSize meshSize, Axis axis)
    {
        float angle_half = angle * 0.5f;
        float angle_offset = 0f;
        switch (axis)
        {
            case Axis.x:
                bendradiu = (meshSize.length.x / (angle / 360)) / Mathf.PI * 0.5f;
                bendcenter = Vector3.down * bendradiu;
                angle_offset = ((meshSize.max.x - meshSize.length.x * 0.5f) / (meshSize.length.x * 0.5f)) * angle_half;

                break;

            case Axis.y:
                bendradiu = (meshSize.length.y / (angle / 360)) / Mathf.PI * 0.5f;
                bendcenter = Vector3.back * bendradiu;
                angle_offset = ((meshSize.max.y - meshSize.length.y * 0.5f) / (meshSize.length.y * 0.5f)) * angle_half;
                break;

            case Axis.z:
                bendradiu = (meshSize.length.z / (angle / 360)) / Mathf.PI * 0.5f;
                bendcenter = Vector3.left * bendradiu;
                angle_offset = ((meshSize.max.z - meshSize.length.z * 0.5f) / (meshSize.length.z * 0.5f)) * angle_half;
                break;

        }
        angle_min = (angle_offset - angle_half) * Mathf.Deg2Rad;
        angle_max = (angle_offset + angle_half) * Mathf.Deg2Rad;
    }
}
