using UnityEngine;
using System.Collections;

public class DeathBlock : MonoBehaviour {

	void OnCollisionEnter (Collision coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            coll.gameObject.GetComponent<Ball>().Respawn();
        }
    }
}
