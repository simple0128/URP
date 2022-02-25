using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// 扭曲修改器
public class MeshModify_Twist : MeshModify
{
    [Tooltip("扭曲角度")]
    public float angle;
    [Tooltip("轴向选择")]
    public Axis axis;
    [Tooltip("翻转效果")]
    public bool filp;
    [Tooltip("效果限制")]
    public bool effectclamp;

    [Range(0, 1f)]
    public float clampmin = 0, clampmax = 1;


    //判断是否为旧数据所需的变量
    float old_angle, old_clampemin, old_clampemax;
    Axis old_axis;
    bool old_filp, old_effectclamp;


    void Start()
    {
        old_angle = angle + 1f;//强制初始值不一样
    }


    public override void GetBoolOldData()
    {
        if (old_angle != angle || old_axis != axis || old_clampemax != clampmax || old_clampemin != clampmin || old_filp != filp)
        {
            isolddata = false;
            old_angle = angle;
            old_axis = axis;
            old_clampemax = clampmax;
            old_clampemin = clampmin;
            old_filp = filp;
        }
        else { isolddata = true; }

    }
    public override Vector3[] ModifyEffect(Vector3[] vectors, Transform m_transform)
    {
        //print("twist!");

        if (angle == 0) { return vectors; }

        Vector3[] temp = new Vector3[vectors.Length];
        MeshSize meshSize = GetMeshSize(vectors);
        float t, angeltemp;
        for (int i = 0; i < vectors.Length; i++)
        {


            switch (axis)
            {
                case Axis.x:
                    t = Mathf.InverseLerp(meshSize.min.x, meshSize.max.x, vectors[i].x);
                    t = GetClamp(t);
                    angeltemp = Mathf.Lerp(0, angle, t);
                    temp[i] = RotationVector(vectors[i], angeltemp, Axis.x);
                    break;

                case Axis.y:
                    t = Mathf.InverseLerp(meshSize.min.y, meshSize.max.y, vectors[i].y);
                    t = GetClamp(t);
                    angeltemp = Mathf.Lerp(0, angle, t);
                    temp[i] = RotationVector(vectors[i], angeltemp, Axis.y);
                    break;

                case Axis.z:
                    t = Mathf.InverseLerp(meshSize.min.z, meshSize.max.z, vectors[i].z);
                    t = GetClamp(t);
                    angeltemp = Mathf.Lerp(0, angle, t);
                    temp[i] = RotationVector(vectors[i], angeltemp, Axis.z);
                    break;


            }

        }

        return temp;
    }


    float GetClamp(float t)
    {
        if (filp) { t = 1 - t; }
        if (!effectclamp) { return t; }
        if (clampmax < clampmin)
        {
            clampmin = clampmax;
        }
        t = Mathf.Clamp(t, clampmin, clampmax);


        return t;
    }


}
