﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedState : MechStateMachineBehaviour {

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		base.OnStateEnter(animator, stateInfo, layerIndex);
		if (cc == null || !cc.enabled) return;

		animator.SetBool ("Grounded", true);

	}
	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if (cc == null || !cc.enabled || !cc.isGrounded) return;
		float speed = Input.GetAxis("Vertical");
		float direction = Input.GetAxis("Horizontal");

		if(animator.GetBool("Boost") == true && !Input.GetKey(KeyCode.LeftShift)){
			animator.SetBool ("Boost", false); // not shutting down ,happens when boosting before slashing
			mctrl.Boost(false);
		}
		animator.SetBool ("OnSlash", false);  // if grounded => not on slash

		if (Input.GetKey(KeyCode.Space)) {
			animator.SetBool("Grounded", false);
			animator.SetBool("Jump", true);
			mctrl.Jump();
			return;
		}

		if (speed > 0 || speed < 0 || direction > 0 || direction < 0) {
			mctrl.Run();

			if (Input.GetKey(KeyCode.LeftShift) && mcbt.EnoughFuelToBoost() && animator.GetBool("Boost")== false) {
				mctrl.GetComponentInChildren<Sounds> ().PlayBoostStart ();
				animator.SetBool("Boost", true);
				mctrl.Boost(true);
			}
		}
			
		animator.SetFloat("Speed", speed);
		animator.SetFloat("Direction", direction);
	}
}
