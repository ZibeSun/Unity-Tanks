using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float m_DampTime = 0.2f;                 
    public float m_ScreenEdgeBuffer = 4f;               //设置相机画幅边缘高度大小为4；
    public float m_MinSize = 6.5f;                      //设置相机最小画幅高度大小为6.5；
    [HideInInspector] public Transform[] m_Targets;     //声明一个摄像目标Transform类对象数组，用于储存摄像目标的位置；
    //[HideInInspector]：使变量不显示在 Inspector 中；

    private Camera m_Camera;                        //声明一个摄像机类对象 m_Camera；
    private float m_ZoomSpeed;                      
    private Vector3 m_MoveVelocity;                 
    private Vector3 m_DesiredPosition;              //相机移动的目标位置

    //脚本加载的时候执行的代码
    private void Awake()
    {
        m_Camera = GetComponentInChildren<Camera>();    //将相机组件返回给 m_Camera；
    }

    //固定的刷新率 0.02s 执行一次，主要用来执行物理模拟的脚本
    private void FixedUpdate()
    {
        Move();         //移动相机位置；
        Zoom();         //调整正交相机画幅大小；
    }

    //摄像机移动函数
    private void Move()
    {
        FindAveragePosition();

        //ref表示按引用传递值

        //Vector3 Vector3.SmoothDamp (Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime, float maxSpeed= Mathf.Infinity, float deltaTime= Time.deltaTime);
        //Vector3.SmoothDamp 可随时间推移将一个向量逐渐改变为所需目标。
        /*
          current	         当前位置。
          target             尝试达到的目标。
          currentVelocity    当前速度，此值由函数在每次调用时进行修改。
          smoothTime         达到目标所需的近似时间。值越小，达到目标的速度越快。
          maxSpeed           可以选择允许限制最大速度。
          deltaTime          自上次调用此函数以来的时间。默认情况下为 Time.deltaTime。
        */
        transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
        //使用Vector3.SmoothDamp来平滑移动相机到合适位置；
    }

    //计算相机移动的目标位置坐标
    private void FindAveragePosition()
    {
        Vector3 averagePos = new Vector3();
        int numTargets = 0;                             //摄像目标数量初始化为0；

        //xxx.Length可以返回对象数组xxx的对象数量；
        for (int i = 0; i < m_Targets.Length; i++)
        {
            //gameObject.activeSelf：返回此 GameObject 的本地活动状态
            if (!m_Targets[i].gameObject.activeSelf)    //判断m_Targets[i]是否被激活，若m_Targets[i]被停用，则结束本次循环，i++，执行下一次循环；
                continue;

            //若m_Targets[i]激活
            averagePos += m_Targets[i].position;        //将m_Targets[i]的位置矢量加给averagePos；
            numTargets++;                               //摄像目标数量+1；
        }

        if (numTargets > 0)
            averagePos /= numTargets;                  //若摄像目标数量>0，则计算m_Targets的平均位置矢量并赋给averagePos；

        averagePos.y = transform.position.y;           //将相机的y轴坐标赋给averagePos的y轴坐标；

        m_DesiredPosition = averagePos;                //将averagePos的矢量坐标赋给相机移动的目标位置；
    }

    //相机画幅调整函数
    private void Zoom()
    {
        float requiredSize = FindRequiredSize();

        //Camera.orthographicSize 的值是正交摄像机高度的一半。正交摄像机的大小取决于宽高比。

        //float Mathf.SmoothDamp (float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed= Mathf.Infinity, float deltaTime= Time.deltaTime);
        //Mathf.SmoothDamp 可随时间推移将一个值逐渐改变为所需目标。
        /*
          current	        当前位置。
          target	        尝试达到的目标。
          currentVelocity	当前速度，此值由函数在每次调用时进行修改。
          smoothTime	    达到目标所需的近似时间。值越小，达到目标的速度越快。
          maxSpeed	        可以选择允许限制最大速度。
          deltaTime	        自上次调用此函数以来的时间。默认情况下为 Time.deltaTime。
        */
        m_Camera.orthographicSize = Mathf.SmoothDamp(m_Camera.orthographicSize, requiredSize, ref m_ZoomSpeed, m_DampTime);
        //使用Mathf.SmoothDamp来平滑缩放正交相机画幅到合适大小；
    }

    //计算相机画幅的合适大小；
    private float FindRequiredSize()
    {
        //Vector3 Transform.InverseTransformPoint (Vector3 position)：将 position 从世界坐标变换到本地坐标。
        Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition);   //获取相机移动目标位置的本地坐标；

        float size = 0f;

        for (int i = 0; i < m_Targets.Length; i++)
        {
            if (!m_Targets[i].gameObject.activeSelf)       //判断m_Targets[i]是否被激活；
                continue;

            Vector3 targetLocalPos = transform.InverseTransformPoint(m_Targets[i].position);   //获取摄像目标的本地坐标；

            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;      //计算摄像目标的本地坐标和相机移动目标位置的本地坐标的坐标差；

            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.y));         //防止有坦克跑到相机画面高度之外；

            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.x) / m_Camera.aspect);      //防止有坦克跑到相机画面宽度之外；
            //Camera.aspect：画幅宽高比（宽度除以高度）。
        }

        size += m_ScreenEdgeBuffer;            //加上画幅边缘高度；

        size = Mathf.Max(size, m_MinSize);     //使相机画幅大小不小于所设置的最小画幅大小；

        return size;
    }

    //每一轮开始时初始化相机的位置以及画幅大小；
    public void SetStartPositionAndSize()
    {
        FindAveragePosition();

        transform.position = m_DesiredPosition;

        m_Camera.orthographicSize = FindRequiredSize();
    }
}