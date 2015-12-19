using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TreeSharpPlus;

public class Part3BehaviorTree : MonoBehaviour
{
	public Transform wander1;
	public Transform wander2;
	public Transform wander3;
	public GameObject participant1;
	public GameObject participant2;
	public GameObject participant3;
	public GameObject opponent;
	public GameObject fightCanvas;
	public GameObject manualPreFightCanvas;
	public GameObject manualFightCanvas;
	public GameObject audienceCanvas;
	public GameObject tabCanvas;

	public GameObject you;

	public Text fightStatus;
	bool isFighting = false;
	bool switchingAllowed = true;

	private BehaviorAgent behaviorAgent;
	// Use this for initialization
	void Start ()
	{
		behaviorAgent = new BehaviorAgent (BuildTreeRoot());
		BehaviorManager.Instance.Register (behaviorAgent);
		behaviorAgent.StartBehavior ();
	}

	void ShowInstructions(GameObject instructions) {
		manualPreFightCanvas.SetActive(false);
		manualFightCanvas.SetActive (false);
		audienceCanvas.SetActive (false);
		if (instructions != null) instructions.SetActive (true);
	}

	// Update is called once per frame
	void Update ()
	{
		you.transform.Rotate (0, Input.GetAxis ("Horizontal") * 2, 0);
		if (Input.GetMouseButtonDown(0)) 
		{
			Debug.Log (Camera.main);
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray,out hit))
			{
				you.GetComponent<BodyMecanim> ().NavGoTo (hit.point);
			}
		}

		GameObject[] clappers = GameObject.FindGameObjectsWithTag("Audience");
		if (Input.GetKeyDown(KeyCode.Tab) && switchingAllowed) {
			if (you == participant1)
				SwitchTo (participant2);
			else if (you == participant2)
				SwitchTo (participant3);
			else if (you == participant3)
				SwitchTo (clappers [0]);
			else {
				for (int i = 0; i < clappers.Length; i++) {
					if (clappers [i] == you) {
						if (i < clappers.Length	 - 1)
							SwitchTo (clappers [i + 1]);
						else
							SwitchTo (participant1);
						break;
					}
				}
			}
		}
	}

	void SwitchTo(GameObject person) {
		Debug.Log ("Switching");
		you.transform.Find ("Camera").gameObject.SetActive (false);
		person.transform.Find ("Camera").gameObject.SetActive (true);
		you = person;
	}

	void toggleSwitching(bool enabled) {
		switchingAllowed = enabled;
		tabCanvas.SetActive (switchingAllowed);
	}
	protected void beginFight(GameObject fighter) {
		opponent.GetComponent<Fighter> ().UpdateStats();
		fighter.GetComponent<Fighter> ().UpdateStats();
		fightCanvas.SetActive (true);
	}

	private void displayFightStatus(string text) {
		fightStatus.text = text;
	}

	private void displayFightStatus(GameObject fighter, string text) {		
		fightStatus.text = fighter.GetComponent<Fighter> ().name + " " + text;
	}

	protected Node ST_ApproachAndWait(GameObject participant, Vector3 target, long waitTime)
	{
		return new Sequence( participant.GetComponent<BehaviorMecanim>().Node_GoTo(target), new LeafWait(Val.V (() => waitTime)));
	}

	protected Node Applaud(GameObject clapper)
	{
		Val<string> gestureName = Val.V (() => "CLAP");
		return new SequenceShuffle(
			clapper.GetComponent<BehaviorMecanim>().ST_PlayHandGesture("CLAP", 1000),
			clapper.GetComponent<BehaviorMecanim>().ST_PlayHandGesture("HANDSUP", 1000),
			clapper.GetComponent<BehaviorMecanim>().ST_PlayHandGesture("WAVE", 1000)
		);
	}

	protected Node ManualApplaud(GameObject clapper)
	{
		return new DecoratorForceStatus(RunStatus.Success,
			new Sequence(
				new LeafInvoke( () => { ShowInstructions(audienceCanvas); }),
				new Selector(
					new Sequence(new LeafAssert(() => {return Input.GetKey(KeyCode.C);}), clapper.GetComponent<BehaviorMecanim>().ST_PlayHandGesture("CLAP", 1000)),
					new Sequence(new LeafAssert(() => {return Input.GetKey(KeyCode.H);}), clapper.GetComponent<BehaviorMecanim>().ST_PlayHandGesture("HANDSUP", 1000)),
					new Sequence(new LeafAssert(() => {return Input.GetKey(KeyCode.W);}), clapper.GetComponent<BehaviorMecanim>().ST_PlayHandGesture("WAVE", 1000))
				)
			)
		);
	}

	protected Node Attack(GameObject p1, GameObject p2) {
		Fighter f1 = p1.GetComponent<Fighter> ();
		Fighter f2 = p2.GetComponent<Fighter> ();
		return new Sequence(
			f1.GetComponent<BehaviorMecanim> ().ST_TurnToFace(f2.transform.position),
			f1.GetComponent<BehaviorMecanim> ().ST_PlayBodyGesture ("KICK", 4000),
			new LeafInvoke( () => {
				if (Random.value < f1.getHitProb()) {
					displayFightStatus(p2, "has been hit!");
					f2.hit(f1.getStrength());
				} else {
					displayFightStatus("The attack missed.");
				}
			})
		);
	}

	protected Node Defend(GameObject defender) {
		return new Sequence (
			defender.GetComponent<BehaviorMecanim> ().ST_PlayBodyGesture ("DUCK", 1000),
			new LeafInvoke (
				() => {
					defender.GetComponent<Fighter> ().defend ();
					displayFightStatus (defender, "has defended.");
				}
			)
		);
	}

	protected Node Die (GameObject player) {
		return player.GetComponent<BehaviorMecanim> ().ST_PlayBodyGesture ("DYING", 1000);
	}

	protected Node Cry(GameObject player) {
		return player.GetComponent<BehaviorMecanim> ().ST_PlayHandGesture ("HANDSUP", 1000);
	}

	protected Node AudienceApplaud()
	{
		GameObject[] clappers = GameObject.FindGameObjectsWithTag("Audience");
		return new ForEach<GameObject>(
			(clapper) =>
			{
				return new DecoratorLoop(
					new Selector(
						new Sequence( 
							new LeafAssert( () => { return you == clapper; }),
							ManualApplaud(clapper)
						),
						Applaud(clapper)
					)
				);
			}
			, clappers);
	}

	protected Node PowerUp(GameObject fighter)
	{
		return new Sequence (
			fighter.GetComponent<BehaviorMecanim> ().ST_PlayFaceGesture (Val.V (() => "ROAR"), 2000),
			new LeafInvoke (() => {
				fighter.transform.Find ("Particle System").gameObject.SetActive (true);
			}),
			new LeafInvoke (() => { 
				if (fighter.transform.localScale.y >= 5)
					return;
				fighter.transform.localScale *= 1.1f;
				fighter.GetComponent<Fighter> ().powerUp();
				displayFightStatus(fighter, "has powered up!");
			}),
			fighter.GetComponent<BehaviorMecanim> ().ST_PlayBodyGesture ("FIGHT", 2000)
		);
	}

	protected Node ApproachFight(GameObject fighter) {
		return new DecoratorForceStatus(RunStatus.Success, new Sequence (
			ST_ApproachAndWait (fighter, opponent.transform.position + new Vector3 (-5, 0, 0), 0),
			new Selector (
				new Sequence (
					new LeafAssert (() => {
						return !isFighting;
					}),
					new LeafInvoke(
						() => { isFighting = true; }
					),
					ST_ApproachAndWait (fighter, opponent.transform.position + new Vector3 (-1, 0, 0), 500),
					new LeafInvoke (() => {
						beginFight(fighter);
					}),
					new DecoratorLoop(FightSequence(fighter))
				),
				new Sequence (
					Cry (fighter),
					ST_ApproachAndWait(fighter, wander1.transform.position, 1000)
				)
			)
		));
	}

	protected Node FightSequence(GameObject fighter) {
		return new DecoratorLoop (
			new Selector(
				new Sequence(
					new LeafAssert( () => {
						return you != fighter;
					}),
					AutoFightSequence(fighter)
				),
				ManualFightSequence(fighter)
			)
		);
	}

	protected Node ManualFightSequence(GameObject fighter) {
		string choice = "";
		return new Sequence (
			new LeafInvoke( () => { 
				ShowInstructions(manualFightCanvas);
				toggleSwitching(false);
			} ),
			new DecoratorForceStatus(RunStatus.Success,
				new DecoratorLoop(
					new Sequence(
						new LeafAssert(() => { 
							if(!Input.GetKey(KeyCode.F) && !Input.GetKey(KeyCode.P) && !Input.GetKey(KeyCode.D)) {
								return true;
							} else if(Input.GetKey(KeyCode.F)) {
								choice = "fight";
							} else if(Input.GetKey(KeyCode.P)) {
								choice = "powerup";
							} else if (Input.GetKey(KeyCode.D)) {
								choice = "defend";
							}
							return false;
						})
					)
				)
			),
			new LeafInvoke( () => { 
				toggleSwitching(true);
			} ),
			new Selector(
				new Sequence(new LeafAssert(() => { return choice == "fight"; }), Attack(fighter, opponent)),
				new Sequence(new LeafAssert(() => { return choice == "defend"; }), Defend(fighter)),
				new Sequence(new LeafAssert(() => { return choice == "powerup"; }), PowerUp(fighter))
			),
			new LeafInvoke( () => { ShowInstructions(null); } ),
			new SelectorShuffle (
				PowerUp (opponent),
				Attack (opponent, fighter),
				Attack (opponent, fighter),
				Defend (opponent)
			),
			new LeafAssert (() => {
				if (fighter.GetComponent<Fighter>().isDead()) {
					displayFightStatus(opponent, "has won");
					return false;
				} else if (opponent.GetComponent<Fighter>().isDead()){
					displayFightStatus(fighter, "has won");
					return false;
				}
				return true;
			})
		);
	}

	protected Node AutoFightSequence(GameObject fighter) {
		return new Sequence (
			new SelectorShuffle (
				PowerUp (fighter),
				Attack (fighter, opponent),
				Attack (fighter, opponent),
				Defend (fighter)
			),
			new SelectorShuffle (
				PowerUp (opponent),
				Attack (opponent, fighter),
				Attack (opponent, fighter),
				Defend (opponent)
			),
			new LeafAssert (() => {
				if (fighter.GetComponent<Fighter>().isDead()) {
					displayFightStatus(opponent, "has won");
					return false;
				} else if (opponent.GetComponent<Fighter>().isDead()){
					displayFightStatus(fighter, "has won");
					return false;
				}
				return true;
			})
		);
	}

	protected Node PrefightAuto(GameObject fighter) {
		return new Sequence (
			new LeafAssert (() => {
				return you != fighter;
			}),
			new SelectorShuffle (
				Applaud(fighter),
				Applaud(fighter),
				PowerUp (fighter),
				Defend (fighter)
			)
		);
	}

	protected Node Prefight(GameObject fighter) {
		return new DecoratorLoop (5,
			new Selector(
				new Sequence(
					new LeafAssert( () => {
						return you != fighter;
					}),
					PrefightAuto(fighter)
				),
				PrefightManual(fighter),
				new LeafAssert( () => { return true; })
			)
		);
	}

	protected Node PrefightManual(GameObject player) {
		return new DecoratorLoop (
			new Sequence(
				new LeafAssert( () => {
					return you == player;
				}),
				new DecoratorForceStatus(RunStatus.Success,
					new Sequence(
						new LeafInvoke( () => { ShowInstructions(manualPreFightCanvas); } ),
						new LeafAssert( () => {
							return Input.GetKey(KeyCode.A);
						}),
						ApproachFight(player)
					)
				)
			)
		);
	}

	protected Node BuildTreeRoot()
	{
		DecoratorLoop applaud =
			new DecoratorLoop(
				AudienceApplaud());

		return new SequenceParallel (
			new SequenceParallel (
				new Sequence (Prefight (participant1), ApproachFight (participant1)),
				new Sequence (Prefight (participant2), ApproachFight (participant2)),
				new Sequence (Prefight (participant3), ApproachFight (participant3))
			),
			applaud
		);
	}
}
