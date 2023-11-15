using System;
using System.Collections.Generic;
using System.Linq;
using SoftMasking.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoftMasking
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(RectTransform))]
	[ExecuteInEditMode]
	[AddComponentMenu("UI/Soft Mask", 14)]
	[HelpURL("https://docs.google.com/document/d/1XhJFNFHNyKXwWsErLkd1FBw0YgOCeo4qkjrMW9_H-hc")]
	public class SoftMask : UIBehaviour, ISoftMask, ICanvasRaycastFilter
	{
		public SoftMask()
		{
			MaterialReplacerChain replacer = new MaterialReplacerChain(MaterialReplacer.globalReplacers, new SoftMask.MaterialReplacerImpl(this));
			this._materials = new MaterialReplacements(replacer, delegate(Material m)
			{
				this._parameters.Apply(m);
			});
		}

		public Shader defaultShader
		{
			get
			{
				return this._defaultShader;
			}
			set
			{
				this.SetShader(ref this._defaultShader, value, true);
			}
		}

		public Shader defaultETC1Shader
		{
			get
			{
				return this._defaultETC1Shader;
			}
			set
			{
				this.SetShader(ref this._defaultETC1Shader, value, false);
			}
		}

		public SoftMask.MaskSource source
		{
			get
			{
				return this._source;
			}
			set
			{
				if (this._source != value)
				{
					this.Set<SoftMask.MaskSource>(ref this._source, value);
				}
			}
		}

		public RectTransform separateMask
		{
			get
			{
				return this._separateMask;
			}
			set
			{
				if (this._separateMask != value)
				{
					this.Set<RectTransform>(ref this._separateMask, value);
					this._graphic = null;
					this._maskTransform = null;
				}
			}
		}

		public Sprite sprite
		{
			get
			{
				return this._sprite;
			}
			set
			{
				if (this._sprite != value)
				{
					this.Set<Sprite>(ref this._sprite, value);
				}
			}
		}

		public SoftMask.BorderMode spriteBorderMode
		{
			get
			{
				return this._spriteBorderMode;
			}
			set
			{
				if (this._spriteBorderMode != value)
				{
					this.Set<SoftMask.BorderMode>(ref this._spriteBorderMode, value);
				}
			}
		}

		public Texture2D texture
		{
			get
			{
				return this._texture;
			}
			set
			{
				if (this._texture != value)
				{
					this.Set<Texture2D>(ref this._texture, value);
				}
			}
		}

		public Rect textureUVRect
		{
			get
			{
				return this._textureUVRect;
			}
			set
			{
				if (this._textureUVRect != value)
				{
					this.Set<Rect>(ref this._textureUVRect, value);
				}
			}
		}

		public Color channelWeights
		{
			get
			{
				return this._channelWeights;
			}
			set
			{
				if (this._channelWeights != value)
				{
					this.Set<Color>(ref this._channelWeights, value);
				}
			}
		}

		public float raycastThreshold
		{
			get
			{
				return this._raycastThreshold;
			}
			set
			{
				this._raycastThreshold = value;
			}
		}

		public bool isUsingRaycastFiltering
		{
			get
			{
				return this._raycastThreshold > 0f;
			}
		}

		public bool isMaskingEnabled
		{
			get
			{
				return base.isActiveAndEnabled && this.canvas;
			}
		}

		public SoftMask.Errors PollErrors()
		{
			SoftMask.Diagnostics diagnostics = new SoftMask.Diagnostics(this);
			return diagnostics.PollErrors();
		}

		public bool IsRaycastLocationValid(Vector2 sp, Camera cam)
		{
			Vector2 vector;
			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(this.maskTransform, sp, cam, out vector))
			{
				return false;
			}
			if (!SoftMask.Mathr.Inside(vector, this.LocalMaskRect(Vector4.zero)))
			{
				return false;
			}
			if (!this._parameters.texture)
			{
				return true;
			}
			if (!this.isUsingRaycastFiltering)
			{
				return true;
			}
			float num;
			if (!this._parameters.SampleMask(vector, out num))
			{
				UnityEngine.Debug.LogErrorFormat(this, "Raycast Threshold greater than 0 can't be used on Soft Mask with texture '{0}' because it's not readable. You can make the texture readable in the Texture Import Settings.", new object[]
				{
					this._parameters.activeTexture.name
				});
				return true;
			}
			return num >= this._raycastThreshold;
		}

		protected override void Start()
		{
			base.Start();
			this.WarnIfDefaultShaderIsNotSet();
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			this.SubscribeOnWillRenderCanvases();
			this.SpawnMaskablesInChildren(base.transform);
			this.FindGraphic();
			if (this.isMaskingEnabled)
			{
				this.UpdateMaskParameters();
			}
			this.NotifyChildrenThatMaskMightChanged();
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			this.UnsubscribeFromWillRenderCanvases();
			if (this._graphic)
			{
				this._graphic.UnregisterDirtyVerticesCallback(new UnityAction(this.OnGraphicDirty));
				this._graphic.UnregisterDirtyMaterialCallback(new UnityAction(this.OnGraphicDirty));
				this._graphic = null;
			}
			this.NotifyChildrenThatMaskMightChanged();
			this.DestroyMaterials();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			this._destroyed = true;
			this.NotifyChildrenThatMaskMightChanged();
		}

		protected virtual void LateUpdate()
		{
			bool isMaskingEnabled = this.isMaskingEnabled;
			if (isMaskingEnabled)
			{
				if (this._maskingWasEnabled != isMaskingEnabled)
				{
					this.SpawnMaskablesInChildren(base.transform);
				}
				Graphic graphic = this._graphic;
				this.FindGraphic();
				if (this._lastMaskRect != this.maskTransform.rect || !object.ReferenceEquals(this._graphic, graphic))
				{
					this._dirty = true;
				}
			}
			this._maskingWasEnabled = isMaskingEnabled;
		}

		protected override void OnRectTransformDimensionsChange()
		{
			base.OnRectTransformDimensionsChange();
			this._dirty = true;
		}

		protected override void OnDidApplyAnimationProperties()
		{
			base.OnDidApplyAnimationProperties();
			this._dirty = true;
		}

		protected override void OnTransformParentChanged()
		{
			base.OnTransformParentChanged();
			this._canvas = null;
			this._dirty = true;
		}

		protected override void OnCanvasHierarchyChanged()
		{
			base.OnCanvasHierarchyChanged();
			this._canvas = null;
			this._dirty = true;
			this.NotifyChildrenThatMaskMightChanged();
		}

		private void OnTransformChildrenChanged()
		{
			this.SpawnMaskablesInChildren(base.transform);
		}

		private void SubscribeOnWillRenderCanvases()
		{
			SoftMask.Touch<CanvasUpdateRegistry>(CanvasUpdateRegistry.instance);
			Canvas.willRenderCanvases += this.OnWillRenderCanvases;
		}

		private void UnsubscribeFromWillRenderCanvases()
		{
			Canvas.willRenderCanvases -= this.OnWillRenderCanvases;
		}

		private void OnWillRenderCanvases()
		{
			if (this.isMaskingEnabled)
			{
				this.UpdateMaskParameters();
			}
		}

		private static T Touch<T>(T obj)
		{
			return obj;
		}

		private RectTransform maskTransform
		{
			get
			{
				return (!this._maskTransform) ? (this._maskTransform = ((!this._separateMask) ? base.GetComponent<RectTransform>() : this._separateMask)) : this._maskTransform;
			}
		}

		private Canvas canvas
		{
			get
			{
				return (!this._canvas) ? (this._canvas = this.NearestEnabledCanvas()) : this._canvas;
			}
		}

		private bool isBasedOnGraphic
		{
			get
			{
				return this._source == SoftMask.MaskSource.Graphic;
			}
		}

		bool ISoftMask.isAlive
		{
			get
			{
				return this && !this._destroyed;
			}
		}

		Material ISoftMask.GetReplacement(Material original)
		{
			return this._materials.Get(original);
		}

		void ISoftMask.ReleaseReplacement(Material replacement)
		{
			this._materials.Release(replacement);
		}

		void ISoftMask.UpdateTransformChildren(Transform transform)
		{
			this.SpawnMaskablesInChildren(transform);
		}

		private void OnGraphicDirty()
		{
			if (this.isBasedOnGraphic)
			{
				this._dirty = true;
			}
		}

		private void FindGraphic()
		{
			if (!this._graphic && this.isBasedOnGraphic)
			{
				this._graphic = this.maskTransform.GetComponent<Graphic>();
				if (this._graphic)
				{
					this._graphic.RegisterDirtyVerticesCallback(new UnityAction(this.OnGraphicDirty));
					this._graphic.RegisterDirtyMaterialCallback(new UnityAction(this.OnGraphicDirty));
				}
			}
		}

		private Canvas NearestEnabledCanvas()
		{
			Canvas[] componentsInParent = base.GetComponentsInParent<Canvas>(false);
			for (int i = 0; i < componentsInParent.Length; i++)
			{
				if (componentsInParent[i].isActiveAndEnabled)
				{
					return componentsInParent[i];
				}
			}
			return null;
		}

		private void UpdateMaskParameters()
		{
			if (this._dirty || this.maskTransform.hasChanged)
			{
				this.CalculateMaskParameters();
				this.maskTransform.hasChanged = false;
				this._lastMaskRect = this.maskTransform.rect;
				this._dirty = false;
			}
			this._materials.ApplyAll();
		}

		private void SpawnMaskablesInChildren(Transform root)
		{
			using (new ClearListAtExit<SoftMaskable>(SoftMask.s_maskables))
			{
				for (int i = 0; i < root.childCount; i++)
				{
					Transform child = root.GetChild(i);
					child.GetComponents<SoftMaskable>(SoftMask.s_maskables);
					if (SoftMask.s_maskables.Count == 0)
					{
						child.gameObject.AddComponent<SoftMaskable>();
					}
				}
			}
		}

		private void InvalidateChildren()
		{
			this.ForEachChildMaskable(delegate(SoftMaskable x)
			{
				x.Invalidate();
			});
		}

		private void NotifyChildrenThatMaskMightChanged()
		{
			this.ForEachChildMaskable(delegate(SoftMaskable x)
			{
				x.MaskMightChanged();
			});
		}

		private void ForEachChildMaskable(Action<SoftMaskable> f)
		{
			base.transform.GetComponentsInChildren<SoftMaskable>(SoftMask.s_maskables);
			using (new ClearListAtExit<SoftMaskable>(SoftMask.s_maskables))
			{
				for (int i = 0; i < SoftMask.s_maskables.Count; i++)
				{
					SoftMaskable softMaskable = SoftMask.s_maskables[i];
					if (softMaskable && softMaskable.gameObject != base.gameObject)
					{
						f(softMaskable);
					}
				}
			}
		}

		private void DestroyMaterials()
		{
			this._materials.DestroyAllAndClear();
		}

		private SoftMask.SourceParameters DeduceSourceParameters()
		{
			SoftMask.SourceParameters result = default(SoftMask.SourceParameters);
			switch (this._source)
			{
			case SoftMask.MaskSource.Graphic:
				if (this._graphic is Image)
				{
					result.image = (Image)this._graphic;
					result.sprite = result.image.sprite;
					result.spriteBorderMode = this.ToBorderMode(result.image.type);
					result.texture = ((!result.sprite) ? null : result.sprite.texture);
				}
				else if (this._graphic is RawImage)
				{
					RawImage rawImage = (RawImage)this._graphic;
					result.texture = (rawImage.texture as Texture2D);
					result.textureUVRect = rawImage.uvRect;
				}
				break;
			case SoftMask.MaskSource.Sprite:
				result.sprite = this._sprite;
				result.spriteBorderMode = this._spriteBorderMode;
				result.texture = ((!result.sprite) ? null : result.sprite.texture);
				break;
			case SoftMask.MaskSource.Texture:
				result.texture = this._texture;
				result.textureUVRect = this._textureUVRect;
				break;
			default:
				UnityEngine.Debug.LogErrorFormat(this, "Unknown MaskSource: {0}", new object[]
				{
					this._source
				});
				break;
			}
			return result;
		}

		private SoftMask.BorderMode ToBorderMode(Image.Type imageType)
		{
			switch (imageType)
			{
			case Image.Type.Simple:
				return SoftMask.BorderMode.Simple;
			case Image.Type.Sliced:
				return SoftMask.BorderMode.Sliced;
			case Image.Type.Tiled:
				return SoftMask.BorderMode.Tiled;
			default:
				UnityEngine.Debug.LogErrorFormat(this, "SoftMask doesn't support image type {0}. Image type Simple will be used.", new object[]
				{
					imageType
				});
				return SoftMask.BorderMode.Simple;
			}
		}

		private void CalculateMaskParameters()
		{
			SoftMask.SourceParameters sourceParameters = this.DeduceSourceParameters();
			if (sourceParameters.sprite)
			{
				this.CalculateSpriteBased(sourceParameters.sprite, sourceParameters.spriteBorderMode);
			}
			else if (sourceParameters.texture)
			{
				this.CalculateTextureBased(sourceParameters.texture, sourceParameters.textureUVRect);
			}
			else
			{
				this.CalculateSolidFill();
			}
		}

		private void CalculateSpriteBased(Sprite sprite, SoftMask.BorderMode borderMode)
		{
			Sprite lastUsedSprite = this._lastUsedSprite;
			this._lastUsedSprite = sprite;
			SoftMask.Errors errors = SoftMask.Diagnostics.CheckSprite(sprite);
			if (errors != SoftMask.Errors.NoError)
			{
				if (lastUsedSprite != sprite)
				{
					this.WarnSpriteErrors(errors);
				}
				this.CalculateSolidFill();
				return;
			}
			if (!sprite)
			{
				this.CalculateSolidFill();
				return;
			}
			this.FillCommonParameters();
			Vector4 vector = SoftMask.Mathr.Move(SoftMask.Mathr.ToVector(sprite.rect), sprite.textureRect.position - sprite.rect.position - sprite.textureRectOffset);
			Vector4 vector2 = SoftMask.Mathr.ToVector(sprite.textureRect);
			Vector4 vector3 = SoftMask.Mathr.BorderOf(vector, vector2);
			Vector2 s = new Vector2((float)sprite.texture.width, (float)sprite.texture.height);
			Vector4 vector4 = this.LocalMaskRect(Vector4.zero);
			this._parameters.maskRectUV = SoftMask.Mathr.Div(vector2, s);
			if (borderMode == SoftMask.BorderMode.Simple)
			{
				Vector4 v = SoftMask.Mathr.Div(vector3, SoftMask.Mathr.Size(vector));
				this._parameters.maskRect = SoftMask.Mathr.ApplyBorder(vector4, SoftMask.Mathr.Mul(v, SoftMask.Mathr.Size(vector4)));
			}
			else
			{
				this._parameters.maskRect = SoftMask.Mathr.ApplyBorder(vector4, vector3 * this.GraphicToCanvasScale(sprite));
				Vector4 v2 = SoftMask.Mathr.Div(vector, s);
				Vector4 border = SoftMask.AdjustBorders(sprite.border * this.GraphicToCanvasScale(sprite), vector4);
				this._parameters.maskBorder = this.LocalMaskRect(border);
				this._parameters.maskBorderUV = SoftMask.Mathr.ApplyBorder(v2, SoftMask.Mathr.Div(sprite.border, s));
			}
			this._parameters.texture = sprite.texture;
			this._parameters.borderMode = borderMode;
			if (borderMode == SoftMask.BorderMode.Tiled)
			{
				this._parameters.tileRepeat = this.MaskRepeat(sprite, this._parameters.maskBorder);
			}
		}

		private static Vector4 AdjustBorders(Vector4 border, Vector4 rect)
		{
			Vector2 vector = SoftMask.Mathr.Size(rect);
			for (int i = 0; i <= 1; i++)
			{
				float num = border[i] + border[i + 2];
				if (vector[i] < num && num != 0f)
				{
					float num2 = vector[i] / num;
					Vector4 ptr = border;
					int index=i;
					border[index = i] = ptr[index] * num2;
					ptr = border;
					int index2 = i + 2;
					border[index2 = i + 2] = ptr[index2] * num2;
				}
			}
			return border;
		}

		private void CalculateTextureBased(Texture2D texture, Rect uvRect)
		{
			this.FillCommonParameters();
			this._parameters.maskRect = this.LocalMaskRect(Vector4.zero);
			this._parameters.maskRectUV = SoftMask.Mathr.ToVector(uvRect);
			this._parameters.texture = texture;
			this._parameters.borderMode = SoftMask.BorderMode.Simple;
		}

		private void CalculateSolidFill()
		{
			this.CalculateTextureBased(null, SoftMask.DefaultUVRect);
		}

		private void FillCommonParameters()
		{
			this._parameters.worldToMask = this.WorldToMask();
			this._parameters.maskChannelWeights = this._channelWeights;
		}

		private float GraphicToCanvasScale(Sprite sprite)
		{
			float num = (!this.canvas) ? 100f : this.canvas.referencePixelsPerUnit;
			float num2 = (!sprite) ? 100f : sprite.pixelsPerUnit;
			return num / num2;
		}

		private Matrix4x4 WorldToMask()
		{
			return this.maskTransform.worldToLocalMatrix * this.canvas.rootCanvas.transform.localToWorldMatrix;
		}

		private Vector4 LocalMaskRect(Vector4 border)
		{
			return SoftMask.Mathr.ApplyBorder(SoftMask.Mathr.ToVector(this.maskTransform.rect), border);
		}

		private Vector2 MaskRepeat(Sprite sprite, Vector4 centralPart)
		{
			Vector4 r = SoftMask.Mathr.ApplyBorder(SoftMask.Mathr.ToVector(sprite.textureRect), sprite.border);
			return SoftMask.Mathr.Div(SoftMask.Mathr.Size(centralPart) * this.GraphicToCanvasScale(sprite), SoftMask.Mathr.Size(r));
		}

		private void WarnIfDefaultShaderIsNotSet()
		{
			if (!this._defaultShader)
			{
				UnityEngine.Debug.LogWarning("SoftMask may not work because its defaultShader is not set", this);
			}
		}

		private void WarnSpriteErrors(SoftMask.Errors errors)
		{
			if ((errors & SoftMask.Errors.TightPackedSprite) != SoftMask.Errors.NoError)
			{
				UnityEngine.Debug.LogError("SoftMask doesn't support tight packed sprites", this);
			}
			if ((errors & SoftMask.Errors.AlphaSplitSprite) != SoftMask.Errors.NoError)
			{
				UnityEngine.Debug.LogError("SoftMask doesn't support sprites with an alpha split texture", this);
			}
		}

		private void Set<T>(ref T field, T value)
		{
			field = value;
			this._dirty = true;
		}

		private void SetShader(ref Shader field, Shader value, bool warnIfNotSet = true)
		{
			if (field != value)
			{
				field = value;
				if (warnIfNotSet)
				{
					this.WarnIfDefaultShaderIsNotSet();
				}
				this.DestroyMaterials();
				this.InvalidateChildren();
			}
		}

		[SerializeField]
		private Shader _defaultShader;

		[SerializeField]
		private Shader _defaultETC1Shader;

		[SerializeField]
		private SoftMask.MaskSource _source;

		[SerializeField]
		private RectTransform _separateMask;

		[SerializeField]
		private Sprite _sprite;

		[SerializeField]
		private SoftMask.BorderMode _spriteBorderMode;

		[SerializeField]
		private Texture2D _texture;

		[SerializeField]
		private Rect _textureUVRect = SoftMask.DefaultUVRect;

		[SerializeField]
		private Color _channelWeights = MaskChannel.alpha;

		[SerializeField]
		private float _raycastThreshold;

		private MaterialReplacements _materials;

		private SoftMask.MaterialParameters _parameters;

		private Sprite _lastUsedSprite;

		private Rect _lastMaskRect;

		private bool _maskingWasEnabled;

		private bool _destroyed;

		private bool _dirty;

		private RectTransform _maskTransform;

		private Graphic _graphic;

		private Canvas _canvas;

		private static readonly Rect DefaultUVRect = new Rect(0f, 0f, 1f, 1f);

		private static readonly List<SoftMask> s_masks = new List<SoftMask>();

		private static readonly List<SoftMaskable> s_maskables = new List<SoftMaskable>();

		[Serializable]
		public enum MaskSource
		{
			Graphic,
			Sprite,
			Texture
		}

		[Serializable]
		public enum BorderMode
		{
			Simple,
			Sliced,
			Tiled
		}

		[Flags]
		[Serializable]
		public enum Errors
		{
			NoError = 0,
			UnsupportedShaders = 1,
			NestedMasks = 2,
			TightPackedSprite = 4,
			AlphaSplitSprite = 8,
			UnsupportedImageType = 16,
			UnreadableTexture = 32
		}

		private struct SourceParameters
		{
			public Image image;

			public Sprite sprite;

			public SoftMask.BorderMode spriteBorderMode;

			public Texture2D texture;

			public Rect textureUVRect;
		}

		private class MaterialReplacerImpl : IMaterialReplacer
		{
			public MaterialReplacerImpl(SoftMask owner)
			{
				this._owner = owner;
			}

			public int order
			{
				get
				{
					return 0;
				}
			}

			public Material Replace(Material original)
			{
				if (original == null || original.HasDefaultUIShader())
				{
					return SoftMask.MaterialReplacerImpl.Replace(original, this._owner._defaultShader);
				}
				if (original.HasDefaultETC1UIShader())
				{
					return SoftMask.MaterialReplacerImpl.Replace(original, this._owner._defaultETC1Shader);
				}
				if (original.SupportsSoftMask())
				{
					return new Material(original);
				}
				return null;
			}

			private static Material Replace(Material original, Shader defaultReplacementShader)
			{
				Material material = (!defaultReplacementShader) ? null : new Material(defaultReplacementShader);
				if (material && original)
				{
					material.CopyPropertiesFromMaterial(original);
				}
				return material;
			}

			private readonly SoftMask _owner;
		}

		private static class Mathr
		{
			public static Vector4 ToVector(Rect r)
			{
				return new Vector4(r.xMin, r.yMin, r.xMax, r.yMax);
			}

			public static Vector4 Div(Vector4 v, Vector2 s)
			{
				return new Vector4(v.x / s.x, v.y / s.y, v.z / s.x, v.w / s.y);
			}

			public static Vector2 Div(Vector2 v, Vector2 s)
			{
				return new Vector2(v.x / s.x, v.y / s.y);
			}

			public static Vector4 Mul(Vector4 v, Vector2 s)
			{
				return new Vector4(v.x * s.x, v.y * s.y, v.z * s.x, v.w * s.y);
			}

			public static Vector2 Size(Vector4 r)
			{
				return new Vector2(r.z - r.x, r.w - r.y);
			}

			public static Vector4 Move(Vector4 v, Vector2 o)
			{
				return new Vector4(v.x + o.x, v.y + o.y, v.z + o.x, v.w + o.y);
			}

			public static Vector4 BorderOf(Vector4 outer, Vector4 inner)
			{
				return new Vector4(inner.x - outer.x, inner.y - outer.y, outer.z - inner.z, outer.w - inner.w);
			}

			public static Vector4 ApplyBorder(Vector4 v, Vector4 b)
			{
				return new Vector4(v.x + b.x, v.y + b.y, v.z - b.z, v.w - b.w);
			}

			public static Vector2 Min(Vector4 r)
			{
				return new Vector2(r.x, r.y);
			}

			public static Vector2 Max(Vector4 r)
			{
				return new Vector2(r.z, r.w);
			}

			public static Vector2 Remap(Vector2 c, Vector4 from, Vector4 to)
			{
				Vector2 s = SoftMask.Mathr.Max(from) - SoftMask.Mathr.Min(from);
				Vector2 b = SoftMask.Mathr.Max(to) - SoftMask.Mathr.Min(to);
				return Vector2.Scale(SoftMask.Mathr.Div(c - SoftMask.Mathr.Min(from), s), b) + SoftMask.Mathr.Min(to);
			}

			public static bool Inside(Vector2 v, Vector4 r)
			{
				return v.x >= r.x && v.y >= r.y && v.x <= r.z && v.y <= r.w;
			}
		}

		private struct MaterialParameters
		{
			public Texture2D activeTexture
			{
				get
				{
					return (!this.texture) ? Texture2D.whiteTexture : this.texture;
				}
			}

			public bool SampleMask(Vector2 localPos, out float mask)
			{
				Vector2 vector = this.XY2UV(localPos);
				bool result;
				try
				{
					mask = this.MaskValue(this.texture.GetPixelBilinear(vector.x, vector.y));
					result = true;
				}
				catch (UnityException)
				{
					mask = 0f;
					result = false;
				}
				return result;
			}

			public void Apply(Material mat)
			{
				mat.SetTexture(SoftMask.MaterialParameters.Ids.SoftMask, this.activeTexture);
				mat.SetVector(SoftMask.MaterialParameters.Ids.SoftMask_Rect, this.maskRect);
				mat.SetVector(SoftMask.MaterialParameters.Ids.SoftMask_UVRect, this.maskRectUV);
				mat.SetColor(SoftMask.MaterialParameters.Ids.SoftMask_ChannelWeights, this.maskChannelWeights);
				mat.SetMatrix(SoftMask.MaterialParameters.Ids.SoftMask_WorldToMask, this.worldToMask);
				mat.EnableKeyword("SOFTMASK_SIMPLE", this.borderMode == SoftMask.BorderMode.Simple);
				mat.EnableKeyword("SOFTMASK_SLICED", this.borderMode == SoftMask.BorderMode.Sliced);
				mat.EnableKeyword("SOFTMASK_TILED", this.borderMode == SoftMask.BorderMode.Tiled);
				if (this.borderMode != SoftMask.BorderMode.Simple)
				{
					mat.SetVector(SoftMask.MaterialParameters.Ids.SoftMask_BorderRect, this.maskBorder);
					mat.SetVector(SoftMask.MaterialParameters.Ids.SoftMask_UVBorderRect, this.maskBorderUV);
					if (this.borderMode == SoftMask.BorderMode.Tiled)
					{
						mat.SetVector(SoftMask.MaterialParameters.Ids.SoftMask_TileRepeat, this.tileRepeat);
					}
				}
			}

			private Vector2 XY2UV(Vector2 localPos)
			{
				switch (this.borderMode)
				{
				case SoftMask.BorderMode.Simple:
					return this.MapSimple(localPos);
				case SoftMask.BorderMode.Sliced:
					return this.MapBorder(localPos, false);
				case SoftMask.BorderMode.Tiled:
					return this.MapBorder(localPos, true);
				default:
					UnityEngine.Debug.LogError("Unknown BorderMode");
					return this.MapSimple(localPos);
				}
			}

			private Vector2 MapSimple(Vector2 localPos)
			{
				return SoftMask.Mathr.Remap(localPos, this.maskRect, this.maskRectUV);
			}

			private Vector2 MapBorder(Vector2 localPos, bool repeat)
			{
				return new Vector2(this.Inset(localPos.x, this.maskRect.x, this.maskBorder.x, this.maskBorder.z, this.maskRect.z, this.maskRectUV.x, this.maskBorderUV.x, this.maskBorderUV.z, this.maskRectUV.z, (!repeat) ? 1f : this.tileRepeat.x), this.Inset(localPos.y, this.maskRect.y, this.maskBorder.y, this.maskBorder.w, this.maskRect.w, this.maskRectUV.y, this.maskBorderUV.y, this.maskBorderUV.w, this.maskRectUV.w, (!repeat) ? 1f : this.tileRepeat.y));
			}

			private float Inset(float v, float x1, float x2, float u1, float u2, float repeat = 1f)
			{
				float num = x2 - x1;
				return Mathf.Lerp(u1, u2, (num == 0f) ? 0f : this.Frac((v - x1) / num * repeat));
			}

			private float Inset(float v, float x1, float x2, float x3, float x4, float u1, float u2, float u3, float u4, float repeat = 1f)
			{
				if (v < x2)
				{
					return this.Inset(v, x1, x2, u1, u2, 1f);
				}
				if (v < x3)
				{
					return this.Inset(v, x2, x3, u2, u3, repeat);
				}
				return this.Inset(v, x3, x4, u3, u4, 1f);
			}

			private float Frac(float v)
			{
				return v - Mathf.Floor(v);
			}

			private float MaskValue(Color mask)
			{
				Color color = mask * this.maskChannelWeights;
				return color.a + color.r + color.g + color.b;
			}

			public Vector4 maskRect;

			public Vector4 maskBorder;

			public Vector4 maskRectUV;

			public Vector4 maskBorderUV;

			public Vector2 tileRepeat;

			public Color maskChannelWeights;

			public Matrix4x4 worldToMask;

			public Texture2D texture;

			public SoftMask.BorderMode borderMode;

			private static class Ids
			{
				public static readonly int SoftMask = Shader.PropertyToID("_SoftMask");

				public static readonly int SoftMask_Rect = Shader.PropertyToID("_SoftMask_Rect");

				public static readonly int SoftMask_UVRect = Shader.PropertyToID("_SoftMask_UVRect");

				public static readonly int SoftMask_ChannelWeights = Shader.PropertyToID("_SoftMask_ChannelWeights");

				public static readonly int SoftMask_WorldToMask = Shader.PropertyToID("_SoftMask_WorldToMask");

				public static readonly int SoftMask_BorderRect = Shader.PropertyToID("_SoftMask_BorderRect");

				public static readonly int SoftMask_UVBorderRect = Shader.PropertyToID("_SoftMask_UVBorderRect");

				public static readonly int SoftMask_TileRepeat = Shader.PropertyToID("_SoftMask_TileRepeat");
			}
		}

		private struct Diagnostics
		{
			public Diagnostics(SoftMask softMask)
			{
				this._softMask = softMask;
			}

			public SoftMask.Errors PollErrors()
			{
				SoftMask softMask = this._softMask;
				SoftMask.Errors errors = SoftMask.Errors.NoError;
				softMask.GetComponentsInChildren<SoftMaskable>(SoftMask.s_maskables);
				using (new ClearListAtExit<SoftMaskable>(SoftMask.s_maskables))
				{
					if (SoftMask.s_maskables.Any((SoftMaskable m) => object.ReferenceEquals(m.mask, softMask) && m.shaderIsNotSupported))
					{
						errors |= SoftMask.Errors.UnsupportedShaders;
					}
				}
				if (this.ThereAreNestedMasks())
				{
					errors |= SoftMask.Errors.NestedMasks;
				}
				errors |= SoftMask.Diagnostics.CheckSprite(this.sprite);
				errors |= this.CheckImage();
				errors |= this.CheckTexture();
				return errors;
			}

			public static SoftMask.Errors CheckSprite(Sprite sprite)
			{
				SoftMask.Errors errors = SoftMask.Errors.NoError;
				if (!sprite)
				{
					return errors;
				}
				if (sprite.packed && sprite.packingMode == SpritePackingMode.Tight)
				{
					errors |= SoftMask.Errors.TightPackedSprite;
				}
				if (sprite.associatedAlphaSplitTexture)
				{
					errors |= SoftMask.Errors.AlphaSplitSprite;
				}
				return errors;
			}

			private Image image
			{
				get
				{
					return this._softMask.DeduceSourceParameters().image;
				}
			}

			private Sprite sprite
			{
				get
				{
					return this._softMask.DeduceSourceParameters().sprite;
				}
			}

			private Texture2D texture
			{
				get
				{
					return this._softMask.DeduceSourceParameters().texture;
				}
			}

			private bool ThereAreNestedMasks()
			{
				SoftMask softMask = this._softMask;
				bool flag = false;
				using (new ClearListAtExit<SoftMask>(SoftMask.s_masks))
				{
					softMask.GetComponentsInParent<SoftMask>(false, SoftMask.s_masks);
					flag |= SoftMask.s_masks.Any((SoftMask x) => SoftMask.Diagnostics.AreCompeting(softMask, x));
					softMask.GetComponentsInChildren<SoftMask>(false, SoftMask.s_masks);
					flag |= SoftMask.s_masks.Any((SoftMask x) => SoftMask.Diagnostics.AreCompeting(softMask, x));
				}
				return flag;
			}

			private SoftMask.Errors CheckImage()
			{
				SoftMask.Errors errors = SoftMask.Errors.NoError;
				if (!this._softMask.isBasedOnGraphic)
				{
					return errors;
				}
				if (this.image && !SoftMask.Diagnostics.IsSupportedImageType(this.image.type))
				{
					errors |= SoftMask.Errors.UnsupportedImageType;
				}
				return errors;
			}

			private SoftMask.Errors CheckTexture()
			{
				SoftMask.Errors errors = SoftMask.Errors.NoError;
				if (this._softMask.isUsingRaycastFiltering && this.texture && !SoftMask.Diagnostics.IsReadable(this.texture))
				{
					errors |= SoftMask.Errors.UnreadableTexture;
				}
				return errors;
			}

			private static bool AreCompeting(SoftMask softMask, SoftMask other)
			{
				return softMask.isMaskingEnabled && softMask != other && other.isMaskingEnabled && softMask.canvas.rootCanvas == other.canvas.rootCanvas && !SoftMask.Diagnostics.SelectChild<SoftMask>(softMask, other).canvas.overrideSorting;
			}

			private static T SelectChild<T>(T first, T second) where T : Component
			{
				return (!first.transform.IsChildOf(second.transform)) ? second : first;
			}

			private static bool IsReadable(Texture2D texture)
			{
				bool result;
				try
				{
					texture.GetPixel(0, 0);
					result = true;
				}
				catch (UnityException)
				{
					result = false;
				}
				return result;
			}

			private static bool IsSupportedImageType(Image.Type type)
			{
				return type == Image.Type.Simple || type == Image.Type.Sliced || type == Image.Type.Tiled;
			}

			private SoftMask _softMask;
		}
	}
}
