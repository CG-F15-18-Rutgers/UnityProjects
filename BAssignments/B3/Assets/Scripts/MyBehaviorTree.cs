using UnityEngine;
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

	protected Node ST_ApproachAndWait(GameObject participant, Transform target, long waitTime)
	{
		Val<Vector3> position = Val.V (() => target.position);
		return new Sequence( participant.GetComponent<BehaviorMecanim>().Node_GoTo(position), new LeafWait(Val.V (() => waitTime)));
	}

    protected Node Applaud(GameObject clapper)
    {
        Val<string> gestureName = Val.V (() => "CLAP");
        return clapper.GetComponent<BehaviorMecanim>().ST_PlayHandGesture(gestureName, 1000);
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
        return new Sequence(
            fighter.GetComponent<BehaviorMecanim>().ST_PlayFaceGesture(Val.V(() => "ROAR"), 2000),
            new LeafInvoke (() => { fighter.transform.Find("Particle System").gameObject.SetActive(true); }),
            fighter.GetComponent<BehaviorMecanim>().Node_BodyAnimation(Val.V (() => "FIGHT"), Val.V (() => true)));
    }

	protected Node BuildTreeRoot()
	{
		DecoratorLoop original = 
			new DecoratorLoop(
				new SequenceShuffle(
                    ST_ApproachAndWait(participant3, wander1, 1000),
                    ST_ApproachAndWait(participant3, wander2, 1000),
                    ST_ApproachAndWait(participant3, wander3, 1000)));

        DecoratorLoop applaud =
            new DecoratorLoop(
                AudienceApplaud());

        SequenceParallel start = new SequenceParallel(
            ST_ApproachAndWait(participant1, wander1, 1000),
            ST_ApproachAndWait(participant2, wander1, 1000));

        return new SequenceParallel(
            PowerUp(opponent), original, applaud
            );
	}
}
