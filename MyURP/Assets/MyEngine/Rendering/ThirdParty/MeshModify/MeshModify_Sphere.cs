using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// 球形化修改器
public class MeshModify_Sphere : MeshModify
{

    public Sphere sphere;


    Sphere old_sphere_data;//判断是否为旧数据所需的变量
    void Start()
    {
        old_sphere_data.radiu = sphere.radiu + 1f;//强制初始值不一样
    }

    public override void GetBoolOldData()
    {

        if (old_sphere_data.center != sphere.center || old_sphere_data.radiu != sphere.radiu)
        {
            isolddata = false;
            old_sphere_data.center = sphere.center;
            old_sphere_data.radiu = sphere.radiu;
        }
        else { isolddata = true; }

    }

    public override Vector3[] ModifyEffect(Vector3[] vectors, Transform m_transform)
    {
       // print("sphere!");

        Vector3[] temp = new Vector3[vectors.Length];
        for (int i = 0; i < vectors.Length; i++)
        {
            temp[i] = vectors[i];
            if (Vector3.Distance(vectors[i], sphere.center) < sphere.radiu)
            {

                temp[i] = (vectors[i] - sphere.center).normalized * sphere.radiu + sphere.center;
            }
            /*   for (int j = 0; j < sphere.Length; j++)
              {

                  if (Vector3.Distance(vectors[i], sphere[j].center) < sphere[j].radiu)
                  {

                      temp[i] = (vectors[i] - sphere[j].center).normalized * sphere[j].radiu + sphere[j].center;
                  }
              } */

        }
        return temp;
    }

}

[System.Serializable]
public struct Sphere
{
    [Tooltip("中心点")]
    public Vector3 center;
    [Tooltip("半径")]
    public float radiu;

}