using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色对象基类 之后的怪物 玩家都继承它
/// </summary>
public class RoleObject : MonoBehaviour
{
    //角色的移动方向
    protected Vector2 moveDir = Vector2.zero;

    //移动速度
    public int speed = 5;

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
                layerInfo2.IsName("Throw"))
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
    Defend
}