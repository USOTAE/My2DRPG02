using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterObject : RoleObject
{
    /// <summary>
    /// 攻击
    /// </summary>
    public override void Atk()
    {
        //切换攻击状态即可 不需要考虑其他互斥 状态机连线控制
        ChangeAction(E_Action_Type.Atk);
    }

    /// <summary>
    /// 移动
    /// </summary>
    /// <param name="dir"></param>
    public void Move(Vector2 dir)
    {
        //改变移动方向 在父类的Update逻辑中 就会去处理移动相关的逻辑
        moveDir = dir;
    }

    /// <summary>
    /// 死亡
    /// </summary>
    public override void Dead()
    {
        ChangeAction(E_Action_Type.Dead);
    }
}
