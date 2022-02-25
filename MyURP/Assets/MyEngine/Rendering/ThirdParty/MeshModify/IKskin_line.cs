using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKskin_line : MonoBehaviour
{

    [Tooltip("使用路径")]
    public Line path;

    [Tooltip("路径变形进度")]
    public float path100;

    [Tooltip("路径循环")]
    public bool loop;
    [Tooltip("缩放长度")]
    public float scale = 1f;

    [Tooltip("旋转设置")]
    public Vector3 rot;

    [Tooltip("指定骨骼")]
    public Transform[] bones;

    float[] bones_z;//保存骨骼z方向的距离
    
    void Start()
    {
        ComputeBonesData();
    }

   
    void Update()
    {
        EffectToPath();
    }



    void EffectToPath()
    {
        if (path == null) { return; }
        for (int i = 0; i < bones.Length; i++)
        {

            float t = path100 / 100 + bones_z[i] * scale / path.curveData.linelength;

            Vector3 pos = path.GetPosOnCuve(t);
            Vector3 posforwad = path.GetPosOnCuve(t + 0.0001f);
            if (loop)
            {
                if (t >= 1)
                {
                    t = t % 1;
                }
                else if (t < 0)
                {
                    t = t % 1 + 1;
                }
                pos = path.GetPosOnCuve(t);
                posforwad = path.GetPosOnCuve(t + 0.0001f);
            }
            else
            {

                if (t >= 1)
                {
                    pos += path.GetPosForward(t) * (t - 1) * path.curveData.linelength;
                    posforwad = pos + path.GetPosForward(t) * (t) * path.curveData.linelength;

                }
                else if (t < 0)
                {
                    pos += path.GetPosForward(t) * (t) * path.curveData.linelength;

                }
            }

            bones[i].position = pos;
            bones[i].LookAt(posforwad);
            bones[i].Rotate(rot, Space.Self);

        }
    }

    //计算骨骼信息
    void ComputeBonesData()
    {
        if (bones.Length < 1) { return; }
        bones_z = new float[bones.Length];
        for (int i = 1; i < bones.Length; i++)
        {
            bones_z[i] = bones_z[i - 1] + Vector3.Distance(bones[i - 1].position, bones[i].position);
        }
    }

}





