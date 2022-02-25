using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 基础变换修改器
/// </summary>
public class MeshModify_Transform : MeshModify
{

    public Vector3 offset;
    public Vector3 rotation;
    public Vector3 scale = Vector3.one;

    //判断是否为旧数据所需的变量
    Vector3 offset2, rot2, scale2;

    void Start()
    {
        offset2 = offset * 2f;//强制初始值不一样
    }

    public override Vector3[] ModifyEffect(Vector3[] vectors, Transform m_transform)
    {
       // print("transform!");
        Vector3[] temp = new Vector3[vectors.Length];
        for (int i = 0; i < vectors.Length; i++)
        {
            temp[i] = new Vector3(vectors[i].x * scale.x, vectors[i].y * scale.y, vectors[i].z * scale.z) + offset;

        }
        temp = RotationVector(temp, rotation);

        // MeshSize meshSize=GetMeshSize(temp);
        //print("max__"+meshSize.max.x+"___min"+meshSize.min.x+"___length__"+meshSize.length);
        return temp;
    }

    public override void GetBoolOldData()
    {
        if (offset2 != offset || rot2 != rotation || scale2 != scale)
        {
            isolddata = false;
            offset2 = offset;
            rot2 = rotation;
            scale2 = scale;
        }
        else
        { isolddata = true; }

    }


}


