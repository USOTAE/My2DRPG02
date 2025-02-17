using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObject : RoleObject
{
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

    /// <summary>
    /// 死亡
    /// </summary>
    public override void Dead()
    {
    }

    /// <summary>
    /// 拾取
    /// </summary>
    public void PickUp()
    {
        roleAnimator.SetTrigger("pickUpTrigger");
    }

    /// <summary>
    /// 投掷
    /// </summary>
    public void Throw()
    {
        roleAnimator.SetTrigger("throwTrigger");
    }


    private void Jump()
    {
        //在地面是true 才来进行跳跃
        //之后再添加跳跃的约束条件
        AnimatorStateInfo layerInfo = roleAnimator.GetCurrentAnimatorStateInfo(1);
        if (roleAnimator.GetBool("isGround") &&
            roleAnimator.GetBool("isDefend") == false &&
            roleAnimator.GetBool("isHit") == false &&
            roleAnimator.GetBool("isHitFly") == false &&
            IsAtkState == false &&
            layerInfo.IsName("Pickup") == false &&
            layerInfo.IsName("Throw") == false)
        {
            nowYSpeed = jumpSpeed;
            //切换动作
            ChangeAction(E_Action_Type.Jump);
            //切换在地面的状态
            ChangeRoleIsGround(false);
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
            ChangeAction(E_Action_Type.JumpAtk);
        }
    }

    /// <summary>
    /// 手部攻击
    /// </summary>
    public override void Atk()
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
        ChangeAction(E_Action_Type.Atk);

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

        //切换腿部攻击
        ChangeAction(E_Action_Type.FootAtk);

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
            case KeyCode.Alpha1:
                Wound(1f);
                break;
            case KeyCode.Alpha2:
                HitFly(-10f,10f);
                break;
            case KeyCode.Alpha3:
                PickUp();
                break;
            case KeyCode.Alpha4:
                Throw();
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
