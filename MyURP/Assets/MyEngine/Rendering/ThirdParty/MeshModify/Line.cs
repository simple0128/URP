using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//创建路径线
public class Line : MonoBehaviour
{
    public CurveData curveData;//曲线数据

    [Tooltip("在Updata中更新数据")]
    public bool updateCurve;//在Updata中更新数据，可以对曲线的形态做动画

    [Tooltip("使用Gizmos方法更新数据")]
    public bool gizmosUpdate = true;//使用Gizmos方法更新数据，可以在不运行的情况下编辑曲线

    [Tooltip("显示/关闭辅助线")]
    public bool viewGizmos = true;

    [Tooltip("显示/关闭辅助点对象")]
    public bool viewHelps;

    [Tooltip("辅助线显示段数")]
    [Range(0, 500)]
    public int gizmoslinecount = 100;
    [Tooltip("辅助线颜色")]
    public Color gizmoscolor = Color.green;

    Transform[] childs;
    Vector3[] childspoints;
    // Use this for initialization
    void Start()
    {

    }


    void Update()
    {
        if (updateCurve) { UpdateCurve(); }

    }


    //更新曲线数据
    void UpdateCurve()
    {

        childs = new Transform[transform.childCount];
        childspoints = new Vector3[transform.childCount];


        for (int i = 0; i < childspoints.Length; i++)
        {
            childs[i] = transform.GetChild(i);
            childspoints[i] = childs[i].position;

        }

        curveData.SetCurve(childspoints);

    }


    /// <summary>
    /// 获取曲线上的值
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public Vector3 GetPosOnCuve(float t)
    {
        Vector3 vector = curveData.GetCurvePoint(t);
        return vector;
       // Vector3 scale = transform.localScale;
       // return transform.position + transform.right * vector.x * scale.x + transform.up * vector.y * scale.y + transform.forward * vector.z * scale.z;
    }

    public Vector3 GetPosForward(float t)
    {
        t = Mathf.Clamp(t, 0, 0.9999f);
        return (GetPosOnCuve(t + 0.0001f) - GetPosOnCuve(t)).normalized;
    }
    //显示、隐藏辅助点对象
    void ViewHelps()
    {
        if (viewHelps)
        {
            foreach (Transform t in childs)
            {
                t.gameObject.SetActive(!t.gameObject.activeSelf);
            }

            viewHelps = false;
        }
    }

    void OnDrawGizmos()
    {

        ViewHelps();

        if (!viewGizmos) { return; }

        if (gizmosUpdate) { UpdateCurve(); }



        Vector3[] gizmospoint = new Vector3[gizmoslinecount];
        for (int i = 0; i < gizmoslinecount; i++)
        {
            float t = i / (gizmoslinecount - 1f);
            gizmospoint[i] = GetPosOnCuve(t);
        }

        //绘制曲线
        Gizmos.color = gizmoscolor;
        for (int i = 0; i < gizmospoint.Length - 1; i++)
        {
            Gizmos.DrawLine(gizmospoint[i], gizmospoint[i + 1]);
        }
    }
}


[System.Serializable]
public class CurveData
{

    public AnimationCurve x=new AnimationCurve(new Keyframe[2] { new Keyframe(0, 0), new Keyframe(1, 1) });
    public AnimationCurve y=new AnimationCurve(new Keyframe[2] { new Keyframe(0, 0), new Keyframe(1, 1) });
    public AnimationCurve z=new AnimationCurve(new Keyframe[2] { new Keyframe(0, 0), new Keyframe(1, 1) });


    public float linelength;//线条总长度
    float[] pointlenth;//每个点的长度值用于计算t值


    /// <summary>
    /// 传入数据设置曲线数据
    /// </summary>
    /// <param name="vectors">坐标数组</param>
    public void SetCurve(Vector3[] vectors)
    {
        if (vectors.Length < 2) { return; }

        linelength = 0;
        pointlenth = new float[vectors.Length];
        pointlenth[0] = 0;
        for (int i = 1; i < vectors.Length; i++)
        {
            linelength += Vector3.Distance(vectors[i - 1], vectors[i]);
            pointlenth[i] = linelength;
        }

        x = new AnimationCurve();
        y = new AnimationCurve();
        z = new AnimationCurve();
        for (int i = 0; i < vectors.Length; i++)
        {
            float t = pointlenth[i] / linelength;
            x.AddKey(t, vectors[i].x);
            y.AddKey(t, vectors[i].y);
            z.AddKey(t, vectors[i].z);
        }

    }

    /// <summary>
    /// 获取曲线上的数据
    /// </summary>
    /// <param name="t">传入t值</param>
    /// <returns></returns>
    public Vector3 GetCurvePoint(float t)
    {
        return new Vector3(x.Evaluate(t), y.Evaluate(t), z.Evaluate(t));
    }

}

