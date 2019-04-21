using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointCalculator : MonoBehaviour {

	[SerializeField] Transform[] sweetSpots;

	[SerializeField] float maxRangeFromPoint = 1f;
	[SerializeField] float fallPower = 4f;
	List<float> distanceHolder = new List<float>();
	[SerializeField] float maxPointsFromDistance = 10f;

	[SerializeField] [Range(0.0005f, 0.01f)] float targetSpeed = 0.005f;
	[SerializeField] [Range(0,1)] float speedRange = 1f;
	List<float> speedHolder = new List<float>();
	[SerializeField] float maxPointsFromSpeed = 100f;

	[SerializeField] int idealArrangement = 1;
	int currentArrangement = 1;

	Vector3 lastInputPos;

	float aggregatePoints;

	[SerializeField] [Range(0,1000)] float pointsLostPerCalc = 0.7f;

	[SerializeField] int framesToAverageOver = 10;

	[SerializeField] [Range(0,1)] float maxPercentNearOnePoint = 0.4f;
	float[] timeNearPoints;

	[SerializeField] Camera sceneCam;

	// Use this for initialization
	void Start () {
		lastInputPos = transform.position;
		timeNearPoints = new float[sweetSpots.Length];
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space)){
			ViewSwitcher.Instance.SwitchToRubView();
		}
		if(Input.GetMouseButtonDown(0)){
			currentArrangement = (currentArrangement + 1) % 3;
		}

		transform.position = sceneCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));

		if(speedHolder.Count < framesToAverageOver){
			speedHolder.Add(SpeedThisFrame());
			distanceHolder.Add(DistanceThisFrame());
			return;
		}

		//Calculate speed
		float averageSpeed = 0;

		for(int i = 0; i < speedHolder.Count; i++){
			averageSpeed += speedHolder[i];
		}

		averageSpeed /= speedHolder.Count;

		float speedDiff = Mathf.Abs((averageSpeed - targetSpeed)) / (targetSpeed * speedRange);

		speedHolder.Clear();

		//Calculate distance
		float averageDistance = 0;

		for(int i = 0; i < distanceHolder.Count; i++){
			averageDistance += distanceHolder[i];
		}

		averageDistance /= distanceHolder.Count;

		float distanceDiff = averageDistance / maxRangeFromPoint;

		distanceHolder.Clear();

		//Aggregate points from distance and speed
		float points = CalculatePointsThisFrame(Mathf.Clamp01(distanceDiff), Mathf.Clamp01(speedDiff), currentArrangement);

		aggregatePoints += points;

		Debug.Log("Total points: " + aggregatePoints);

		//Decay points each time by a fixed rate
		aggregatePoints = Mathf.Clamp(aggregatePoints - pointsLostPerCalc, 0, 10000);
	}

	float DistanceThisFrame(){
		float minDist = Mathf.Infinity;
		int minIndex = -1;

		Vector2 transformVec2 = new Vector2(transform.position.x, transform.position.y);

		for(int i = 0; i < sweetSpots.Length; i++){
			float dist = Vector2.Distance(transform.position, sweetSpots[i].position);

			if(dist < minDist){
				minIndex = i;
				minDist = dist;
			}
		}

		if(minIndex != -1){
			timeNearPoints[minIndex] += Time.deltaTime;
		}

		return minDist;
	}

	float SpeedThisFrame(){
		float delta = Vector3.Distance(transform.position, lastInputPos) * Time.deltaTime;

		lastInputPos = transform.position;

		return delta;
	}

	float CalculatePointsThisFrame(float distance, float speedDifference, int position){
		if(distance == 1 || speedDifference == 1)
			return 0;
		float speedPoints = maxPointsFromSpeed * (1 - speedDifference);
		float distancePoints = maxPointsFromDistance * (1 - distance);
		float positionPoints = 1;
		
		if(position != idealArrangement){
			positionPoints = 0.9f;
		}

		float totalTime = 0;
		for(int i = 0; i < timeNearPoints.Length; i++){
			totalTime += timeNearPoints[i];
		}
		for(int i = 0; i < timeNearPoints.Length; i++){
			timeNearPoints[i] = timeNearPoints[i] / totalTime;
			if(timeNearPoints[i] > maxPercentNearOnePoint){
				// Debug.Log(timeNearPoints[i]);
				return 0;
			}
			timeNearPoints[i] = 0;
		}

		// Debug.Log("Speed: " + speedPoints + "  Dist: " + distancePoints);

		return (speedPoints + distancePoints) * positionPoints;
	}
}
