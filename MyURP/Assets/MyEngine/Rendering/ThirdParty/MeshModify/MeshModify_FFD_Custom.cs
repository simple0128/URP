using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//模型FFD_NxNxN修改器
public class MeshModify_FFD_Custom : MeshModify
{
    [Tooltip("ffd控制点数量，注意不要给太大的数字")]
    public Vector3Int ffdcount;
    [Tooltip("点击生成辅助点，请在编辑模式下使用")]
    public bool createhelpers = false;

    public bool viewgimzos = true;
    public float Gizmosboxsize = 1f;
    public Color gizmosColor = Color.white;

    bool startset;

    Transform[,,] helperpoint;
    public Transform helpertop;
    public Transform[] helpers;

    Vector3[] helperspos;


    void Start()
    {
        helperspos = new Vector3[ffdcount.x * ffdcount.y * ffdcount.z];
        createhelpers = true;
        NewHelpers();

        helperspos[0]=helpers[0].position+Vector3.one;//强制初始值不一样

    }

    void FFDcoutClamp()
    {
        ffdcount.x = Mathf.Clamp(ffdcount.x, 2, ffdcount.x);
        ffdcount.y = Mathf.Clamp(ffdcount.y, 2, ffdcount.x);
        ffdcount.z = Mathf.Clamp(ffdcount.z, 2, ffdcount.z);
    }

    void NewHelpers()
    {

        if (createhelpers) { return; }
        createhelpers = true;
        FFDcoutClamp();
        int helperpointcount = ffdcount.x * ffdcount.y * ffdcount.z;


        string helpertopname = string.Format("ffd{0}x{1}x{2}", ffdcount.x, ffdcount.y, ffdcount.z);
        if (helpertop == null)
        {

            Transform g = new GameObject().transform;
            g.SetParent(transform);
            g.localPosition = Vector3.zero;
            g.localRotation = Quaternion.identity;
            g.localScale = Vector3.one;
            helpertop = g;
        }
        else
        {

            if (helpertop.childCount > 0)
            {
                Transform[] transforms = new Transform[helpertop.childCount];
                for (int i = 0; i < helpertop.childCount; i++)
                {
                    transforms[i] = helpertop.GetChild(i);
                }

                for (int i = 0; i < transforms.Length; i++)
                {
                    DestroyImmediate(transforms[i].gameObject);
                }
            }
        }
        helpertop.name = helpertopname;

        helperspos = new Vector3[ffdcount.x * ffdcount.y * ffdcount.z];



        helpers = new Transform[helperpointcount];
        helperpoint = new Transform[ffdcount.x, ffdcount.y, ffdcount.z];
        int id = 0;
        for (int i = 0; i < ffdcount.x; i++)
        {
            for (int j = 0; j < ffdcount.y; j++)
            {
                for (int k = 0; k < ffdcount.z; k++)
                {
                    string pointname = helpertopname + "_" + i + "x" + j + "x" + k;
                    Transform h;
                    if (helpers[id] == null)
                    {
                        h = new GameObject().transform;
                        h.SetParent(helpertop);

                        // float t_x = i * 1f / (ffdcount.x - 1);
                        // float t_y = j * 1f / (ffdcount.y - 1);
                        // float t_z = k * 1f / (ffdcount.z - 1);
                        h.localPosition = Vector3.one;
                        h.localRotation = Quaternion.identity;
                        h.localScale = Vector3.one;

                        helpers[id] = h;
                    }
                    helpers[id].name = pointname;
                    helperpoint[i, j, k] = helpers[id];

                    id++;
                }
            }
        }
        print(string.Format("{0} 生成辅助控制点完成！一共生成{1}个辅助控制点", helpertopname, helpertop.childCount));


    }

