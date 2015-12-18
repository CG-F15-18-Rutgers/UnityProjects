using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TreeSharpPlus;

public class MyBehaviorTree : MonoBehaviour
{
	public Transform wander1;
	public Transform wander2;
	public Transform wander3;
    public GameObject participant1;
	public GameObject participant2;
    public GameObject participant3;
    public GameObject opponent;
	public GameObject fightCanvas;

	public Text fightStatus;
	bool isFighting = false;

	private BehaviorAgent behaviorAgent;
	// Use this for initialization
	void Start ()
	{
		behaviorAgent = new BehaviorAgent (BuildTreeRoot());
		BehaviorManager.Instance.Register (behaviorAgent);
		behaviorAgent.StartBehavior ();
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void SetActiveCamera() {
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
                return Applaud(clapper);
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
					new DecoratorLoop(AutoFightSequence(fighter))
				),
				new Sequence (
					Cry (fighter),
					ST_ApproachAndWait(fighter, wander1.transform.position, 1000)
				)
			)
		));
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

	protected Node Prefight(GameObject fighter) {
		return new DecoratorLoop (1,
			new SelectorShuffle(
				new SelectorShuffle(
					ST_ApproachAndWait(fighter, wander1.transform.position, 1000),
					ST_ApproachAndWait(fighter, wander2.transform.position, 1000),
					ST_ApproachAndWait(fighter, wander3.transform.position, 1000)
				),
				PowerUp(fighter),
				Defend(fighter)
			)
		);
	}
	protected Node BuildTreeRoot()
	{
        DecoratorLoop applaud =
            new DecoratorLoop(
                AudienceApplaud());

        SequenceParallel start = new SequenceParallel(
			ST_ApproachAndWait(participant1, wander1.transform.position, 1000),
			ST_ApproachAndWait(participant2, wander1.transform.position, 1000));	

		return new SequenceParallel (
			new SequenceParallel (
				new Sequence (Prefight (participant1), ApproachFight (participant1)),
				new Sequence (Prefight (participant2), ApproachFight (participant2)),
				new Sequence (Prefight (participant3), ApproachFight (participant3))
			),
			new DecoratorLoop (applaud)
		);
	}
}
