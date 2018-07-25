using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrialGenerator : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public void findOne(bool v)
    {
        Vector3 p1, p2;

        TwoHeightComparisons(out p1, out p2, Terrain.activeTerrain, 0, 1024, 0, 1024, 50, 300, 10, 20, 1, 0.6f);
        TwoSightComparisons(out p1, out p2, Terrain.activeTerrain, 128, 964, 128, 964, 50, 300, 5);
        Debug.Log("Points: "+ p1 +", "+ p2);
    }

    /// <summary>
    /// Returns two points that match a series of constraints to be used for a hieght comparision task 
    /// </summary>
    public static void TwoHeightComparisons(out Vector3 point1, out Vector3 point2, Terrain map, float xStart,float xEnd, float yStart, float yEnd, float minDist, float maxDist, float minHeightDiff, float maxHeightDiff, float highBand, float lowBand)
    {
        int runCount = 0;
        float maxHeight = map.terrainData.size.y;

        while (true)
        {
            runCount++;
            point1 = new Vector3(Random.Range(xStart, xEnd), 0, Random.Range(yStart, yEnd));
            point1.y = map.SampleHeight(point1);
            if (point1.y / maxHeight < lowBand || point1.y / maxHeight > highBand)
                continue;

            Vector2 dir = Random.insideUnitCircle;
            point2 = point1 + new Vector3(dir.x, 0, dir.y) * (maxDist - minDist);
            point2 += new Vector3(minDist, 0, minDist);
            point2.x = Mathf.Clamp(point2.x, xStart, xEnd);
            point2.z = Mathf.Clamp(point2.z, yStart, yEnd);
            point2.y = map.SampleHeight(point2);
            if (point2.y / maxHeight < lowBand || point2.y / maxHeight > highBand)
                continue;


            if (Vector3.Distance(point1, point2) > maxDist ||
                Vector3.Distance(point1, point2) < minDist || 
                Mathf.Abs(point1.y - point2.y)/maxHeight > maxHeightDiff ||
                Mathf.Abs(point1.y - point2.y)/maxHeight < minHeightDiff)
                continue;

            break;
        }

        //Debug.Log("Found in " + runCount + " runs.");
    }

    public static void TwoSightComparisons(out Vector3 point1, out Vector3 point2, Terrain map, float xStart, float xEnd, float yStart, float yEnd, float minDist, float maxDist, float checkRadius)
    {
        int runCount = 0;
        while (true)
        {
            runCount++;
            point1 = new Vector3(Random.Range(xStart, xEnd), 0, Random.Range(yStart, yEnd));
            point1.y = map.SampleHeight(point1)+ 1;

            Vector2 dir = Random.insideUnitCircle;
            point2 = point1 + new Vector3(dir.x, 0, dir.y) * (maxDist - minDist);
            point2 += new Vector3(dir.x, 0, dir.y).normalized * minDist;

            point2.x = Mathf.Clamp(point2.x, xStart, xEnd);
            point2.z = Mathf.Clamp(point2.z, yStart, yEnd);
            point2.y = map.SampleHeight(point2) + 1;

            if (Vector3.Distance(point1, point2) < minDist)
                continue;


            Vector3 dirVec = (point1 - point2).normalized;
            Vector3 point1L, point1R, point2L, point2R;

            point1L = point1 + Vector3.Cross(dirVec, Vector3.up).normalized * checkRadius;
            point1L.y = map.SampleHeight(point1L) + 1;
            point1R = point1 - Vector3.Cross(dirVec, Vector3.up).normalized * checkRadius;
            point1R.y = map.SampleHeight(point1R) + 1;
            point2L = point2 + Vector3.Cross(dirVec, Vector3.up).normalized * checkRadius;
            point2L.y = map.SampleHeight(point2L) + 1;
            point2R = point2 - Vector3.Cross(dirVec, Vector3.up).normalized * checkRadius;
            point2R.y = map.SampleHeight(point2R) + 1;


            //LL, LC, LR ; CC, CR; RR
            int sightDegree = 0;

            //LL
            sightDegree += Physics.Linecast(point1L, point2L) ? 1 : 0;
            //LC
            sightDegree += Physics.Linecast(point1L, point2) ? 1 : 0;
            //LR
            sightDegree += Physics.Linecast(point1L, point2R) ? 1 : 0;
            //CL
            sightDegree += Physics.Linecast(point1, point2L) ? 1 : 0;
            //CC
            sightDegree += Physics.Linecast(point1, point2) ? 1 : 0;
            //CR
            sightDegree += Physics.Linecast(point1, point2R) ? 1 : 0;
            //RL
            sightDegree += Physics.Linecast(point1R, point2L) ? 1 : 0;
            //RC
            sightDegree += Physics.Linecast(point1R, point2) ? 1 : 0;
            //RR
            sightDegree += Physics.Linecast(point1R, point2R) ? 1 : 0;

            if(sightDegree > 2 && sightDegree < 7)
                break;
        }

        //Debug.Log("Found in " + runCount + " runs.");
    }

}
