using System.Collections.Generic; 
using UnityEngine;

public abstract class FSM : MonoBehaviour {
    public FSMNode currentNode;
    public Animator _anim;
    public abstract void Update();
    public Animator GetAnimator(){
        return _anim;
    }
}