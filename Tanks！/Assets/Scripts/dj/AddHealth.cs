using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using UnityEngine;

public class AddHealth : MonoBehaviour
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

    void OnTriggerEnter(Collider other)           //OnTriggerEnter的输入参数为碰撞对象的 Collider 组件；
    {
        if (other.gameObject.CompareTag("Player")) //判断碰撞对象是否使用了"addhealth"标记；
        {
            
            TankHealth targetHealth = other.gameObject.GetComponent<TankHealth>();
            targetHealth.TakeHealth(50);
            gameObject.SetActive(false);    
        }
    }
}
