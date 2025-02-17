using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色对象基类 之后的怪物 玩家都继承它
/// </summary>
public abstract class RoleObject : MonoBehaviour
{
    //角色的移动方向
    protected Vector2 moveDir = Vector2.zero;

    //玩家跳跃初速度
    public float jumpSpeed = 10;
    //重力加速度
    public float gSpeed = 9.8f;
    //移动速度
    public int speed = 5;
    //当前竖直上抛的速度
    protected float nowYSpeed;
    //击退速度
    protected float nowXSpeed;

    //身体sprite
    protected SpriteRenderer roleSprite;
    //角色animator
    protected Animator roleAnimator;
    //身体子对象
    protected Transform bodyTransform;
    //影子对象
    protected Transform shadowTransform;

    protected virtual void Awake()
    {
        //多个子对象时先找到目标子对象
        bodyTransform = this.transform.Find("Role");
        shadowTransform = this.transform.Find("Shadow");
        roleSprite = bodyTransform.GetComponentInChildren<SpriteRenderer>();
        roleAnimator = this.GetComponentInChildren<Animator>();
    }

    protected virtual void Update()
    {
        //处理移动
        CheckMove();
        //处理击飞和跳跃
        CheckJumpOrHitFly();

    }

    /// <summary>
    /// 都有攻击行为
    /// </summary>
    public abstract void Atk();

    /// <summary>
    /// 都有死亡方法
    /// </summary>
    public abstract void Dead();

    /// <summary>
    /// 检测玩家移动相关逻辑
    /// </summary>
    protected void CheckMove()
    {
        //在移动前加判断 满足移动条件才去移动
        if (CanMoving)
        {
            //角色移动逻辑
            this.transform.Translate(moveDir.normalized * speed * Time.deltaTime);
            //控制玩家转向 不考虑0 解决松开按键后的强制转向问题
            if (moveDir.x > 0)
            {
                roleSprite.flipX = false;
                //shadowTransform.localPosition = Vector3.right * 0.04f;
                shadowTransform.localPosition = new Vector3(-0.04f, -1.2f);
            }
            else if (moveDir.x < 0)
            {
                roleSprite.flipX = true;
                //shadowTransform.localPosition = Vector3.right * -0.04f;
                shadowTransform.localPosition = new Vector3(0.04f, -1.2f);
            }
        }

        //是否移动
        if (moveDir == Vector2.zero)
            ChangeAction(E_Action_Type.Idle);
        else
            ChangeAction(E_Action_Type.Move);
    }

