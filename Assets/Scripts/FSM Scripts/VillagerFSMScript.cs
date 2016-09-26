using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FSMBehaviour;

public class VillagerFSMScript : FSM
{

	public FSMNode walkingToTreeFSMNode;
	public FSMNode pickingFSMNode;
	public FSMNode walkingToVillageFSMNode;
	public FSMNode droppingFruitFSMNode;
	public FSMNode inVillageFSMNode;

	// Tree toward which NPC is currently walking
	public GameObject targetTree = null;

	void Start ()
	{
		Debug.Log ("Starting '" + name + "' FSM node");

		walkingToTreeFSMNode = new WalkingToTreeNode (gameObject, "walking-to-tree");
		pickingFSMNode = new PickingNode(gameObject, "picking");
		walkingToVillageFSMNode = new WalkingToVillageNode(gameObject, "walking-to-village");
		droppingFruitFSMNode = new DroppingFruitNode(gameObject, "dropping-fruit");
		inVillageFSMNode = new InVillageNode(gameObject, "in-village");

		currentNode = walkingToTreeFSMNode;
		Debug.Log ("started for gameObject " + gameObject.name);
		currentNode.OnEntry ();
	}
}

/// <summary>
/// Contains utilities such as location of nearest tree, fruit counts, etc.
/// </summary>
public class VillagerFSMNode : FSMNode
{
	// Triggers used to change state
	protected int walkHash = Animator.StringToHash ("Walk");
	protected int idleHash = Animator.StringToHash ("Idle");
	protected int dropHash = Animator.StringToHash ("Drop");
	protected int pickHash = Animator.StringToHash ("Pickup");

	// Names of animation states
	protected int idleStateHash = Animator.StringToHash ("idle");
	protected int walkingStateHash = Animator.StringToHash ("walking");
	protected int dropStateHash = Animator.StringToHash ("dropping");
	protected int pickupStateHash = Animator.StringToHash ("pickingFruit");

	protected Animator _anim;
	protected VillagerFSMScript _villagerFSM;
	protected Villager _villager = gameObject.GetComponent<Villager>();

	protected float _speed = 5f;




	// Distance of the avatar to the given target object
	protected float DistanceToObject(GameObject target) {
		return (gameObject.transform.position-target.transform.position).magnitude;
	}

	const float veryFar = 999999999.0f;

	// Sets the targetTree to the nearest tree that still has fruit. Sets
	// targetTree to null if there is no tree with fruit.
	protected void SetTargetTree ()
	{
		// Find all the trees
		GameObject[] trees = GameObject.FindGameObjectsWithTag ("fruitTree");
		// Debug.Log ("Num fruit trees = " + trees.Length);
		_villagerFSM.targetTree = null;
		float dist = veryFar;
		foreach (GameObject t in trees) {
			Debug.Assert (t.GetComponent<FruitCount> () != null);
			if (t.GetComponent<FruitCount> ().numFruits > 0
					&& DistanceToObject(t) < dist) {
				_villagerFSM.targetTree = t;
				dist = DistanceToObject(t);
			}
		}
	}

	protected int FruitLeftToGather() {
		int numLeft = 0;
		GameObject[] trees = GameObject.FindGameObjectsWithTag ("fruitTree");
		foreach (GameObject t in trees) {
			Debug.Assert (t.GetComponent<FruitCount> () != null);
			numLeft += t.GetComponent<FruitCount> ().numFruits;
		}
		return numLeft;
	}

	public VillagerFSMNode (GameObject gameObject, string name) : base (gameObject, name) {
		_anim = gameObject.GetComponent<Animator> ();
		_villagerFSM = gameObject.GetComponent<VillagerFSMScript>();
	}
}

public class WalkingToTreeNode : VillagerFSMNode
{
	float pickingDistance = 5.0f;

	public override void OnEntry ()
	{
		SetTargetTree ();
		if(_villagerFSM.targetTree != null) {
			gameObject.transform.LookAt (_villagerFSM.targetTree.transform.position);

			_anim.SetTrigger (walkHash);
		}
	}

	public override void Do (float elapsedTime)
	{
		//Debug.Log ("Do in " + name);
		SetTargetTree ();

		if(_villagerFSM.targetTree != null) {
			gameObject.transform.LookAt (_villagerFSM.targetTree.transform.position);
			gameObject.transform.position += gameObject.transform.forward * _speed * elapsedTime;
			gameObject.transform.position =
				new Vector3 (
					gameObject.transform.position.x,
					Terrain.activeTerrain.SampleHeight (gameObject.transform.position),
					gameObject.transform.position.z);
		}
	}

	public override FSMNode CheckTransitions() {
		// Check if we're already there - if we are, transition to picking
		if(_villagerFSM.targetTree == null) {
			// walk back to village		
			return _villagerFSM.walkingToVillageFSMNode;
		} else if(DistanceToObject(_villagerFSM.targetTree) <= pickingDistance) {
			return _villagerFSM.pickingFSMNode;
		} else {
			return null;
		}
	}

