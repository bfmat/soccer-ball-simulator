using UnityEngine;
using System.Collections;

public class DisappearingBlock : MonoBehaviour {

    private float shrinkPerFixedUpdate = 0.01f;

    void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            StartCoroutine(ShrinkAndDisappear());
        }
    }

    public IEnumerator ShrinkAndDisappear()
    {
        while (true)
        {
            transform.localScale -= new Vector3(shrinkPerFixedUpdate, shrinkPerFixedUpdate, shrinkPerFixedUpdate);
            yield return new WaitForFixedUpdate();
            if (transform.localScale.x <= 0)
                gameObject.SetActive(false);
        }
    }
}
