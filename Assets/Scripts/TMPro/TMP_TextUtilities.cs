using System;
using UnityEngine;

namespace TMPro
{
	public static class TMP_TextUtilities
	{
		public static CaretInfo GetCursorInsertionIndex(TMP_Text textComponent, Vector3 position, Camera camera)
		{
			int num = TMP_TextUtilities.FindNearestCharacter(textComponent, position, camera, false);
			RectTransform rectTransform = textComponent.rectTransform;
			TMP_TextUtilities.ScreenPointToWorldPointInRectangle(rectTransform, position, camera, out position);
			TMP_CharacterInfo tmp_CharacterInfo = textComponent.textInfo.characterInfo[num];
			Vector3 vector = rectTransform.TransformPoint(tmp_CharacterInfo.bottomLeft);
			Vector3 vector2 = rectTransform.TransformPoint(tmp_CharacterInfo.topRight);
			float num2 = (position.x - vector.x) / (vector2.x - vector.x);
			if (num2 < 0.5f)
			{
				return new CaretInfo(num, CaretPosition.Left);
			}
			return new CaretInfo(num, CaretPosition.Right);
		}

		public static int GetCursorIndexFromPosition(TMP_Text textComponent, Vector3 position, Camera camera)
		{
			int num = TMP_TextUtilities.FindNearestCharacter(textComponent, position, camera, false);
			RectTransform rectTransform = textComponent.rectTransform;
			TMP_TextUtilities.ScreenPointToWorldPointInRectangle(rectTransform, position, camera, out position);
			TMP_CharacterInfo tmp_CharacterInfo = textComponent.textInfo.characterInfo[num];
			Vector3 vector = rectTransform.TransformPoint(tmp_CharacterInfo.bottomLeft);
			Vector3 vector2 = rectTransform.TransformPoint(tmp_CharacterInfo.topRight);
			float num2 = (position.x - vector.x) / (vector2.x - vector.x);
			if (num2 < 0.5f)
			{
				return num;
			}
			return num + 1;
		}

		public static int GetCursorIndexFromPosition(TMP_Text textComponent, Vector3 position, Camera camera, out CaretPosition cursor)
		{
			int num = TMP_TextUtilities.FindNearestCharacter(textComponent, position, camera, false);
			RectTransform rectTransform = textComponent.rectTransform;
			TMP_TextUtilities.ScreenPointToWorldPointInRectangle(rectTransform, position, camera, out position);
			TMP_CharacterInfo tmp_CharacterInfo = textComponent.textInfo.characterInfo[num];
			Vector3 vector = rectTransform.TransformPoint(tmp_CharacterInfo.bottomLeft);
			Vector3 vector2 = rectTransform.TransformPoint(tmp_CharacterInfo.topRight);
			float num2 = (position.x - vector.x) / (vector2.x - vector.x);
			if (num2 < 0.5f)
			{
				cursor = CaretPosition.Left;
				return num;
			}
			cursor = CaretPosition.Right;
			return num;
		}

		public static bool IsIntersectingRectTransform(RectTransform rectTransform, Vector3 position, Camera camera)
		{
			TMP_TextUtilities.ScreenPointToWorldPointInRectangle(rectTransform, position, camera, out position);
			rectTransform.GetWorldCorners(TMP_TextUtilities.m_rectWorldCorners);
			return TMP_TextUtilities.PointIntersectRectangle(position, TMP_TextUtilities.m_rectWorldCorners[0], TMP_TextUtilities.m_rectWorldCorners[1], TMP_TextUtilities.m_rectWorldCorners[2], TMP_TextUtilities.m_rectWorldCorners[3]);
		}

