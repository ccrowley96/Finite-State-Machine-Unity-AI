using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FSMBehaviour {

	public abstract class FSMNode {
		public string name;

		protected GameObject gameObject;

		virtual public void Do(float elapsedTime) {}
		
		virtual public void OnEntry() {}
		
		virtual public void OnExit() {}

		// Returns first transition whose guard evaluates to true. If none, returns null.
		virtual public FSMNode CheckTransitions() {
			return null;
		}

		public FSMNode(GameObject gameObject, string name) {
			this.name = name;
			this.gameObject = gameObject;
		}
	}
}