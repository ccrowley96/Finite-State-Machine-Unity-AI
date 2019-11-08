using UnityEngine;

public class Walking : FSMNode {

    public float speed = 5.0f;
	public float pickingDistance = 5.0f;
    public float droppingDistance = 12.0f;

    private int walkHash = Animator.StringToHash("Walk");
    private float localTimeDelta;
    private float lastTimeStamp;
    bool atDest = false;
	public GameObject target = null;

    public override void entry(){
        localTimeDelta = 0.0f;
        lastTimeStamp = Time.time;
        // if(target == null) return;
        _anim.SetTrigger(walkHash);
    }
    public override FSMNode doActivity(){
        localTimeDelta = Time.time - lastTimeStamp;
        lastTimeStamp = Time.time;
        if(target == null){
            if(controller.appleBackpack > 0)
                target = controller.selectVillage();
            else{
                Idle idle = gameObject.AddComponent(typeof(Idle)) as Idle;
                idle.controller = this.controller;
                exit();
                return idle;
            }
        }

        float distanceThresh = target.tag == "fruitTree" ? pickingDistance : droppingDistance;

        if((transform.position - target.transform.position).magnitude <= distanceThresh && !atDest) {
			atDest = true;
		} else if(atDest) {
            // Check if @ tree 
            if(target.tag == "fruitTree"){
                Picking pickNode = gameObject.AddComponent(typeof(Picking)) as Picking;
                pickNode.controller = this.controller;
                pickNode.pickTarget = target;
                exit();
                return pickNode;
            } else{ // else village
                Dropping dropNode = gameObject.AddComponent(typeof(Dropping)) as Dropping;
                dropNode.controller = this.controller;
                exit();
                return dropNode;
            }
		} else {
            transform.LookAt(target.transform.position);
            transform.position += transform.forward * speed * localTimeDelta;
			transform.position =
				new Vector3(
					transform.position.x,
					Terrain.activeTerrain.SampleHeight(transform.position),
			    	transform.position.z);
		}
        return this;
    }

    public override void exit(){
        Destroy (this);
    }
}