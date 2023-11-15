using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.Water
{
	public class Water : MonoBehaviour
	{
		private void Awake()
		{
			this.myRenderer = base.GetComponent<Renderer>();
			this.material_horizon_color = Shader.PropertyToID("_Color");
			this.material_WaveSpeed = Shader.PropertyToID("WaveSpeed");
			this.material_WaveScale = Shader.PropertyToID("_WaveScale");
			this.material_WaveOffset = Shader.PropertyToID("_WaveOffset");
			this.material_WaveScale4 = Shader.PropertyToID("_WaveScale4");
			this.material_ReflectionTex = Shader.PropertyToID("_ReflectionTex");
			this.material_RefractionTex = Shader.PropertyToID("_RefractionTex");
			Shader.EnableKeyword("WATER_REFRACTIVE");
			this.pos = base.transform.position;
			this.normal = base.transform.up;
			this.mat = this.myRenderer.sharedMaterial;
		}

		private void Start()
		{
			CameraColorChangeListener.OnCameraColorChange += this.CameraColorChangeListener_OnCameraColorChange;
			if (ScreenManager.Instance != null)
			{
				ScreenManager.Instance.OnScreenTransitionStarted += this.Instance_OnScreenTransitionStarted;
				ScreenManager.Instance.OnScreenTransitionEnded += this.Instance_OnScreenTransitionEnded;
			}
			this.CreateWaterObjects(this.mainCamera, out this.reflectionCamera, out this.refractionCamera);
		}

		private void Instance_OnScreenTransitionStarted(ScreenManager.Screen to, ScreenManager.Screen from)
		{
			if (to == ScreenManager.Screen.Main || to == ScreenManager.Screen.Tutorial || to == ScreenManager.Screen.Tournament)
			{
				if (!SettingsManager.Instance.IsHighQuality)
				{
					return;
				}
				this.myRenderer.enabled = true;
			}
		}

		private void Instance_OnScreenTransitionEnded(ScreenManager.Screen to, ScreenManager.Screen from)
		{
			if (to != ScreenManager.Screen.Main && to != ScreenManager.Screen.Tutorial && to != ScreenManager.Screen.Tournament)
			{
				this.myRenderer.enabled = false;
			}
		}

		private void CameraColorChangeListener_OnCameraColorChange(Color arg1, Color arg2)
		{
			if (this.waterMode == Water.WaterMode.Simple)
			{
				this.myRenderer.sharedMaterial.SetColor(this.material_horizon_color, arg1);
			}
		}

		public void OnWillRenderObject()
		{
			if (!base.enabled || !this.myRenderer || !this.myRenderer.sharedMaterial || !this.myRenderer.enabled)
			{
				return;
			}
			if (Water.s_InsideWater)
			{
				return;
			}
			Water.s_InsideWater = true;
			this.m_HardwareWaterSupport = this.FindHardwareWaterSupport();
			Water.WaterMode waterMode = this.GetWaterMode();
			int pixelLightCount = QualitySettings.pixelLightCount;
			if (this.disablePixelLights)
			{
				QualitySettings.pixelLightCount = 0;
			}
			this.UpdateCameraModes(this.mainCamera, this.reflectionCamera);
			this.UpdateCameraModes(this.mainCamera, this.refractionCamera);
			if (waterMode >= Water.WaterMode.Reflective)
			{
				float w = -Vector3.Dot(this.normal, this.pos) - this.clipPlaneOffset;
				Vector4 plane = new Vector4(this.normal.x, this.normal.y, this.normal.z, w);
				Matrix4x4 zero = Matrix4x4.zero;
				Water.CalculateReflectionMatrix(ref zero, plane);
				Vector3 position = this.mainCamera.transform.position;
				Vector3 position2 = zero.MultiplyPoint(position);
				this.reflectionCamera.worldToCameraMatrix = this.mainCamera.worldToCameraMatrix * zero;
				Vector4 clipPlane = this.CameraSpacePlane(this.reflectionCamera, this.pos, this.normal, 1f);
				this.reflectionCamera.projectionMatrix = this.mainCamera.CalculateObliqueMatrix(clipPlane);
				this.reflectionCamera.cullingMatrix = this.mainCamera.projectionMatrix * this.mainCamera.worldToCameraMatrix;
				this.reflectionCamera.cullingMask = (-17 & this.reflectLayers.value);
				this.reflectionCamera.targetTexture = this.m_ReflectionTexture;
				bool invertCulling = GL.invertCulling;
				GL.invertCulling = !invertCulling;
				this.reflectionCamera.transform.position = position2;
				Vector3 eulerAngles = this.mainCamera.transform.eulerAngles;
				this.reflectionCamera.transform.eulerAngles = new Vector3(-eulerAngles.x, eulerAngles.y, eulerAngles.z);
				this.reflectionCamera.Render();
				this.reflectionCamera.transform.position = position;
				GL.invertCulling = invertCulling;
				this.myRenderer.sharedMaterial.SetTexture(this.material_ReflectionTex, this.m_ReflectionTexture);
			}
			if (waterMode >= Water.WaterMode.Refractive)
			{
				this.refractionCamera.worldToCameraMatrix = this.mainCamera.worldToCameraMatrix;
				Vector4 clipPlane2 = this.CameraSpacePlane(this.refractionCamera, this.pos, this.normal, -1f);
				this.refractionCamera.projectionMatrix = this.mainCamera.CalculateObliqueMatrix(clipPlane2);
				this.refractionCamera.cullingMatrix = this.mainCamera.projectionMatrix * this.mainCamera.worldToCameraMatrix;
				this.refractionCamera.cullingMask = (-17 & this.refractLayers.value);
				this.refractionCamera.targetTexture = this.m_RefractionTexture;
				this.refractionCamera.transform.position = this.mainCamera.transform.position;
				this.refractionCamera.transform.rotation = this.mainCamera.transform.rotation;
				this.refractionCamera.Render();
				this.myRenderer.sharedMaterial.SetTexture(this.material_RefractionTex, this.m_RefractionTexture);
			}
			if (this.disablePixelLights)
			{
				QualitySettings.pixelLightCount = pixelLightCount;
			}
			Water.s_InsideWater = false;
		}

		private void OnDisable()
		{
			if (this.m_ReflectionTexture)
			{
				UnityEngine.Object.DestroyImmediate(this.m_ReflectionTexture);
				this.m_ReflectionTexture = null;
			}
			if (this.m_RefractionTexture)
			{
				UnityEngine.Object.DestroyImmediate(this.m_RefractionTexture);
				this.m_RefractionTexture = null;
			}
			foreach (KeyValuePair<Camera, Camera> keyValuePair in this.m_ReflectionCameras)
			{
				UnityEngine.Object.DestroyImmediate(keyValuePair.Value.gameObject);
			}
			this.m_ReflectionCameras.Clear();
			foreach (KeyValuePair<Camera, Camera> keyValuePair2 in this.m_RefractionCameras)
			{
				UnityEngine.Object.DestroyImmediate(keyValuePair2.Value.gameObject);
			}
			this.m_RefractionCameras.Clear();
		}

		private void Update()
		{
			if (!this.myRenderer)
			{
				return;
			}
			if (!this.mat)
			{
				return;
			}
			Vector4 vector = this.mat.GetVector(this.material_WaveSpeed);
			float @float = this.mat.GetFloat(this.material_WaveScale);
			Vector4 value = new Vector4(@float, @float, @float * 0.4f, @float * 0.45f);
			double num = (double)Time.timeSinceLevelLoad / 20.0;
			Vector4 value2 = new Vector4((float)Math.IEEERemainder((double)(vector.x * value.x) * num, 1.0), (float)Math.IEEERemainder((double)(vector.y * value.y) * num + (double)(-0.3f * BoatMovementHandler.boatMovementSpeed * BoatMovementHandler.waterShaderMovementFix), 1.0), (float)Math.IEEERemainder((double)(vector.z * value.z) * num, 1.0), (float)Math.IEEERemainder((double)(vector.w * value.w) * num, 1.0));
			this.mat.SetVector(this.material_WaveOffset, value2);
			this.mat.SetVector(this.material_WaveScale4, value);
		}

		public void SetLowQuality()
		{
			base.gameObject.SetActive(false);
		}

		public void SetMediumQuality()
		{
			base.gameObject.SetActive(true);
			base.transform.position = this.MediumFPSPosition;
			this.waterMode = Water.WaterMode.Simple;
			this.myRenderer.sharedMaterial.SetColor(this.material_horizon_color, Camera.main.backgroundColor);
			this.myRenderer.sharedMaterial.SetTexture("_ReflectiveColor", this.MediumFPSShaderTexture);
		}

		public void SetHighQuality()
		{
			base.gameObject.SetActive(true);
			base.transform.position = this.HighFPSPosition;
			this.waterMode = Water.WaterMode.Refractive;
			this.myRenderer.sharedMaterial.SetColor(this.material_horizon_color, this.HighFPScolor);
			this.myRenderer.sharedMaterial.SetTexture("_ReflectiveColor", this.HighFPSShaderTexture);
		}

		private void UpdateCameraModes(Camera src, Camera dest)
		{
			if (dest == null)
			{
				return;
			}
			dest.clearFlags = src.clearFlags;
			dest.backgroundColor = src.backgroundColor;
			if (src.clearFlags == CameraClearFlags.Skybox)
			{
				Skybox component = src.GetComponent<Skybox>();
				Skybox component2 = dest.GetComponent<Skybox>();
				if (!component || !component.material)
				{
					component2.enabled = false;
				}
				else
				{
					component2.enabled = true;
					component2.material = component.material;
				}
			}
			dest.farClipPlane = src.farClipPlane;
			dest.nearClipPlane = src.nearClipPlane;
			dest.orthographic = src.orthographic;
			dest.fieldOfView = src.fieldOfView;
			dest.aspect = src.aspect;
			dest.orthographicSize = src.orthographicSize;
		}

		private void CreateWaterObjects(Camera currentCamera, out Camera reflectionCamera, out Camera refractionCamera)
		{
			Water.WaterMode waterMode = this.GetWaterMode();
			reflectionCamera = null;
			refractionCamera = null;
			if (waterMode >= Water.WaterMode.Reflective)
			{
				if (!this.m_ReflectionTexture || this.m_OldReflectionTextureSize != this.textureSize)
				{
					if (this.m_ReflectionTexture)
					{
						UnityEngine.Object.DestroyImmediate(this.m_ReflectionTexture);
					}
					this.m_ReflectionTexture = new RenderTexture(this.textureSize, this.textureSize, 16);
					this.m_ReflectionTexture.name = "__WaterReflection" + base.GetInstanceID();
					this.m_ReflectionTexture.isPowerOfTwo = true;
					this.m_ReflectionTexture.hideFlags = HideFlags.DontSave;
					this.m_OldReflectionTextureSize = this.textureSize;
				}
				this.m_ReflectionCameras.TryGetValue(currentCamera, out reflectionCamera);
				if (!reflectionCamera)
				{
					GameObject gameObject = new GameObject(string.Concat(new object[]
					{
						"Water Refl Camera id",
						base.GetInstanceID(),
						" for ",
						currentCamera.GetInstanceID()
					}), new Type[]
					{
						typeof(Camera),
						typeof(Skybox)
					});
					reflectionCamera = gameObject.GetComponent<Camera>();
					reflectionCamera.enabled = false;
					reflectionCamera.transform.position = base.transform.position;
					reflectionCamera.transform.rotation = base.transform.rotation;
					reflectionCamera.gameObject.AddComponent<FlareLayer>();
					gameObject.hideFlags = HideFlags.HideAndDontSave;
					this.m_ReflectionCameras[currentCamera] = reflectionCamera;
				}
			}
			if (waterMode >= Water.WaterMode.Refractive)
			{
				if (!this.m_RefractionTexture || this.m_OldRefractionTextureSize != this.textureSize)
				{
					if (this.m_RefractionTexture)
					{
						UnityEngine.Object.DestroyImmediate(this.m_RefractionTexture);
					}
					this.m_RefractionTexture = new RenderTexture(this.textureSize, this.textureSize, 16);
					this.m_RefractionTexture.name = "__WaterRefraction" + base.GetInstanceID();
					this.m_RefractionTexture.isPowerOfTwo = true;
					this.m_RefractionTexture.hideFlags = HideFlags.DontSave;
					this.m_OldRefractionTextureSize = this.textureSize;
				}
				this.m_RefractionCameras.TryGetValue(currentCamera, out refractionCamera);
				if (!refractionCamera)
				{
					GameObject gameObject2 = new GameObject(string.Concat(new object[]
					{
						"Water Refr Camera id",
						base.GetInstanceID(),
						" for ",
						currentCamera.GetInstanceID()
					}), new Type[]
					{
						typeof(Camera),
						typeof(Skybox)
					});
					refractionCamera = gameObject2.GetComponent<Camera>();
					refractionCamera.enabled = false;
					refractionCamera.transform.position = base.transform.position;
					refractionCamera.transform.rotation = base.transform.rotation;
					refractionCamera.gameObject.AddComponent<FlareLayer>();
					gameObject2.hideFlags = HideFlags.HideAndDontSave;
					this.m_RefractionCameras[currentCamera] = refractionCamera;
				}
			}
		}

		private Water.WaterMode GetWaterMode()
		{
			return this.waterMode;
		}

		private Water.WaterMode FindHardwareWaterSupport()
		{
			return Water.WaterMode.Refractive;
		}

		private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
		{
			Vector3 point = pos + normal * this.clipPlaneOffset;
			Matrix4x4 worldToCameraMatrix = cam.worldToCameraMatrix;
			Vector3 lhs = worldToCameraMatrix.MultiplyPoint(point);
			Vector3 rhs = worldToCameraMatrix.MultiplyVector(normal).normalized * sideSign;
			return new Vector4(rhs.x, rhs.y, rhs.z, -Vector3.Dot(lhs, rhs));
		}

		private static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane)
		{
			reflectionMat.m00 = 1f - 2f * plane[0] * plane[0];
			reflectionMat.m01 = -2f * plane[0] * plane[1];
			reflectionMat.m02 = -2f * plane[0] * plane[2];
			reflectionMat.m03 = -2f * plane[3] * plane[0];
			reflectionMat.m10 = -2f * plane[1] * plane[0];
			reflectionMat.m11 = 1f - 2f * plane[1] * plane[1];
			reflectionMat.m12 = -2f * plane[1] * plane[2];
			reflectionMat.m13 = -2f * plane[3] * plane[1];
			reflectionMat.m20 = -2f * plane[2] * plane[0];
			reflectionMat.m21 = -2f * plane[2] * plane[1];
			reflectionMat.m22 = 1f - 2f * plane[2] * plane[2];
			reflectionMat.m23 = -2f * plane[3] * plane[2];
			reflectionMat.m30 = 0f;
			reflectionMat.m31 = 0f;
			reflectionMat.m32 = 0f;
			reflectionMat.m33 = 1f;
		}

		public Texture HighFPSShaderTexture;

		public Texture MediumFPSShaderTexture;

		public Vector3 HighFPSPosition;

		public Vector3 MediumFPSPosition;

		public Color HighFPScolor;

		public Water.WaterMode waterMode = Water.WaterMode.Refractive;

		public bool disablePixelLights = true;

		public int textureSize = 256;

		public float clipPlaneOffset = 0.07f;

		public LayerMask reflectLayers = -1;

		public LayerMask refractLayers = -1;

		private Dictionary<Camera, Camera> m_ReflectionCameras = new Dictionary<Camera, Camera>();

		private Dictionary<Camera, Camera> m_RefractionCameras = new Dictionary<Camera, Camera>();

		private RenderTexture m_ReflectionTexture;

		private RenderTexture m_RefractionTexture;

		private Water.WaterMode m_HardwareWaterSupport = Water.WaterMode.Refractive;

		private int m_OldReflectionTextureSize;

		private int m_OldRefractionTextureSize;

		private static bool s_InsideWater;

		private Renderer myRenderer;

		private int material_horizon_color;

		private int material_WaveSpeed;

		private int material_WaveScale;

		private int material_WaveOffset;

		private int material_WaveScale4;

		private int material_ReflectionTex;

		private int material_RefractionTex;

		[SerializeField]
		private Camera mainCamera;

		private Vector3 pos;

		private Vector3 normal;

		private Material mat;

		private Camera reflectionCamera;

		private Camera refractionCamera;

		public enum WaterMode
		{
			Simple,
			Reflective,
			Refractive
		}
	}
}
