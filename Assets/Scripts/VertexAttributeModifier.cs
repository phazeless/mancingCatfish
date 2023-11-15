using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class VertexAttributeModifier : MonoBehaviour
{
	private void Awake()
	{
		this.m_TextComponent = base.gameObject.GetComponent<TMP_Text>();
	}

	private void Start()
	{
		base.StartCoroutine(this.WarpText());
	}

	private AnimationCurve CopyAnimationCurve(AnimationCurve curve)
	{
		return new AnimationCurve
		{
			keys = curve.keys
		};
	}

	private IEnumerator WarpText()
	{
		this.VertexCurve.preWrapMode = WrapMode.Once;
		this.VertexCurve.postWrapMode = WrapMode.Once;
		Mesh mesh = this.m_TextComponent.textInfo.meshInfo[0].mesh;
		this.m_TextComponent.havePropertiesChanged = true;
		this.CurveScale *= 10f;
		float old_CurveScale = this.CurveScale;
		AnimationCurve old_curve = this.CopyAnimationCurve(this.VertexCurve);
		for (;;)
		{
			TMP_TextInfo textInfo = this.m_TextComponent.textInfo;
			this.time += Time.deltaTime * (float)textInfo.characterCount * 2f;
			if (this.time > 80f && old_CurveScale == this.CurveScale && old_curve.keys[1].value == this.VertexCurve.keys[1].value)
			{
				yield return null;
			}
			else
			{
				old_CurveScale = this.CurveScale;
				old_curve = this.CopyAnimationCurve(this.VertexCurve);
				this.m_TextComponent.ForceMeshUpdate();
				int characterCount = textInfo.characterCount;
				if (characterCount != 0)
				{
					float boundsMinX = mesh.bounds.min.x;
					float boundsMaxX = mesh.bounds.max.x;
					for (int i = 0; i < characterCount; i++)
					{
						if (textInfo.characterInfo[i].isVisible)
						{
							int vertexIndex = textInfo.characterInfo[i].vertexIndex;
							int materialReferenceIndex = textInfo.characterInfo[i].materialReferenceIndex;
							Vector3[] vertices = textInfo.meshInfo[materialReferenceIndex].vertices;
							float pointSize = textInfo.characterInfo[i].pointSize;
							Vector3 vector = new Vector2((vertices[vertexIndex].x + vertices[vertexIndex + 2].x) / 2f, textInfo.characterInfo[i].baseLine);
							vertices[vertexIndex] += -vector;
							vertices[vertexIndex + 1] += -vector;
							vertices[vertexIndex + 2] += -vector;
							vertices[vertexIndex + 3] += -vector;
							float num = (vector.x - boundsMinX) / (boundsMaxX - boundsMinX);
							float num2 = num + 0.0001f;
							float y = this.VertexCurve.Evaluate(num) * this.CurveScale;
							float y2 = this.VertexCurve.Evaluate(num2) * this.CurveScale;
							Vector3 lhs = new Vector3(1f, 0f, 0f);
							Vector3 rhs = new Vector3(num2 * (boundsMaxX - boundsMinX) + boundsMinX, y2) - new Vector3(vector.x, y);
							float num3 = Mathf.Acos(Vector3.Dot(lhs, rhs.normalized)) * 57.29578f;
							float z = (Vector3.Cross(lhs, rhs).z <= 0f) ? (360f - num3) : num3;
							float num4 = this.time - (float)i;
							if (num4 > 20f)
							{
								num4 = 20f;
							}
							else if (num4 < 0f)
							{
								num4 = 0f;
							}
							Matrix4x4 matrix = Matrix4x4.TRS(new Vector3(0f, y, 0f), Quaternion.Euler(0f, 0f, z), Vector3.one * this.bounceCurve.Evaluate(num4 * 0.05f));
							vertices[vertexIndex] = matrix.MultiplyPoint3x4(vertices[vertexIndex]);
							vertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 1]);
							vertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 2]);
							vertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 3]);
							vertices[vertexIndex] += vector;
							vertices[vertexIndex + 1] += vector;
							vertices[vertexIndex + 2] += vector;
							vertices[vertexIndex + 3] += vector;
						}
					}
					this.m_TextComponent.UpdateVertexData();
					yield return new WaitForSeconds(0.025f);
				}
			}
		}
		yield break;
	}

	private TMP_Text m_TextComponent;

	public AnimationCurve VertexCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(0.25f, 2f),
		new Keyframe(0.5f, 0f),
		new Keyframe(0.75f, 2f),
		new Keyframe(1f, 0f)
	});

	public AnimationCurve bounceCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(0.3f, 2f),
		new Keyframe(0.7f, 0.65f),
		new Keyframe(1f, 1f)
	});

	public float AngleMultiplier = 1f;

	public float SpeedMultiplier = 1f;

	public float CurveScale = 1f;

	private float time;
}