    public override void GetBoolOldData()
    {
        isolddata = true;
        for (int i = 0; i < helpers.Length; i++)
        {
            // print(""+helperspos);
            if (helpers[i].position != helperspos[i])
            {
                isolddata = false;
                helperspos[i] = helpers[i].position;
            }
        }


    }
    public override Vector3[] ModifyEffect(Vector3[] vectors, Transform m_transform)
    {
        //print("ffd_custom!!");
        MeshSize meshSize = GetMeshSize(vectors);
        StartSet(meshSize);

        Vector3[] temp = new Vector3[vectors.Length];

        Vector3[,] pointyz = new Vector3[ffdcount.y, ffdcount.z];
        Vector3[] xtemp = new Vector3[ffdcount.x];

        Vector3[] pointz = new Vector3[ffdcount.z];
        Vector3[] ytemp = new Vector3[ffdcount.y];

        for (int i = 0; i < temp.Length; i++)
        {
            Vector3 t = GetTonMeshSize(meshSize, vectors[i]);

            //获取X截面的点


            for (int j = 0; j < ffdcount.y; j++)
            {
                for (int k = 0; k < ffdcount.z; k++)
                {

                    for (int l = 0; l < ffdcount.x; l++)
                    {
                        xtemp[l] = helperpoint[l, j, k].localPosition;
                    }
                    pointyz[j, k] = GetPointOnBizerCurve(xtemp, t.x);
                }
            }

            //获取y截面的点

            for (int j = 0; j < ffdcount.z; j++)
            {
                for (int k = 0; k < ffdcount.y; k++)
                {
                    ytemp[k] = pointyz[k, j];

                }
                pointz[j] = GetPointOnBizerCurve(ytemp, t.y);
            }
            //获取z
            temp[i] = GetPointOnBizerCurve(pointz, t.z);


        }
        return temp;
    }

    void StartSet(MeshSize meshSize)
    {

        if (startset) return;

        helperpoint = new Transform[4, 4, 4];
        string helpertopname = string.Format("ffd{0}x{1}x{2}", ffdcount.x, ffdcount.y, ffdcount.z);
        if (helpertop == null)
        {
            Transform g = new GameObject().transform;
            g.SetParent(transform);
            g.localPosition = Vector3.zero;
            g.localRotation = Quaternion.identity;
            g.localScale = Vector3.one;
            helpertop = g;
        }
        helpertop.name = helpertopname;

        int helperpointcount = ffdcount.x * ffdcount.y * ffdcount.z;
        if (helpers.Length != helperpointcount)
        {
            for (int i = 0; i < helpers.Length; i++)
            {
                Destroy(helpers[i].gameObject);
            }
            helpers = new Transform[helperpointcount];
        }
        helperspos = new Vector3[helperpointcount];
        helperpoint = new Transform[ffdcount.x, ffdcount.y, ffdcount.z];

        int id = 0;
        for (int i = 0; i < ffdcount.x; i++)
        {
            for (int j = 0; j < ffdcount.y; j++)
            {
                for (int k = 0; k < ffdcount.z; k++)
                {
                    string pointname = helpertopname + "_" + i + "x" + j + "x" + k;
                    Transform h;
                    if (helpers[id] == null)
                    {
                        h = new GameObject().transform;

                        h.SetParent(helpertop);


                        helpers[id] = h;
                    }

                    helpers[id].name = pointname;
                    float t_x = i * 1f / (ffdcount.x - 1);
                    float t_y = j * 1f / (ffdcount.y - 1);
                    float t_z = k * 1f / (ffdcount.z - 1);
                    helpers[id].localPosition = new Vector3(Mathf.Lerp(meshSize.min.x, meshSize.max.x, t_x), Mathf.Lerp(meshSize.min.y, meshSize.max.y, t_y), Mathf.Lerp(meshSize.min.z, meshSize.max.z, t_z));
                    helpers[id].localRotation = Quaternion.identity;
                    helpers[id].localScale = Vector3.one;
                    helperpoint[i, j, k] = helpers[id];

                    id++;
                }
            }
        }


        startset = true;
    }

    void OnDrawGizmos()
    {
        NewHelpers();

        if (!viewgimzos) return;
        if (helperpoint == null) return;

        //Gizmos画点和线
        Gizmos.color = gizmosColor;

        int x_length = helperpoint.GetLength(0);
        int y_length = helperpoint.GetLength(1);
        int z_length = helperpoint.GetLength(2);
        for (int i = 0; i < x_length; i++)
        {
            for (int j = 0; j < y_length; j++)
            {
                for (int k = 0; k < z_length; k++)
                {
                    if (helperpoint[i, j, k] == null)
                    {
                        break;
                    }

                    Gizmos.DrawWireCube(helperpoint[i, j, k].position, Vector3.one * Gizmosboxsize);

                    if (i < x_length - 1) { Gizmos.DrawLine(helperpoint[i, j, k].position, helperpoint[i + 1, j, k].position); }
                    if (j < y_length - 1) { Gizmos.DrawLine(helperpoint[i, j, k].position, helperpoint[i, j + 1, k].position); }
                    if (k < z_length - 1) { Gizmos.DrawLine(helperpoint[i, j, k].position, helperpoint[i, j, k + 1].position); }

                }
            }
        }

    }



}
