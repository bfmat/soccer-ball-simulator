using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Ball : MonoBehaviour
{

    [SerializeField] private GameObject youWinUI, coinsUI;
	private float m_MovePower = 20;
	private const float m_MaxAngularVelocity = 25;
	private float m_JumpPower = 4;
	private const float k_GroundRayLength = 1f;
	private Rigidbody m_Rigidbody;
	private List<GameObject> balls;
	private SphereCollider spColl;
	private BoxCollider boxColl;
    private bool levitating = false;
    private bool bomb = false;
    private bool respawnDisabled = false;
    private bool useTorque = false;
    private bool usingTorque = false;
    private BallUserControl controls;
    private int points = 0;
    private Text coinsUIText;

	private void Start ()
	{
		m_Rigidbody = GetComponent<Rigidbody> ();
		GetComponent<Rigidbody> ().maxAngularVelocity = m_MaxAngularVelocity;
		balls = new List<GameObject> ();
		foreach (Transform t in transform) {
			balls.Add (t.gameObject);
		}
		spColl = GetComponent<SphereCollider> ();
		boxColl = GetComponent<BoxCollider> ();
        controls = GetComponent<BallUserControl>();
        coinsUIText = coinsUI.GetComponent<Text>();
        coinsUIText.text = "Coins: " + points + " / " + ProcGen.totalCoins;
    }

	void FixedUpdate() {
        if (levitating)
        {
            RaycastHit hit;
            float distanceToGround = float.MaxValue;
            if (Physics.Raycast(new Ray(transform.position, Vector3.down), out hit))
            {
                distanceToGround = hit.distance;
            }
            m_Rigidbody.AddForce((Vector3.up * 20) / distanceToGround);
        }
        
		if (transform.position.y < -10) {
            Respawn();
		}

        if (Vector3.Distance(ProcGen.winLocation, transform.position) < 3f)
        {
            Win();
        }
	}

	public void Move (Vector3 moveDirection, bool jump)
	{
        if (usingTorque)
        {
            m_Rigidbody.AddTorque(new Vector3(moveDirection.z, 0, -moveDirection.x) * m_MovePower);
            m_Rigidbody.AddForce(moveDirection * m_MovePower);
        }
        else
        {
            m_Rigidbody.AddForce(moveDirection * m_MovePower);
        }

        if (Physics.Raycast (transform.position, -Vector3.up, k_GroundRayLength) && jump && !levitating) {
			m_Rigidbody.AddForce (Vector3.up * m_JumpPower, ForceMode.Impulse);
		}

        if (levitating && jump)
        {
            m_Rigidbody.AddForce(Vector3.up * 8);
        }
	}

	public void GetPowerUp (PowerUpType powerUp) {
        string powerUpName = PowerUp.GetPowerUpNameWithIdentifier(powerUp);
		foreach (GameObject b in balls) {
			if (b.name == powerUpName) {
				b.SetActive (true);
                switch (powerUp)
                {
                    case PowerUpType.FootBall:
                        isBox = false;
                        m_MovePower = 20;
                        levitating = false;
                        bomb = false;
                        m_Rigidbody.useGravity = true;
                        useTorque = false;
                        m_JumpPower = 4;
                        break;

                    case PowerUpType.AtomBall:
                        isBox = false;
                        m_MovePower = 15;
                        levitating = true;
                        bomb = false;
                        m_Rigidbody.useGravity = true;
                        useTorque = false;
                        m_JumpPower = 4;
                        break;

                    case PowerUpType.BombBall:
                        isBox = false;
                        m_MovePower = 15;
                        levitating = false;
                        bomb = true;
                        m_Rigidbody.useGravity = true;
                        useTorque = false;
                        m_JumpPower = 4;
                        break;

                    case PowerUpType.BuckyBall:
                        isBox = false;
                        m_MovePower = 30;
                        levitating = false;
                        bomb = false;
                        m_Rigidbody.useGravity = false;
                        Invoke("GetFootBallPowerUp", 10f);
                        useTorque = false;
                        m_JumpPower = 4;
                        break;

                    case PowerUpType.SpikeBall:
                        isBox = false;
                        m_MovePower = 25;
                        levitating = false;
                        bomb = false;
                        m_Rigidbody.useGravity = true;
                        useTorque = true;
                        m_JumpPower = 4;
                        break;

                    case PowerUpType.SplitMetalBall:
                        isBox = false;
                        m_MovePower = 15;
                        levitating = false;
                        bomb = false;
                        m_Rigidbody.useGravity = true;
                        useTorque = false;
                        m_JumpPower = 10;
                        break;

                    case PowerUpType.BoxSmall:
                        isBox = true;
                        m_MovePower = 15;
                        levitating = false;
                        bomb = false;
                        m_Rigidbody.useGravity = true;
                        useTorque = false;
                        m_JumpPower = 14;
                        break;
                }
			} else {
				b.SetActive (false);
			}
		}
	}

    public void Respawn()
    {
        if (respawnDisabled && controls.enabled) return;
        if (bomb && controls.enabled) {
            Explode();
            respawnDisabled = true;
            Invoke("ReEnableRespawn", 3f);
            GetPowerUp(PowerUpType.FootBall);
            return;
        }
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnCollisionEnter(Collision coll)
    {
        if (controls.enabled == false)
        {
            controls.enabled = true;
        }
    }

    void OnCollisionStay(Collision coll)
    {
        if (coll.gameObject.tag == "EnvironmentBlock" || coll.gameObject.tag == "DisappearingBlock")
        {
            transform.SetParent(coll.gameObject.transform, true);
            if (coll.gameObject.tag == "DisappearingBlock")
            {
                transform.localScale = new Vector3(1 / transform.parent.localScale.x, 1 / transform.parent.localScale.y, 1 / transform.parent.localScale.z);
            }
        }
        if (useTorque) usingTorque = true;
        else usingTorque = false;
    }

    void OnCollisionExit(Collision coll)
    {
        transform.SetParent(null, true);
        usingTorque = false;
    }

    private bool isBox
    {
        set
        {
            boxColl.enabled = value;
            spColl.enabled = !value;
        }
    }

    private void ReEnableRespawn()
    {
        respawnDisabled = false;
    }

    private void GetFootBallPowerUp()
    {
        GetPowerUp(PowerUpType.FootBall);
    }

    public void Explode()
    {
        GameObject[] blocks = GameObject.FindGameObjectsWithTag("EnvironmentBlock");
        List<GameObject> blocksNearby = new List<GameObject>();
        float blastRadiusSqr = 2500;
        foreach (GameObject block in blocks)
        {
            if ((transform.position - block.transform.position).sqrMagnitude < blastRadiusSqr) blocksNearby.Add(block);
        }
        GetComponent<ParticleSystem>().Play();
        GetComponent<Rigidbody>().AddForce(new Vector3(0f, 50f, 0f), ForceMode.VelocityChange);
        for (int i = 0; i < blocksNearby.Count; i++)
        {
            try
            {
                blocksNearby[i].transform.SetParent(null);
                Rigidbody r = blocksNearby[i].AddComponent<Rigidbody>();
                r.mass = 1f;
                r.useGravity = true;
                r.AddExplosionForce(70f, transform.position, 50f, 0f, ForceMode.VelocityChange);
            }
            catch { }
        }
    }

    private void Win()
    {
        m_Rigidbody.useGravity = false;
        m_Rigidbody.velocity = Vector3.zero;
        m_Rigidbody.drag = 0f;
        m_Rigidbody.angularDrag = 0f;
        m_Rigidbody.AddForce(new Vector3(0f, 500f, 0f), ForceMode.Force);
        youWinUI.SetActive(true);
        controls.enabled = false;
        PlayerPrefs.DeleteKey(ProcGen.seed);
        Invoke("LoadMainMenu", 5f);
    }

    public void CollectCoin()
    {
        points++;
        coinsUIText.text = "Coins: " + points + " / " + ProcGen.totalCoins;
    }

    private void LoadMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}