using UnityEngine;

public class Dropping : FSMNode {

	private int dropHash = Animator.StringToHash ("Drop");
    public GameObject dropTarget;
    public override void entry(){
        _anim.SetTrigger(dropHash);
    }
    public override FSMNode doActivity(){
        controller.dropApples();
        
        // On animation completion, select next node
        if(_anim.GetCurrentAnimatorStateInfo(0).IsName("idle")){
            Walking walkNode = gameObject.AddComponent(typeof(Walking)) as Walking;
            walkNode.controller = this.controller;
            walkNode.target = controller.selectRandomTree();
            exit();
            return walkNode;
        }
        return this;
    }
    public override void exit(){
        Destroy(this);
    }
}