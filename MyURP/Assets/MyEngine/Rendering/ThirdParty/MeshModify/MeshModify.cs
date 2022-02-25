using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 模型变形器基础类
/// </summary>
public class MeshModify : MonoBehaviour
{
    public ModifyData modifyData;

/// <summary>
/// 是否是旧的参数
/// </summary>
    [HideInInspector]
    public bool isolddata = false;

    /// <summary>
    /// 修改器通用方法，各自重写功能
    /// </summary>
    /// <param name="vectors">传入顶点数组</param>
    /// <param name="m_transform">模型所在的transform</param>
    /// <returns>返回各种修改器计算的结果</returns>
    public virtual Vector3[] ModifyEffect(Vector3[] vectors, Transform m_transform)
    {
        return vectors;
    }


    /// <summary>
    ///  修改器通用方法，在计算变形前判断是否需要重新计算
    /// </summary>
    /// <param name="m_data"></param>
    /// <returns></returns>
    public  ModifyData ModifyEffect(ModifyData m_data)
    {
        GetBoolOldData();
        if (m_data.isOldVectors && isolddata)
        {
            modifyData.isOldVectors = true;
            return this.modifyData;

        }
        else
        {
            modifyData.Vectors = ModifyEffect(m_data.Vectors, m_data.m_transform);
            // print("aaaa");
            modifyData.isOldVectors = false;
            return this.modifyData;
        }
    }

    /// <summary>
    /// 判断变形器参数是否改变
    /// </summary>
    public virtual void GetBoolOldData() { }

    /// <summary>
    /// 获取顶点在模型上的t值
    /// </summary>
    /// <param name="meshSize">模型大小</param>
    /// <param name="v">顶点位置</param>
    /// <param name="axis">轴向</param>
    /// <returns>返回指定轴向的t值</returns>
    public float GetTonMeshSize(MeshSize meshSize, Vector3 v, Axis axis)
    {
        float t = 0;
        switch (axis)
        {
            case Axis.x:
                t = Mathf.InverseLerp(meshSize.min.x, meshSize.max.x, v.x);
                break;
            case Axis.y:
                t = Mathf.InverseLerp(meshSize.min.y, meshSize.max.y, v.y);
                break;
            case Axis.z:
                t = Mathf.InverseLerp(meshSize.min.z, meshSize.max.z, v.z);
                break;
        }
        return t;
    }

    /// <summary>
    /// 获取顶点在模型上的t值
    /// </summary>
    /// <param name="meshSize">模型大小</param>
    /// <param name="v">顶点位置</param>
    /// <returns>返回xyz轴向的t值</returns>
    public Vector3 GetTonMeshSize(MeshSize meshSize, Vector3 v)
    {
        Vector3 t = Vector3.zero;
        t.x = Mathf.InverseLerp(meshSize.min.x, meshSize.max.x, v.x);
        t.y = Mathf.InverseLerp(meshSize.min.y, meshSize.max.y, v.y);
        t.z = Mathf.InverseLerp(meshSize.min.z, meshSize.max.z, v.z);
        return t;
    }


    /// <summary>
    /// 根据轴向旋转位置
    /// </summary>
    /// <param name="v"></param>
    /// <param name="angle">旋转角度</param>
    /// <param name="axis">使用轴</param>
    /// <returns></returns>
    public Vector3 RotationVector(Vector3 v, float angle, Axis axis)
    {
        if (angle % 360 == 0) { return v; }
        angle *= Mathf.Deg2Rad;
        Vector3 temp = new Vector3();
        switch (axis)
        {
            case Axis.x:
                temp.x = v.x;
                temp.y = v.z * Mathf.Sin(angle) + v.y * Mathf.Cos(angle);
                temp.z = v.z * Mathf.Cos(angle) - v.y * Mathf.Sin(angle);
                break;

            case Axis.y:
                temp.x = v.z * Mathf.Sin(angle) + v.x * Mathf.Cos(angle);
                temp.y = v.y;
                temp.z = v.z * Mathf.Cos(angle) - v.x * Mathf.Sin(angle);
                break;

            case Axis.z:
                temp.x = v.x * Mathf.Cos(angle) - v.y * Mathf.Sin(angle);
                temp.y = v.x * Mathf.Sin(angle) + v.y * Mathf.Cos(angle);
                temp.z = v.z;
                break;
        }
        return temp;

    }

