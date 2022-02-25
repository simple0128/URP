using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//沿路径移动
public class MoveOnPath : MonoBehaviour
{

    [Tooltip("路径对象")]
    public Line path;
    [Tooltip("路径变形进度")]
    public float path100;
    [Tooltip("循环")]
    public bool loop;
    [Tooltip("朝向路径前方")]
    public bool lookforward;
    [Tooltip("旋转")]
    public float rotate;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        float t = path100 / 100;
        if (loop)
        {
            t = t % 1;
            if (t < 0) { t += 1; }
        }

        transform.position = path.GetPosOnCuve(t);
        Vector3 forward = path.GetPosOnCuve(t + 0.0001f);
        if (lookforward)
        {
            transform.LookAt(forward);
            transform.Rotate(Vector3.forward * rotate, Space.Self);
        }
    }
}
