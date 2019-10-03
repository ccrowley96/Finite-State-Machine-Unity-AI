using UnityEngine;

public abstract class FSMNode: MonoBehaviour{
    public FSMNode nextNode;
    public static Animator _anim;
    public static FSMController controller;
    public void Start(){
        // On activation, call entry
        this.entry();
    }
    public abstract void entry();
    public abstract FSMNode doActivity();
    public abstract void exit();
}