using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactorController : MonoBehaviour {
	public float HP = 100f;
	[System.Serializable]
	public class MoveSettings
	{
		public float forwardVel = 10;
		public float runVel = 15;
		public float rotateVel = 150;
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
		public string RUN_AXIS = "Fire3";
	}

	public MoveSettings moveSetting = new MoveSettings ();
	public PhysSettings physSetting = new PhysSettings ();
	public InputSettings inputSetting = new InputSettings ();


	Vector3 velocity = Vector3.zero;
	Quaternion targetRotation;
	Rigidbody rBody;
	Animator anim;
	float forwardInput, turnInput, runInput;

	public Quaternion TargetRotation
	{
		get{return targetRotation;}
	}
		
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		targetRotation = transform.rotation;
		rBody = GetComponent<Rigidbody> ();
		forwardInput = turnInput = 0;
	}

	void GetInput(){
		forwardInput = Input.GetAxis (inputSetting.FORWARD_AXIS);
		turnInput = Input.GetAxis (inputSetting.TURN_AXIS);
		runInput = Input.GetAxisRaw (inputSetting.RUN_AXIS);
	}

	// Update is called once per frame
	void Update () {
		GetInput ();
		Turn ();
		Walk ();
		rBody.velocity = transform.TransformDirection (velocity);
		if (Input.GetKeyDown ("e")) {
			anim.SetFloat ("Blend", 3);
		}
		if (Input.GetKeyDown ("f")) {
			anim.Play ("Dance");
		}
	}

	//	void FixUpdate(){
	//		Walk ();
	//	}

//	private void Move ()
//	{
//		v = Mathf.Lerp (v, m_MovementInputValue, Time.deltaTime);
//		//   transform.Translate (Vector3.forward * v * Time.deltaTime * m_Speed);
//		anim.SetFloat ("Blend", v);
//	}

	void Walk(){
		if (Mathf.Abs (forwardInput) > inputSetting.inputDelay) {
			if (runInput == 0) {
				velocity.z = forwardInput * moveSetting.forwardVel;
				anim.SetFloat ("Blend", forwardInput);

			} else {
				velocity.z = forwardInput * moveSetting.runVel;

				anim.SetFloat ("Blend", forwardInput + 1);
				}
			}
		 else {
			velocity.z = 0;
		}
	}

	void Turn(){
		if (Mathf.Abs (turnInput) > inputSetting.inputDelay) {
			targetRotation *= Quaternion.AngleAxis (moveSetting.rotateVel * turnInput * Time.deltaTime, Vector3.up);
		}
		transform.rotation = targetRotation;
	}


}
