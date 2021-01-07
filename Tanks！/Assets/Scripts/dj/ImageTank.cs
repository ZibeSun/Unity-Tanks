using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageTank : MonoBehaviour
{
    public float WaitTime = 5f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        WaitTime -= Time.deltaTime;
        transform.Rotate(new Vector3(15, 45, 30) * Time.deltaTime);  //使用transform.Rotate来旋转方块；
        //Time.deltaTime为增量时间，即完成上一帧所用的时间（以秒为单位）；
        //为了使方块的旋转更加平滑，并且与运行时的帧率无关，用Time.deltaTime去乘以transform.Rotate的输入参数Vector3(15, 45, 30)，这样就可以使方块每秒旋转的角度都相同，从而实现我们的目标；

        if (WaitTime <= 0)
        {
            WaitTime = 5f;
            gameObject.SetActive(false);
        }
    }
}
