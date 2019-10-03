using System.Collections.Generic; 
using UnityEngine;

public abstract class FSM : MonoBehaviour {
    public FSMNode currentNode;
    public Animator _anim;
    public Utils.priority priority = Utils.priority.low;

    private float elapsedTime = 0.0f;
    public void Update(){
        if(currentNode){
            // Check priority
            if(priority == Utils.priority.low){//update once per second
                if (elapsedTime >= 1.0f) {
                    elapsedTime = 0.0f;
                    currentNode = currentNode.doActivity();
                }
            } else{
                currentNode = currentNode.doActivity();
            }
        }
        elapsedTime += Time.deltaTime;
    }
}