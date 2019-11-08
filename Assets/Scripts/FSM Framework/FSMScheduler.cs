using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FSMScheduler : MonoBehaviour {

    public List<GameObject> FSMList = new List<GameObject>();
    private float elapsedTime = 0.0f;
    public void Update(){
        foreach(GameObject g in FSMList){
            FSM f = g.GetComponent<FSM>();
            if(f.priority == Utils.priority.low){ //update once per second
                if (elapsedTime >= 1.0f) {
                    elapsedTime = 0.0f;
                    f.updateFSM();
                }
            } else{ // update every frame
                f.updateFSM();
            }
        }
        
        elapsedTime += Time.deltaTime;
    }
}