	public WalkingToTreeNode (GameObject gameObject, string name) : base (gameObject, name)
	{
		Debug.Log ("Initialized WalkingNode for gameobject " + gameObject.name);
	}
}

public class PickingNode : VillagerFSMNode
{
	public override void OnEntry ()
	{
		// show the picking animation
		_anim.SetTrigger (pickHash);

		// grab fruit from the tree
		int numFruitCarryable = _villager.maxFruitCarried - _villager.numFruitCarried;

		FruitCount t = _villagerFSM.targetTree.GetComponent<FruitCount>();

		int numFruitPicked = Mathf.Min(t.numFruits, numFruitCarryable);
		t.numFruits -= numFruitPicked;
		_villager.numFruitCarried += numFruitPicked;
	}
		
	public override void Do (float elapsedTime)
	{
		//Debug.Log ("Do in " + name);
	}

	public override void OnExit() {
	}

	public override FSMNode CheckTransitions() {
		// Transition when the picking animation is done. If we got enough fruit, head back to the
		// village. If we can carry more fruit, head to another tree.

		// This state is active until the picking animation is done. We first
		// transition into this animation state, then execute this animation state.
		if(_anim.GetAnimatorTransitionInfo(0).IsName("walking -> pickingFruit") ||
			_anim.GetCurrentAnimatorStateInfo(0).shortNameHash == pickupStateHash) {
			return null;
		} else {
			// If we can carry more, go to another tree. Otherwise return to the village.
			if(_villager.numFruitCarried < _villager.maxFruitCarried) {
				return _villagerFSM.walkingToTreeFSMNode;
			} else {
				return _villagerFSM.walkingToVillageFSMNode;
			}
		}
	}

	public PickingNode (GameObject gameObject, string name) : base (gameObject, name)
	{
		Debug.Log ("Initialized PickingNode for gameobject " + gameObject.name);
	}
}

public class WalkingToVillageNode : VillagerFSMNode
{
	float arrivedThreshold = 5.0f;

	GameObject villageMarker;

	void FaceVillage() {
		gameObject.transform.LookAt (villageMarker.transform.position);
	}


	public override void OnEntry ()
	{
		// Point avatar toward village centre
		FaceVillage();

		// show the walking animation
		_anim.SetTrigger (walkHash);
	}

	public override void Do (float elapsedTime)
	{
		// Debug.Log ("Do in " + name);
		gameObject.transform.position += gameObject.transform.forward * _speed * elapsedTime;
		gameObject.transform.position =
			new Vector3 (
				gameObject.transform.position.x,
				Terrain.activeTerrain.SampleHeight (gameObject.transform.position),
				gameObject.transform.position.z);
		
	}

	public override void OnExit() {
	}

	public override FSMNode CheckTransitions() {
		// transition when arrived in village
		bool inVillage = DistanceToObject(villageMarker) <= arrivedThreshold;
		if(inVillage) {
			return _villagerFSM.droppingFruitFSMNode;
		} else {
			// still walking, keep going
			return null;
		}
	}

	public WalkingToVillageNode (GameObject gameObject, string name) : base (gameObject, name)
	{
		Debug.Log ("Initialized WalkingToVillageNode for gameobject " + gameObject.name);

		villageMarker = _villager.home;
	}
}

public class DroppingFruitNode : VillagerFSMNode
{
	public override void OnEntry ()
	{
		// show the dropping animation
		_anim.SetTrigger (dropHash);

		// This villager is no longer carrying fruit
		_villager.numFruitCarried = 0;

		//TODO: Add GUI to show how much fruit has been dropped off in village
	}

	public override void Do (float elapsedTime)
	{
	}

	public override void OnExit() {
	}

	public override FSMNode CheckTransitions() {
		// once dropping animation is done, transition into InVillage state
		if(_anim.GetAnimatorTransitionInfo(0).IsName("walking -> dropping") ||
			_anim.GetCurrentAnimatorStateInfo(0).shortNameHash == dropStateHash) {
			return null;
		} else {
			return _villagerFSM.inVillageFSMNode;
		}
	}

	public DroppingFruitNode (GameObject gameObject, string name) : base (gameObject, name)
	{
		Debug.Log ("Initialized WalkingToVillageNode for gameobject " + gameObject.name);
	}
}

public class InVillageNode : VillagerFSMNode
{
	public override void OnEntry ()
	{
		// show the idle animation
		_anim.SetTrigger (idleHash);
	}

	public override void Do (float elapsedTime)
	{
	}

	public override void OnExit() {
	}

	public override FSMNode CheckTransitions() {
		// if there is more fruit available, go fetch it
		if(FruitLeftToGather() > 0) {
			return _villagerFSM.walkingToTreeFSMNode;
		} else {
			// fruit is all done; hang out
			return null;
		}
	}

	public InVillageNode (GameObject gameObject, string name) : base (gameObject, name)
	{
		Debug.Log ("Initialized WalkingToVillageNode for gameobject " + gameObject.name);
	}
}