using UnityEngine;

public class Picking : FSMNode {

	private int pickHash = Animator.StringToHash ("Pickup");
    public GameObject pickTarget;
    public override void entry(){
        _anim.SetTrigger(pickHash);
    }
    public override FSMNode doActivity(){
       
        // On animation completion, select next node
        if(_anim.GetCurrentAnimatorStateInfo(0).IsName("idle")){
            Utils.pickingStates ps = controller.pickTree(pickTarget);

            Walking walkNode = gameObject.AddComponent(typeof(Walking)) as Walking;
            walkNode.controller = this.controller;
            //animation finished
            if(ps == Utils.pickingStates.newTree){ // walk to random tree
                walkNode.target = controller.selectRandomTree();
                exit();
                return walkNode;
            } else{ // walk to village for drop off
                walkNode.target = controller.selectVillage();
                exit();
                return walkNode;
            }
        }
        return this;
    }
    public override void exit(){
        Destroy (this);
    }
}