using UnityEngine;

public class Turret : MonoBehaviour {
    [SerializeField]
    private GameObject bullet;
    private GameObject player;
    private float shotDelay;
    private float speed;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        speed = Random.value * 150f;
        InvokeRepeating("Fire", Random.value * 10f, Random.value * 10f);
    }

    void FixedUpdate()
    {
        transform.LookAt(player.transform);
    }

    private void Fire()
    {
        Vector3 velocity = (player.transform.position - transform.position).normalized * speed;
        GameObject firedBullet = Instantiate(bullet);
        firedBullet.transform.position = transform.position;
        Rigidbody bulletRigidbody = firedBullet.GetComponent<Rigidbody>();
        bulletRigidbody.AddForce(velocity, ForceMode.VelocityChange);
    }
}