    /// <summary>
    /// 检测处理 跳跃或者击飞逻辑
    /// </summary>
    protected void CheckJumpOrHitFly()
    {
        //处理 跳跃逻辑
        //不是在地面上 就可以跳跃
        if (roleAnimator.GetBool("isGround") == false)
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
                ChangeRoleIsGround(true);

                //落地后 不管击退多厉害 都得停下来
                nowXSpeed = 0;
                //应该让他延迟站起来
                Invoke(nameof(DelayClearHitFly), 2f);
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
    public virtual void Wound(float hitTime)
    {
        //如果受伤时是击飞状态 不需要再处理受伤的动作逻辑
        if (roleAnimator.GetBool("isHitFly"))
            return;

        //如果处于受伤状态 又受伤 需要把上一次延时函数取消
        CancelInvoke(nameof(DelayClearHit));
        //切换受伤动作
        ChangeAction(E_Action_Type.Hit);
        //延时函数 处理过一段时间结束受伤状态
        Invoke(nameof(DelayClearHit), hitTime);
    }

    /// <summary>
    /// 击飞方法
    /// </summary>
    public virtual void HitFly(float xSpeed, float ySpeed)
    {
        //如果已经在击飞状态 直接返回 不能再被击飞
        if (roleAnimator.GetBool("isHitFly"))
            return;

        //如果当前是受伤状态 击飞的优先级高于它 需要改变状态
        if (roleAnimator.GetBool("isHit"))
        {
            //取消延迟清除受伤状态
            CancelInvoke(nameof(DelayClearHit));
            //直接清除受伤状态
            roleAnimator.SetBool("isHit", false);
        }


        //切换击飞动作
        ChangeAction(E_Action_Type.HitFly);
        //改变玩家不在地面
        ChangeRoleIsGround(false);
        //初始竖直上抛的速度
        nowYSpeed = ySpeed;
        nowXSpeed = xSpeed;
    }

    private void DelayClearHit()
    {
        roleAnimator.SetBool("isHit", false);
    }

    /// <summary>
    /// 延迟起身
    /// </summary>
    private void DelayClearHitFly()
    {
        roleAnimator.SetBool("isHitFly", false);
    }

    /// <summary>
    /// 切换动作
    /// </summary>
    /// <param name="type"></param>
    protected void ChangeAction(E_Action_Type type)
    {
        switch (type)
        {
            case E_Action_Type.Idle:
                roleAnimator.SetBool("isMoving", false);
                break;
            case E_Action_Type.Move:
                roleAnimator.SetBool("isMoving", true);
                break;
            case E_Action_Type.Jump:
                roleAnimator.SetTrigger("jumpTrigger");
                break;
            case E_Action_Type.JumpAtk:
                roleAnimator.SetTrigger("jumpAtkTrigger");
                break;
            case E_Action_Type.Atk:
                roleAnimator.SetTrigger("atkTrigger");
                break;
            case E_Action_Type.FootAtk:
                roleAnimator.SetTrigger("footAtkTrigger");
                break;
            case E_Action_Type.Hit:
                roleAnimator.SetBool("isHit", true);
                break;
            case E_Action_Type.HitFly:
                roleAnimator.SetBool("isHitFly", true);
                break;
            case E_Action_Type.Dead:
                roleAnimator.SetBool("isDead", true);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 切换玩家是否在地面状态
    /// </summary>
    /// <param name="isGround"></param>
    protected void ChangeRoleIsGround(bool isGround)
    {
        roleAnimator.SetBool("isGround", isGround);
    }

    protected bool CanMoving
    {
        get
        {
            //去得到状态机中两层的状态 判断是否是可以移动的状态
            //AnimatorStateInfo layerInfo1 = roleAnimator.GetCurrentAnimatorStateInfo(0);
            AnimatorStateInfo layerInfo2 = roleAnimator.GetCurrentAnimatorStateInfo(1);
            if (IsAtkState ||
                layerInfo2.IsName("Defend") ||
                layerInfo2.IsName("Hit") ||
                layerInfo2.IsName("HitFly") ||
                layerInfo2.IsName("Pickup") ||
                layerInfo2.IsName("Throw") ||
                roleAnimator.GetBool("isDead"))
                return false;

            return true;
        }
    }

    /// <summary>
    /// 是否是攻击状态
    /// </summary>
    protected bool IsAtkState
    {
        get
        {
            AnimatorStateInfo layerInfo = roleAnimator.GetCurrentAnimatorStateInfo(1);
            if (layerInfo.IsName("Atk1") ||
                layerInfo.IsName("Atk2") ||
                layerInfo.IsName("Atk3") ||
                layerInfo.IsName("FootAtk1") ||
                layerInfo.IsName("FootAtk2"))
                return true;
            return false;
        }
    }
}

public enum E_Action_Type
{

    Idle,
    Move,
    Jump,
    /// <summary>
    /// 跳跃攻击
    /// </summary>
    JumpAtk,
    Atk,
    /// <summary>
    /// 腿部攻击 对应Kick动画
    /// </summary>
    FootAtk,
    Hit,
    /// <summary>
    /// 击飞
    /// </summary>
    HitFly,
    /// <summary>
    /// 投掷道具
    /// </summary>
    Throw,
    Pickup,
    Defend,
    /// <summary>
    /// 死亡
    /// </summary>
    Dead
}