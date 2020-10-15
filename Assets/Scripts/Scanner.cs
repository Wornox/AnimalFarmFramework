using UnityEngine;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Collections;
using UnityEngine.Jobs;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;

public class Scanner : MonoBehaviour
{
	AnimalComponentReferences ACR;
	AnimalProperties AP;
	LineRenderer LR;

	List<Collider> scannedObjects;

	public bool draw_scanner;

	public Collider[] enemy;

	void Awake()
	{
		ACR = GetComponent<AnimalComponentReferences>();
		AP = GetComponent<AnimalProperties>();
		LR = ACR.animalMeshController.lineRenderer;
		scannedObjects = new List<Collider>();

	}

	static bool first = true;

    void Update()
    {
		//ScanArea();
		if (first)
		{
			first = false;
			//ScanAllEnemy();
		}

		//Draw scanner area
		if(draw_scanner)DrawCircle();
		else
		{
			LR.positionCount = 0;
			LR.enabled = false;
		}
	}

	private void ScanAllEnemy()
	{
		var list = GameObject.Find("AnimalController").GetComponent<AnimalController>().animalPrefabList;
		TransformAccessArray transformArray = new TransformAccessArray(list.Count);

		foreach (var a in list)
		{
			if(a != null)
			{
				var scanner = a.GetComponent<Scanner>();
				if(scanner != null)
				{
					transformArray.Add(a.transform);
				}
			}
		}

		AP = GetComponent<AnimalProperties>();
		string[] enemyMask = new string[AP.enemyTypes.Count];
		for (int i = 0; i < AP.enemyTypes.Count; i++)
		{
			enemyMask[i] = AP.enemyTypes[i].ToString();
		}

		Scanner.ScanJob job = new Scanner.ScanJob
		{
			range = AP.viewRadius,
			layerMask = LayerMask.GetMask(enemyMask),
		};

		JobHandle handle = job.Schedule(transformArray);

		handle.Complete();

		for (int i = 0; i < list.Count; i++)
		{
			Collider[] temp = new Collider[job.allscan.Length];
			for (int j = 0; j < job.allscan.Length; j++)
			{
				temp[j] = job.allscan[i][j].GetComponent<Collider>();
			}
			list[i].GetComponent<Scanner>().enemy = temp;
		}
	}

	public struct ScanJob : IJobParallelForTransform
	{
		public float range;
		public LayerMask layerMask;
		public NativeArray<TransformAccessArray> allscan;
		public TransformAccess result;
		public void Execute(int index, TransformAccess transform)
		{
			Collider[] asd = Physics.OverlapSphere(transform.position, range, layerMask);
			TransformAccessArray transformArray = new TransformAccessArray(asd.Length);
			for (int i = 0; i < asd.Length; i++)
			{
				transformArray[i] = asd[i].transform;
			}
			allscan[index] = transformArray;
		}
	}

	public Collider[] ScanArea(LayerMask layerMask,float range)
	{
		Collider[] allscan = Physics.OverlapSphere(transform.position, range, layerMask);
		return allscan;
	}

	public Collider[] ScanArea(LayerMask layerMask)
	{
		return ScanArea(layerMask, AP.viewRadius);
	}

	void DrawCircle()
	{
		if (LR.positionCount != 0) return; //drawing is depeded on local positions, so after parent(animal) is moveing, the cirecle moves with it wihtout manually updating
		var segments = 360;
		LR.enabled = true;
		LR.useWorldSpace = false;
		LR.startWidth = 0.1f;
		LR.endWidth = 0.1f;
		LR.positionCount = segments + 1;

		var pointCount = segments + 1; // add extra point to make startpoint and endpoint the same to close the circle
		var points = new Vector3[pointCount];

		for (int i = 0; i < pointCount; i++)
		{
			var rad = Mathf.Deg2Rad * (i * 360f / segments);
			points[i] = new Vector3(Mathf.Sin(rad) * AP.viewRadius * 2, 2, Mathf.Cos(rad) * AP.viewRadius * 2);
		}

		LR.SetPositions(points);
		LR.startWidth = 0.3f;
		LR.endWidth = 0.3f;
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawSphere(transform.position, AP.viewRadius);
	}

}
