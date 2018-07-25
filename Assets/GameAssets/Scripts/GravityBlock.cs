using UnityEngine;

public class GravityBlock : MonoBehaviour {

    private float gravityStrength;
    private float maxDistanceForEffect;
    private GameObject player;
    private Rigidbody playerRB;
    private ParticleSystem particleSys;

    void Start()
    {
        gravityStrength = (Random.value - 0.5f) * 50;
        maxDistanceForEffect = Random.value * 50;
        player = GameObject.FindGameObjectWithTag("Player");
        playerRB = player.GetComponent<Rigidbody>();
        particleSys = GetComponent<ParticleSystem>();
        particleSys.startSpeed = -gravityStrength;
        particleSys.startLifetime = maxDistanceForEffect / Mathf.Abs(gravityStrength);
        ParticleSystem.ShapeModule shape = particleSys.shape;
        if (gravityStrength > 0) shape.radius = maxDistanceForEffect;
        else shape.radius = 0.1f;
    }

    void FixedUpdate()
    {
        Vector3 delta = transform.position - player.transform.position;
        float dist = delta.magnitude;
        if (dist > maxDistanceForEffect) return;
        delta.Normalize();
        Vector3 gravity = (delta * gravityStrength * 10) / (dist * dist);
        playerRB.AddForce(gravity, ForceMode.Acceleration);
    }
}
