using UnityEngine;
using System;

public abstract class FSMNode: MonoBehaviour{
    public FSMController controller;
    public Animator _anim{
        get {
            return controller._anim;
        }
    }
    public void Start(){
        // On activation, call entry
        this.entry();
    }
    public abstract void entry();
    public abstract FSMNode doActivity();
    public abstract void exit();
    
}