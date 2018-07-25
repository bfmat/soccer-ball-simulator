//using UnityEngine;
//using System.Collections;

//[RequireComponent(typeof (NavMeshAgent))]

/*
public class SoccerPlayerAIOriginal : MonoBehaviour
{
    private Animator thisAnimator;
    private Rigidbody thisRigidbody;
    private Vector3 velocity;
    private GameObject player;
	private Ball ball;

	public NavMeshAgent agent { get; private set; }             // the navmesh agent required for the path finding
	public ThirdPersonCharacter character { get; private set; } // the character we are controlling
	public Transform target;

    // Use this for initialization
    void Start()
    {
		player = GameObject.FindGameObjectWithTag ("Player");
		ball = player.GetComponent<Ball> ();

        thisAnimator = GetComponent<Animator>();
        thisRigidbody = GetComponent<Rigidbody>();
        thisRigidbody.isKinematic = true;

		agent = GetComponentInChildren<NavMeshAgent>();
		character = GetComponent<ThirdPersonCharacter>();

		agent.updateRotation = false;
		agent.updatePosition = true;
    }

	void FixedUpdate(){
		if (character == null)
			return;
		
		if (Vector3.Distance (player.transform.position, transform.position) <= 0.7f) {
			if (!ball.whackEnemy)
				GameManager.S.ReEnableWithDelay (1f, gameObject);
			character.enabled = false;
			enabled = false;
			return;
		}

		if (target != null)
			agent.SetDestination(target.position);

		if (agent.remainingDistance > agent.stoppingDistance)
			character.Move(agent.desiredVelocity, false, false);
		else
			character.Move(Vector3.zero, false, false);
	}
}
*/