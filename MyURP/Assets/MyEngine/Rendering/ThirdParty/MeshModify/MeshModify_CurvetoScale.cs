using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//模型曲线缩放修改器
public class MeshModify_CurvetoScale : MeshModify
{
    public CurvetoEffect curvetoEffect;
    CurvetoEffect old_curvetoeffect;
    // Use this for initialization
    void Start()
    {
        if (curvetoEffect.curve.keys.Length == 0)
        {
            curvetoEffect.curve = new AnimationCurve(new Keyframe[2] { new Keyframe(0, 0), new Keyframe(1, 1) });
        }

        if (curvetoEffect.curve_mul.keys.Length == 0)
        {
            curvetoEffect.curve_mul = new AnimationCurve(new Keyframe[2] { new Keyframe(0, 0), new Keyframe(1, 1) });
        }

        old_curvetoeffect.usemul = !curvetoEffect.usemul;//强制初始值不一样
    }




    public override Vector3[] ModifyEffect(Vector3[] vectors, Transform m_transform)
    {
        // print("curvescale!");

        if (curvetoEffect.strength == 0) { return vectors; }
        MeshSize meshSize = GetMeshSize(vectors);
        Vector3[] temp = new Vector3[vectors.Length];
        float t;
        for (int i = 0; i < vectors.Length; i++)
        {
            temp[i] = vectors[i];

            t = GetTonMeshSize(meshSize, temp[i], curvetoEffect.axis);
            temp[i] = curvetoEffect.ScaleVector(temp[i], t, true);

        }



        return temp;
    }


    public override void GetBoolOldData()
    {
        if (curvetoEffect.anioffset)
        {
            isolddata = false;
            return;
        }

        if (old_curvetoeffect.strength != curvetoEffect.strength
             || old_curvetoeffect.usemul != curvetoEffect.usemul
             || old_curvetoeffect.axis != curvetoEffect.axis
             || old_curvetoeffect.x != curvetoEffect.x
             || old_curvetoeffect.y != curvetoEffect.y
             || old_curvetoeffect.z != curvetoEffect.z
             || old_curvetoeffect.offset != curvetoEffect.offset)
        {

            isolddata = false;

            old_curvetoeffect.strength = curvetoEffect.strength;
            old_curvetoeffect.usemul = curvetoEffect.usemul;
            old_curvetoeffect.axis = curvetoEffect.axis;
            old_curvetoeffect.x = curvetoEffect.x;
            old_curvetoeffect.y = curvetoEffect.y;
            old_curvetoeffect.z = curvetoEffect.z;
            old_curvetoeffect.offset = curvetoEffect.offset;

        }
        else
        {
            isolddata = true;
        }

    }

}