		public static int FindIntersectingCharacter(TMP_Text text, Vector3 position, Camera camera, bool visibleOnly)
		{
			RectTransform rectTransform = text.rectTransform;
			TMP_TextUtilities.ScreenPointToWorldPointInRectangle(rectTransform, position, camera, out position);
			for (int i = 0; i < text.textInfo.characterCount; i++)
			{
				TMP_CharacterInfo tmp_CharacterInfo = text.textInfo.characterInfo[i];
				if ((!visibleOnly || tmp_CharacterInfo.isVisible) && (text.OverflowMode != TextOverflowModes.Page || (int)(tmp_CharacterInfo.pageNumber + 1) == text.pageToDisplay))
				{
					Vector3 a = rectTransform.TransformPoint(tmp_CharacterInfo.bottomLeft);
					Vector3 b = rectTransform.TransformPoint(new Vector3(tmp_CharacterInfo.bottomLeft.x, tmp_CharacterInfo.topRight.y, 0f));
					Vector3 c = rectTransform.TransformPoint(tmp_CharacterInfo.topRight);
					Vector3 d = rectTransform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.bottomLeft.y, 0f));
					if (TMP_TextUtilities.PointIntersectRectangle(position, a, b, c, d))
					{
						return i;
					}
				}
			}
			return -1;
		}

		public static int FindNearestCharacter(TMP_Text text, Vector3 position, Camera camera, bool visibleOnly)
		{
			RectTransform rectTransform = text.rectTransform;
			float num = float.PositiveInfinity;
			int result = 0;
			TMP_TextUtilities.ScreenPointToWorldPointInRectangle(rectTransform, position, camera, out position);
			for (int i = 0; i < text.textInfo.characterCount; i++)
			{
				TMP_CharacterInfo tmp_CharacterInfo = text.textInfo.characterInfo[i];
				if ((!visibleOnly || tmp_CharacterInfo.isVisible) && (text.OverflowMode != TextOverflowModes.Page || (int)(tmp_CharacterInfo.pageNumber + 1) == text.pageToDisplay))
				{
					Vector3 vector = rectTransform.TransformPoint(tmp_CharacterInfo.bottomLeft);
					Vector3 vector2 = rectTransform.TransformPoint(new Vector3(tmp_CharacterInfo.bottomLeft.x, tmp_CharacterInfo.topRight.y, 0f));
					Vector3 vector3 = rectTransform.TransformPoint(tmp_CharacterInfo.topRight);
					Vector3 vector4 = rectTransform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.bottomLeft.y, 0f));
					if (TMP_TextUtilities.PointIntersectRectangle(position, vector, vector2, vector3, vector4))
					{
						return i;
					}
					float num2 = TMP_TextUtilities.DistanceToLine(vector, vector2, position);
					float num3 = TMP_TextUtilities.DistanceToLine(vector2, vector3, position);
					float num4 = TMP_TextUtilities.DistanceToLine(vector3, vector4, position);
					float num5 = TMP_TextUtilities.DistanceToLine(vector4, vector, position);
					float num6 = (num2 >= num3) ? num3 : num2;
					num6 = ((num6 >= num4) ? num4 : num6);
					num6 = ((num6 >= num5) ? num5 : num6);
					if (num > num6)
					{
						num = num6;
						result = i;
					}
				}
			}
			return result;
		}

		public static int FindIntersectingWord(TMP_Text text, Vector3 position, Camera camera)
		{
			RectTransform rectTransform = text.rectTransform;
			TMP_TextUtilities.ScreenPointToWorldPointInRectangle(rectTransform, position, camera, out position);
			for (int i = 0; i < text.textInfo.wordCount; i++)
			{
				TMP_WordInfo tmp_WordInfo = text.textInfo.wordInfo[i];
				bool flag = false;
				Vector3 a = Vector3.zero;
				Vector3 b = Vector3.zero;
				Vector3 d = Vector3.zero;
				Vector3 c = Vector3.zero;
				float num = float.NegativeInfinity;
				float num2 = float.PositiveInfinity;
				for (int j = 0; j < tmp_WordInfo.characterCount; j++)
				{
					int num3 = tmp_WordInfo.firstCharacterIndex + j;
					TMP_CharacterInfo tmp_CharacterInfo = text.textInfo.characterInfo[num3];
					int lineNumber = (int)tmp_CharacterInfo.lineNumber;
					bool flag2 = num3 <= text.maxVisibleCharacters && (int)tmp_CharacterInfo.lineNumber <= text.maxVisibleLines && (text.OverflowMode != TextOverflowModes.Page || (int)(tmp_CharacterInfo.pageNumber + 1) == text.pageToDisplay);
					num = Mathf.Max(num, tmp_CharacterInfo.ascender);
					num2 = Mathf.Min(num2, tmp_CharacterInfo.descender);
					if (!flag && flag2)
					{
						flag = true;
						a = new Vector3(tmp_CharacterInfo.bottomLeft.x, tmp_CharacterInfo.descender, 0f);
						b = new Vector3(tmp_CharacterInfo.bottomLeft.x, tmp_CharacterInfo.ascender, 0f);
						if (tmp_WordInfo.characterCount == 1)
						{
							flag = false;
							d = new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.descender, 0f);
							c = new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.ascender, 0f);
							a = rectTransform.TransformPoint(new Vector3(a.x, num2, 0f));
							b = rectTransform.TransformPoint(new Vector3(b.x, num, 0f));
							c = rectTransform.TransformPoint(new Vector3(c.x, num, 0f));
							d = rectTransform.TransformPoint(new Vector3(d.x, num2, 0f));
							if (TMP_TextUtilities.PointIntersectRectangle(position, a, b, c, d))
							{
								return i;
							}
						}
					}
					if (flag && j == tmp_WordInfo.characterCount - 1)
					{
						flag = false;
						d = new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.descender, 0f);
						c = new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.ascender, 0f);
						a = rectTransform.TransformPoint(new Vector3(a.x, num2, 0f));
						b = rectTransform.TransformPoint(new Vector3(b.x, num, 0f));
						c = rectTransform.TransformPoint(new Vector3(c.x, num, 0f));
						d = rectTransform.TransformPoint(new Vector3(d.x, num2, 0f));
						if (TMP_TextUtilities.PointIntersectRectangle(position, a, b, c, d))
						{
							return i;
						}
					}
					else if (flag && lineNumber != (int)text.textInfo.characterInfo[num3 + 1].lineNumber)
					{
						flag = false;
						d = new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.descender, 0f);
						c = new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.ascender, 0f);
						a = rectTransform.TransformPoint(new Vector3(a.x, num2, 0f));
						b = rectTransform.TransformPoint(new Vector3(b.x, num, 0f));
						c = rectTransform.TransformPoint(new Vector3(c.x, num, 0f));
						d = rectTransform.TransformPoint(new Vector3(d.x, num2, 0f));
						num = float.NegativeInfinity;
						num2 = float.PositiveInfinity;
						if (TMP_TextUtilities.PointIntersectRectangle(position, a, b, c, d))
						{
							return i;
						}
					}
				}
			}
			return -1;
		}

		public static int FindNearestWord(TMP_Text text, Vector3 position, Camera camera)
		{
			RectTransform rectTransform = text.rectTransform;
			float num = float.PositiveInfinity;
			int result = 0;
			TMP_TextUtilities.ScreenPointToWorldPointInRectangle(rectTransform, position, camera, out position);
			for (int i = 0; i < text.textInfo.wordCount; i++)
			{
				TMP_WordInfo tmp_WordInfo = text.textInfo.wordInfo[i];
				bool flag = false;
				Vector3 vector = Vector3.zero;
				Vector3 vector2 = Vector3.zero;
				Vector3 vector3 = Vector3.zero;
				Vector3 vector4 = Vector3.zero;
				for (int j = 0; j < tmp_WordInfo.characterCount; j++)
				{
					int num2 = tmp_WordInfo.firstCharacterIndex + j;
					TMP_CharacterInfo tmp_CharacterInfo = text.textInfo.characterInfo[num2];
					int lineNumber = (int)tmp_CharacterInfo.lineNumber;
					bool flag2 = num2 <= text.maxVisibleCharacters && (int)tmp_CharacterInfo.lineNumber <= text.maxVisibleLines && (text.OverflowMode != TextOverflowModes.Page || (int)(tmp_CharacterInfo.pageNumber + 1) == text.pageToDisplay);
					if (!flag && flag2)
					{
						flag = true;
						vector = rectTransform.TransformPoint(new Vector3(tmp_CharacterInfo.bottomLeft.x, tmp_CharacterInfo.descender, 0f));
						vector2 = rectTransform.TransformPoint(new Vector3(tmp_CharacterInfo.bottomLeft.x, tmp_CharacterInfo.ascender, 0f));
						if (tmp_WordInfo.characterCount == 1)
						{
							flag = false;
							vector3 = rectTransform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.descender, 0f));
							vector4 = rectTransform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.ascender, 0f));
							if (TMP_TextUtilities.PointIntersectRectangle(position, vector, vector2, vector4, vector3))
							{
								return i;
							}
							float num3 = TMP_TextUtilities.DistanceToLine(vector, vector2, position);
							float num4 = TMP_TextUtilities.DistanceToLine(vector2, vector4, position);
							float num5 = TMP_TextUtilities.DistanceToLine(vector4, vector3, position);
							float num6 = TMP_TextUtilities.DistanceToLine(vector3, vector, position);
							float num7 = (num3 >= num4) ? num4 : num3;
							num7 = ((num7 >= num5) ? num5 : num7);
							num7 = ((num7 >= num6) ? num6 : num7);
							if (num > num7)
							{
								num = num7;
								result = i;
							}
						}
					}
					if (flag && j == tmp_WordInfo.characterCount - 1)
					{
						flag = false;
						vector3 = rectTransform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.descender, 0f));
						vector4 = rectTransform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.ascender, 0f));
						if (TMP_TextUtilities.PointIntersectRectangle(position, vector, vector2, vector4, vector3))
						{
							return i;
						}
						float num8 = TMP_TextUtilities.DistanceToLine(vector, vector2, position);
						float num9 = TMP_TextUtilities.DistanceToLine(vector2, vector4, position);
						float num10 = TMP_TextUtilities.DistanceToLine(vector4, vector3, position);
						float num11 = TMP_TextUtilities.DistanceToLine(vector3, vector, position);
						float num12 = (num8 >= num9) ? num9 : num8;
						num12 = ((num12 >= num10) ? num10 : num12);
						num12 = ((num12 >= num11) ? num11 : num12);
						if (num > num12)
						{
							num = num12;
							result = i;
						}
					}
					else if (flag && lineNumber != (int)text.textInfo.characterInfo[num2 + 1].lineNumber)
					{
						flag = false;
						vector3 = rectTransform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.descender, 0f));
						vector4 = rectTransform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.ascender, 0f));
						if (TMP_TextUtilities.PointIntersectRectangle(position, vector, vector2, vector4, vector3))
						{
							return i;
						}
						float num13 = TMP_TextUtilities.DistanceToLine(vector, vector2, position);
						float num14 = TMP_TextUtilities.DistanceToLine(vector2, vector4, position);
						float num15 = TMP_TextUtilities.DistanceToLine(vector4, vector3, position);
						float num16 = TMP_TextUtilities.DistanceToLine(vector3, vector, position);
						float num17 = (num13 >= num14) ? num14 : num13;
						num17 = ((num17 >= num15) ? num15 : num17);
						num17 = ((num17 >= num16) ? num16 : num17);
						if (num > num17)
						{
							num = num17;
							result = i;
						}
					}
				}
			}
			return result;
		}

		public static int FindIntersectingLink(TMP_Text text, Vector3 position, Camera camera)
		{
			Transform transform = text.transform;
			TMP_TextUtilities.ScreenPointToWorldPointInRectangle(transform, position, camera, out position);
			for (int i = 0; i < text.textInfo.linkCount; i++)
			{
				TMP_LinkInfo tmp_LinkInfo = text.textInfo.linkInfo[i];
				bool flag = false;
				Vector3 a = Vector3.zero;
				Vector3 b = Vector3.zero;
				Vector3 d = Vector3.zero;
				Vector3 c = Vector3.zero;
				for (int j = 0; j < tmp_LinkInfo.linkTextLength; j++)
				{
					int num = tmp_LinkInfo.linkTextfirstCharacterIndex + j;
					TMP_CharacterInfo tmp_CharacterInfo = text.textInfo.characterInfo[num];
					int lineNumber = (int)tmp_CharacterInfo.lineNumber;
					if (text.OverflowMode != TextOverflowModes.Page || (int)(tmp_CharacterInfo.pageNumber + 1) == text.pageToDisplay)
					{
						if (!flag)
						{
							flag = true;
							a = transform.TransformPoint(new Vector3(tmp_CharacterInfo.bottomLeft.x, tmp_CharacterInfo.descender, 0f));
							b = transform.TransformPoint(new Vector3(tmp_CharacterInfo.bottomLeft.x, tmp_CharacterInfo.ascender, 0f));
							if (tmp_LinkInfo.linkTextLength == 1)
							{
								flag = false;
								d = transform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.descender, 0f));
								c = transform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.ascender, 0f));
								if (TMP_TextUtilities.PointIntersectRectangle(position, a, b, c, d))
								{
									return i;
								}
							}
						}
						if (flag && j == tmp_LinkInfo.linkTextLength - 1)
						{
							flag = false;
							d = transform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.descender, 0f));
							c = transform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.ascender, 0f));
							if (TMP_TextUtilities.PointIntersectRectangle(position, a, b, c, d))
							{
								return i;
							}
						}
						else if (flag && lineNumber != (int)text.textInfo.characterInfo[num + 1].lineNumber)
						{
							flag = false;
							d = transform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.descender, 0f));
							c = transform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.ascender, 0f));
							if (TMP_TextUtilities.PointIntersectRectangle(position, a, b, c, d))
							{
								return i;
							}
						}
					}
				}
			}
			return -1;
		}

		public static int FindNearestLink(TMP_Text text, Vector3 position, Camera camera)
		{
			RectTransform rectTransform = text.rectTransform;
			TMP_TextUtilities.ScreenPointToWorldPointInRectangle(rectTransform, position, camera, out position);
			float num = float.PositiveInfinity;
			int result = 0;
			for (int i = 0; i < text.textInfo.linkCount; i++)
			{
				TMP_LinkInfo tmp_LinkInfo = text.textInfo.linkInfo[i];
				bool flag = false;
				Vector3 vector = Vector3.zero;
				Vector3 vector2 = Vector3.zero;
				Vector3 vector3 = Vector3.zero;
				Vector3 vector4 = Vector3.zero;
				for (int j = 0; j < tmp_LinkInfo.linkTextLength; j++)
				{
					int num2 = tmp_LinkInfo.linkTextfirstCharacterIndex + j;
					TMP_CharacterInfo tmp_CharacterInfo = text.textInfo.characterInfo[num2];
					int lineNumber = (int)tmp_CharacterInfo.lineNumber;
					if (text.OverflowMode != TextOverflowModes.Page || (int)(tmp_CharacterInfo.pageNumber + 1) == text.pageToDisplay)
					{
						if (!flag)
						{
							flag = true;
							vector = rectTransform.TransformPoint(new Vector3(tmp_CharacterInfo.bottomLeft.x, tmp_CharacterInfo.descender, 0f));
							vector2 = rectTransform.TransformPoint(new Vector3(tmp_CharacterInfo.bottomLeft.x, tmp_CharacterInfo.ascender, 0f));
							if (tmp_LinkInfo.linkTextLength == 1)
							{
								flag = false;
								vector3 = rectTransform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.descender, 0f));
								vector4 = rectTransform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.ascender, 0f));
								if (TMP_TextUtilities.PointIntersectRectangle(position, vector, vector2, vector4, vector3))
								{
									return i;
								}
								float num3 = TMP_TextUtilities.DistanceToLine(vector, vector2, position);
								float num4 = TMP_TextUtilities.DistanceToLine(vector2, vector4, position);
								float num5 = TMP_TextUtilities.DistanceToLine(vector4, vector3, position);
								float num6 = TMP_TextUtilities.DistanceToLine(vector3, vector, position);
								float num7 = (num3 >= num4) ? num4 : num3;
								num7 = ((num7 >= num5) ? num5 : num7);
								num7 = ((num7 >= num6) ? num6 : num7);
								if (num > num7)
								{
									num = num7;
									result = i;
								}
							}
						}
						if (flag && j == tmp_LinkInfo.linkTextLength - 1)
						{
							flag = false;
							vector3 = rectTransform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.descender, 0f));
							vector4 = rectTransform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.ascender, 0f));
							if (TMP_TextUtilities.PointIntersectRectangle(position, vector, vector2, vector4, vector3))
							{
								return i;
							}
							float num8 = TMP_TextUtilities.DistanceToLine(vector, vector2, position);
							float num9 = TMP_TextUtilities.DistanceToLine(vector2, vector4, position);
							float num10 = TMP_TextUtilities.DistanceToLine(vector4, vector3, position);
							float num11 = TMP_TextUtilities.DistanceToLine(vector3, vector, position);
							float num12 = (num8 >= num9) ? num9 : num8;
							num12 = ((num12 >= num10) ? num10 : num12);
							num12 = ((num12 >= num11) ? num11 : num12);
							if (num > num12)
							{
								num = num12;
								result = i;
							}
						}
						else if (flag && lineNumber != (int)text.textInfo.characterInfo[num2 + 1].lineNumber)
						{
							flag = false;
							vector3 = rectTransform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.descender, 0f));
							vector4 = rectTransform.TransformPoint(new Vector3(tmp_CharacterInfo.topRight.x, tmp_CharacterInfo.ascender, 0f));
							if (TMP_TextUtilities.PointIntersectRectangle(position, vector, vector2, vector4, vector3))
							{
								return i;
							}
							float num13 = TMP_TextUtilities.DistanceToLine(vector, vector2, position);
							float num14 = TMP_TextUtilities.DistanceToLine(vector2, vector4, position);
							float num15 = TMP_TextUtilities.DistanceToLine(vector4, vector3, position);
							float num16 = TMP_TextUtilities.DistanceToLine(vector3, vector, position);
							float num17 = (num13 >= num14) ? num14 : num13;
							num17 = ((num17 >= num15) ? num15 : num17);
							num17 = ((num17 >= num16) ? num16 : num17);
							if (num > num17)
							{
								num = num17;
								result = i;
							}
						}
					}
				}
			}
			return result;
		}

		private static bool PointIntersectRectangle(Vector3 m, Vector3 a, Vector3 b, Vector3 c, Vector3 d)
		{
			Vector3 vector = b - a;
			Vector3 rhs = m - a;
			Vector3 vector2 = c - b;
			Vector3 rhs2 = m - b;
			float num = Vector3.Dot(vector, rhs);
			float num2 = Vector3.Dot(vector2, rhs2);
			return 0f <= num && num <= Vector3.Dot(vector, vector) && 0f <= num2 && num2 <= Vector3.Dot(vector2, vector2);
		}

		public static bool ScreenPointToWorldPointInRectangle(Transform transform, Vector2 screenPoint, Camera cam, out Vector3 worldPoint)
		{
			worldPoint = Vector2.zero;
			Ray ray = RectTransformUtility.ScreenPointToRay(cam, screenPoint);
			Plane plane = new Plane(transform.rotation * Vector3.back, transform.position);
			float distance;
			if (!plane.Raycast(ray, out distance))
			{
				return false;
			}
			worldPoint = ray.GetPoint(distance);
			return true;
		}

		private static bool IntersectLinePlane(TMP_TextUtilities.LineSegment line, Vector3 point, Vector3 normal, out Vector3 intersectingPoint)
		{
			intersectingPoint = Vector3.zero;
			Vector3 vector = line.Point2 - line.Point1;
			Vector3 rhs = line.Point1 - point;
			float num = Vector3.Dot(normal, vector);
			float num2 = -Vector3.Dot(normal, rhs);
			if (Mathf.Abs(num) < Mathf.Epsilon)
			{
				return num2 == 0f;
			}
			float num3 = num2 / num;
			if (num3 < 0f || num3 > 1f)
			{
				return false;
			}
			intersectingPoint = line.Point1 + num3 * vector;
			return true;
		}

		public static float DistanceToLine(Vector3 a, Vector3 b, Vector3 point)
		{
			Vector3 vector = b - a;
			Vector3 vector2 = a - point;
			float num = Vector3.Dot(vector, vector2);
			if (num > 0f)
			{
				return Vector3.Dot(vector2, vector2);
			}
			Vector3 vector3 = point - b;
			if (Vector3.Dot(vector, vector3) > 0f)
			{
				return Vector3.Dot(vector3, vector3);
			}
			Vector3 vector4 = vector2 - vector * (num / Vector3.Dot(vector, vector));
			return Vector3.Dot(vector4, vector4);
		}

		public static char ToLowerFast(char c)
		{
			return "-------------------------------- !-#$%&-()*+,-./0123456789:;<=>?@abcdefghijklmnopqrstuvwxyz[-]^_`abcdefghijklmnopqrstuvwxyz{|}~-"[(int)c];
		}

		public static char ToUpperFast(char c)
		{
			return "-------------------------------- !-#$%&-()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[-]^_`ABCDEFGHIJKLMNOPQRSTUVWXYZ{|}~-"[(int)c];
		}

		public static int GetSimpleHashCode(string s)
		{
			int num = 0;
			for (int i = 0; i < s.Length; i++)
			{
				num = ((num << 5) + num ^ (int)s[i]);
			}
			return num;
		}

		public static uint GetSimpleHashCodeLowercase(string s)
		{
			uint num = 5381u;
			for (int i = 0; i < s.Length; i++)
			{
				num = ((num << 5) + num ^ (uint)TMP_TextUtilities.ToLowerFast(s[i]));
			}
			return num;
		}

		public static int HexToInt(char hex)
		{
			switch (hex)
			{
			case '0':
				return 0;
			case '1':
				return 1;
			case '2':
				return 2;
			case '3':
				return 3;
			case '4':
				return 4;
			case '5':
				return 5;
			case '6':
				return 6;
			case '7':
				return 7;
			case '8':
				return 8;
			case '9':
				return 9;
			default:
				switch (hex)
				{
				case 'a':
					return 10;
				case 'b':
					return 11;
				case 'c':
					return 12;
				case 'd':
					return 13;
				case 'e':
					return 14;
				case 'f':
					return 15;
				default:
					return 15;
				}
				break;
			case 'A':
				return 10;
			case 'B':
				return 11;
			case 'C':
				return 12;
			case 'D':
				return 13;
			case 'E':
				return 14;
			case 'F':
				return 15;
			}
		}

		public static int StringToInt(string s)
		{
			int num = 0;
			for (int i = 0; i < s.Length; i++)
			{
				num += TMP_TextUtilities.HexToInt(s[i]) * (int)Mathf.Pow(16f, (float)(s.Length - 1 - i));
			}
			return num;
		}

		private static Vector3[] m_rectWorldCorners = new Vector3[4];

		private const string k_lookupStringL = "-------------------------------- !-#$%&-()*+,-./0123456789:;<=>?@abcdefghijklmnopqrstuvwxyz[-]^_`abcdefghijklmnopqrstuvwxyz{|}~-";

		private const string k_lookupStringU = "-------------------------------- !-#$%&-()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[-]^_`ABCDEFGHIJKLMNOPQRSTUVWXYZ{|}~-";

		private struct LineSegment
		{
			public LineSegment(Vector3 p1, Vector3 p2)
			{
				this.Point1 = p1;
				this.Point2 = p2;
			}

			public Vector3 Point1;

			public Vector3 Point2;
		}
	}
}
