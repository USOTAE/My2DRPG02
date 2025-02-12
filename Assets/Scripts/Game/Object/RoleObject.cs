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

    protected virtual void Awake()
    {
        //多个子对象时先找到目标子对象
        bodyTransform = this.transform.Find("Role");
        roleSprite = bodyTransform.GetComponentInChildren<SpriteRenderer>();
        roleAnimator = this.GetComponentInChildren<Animator>();
    }

    protected virtual void Update()
    {
        //角色移动逻辑
        this.transform.Translate(moveDir.normalized * speed * Time.deltaTime);
        //控制玩家转向 不考虑0 解决松开按键后的强制转向问题
        if (moveDir.x > 0)
        {
            roleSprite.flipX = false;
        }
        else if (moveDir.x < 0)
        {
            roleSprite.flipX = true;
        }

        //是否移动
        if(moveDir == Vector2.zero)
        {
            ChangeAction(E_Action_Type.Idle);
        }
        else
        {
            ChangeAction(E_Action_Type.Move);
        }
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
                break;
            case E_Action_Type.JumpAtk:
                break;
            case E_Action_Type.Atk1:
                break;
            case E_Action_Type.Atk2:
                break;
            case E_Action_Type.Atk3:
                break;
            case E_Action_Type.FootAtk1:
                break;
            case E_Action_Type.FootAtk2:
                break;
            case E_Action_Type.Hit:
                break;
            case E_Action_Type.HitFly:
                break;
            case E_Action_Type.Throw:
                break;
            case E_Action_Type.Pickup:
                break;
            case E_Action_Type.Defend:
                break;
            default:
                break;
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
    Atk1,
    Atk2,
    Atk3,
    /// <summary>
    /// 腿部攻击 对应Kick动画
    /// </summary>
    FootAtk1,
    FootAtk2,
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