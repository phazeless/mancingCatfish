using System;
using UnityEngine;

public class MoveTowards : MonoBehaviour
{
	private void Start()
	{
		this.speed = UnityEngine.Random.Range(0.5f, 2f);
		base.transform.position = new Vector3(base.transform.position.x + UnityEngine.Random.Range(-1f, 1f), base.transform.position.y, 11f);
	}

	private void FixedUpdate()
	{
		this.Move();
	}

	private void Move()
	{
		this.LookAt(this.target);
		float d = Mathf.Sin(Time.time * 15f) * UnityEngine.Random.Range(0.001f, 0.01f);
		base.transform.Translate(Vector3.right * d);
		base.transform.Translate(Vector3.up * Time.deltaTime * this.speed);
	}

	private void Wiggle()
	{
	}

	private void LookAt(Vector3 target)
	{
		Vector3 vector = target - base.transform.position;
		vector.Normalize();
		float num = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
		base.transform.rotation = Quaternion.Euler(0f, 0f, num - 90f);
	}

	public Vector3 target = Vector3.zero;

	private float speed;
}
