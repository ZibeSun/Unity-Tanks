using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask m_TankMask;                      //声明一个公共的层 m_TankMask;
    public ParticleSystem m_ExplosionParticles;       //声明一个公共粒子系统组件 m_ExplosionParticles
    public AudioSource m_ExplosionAudio;               
    public float m_MaxDamage = 100f;                  //设置炮弹最大伤害值为100；
    public float m_ExplosionForce = 1000f;            //设置爆炸力为1000；
    public float m_MaxLifeTime = 2f;                  //
    public float m_ExplosionRadius = 5f;              //设置炮弹爆炸半径为5

    //场景加载好，开始的时候执行一次 （用来初始化数据），一定会在Update系列函数前面
    private void Start()
    {
        Destroy(gameObject, m_MaxLifeTime);      
    }

    //当炮弹击中坦克时运行的代码
    private void OnTriggerEnter(Collider other)
    {
        //Find all the tanks in an area around the shell and damage them.
        //Physics.OverlapSphere()：计算并存储接触球体或位于球体内部的碰撞体。返回一个Collider[] 数组，其中包含与球体接触或位于球体内部的所有碰撞体。
        //Collider[] OverlapSphere (Vector3 position, float radius, int layerMask= AllLayers, QueryTriggerInteraction queryTriggerInteraction= QueryTriggerInteraction.UseGlobal);
        /*
            position	                球体的中心。
            radius	                    球体的半径。
            layerMask	                定义要在查询中包括哪些碰撞体层。
            queryTriggerInteraction	    指定该查询是否应该命中触发器。
         */
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);
        //创建Collider对象数组colliders接收炮弹爆炸范围的所有碰撞体；
        for(int i=0;i<colliders.Length;i++)            
        {
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();
            //创建一个刚体对象targetRigidbody接收colliders[i]的刚体组件；
            if (!targetRigidbody)      
                continue;               //若刚体对象targetRigidbody不存在，则跳过本次循环；

            //Rigidbody.AddExplosionForce (float explosionForce, Vector3 explosionPosition, float explosionRadius, float upwardsModifier= 0.0f, ForceMode mode= ForceMode.Force));
            /*            
                explosionForce	            爆炸力（可以根据距离进行修改）。
                explosionPosition	        表示爆炸波及范围的球体的中心。
                explosionRadius	            表示爆炸波及范围的球体的半径。
                upwardsModifier	            调整爆炸的视位，呈现掀起物体的效果。
                mode	                    用于将力施加到其目标的方法。
             */
            //AddExplosionForce：向模拟爆炸效果的刚体施加力。
            targetRigidbody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);

            TankHealth targetHealth=targetRigidbody.GetComponent< TankHealth > ();
            //创建一个TankHealth类对象targetHealth接收爆炸范围内坦克的TankHealth；
            if (!targetHealth)          
                continue;                 //若TankHealth类对象targetHealth不存在，则跳过本次循环；

            float damage = CalculateDamage(targetRigidbody.position);   //计算坦克所受伤害值；

            targetHealth.TakeDamage(damage);       //更新坦克的血量数值以及血量显示；
        }

        m_ExplosionParticles.transform.parent = null;         
        //将粒子组件所依附的父级对象设置为空，即将粒子组件与其父级炮弹对象分离开来，使粒子组件成为一个层级对象；
        //这样做以实现炮弹被移除时粒子效果和爆炸音效可以继续播放；
        m_ExplosionParticles.Play();         //启动粒子系统；
        m_ExplosionAudio.Play();             //播放爆炸音效；

        Destroy(m_ExplosionParticles.gameObject, m_ExplosionParticles.main.duration);   //移除粒子组件所依附的对象；
        Destroy(gameObject);                  //移除炮弹对象；
    }

    //计算坦克所受伤害值；
    private float CalculateDamage(Vector3 targetPosition)
    {
        // Calculate the amount of damage a target should take based on it's position.
        Vector3 explosionToTarget = targetPosition - transform.position;
        //计算炮弹爆炸位置与坦克的位置矢量差；
        float explosionDistance = explosionToTarget.magnitude;
        //Vector3.magnitude：返回该向量的长度(x*x+y*y+z*z)。
        float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;
        //计算炮弹爆炸位置与坦克的距离占爆炸半径的百分比；
        float damage = relativeDistance * m_MaxDamage;
        //计算伤害值；
        damage = Mathf.Max(0f, damage);
        //若算的伤害值小于0，则坦克位置在爆炸范围之外，坦克所受伤害值为0
        //否则返回伤害值；
        return damage;
    }
}