using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FSMBehaviour {
	public class FSM : MonoBehaviour {

		public float lodFrequency = 0.016f;

		protected List<FSMNode> _nodes = new List<FSMNode>();
		protected FSMNode currentNode;

		float lastTime = 0f;

		public void Run() {
			//Debug.Log("Running " + name + " with current node " + currentNode.name);

			// Update time
			float elapsedTime = 0f;
			if(lastTime > 0) {
				elapsedTime = Time.time - lastTime;
			}
			lastTime = Time.time;

			// see if a transition is required
			FSMNode nextNode = currentNode.CheckTransitions();
			if(nextNode != null) {
				Debug.Log("Transitioning to " + nextNode.name);
				currentNode.OnExit();
				currentNode = nextNode;
				currentNode.OnEntry();
			}
			currentNode.Do(elapsedTime);
		}
	}
}