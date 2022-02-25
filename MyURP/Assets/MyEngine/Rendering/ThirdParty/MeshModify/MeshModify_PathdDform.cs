using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// 路径变形修改器
public class MeshModify_PathdDform : MeshModify
{
    [Tooltip("辅助对象")]
    public Transform helper;
    [Tooltip("摄像机，不指定默认为主摄像机")]
    public Camera _camera;
    [Tooltip("开启在摄像机前方显示")]
    public bool followcameraforward;

    [Tooltip("路径对象")]
    public Line path;
    [Tooltip("使用轴")]
    public Axis useaxis = Axis.z;
    [Tooltip("路径变形进度")]
    public float pathform100;

    [Tooltip("是否循环")]
    public bool loop;
    [Tooltip("缩放模型在路径上的长度")]
    public float scale = 1;
    [Tooltip("旋转")]
    public float rotate;
    [Tooltip("扭曲")]
    public float twist;

    [Tooltip("开启模型在路径上的半径值受曲线控制")]
    public bool radiubycurve;
    [Tooltip("模型在路径上的半径值曲线")]
    public AnimationCurve radiucurve;
    [Tooltip("挂点插槽")]
    public EffectObj[] effectObjs;


    //判断是否为旧数据所需的变量
    Axis old_useaxis = Axis.z;
    float old_pathform100;
    bool old_loop;
    float old_scale = 1;
    float old_rotate;
    float old_twist;
    bool old_radiubycurve;
    //判断是否为旧数据所需的变量

    void Start()
    {
        if (_camera == null) { _camera = Camera.main; }
        if(radiucurve.keys.Length==0){radiucurve= new AnimationCurve(new Keyframe[2] { new Keyframe(0, 0), new Keyframe(1, 1) });}

        old_loop = !loop;//强制初始值不一样
    }

    public override void GetBoolOldData()
    {

        if (old_useaxis != useaxis
            || old_pathform100 != pathform100
            || old_loop != loop
            || old_scale != scale
            || old_rotate != rotate
            || old_twist != twist
            || old_radiubycurve != radiubycurve)
        {

            isolddata = false;
            old_useaxis = useaxis;
            old_pathform100 = pathform100;
            old_loop = loop;
            old_scale = scale;
            old_rotate = rotate;
            old_twist = twist;
            old_radiubycurve = radiubycurve;


        }
        else
        {
            isolddata = true;
        }

    }


    public override Vector3[] ModifyEffect(Vector3[] vectors, Transform m_transform)
    {
       // print("pathdform!");

        if (path == null) { return vectors; }
        GetHelper();
        UpdateEffectObjPos();

        m_transform.rotation = Quaternion.identity;

        if (followcameraforward) { m_transform.position = _camera.transform.position + _camera.transform.forward * 10f; }//设置位置保持摄像机前方范围内

        Vector3[] temp = new Vector3[vectors.Length];
        for (int i = 0; i < vectors.Length; i++)
        {
            Vector3 v = new Vector3();
            //切换轴向
            switch (useaxis)
            {
                case Axis.x:
                    v.z = vectors[i].x;
                    v.x = vectors[i].y;
                    v.y = vectors[i].z;
                    break;
                case Axis.y:
                    v.z = vectors[i].y;
                    v.x = vectors[i].z;
                    v.y = vectors[i].x;
                    break;
                case Axis.z:
                    v = vectors[i];
                    break;
            }

            float t = pathform100 / 100 + v.z / path.curveData.linelength * scale;
            if (loop)
            {
                t %= 1;
                if (t < 0) { t += 1; }

            }
            helper.position = path.GetPosOnCuve(t);
            Vector3 forward = path.GetPosOnCuve(t + 0.0001f);
            helper.LookAt(forward);

            float angle = rotate + Mathf.Lerp(0, twist, t);
            helper.Rotate(0, 0, angle, Space.Self);
            float x_mul = Mathf.Sign(scale);

            float radiu_mul = 1;
            if (radiubycurve) { radiu_mul = radiucurve.Evaluate(t); }
            temp[i] = helper.position + (helper.right * v.x * x_mul + helper.up * v.y) * radiu_mul - m_transform.position;


        }


        return temp;
    }

    void GetHelper()
    {
        if (helper == null)
        {
            helper = new GameObject("pathform_helper").transform;
            helper.SetParent(transform);
        }
    }


    //更新效果挂点在路径上的位置
    void UpdateEffectObjPos()
    {
        foreach (EffectObj j in effectObjs)
        {
            float t = (float)pathform100 / 100;
            if (loop)
            {
                t = t % 1;
            }
            t += j.offset_t;
            Vector3 forwad = path.GetPosForward(t);
            j.transform.position = path.GetPosOnCuve(t);
            j.transform.rotation = Quaternion.LookRotation(forwad);
            j.transform.position += j.transform.forward * j.offsetpos.z + j.transform.up * j.offsetpos.y + j.transform.right * j.offsetpos.x;
            j.transform.Rotate(Vector3.forward * j.rotation, Space.Self);

            if (j.scalebycurve)
            {
                j.transform.localScale = Vector3.one * radiucurve.Evaluate(t);
            }
        }
    }
}




[System.Serializable]
public struct EffectObj
{
    [Tooltip("效果对象")]
    public Transform transform;
    [Tooltip("在路径变形进度上的偏移")]
    public float offset_t;
    [Tooltip("旋转")]
    public float rotation;
    [Tooltip("位置偏移")]
    public Vector3 offsetpos;
    [Tooltip("是否根据曲线进行缩放")]
    public bool scalebycurve;
}

