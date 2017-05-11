using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AIStateChange : StateMachineBehaviour 
{
	[Header ("OnEnter")]
	public List<AIComponents> enterOn;
	public List<AIComponents> enterOff;
	public List<AIComponents> enterToggle;

	[Header ("OnExit")]
	public List<AIComponents> exitOn;
	public List<AIComponents> exitOff;
	public List<AIComponents> exitToggle;

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
	{
		Component[] components = animator.gameObject.GetComponents (typeof(PlayersGameplay));

		foreach (Component c in components) 
		{
			foreach(AIComponents o in enterOn)
			{
				if(o.ToString () == c.GetType ().ToString ())
					(c as Behaviour).enabled = true;
			}

			foreach(AIComponents o in enterOff)
			{
				if(o.ToString () == c.GetType ().ToString ())
					(c as Behaviour).enabled = false;
			}

			foreach(AIComponents o in enterToggle){
				if(o.ToString () == c.GetType ().ToString ())
					(c as Behaviour).enabled = !(c as Behaviour).isActiveAndEnabled;
			}
		}
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	//override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
	{
		Component[] components = animator.gameObject.GetComponents (typeof(PlayersGameplay));

		foreach (Component c in components) 
		{
			foreach(AIComponents o in exitOn)
			{
				if(o.ToString () == c.GetType ().ToString ())
					(c as Behaviour).enabled = true;
			}

			foreach(AIComponents o in exitOff)
			{
				if(o.ToString () == c.GetType ().ToString ())
					(c as Behaviour).enabled = false;
			}

			foreach(AIComponents o in exitToggle){
				if(o.ToString () == c.GetType ().ToString ())
					(c as Behaviour).enabled = !(c as Behaviour).isActiveAndEnabled;
			}
		}
	}

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
