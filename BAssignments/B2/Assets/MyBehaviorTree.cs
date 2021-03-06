﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TreeSharpPlus;
using UnityEngine.UI;

public class MyBehaviorTree : MonoBehaviour
{
	public Transform wander1;
	public Transform wander2;
	public Transform wander3;
	public GameObject punchee;
	public GameObject participant;
	public GameObject benchGuy1;
	public GameObject benchGuy2;
	public GameObject demonFire;

	private GameObject[] daniels;
	private bool isFirst = true;
	private GameObject[] acts;

	private BehaviorAgent behaviorAgent;
	// Use this for initialization
	void Start ()
	{
		behaviorAgent = new BehaviorAgent (this.BuildTreeRoot ());
		BehaviorManager.Instance.Register (behaviorAgent);
		behaviorAgent.StartBehavior ();
		acts = new GameObject[]{ GameObject.Find ("act1"), GameObject.Find ("act2"), GameObject.Find ("act3"), GameObject.Find ("act4") };
	}

	protected bool isFirstFinished() {
		bool val = isFirst;
		isFirst = false;
		return val;
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	protected Node ST_ApproachAndWait(GameObject daniel, Transform target)
	{
		Val<Vector3> position = Val.V (() => target.position);
		return new Sequence( daniel.GetComponent<BehaviorMecanim>().Node_GoTo(position), new LeafWait(1000));
	}

	protected Node ST_ApproachAndWaitDemonFire(GameObject daniel)
	{
		Val<Vector3> position = Val.V (() => demonFire.transform.position);
		return new Sequence( daniel.GetComponent<BehaviorMecanim>().Node_GoTo(position), daniel.GetComponent<BehaviorMecanim>().Node_Disappear());
	}

	protected Node Squat(GameObject daniel) {
		BehaviorMecanim behavior = daniel.GetComponent<BehaviorMecanim> ();
		return new Sequence (behavior.Node_SquatDown (), behavior.Node_SquatUp ());
	}

	protected Node UserPunch(GameObject daniel) {
		BehaviorMecanim behavior = daniel.GetComponent<BehaviorMecanim> ();
		return behavior.Node_UserPunch (punchee);
	}

	protected Node Punch(GameObject from, GameObject to) {
		BehaviorMecanim behavior = from.GetComponent<BehaviorMecanim> ();
		return behavior.Node_Punch (to);
	}

	protected Node BecomeCrab(GameObject daniel) {
		BehaviorMecanim behavior = daniel.GetComponent<BehaviorMecanim> ();
		return behavior.Node_BecomeCrab ();
	}

	protected Node LookAt(GameObject from, GameObject to) {
		BehaviorMecanim behavior = from.GetComponent<BehaviorMecanim> ();
		return behavior.Node_OrientTowards (to.transform.position + new Vector3(0,2,0));
	}

	protected bool allCrabs() {
		foreach(GameObject daniel in daniels) {
			if (!daniel.GetComponent<IKController>().IsCrab ()) {
				return false;
			}
		}
		return true;
	}

	protected Node SetCamera(int index) {
		return new LeafInvoke (() => {
			this.GetComponent<CameraController> ().SetCamera (index);
			return RunStatus.Success;
		},
		() => {});
	}

	protected Node Quote(string message) {
		return new LeafInvoke (() => {
			GameObject.Find ("quote").GetComponent<Text> ().text = "\"" + message + "\"";
		}, () => {});
	}

	protected Node ShowAct(int index) {
		return new LeafInvoke(() => {
			for (int i = 0; i < acts.Length; i++) acts[i].SetActive(false);
			acts[index].SetActive(true);
		});
	}

	protected Node CheckFirstGuy(GameObject daniel) {
		return new IfThenElse (
			new LeafInvoke (() => {
			if (isFirstFinished ())
				return RunStatus.Success;
			else
				return RunStatus.Failure;
			}, () => {}),
			new Sequence(
				new LeafInvoke (() => {
					daniel.transform.localScale = new Vector3(4,4,4);
					return RunStatus.Success;
				}, () => {}), 
				this.ST_ApproachAndWait (daniel, this.wander1)
			),
			this.ST_ApproachAndWait (daniel, this.wander2)
		);
	}
		
	protected Node BuildTreeRoot()
	{
		daniels = GameObject.FindGameObjectsWithTag ("Daniel");

		Sequence beginStory = new Sequence (
			this.SetCamera(0),
			this.ShowAct(0),
			new SequenceParallel (
				this.ST_ApproachAndWait (benchGuy1, this.wander2),
				this.ST_ApproachAndWait (benchGuy2, this.wander2)
			),
			this.LookAt (benchGuy2, benchGuy1),
			this.Quote ("Hey man, I wanted to use that."),
			new LeafWait(2000),
			this.Quote ("Pssh, get out of my way."),
			new LeafWait(2000),
			this.Quote ("Unfortunately for you, I have inverse kinematic punches enabled."),
			new DecoratorLoop(3, new Sequence(
				this.Punch (benchGuy1, benchGuy2),
				this.Punch (benchGuy2, benchGuy1)
		    )),
			this.Quote ("Yeah that's what I thought."),
			this.ST_ApproachAndWait (benchGuy1, this.wander1)
		);

		ForEach<GameObject> middleStory = new ForEach<GameObject> (
			(daniel) => {
			return new Sequence (
				this.ShowAct(1),
				this.SetCamera(2),
				this.Quote ("Now it's time to work out!"),
				new DecoratorLoop(6, new SequenceShuffle(
					this.Squat (daniel),
					this.Punch (daniel, punchee)
				)),
				new LeafWait(1000),
				this.BecomeCrab (daniel),
				new LeafWait(1000)
				);
		}
		, daniels);


		ForEach<GameObject> endStory = new ForEach<GameObject> (
			(daniel) => {
			return new Sequence(
				this.ShowAct(2),
				this.SetCamera (1),
				this.Quote ("The time has come to choose a new Crab God."),
				new LeafWait(1000),
				this.ST_ApproachAndWaitDemonFire(daniel),
				this.CheckFirstGuy(daniel),
				this.Quote ("HAHA! I am the new Crab God now, bow to me lesser beings!"),
				new LeafWait(1000),
				this.ShowAct(3)
				);
		}, daniels );

		return new Sequence(beginStory, middleStory, endStory);
	}
}
