using UnityEngine;
using System.Collections;

public class ExplosiveFlyingEnemy : MonoBehaviour {

    private GameObject player;
    private Rigidbody rig;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rig = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        transform.LookAt(player.transform);
        rig.AddRelativeForce(Vector3.forward * 5);
        if (Vector3.Distance(transform.position, player.transform.position) < 3f)
        {
            player.GetComponent<Ball>().Explode();
            Destroy(gameObject);
        }
    }
}
