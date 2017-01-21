using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sea : MonoBehaviour {
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
		//StartCoroutine (RandomSpashCoroutine ());
	}

	public int GetClosestIndex(float x)
	{
		float halfGridSize = gridSize / 2;
		if (x < -halfGridSize || x > halfGridSize) {
			return 0;
		}

		return Mathf.RoundToInt (((x + halfGridSize) / gridSize) * gridSubdivision);
	}

	public Vector3 GetHeightVelocity(float x)
	{
		int closestIndex = GetClosestIndex (x);

		float position0 = positions [closestIndex];
		float position1 = positions [closestIndex + 1];

		float gridDist = gridSize / (gridSubdivision - 1.0f);
		float angle = Mathf.Rad2Deg * Mathf.Atan ((position1 - position0) / gridDist);

		return new Vector3 (positions [closestIndex], velocities [closestIndex], angle);
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
			vertices.Add (new Vector3 (x, -15.0f, 0.0f));
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
		if (index >= 5 && index < gridSubdivision - 5) {
			positions [index-5] += speed * 0.01f;
			positions [index-4] += speed * 0.05f;
			positions [index-3] += speed * 0.2f;
			positions [index-2] += speed * 0.5f;
			positions [index-1] += speed * 0.8f;
			positions [index-0] += speed;
			positions [index+1] += speed * 0.8f;
			positions [index+2] += speed * 0.5f;
			positions [index+3] += speed * 0.2f;
			positions [index+4] += speed * 0.05f;
			positions [index+5] += speed * 0.01f;
		}
	}

	public void Splash(float x, float speed)
	{
		int index = GetClosestIndex (x);
		if (index >= 5 && index < gridSubdivision - 5) {
			positions [index-5] += speed * 0.01f;
			positions [index-4] += speed * 0.05f;
			positions [index-3] += speed * 0.2f;
			positions [index-2] += speed * 0.5f;
			positions [index-1] += speed * 0.8f;
			positions [index-0] += speed;
			positions [index+1] += speed * 0.8f;
			positions [index+2] += speed * 0.5f;
			positions [index+3] += speed * 0.2f;
			positions [index+4] += speed * 0.05f;
			positions [index+5] += speed * 0.01f;
		}
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

		// fix the sidex of the screen
		velocities [0] = 0.0f;
		velocities [gridSubdivision-1] = 0.0f;
		positions [0] = 0.0f;
		positions [gridSubdivision-1] = 0.0f;

		// do some passes where springs pull on their neighbours
		float[] leftDeltas = new float[gridSubdivision];
		float[] rightDeltas = new float[gridSubdivision];
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
		UpdateSea ();
	}
}
