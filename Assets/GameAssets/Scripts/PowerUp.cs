using UnityEngine;
using System.Collections;

public class PowerUp : MonoBehaviour {
	public PowerUpType powerUp;
	private const float maxRotationSpeedOnSingleAxis = 5f;
	private const float shrinkPerFixedUpdate = 0.01f;
	private Rigidbody rig;

	void Start() {
		rig = GetComponent<Rigidbody> ();
		rig.angularVelocity = new Vector3 (RandomRotationSpeedOnAxis (), RandomRotationSpeedOnAxis (), RandomRotationSpeedOnAxis ());
		GameObject ball = Instantiate (GetPowerUpWithName(GetPowerUpNameWithIdentifier(powerUp)));
		ball.SetActive (true);
		ball.transform.parent = transform;
		ball.transform.position = transform.position;
		ball.transform.localScale = Vector3.one;
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			other.GetComponent<Ball> ().GetPowerUp (powerUp);
			StartCoroutine (ShrinkAndDisappear ());
		}
	}

	private float RandomRotationSpeedOnAxis() {
		float rand = (Random.value - 0.5f) * 2;
		return rand * maxRotationSpeedOnSingleAxis;
	}

	private IEnumerator ShrinkAndDisappear() {
		while (true) {
			transform.localScale -= new Vector3 (shrinkPerFixedUpdate, shrinkPerFixedUpdate, shrinkPerFixedUpdate);
			yield return new WaitForFixedUpdate ();
			if (transform.localScale.x <= 0)
				Destroy (gameObject);
		}
	}

    

    private GameObject GetPowerUpWithName (string name)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        foreach (Transform t in player.transform)
        {
            if (t.name == name) return t.gameObject;
        }
        return null;
    }

    public static string GetPowerUpNameWithIdentifier(PowerUpType identifier)
    {
        string name = "";
        switch (identifier)
        {
            case PowerUpType.FootBall: name = "FootBall"; break;
            case PowerUpType.AtomBall: name = "AtomBall"; break;
            case PowerUpType.BombBall: name = "BombBall"; break;
            case PowerUpType.BuckyBall: name = "BuckyBall"; break;
            case PowerUpType.SpikeBall: name = "SpikeBall"; break;
            case PowerUpType.SplitMetalBall: name = "SplitMetalBall"; break;
            case PowerUpType.BoxSmall: name = "BoxSmall"; break;
        }
        return name;
    }
}

public enum PowerUpType
{
    FootBall,
    AtomBall,
    BombBall,
    BuckyBall,
    SpikeBall,
    SplitMetalBall,
    BoxSmall
}