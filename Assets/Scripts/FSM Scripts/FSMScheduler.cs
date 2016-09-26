using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using FSMBehaviour;

public class FSMScheduler : MonoBehaviour {

	public List<FSM> fsms = new List<FSM>();

	Dictionary<string, float> _timeSinceInvocation = new Dictionary<string, float>();

	void Start() {
		GameObject[] fsmObjects = GameObject.FindGameObjectsWithTag("FSM");
		Debug.Log("Found " + fsmObjects.Length + " FSM's");
		foreach(GameObject g in fsmObjects) {
			FSM f = g.GetComponent<FSM>();
			string name = f == null ? "<None>" : f.name;
			Debug.Log("Adding " + g.name + " with component " + name);
			fsms.Add (g.GetComponent<FSM>());

			_timeSinceInvocation[f.name] = 0f;
		}
	}

	void Update() {
		foreach(FSM f in fsms) {
			_timeSinceInvocation[f.name] += Time.deltaTime;
			if(_timeSinceInvocation[f.name] >= f.lodFrequency) {
				f.Run();
				_timeSinceInvocation[f.name] = 0f;
			}
		}
	}
}
