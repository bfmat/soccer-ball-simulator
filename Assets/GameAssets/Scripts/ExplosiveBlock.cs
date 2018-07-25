using UnityEngine;
using System.Collections.Generic;

public class ExplosiveBlock : MonoBehaviour {

    [SerializeField] [Range(1, 50)] private int blastRadius;
    private List<GameObject> blocksNearby;
    private int blastRadiusSqr;

    void Start()
    {
        GameObject[] blocks = GameObject.FindGameObjectsWithTag("EnvironmentBlock");
        blocksNearby = new List<GameObject>();
        blastRadiusSqr = blastRadius * blastRadius;
        foreach (GameObject block in blocks)
        {
            if ((transform.position - block.transform.position).sqrMagnitude < blastRadiusSqr) blocksNearby.Add(block);
        }
    }

    void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            Explode();
        }
    }

    private void Explode()
    {
        GetComponent<ParticleSystem>().Play();
        GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>().AddForce(new Vector3(0f, 50f, 0f), ForceMode.VelocityChange);
        for (int i = 0; i < blocksNearby.Count; i++)
        {
            try
            {
                blocksNearby[i].transform.SetParent(null);
                Rigidbody r = blocksNearby[i].AddComponent<Rigidbody>();
                r.mass = 1f;
                r.useGravity = true;
                r.AddExplosionForce(70f, transform.position, 50f, 0f, ForceMode.VelocityChange);
            } catch { }
            
        }
        Destroy(GetComponent<MeshRenderer>());
        Destroy(GetComponent<BoxCollider>());
        Destroy(this);
    }
}
