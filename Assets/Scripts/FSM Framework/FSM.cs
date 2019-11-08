using System.Collections.Generic; 
using UnityEngine;

public abstract class FSM : MonoBehaviour {
    public FSMNode currentNode;
    public Animator _anim;
    public Utils.priority priority = Utils.priority.high;

    public void updateFSM(){
        if(currentNode){
            currentNode = currentNode.doActivity();
        }
    }
}