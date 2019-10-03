using UnityEngine;

public class Idle : FSMNode {

	private int idleHash = Animator.StringToHash ("Idle");
    public override void entry(){
        _anim.SetTrigger(idleHash);
    }
    public override FSMNode doActivity(){
        return this;
    }
    public override void exit(){
        Destroy(this);
    }
}