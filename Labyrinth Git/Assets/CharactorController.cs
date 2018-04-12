using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactorController : MonoBehaviour {
	[System.Serializable]
	public class MoveSettings
	{
		public float forwardVel = 10;
		public float runVel = 15;
		public float rotateVel = 100;
		public float jumpVel = 100;
		public float distToGrounded = 27.0f;
		public LayerMask ground;
	}

	[System.Serializable]
	public class PhysSettings
	{
		public float downAccel = 2.75f;
	}

	[System.Serializable]
	public class InputSettings
	{
		public float inputDelay = 0.1f;
		public string FORWARD_AXIS = "Vertical";
		public string TURN_AXIS = "Horizontal";
		public string JUMP_AXIS = "Jump";
		public string RUN_AXIS = "Fire3";
	}

	public MoveSettings moveSetting = new MoveSettings ();
	public PhysSettings physSetting = new PhysSettings ();
	public InputSettings inputSetting = new InputSettings ();


	Vector3 velocity = Vector3.zero;
	Quaternion targetRotation;
	Rigidbody rBody;
	Animation anim;
	float forwardInput, turnInput, jumpInput, runInput;

	public Quaternion TargetRotation
	{
		get{return targetRotation;}
	}

	bool Grounded(){
		return Physics.Raycast (transform.position, Vector3.down, moveSetting.distToGrounded, moveSetting.ground);
	}
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animation> ();
		targetRotation = transform.rotation;
		rBody = GetComponent<Rigidbody> ();
		forwardInput = turnInput = 0;
	}

	void GetInput(){
		forwardInput = Input.GetAxis (inputSetting.FORWARD_AXIS);
		turnInput = Input.GetAxis (inputSetting.TURN_AXIS);
		jumpInput = Input.GetAxisRaw (inputSetting.JUMP_AXIS);
		runInput = Input.GetAxisRaw (inputSetting.RUN_AXIS);
	}

	// Update is called once per frame
	void Update () {
		GetInput ();
		Turn ();
		Walk ();
		Jump ();
		rBody.velocity = transform.TransformDirection (velocity);
		if (Input.GetKeyDown ("e")) {
			anim.Play ("attack");
		}
		if (Input.GetKeyDown ("f")) {
			anim.Play ("Dance");
		}
	}

	//	void FixUpdate(){
	//		Walk ();
	//	}

	void Walk(){
		if (Mathf.Abs (forwardInput) > inputSetting.inputDelay) {
			if (runInput == 0) {
				velocity.z = forwardInput * moveSetting.forwardVel;
				if (anim.IsPlaying ("Jump")) {
				} else {
					anim.Play ("walk");
				}
			} else {
				velocity.z = forwardInput * moveSetting.runVel;
				if (anim.IsPlaying ("Jump")) {
				} else {
					anim.Play ("run");
				}
			}
		} else {
			velocity.z = 0;
		}
	}

	void Turn(){
		if (Mathf.Abs (turnInput) > inputSetting.inputDelay) {
			targetRotation *= Quaternion.AngleAxis (moveSetting.rotateVel * turnInput * Time.deltaTime, Vector3.up);
		}
		transform.rotation = targetRotation;
	}

	void Jump(){
		if (jumpInput > 0 && Grounded ()) {
			//jump
			velocity.y = moveSetting.jumpVel;
			anim.Play ("Jump");
		} else if (jumpInput == 0 && Grounded ()) {
			//
			velocity.y = 0;
		} else {
			velocity.y -= physSetting.downAccel;
		}
	}
}
