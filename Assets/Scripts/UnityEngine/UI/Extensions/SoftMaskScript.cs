using System;

namespace UnityEngine.UI.Extensions
{
	[ExecuteInEditMode]
	[AddComponentMenu("UI/Effects/Extensions/SoftMaskScript")]
	public class SoftMaskScript : MonoBehaviour
	{
		private void Start()
		{
			this.myRect = base.GetComponent<RectTransform>();
			if (!this.MaskArea)
			{
				this.MaskArea = this.myRect;
			}
			if (base.GetComponent<Graphic>() != null)
			{
				this.mat = new Material(Shader.Find("UI Extensions/SoftMaskShader"));
				base.GetComponent<Graphic>().material = this.mat;
			}
			if (base.GetComponent<Text>())
			{
				this.isText = true;
				this.mat = new Material(Shader.Find("UI Extensions/SoftMaskShaderText"));
				base.GetComponent<Text>().material = this.mat;
				this.GetCanvas();
				if (base.transform.parent.GetComponent<Button>() == null && base.transform.parent.GetComponent<Mask>() == null)
				{
					base.transform.parent.gameObject.AddComponent<Mask>();
				}
				if (base.transform.parent.GetComponent<Mask>() != null)
				{
					base.transform.parent.GetComponent<Mask>().enabled = false;
				}
			}
			if (this.CascadeToALLChildren)
			{
				for (int i = 0; i < base.transform.childCount; i++)
				{
					this.SetSAM(base.transform.GetChild(i));
				}
			}
			this.MaterialNotSupported = (this.mat == null);
		}

		private void SetSAM(Transform t)
		{
			SoftMaskScript softMaskScript = t.gameObject.GetComponent<SoftMaskScript>();
			if (softMaskScript == null)
			{
				softMaskScript = t.gameObject.AddComponent<SoftMaskScript>();
			}
			softMaskScript.MaskArea = this.MaskArea;
			softMaskScript.AlphaMask = this.AlphaMask;
			softMaskScript.CutOff = this.CutOff;
			softMaskScript.HardBlend = this.HardBlend;
			softMaskScript.FlipAlphaMask = this.FlipAlphaMask;
			softMaskScript.maskScalingRect = this.maskScalingRect;
			softMaskScript.DontClipMaskScalingRect = this.DontClipMaskScalingRect;
			softMaskScript.CascadeToALLChildren = this.CascadeToALLChildren;
		}

		private void GetCanvas()
		{
			Transform transform = base.transform;
			int num = 100;
			int num2 = 0;
			while (this.canvas == null && num2 < num)
			{
				this.canvas = transform.gameObject.GetComponent<Canvas>();
				if (this.canvas == null)
				{
					transform = transform.parent;
				}
				num2++;
			}
		}

		private void Update()
		{
			this.SetMask();
		}

		private void SetMask()
		{
			if (this.MaterialNotSupported)
			{
				return;
			}
			this.maskRect = this.MaskArea.rect;
			this.contentRect = this.myRect.rect;
			if (this.isText)
			{
				this.maskScalingRect = null;
				if (this.canvas.renderMode == RenderMode.ScreenSpaceOverlay && Application.isPlaying)
				{
					this.p = this.canvas.transform.InverseTransformPoint(this.MaskArea.transform.position);
					this.siz = new Vector2(this.maskRect.width, this.maskRect.height);
				}
				else
				{
					this.worldCorners = new Vector3[4];
					this.MaskArea.GetWorldCorners(this.worldCorners);
					this.siz = this.worldCorners[2] - this.worldCorners[0];
					this.p = this.MaskArea.transform.position;
				}
				this.min = this.p - new Vector2(this.siz.x, this.siz.y) * 0.5f;
				this.max = this.p + new Vector2(this.siz.x, this.siz.y) * 0.5f;
			}
			else
			{
				if (this.maskScalingRect != null)
				{
					this.maskRect = this.maskScalingRect.rect;
				}
				if (this.maskScalingRect != null)
				{
					this.centre = this.myRect.transform.InverseTransformPoint(this.maskScalingRect.transform.TransformPoint(this.maskScalingRect.rect.center));
				}
				else
				{
					this.centre = this.myRect.transform.InverseTransformPoint(this.MaskArea.transform.TransformPoint(this.MaskArea.rect.center));
				}
				this.centre += (Vector2)this.myRect.transform.InverseTransformPoint(this.myRect.transform.position) - this.myRect.rect.center;
				this.AlphaUV = new Vector2(this.maskRect.width / this.contentRect.width, this.maskRect.height / this.contentRect.height);
				this.min = this.centre;
				this.max = this.min;
				this.siz = new Vector2(this.maskRect.width, this.maskRect.height) * 0.5f;
				this.min -= this.siz;
				this.max += this.siz;
				this.min = new Vector2(this.min.x / this.contentRect.width, this.min.y / this.contentRect.height) + this.tp;
				this.max = new Vector2(this.max.x / this.contentRect.width, this.max.y / this.contentRect.height) + this.tp;
			}
			this.mat.SetFloat("_HardBlend", (float)((!this.HardBlend) ? 0 : 1));
			this.mat.SetVector("_Min", this.min);
			this.mat.SetVector("_Max", this.max);
			this.mat.SetInt("_FlipAlphaMask", (!this.FlipAlphaMask) ? 0 : 1);
			this.mat.SetTexture("_AlphaMask", this.AlphaMask);
			this.mat.SetInt("_NoOuterClip", (!this.DontClipMaskScalingRect || !(this.maskScalingRect != null)) ? 0 : 1);
			if (!this.isText)
			{
				this.mat.SetVector("_AlphaUV", this.AlphaUV);
			}
			this.mat.SetFloat("_CutOff", this.CutOff);
		}

		private Material mat;

		private Canvas canvas;

		[Tooltip("The area that is to be used as the container.")]
		public RectTransform MaskArea;

		private RectTransform myRect;

		[Tooltip("A Rect Transform that can be used to scale and move the mask - Does not apply to Text UI Components being masked")]
		public RectTransform maskScalingRect;

		[Tooltip("Texture to be used to do the soft alpha")]
		public Texture AlphaMask;

		[Tooltip("At what point to apply the alpha min range 0-1")]
		[Range(0f, 1f)]
		public float CutOff;

		[Tooltip("Implement a hard blend based on the Cutoff")]
		public bool HardBlend;

		[Tooltip("Flip the masks alpha value")]
		public bool FlipAlphaMask;

		[Tooltip("If Mask Scaling Rect is given and this value is true, the area around the mask will not be clipped")]
		public bool DontClipMaskScalingRect;

		[Tooltip("If set to true, this mask is applied to all child Text and Graphic objects belonging to this object.")]
		public bool CascadeToALLChildren;

		private Vector3[] worldCorners;

		private Vector2 AlphaUV;

		private Vector2 min;

		private Vector2 max = Vector2.one;

		private Vector2 p;

		private Vector2 siz;

		private Vector2 tp = new Vector2(0.5f, 0.5f);

		private bool MaterialNotSupported;

		private Rect maskRect;

		private Rect contentRect;

		private Vector2 centre;

		private bool isText;

		private Sprite maskRectSprite;
	}
}
