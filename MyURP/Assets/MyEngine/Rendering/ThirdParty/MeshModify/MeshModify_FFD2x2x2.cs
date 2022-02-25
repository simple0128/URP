using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//模型FFD2X2X2修改器
public class MeshModify_FFD2x2x2 : MeshModify
{
    [Tooltip("重置辅助点")]
    public bool resetstartpos;

    // [Tooltip("[0]x0y0z0,[1]x0y0z1,[2]x0y1z1,[3]x0y1z0\n[4]x1y0z0,[5]x1y0z1,[5]x1y1z1,[7]x1y1z0")]
    Vector3[] helperpoint = new Vector3[8];//数组不能K动画，可以改用8个单独的变量

    public Vector3 helperpoint0, helperpoint1, helperpoint2, helperpoint3, helperpoint4, helperpoint5, helperpoint6, helperpoint7;//数组不能K动画，可以改用8个单独的变量
    [Tooltip("显示Gzimos")]
    public bool ViewGzimos = true;
    [Tooltip("辅助点大小")]
    public float gizmosSize = 1f;

    public Color gizmospointColor = Color.white;// gizmos端点颜色
    public Color gizmosXaxisColor = Color.red;// gizmos x方向线条颜色
    public Color gizmosYaxisColor = Color.green;// gizmos y方向线条颜色
    public Color gizmosZaxisColor = Color.blue;// gizmos z方向线条颜色

    MeshSize meshSize;

    //判断是否为旧数据所需的变量
    Vector3[] old_helperpoint = new Vector3[8];

    void Start()
    {
        resetstartpos = false;
        Sethelperpionts();
        old_helperpoint[0] = helperpoint[0] + Vector3.up;//强制初始值不一样

    }
    public override void GetBoolOldData()

    {
        Sethelperpionts();
        isolddata = true;

        for (int i = 0; i < helperpoint.Length; i++)
        {
            if (old_helperpoint[i] != helperpoint[i])
            {
                isolddata = false;
                old_helperpoint[i] = helperpoint[i];
            }
        }
    }

    void Sethelperpionts()
    {
        helperpoint[0] = helperpoint0;
        helperpoint[1] = helperpoint1;
        helperpoint[2] = helperpoint2;
        helperpoint[3] = helperpoint3;
        helperpoint[4] = helperpoint4;
        helperpoint[5] = helperpoint5;
        helperpoint[6] = helperpoint6;
        helperpoint[7] = helperpoint7;
    }
    public override Vector3[] ModifyEffect(Vector3[] vectors, Transform m_transform)
    {

        //print("ffd222!");
        Sethelperpionts();

        Setstartpos(vectors);

        Vector3[] temp = new Vector3[vectors.Length];
        for (int i = 0; i < vectors.Length; i++)
        {
            temp[i] = vectors[i];
            //计算每个顶点原本的xyz方向的插值量
            float xlerp = GetTonMeshSize(meshSize, temp[i], Axis.x);
            float ylerp = GetTonMeshSize(meshSize, temp[i], Axis.y);
            float zlerp = GetTonMeshSize(meshSize, temp[i], Axis.z);

            //思路》先找立体-面-再找线-再找点
            //先找X截面值 -

            Vector3 minyminz = Vector3.Lerp(helperpoint[0], helperpoint[4], xlerp);
            Vector3 minymaxz = Vector3.Lerp(helperpoint[1], helperpoint[5], xlerp);
            Vector3 maxymaxz = Vector3.Lerp(helperpoint[2], helperpoint[6], xlerp);
            Vector3 maxyminz = Vector3.Lerp(helperpoint[3], helperpoint[7], xlerp);

            //再找y截面值

            Vector3 minz = Vector3.Lerp(minyminz, maxyminz, ylerp);
            Vector3 maxz = Vector3.Lerp(minymaxz, maxymaxz, ylerp);

            //再找Z截面值

            temp[i] = Vector3.Lerp(minz, maxz, zlerp);

        }

        return temp;
    }



    /// <summary>
    /// 计算模型的大小和设置8个端点的位置
    /// </summary>
    /// <param name="vectors"></param>
    void Setstartpos(Vector3[] vectors)
    {
        if (resetstartpos) { return; }//做个开关只计算一次

        meshSize = GetMeshSize(vectors);
        helperpoint0 = new Vector3(meshSize.min.x, meshSize.min.y, meshSize.min.z);
        helperpoint1 = new Vector3(meshSize.min.x, meshSize.min.y, meshSize.max.z);
        helperpoint2 = new Vector3(meshSize.min.x, meshSize.max.y, meshSize.max.z);
        helperpoint3 = new Vector3(meshSize.min.x, meshSize.max.y, meshSize.min.z);

        helperpoint4 = new Vector3(meshSize.max.x, meshSize.min.y, meshSize.min.z);
        helperpoint5 = new Vector3(meshSize.max.x, meshSize.min.y, meshSize.max.z);
        helperpoint6 = new Vector3(meshSize.max.x, meshSize.max.y, meshSize.max.z);
        helperpoint7 = new Vector3(meshSize.max.x, meshSize.max.y, meshSize.min.z);

        // helperpoint0 = helperpoint[0];
        // helperpoint1 = helperpoint[1];
        // helperpoint2 = helperpoint[2];
        // helperpoint3 = helperpoint[3];
        // helperpoint4 = helperpoint[4];
        // helperpoint5 = helperpoint[5];
        // helperpoint6 = helperpoint[6];
        // helperpoint7 = helperpoint[7];
        resetstartpos = true;
    }

    void OnDrawGizmos()
    {
        if (!ViewGzimos) { return; }

        //画8个端点
        Gizmos.color = gizmospointColor;

        Vector3[] temppoint = new Vector3[helperpoint.Length];

        for (int i = 0; i < temppoint.Length; i++)
        {
            temppoint[i] = helperpoint[i] + transform.position;
            Gizmos.DrawWireCube(temppoint[i], gizmosSize * Vector3.one);
        }


        //画x方向的线
        Gizmos.color = gizmosXaxisColor;
        Gizmos.DrawLine(temppoint[0], temppoint[4]);
        Gizmos.DrawLine(temppoint[1], temppoint[5]);
        Gizmos.DrawLine(temppoint[2], temppoint[6]);
        Gizmos.DrawLine(temppoint[3], temppoint[7]);
        //画y方向的线
        Gizmos.color = gizmosYaxisColor;
        Gizmos.DrawLine(temppoint[0], temppoint[3]);
        Gizmos.DrawLine(temppoint[1], temppoint[2]);
        Gizmos.DrawLine(temppoint[4], temppoint[7]);
        Gizmos.DrawLine(temppoint[5], temppoint[6]);
        //画z方向的线
        Gizmos.color = gizmosZaxisColor;
        Gizmos.DrawLine(temppoint[0], temppoint[1]);
        Gizmos.DrawLine(temppoint[2], temppoint[3]);
        Gizmos.DrawLine(temppoint[4], temppoint[5]);
        Gizmos.DrawLine(temppoint[6], temppoint[7]);

    }

}


