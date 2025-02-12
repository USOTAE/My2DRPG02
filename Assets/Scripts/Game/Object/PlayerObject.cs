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
    //当前的跳跃速度
    private float nowJumpSpeed;

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
            bodyTransform.Translate(Vector2.up * nowJumpSpeed * Time.deltaTime);
            //竖直上抛 下落逻辑 速度变化
            //v = v - gt
            nowJumpSpeed -= gSpeed * Time.deltaTime;

            //我们判断高度是否小于等于0 即可判断是否落地
            //注意 一定不是判断 == 0 因为是 - 帧间隔时间*速度 大部分情况都不会刚刚等于0
            //注意使用的是localPosition 了解Position和localPosition
            if (bodyTransform.localPosition.y <= 0)
            {
                //放置到地面
                bodyTransform.localPosition = Vector2.zero;
                //改变地面标识
                roleAnimator.SetBool("isGround", true);
            }
        }
    }

    private void Jump()
    {
        //在地面是true 才来进行跳跃
        //之后再添加跳跃的约束条件
        if (roleAnimator.GetBool("isGround"))
        {
            nowJumpSpeed = jumpSpeed;
            //切换动作
            roleAnimator.SetTrigger("jumpTrigger");
            //切换在地面的状态
            roleAnimator.SetBool("isGround", false);
        }
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
                Debug.Log("J");
                break;
            case KeyCode.K:
                Debug.Log("K");
                break;
            case KeyCode.L:
                Debug.Log("L");
                break;
            case KeyCode.Space:
                Debug.Log("Space");
                Jump();
                break;
        }
    }

    private void OnDestroy()
    {
        //事件 有加就有减 移除时一定要注销事件
        RemoveController();
    }
}
