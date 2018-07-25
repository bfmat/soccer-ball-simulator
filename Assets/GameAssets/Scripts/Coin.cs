using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour {

	void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<Ball>().CollectCoin();
            Destroy(gameObject);
        }
    }
}
