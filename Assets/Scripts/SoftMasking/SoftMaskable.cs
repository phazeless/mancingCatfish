using System;
using System.Collections.Generic;
using SoftMasking.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoftMasking
{
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[AddComponentMenu("")]
	public class SoftMaskable : UIBehaviour, IMaterialModifier
	{
		public bool shaderIsNotSupported { get; private set; }

		public bool isMaskingEnabled
		{
			get
			{
				return this.mask != null && this.mask.isAlive && this.mask.isMaskingEnabled && this._affectedByMask;
			}
		}

		public ISoftMask mask
		{
			get
			{
				return this._mask;
			}
			private set
			{
				if (this._mask != value)
				{
					if (this._mask != null)
					{
						this.replacement = null;
					}
					this._mask = ((value == null || !value.isAlive) ? null : value);
					this.Invalidate();
				}
			}
		}

		public Material GetModifiedMaterial(Material baseMaterial)
		{
			if (this.isMaskingEnabled)
			{
				Material replacement = this.mask.GetReplacement(baseMaterial);
				this.replacement = replacement;
				if (this.replacement)
				{
					this.shaderIsNotSupported = false;
					return this.replacement;
				}
				if (!baseMaterial.HasDefaultUIShader())
				{
					this.SetShaderNotSupported(baseMaterial);
				}
			}
			else
			{
				this.shaderIsNotSupported = false;
				this.replacement = null;
			}
			return baseMaterial;
		}

		public void Invalidate()
		{
			if (this.graphic)
			{
				this.graphic.SetMaterialDirty();
			}
		}

		public void MaskMightChanged()
		{
			if (this.FindMaskOrDie())
			{
				this.Invalidate();
			}
		}

		protected override void Awake()
		{
			base.Awake();
			base.hideFlags = HideFlags.HideInInspector;
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if (this.FindMaskOrDie())
			{
				this.RequestChildTransformUpdate();
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			this.mask = null;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			this._destroyed = true;
		}

		protected override void OnTransformParentChanged()
		{
			base.OnTransformParentChanged();
			this.FindMaskOrDie();
		}

		protected override void OnCanvasHierarchyChanged()
		{
			base.OnCanvasHierarchyChanged();
			this.FindMaskOrDie();
		}

		private void OnTransformChildrenChanged()
		{
			this.RequestChildTransformUpdate();
		}

		private void RequestChildTransformUpdate()
		{
			if (this.mask != null)
			{
				this.mask.UpdateTransformChildren(base.transform);
			}
		}

		private Graphic graphic
		{
			get
			{
				return (!this._graphic) ? (this._graphic = base.GetComponent<Graphic>()) : this._graphic;
			}
		}

		private Material replacement
		{
			get
			{
				return this._replacement;
			}
			set
			{
				if (this._replacement != value)
				{
					if (this._replacement != null && this.mask != null)
					{
						this.mask.ReleaseReplacement(this._replacement);
					}
					this._replacement = value;
				}
			}
		}

		private bool FindMaskOrDie()
		{
			if (this._destroyed)
			{
				return false;
			}
			this.mask = (SoftMaskable.NearestMask(base.transform, out this._affectedByMask, true) ?? SoftMaskable.NearestMask(base.transform, out this._affectedByMask, false));
			if (this.mask == null)
			{
				this._destroyed = true;
				UnityEngine.Object.DestroyImmediate(this);
				return false;
			}
			return true;
		}

		private static ISoftMask NearestMask(Transform transform, out bool processedByThisMask, bool enabledOnly = true)
		{
			processedByThisMask = true;
			Transform transform2 = transform;
			while (transform2)
			{
				if (transform2 != transform)
				{
					ISoftMask isoftMask = SoftMaskable.GetISoftMask(transform2, enabledOnly);
					if (isoftMask != null)
					{
						return isoftMask;
					}
				}
				if (SoftMaskable.IsOverridingSortingCanvas(transform2))
				{
					processedByThisMask = false;
				}
				transform2 = transform2.parent;
			}
			return null;
		}

		private static ISoftMask GetISoftMask(Transform current, bool shouldBeEnabled = true)
		{
			ISoftMask component = SoftMaskable.GetComponent<ISoftMask>(current, SoftMaskable.s_softMasks);
			if (component != null && component.isAlive && (!shouldBeEnabled || component.isMaskingEnabled))
			{
				return component;
			}
			return null;
		}

		private static bool IsOverridingSortingCanvas(Transform transform)
		{
			Canvas component = SoftMaskable.GetComponent<Canvas>(transform, SoftMaskable.s_canvases);
			return component && component.overrideSorting;
		}

		private static T GetComponent<T>(Component component, List<T> cachedList) where T : class
		{
			component.GetComponents<T>(cachedList);
			T result;
			using (new ClearListAtExit<T>(cachedList))
			{
				result = ((cachedList.Count <= 0) ? ((T)((object)null)) : cachedList[0]);
			}
			return result;
		}

		private void SetShaderNotSupported(Material material)
		{
			if (!this.shaderIsNotSupported)
			{
				UnityEngine.Debug.LogWarningFormat(base.gameObject, "SoftMask will not work on {0} because material {1} doesn't support masking. Add masking support to your material or set Graphic's material to None to use a default one.", new object[]
				{
					this.graphic,
					material
				});
				this.shaderIsNotSupported = true;
			}
		}

		private ISoftMask _mask;

		private Graphic _graphic;

		private Material _replacement;

		private bool _affectedByMask;

		private bool _destroyed;

		private static List<ISoftMask> s_softMasks = new List<ISoftMask>();

		private static List<Canvas> s_canvases = new List<Canvas>();
	}
}
