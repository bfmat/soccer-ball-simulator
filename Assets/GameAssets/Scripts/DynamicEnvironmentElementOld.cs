using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DynamicEnvironmentElementOld : MonoBehaviour {

    [SerializeField] private Vector3 rotPerTick;
    [SerializeField] private Vector3[] translationWaypoints;
    [SerializeField] private float timeToHalfCycleThroughAllWaypoints;
    private float[] translationTimings;
    private int lastWaypointLeft = -1;
    private bool returning = false;

    void Start() {
        ConfigureCenterOfRotation();
        if (translationWaypoints.Length > 0)
        {
            AddOriginalPositionToWaypoints();
            ConfigureTranslationTimings();
            StartNextWaypointTrajectory();
        }
        StartCoroutine(Rotate(Quaternion.Euler(rotPerTick)));
    }

    private IEnumerator Rotate(Quaternion rotPerTick) {
        while (enabled) {
            transform.rotation *= rotPerTick;
            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator Translate(Vector3 start, Vector3 end, float time) {
        float timeStart = Time.time;
        while (enabled) {
            float u = Time.time - timeStart / time;
            transform.position = Vector3.Lerp(start, end, u);
            if (transform.position == end)
            {
                Invoke("StartNextWaypointTrajectory", 0f);
                yield break;
            }
            yield return null;
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

    private void ConfigureTranslationTimings()
    {
        float[] magnitudes = new float[translationWaypoints.Length - 1];
        translationTimings = new float[magnitudes.Length];
        for (int i = 0; i < translationWaypoints.Length - 1; i++) {
            Vector3 delta = translationWaypoints[i] - translationWaypoints[i + 1];
            magnitudes[i] = delta.magnitude;
        }
        float totalMagnitudes = 0f;
        foreach (float m in magnitudes)
        {
            totalMagnitudes += m;
        }
        for (int i = 0; i < magnitudes.Length; i++)
        {
            magnitudes[i] /= totalMagnitudes;
            translationTimings[i] = magnitudes[i] * timeToHalfCycleThroughAllWaypoints;
        }
    }

    private void StartNextWaypointTrajectory()
    {
        int r = returning ? -1 : 1;
        int waypointAboutToLeave = lastWaypointLeft + r;
        StartCoroutine(Translate(translationWaypoints[waypointAboutToLeave], translationWaypoints[GetNextReturningElementWithCount(translationWaypoints.Length, waypointAboutToLeave, !returning)], translationTimings[lastWaypointLeft + r]));
        lastWaypointLeft += r;
        if (lastWaypointLeft == translationWaypoints.Length - 2 && !returning) returning = true;
        else if (lastWaypointLeft == 1 && returning) returning = false;
    }

    private void AddOriginalPositionToWaypoints()
    {
        Vector3[] temp = translationWaypoints;
        translationWaypoints = new Vector3[temp.Length + 1];
        translationWaypoints[0] = transform.position;
        for (int i = 0; i < temp.Length; i++)
        {
            translationWaypoints[i + 1] = temp[i];
        }
    }

    private int GetNextReturningElementWithCount(int count, int index, bool ascending)
    {
        int maxElement = count - 1;
        if (ascending)
        {
            if (index == maxElement)
            {
                return maxElement - 1;
            } else
            {
                return index + 1;
            }
        } else
        {
            if (index == 0)
            {
                return 1;
            } else
            {
                return index - 1;
            }
        }
    }
}