    /// <summary>
    /// 根据轴向旋转位置数组
    /// </summary>
    /// <param name="vectors"></param>
    /// <param name="angle">旋转角度</param>
    /// <param name="axis">使用轴</param>
    /// <returns></returns>
    public Vector3[] RotationVector(Vector3[] vectors, float angle, Axis axis)
    {

        if (angle % 360 == 0) { return vectors; }

        Vector3[] temp = new Vector3[vectors.Length];
        for (int i = 0; i < vectors.Length; i++)
        {
            temp[i] = RotationVector(vectors[i], angle, axis);
        }
        return temp;
    }

    /// <summary>
    /// 根据欧拉角旋转位置数组，旋转顺序xyz
    /// </summary>
    /// <param name="vectors">数组</param>
    /// <param name="angle">欧拉角</param>
    /// <returns></returns>
    public Vector3[] RotationVector(Vector3[] vectors, Vector3 angle)
    {
        if (angle == Vector3.zero) { return vectors; }
        Vector3[] temp = vectors;
        temp = RotationVector(temp, angle.x, Axis.x);
        temp = RotationVector(temp, angle.y, Axis.y);
        temp = RotationVector(temp, angle.z, Axis.z);
        return temp;
    }


    /// <summary>
    /// 计算模型顶点xyz的最大最小值和长度
    /// </summary>
    /// <param name="vectors"></param>
    /// <returns></returns>
    public static MeshSize GetMeshSize(Vector3[] vectors)
    {
        MeshSize meshSize = new MeshSize();
        meshSize.max = vectors[0];
        meshSize.min = vectors[0];
        foreach (Vector3 v in vectors)
        {

            if (meshSize.min.x > v.x)
            {
                meshSize.min.x = v.x;
            }

            if (meshSize.min.y > v.y)
            {
                meshSize.min.y = v.y;
            }
            if (meshSize.min.z > v.z)
            {
                meshSize.min.z = v.z;
            }

            if (meshSize.max.x < v.x)
            {
                meshSize.max.x = v.x;
            }
            if (meshSize.max.y < v.y)
            {
                meshSize.max.y = v.y;
            }
            if (meshSize.max.z < v.z)
            {
                meshSize.max.z = v.z;
            }
        }

        meshSize.length.x = meshSize.max.x - meshSize.min.x;
        meshSize.length.y = meshSize.max.y - meshSize.min.y;
        meshSize.length.z = meshSize.max.z - meshSize.min.z;
        return meshSize;
    }

/// <summary>
/// 将一组顶点位置用贝塞尔平滑，根据t值取出位置值
/// </summary>
/// <param name="vectors">位置数组</param>
/// <param name="t">t值</param>
/// <returns>返回位置值</returns>
    public static Vector3 GetPointOnBizerCurve(Vector3[] vectors, float t)
    {
       // if(vectors.Length==2){return Vector3.Lerp}
        Vector3[] temp = vectors;
        for (int i = 0; i < vectors.Length - 1; i++)
        {
            Vector3[] temp2 = new Vector3[temp.Length - 1];
            for (int j = 0; j < temp2.Length; j++)
            {
                temp2[j] = Vector3.Lerp(temp[j], temp[j + 1], t);
            }
            temp = temp2;
        }
        return temp[0];
    }
}


/// <summary>
/// 轴向枚举xyz
/// </summary>
public enum Axis { x, y, z }

/// <summary>
/// 模型的大小和XYZ轴的最大最小值
/// </summary>
public struct MeshSize
{
    /// <summary>
    /// 模型xyz最小值
    /// </summary>
    public Vector3 min;
    /// <summary>
    /// 模型xyz最大值
    /// </summary>
    public Vector3 max;
    /// <summary>
    /// 模型xyz方向的长度
    /// </summary>
    public Vector3 length;
}


/// <summary>
/// 修改器所需数据结构
/// </summary>
public struct ModifyData
{
    /// <summary>
    /// 顶点数据
    /// </summary>
    public Vector3[] Vectors;
    /// <summary>
    /// 标记是否为计算过的数据
    /// </summary>
    public bool isOldVectors;
    /// <summary>
    /// 模型对象的transform组件
    /// </summary>
    public Transform m_transform;

}
