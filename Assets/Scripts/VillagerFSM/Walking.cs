using UnityEngine;

public class Walking : FSMNode {

    public float speed = 20.0f;
	public float pickingDistance = 5.0f;
    public float droppingDistance = 12.0f;

    private int walkHash = Animator.StringToHash("Walk");
    bool atDest = false;
	public GameObject target = null;

    public override void entry(){
        if(target == null) return;
        _anim.SetTrigger(walkHash);
    }
    public override FSMNode doActivity(){
        if(target == null){
            exit();
            return gameObject.AddComponent(typeof(Idle)) as Idle;
        }

        float distanceThresh = target.tag == "fruitTree" ? pickingDistance : droppingDistance;

        if((transform.position - target.transform.position).magnitude <= distanceThresh && !atDest) {
			atDest = true;
		} else if(atDest) {
            // Check if @ tree 
            if(target.tag == "fruitTree"){
                Picking pickNode = gameObject.AddComponent(typeof(Picking)) as Picking;
                pickNode.pickTarget = target;
                exit();
                return pickNode;
            } else{ // else village
                Dropping dropNode = gameObject.AddComponent(typeof(Dropping)) as Dropping;
                exit();
                return dropNode;
            }
		} else {
            transform.LookAt(target.transform.position);
			transform.position += transform.forward * speed * Time.deltaTime;
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