using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//模型曲线偏移修改器
public class MeshModify_CurvetoOffset : MeshModify
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
         //print("curveoffset!");
        if (curvetoEffect.strength == 0) { return vectors; }
        MeshSize meshSize = GetMeshSize(vectors);
        Vector3[] temp = new Vector3[vectors.Length];
        float t;
        for (int i = 0; i < vectors.Length; i++)
        {
            temp[i] = vectors[i];

            t = GetTonMeshSize(meshSize, temp[i], curvetoEffect.axis);
            temp[i] = curvetoEffect.ScaleVector(temp[i], t,false);

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

[System.Serializable]
public struct CurvetoEffect
{
    [Tooltip("强度")]
    [Range(0, 1f)]
    public float strength;
    [Tooltip("曲线")]
    public AnimationCurve curve;
    [Tooltip("开启强度曲线")]
    public bool usemul;
    [Tooltip("强度曲线")]
    public AnimationCurve curve_mul;
    [Tooltip("T值轴向")]
    public Axis axis;
    [Tooltip("效果作用于：")]
    public bool x, y, z;
    [Tooltip("t值偏移")]
    public float offset;
    [Tooltip("t值偏移循环")]
    public bool offsetloop;
    [Tooltip("动态偏移")]
    public bool anioffset;
    [Tooltip("动态偏移速度")]
    public float anioffsetspeed;
    float ani_t;

    /// <summary>
    /// 根据曲线缩放或偏移顶点
    /// </summary>
    /// <param name="v"></param>
    /// <param name="t"></param>
    /// <param name="mode_scale">是：缩放，否：偏移</param>
    /// <returns></returns>
    public Vector3 ScaleVector(Vector3 v, float t ,bool mode_scale)
    {
        float mul = 1;
        if (usemul) { mul = curve_mul.Evaluate(t); }

        if (anioffset)
        {
            ani_t += Time.deltaTime * anioffsetspeed * 0.001f;
            if (ani_t > 1)
            {
                ani_t = 0;
            }
        }
        t = t + offset + ani_t;
        if (offsetloop)
        {
            if (t > 1)
            {
                t = t % 1;
            }
            if (t < 0)
            {
                t = t % 1 + 1;
            }
        }


        float scale = curve.Evaluate(t) * strength * mul;
        if (mode_scale)
        {
            if (x) { v.x *= scale; }
            if (y) { v.y *= scale; }
            if (z) { v.z *= scale; }
        }
        else
        {
            if (x) { v.x += scale; }
            if (y) { v.y += scale; }
            if (z) { v.z += scale; }
        }


        return v;
    }

}


