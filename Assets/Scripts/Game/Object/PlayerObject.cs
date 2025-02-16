using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObject : RoleObject
{
    //玩家跳跃初速度
    public float jumpSpeed = 10;
    //重力加速度
    public float gSpeed = 9.8f;
    //当前的Y轴速度
    private float nowYSpeed;
    //击退速度
    private float nowXSpeed;
    //攻击连招计数
    private int atkCount = 0;
    //腿部攻击连招计数
    private int footAtkCount = 0;

    protected override void Awake()
    {
        base.Awake();

        //开启输入控制
        InputMgr.GetInstance().StartOrEndCheck(true);
        //获取输入权限
        GetController();
    }

    protected override void Update()
    {
        base.Update();

        //处理 跳跃逻辑
        //不是在地面上 就可以跳跃
        if(roleAnimator.GetBool("isGround") == false)
        {
            
            //跳跃身体对象
            bodyTransform.Translate(Vector2.up * nowYSpeed * Time.deltaTime);
            //竖直上抛 下落逻辑 速度变化
            //v = v - gt
            nowYSpeed -= gSpeed * Time.deltaTime;

            //我们判断高度是否小于等于0 即可判断是否落地
            //注意 一定不是判断 == 0 因为是 - 帧间隔时间*速度 大部分情况都不会刚刚等于0
            //注意使用的是localPosition 了解Position和localPosition
            if (bodyTransform.localPosition.y <= 0)
            {
                //放置到地面
                bodyTransform.localPosition = Vector2.zero;
                //改变地面标识
                roleAnimator.SetBool("isGround", true);

                //落地后 不管击退多厉害 都得停下来
                nowXSpeed = 0;
                //应该让他延迟站起来
                Invoke(nameof(DelayClearHitFly), .2f);
            }
        }

        if (nowXSpeed != 0)
        {
            this.transform.Translate(Vector2.right * nowXSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// 受伤方法
    /// </summary>
    public void Wound(float hitTime)
    {
        //如果处于受伤状态 又受伤 需要把上一次延时函数取消
        CancelInvoke(nameof(DelayClearHit));
        roleAnimator.SetBool("isHit", true);
        //延时函数 处理过一段时间结束受伤状态
        Invoke(nameof(DelayClearHit), hitTime);
    }

    /// <summary>
    /// 击飞方法
    /// </summary>
    public void HitFly(float xSpeed, float ySpeed)
    {

        roleAnimator.SetBool("isHitFly", true);
        //改变玩家不在地面
        roleAnimator.SetBool("isGround", false);
        //初始竖直上抛的速度
        nowYSpeed = ySpeed;
        nowXSpeed = xSpeed;
    }

    private void DelayClearHit()
    {
        roleAnimator.SetBool("isHit", false);
    }

    private void Jump()
    {
        //在地面是true 才来进行跳跃
        //之后再添加跳跃的约束条件
        if (roleAnimator.GetBool("isGround") &&
            roleAnimator.GetBool("isDefend") == false &&
            roleAnimator.GetBool("isHit") == false &&
            IsAtkState == false)
        {
            nowYSpeed = jumpSpeed;
            //切换动作
            roleAnimator.SetTrigger("jumpTrigger");
            //切换在地面的状态
            roleAnimator.SetBool("isGround", false);
        }
    }

    /// <summary>
    /// 跳跃攻击
    /// </summary>
    private void JumpAtk()
    {
        //如果当前处于跳跃攻击状态 就不要再触发跳跃攻击
        if (roleAnimator.GetCurrentAnimatorStateInfo(1).IsName("JumpAtk") == false)
        {
            roleAnimator.SetTrigger("jumpAtkTrigger");
        }
    }

    /// <summary>
    /// 手部攻击
    /// </summary>
    private void Atk()
    {
        //判断当前能不能执行攻击行为
        //这里判断有几种方式
        //1 根据状态判断 
        //2 根据条件判断
        //不满足攻击的条件直接返回
        if (roleAnimator.GetBool("isDefend"))
            return;

        //为了让其真正的计时点击连招 使用两个条件来控制
        //目的是避免点击事件间隔和动画时间过短导致的重复播放问题

        //方式1 触发条件去处理
        roleAnimator.SetTrigger("atk1Trigger");

        //方式2 使用int累加来处理
        //让计数清零
        //1 延迟函数
        //2 自己写计时逻辑(自己实现 延时逻辑 延时管理器)
        //3 通过协同程序去计时

        //每次按键 取消上一次延迟函数
        CancelInvoke(nameof(DelayClearAtkCount));
        //手部攻击连招计数
        atkCount++;
        roleAnimator.SetInteger("atkCount", atkCount);
        //连招计数延迟清零
        Invoke(nameof(DelayClearAtkCount), .3f);
    }

    /// <summary>
    /// 腿部攻击 参考手部攻击
    /// </summary>
    private void FootAtk()
    {
        if (roleAnimator.GetBool("isDefend"))
            return;

        roleAnimator.SetTrigger("footAtkTrigger");

        CancelInvoke(nameof(DelayClearFootAtkCount));
        footAtkCount++;
        roleAnimator.SetInteger("footAtkCount", footAtkCount);
        Invoke(nameof(DelayClearFootAtkCount), .3f);
    }

    /// <summary>
    /// 是否格挡
    /// </summary>
    /// <param name="isDefend"></param>
    private void Defend(bool isDefend)
    {
        roleAnimator.SetBool("isDefend", isDefend);
    }

    private void DelayClearFootAtkCount()
    {
        footAtkCount = 0;
        roleAnimator.SetInteger("footAtkCount", footAtkCount);
    }

    private void DelayClearAtkCount()
    {
        atkCount = 0;
        roleAnimator.SetInteger("atkCount", atkCount);
    }

    /// <summary>
    /// 延迟起身
    /// </summary>
    private void DelayClearHitFly()
    {
        roleAnimator.SetBool("isHitFly", false);
    }

    /// <summary>
    /// 给予控制权
    /// </summary>
    public void GetController()
    {
        //事件中心有加就有减 
        //一定不要往里往里面传lambda表达式 一定是在下方去声明一个函数
        EventCenter.GetInstance().AddEventListener<float>("Horizontal", CheckX);
        EventCenter.GetInstance().AddEventListener<float>("Vertical", CheckY);

        //监听按键按下
        EventCenter.GetInstance().AddEventListener<KeyCode>("SomeKeyDown", CheckKeyDown);
        //监听按键抬起
        EventCenter.GetInstance().AddEventListener<KeyCode>("SomeKeyUp", CheckKeyUp);
    }

    /// <summary>
    /// 剥夺控制权
    /// </summary>
    public void RemoveController()
    {
        //事件中心有加就有减 
        //一定不要往里往里面传lambda表达式 一定是在下方去声明一个函数
        EventCenter.GetInstance().RemoveEventListener<float>("Horizontal", CheckX);
        EventCenter.GetInstance().RemoveEventListener<float>("Vertical", CheckY);
        //监听按键按下
        EventCenter.GetInstance().RemoveEventListener<KeyCode>("SomeKeyDown", CheckKeyDown);
        EventCenter.GetInstance().RemoveEventListener<KeyCode>("SomeKeyUp", CheckKeyUp);
    }

    private void CheckX(float x)
    {
        //x 就会是 -1 0 1三个值的数
        //按A为-1 不按为0 按D为1
        //获取横向移动输入
        moveDir.x = x;
    }

    private void CheckY(float y)
    {
        moveDir.y = y;
    }

    /// <summary>
    /// 检测玩家 除移动以外的输入
    /// </summary>
    /// <param name="key"></param>
    private void CheckKeyDown(KeyCode key)
    {
        switch(key)
        {
            case KeyCode.J:
                //手部攻击
                //如果不在地面 处理跳跃攻击逻辑
                if (roleAnimator.GetBool("isGround") == false)
                    JumpAtk();
                //在地面上 能够攻击
                //以后如果还有限制条件再加
                else
                    Atk();
                break;
            case KeyCode.K:
                //腿部攻击
                if (roleAnimator.GetBool("isGround") == false)
                    JumpAtk();
                else
                    FootAtk();
                break;
            case KeyCode.L:
                //格挡
                Defend(true);
                break;
            case KeyCode.Space:
                Debug.Log("Space");
                Jump();
                break;
            //测试按键
            case KeyCode.B:
                //Wound(.2f);
                //Wound(1f);
                HitFly(-10f,10f);
                break;
        }
    }

    private void CheckKeyUp(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.L:
                Defend(false);
                break;
        }
    }


    private void OnDestroy()
    {
        //事件 有加就有减 移除时一定要注销事件
        RemoveController();
    }
}
