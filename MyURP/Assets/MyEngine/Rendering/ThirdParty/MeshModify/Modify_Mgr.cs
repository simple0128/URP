using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 模型变形器管理器
/// </summary>
public class Modify_Mgr : MonoBehaviour
{
    [Tooltip("模型网格组件对象")]
    public MeshFilter meshFilter;
    [Tooltip("使用优化计算模式")]
    public bool UseBeterMod;
    [Tooltip("实时更新修改器列表")]
    public bool Updatemodifylist;
    [Tooltip("修改器列表")]
    public MeshModify[] modifies;

    ModifyData modifyData;

    Vector3[]  vectorsStart;//模型的原始顶点数组

    void Start()
    {
        if (meshFilter == null) { meshFilter = GetComponentInParent<MeshFilter>(); }

        vectorsStart = meshFilter.mesh.vertices;
        modifyData.m_transform = meshFilter.transform;

        GetModifys();
    }


    void Update()
    {
        if (Updatemodifylist) { GetModifys(); }
        UpdateModifyEffect();
    }

/// <summary>
/// 获取所有的变形修改器组件
/// </summary>
    void GetModifys()
    {
        modifies = GetComponentsInChildren<MeshModify>();
    }

/// <summary>
/// 更新效果
/// </summary>
    void UpdateModifyEffect()
    {
       // vectorstemp = vectorsStart;

        modifyData.Vectors = vectorsStart;
        modifyData.isOldVectors = true;

        foreach (MeshModify m in modifies)
        {
            if (m.enabled && m.gameObject.activeSelf)
            {
                if (UseBeterMod)
                {
                    modifyData = m.ModifyEffect(modifyData);
                    //vectorstemp = modifyData.Vectors;
                }
                else
                {
                    modifyData.Vectors = m.ModifyEffect(modifyData.Vectors, meshFilter.transform);
                }


            }

        }


        meshFilter.mesh.vertices = modifyData.Vectors;


        meshFilter.mesh.RecalculateNormals();
    }


}
