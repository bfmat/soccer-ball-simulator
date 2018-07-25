using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DynamicEnvironmentElement : MonoBehaviour {

    public Vector3 rotPerSecond;
    public List<Vector3> translationWaypoints;
    public float waypointCyclingVelocity;
    public float requiredWaypointAccuracy;
    private Vector3 _velocity;

    void Start() {
        ConfigureCenterOfRotation();
        if (translationWaypoints.Count > 0)
        {
            translationWaypoints.Insert(0, transform.position);
            StartCoroutine(CycleThroughWaypoints());
        }
        StartCoroutine(Rotate(Quaternion.Euler(rotPerSecond * Time.fixedDeltaTime)));
    }

    void FixedUpdate()
    {
        transform.position += _velocity * Time.fixedDeltaTime;
    }

    private IEnumerator Rotate(Quaternion rotPerTick) {
        while (enabled) {
            transform.rotation *= rotPerTick;
            yield return new WaitForFixedUpdate();
        }
    }

    private Vector3 AverageVector3(List<Vector3> vector3s) {
        Vector3 total = Vector3.zero;
        foreach (Vector3 v in vector3s) {
            total += v;
        }
        return total / vector3s.Count;
    }

    private void ConfigureCenterOfRotation()
    {
        List<Vector3> positions = new List<Vector3>();
        List<GameObject> objects = new List<GameObject>();
        foreach (Transform t in transform)
        {
            objects.Add(t.gameObject);
        }
        foreach (GameObject g in objects)
        {
            g.transform.SetParent(null, true);
            positions.Add(g.transform.position);
        }
        transform.position = AverageVector3(positions);
        foreach (GameObject g in objects)
        {
            g.transform.SetParent(transform, true);
        }
    }

    private IEnumerator CycleThroughWaypoints()
    {
        ushort targetWaypoint = 0;
        bool cyclingForward = true;
        while (enabled)
        {
            if (Vector3.Distance(transform.position, translationWaypoints[targetWaypoint]) < requiredWaypointAccuracy)
            {
                if (cyclingForward)
                {
                    if (targetWaypoint == translationWaypoints.Count - 1)
                    {
                        cyclingForward = false;
                        targetWaypoint--;
                    }
                    else
                    {
                        targetWaypoint++;
                    }
                }
                else
                {
                    if (targetWaypoint == 0)
                    {
                        cyclingForward = true;
                        targetWaypoint++;
                    }
                    else
                    {
                        targetWaypoint--;
                    }
                }
            }
            velocity = translationWaypoints[targetWaypoint] - transform.position;
            yield return new WaitForFixedUpdate();
        }
    }

    private Vector3 velocity
    {
        set
        {
            _velocity = value.normalized * waypointCyclingVelocity;
        }
    }
}