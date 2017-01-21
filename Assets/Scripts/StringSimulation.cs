using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringSimulation : MonoBehaviour {
	public int gridSize;
	public int gridSubdivision;

	float k = 0.025f;	// Sprint constant (0.025 works well)
	float d = 0.03f;	// amount of daming (0.05f works well)
	float spread = 0.01f;	// Spread, between 0 and 0.5

	private Mesh planeMesh;
	private List<Vector3> vertices = new List<Vector3> ();
	private List<float> positions = new List<float> ();
	private List<float> velocities = new List<float> ();

	// Use this for initialization
	void Start () {
		CreatePlane ();
	}

	public Vector2 GetHeightVelocity(float x)
	{
		float halfGridSize = gridSize / 2;
		if (x < -halfGridSize || x > halfGridSize) {
			return new Vector2 (0.0f, 0.0f);
		}

		int closestIndex = Mathf.RoundToInt (((x + halfGridSize) / gridSize) * gridSubdivision);

		return new Vector2 (positions [closestIndex], velocities [closestIndex]);
	}

	private void CreatePlane() {
		planeMesh = new Mesh ();


		List<Vector2> uvs = new List<Vector2> ();
		for (int i = 0; i < gridSubdivision; i++) {
			float x = -gridSize / 2.0f + (gridSize / (gridSubdivision - 1.0f)) * i;

			positions.Add (Mathf.Sin (i / 10.0f) * 0.0f);
			velocities.Add (0.0f);

			vertices.Add (new Vector3 (x, positions[i], 0.0f));
			uvs.Add (new Vector2 (0.0f, 0.0f));
		}
		for (int i = 0; i < gridSubdivision; i++) {
			float x = -gridSize / 2.0f + (gridSize / (gridSubdivision - 1.0f)) * i;
			vertices.Add (new Vector3 (x, -5.0f, 0.0f));
			uvs.Add (new Vector2 (0.0f, 0.0f));
		}


		List<int> indices = new List<int> ();
		for (int i = 0; i < gridSubdivision - 1; i++) {
			indices.Add (i + gridSubdivision);
			indices.Add (i + 0);
			indices.Add (i + gridSubdivision + 1);
			indices.Add (i + gridSubdivision + 1);
			indices.Add (i + 0);
			indices.Add (i + 1);
		}

		planeMesh.SetVertices(vertices);
		planeMesh.SetIndices (indices.ToArray(), MeshTopology.Triangles, 0);
		planeMesh.SetUVs(0, uvs);

		planeMesh.RecalculateNormals ();

		GetComponent<MeshFilter> ().mesh = planeMesh;
	}

	public void Splash(int index, float speed)
	{
		if (index >= 0 && index < gridSubdivision)
			positions[index] = speed;
	}

	public void UpdateSea()
	{
		// update spring physics
		for (int i = 0; i < gridSubdivision; i++) {
			float x = positions[i] - 0.0f;
			float acceleration = -k * x - velocities[i] * d;

			positions[i] += velocities[i];
			velocities[i] += acceleration;
		}

		velocities [0] = 0.0f;
		velocities [gridSubdivision-1] = 0.0f;
		positions [0] = 0.0f;
		positions [gridSubdivision-1] = 0.0f;

		float[] leftDeltas = new float[gridSubdivision];
		float[] rightDeltas = new float[gridSubdivision];

		// do some passes where springs pull on their neighbours
		for (int j = 0; j < 8; j++)
		{
			for (int i = 0; i < gridSubdivision; i++)
			{
				if (i > 0)
				{
					leftDeltas[i] = spread * (positions[i] - positions[i - 1]);
					velocities[i - 1] += leftDeltas[i];
				}
				if (i < gridSubdivision - 1)
				{
					rightDeltas[i] = spread * (positions[i] - positions[i + 1]);
					velocities[i + 1] += rightDeltas[i];
				}
			}

			for (int i = 0; i < gridSubdivision; i++)
			{
				if (i > 0)
					positions[i - 1] += leftDeltas[i];
				if (i < gridSubdivision - 1)
					positions[i + 1] += rightDeltas[i];
			}
		}

		// update vertices
		for (int i = 0; i < gridSubdivision; i++) {
			vertices [i] = new Vector3 (vertices[i].x, positions[i], 0.0f);
		}
		planeMesh.SetVertices(vertices);
		GetComponent<MeshFilter> ().mesh = planeMesh;
	}

	// Update is called once per frame
	void Update () {
		if (Input.anyKeyDown) {
			Splash (Random.Range(10, gridSubdivision - 10), -3);
		}

		UpdateSea ();
	}
}
