using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace TMPro
{
	public class TMP_Text : MaskableGraphic
	{
		public string text
		{
			get
			{
				return this.m_text;
			}
			set
			{
				if (this.m_text == value)
				{
					return;
				}
				this.m_text = value;
				this.m_inputSource = TMP_Text.TextInputSources.String;
				this.m_havePropertiesChanged = true;
				this.m_isCalculateSizeRequired = true;
				this.m_isInputParsingRequired = true;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		public bool isRightToLeftText
		{
			get
			{
				return this.m_isRightToLeft;
			}
			set
			{
				if (this.m_isRightToLeft == value)
				{
					return;
				}
				this.m_isRightToLeft = value;
				this.m_havePropertiesChanged = true;
				this.m_isCalculateSizeRequired = true;
				this.m_isInputParsingRequired = true;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		public TMP_FontAsset font
		{
			get
			{
				return this.m_fontAsset;
			}
			set
			{
				if (this.m_fontAsset == value)
				{
					return;
				}
				this.m_fontAsset = value;
				this.LoadFontAsset();
				this.m_havePropertiesChanged = true;
				this.m_isCalculateSizeRequired = true;
				this.m_isInputParsingRequired = true;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		public virtual Material fontSharedMaterial
		{
			get
			{
				return this.m_sharedMaterial;
			}
			set
			{
				if (this.m_sharedMaterial == value)
				{
					return;
				}
				this.SetSharedMaterial(value);
				this.m_havePropertiesChanged = true;
				this.m_isInputParsingRequired = true;
				this.SetVerticesDirty();
				this.SetMaterialDirty();
			}
		}

		public virtual Material[] fontSharedMaterials
		{
			get
			{
				return this.GetSharedMaterials();
			}
			set
			{
				this.SetSharedMaterials(value);
				this.m_havePropertiesChanged = true;
				this.m_isInputParsingRequired = true;
				this.SetVerticesDirty();
				this.SetMaterialDirty();
			}
		}

		public Material fontMaterial
		{
			get
			{
				return this.GetMaterial(this.m_sharedMaterial);
			}
			set
			{
				if (this.m_sharedMaterial != null && this.m_sharedMaterial.GetInstanceID() == value.GetInstanceID())
				{
					return;
				}
				this.m_sharedMaterial = value;
				this.m_padding = this.GetPaddingForMaterial();
				this.m_havePropertiesChanged = true;
				this.m_isInputParsingRequired = true;
				this.SetVerticesDirty();
				this.SetMaterialDirty();
			}
		}

		public virtual Material[] fontMaterials
		{
			get
			{
				return this.GetMaterials(this.m_fontSharedMaterials);
			}
			set
			{
				this.SetSharedMaterials(value);
				this.m_havePropertiesChanged = true;
				this.m_isInputParsingRequired = true;
				this.SetVerticesDirty();
				this.SetMaterialDirty();
			}
		}

		public override Color color
		{
			get
			{
				return this.m_fontColor;
			}
			set
			{
				if (this.m_fontColor == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_fontColor = value;
				this.SetVerticesDirty();
			}
		}

		public float alpha
		{
			get
			{
				return this.m_fontColor.a;
			}
			set
			{
				if (this.m_fontColor.a == value)
				{
					return;
				}
				this.m_fontColor.a = value;
				this.m_havePropertiesChanged = true;
				this.SetVerticesDirty();
			}
		}

		public bool enableVertexGradient
		{
			get
			{
				return this.m_enableVertexGradient;
			}
			set
			{
				if (this.m_enableVertexGradient == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_enableVertexGradient = value;
				this.SetVerticesDirty();
			}
		}

		public VertexGradient colorGradient
		{
			get
			{
				return this.m_fontColorGradient;
			}
			set
			{
				this.m_havePropertiesChanged = true;
				this.m_fontColorGradient = value;
				this.SetVerticesDirty();
			}
		}

		public TMP_ColorGradient colorGradientPreset
		{
			get
			{
				return this.m_fontColorGradientPreset;
			}
			set
			{
				this.m_havePropertiesChanged = true;
				this.m_fontColorGradientPreset = value;
				this.SetVerticesDirty();
			}
		}

		public TMP_SpriteAsset spriteAsset
		{
			get
			{
				return this.m_spriteAsset;
			}
			set
			{
				this.m_spriteAsset = value;
			}
		}

		public bool tintAllSprites
		{
			get
			{
				return this.m_tintAllSprites;
			}
			set
			{
				if (this.m_tintAllSprites == value)
				{
					return;
				}
				this.m_tintAllSprites = value;
				this.m_havePropertiesChanged = true;
				this.SetVerticesDirty();
			}
		}

		public bool overrideColorTags
		{
			get
			{
				return this.m_overrideHtmlColors;
			}
			set
			{
				if (this.m_overrideHtmlColors == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_overrideHtmlColors = value;
				this.SetVerticesDirty();
			}
		}

		public Color32 faceColor
		{
			get
			{
				if (this.m_sharedMaterial == null)
				{
					return this.m_faceColor;
				}
				this.m_faceColor = this.m_sharedMaterial.GetColor(ShaderUtilities.ID_FaceColor);
				return this.m_faceColor;
			}
			set
			{
				if (this.m_faceColor.Compare(value))
				{
					return;
				}
				this.SetFaceColor(value);
				this.m_havePropertiesChanged = true;
				this.m_faceColor = value;
				this.SetVerticesDirty();
				this.SetMaterialDirty();
			}
		}

		public Color32 outlineColor
		{
			get
			{
				if (this.m_sharedMaterial == null)
				{
					return this.m_outlineColor;
				}
				this.m_outlineColor = this.m_sharedMaterial.GetColor(ShaderUtilities.ID_OutlineColor);
				return this.m_outlineColor;
			}
			set
			{
				if (this.m_outlineColor.Compare(value))
				{
					return;
				}
				this.SetOutlineColor(value);
				this.m_havePropertiesChanged = true;
				this.m_outlineColor = value;
				this.SetVerticesDirty();
			}
		}

		public float outlineWidth
		{
			get
			{
				if (this.m_sharedMaterial == null)
				{
					return this.m_outlineWidth;
				}
				this.m_outlineWidth = this.m_sharedMaterial.GetFloat(ShaderUtilities.ID_OutlineWidth);
				return this.m_outlineWidth;
			}
			set
			{
				if (this.m_outlineWidth == value)
				{
					return;
				}
				this.SetOutlineThickness(value);
				this.m_havePropertiesChanged = true;
				this.m_outlineWidth = value;
				this.SetVerticesDirty();
			}
		}

		public float fontSize
		{
			get
			{
				return this.m_fontSize;
			}
			set
			{
				if (this.m_fontSize == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_isCalculateSizeRequired = true;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
				this.m_fontSize = value;
				if (!this.m_enableAutoSizing)
				{
					this.m_fontSizeBase = this.m_fontSize;
				}
			}
		}

		public float fontScale
		{
			get
			{
				return this.m_fontScale;
			}
		}

		public int fontWeight
		{
			get
			{
				return this.m_fontWeight;
			}
			set
			{
				if (this.m_fontWeight == value)
				{
					return;
				}
				this.m_fontWeight = value;
				this.m_isCalculateSizeRequired = true;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		public float pixelsPerUnit
		{
			get
			{
				Canvas canvas = base.canvas;
				if (!canvas)
				{
					return 1f;
				}
				if (!this.font)
				{
					return canvas.scaleFactor;
				}
				if (this.m_currentFontAsset == null || this.m_currentFontAsset.fontInfo.PointSize <= 0f || this.m_fontSize <= 0f)
				{
					return 1f;
				}
				return this.m_fontSize / this.m_currentFontAsset.fontInfo.PointSize;
			}
		}

		public bool enableAutoSizing
		{
			get
			{
				return this.m_enableAutoSizing;
			}
			set
			{
				if (this.m_enableAutoSizing == value)
				{
					return;
				}
				this.m_enableAutoSizing = value;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		public float fontSizeMin
		{
			get
			{
				return this.m_fontSizeMin;
			}
			set
			{
				if (this.m_fontSizeMin == value)
				{
					return;
				}
				this.m_fontSizeMin = value;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		public float fontSizeMax
		{
			get
			{
				return this.m_fontSizeMax;
			}
			set
			{
				if (this.m_fontSizeMax == value)
				{
					return;
				}
				this.m_fontSizeMax = value;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		public FontStyles fontStyle
		{
			get
			{
				return this.m_fontStyle;
			}
			set
			{
				if (this.m_fontStyle == value)
				{
					return;
				}
				this.m_fontStyle = value;
				this.m_havePropertiesChanged = true;
				this.checkPaddingRequired = true;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		public bool isUsingBold
		{
			get
			{
				return this.m_isUsingBold;
			}
		}

		public TextAlignmentOptions alignment
		{
			get
			{
				return this.m_textAlignment;
			}
			set
			{
				if (this.m_textAlignment == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_textAlignment = value;
				this.SetVerticesDirty();
			}
		}

		public float characterSpacing
		{
			get
			{
				return this.m_characterSpacing;
			}
			set
			{
				if (this.m_characterSpacing == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_isCalculateSizeRequired = true;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
				this.m_characterSpacing = value;
			}
		}

		public float lineSpacing
		{
			get
			{
				return this.m_lineSpacing;
			}
			set
			{
				if (this.m_lineSpacing == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_isCalculateSizeRequired = true;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
				this.m_lineSpacing = value;
			}
		}

		public float paragraphSpacing
		{
			get
			{
				return this.m_paragraphSpacing;
			}
			set
			{
				if (this.m_paragraphSpacing == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_isCalculateSizeRequired = true;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
				this.m_paragraphSpacing = value;
			}
		}

		public float characterWidthAdjustment
		{
			get
			{
				return this.m_charWidthMaxAdj;
			}
			set
			{
				if (this.m_charWidthMaxAdj == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_isCalculateSizeRequired = true;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
				this.m_charWidthMaxAdj = value;
			}
		}

		public bool enableWordWrapping
		{
			get
			{
				return this.m_enableWordWrapping;
			}
			set
			{
				if (this.m_enableWordWrapping == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_isInputParsingRequired = true;
				this.m_isCalculateSizeRequired = true;
				this.m_enableWordWrapping = value;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		public float wordWrappingRatios
		{
			get
			{
				return this.m_wordWrappingRatios;
			}
			set
			{
				if (this.m_wordWrappingRatios == value)
				{
					return;
				}
				this.m_wordWrappingRatios = value;
				this.m_havePropertiesChanged = true;
				this.m_isCalculateSizeRequired = true;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		public bool enableAdaptiveJustification
		{
			get
			{
				return this.m_enableAdaptiveJustification;
			}
			set
			{
				if (this.m_enableAdaptiveJustification == value)
				{
					return;
				}
				this.m_enableAdaptiveJustification = value;
				this.m_havePropertiesChanged = true;
				this.m_isCalculateSizeRequired = true;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		public TextOverflowModes OverflowMode
		{
			get
			{
				return this.m_overflowMode;
			}
			set
			{
				if (this.m_overflowMode == value)
				{
					return;
				}
				this.m_overflowMode = value;
				this.m_havePropertiesChanged = true;
				this.m_isCalculateSizeRequired = true;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		public bool enableKerning
		{
			get
			{
				return this.m_enableKerning;
			}
			set
			{
				if (this.m_enableKerning == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_isCalculateSizeRequired = true;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
				this.m_enableKerning = value;
			}
		}

		public bool extraPadding
		{
			get
			{
				return this.m_enableExtraPadding;
			}
			set
			{
				if (this.m_enableExtraPadding == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_enableExtraPadding = value;
				this.UpdateMeshPadding();
				this.SetVerticesDirty();
			}
		}

		public bool richText
		{
			get
			{
				return this.m_isRichText;
			}
			set
			{
				if (this.m_isRichText == value)
				{
					return;
				}
				this.m_isRichText = value;
				this.m_havePropertiesChanged = true;
				this.m_isCalculateSizeRequired = true;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
				this.m_isInputParsingRequired = true;
			}
		}

		public bool parseCtrlCharacters
		{
			get
			{
				return this.m_parseCtrlCharacters;
			}
			set
			{
				if (this.m_parseCtrlCharacters == value)
				{
					return;
				}
				this.m_parseCtrlCharacters = value;
				this.m_havePropertiesChanged = true;
				this.m_isCalculateSizeRequired = true;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
				this.m_isInputParsingRequired = true;
			}
		}

		public bool isOverlay
		{
			get
			{
				return this.m_isOverlay;
			}
			set
			{
				if (this.m_isOverlay == value)
				{
					return;
				}
				this.m_isOverlay = value;
				this.SetShaderDepth();
				this.m_havePropertiesChanged = true;
				this.SetVerticesDirty();
			}
		}

		public bool isOrthographic
		{
			get
			{
				return this.m_isOrthographic;
			}
			set
			{
				if (this.m_isOrthographic == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_isOrthographic = value;
				this.SetVerticesDirty();
			}
		}

		public bool enableCulling
		{
			get
			{
				return this.m_isCullingEnabled;
			}
			set
			{
				if (this.m_isCullingEnabled == value)
				{
					return;
				}
				this.m_isCullingEnabled = value;
				this.SetCulling();
				this.m_havePropertiesChanged = true;
			}
		}

		public bool ignoreVisibility
		{
			get
			{
				return this.m_ignoreCulling;
			}
			set
			{
				if (this.m_ignoreCulling == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_ignoreCulling = value;
			}
		}

		public TextureMappingOptions horizontalMapping
		{
			get
			{
				return this.m_horizontalMapping;
			}
			set
			{
				if (this.m_horizontalMapping == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_horizontalMapping = value;
				this.SetVerticesDirty();
			}
		}

		public TextureMappingOptions verticalMapping
		{
			get
			{
				return this.m_verticalMapping;
			}
			set
			{
				if (this.m_verticalMapping == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_verticalMapping = value;
				this.SetVerticesDirty();
			}
		}

		public TextRenderFlags renderMode
		{
			get
			{
				return this.m_renderMode;
			}
			set
			{
				if (this.m_renderMode == value)
				{
					return;
				}
				this.m_renderMode = value;
				this.m_havePropertiesChanged = true;
			}
		}

		public int maxVisibleCharacters
		{
			get
			{
				return this.m_maxVisibleCharacters;
			}
			set
			{
				if (this.m_maxVisibleCharacters == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_maxVisibleCharacters = value;
				this.SetVerticesDirty();
			}
		}

		public int maxVisibleWords
		{
			get
			{
				return this.m_maxVisibleWords;
			}
			set
			{
				if (this.m_maxVisibleWords == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_maxVisibleWords = value;
				this.SetVerticesDirty();
			}
		}

		public int maxVisibleLines
		{
			get
			{
				return this.m_maxVisibleLines;
			}
			set
			{
				if (this.m_maxVisibleLines == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_isInputParsingRequired = true;
				this.m_maxVisibleLines = value;
				this.SetVerticesDirty();
			}
		}

		public bool useMaxVisibleDescender
		{
			get
			{
				return this.m_useMaxVisibleDescender;
			}
			set
			{
				if (this.m_useMaxVisibleDescender == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_isInputParsingRequired = true;
				this.SetVerticesDirty();
			}
		}

		public int pageToDisplay
		{
			get
			{
				return this.m_pageToDisplay;
			}
			set
			{
				if (this.m_pageToDisplay == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_pageToDisplay = value;
				this.SetVerticesDirty();
			}
		}

		public virtual Vector4 margin
		{
			get
			{
				return this.m_margin;
			}
			set
			{
				if (this.m_margin == value)
				{
					return;
				}
				this.m_margin = value;
				this.ComputeMarginSize();
				this.m_havePropertiesChanged = true;
				this.SetVerticesDirty();
			}
		}

		public TMP_TextInfo textInfo
		{
			get
			{
				return this.m_textInfo;
			}
		}

		public bool havePropertiesChanged
		{
			get
			{
				return this.m_havePropertiesChanged;
			}
			set
			{
				if (this.m_havePropertiesChanged == value)
				{
					return;
				}
				this.m_havePropertiesChanged = value;
				this.m_isInputParsingRequired = true;
				this.SetAllDirty();
			}
		}

		public bool isUsingLegacyAnimationComponent
		{
			get
			{
				return this.m_isUsingLegacyAnimationComponent;
			}
			set
			{
				this.m_isUsingLegacyAnimationComponent = value;
			}
		}

		public new Transform transform
		{
			get
			{
				if (this.m_transform == null)
				{
					this.m_transform = base.GetComponent<Transform>();
				}
				return this.m_transform;
			}
		}

		public new RectTransform rectTransform
		{
			get
			{
				if (this.m_rectTransform == null)
				{
					this.m_rectTransform = base.GetComponent<RectTransform>();
				}
				return this.m_rectTransform;
			}
		}

		public virtual bool autoSizeTextContainer { get; set; }

		public virtual Mesh mesh
		{
			get
			{
				return this.m_mesh;
			}
		}

		public bool isVolumetricText
		{
			get
			{
				return this.m_isVolumetricText;
			}
			set
			{
				if (this.m_isVolumetricText == value)
				{
					return;
				}
				this.m_havePropertiesChanged = value;
				this.m_textInfo.ResetVertexLayout(value);
				this.m_isInputParsingRequired = true;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		public Bounds bounds
		{
			get
			{
				if (this.m_mesh == null)
				{
					return default(Bounds);
				}
				return this.GetCompoundBounds();
			}
		}

		public Bounds textBounds
		{
			get
			{
				if (this.m_textInfo == null)
				{
					return default(Bounds);
				}
				return this.GetTextBounds();
			}
		}

		public float flexibleHeight
		{
			get
			{
				return this.m_flexibleHeight;
			}
		}

		public float flexibleWidth
		{
			get
			{
				return this.m_flexibleWidth;
			}
		}

		public float minHeight
		{
			get
			{
				return this.m_minHeight;
			}
		}

		public float minWidth
		{
			get
			{
				return this.m_minWidth;
			}
		}

		public virtual float preferredWidth
		{
			get
			{
				if (!this.m_isPreferredWidthDirty)
				{
					return this.m_preferredWidth;
				}
				this.m_preferredWidth = this.GetPreferredWidth();
				return this.m_preferredWidth;
			}
		}

		public virtual float preferredHeight
		{
			get
			{
				if (!this.m_isPreferredHeightDirty)
				{
					return this.m_preferredHeight;
				}
				this.m_preferredHeight = this.GetPreferredHeight();
				return this.m_preferredHeight;
			}
		}

		public virtual float renderedWidth
		{
			get
			{
				return this.GetRenderedWidth();
			}
		}

		public virtual float renderedHeight
		{
			get
			{
				return this.GetRenderedHeight();
			}
		}

		public int layoutPriority
		{
			get
			{
				return this.m_layoutPriority;
			}
		}

		protected virtual void LoadFontAsset()
		{
		}

		protected virtual void SetSharedMaterial(Material mat)
		{
		}

		protected virtual Material GetMaterial(Material mat)
		{
			return null;
		}

		protected virtual void SetFontBaseMaterial(Material mat)
		{
		}

		protected virtual Material[] GetSharedMaterials()
		{
			return null;
		}

		protected virtual void SetSharedMaterials(Material[] materials)
		{
		}

		protected virtual Material[] GetMaterials(Material[] mats)
		{
			return null;
		}

		protected virtual Material CreateMaterialInstance(Material source)
		{
			Material material = new Material(source);
			material.shaderKeywords = source.shaderKeywords;
			Material material2 = material;
			material2.name += " (Instance)";
			return material;
		}

		protected void SetVertexColorGradient(TMP_ColorGradient gradient)
		{
			if (gradient == null)
			{
				return;
			}
			this.m_fontColorGradient.bottomLeft = gradient.bottomLeft;
			this.m_fontColorGradient.bottomRight = gradient.bottomRight;
			this.m_fontColorGradient.topLeft = gradient.topLeft;
			this.m_fontColorGradient.topRight = gradient.topRight;
			this.SetVerticesDirty();
		}

		protected virtual void SetFaceColor(Color32 color)
		{
		}

		protected virtual void SetOutlineColor(Color32 color)
		{
		}

		protected virtual void SetOutlineThickness(float thickness)
		{
		}

		protected virtual void SetShaderDepth()
		{
		}

		protected virtual void SetCulling()
		{
		}

		protected virtual float GetPaddingForMaterial()
		{
			return 0f;
		}

		protected virtual float GetPaddingForMaterial(Material mat)
		{
			return 0f;
		}

		protected virtual Vector3[] GetTextContainerLocalCorners()
		{
			return null;
		}

		public virtual void ForceMeshUpdate()
		{
		}

		public virtual void ForceMeshUpdate(bool ignoreActiveState)
		{
		}

		internal void SetTextInternal(string text)
		{
			this.m_text = text;
			this.m_renderMode = TextRenderFlags.DontRender;
			this.m_isInputParsingRequired = true;
			this.ForceMeshUpdate();
			this.m_renderMode = TextRenderFlags.Render;
		}

		public virtual void UpdateGeometry(Mesh mesh, int index)
		{
		}

		public virtual void UpdateVertexData(TMP_VertexDataUpdateFlags flags)
		{
		}

		public virtual void UpdateVertexData()
		{
		}

		public virtual void SetVertices(Vector3[] vertices)
		{
		}

		public virtual void UpdateMeshPadding()
		{
		}

		public new void CrossFadeColor(Color targetColor, float duration, bool ignoreTimeScale, bool useAlpha)
		{
			base.CrossFadeColor(targetColor, duration, ignoreTimeScale, useAlpha);
			this.InternalCrossFadeColor(targetColor, duration, ignoreTimeScale, useAlpha);
		}

		public new void CrossFadeAlpha(float alpha, float duration, bool ignoreTimeScale)
		{
			base.CrossFadeAlpha(alpha, duration, ignoreTimeScale);
			this.InternalCrossFadeAlpha(alpha, duration, ignoreTimeScale);
		}

		protected virtual void InternalCrossFadeColor(Color targetColor, float duration, bool ignoreTimeScale, bool useAlpha)
		{
		}

		protected virtual void InternalCrossFadeAlpha(float alpha, float duration, bool ignoreTimeScale)
		{
		}

		protected void ParseInputText()
		{
			this.m_isInputParsingRequired = false;
			switch (this.m_inputSource)
			{
			case TMP_Text.TextInputSources.Text:
			case TMP_Text.TextInputSources.String:
				this.StringToCharArray(this.m_text, ref this.m_char_buffer);
				break;
			case TMP_Text.TextInputSources.SetText:
				this.SetTextArrayToCharArray(this.m_input_CharArray, ref this.m_char_buffer);
				break;
			}
			this.SetArraySizes(this.m_char_buffer);
		}

		public void SetText(string text)
		{
			this.m_inputSource = TMP_Text.TextInputSources.SetCharArray;
			this.StringToCharArray(text, ref this.m_char_buffer);
			this.m_isInputParsingRequired = true;
			this.m_havePropertiesChanged = true;
			this.m_isCalculateSizeRequired = true;
			this.SetVerticesDirty();
			this.SetLayoutDirty();
		}

		public void SetText(string text, float arg0)
		{
			this.SetText(text, arg0, 255f, 255f);
		}

		public void SetText(string text, float arg0, float arg1)
		{
			this.SetText(text, arg0, arg1, 255f);
		}

		public void SetText(string text, float arg0, float arg1, float arg2)
		{
			if (text == this.old_text && arg0 == this.old_arg0 && arg1 == this.old_arg1 && arg2 == this.old_arg2)
			{
				return;
			}
			this.old_text = text;
			this.old_arg1 = 255f;
			this.old_arg2 = 255f;
			int precision = 0;
			int num = 0;
			for (int i = 0; i < text.Length; i++)
			{
				char c = text[i];
				if (c == '{')
				{
					if (text[i + 2] == ':')
					{
						precision = (int)(text[i + 3] - '0');
					}
					int num2 = (int)(text[i + 1] - '0');
					if (num2 != 0)
					{
						if (num2 != 1)
						{
							if (num2 == 2)
							{
								this.old_arg2 = arg2;
								this.AddFloatToCharArray(arg2, ref num, precision);
							}
						}
						else
						{
							this.old_arg1 = arg1;
							this.AddFloatToCharArray(arg1, ref num, precision);
						}
					}
					else
					{
						this.old_arg0 = arg0;
						this.AddFloatToCharArray(arg0, ref num, precision);
					}
					if (text[i + 2] == ':')
					{
						i += 4;
					}
					else
					{
						i += 2;
					}
				}
				else
				{
					this.m_input_CharArray[num] = c;
					num++;
				}
			}
			this.m_input_CharArray[num] = '\0';
			this.m_charArray_Length = num;
			this.m_inputSource = TMP_Text.TextInputSources.SetText;
			this.m_isInputParsingRequired = true;
			this.m_havePropertiesChanged = true;
			this.m_isCalculateSizeRequired = true;
			this.SetVerticesDirty();
			this.SetLayoutDirty();
		}

		public void SetText(StringBuilder text)
		{
			this.m_inputSource = TMP_Text.TextInputSources.SetCharArray;
			this.StringBuilderToIntArray(text, ref this.m_char_buffer);
			this.m_isInputParsingRequired = true;
			this.m_havePropertiesChanged = true;
			this.m_isCalculateSizeRequired = true;
			this.SetVerticesDirty();
			this.SetLayoutDirty();
		}

		public void SetCharArray(char[] charArray)
		{
			if (charArray == null || charArray.Length == 0)
			{
				return;
			}
			if (this.m_char_buffer.Length <= charArray.Length)
			{
				int num = Mathf.NextPowerOfTwo(charArray.Length + 1);
				this.m_char_buffer = new int[num];
			}
			int num2 = 0;
			int i = 0;
			while (i < charArray.Length)
			{
				if (charArray[i] != '\\' || i >= charArray.Length - 1)
				{
					goto IL_BC;
				}
				int num3 = (int)charArray[i + 1];
				if (num3 != 110)
				{
					if (num3 != 114)
					{
						if (num3 != 116)
						{
							goto IL_BC;
						}
						this.m_char_buffer[num2] = 9;
						i++;
						num2++;
					}
					else
					{
						this.m_char_buffer[num2] = 13;
						i++;
						num2++;
					}
				}
				else
				{
					this.m_char_buffer[num2] = 10;
					i++;
					num2++;
				}
				IL_CB:
				i++;
				continue;
				IL_BC:
				this.m_char_buffer[num2] = (int)charArray[i];
				num2++;
				goto IL_CB;
			}
			this.m_char_buffer[num2] = 0;
			this.m_inputSource = TMP_Text.TextInputSources.SetCharArray;
			this.m_havePropertiesChanged = true;
			this.m_isInputParsingRequired = true;
		}

		protected void SetTextArrayToCharArray(char[] charArray, ref int[] charBuffer)
		{
			if (charArray == null || this.m_charArray_Length == 0)
			{
				return;
			}
			if (charBuffer.Length <= this.m_charArray_Length)
			{
				int num = (this.m_charArray_Length <= 1024) ? Mathf.NextPowerOfTwo(this.m_charArray_Length + 1) : (this.m_charArray_Length + 256);
				charBuffer = new int[num];
			}
			int num2 = 0;
			for (int i = 0; i < this.m_charArray_Length; i++)
			{
				if (char.IsHighSurrogate(charArray[i]) && char.IsLowSurrogate(charArray[i + 1]))
				{
					charBuffer[num2] = char.ConvertToUtf32(charArray[i], charArray[i + 1]);
					i++;
					num2++;
				}
				else
				{
					charBuffer[num2] = (int)charArray[i];
					num2++;
				}
			}
			charBuffer[num2] = 0;
		}

		protected void StringToCharArray(string text, ref int[] chars)
		{
			if (text == null)
			{
				chars[0] = 0;
				return;
			}
			if (chars == null || chars.Length <= text.Length)
			{
				int num = (text.Length <= 1024) ? Mathf.NextPowerOfTwo(text.Length + 1) : (text.Length + 256);
				chars = new int[num];
			}
			int num2 = 0;
			int i = 0;
			while (i < text.Length)
			{
				if (this.m_inputSource != TMP_Text.TextInputSources.Text || text[i] != '\\' || text.Length <= i + 1)
				{
					goto IL_1DB;
				}
				int num3 = (int)text[i + 1];
				switch (num3)
				{
				case 114:
					if (!this.m_parseCtrlCharacters)
					{
						goto IL_1DB;
					}
					chars[num2] = 13;
					i++;
					num2++;
					break;
				default:
					if (num3 != 85)
					{
						if (num3 != 92)
						{
							if (num3 != 110)
							{
								goto IL_1DB;
							}
							if (!this.m_parseCtrlCharacters)
							{
								goto IL_1DB;
							}
							chars[num2] = 10;
							i++;
							num2++;
						}
						else
						{
							if (!this.m_parseCtrlCharacters)
							{
								goto IL_1DB;
							}
							if (text.Length <= i + 2)
							{
								goto IL_1DB;
							}
							chars[num2] = (int)text[i + 1];
							chars[num2 + 1] = (int)text[i + 2];
							i += 2;
							num2 += 2;
						}
					}
					else
					{
						if (text.Length <= i + 9)
						{
							goto IL_1DB;
						}
						chars[num2] = this.GetUTF32(i + 2);
						i += 9;
						num2++;
					}
					break;
				case 116:
					if (!this.m_parseCtrlCharacters)
					{
						goto IL_1DB;
					}
					chars[num2] = 9;
					i++;
					num2++;
					break;
				case 117:
					if (text.Length <= i + 5)
					{
						goto IL_1DB;
					}
					chars[num2] = (int)((ushort)this.GetUTF16(i + 2));
					i += 5;
					num2++;
					break;
				}
				IL_234:
				i++;
				continue;
				IL_1DB:
				if (char.IsHighSurrogate(text[i]) && char.IsLowSurrogate(text[i + 1]))
				{
					chars[num2] = char.ConvertToUtf32(text[i], text[i + 1]);
					i++;
					num2++;
					goto IL_234;
				}
				chars[num2] = (int)text[i];
				num2++;
				goto IL_234;
			}
			chars[num2] = 0;
		}

		protected void StringBuilderToIntArray(StringBuilder text, ref int[] chars)
		{
			if (text == null)
			{
				chars[0] = 0;
				return;
			}
			if (chars == null || chars.Length <= text.Length)
			{
				int num = (text.Length <= 1024) ? Mathf.NextPowerOfTwo(text.Length + 1) : (text.Length + 256);
				chars = new int[num];
			}
			int num2 = 0;
			int i = 0;
			while (i < text.Length)
			{
				if (!this.m_parseCtrlCharacters || text[i] != '\\' || text.Length <= i + 1)
				{
					goto IL_19B;
				}
				int num3 = (int)text[i + 1];
				switch (num3)
				{
				case 114:
					chars[num2] = 13;
					i++;
					num2++;
					break;
				default:
					if (num3 != 85)
					{
						if (num3 != 92)
						{
							if (num3 != 110)
							{
								goto IL_19B;
							}
							chars[num2] = 10;
							i++;
							num2++;
						}
						else
						{
							if (text.Length <= i + 2)
							{
								goto IL_19B;
							}
							chars[num2] = (int)text[i + 1];
							chars[num2 + 1] = (int)text[i + 2];
							i += 2;
							num2 += 2;
						}
					}
					else
					{
						if (text.Length <= i + 9)
						{
							goto IL_19B;
						}
						chars[num2] = this.GetUTF32(i + 2);
						i += 9;
						num2++;
					}
					break;
				case 116:
					chars[num2] = 9;
					i++;
					num2++;
					break;
				case 117:
					if (text.Length <= i + 5)
					{
						goto IL_19B;
					}
					chars[num2] = (int)((ushort)this.GetUTF16(i + 2));
					i += 5;
					num2++;
					break;
				}
				IL_1F4:
				i++;
				continue;
				IL_19B:
				if (char.IsHighSurrogate(text[i]) && char.IsLowSurrogate(text[i + 1]))
				{
					chars[num2] = char.ConvertToUtf32(text[i], text[i + 1]);
					i++;
					num2++;
					goto IL_1F4;
				}
				chars[num2] = (int)text[i];
				num2++;
				goto IL_1F4;
			}
			chars[num2] = 0;
		}

		protected void AddFloatToCharArray(float number, ref int index, int precision)
		{
			if (number < 0f)
			{
				this.m_input_CharArray[index++] = '-';
				number = -number;
			}
			number += this.k_Power[Mathf.Min(9, precision)];
			int num = (int)number;
			this.AddIntToCharArray(num, ref index, precision);
			if (precision > 0)
			{
				this.m_input_CharArray[index++] = '.';
				number -= (float)num;
				for (int i = 0; i < precision; i++)
				{
					number *= 10f;
					int num2 = (int)number;
					this.m_input_CharArray[index++] = (char)(num2 + 48);
					number -= (float)num2;
				}
			}
		}

		protected void AddIntToCharArray(int number, ref int index, int precision)
		{
			if (number < 0)
			{
				this.m_input_CharArray[index++] = '-';
				number = -number;
			}
			int num = index;
			do
			{
				this.m_input_CharArray[num++] = (char)(number % 10 + 48);
				number /= 10;
			}
			while (number > 0);
			int num2 = num;
			while (index + 1 < num)
			{
				num--;
				char c = this.m_input_CharArray[index];
				this.m_input_CharArray[index] = this.m_input_CharArray[num];
				this.m_input_CharArray[num] = c;
				index++;
			}
			index = num2;
		}

		protected virtual int SetArraySizes(int[] chars)
		{
			return 0;
		}

		protected virtual void GenerateTextMesh()
		{
		}

		public Vector2 GetPreferredValues()
		{
			if (this.m_isInputParsingRequired || this.m_isTextTruncated)
			{
				this.m_isCalculatingPreferredValues = true;
				this.ParseInputText();
			}
			float preferredWidth = this.GetPreferredWidth();
			float preferredHeight = this.GetPreferredHeight();
			return new Vector2(preferredWidth, preferredHeight);
		}

		public Vector2 GetPreferredValues(float width, float height)
		{
			if (this.m_isInputParsingRequired || this.m_isTextTruncated)
			{
				this.m_isCalculatingPreferredValues = true;
				this.ParseInputText();
			}
			Vector2 margin = new Vector2(width, height);
			float preferredWidth = this.GetPreferredWidth(margin);
			float preferredHeight = this.GetPreferredHeight(margin);
			return new Vector2(preferredWidth, preferredHeight);
		}

		public Vector2 GetPreferredValues(string text)
		{
			this.m_isCalculatingPreferredValues = true;
			this.StringToCharArray(text, ref this.m_char_buffer);
			this.SetArraySizes(this.m_char_buffer);
			Vector2 margin = TMP_Text.k_LargePositiveVector2;
			float preferredWidth = this.GetPreferredWidth(margin);
			float preferredHeight = this.GetPreferredHeight(margin);
			return new Vector2(preferredWidth, preferredHeight);
		}

		public Vector2 GetPreferredValues(string text, float width, float height)
		{
			this.m_isCalculatingPreferredValues = true;
			this.StringToCharArray(text, ref this.m_char_buffer);
			this.SetArraySizes(this.m_char_buffer);
			Vector2 margin = new Vector2(width, height);
			float preferredWidth = this.GetPreferredWidth(margin);
			float preferredHeight = this.GetPreferredHeight(margin);
			return new Vector2(preferredWidth, preferredHeight);
		}

		protected float GetPreferredWidth()
		{
			float defaultFontSize = (!this.m_enableAutoSizing) ? this.m_fontSize : this.m_fontSizeMax;
			Vector2 marginSize = TMP_Text.k_LargePositiveVector2;
			if (this.m_isInputParsingRequired || this.m_isTextTruncated)
			{
				this.m_isCalculatingPreferredValues = true;
				this.ParseInputText();
			}
			float x = this.CalculatePreferredValues(defaultFontSize, marginSize).x;
			this.m_isPreferredWidthDirty = false;
			return x;
		}

		protected float GetPreferredWidth(Vector2 margin)
		{
			float defaultFontSize = (!this.m_enableAutoSizing) ? this.m_fontSize : this.m_fontSizeMax;
			return this.CalculatePreferredValues(defaultFontSize, margin).x;
		}

		protected float GetPreferredHeight()
		{
			float defaultFontSize = (!this.m_enableAutoSizing) ? this.m_fontSize : this.m_fontSizeMax;
			Vector2 marginSize = new Vector2((this.m_marginWidth == 0f) ? TMP_Text.k_LargePositiveFloat : this.m_marginWidth, TMP_Text.k_LargePositiveFloat);
			if (this.m_isInputParsingRequired || this.m_isTextTruncated)
			{
				this.m_isCalculatingPreferredValues = true;
				this.ParseInputText();
			}
			float y = this.CalculatePreferredValues(defaultFontSize, marginSize).y;
			this.m_isPreferredHeightDirty = false;
			return y;
		}

		protected float GetPreferredHeight(Vector2 margin)
		{
			float defaultFontSize = (!this.m_enableAutoSizing) ? this.m_fontSize : this.m_fontSizeMax;
			return this.CalculatePreferredValues(defaultFontSize, margin).y;
		}

		public Vector2 GetRenderedValues()
		{
			return this.GetTextBounds().size;
		}

		protected float GetRenderedWidth()
		{
			return this.GetRenderedValues().x;
		}

		protected float GetRenderedHeight()
		{
			return this.GetRenderedValues().y;
		}

		protected virtual Vector2 CalculatePreferredValues(float defaultFontSize, Vector2 marginSize)
		{
			if (this.m_fontAsset == null || this.m_fontAsset.characterDictionary == null)
			{
				UnityEngine.Debug.LogWarning("Can't Generate Mesh! No Font Asset has been assigned to Object ID: " + base.GetInstanceID());
				return Vector2.zero;
			}
			if (this.m_char_buffer == null || this.m_char_buffer.Length == 0 || this.m_char_buffer[0] == 0)
			{
				return Vector2.zero;
			}
			this.m_currentFontAsset = this.m_fontAsset;
			this.m_currentMaterial = this.m_sharedMaterial;
			this.m_currentMaterialIndex = 0;
			this.m_materialReferenceStack.SetDefault(new MaterialReference(0, this.m_currentFontAsset, null, this.m_currentMaterial, this.m_padding));
			int totalCharacterCount = this.m_totalCharacterCount;
			if (this.m_internalCharacterInfo == null || totalCharacterCount > this.m_internalCharacterInfo.Length)
			{
				this.m_internalCharacterInfo = new TMP_CharacterInfo[(totalCharacterCount <= 1024) ? Mathf.NextPowerOfTwo(totalCharacterCount) : (totalCharacterCount + 256)];
			}
			this.m_fontScale = defaultFontSize / this.m_currentFontAsset.fontInfo.PointSize * ((!this.m_isOrthographic) ? 0.1f : 1f);
			this.m_fontScaleMultiplier = 1f;
			float num = defaultFontSize / this.m_fontAsset.fontInfo.PointSize * this.m_fontAsset.fontInfo.Scale * ((!this.m_isOrthographic) ? 0.1f : 1f);
			float num2 = this.m_fontScale;
			this.m_currentFontSize = defaultFontSize;
			this.m_sizeStack.SetDefault(this.m_currentFontSize);
			this.m_style = this.m_fontStyle;
			this.m_baselineOffset = 0f;
			this.m_styleStack.Clear();
			this.m_lineOffset = 0f;
			this.m_lineHeight = 0f;
			float num3 = this.m_currentFontAsset.fontInfo.LineHeight - (this.m_currentFontAsset.fontInfo.Ascender - this.m_currentFontAsset.fontInfo.Descender);
			this.m_cSpacing = 0f;
			this.m_monoSpacing = 0f;
			this.m_xAdvance = 0f;
			float a = 0f;
			this.tag_LineIndent = 0f;
			this.tag_Indent = 0f;
			this.m_indentStack.SetDefault(0f);
			this.tag_NoParsing = false;
			this.m_characterCount = 0;
			this.m_firstCharacterOfLine = 0;
			this.m_maxLineAscender = TMP_Text.k_LargeNegativeFloat;
			this.m_maxLineDescender = TMP_Text.k_LargePositiveFloat;
			this.m_lineNumber = 0;
			float x = marginSize.x;
			this.m_marginLeft = 0f;
			this.m_marginRight = 0f;
			this.m_width = -1f;
			float num4 = 0f;
			float num5 = 0f;
			float num6 = 0f;
			this.m_maxAscender = 0f;
			this.m_maxDescender = 0f;
			bool flag = true;
			bool flag2 = false;
			WordWrapState wordWrapState = default(WordWrapState);
			this.SaveWordWrappingState(ref wordWrapState, 0, 0);
			WordWrapState wordWrapState2 = default(WordWrapState);
			int num7 = 0;
			int num8 = 0;
			int num9 = 0;
			while (this.m_char_buffer[num9] != 0)
			{
				int num10 = this.m_char_buffer[num9];
				this.m_textElementType = TMP_TextElementType.Character;
				this.m_currentMaterialIndex = this.m_textInfo.characterInfo[this.m_characterCount].materialReferenceIndex;
				this.m_currentFontAsset = this.m_materialReferences[this.m_currentMaterialIndex].fontAsset;
				int currentMaterialIndex = this.m_currentMaterialIndex;
				if (!this.m_isRichText || num10 != 60)
				{
					goto IL_3AD;
				}
				this.m_isParsingText = true;
				if (!this.ValidateHtmlTag(this.m_char_buffer, num9 + 1, out num8))
				{
					goto IL_3AD;
				}
				num9 = num8;
				if (this.m_textElementType != TMP_TextElementType.Character)
				{
					goto IL_3AD;
				}
				IL_11C1:
				num9++;
				continue;
				IL_3AD:
				this.m_isParsingText = false;
				bool isUsingAlternateTypeface = this.m_textInfo.characterInfo[this.m_characterCount].isUsingAlternateTypeface;
				float num11 = 1f;
				if (this.m_textElementType == TMP_TextElementType.Character)
				{
					if ((this.m_style & FontStyles.UpperCase) == FontStyles.UpperCase)
					{
						if (char.IsLower((char)num10))
						{
							num10 = (int)char.ToUpper((char)num10);
						}
					}
					else if ((this.m_style & FontStyles.LowerCase) == FontStyles.LowerCase)
					{
						if (char.IsUpper((char)num10))
						{
							num10 = (int)char.ToLower((char)num10);
						}
					}
					else if (((this.m_fontStyle & FontStyles.SmallCaps) == FontStyles.SmallCaps || (this.m_style & FontStyles.SmallCaps) == FontStyles.SmallCaps) && char.IsLower((char)num10))
					{
						num11 = 0.8f;
						num10 = (int)char.ToUpper((char)num10);
					}
				}
				if (this.m_textElementType == TMP_TextElementType.Sprite)
				{
					TMP_Sprite tmp_Sprite = this.m_currentSpriteAsset.spriteInfoList[this.m_spriteIndex];
					if (tmp_Sprite == null)
					{
						goto IL_11C1;
					}
					num10 = 57344 + this.m_spriteIndex;
					this.m_currentFontAsset = this.m_fontAsset;
					float num12 = this.m_currentFontSize / this.m_fontAsset.fontInfo.PointSize * this.m_fontAsset.fontInfo.Scale * ((!this.m_isOrthographic) ? 0.1f : 1f);
					num2 = this.m_fontAsset.fontInfo.Ascender / tmp_Sprite.height * tmp_Sprite.scale * num12;
					this.m_cached_TextElement = tmp_Sprite;
					this.m_internalCharacterInfo[this.m_characterCount].elementType = TMP_TextElementType.Sprite;
					this.m_currentMaterialIndex = currentMaterialIndex;
				}
				else if (this.m_textElementType == TMP_TextElementType.Character)
				{
					this.m_cached_TextElement = this.m_textInfo.characterInfo[this.m_characterCount].textElement;
					if (this.m_cached_TextElement == null)
					{
						goto IL_11C1;
					}
					this.m_currentMaterialIndex = this.m_textInfo.characterInfo[this.m_characterCount].materialReferenceIndex;
					this.m_fontScale = this.m_currentFontSize * num11 / this.m_currentFontAsset.fontInfo.PointSize * this.m_currentFontAsset.fontInfo.Scale * ((!this.m_isOrthographic) ? 0.1f : 1f);
					num2 = this.m_fontScale * this.m_fontScaleMultiplier * this.m_cached_TextElement.scale;
					this.m_internalCharacterInfo[this.m_characterCount].elementType = TMP_TextElementType.Character;
				}
				float num13 = num2;
				if (num10 == 173)
				{
					num2 = 0f;
				}
				this.m_internalCharacterInfo[this.m_characterCount].character = (char)num10;
				if (this.m_enableKerning && this.m_characterCount >= 1)
				{
					int character = (int)this.m_internalCharacterInfo[this.m_characterCount - 1].character;
					KerningPairKey kerningPairKey = new KerningPairKey(character, num10);
					KerningPair kerningPair;
					this.m_currentFontAsset.kerningDictionary.TryGetValue(kerningPairKey.key, out kerningPair);
					if (kerningPair != null)
					{
						this.m_xAdvance += kerningPair.XadvanceOffset * num2;
					}
				}
				float num14 = 0f;
				if (this.m_monoSpacing != 0f)
				{
					num14 = this.m_monoSpacing / 2f - (this.m_cached_TextElement.width / 2f + this.m_cached_TextElement.xOffset) * num2;
					this.m_xAdvance += num14;
				}
				float num15;
				if (this.m_textElementType == TMP_TextElementType.Character && !isUsingAlternateTypeface && ((this.m_style & FontStyles.Bold) == FontStyles.Bold || (this.m_fontStyle & FontStyles.Bold) == FontStyles.Bold))
				{
					num15 = 1f + this.m_currentFontAsset.boldSpacing * 0.01f;
				}
				else
				{
					num15 = 1f;
				}
				this.m_internalCharacterInfo[this.m_characterCount].baseLine = 0f - this.m_lineOffset + this.m_baselineOffset;
				float num16 = this.m_currentFontAsset.fontInfo.Ascender * ((this.m_textElementType != TMP_TextElementType.Character) ? this.m_internalCharacterInfo[this.m_characterCount].scale : num2) + this.m_baselineOffset;
				this.m_internalCharacterInfo[this.m_characterCount].ascender = num16 - this.m_lineOffset;
				this.m_maxLineAscender = ((num16 <= this.m_maxLineAscender) ? this.m_maxLineAscender : num16);
				float num17 = this.m_currentFontAsset.fontInfo.Descender * ((this.m_textElementType != TMP_TextElementType.Character) ? this.m_internalCharacterInfo[this.m_characterCount].scale : num2) + this.m_baselineOffset;
				float num18 = this.m_internalCharacterInfo[this.m_characterCount].descender = num17 - this.m_lineOffset;
				this.m_maxLineDescender = ((num17 >= this.m_maxLineDescender) ? this.m_maxLineDescender : num17);
				if ((this.m_style & FontStyles.Subscript) == FontStyles.Subscript || (this.m_style & FontStyles.Superscript) == FontStyles.Superscript)
				{
					float num19 = (num16 - this.m_baselineOffset) / this.m_currentFontAsset.fontInfo.SubSize;
					num16 = this.m_maxLineAscender;
					this.m_maxLineAscender = ((num19 <= this.m_maxLineAscender) ? this.m_maxLineAscender : num19);
					float num20 = (num17 - this.m_baselineOffset) / this.m_currentFontAsset.fontInfo.SubSize;
					num17 = this.m_maxLineDescender;
					this.m_maxLineDescender = ((num20 >= this.m_maxLineDescender) ? this.m_maxLineDescender : num20);
				}
				if (this.m_lineNumber == 0)
				{
					this.m_maxAscender = ((this.m_maxAscender <= num16) ? num16 : this.m_maxAscender);
				}
				if (num10 == 9 || !char.IsWhiteSpace((char)num10) || this.m_textElementType == TMP_TextElementType.Sprite)
				{
					float num21 = (this.m_width == -1f) ? (x + 0.0001f - this.m_marginLeft - this.m_marginRight) : Mathf.Min(x + 0.0001f - this.m_marginLeft - this.m_marginRight, this.m_width);
					num6 = this.m_xAdvance + this.m_cached_TextElement.xAdvance * ((num10 == 173) ? num13 : num2);
					if (num6 > num21 && this.enableWordWrapping && this.m_characterCount != this.m_firstCharacterOfLine)
					{
						if (num7 == wordWrapState2.previous_WordBreak || flag)
						{
							if (!this.m_isCharacterWrappingEnabled)
							{
								this.m_isCharacterWrappingEnabled = true;
							}
							else
							{
								flag2 = true;
							}
						}
						num9 = this.RestoreWordWrappingState(ref wordWrapState2);
						num7 = num9;
						if (this.m_char_buffer[num9] == 173)
						{
							this.m_isTextTruncated = true;
							this.m_char_buffer[num9] = 45;
							this.CalculatePreferredValues(defaultFontSize, marginSize);
							return Vector2.zero;
						}
						if (this.m_lineNumber > 0 && !TMP_Math.Approximately(this.m_maxLineAscender, this.m_startOfLineAscender) && this.m_lineHeight == 0f)
						{
							float num22 = this.m_maxLineAscender - this.m_startOfLineAscender;
							this.m_lineOffset += num22;
							wordWrapState2.lineOffset = this.m_lineOffset;
							wordWrapState2.previousLineAscender = this.m_maxLineAscender;
						}
						float num23 = this.m_maxLineAscender - this.m_lineOffset;
						float num24 = this.m_maxLineDescender - this.m_lineOffset;
						this.m_maxDescender = ((this.m_maxDescender >= num24) ? num24 : this.m_maxDescender);
						this.m_firstCharacterOfLine = this.m_characterCount;
						num4 += this.m_xAdvance;
						if (this.m_enableWordWrapping)
						{
							num5 = this.m_maxAscender - this.m_maxDescender;
						}
						else
						{
							num5 = Mathf.Max(num5, num23 - num24);
						}
						this.SaveWordWrappingState(ref wordWrapState, num9, this.m_characterCount - 1);
						this.m_lineNumber++;
						if (this.m_lineHeight == 0f)
						{
							float num25 = this.m_internalCharacterInfo[this.m_characterCount].ascender - this.m_internalCharacterInfo[this.m_characterCount].baseLine;
							float num26 = 0f - this.m_maxLineDescender + num25 + (num3 + this.m_lineSpacing + this.m_lineSpacingDelta) * num;
							this.m_lineOffset += num26;
							this.m_startOfLineAscender = num25;
						}
						else
						{
							this.m_lineOffset += this.m_lineHeight + this.m_lineSpacing * num;
						}
						this.m_maxLineAscender = TMP_Text.k_LargeNegativeFloat;
						this.m_maxLineDescender = TMP_Text.k_LargePositiveFloat;
						this.m_xAdvance = this.tag_Indent;
						goto IL_11C1;
					}
				}
				if (this.m_lineNumber > 0 && !TMP_Math.Approximately(this.m_maxLineAscender, this.m_startOfLineAscender) && this.m_lineHeight == 0f && !this.m_isNewPage)
				{
					float num27 = this.m_maxLineAscender - this.m_startOfLineAscender;
					num18 -= num27;
					this.m_lineOffset += num27;
					this.m_startOfLineAscender += num27;
					wordWrapState2.lineOffset = this.m_lineOffset;
					wordWrapState2.previousLineAscender = this.m_startOfLineAscender;
				}
				if (num10 == 9)
				{
					float num28 = this.m_currentFontAsset.fontInfo.TabWidth * num2;
					float num29 = Mathf.Ceil(this.m_xAdvance / num28) * num28;
					this.m_xAdvance = ((num29 <= this.m_xAdvance) ? (this.m_xAdvance + num28) : num29);
				}
				else if (this.m_monoSpacing != 0f)
				{
					this.m_xAdvance += this.m_monoSpacing - num14 + (this.m_characterSpacing + this.m_currentFontAsset.normalSpacingOffset) * num2 + this.m_cSpacing;
				}
				else
				{
					this.m_xAdvance += (this.m_cached_TextElement.xAdvance * num15 + this.m_characterSpacing + this.m_currentFontAsset.normalSpacingOffset) * num2 + this.m_cSpacing;
				}
				if (num10 == 13)
				{
					a = Mathf.Max(a, num4 + this.m_xAdvance);
					num4 = 0f;
					this.m_xAdvance = this.tag_Indent;
				}
				if (num10 == 10 || this.m_characterCount == totalCharacterCount - 1)
				{
					if (this.m_lineNumber > 0 && !TMP_Math.Approximately(this.m_maxLineAscender, this.m_startOfLineAscender) && this.m_lineHeight == 0f)
					{
						float num30 = this.m_maxLineAscender - this.m_startOfLineAscender;
						num18 -= num30;
						this.m_lineOffset += num30;
					}
					float num31 = this.m_maxLineDescender - this.m_lineOffset;
					this.m_maxDescender = ((this.m_maxDescender >= num31) ? num31 : this.m_maxDescender);
					this.m_firstCharacterOfLine = this.m_characterCount + 1;
					if (num10 == 10 && this.m_characterCount != totalCharacterCount - 1)
					{
						a = Mathf.Max(a, num4 + num6);
						num4 = 0f;
					}
					else
					{
						num4 = Mathf.Max(a, num4 + num6);
					}
					num5 = this.m_maxAscender - this.m_maxDescender;
					if (num10 == 10)
					{
						this.SaveWordWrappingState(ref wordWrapState, num9, this.m_characterCount);
						this.SaveWordWrappingState(ref wordWrapState2, num9, this.m_characterCount);
						this.m_lineNumber++;
						if (this.m_lineHeight == 0f)
						{
							float num26 = 0f - this.m_maxLineDescender + num16 + (num3 + this.m_lineSpacing + this.m_paragraphSpacing + this.m_lineSpacingDelta) * num;
							this.m_lineOffset += num26;
						}
						else
						{
							this.m_lineOffset += this.m_lineHeight + (this.m_lineSpacing + this.m_paragraphSpacing) * num;
						}
						this.m_maxLineAscender = TMP_Text.k_LargeNegativeFloat;
						this.m_maxLineDescender = TMP_Text.k_LargePositiveFloat;
						this.m_startOfLineAscender = num16;
						this.m_xAdvance = this.tag_LineIndent + this.tag_Indent;
					}
				}
				if (this.m_enableWordWrapping || this.m_overflowMode == TextOverflowModes.Truncate || this.m_overflowMode == TextOverflowModes.Ellipsis)
				{
					if ((char.IsWhiteSpace((char)num10) || num10 == 45 || num10 == 173) && !this.m_isNonBreakingSpace && num10 != 160 && num10 != 8209 && num10 != 8239 && num10 != 8288)
					{
						this.SaveWordWrappingState(ref wordWrapState2, num9, this.m_characterCount);
						this.m_isCharacterWrappingEnabled = false;
						flag = false;
					}
					else if (((num10 > 4352 && num10 < 4607) || (num10 > 11904 && num10 < 40959) || (num10 > 43360 && num10 < 43391) || (num10 > 44032 && num10 < 55295) || (num10 > 63744 && num10 < 64255) || (num10 > 65072 && num10 < 65103) || (num10 > 65280 && num10 < 65519)) && !this.m_isNonBreakingSpace)
					{
						if (flag || flag2 || (!TMP_Settings.linebreakingRules.leadingCharacters.ContainsKey(num10) && this.m_characterCount < totalCharacterCount - 1 && !TMP_Settings.linebreakingRules.followingCharacters.ContainsKey((int)this.m_internalCharacterInfo[this.m_characterCount + 1].character)))
						{
							this.SaveWordWrappingState(ref wordWrapState2, num9, this.m_characterCount);
							this.m_isCharacterWrappingEnabled = false;
							flag = false;
						}
					}
					else if (flag || this.m_isCharacterWrappingEnabled || flag2)
					{
						this.SaveWordWrappingState(ref wordWrapState2, num9, this.m_characterCount);
					}
				}
				this.m_characterCount++;
				goto IL_11C1;
			}
			this.m_isCharacterWrappingEnabled = false;
			num4 += ((this.m_margin.x <= 0f) ? 0f : this.m_margin.x);
			num4 += ((this.m_margin.z <= 0f) ? 0f : this.m_margin.z);
			num5 += ((this.m_margin.y <= 0f) ? 0f : this.m_margin.y);
			num5 += ((this.m_margin.w <= 0f) ? 0f : this.m_margin.w);
			num4 = (float)((int)(num4 * 100f + 1f)) / 100f;
			num5 = (float)((int)(num5 * 100f + 1f)) / 100f;
			return new Vector2(num4, num5);
		}

		protected virtual Bounds GetCompoundBounds()
		{
			return default(Bounds);
		}

		protected Bounds GetTextBounds()
		{
			if (this.m_textInfo == null)
			{
				return default(Bounds);
			}
			Extents extents = new Extents(TMP_Text.k_LargePositiveVector2, TMP_Text.k_LargeNegativeVector2);
			for (int i = 0; i < this.m_textInfo.characterCount; i++)
			{
				if (this.m_textInfo.characterInfo[i].isVisible)
				{
					extents.min.x = Mathf.Min(extents.min.x, this.m_textInfo.characterInfo[i].bottomLeft.x);
					extents.min.y = Mathf.Min(extents.min.y, this.m_textInfo.characterInfo[i].descender);
					extents.max.x = Mathf.Max(extents.max.x, this.m_textInfo.characterInfo[i].xAdvance);
					extents.max.y = Mathf.Max(extents.max.y, this.m_textInfo.characterInfo[i].ascender);
				}
			}
			Vector2 v;
			v.x = extents.max.x - extents.min.x;
			v.y = extents.max.y - extents.min.y;
			Vector2 v2 = (extents.min + extents.max) / 2f;
			return new Bounds(v2, v);
		}

		protected virtual void AdjustLineOffset(int startIndex, int endIndex, float offset)
		{
		}

		protected void ResizeLineExtents(int size)
		{
			size = ((size <= 1024) ? Mathf.NextPowerOfTwo(size + 1) : (size + 256));
			TMP_LineInfo[] array = new TMP_LineInfo[size];
			for (int i = 0; i < size; i++)
			{
				if (i < this.m_textInfo.lineInfo.Length)
				{
					array[i] = this.m_textInfo.lineInfo[i];
				}
				else
				{
					array[i].lineExtents.min = TMP_Text.k_LargePositiveVector2;
					array[i].lineExtents.max = TMP_Text.k_LargeNegativeVector2;
					array[i].ascender = TMP_Text.k_LargeNegativeFloat;
					array[i].descender = TMP_Text.k_LargePositiveFloat;
				}
			}
			this.m_textInfo.lineInfo = array;
		}

		public virtual TMP_TextInfo GetTextInfo(string text)
		{
			return null;
		}

		protected virtual void ComputeMarginSize()
		{
		}

		protected int GetArraySizes(int[] chars)
		{
			int num = 0;
			this.m_totalCharacterCount = 0;
			this.m_isUsingBold = false;
			this.m_isParsingText = false;
			int num2 = 0;
			while (chars[num2] != 0)
			{
				int num3 = chars[num2];
				if (this.m_isRichText && num3 == 60 && this.ValidateHtmlTag(chars, num2 + 1, out num))
				{
					num2 = num;
					if ((this.m_style & FontStyles.Bold) == FontStyles.Bold)
					{
						this.m_isUsingBold = true;
					}
				}
				else
				{
					if (!char.IsWhiteSpace((char)num3))
					{
					}
					this.m_totalCharacterCount++;
				}
				num2++;
			}
			return this.m_totalCharacterCount;
		}

		protected void SaveWordWrappingState(ref WordWrapState state, int index, int count)
		{
			state.currentFontAsset = this.m_currentFontAsset;
			state.currentSpriteAsset = this.m_currentSpriteAsset;
			state.currentMaterial = this.m_currentMaterial;
			state.currentMaterialIndex = this.m_currentMaterialIndex;
			state.previous_WordBreak = index;
			state.total_CharacterCount = count;
			state.visible_CharacterCount = this.m_lineVisibleCharacterCount;
			state.visible_LinkCount = this.m_textInfo.linkCount;
			state.firstCharacterIndex = this.m_firstCharacterOfLine;
			state.firstVisibleCharacterIndex = this.m_firstVisibleCharacterOfLine;
			state.lastVisibleCharIndex = this.m_lastVisibleCharacterOfLine;
			state.fontStyle = this.m_style;
			state.fontScale = this.m_fontScale;
			state.fontScaleMultiplier = this.m_fontScaleMultiplier;
			state.currentFontSize = this.m_currentFontSize;
			state.xAdvance = this.m_xAdvance;
			state.maxCapHeight = this.m_maxCapHeight;
			state.maxAscender = this.m_maxAscender;
			state.maxDescender = this.m_maxDescender;
			state.maxLineAscender = this.m_maxLineAscender;
			state.maxLineDescender = this.m_maxLineDescender;
			state.previousLineAscender = this.m_startOfLineAscender;
			state.preferredWidth = this.m_preferredWidth;
			state.preferredHeight = this.m_preferredHeight;
			state.meshExtents = this.m_meshExtents;
			state.lineNumber = this.m_lineNumber;
			state.lineOffset = this.m_lineOffset;
			state.baselineOffset = this.m_baselineOffset;
			state.vertexColor = this.m_htmlColor;
			state.tagNoParsing = this.tag_NoParsing;
			state.colorStack = this.m_colorStack;
			state.sizeStack = this.m_sizeStack;
			state.fontWeightStack = this.m_fontWeightStack;
			state.styleStack = this.m_styleStack;
			state.actionStack = this.m_actionStack;
			state.materialReferenceStack = this.m_materialReferenceStack;
			if (this.m_lineNumber < this.m_textInfo.lineInfo.Length)
			{
				state.lineInfo = this.m_textInfo.lineInfo[this.m_lineNumber];
			}
		}

		protected int RestoreWordWrappingState(ref WordWrapState state)
		{
			int previous_WordBreak = state.previous_WordBreak;
			this.m_currentFontAsset = state.currentFontAsset;
			this.m_currentSpriteAsset = state.currentSpriteAsset;
			this.m_currentMaterial = state.currentMaterial;
			this.m_currentMaterialIndex = state.currentMaterialIndex;
			this.m_characterCount = state.total_CharacterCount + 1;
			this.m_lineVisibleCharacterCount = state.visible_CharacterCount;
			this.m_textInfo.linkCount = state.visible_LinkCount;
			this.m_firstCharacterOfLine = state.firstCharacterIndex;
			this.m_firstVisibleCharacterOfLine = state.firstVisibleCharacterIndex;
			this.m_lastVisibleCharacterOfLine = state.lastVisibleCharIndex;
			this.m_style = state.fontStyle;
			this.m_fontScale = state.fontScale;
			this.m_fontScaleMultiplier = state.fontScaleMultiplier;
			this.m_currentFontSize = state.currentFontSize;
			this.m_xAdvance = state.xAdvance;
			this.m_maxCapHeight = state.maxCapHeight;
			this.m_maxAscender = state.maxAscender;
			this.m_maxDescender = state.maxDescender;
			this.m_maxLineAscender = state.maxLineAscender;
			this.m_maxLineDescender = state.maxLineDescender;
			this.m_startOfLineAscender = state.previousLineAscender;
			this.m_preferredWidth = state.preferredWidth;
			this.m_preferredHeight = state.preferredHeight;
			this.m_meshExtents = state.meshExtents;
			this.m_lineNumber = state.lineNumber;
			this.m_lineOffset = state.lineOffset;
			this.m_baselineOffset = state.baselineOffset;
			this.m_htmlColor = state.vertexColor;
			this.tag_NoParsing = state.tagNoParsing;
			this.m_colorStack = state.colorStack;
			this.m_sizeStack = state.sizeStack;
			this.m_fontWeightStack = state.fontWeightStack;
			this.m_styleStack = state.styleStack;
			this.m_actionStack = state.actionStack;
			this.m_materialReferenceStack = state.materialReferenceStack;
			if (this.m_lineNumber < this.m_textInfo.lineInfo.Length)
			{
				this.m_textInfo.lineInfo[this.m_lineNumber] = state.lineInfo;
			}
			return previous_WordBreak;
		}

		protected virtual void SaveGlyphVertexInfo(float padding, float style_padding, Color32 vertexColor)
		{
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.position = this.m_textInfo.characterInfo[this.m_characterCount].bottomLeft;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.position = this.m_textInfo.characterInfo[this.m_characterCount].topLeft;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.position = this.m_textInfo.characterInfo[this.m_characterCount].topRight;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.position = this.m_textInfo.characterInfo[this.m_characterCount].bottomRight;
			vertexColor.a = ((this.m_fontColor32.a >= vertexColor.a) ? vertexColor.a : this.m_fontColor32.a);
			if (!this.m_enableVertexGradient)
			{
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.color = vertexColor;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.color = vertexColor;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.color = vertexColor;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.color = vertexColor;
			}
			else if (!this.m_overrideHtmlColors && !this.m_htmlColor.CompareRGB(this.m_fontColor32))
			{
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.color = vertexColor;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.color = vertexColor;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.color = vertexColor;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.color = vertexColor;
			}
			else if (this.m_fontColorGradientPreset != null)
			{
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.color = this.m_fontColorGradientPreset.bottomLeft * vertexColor;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.color = this.m_fontColorGradientPreset.topLeft * vertexColor;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.color = this.m_fontColorGradientPreset.topRight * vertexColor;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.color = this.m_fontColorGradientPreset.bottomRight * vertexColor;
			}
			else
			{
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.color = this.m_fontColorGradient.bottomLeft * vertexColor;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.color = this.m_fontColorGradient.topLeft * vertexColor;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.color = this.m_fontColorGradient.topRight * vertexColor;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.color = this.m_fontColorGradient.bottomRight * vertexColor;
			}
			if (!this.m_isSDFShader)
			{
				style_padding = 0f;
			}
			FaceInfo fontInfo = this.m_currentFontAsset.fontInfo;
			Vector2 uv;
			uv.x = (this.m_cached_TextElement.x - padding - style_padding) / fontInfo.AtlasWidth;
			uv.y = 1f - (this.m_cached_TextElement.y + padding + style_padding + this.m_cached_TextElement.height) / fontInfo.AtlasHeight;
			Vector2 uv2;
			uv2.x = uv.x;
			uv2.y = 1f - (this.m_cached_TextElement.y - padding - style_padding) / fontInfo.AtlasHeight;
			Vector2 uv3;
			uv3.x = (this.m_cached_TextElement.x + padding + style_padding + this.m_cached_TextElement.width) / fontInfo.AtlasWidth;
			uv3.y = uv2.y;
			Vector2 uv4;
			uv4.x = uv3.x;
			uv4.y = uv.y;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.uv = uv;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.uv = uv2;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.uv = uv3;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.uv = uv4;
		}

		protected virtual void SaveSpriteVertexInfo(Color32 vertexColor)
		{
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.position = this.m_textInfo.characterInfo[this.m_characterCount].bottomLeft;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.position = this.m_textInfo.characterInfo[this.m_characterCount].topLeft;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.position = this.m_textInfo.characterInfo[this.m_characterCount].topRight;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.position = this.m_textInfo.characterInfo[this.m_characterCount].bottomRight;
			if (this.m_tintAllSprites)
			{
				this.m_tintSprite = true;
			}
			Color32 color = (!this.m_tintSprite) ? this.m_spriteColor : this.m_spriteColor.Multiply(vertexColor);
			color.a = ((color.a >= this.m_fontColor32.a) ? this.m_fontColor32.a : (color.a = ((color.a >= vertexColor.a) ? vertexColor.a : color.a)));
			if (!this.m_enableVertexGradient)
			{
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.color = color;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.color = color;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.color = color;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.color = color;
			}
			else if (!this.m_overrideHtmlColors && !this.m_htmlColor.CompareRGB(this.m_fontColor32))
			{
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.color = color;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.color = color;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.color = color;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.color = color;
			}
			else if (this.m_fontColorGradientPreset != null)
			{
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.color = ((!this.m_tintSprite) ? color : color.Multiply(this.m_fontColorGradientPreset.bottomLeft));
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.color = ((!this.m_tintSprite) ? color : color.Multiply(this.m_fontColorGradientPreset.topLeft));
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.color = ((!this.m_tintSprite) ? color : color.Multiply(this.m_fontColorGradientPreset.topRight));
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.color = ((!this.m_tintSprite) ? color : color.Multiply(this.m_fontColorGradientPreset.bottomRight));
			}
			else
			{
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.color = ((!this.m_tintSprite) ? color : color.Multiply(this.m_fontColorGradient.bottomLeft));
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.color = ((!this.m_tintSprite) ? color : color.Multiply(this.m_fontColorGradient.topLeft));
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.color = ((!this.m_tintSprite) ? color : color.Multiply(this.m_fontColorGradient.topRight));
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.color = ((!this.m_tintSprite) ? color : color.Multiply(this.m_fontColorGradient.bottomRight));
			}
			Vector2 uv = new Vector2(this.m_cached_TextElement.x / (float)this.m_currentSpriteAsset.spriteSheet.width, this.m_cached_TextElement.y / (float)this.m_currentSpriteAsset.spriteSheet.height);
			Vector2 uv2 = new Vector2(uv.x, (this.m_cached_TextElement.y + this.m_cached_TextElement.height) / (float)this.m_currentSpriteAsset.spriteSheet.height);
			Vector2 uv3 = new Vector2((this.m_cached_TextElement.x + this.m_cached_TextElement.width) / (float)this.m_currentSpriteAsset.spriteSheet.width, uv2.y);
			Vector2 uv4 = new Vector2(uv3.x, uv.y);
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.uv = uv;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.uv = uv2;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.uv = uv3;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.uv = uv4;
		}

		protected virtual void FillCharacterVertexBuffers(int i, int index_X4)
		{
			int materialReferenceIndex = this.m_textInfo.characterInfo[i].materialReferenceIndex;
			index_X4 = this.m_textInfo.meshInfo[materialReferenceIndex].vertexCount;
			TMP_CharacterInfo[] characterInfo = this.m_textInfo.characterInfo;
			this.m_textInfo.characterInfo[i].vertexIndex = index_X4;
			this.m_textInfo.meshInfo[materialReferenceIndex].vertices[index_X4] = characterInfo[i].vertex_BL.position;
			this.m_textInfo.meshInfo[materialReferenceIndex].vertices[1 + index_X4] = characterInfo[i].vertex_TL.position;
			this.m_textInfo.meshInfo[materialReferenceIndex].vertices[2 + index_X4] = characterInfo[i].vertex_TR.position;
			this.m_textInfo.meshInfo[materialReferenceIndex].vertices[3 + index_X4] = characterInfo[i].vertex_BR.position;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[index_X4] = characterInfo[i].vertex_BL.uv;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[1 + index_X4] = characterInfo[i].vertex_TL.uv;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[2 + index_X4] = characterInfo[i].vertex_TR.uv;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[3 + index_X4] = characterInfo[i].vertex_BR.uv;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[index_X4] = characterInfo[i].vertex_BL.uv2;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[1 + index_X4] = characterInfo[i].vertex_TL.uv2;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[2 + index_X4] = characterInfo[i].vertex_TR.uv2;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[3 + index_X4] = characterInfo[i].vertex_BR.uv2;
			this.m_textInfo.meshInfo[materialReferenceIndex].colors32[index_X4] = characterInfo[i].vertex_BL.color;
			this.m_textInfo.meshInfo[materialReferenceIndex].colors32[1 + index_X4] = characterInfo[i].vertex_TL.color;
			this.m_textInfo.meshInfo[materialReferenceIndex].colors32[2 + index_X4] = characterInfo[i].vertex_TR.color;
			this.m_textInfo.meshInfo[materialReferenceIndex].colors32[3 + index_X4] = characterInfo[i].vertex_BR.color;
			this.m_textInfo.meshInfo[materialReferenceIndex].vertexCount = index_X4 + 4;
		}

		protected virtual void FillCharacterVertexBuffers(int i, int index_X4, bool isVolumetric)
		{
			int materialReferenceIndex = this.m_textInfo.characterInfo[i].materialReferenceIndex;
			index_X4 = this.m_textInfo.meshInfo[materialReferenceIndex].vertexCount;
			TMP_CharacterInfo[] characterInfo = this.m_textInfo.characterInfo;
			this.m_textInfo.characterInfo[i].vertexIndex = index_X4;
			this.m_textInfo.meshInfo[materialReferenceIndex].vertices[index_X4] = characterInfo[i].vertex_BL.position;
			this.m_textInfo.meshInfo[materialReferenceIndex].vertices[1 + index_X4] = characterInfo[i].vertex_TL.position;
			this.m_textInfo.meshInfo[materialReferenceIndex].vertices[2 + index_X4] = characterInfo[i].vertex_TR.position;
			this.m_textInfo.meshInfo[materialReferenceIndex].vertices[3 + index_X4] = characterInfo[i].vertex_BR.position;
			if (isVolumetric)
			{
				Vector3 b = new Vector3(0f, 0f, this.m_fontSize * this.m_fontScale);
				this.m_textInfo.meshInfo[materialReferenceIndex].vertices[4 + index_X4] = characterInfo[i].vertex_BL.position + b;
				this.m_textInfo.meshInfo[materialReferenceIndex].vertices[5 + index_X4] = characterInfo[i].vertex_TL.position + b;
				this.m_textInfo.meshInfo[materialReferenceIndex].vertices[6 + index_X4] = characterInfo[i].vertex_TR.position + b;
				this.m_textInfo.meshInfo[materialReferenceIndex].vertices[7 + index_X4] = characterInfo[i].vertex_BR.position + b;
			}
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[index_X4] = characterInfo[i].vertex_BL.uv;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[1 + index_X4] = characterInfo[i].vertex_TL.uv;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[2 + index_X4] = characterInfo[i].vertex_TR.uv;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[3 + index_X4] = characterInfo[i].vertex_BR.uv;
			if (isVolumetric)
			{
				this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[4 + index_X4] = characterInfo[i].vertex_BL.uv;
				this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[5 + index_X4] = characterInfo[i].vertex_TL.uv;
				this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[6 + index_X4] = characterInfo[i].vertex_TR.uv;
				this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[7 + index_X4] = characterInfo[i].vertex_BR.uv;
			}
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[index_X4] = characterInfo[i].vertex_BL.uv2;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[1 + index_X4] = characterInfo[i].vertex_TL.uv2;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[2 + index_X4] = characterInfo[i].vertex_TR.uv2;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[3 + index_X4] = characterInfo[i].vertex_BR.uv2;
			if (isVolumetric)
			{
				this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[4 + index_X4] = characterInfo[i].vertex_BL.uv2;
				this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[5 + index_X4] = characterInfo[i].vertex_TL.uv2;
				this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[6 + index_X4] = characterInfo[i].vertex_TR.uv2;
				this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[7 + index_X4] = characterInfo[i].vertex_BR.uv2;
			}
			this.m_textInfo.meshInfo[materialReferenceIndex].colors32[index_X4] = characterInfo[i].vertex_BL.color;
			this.m_textInfo.meshInfo[materialReferenceIndex].colors32[1 + index_X4] = characterInfo[i].vertex_TL.color;
			this.m_textInfo.meshInfo[materialReferenceIndex].colors32[2 + index_X4] = characterInfo[i].vertex_TR.color;
			this.m_textInfo.meshInfo[materialReferenceIndex].colors32[3 + index_X4] = characterInfo[i].vertex_BR.color;
			if (isVolumetric)
			{
				Color32 color = new Color32(byte.MaxValue, byte.MaxValue, 128, byte.MaxValue);
				this.m_textInfo.meshInfo[materialReferenceIndex].colors32[4 + index_X4] = color;
				this.m_textInfo.meshInfo[materialReferenceIndex].colors32[5 + index_X4] = color;
				this.m_textInfo.meshInfo[materialReferenceIndex].colors32[6 + index_X4] = color;
				this.m_textInfo.meshInfo[materialReferenceIndex].colors32[7 + index_X4] = color;
			}
			this.m_textInfo.meshInfo[materialReferenceIndex].vertexCount = index_X4 + (isVolumetric ? 8 : 4);
		}

		protected virtual void FillSpriteVertexBuffers(int i, int index_X4)
		{
			int materialReferenceIndex = this.m_textInfo.characterInfo[i].materialReferenceIndex;
			index_X4 = this.m_textInfo.meshInfo[materialReferenceIndex].vertexCount;
			TMP_CharacterInfo[] characterInfo = this.m_textInfo.characterInfo;
			this.m_textInfo.characterInfo[i].vertexIndex = index_X4;
			this.m_textInfo.meshInfo[materialReferenceIndex].vertices[index_X4] = characterInfo[i].vertex_BL.position;
			this.m_textInfo.meshInfo[materialReferenceIndex].vertices[1 + index_X4] = characterInfo[i].vertex_TL.position;
			this.m_textInfo.meshInfo[materialReferenceIndex].vertices[2 + index_X4] = characterInfo[i].vertex_TR.position;
			this.m_textInfo.meshInfo[materialReferenceIndex].vertices[3 + index_X4] = characterInfo[i].vertex_BR.position;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[index_X4] = characterInfo[i].vertex_BL.uv;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[1 + index_X4] = characterInfo[i].vertex_TL.uv;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[2 + index_X4] = characterInfo[i].vertex_TR.uv;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[3 + index_X4] = characterInfo[i].vertex_BR.uv;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[index_X4] = characterInfo[i].vertex_BL.uv2;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[1 + index_X4] = characterInfo[i].vertex_TL.uv2;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[2 + index_X4] = characterInfo[i].vertex_TR.uv2;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[3 + index_X4] = characterInfo[i].vertex_BR.uv2;
			this.m_textInfo.meshInfo[materialReferenceIndex].colors32[index_X4] = characterInfo[i].vertex_BL.color;
			this.m_textInfo.meshInfo[materialReferenceIndex].colors32[1 + index_X4] = characterInfo[i].vertex_TL.color;
			this.m_textInfo.meshInfo[materialReferenceIndex].colors32[2 + index_X4] = characterInfo[i].vertex_TR.color;
			this.m_textInfo.meshInfo[materialReferenceIndex].colors32[3 + index_X4] = characterInfo[i].vertex_BR.color;
			this.m_textInfo.meshInfo[materialReferenceIndex].vertexCount = index_X4 + 4;
		}

		protected virtual void DrawUnderlineMesh(Vector3 start, Vector3 end, ref int index, float startScale, float endScale, float maxScale, float sdfScale, Color32 underlineColor)
		{
			if (this.m_cached_Underline_GlyphInfo == null)
			{
				if (!TMP_Settings.warningsDisabled)
				{
					UnityEngine.Debug.LogWarning("Unable to add underline since the Font Asset doesn't contain the underline character.", this);
				}
				return;
			}
			int num = index + 12;
			if (num > this.m_textInfo.meshInfo[0].vertices.Length)
			{
				this.m_textInfo.meshInfo[0].ResizeMeshInfo(num / 4);
			}
			start.y = Mathf.Min(start.y, end.y);
			end.y = Mathf.Min(start.y, end.y);
			float num2 = this.m_cached_Underline_GlyphInfo.width / 2f * maxScale;
			if (end.x - start.x < this.m_cached_Underline_GlyphInfo.width * maxScale)
			{
				num2 = (end.x - start.x) / 2f;
			}
			float num3 = this.m_padding * startScale / maxScale;
			float num4 = this.m_padding * endScale / maxScale;
			float height = this.m_cached_Underline_GlyphInfo.height;
			Vector3[] vertices = this.m_textInfo.meshInfo[0].vertices;
			vertices[index] = start + new Vector3(0f, 0f - (height + this.m_padding) * maxScale, 0f);
			vertices[index + 1] = start + new Vector3(0f, this.m_padding * maxScale, 0f);
			vertices[index + 2] = vertices[index + 1] + new Vector3(num2, 0f, 0f);
			vertices[index + 3] = vertices[index] + new Vector3(num2, 0f, 0f);
			vertices[index + 4] = vertices[index + 3];
			vertices[index + 5] = vertices[index + 2];
			vertices[index + 6] = end + new Vector3(-num2, this.m_padding * maxScale, 0f);
			vertices[index + 7] = end + new Vector3(-num2, -(height + this.m_padding) * maxScale, 0f);
			vertices[index + 8] = vertices[index + 7];
			vertices[index + 9] = vertices[index + 6];
			vertices[index + 10] = end + new Vector3(0f, this.m_padding * maxScale, 0f);
			vertices[index + 11] = end + new Vector3(0f, -(height + this.m_padding) * maxScale, 0f);
			Vector2[] uvs = this.m_textInfo.meshInfo[0].uvs0;
			Vector2 vector = new Vector2((this.m_cached_Underline_GlyphInfo.x - num3) / this.m_fontAsset.fontInfo.AtlasWidth, 1f - (this.m_cached_Underline_GlyphInfo.y + this.m_padding + this.m_cached_Underline_GlyphInfo.height) / this.m_fontAsset.fontInfo.AtlasHeight);
			Vector2 vector2 = new Vector2(vector.x, 1f - (this.m_cached_Underline_GlyphInfo.y - this.m_padding) / this.m_fontAsset.fontInfo.AtlasHeight);
			Vector2 vector3 = new Vector2((this.m_cached_Underline_GlyphInfo.x - num3 + this.m_cached_Underline_GlyphInfo.width / 2f) / this.m_fontAsset.fontInfo.AtlasWidth, vector2.y);
			Vector2 vector4 = new Vector2(vector3.x, vector.y);
			Vector2 vector5 = new Vector2((this.m_cached_Underline_GlyphInfo.x + num4 + this.m_cached_Underline_GlyphInfo.width / 2f) / this.m_fontAsset.fontInfo.AtlasWidth, vector2.y);
			Vector2 vector6 = new Vector2(vector5.x, vector.y);
			Vector2 vector7 = new Vector2((this.m_cached_Underline_GlyphInfo.x + num4 + this.m_cached_Underline_GlyphInfo.width) / this.m_fontAsset.fontInfo.AtlasWidth, vector2.y);
			Vector2 vector8 = new Vector2(vector7.x, vector.y);
			uvs[index] = vector;
			uvs[1 + index] = vector2;
			uvs[2 + index] = vector3;
			uvs[3 + index] = vector4;
			uvs[4 + index] = new Vector2(vector3.x - vector3.x * 0.001f, vector.y);
			uvs[5 + index] = new Vector2(vector3.x - vector3.x * 0.001f, vector2.y);
			uvs[6 + index] = new Vector2(vector3.x + vector3.x * 0.001f, vector2.y);
			uvs[7 + index] = new Vector2(vector3.x + vector3.x * 0.001f, vector.y);
			uvs[8 + index] = vector6;
			uvs[9 + index] = vector5;
			uvs[10 + index] = vector7;
			uvs[11 + index] = vector8;
			float x = (vertices[index + 2].x - start.x) / (end.x - start.x);
			float scale = Mathf.Abs(sdfScale);
			Vector2[] uvs2 = this.m_textInfo.meshInfo[0].uvs2;
			uvs2[index] = this.PackUV(0f, 0f, scale);
			uvs2[1 + index] = this.PackUV(0f, 1f, scale);
			uvs2[2 + index] = this.PackUV(x, 1f, scale);
			uvs2[3 + index] = this.PackUV(x, 0f, scale);
			float x2 = (vertices[index + 4].x - start.x) / (end.x - start.x);
			x = (vertices[index + 6].x - start.x) / (end.x - start.x);
			uvs2[4 + index] = this.PackUV(x2, 0f, scale);
			uvs2[5 + index] = this.PackUV(x2, 1f, scale);
			uvs2[6 + index] = this.PackUV(x, 1f, scale);
			uvs2[7 + index] = this.PackUV(x, 0f, scale);
			x2 = (vertices[index + 8].x - start.x) / (end.x - start.x);
			x = (vertices[index + 6].x - start.x) / (end.x - start.x);
			uvs2[8 + index] = this.PackUV(x2, 0f, scale);
			uvs2[9 + index] = this.PackUV(x2, 1f, scale);
			uvs2[10 + index] = this.PackUV(1f, 1f, scale);
			uvs2[11 + index] = this.PackUV(1f, 0f, scale);
			Color32[] colors = this.m_textInfo.meshInfo[0].colors32;
			colors[index] = underlineColor;
			colors[1 + index] = underlineColor;
			colors[2 + index] = underlineColor;
			colors[3 + index] = underlineColor;
			colors[4 + index] = underlineColor;
			colors[5 + index] = underlineColor;
			colors[6 + index] = underlineColor;
			colors[7 + index] = underlineColor;
			colors[8 + index] = underlineColor;
			colors[9 + index] = underlineColor;
			colors[10 + index] = underlineColor;
			colors[11 + index] = underlineColor;
			index += 12;
		}

		protected void GetSpecialCharacters(TMP_FontAsset fontAsset)
		{
			if (!fontAsset.characterDictionary.TryGetValue(95, out this.m_cached_Underline_GlyphInfo))
			{
			}
			if (!fontAsset.characterDictionary.TryGetValue(8230, out this.m_cached_Ellipsis_GlyphInfo))
			{
			}
		}

		protected TMP_FontAsset GetFontAssetForWeight(int fontWeight)
		{
			bool flag = (this.m_style & FontStyles.Italic) == FontStyles.Italic || (this.m_fontStyle & FontStyles.Italic) == FontStyles.Italic;
			int num = fontWeight / 100;
			TMP_FontAsset result;
			if (flag)
			{
				result = this.m_currentFontAsset.fontWeights[num].italicTypeface;
			}
			else
			{
				result = this.m_currentFontAsset.fontWeights[num].regularTypeface;
			}
			return result;
		}

		protected virtual void SetActiveSubMeshes(bool state)
		{
		}

		protected Vector2 PackUV(float x, float y, float scale)
		{
			Vector2 result;
			result.x = Mathf.Floor(x * 511f);
			result.y = Mathf.Floor(y * 511f);
			result.x = result.x * 4096f + result.y;
			result.y = scale;
			return result;
		}

		protected float PackUV(float x, float y)
		{
			double num = Math.Floor((double)(x * 511f));
			double num2 = Math.Floor((double)(y * 511f));
			return (float)(num * 4096.0 + num2);
		}

		protected int HexToInt(char hex)
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

		protected int GetUTF16(int i)
		{
			int num = this.HexToInt(this.m_text[i]) * 4096;
			num += this.HexToInt(this.m_text[i + 1]) * 256;
			num += this.HexToInt(this.m_text[i + 2]) * 16;
			return num + this.HexToInt(this.m_text[i + 3]);
		}

		protected int GetUTF32(int i)
		{
			int num = 0;
			num += this.HexToInt(this.m_text[i]) * 268435456;
			num += this.HexToInt(this.m_text[i + 1]) * 16777216;
			num += this.HexToInt(this.m_text[i + 2]) * 1048576;
			num += this.HexToInt(this.m_text[i + 3]) * 65536;
			num += this.HexToInt(this.m_text[i + 4]) * 4096;
			num += this.HexToInt(this.m_text[i + 5]) * 256;
			num += this.HexToInt(this.m_text[i + 6]) * 16;
			return num + this.HexToInt(this.m_text[i + 7]);
		}

		protected Color32 HexCharsToColor(char[] hexChars, int tagCount)
		{
			if (tagCount == 7)
			{
				byte r = (byte)(this.HexToInt(hexChars[1]) * 16 + this.HexToInt(hexChars[2]));
				byte g = (byte)(this.HexToInt(hexChars[3]) * 16 + this.HexToInt(hexChars[4]));
				byte b = (byte)(this.HexToInt(hexChars[5]) * 16 + this.HexToInt(hexChars[6]));
				return new Color32(r, g, b, byte.MaxValue);
			}
			if (tagCount == 9)
			{
				byte r2 = (byte)(this.HexToInt(hexChars[1]) * 16 + this.HexToInt(hexChars[2]));
				byte g2 = (byte)(this.HexToInt(hexChars[3]) * 16 + this.HexToInt(hexChars[4]));
				byte b2 = (byte)(this.HexToInt(hexChars[5]) * 16 + this.HexToInt(hexChars[6]));
				byte a = (byte)(this.HexToInt(hexChars[7]) * 16 + this.HexToInt(hexChars[8]));
				return new Color32(r2, g2, b2, a);
			}
			if (tagCount == 13)
			{
				byte r3 = (byte)(this.HexToInt(hexChars[7]) * 16 + this.HexToInt(hexChars[8]));
				byte g3 = (byte)(this.HexToInt(hexChars[9]) * 16 + this.HexToInt(hexChars[10]));
				byte b3 = (byte)(this.HexToInt(hexChars[11]) * 16 + this.HexToInt(hexChars[12]));
				return new Color32(r3, g3, b3, byte.MaxValue);
			}
			if (tagCount == 15)
			{
				byte r4 = (byte)(this.HexToInt(hexChars[7]) * 16 + this.HexToInt(hexChars[8]));
				byte g4 = (byte)(this.HexToInt(hexChars[9]) * 16 + this.HexToInt(hexChars[10]));
				byte b4 = (byte)(this.HexToInt(hexChars[11]) * 16 + this.HexToInt(hexChars[12]));
				byte a2 = (byte)(this.HexToInt(hexChars[13]) * 16 + this.HexToInt(hexChars[14]));
				return new Color32(r4, g4, b4, a2);
			}
			return new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
		}

		protected Color32 HexCharsToColor(char[] hexChars, int startIndex, int length)
		{
			if (length == 7)
			{
				byte r = (byte)(this.HexToInt(hexChars[startIndex + 1]) * 16 + this.HexToInt(hexChars[startIndex + 2]));
				byte g = (byte)(this.HexToInt(hexChars[startIndex + 3]) * 16 + this.HexToInt(hexChars[startIndex + 4]));
				byte b = (byte)(this.HexToInt(hexChars[startIndex + 5]) * 16 + this.HexToInt(hexChars[startIndex + 6]));
				return new Color32(r, g, b, byte.MaxValue);
			}
			if (length == 9)
			{
				byte r2 = (byte)(this.HexToInt(hexChars[startIndex + 1]) * 16 + this.HexToInt(hexChars[startIndex + 2]));
				byte g2 = (byte)(this.HexToInt(hexChars[startIndex + 3]) * 16 + this.HexToInt(hexChars[startIndex + 4]));
				byte b2 = (byte)(this.HexToInt(hexChars[startIndex + 5]) * 16 + this.HexToInt(hexChars[startIndex + 6]));
				byte a = (byte)(this.HexToInt(hexChars[startIndex + 7]) * 16 + this.HexToInt(hexChars[startIndex + 8]));
				return new Color32(r2, g2, b2, a);
			}
			return new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
		}

		protected float ConvertToFloat(char[] chars, int startIndex, int length, int decimalPointIndex)
		{
			if (startIndex == 0)
			{
				return -9999f;
			}
			int num = startIndex + length - 1;
			float num2 = 0f;
			float num3 = 1f;
			decimalPointIndex = ((decimalPointIndex <= 0) ? (num + 1) : decimalPointIndex);
			if (chars[startIndex] == '-')
			{
				startIndex++;
				num3 = -1f;
			}
			if (chars[startIndex] == '+' || chars[startIndex] == '%')
			{
				startIndex++;
			}
			for (int i = startIndex; i < num + 1; i++)
			{
				if (!char.IsDigit(chars[i]) && chars[i] != '.')
				{
					return -9999f;
				}
				int num4 = decimalPointIndex - i;
				switch (num4 + 3)
				{
				case 0:
					num2 += (float)(chars[i] - '0') * 0.001f;
					break;
				case 1:
					num2 += (float)(chars[i] - '0') * 0.01f;
					break;
				case 2:
					num2 += (float)(chars[i] - '0') * 0.1f;
					break;
				case 4:
					num2 += (float)(chars[i] - '0');
					break;
				case 5:
					num2 += (float)((chars[i] - '0') * '\n');
					break;
				case 6:
					num2 += (float)((chars[i] - '0') * 'd');
					break;
				case 7:
					num2 += (float)((chars[i] - '0') * 'Ϩ');
					break;
				}
			}
			return num2 * num3;
		}

		protected bool ValidateHtmlTag(int[] chars, int startIndex, out int endIndex)
		{
			int num = 0;
			byte b = 0;
			TagUnits tagUnits = TagUnits.Pixels;
			TagType tagType = TagType.None;
			int num2 = 0;
			this.m_xmlAttribute[num2].nameHashCode = 0;
			this.m_xmlAttribute[num2].valueType = TagType.None;
			this.m_xmlAttribute[num2].valueHashCode = 0;
			this.m_xmlAttribute[num2].valueStartIndex = 0;
			this.m_xmlAttribute[num2].valueLength = 0;
			this.m_xmlAttribute[num2].valueDecimalIndex = 0;
			endIndex = startIndex;
			bool flag = false;
			bool flag2 = false;
			int num3 = startIndex;
			while (num3 < chars.Length && chars[num3] != 0 && num < this.m_htmlTag.Length && chars[num3] != 60)
			{
				if (chars[num3] == 62)
				{
					flag2 = true;
					endIndex = num3;
					this.m_htmlTag[num] = '\0';
					break;
				}
				this.m_htmlTag[num] = (char)chars[num3];
				num++;
				if (b == 1)
				{
					if (tagType == TagType.None)
					{
						if (chars[num3] == 43 || chars[num3] == 45 || char.IsDigit((char)chars[num3]))
						{
							tagType = TagType.NumericalValue;
							this.m_xmlAttribute[num2].valueType = TagType.NumericalValue;
							this.m_xmlAttribute[num2].valueStartIndex = num - 1;
							XML_TagAttribute[] xmlAttribute = this.m_xmlAttribute;
							int num4 = num2;
							xmlAttribute[num4].valueLength = xmlAttribute[num4].valueLength + 1;
						}
						else if (chars[num3] == 35)
						{
							tagType = TagType.ColorValue;
							this.m_xmlAttribute[num2].valueType = TagType.ColorValue;
							this.m_xmlAttribute[num2].valueStartIndex = num - 1;
							XML_TagAttribute[] xmlAttribute2 = this.m_xmlAttribute;
							int num5 = num2;
							xmlAttribute2[num5].valueLength = xmlAttribute2[num5].valueLength + 1;
						}
						else if (chars[num3] == 34)
						{
							tagType = TagType.StringValue;
							this.m_xmlAttribute[num2].valueType = TagType.StringValue;
							this.m_xmlAttribute[num2].valueStartIndex = num;
						}
						else
						{
							tagType = TagType.StringValue;
							this.m_xmlAttribute[num2].valueType = TagType.StringValue;
							this.m_xmlAttribute[num2].valueStartIndex = num - 1;
							this.m_xmlAttribute[num2].valueHashCode = ((this.m_xmlAttribute[num2].valueHashCode << 5) + this.m_xmlAttribute[num2].valueHashCode ^ chars[num3]);
							XML_TagAttribute[] xmlAttribute3 = this.m_xmlAttribute;
							int num6 = num2;
							xmlAttribute3[num6].valueLength = xmlAttribute3[num6].valueLength + 1;
						}
					}
					else if (tagType == TagType.NumericalValue)
					{
						if (chars[num3] == 46)
						{
							this.m_xmlAttribute[num2].valueDecimalIndex = num - 1;
						}
						if (chars[num3] == 112 || chars[num3] == 101 || chars[num3] == 37 || chars[num3] == 32)
						{
							b = 2;
							tagType = TagType.None;
							num2++;
							this.m_xmlAttribute[num2].nameHashCode = 0;
							this.m_xmlAttribute[num2].valueType = TagType.None;
							this.m_xmlAttribute[num2].valueHashCode = 0;
							this.m_xmlAttribute[num2].valueStartIndex = 0;
							this.m_xmlAttribute[num2].valueLength = 0;
							this.m_xmlAttribute[num2].valueDecimalIndex = 0;
							if (chars[num3] == 101)
							{
								tagUnits = TagUnits.FontUnits;
							}
							else if (chars[num3] == 37)
							{
								tagUnits = TagUnits.Percentage;
							}
						}
						else if (b != 2)
						{
							XML_TagAttribute[] xmlAttribute4 = this.m_xmlAttribute;
							int num7 = num2;
							xmlAttribute4[num7].valueLength = xmlAttribute4[num7].valueLength + 1;
						}
					}
					else if (tagType == TagType.ColorValue)
					{
						if (chars[num3] != 32)
						{
							XML_TagAttribute[] xmlAttribute5 = this.m_xmlAttribute;
							int num8 = num2;
							xmlAttribute5[num8].valueLength = xmlAttribute5[num8].valueLength + 1;
						}
						else
						{
							b = 2;
							tagType = TagType.None;
							num2++;
							this.m_xmlAttribute[num2].nameHashCode = 0;
							this.m_xmlAttribute[num2].valueType = TagType.None;
							this.m_xmlAttribute[num2].valueHashCode = 0;
							this.m_xmlAttribute[num2].valueStartIndex = 0;
							this.m_xmlAttribute[num2].valueLength = 0;
							this.m_xmlAttribute[num2].valueDecimalIndex = 0;
						}
					}
					else if (tagType == TagType.StringValue)
					{
						if (chars[num3] != 34)
						{
							this.m_xmlAttribute[num2].valueHashCode = ((this.m_xmlAttribute[num2].valueHashCode << 5) + this.m_xmlAttribute[num2].valueHashCode ^ chars[num3]);
							XML_TagAttribute[] xmlAttribute6 = this.m_xmlAttribute;
							int num9 = num2;
							xmlAttribute6[num9].valueLength = xmlAttribute6[num9].valueLength + 1;
						}
						else
						{
							b = 2;
							tagType = TagType.None;
							num2++;
							this.m_xmlAttribute[num2].nameHashCode = 0;
							this.m_xmlAttribute[num2].valueType = TagType.None;
							this.m_xmlAttribute[num2].valueHashCode = 0;
							this.m_xmlAttribute[num2].valueStartIndex = 0;
							this.m_xmlAttribute[num2].valueLength = 0;
							this.m_xmlAttribute[num2].valueDecimalIndex = 0;
						}
					}
				}
				if (chars[num3] == 61)
				{
					b = 1;
				}
				if (b == 0 && chars[num3] == 32)
				{
					if (flag)
					{
						return false;
					}
					flag = true;
					b = 2;
					tagType = TagType.None;
					num2++;
					this.m_xmlAttribute[num2].nameHashCode = 0;
					this.m_xmlAttribute[num2].valueType = TagType.None;
					this.m_xmlAttribute[num2].valueHashCode = 0;
					this.m_xmlAttribute[num2].valueStartIndex = 0;
					this.m_xmlAttribute[num2].valueLength = 0;
					this.m_xmlAttribute[num2].valueDecimalIndex = 0;
				}
				if (b == 0)
				{
					this.m_xmlAttribute[num2].nameHashCode = (this.m_xmlAttribute[num2].nameHashCode << 3) - this.m_xmlAttribute[num2].nameHashCode + chars[num3];
				}
				if (b == 2 && chars[num3] == 32)
				{
					b = 0;
				}
				num3++;
			}
			if (!flag2)
			{
				return false;
			}
			if (this.tag_NoParsing && this.m_xmlAttribute[0].nameHashCode != 53822163 && this.m_xmlAttribute[0].nameHashCode != 49429939)
			{
				return false;
			}
			if (this.m_xmlAttribute[0].nameHashCode == 53822163 || this.m_xmlAttribute[0].nameHashCode == 49429939)
			{
				this.tag_NoParsing = false;
				return true;
			}
			if (this.m_htmlTag[0] == '#' && num == 7)
			{
				this.m_htmlColor = this.HexCharsToColor(this.m_htmlTag, num);
				this.m_colorStack.Add(this.m_htmlColor);
				return true;
			}
			if (this.m_htmlTag[0] == '#' && num == 9)
			{
				this.m_htmlColor = this.HexCharsToColor(this.m_htmlTag, num);
				this.m_colorStack.Add(this.m_htmlColor);
				return true;
			}
			int nameHashCode = this.m_xmlAttribute[0].nameHashCode;
			switch (nameHashCode)
			{
			case 83:
				break;
			default:
				switch (nameHashCode)
				{
				case 115:
					break;
				default:
					switch (nameHashCode)
					{
					case 412:
						break;
					default:
						if (nameHashCode != 426)
						{
							if (nameHashCode != 427)
							{
								switch (nameHashCode)
								{
								case 444:
									goto IL_E11;
								default:
								{
									float num12;
									if (nameHashCode != 16034505)
									{
										if (nameHashCode != 343615334 && nameHashCode != 374360934)
										{
											if (nameHashCode != 457225591)
											{
												if (nameHashCode != 514803617)
												{
													if (nameHashCode != 551025096)
													{
														if (nameHashCode != 566686826)
														{
															if (nameHashCode == 730022849)
															{
																goto IL_2D17;
															}
															if (nameHashCode == 766244328)
															{
																goto IL_2D5A;
															}
															if (nameHashCode != 781906058)
															{
																if (nameHashCode != 1100728678)
																{
																	if (nameHashCode != 1109349752)
																	{
																		if (nameHashCode != 1109386397)
																		{
																			if (nameHashCode != 1897350193)
																			{
																				if (nameHashCode != 1897386838)
																				{
																					if (nameHashCode != 2012149182)
																					{
																						if (nameHashCode != -1885698441)
																						{
																							if (nameHashCode != -1883544150)
																							{
																								if (nameHashCode != -1847322671)
																								{
																									if (nameHashCode != -1831660941)
																									{
																										if (nameHashCode != -1690034531)
																										{
																											if (nameHashCode == -1668324918)
																											{
																												goto IL_2D27;
																											}
																											if (nameHashCode == -1632103439)
																											{
																												goto IL_2D6B;
																											}
																											if (nameHashCode == -1616441709)
																											{
																												goto IL_2D49;
																											}
																											if (nameHashCode != -884817987)
																											{
																												if (nameHashCode == -855002522)
																												{
																													goto IL_2EBE;
																												}
																												if (nameHashCode == -842693512)
																												{
																													goto IL_30FA;
																												}
																												if (nameHashCode == -842656867)
																												{
																													goto IL_26F8;
																												}
																												if (nameHashCode == -445573839)
																												{
																													goto IL_31D5;
																												}
																												if (nameHashCode == -445537194)
																												{
																													goto IL_27E6;
																												}
																												if (nameHashCode != -330774850)
																												{
																													if (nameHashCode != 66)
																													{
																														if (nameHashCode != 73)
																														{
																															if (nameHashCode == 98)
																															{
																																goto IL_D84;
																															}
																															if (nameHashCode != 105)
																															{
																																if (nameHashCode == 395)
																																{
																																	break;
																																}
																																if (nameHashCode == 402 || nameHashCode == 434)
																																{
																																	this.m_style &= (FontStyles)(-3);
																																	return true;
																																}
																																if (nameHashCode != 4556)
																																{
																																	if (nameHashCode != 4728)
																																	{
																																		if (nameHashCode != 4742)
																																		{
																																			if (nameHashCode == 6380)
																																			{
																																				goto IL_1284;
																																			}
																																			if (nameHashCode == 6552)
																																			{
																																				goto IL_E61;
																																			}
																																			if (nameHashCode != 6566)
																																			{
																																				if (nameHashCode != 20677)
																																				{
																																					if (nameHashCode != 20849)
																																					{
																																						if (nameHashCode != 20863)
																																						{
																																							if (nameHashCode == 22501)
																																							{
																																								goto IL_133E;
																																							}
																																							if (nameHashCode == 22673)
																																							{
																																								goto IL_ED3;
																																							}
																																							if (nameHashCode != 22687)
																																							{
																																								int valueHashCode;
																																								if (nameHashCode != 28511)
																																								{
																																									if (nameHashCode != 30266)
																																									{
																																										if (nameHashCode != 31169)
																																										{
																																											if (nameHashCode != 31191)
																																											{
																																												if (nameHashCode != 32745)
																																												{
																																													if (nameHashCode == 41311)
																																													{
																																														goto IL_1751;
																																													}
																																													if (nameHashCode == 43066)
																																													{
																																														goto IL_1E47;
																																													}
																																													if (nameHashCode == 43969)
																																													{
																																														goto IL_1439;
																																													}
																																													if (nameHashCode == 43991)
																																													{
																																														goto IL_13F8;
																																													}
																																													if (nameHashCode != 45545)
																																													{
																																														if (nameHashCode != 141358)
																																														{
																																															if (nameHashCode != 143113)
																																															{
																																																if (nameHashCode != 144016)
																																																{
																																																	if (nameHashCode != 145592)
																																																	{
																																																		if (nameHashCode == 154158)
																																																		{
																																																			goto IL_1A70;
																																																		}
																																																		if (nameHashCode == 155913)
																																																		{
																																																			goto IL_1F79;
																																																		}
																																																		if (nameHashCode == 156816)
																																																		{
																																																			goto IL_1442;
																																																		}
																																																		if (nameHashCode != 158392)
																																																		{
																																																			if (nameHashCode != 186285)
																																																			{
																																																				if (nameHashCode != 186622)
																																																				{
																																																					if (nameHashCode != 192323)
																																																					{
																																																						if (nameHashCode != 230446)
																																																						{
																																																							TMP_Style style;
																																																							if (nameHashCode != 233057)
																																																							{
																																																								if (nameHashCode != 237918)
																																																								{
																																																									if (nameHashCode == 275917)
																																																									{
																																																										goto IL_1FE0;
																																																									}
																																																									if (nameHashCode == 276254)
																																																									{
																																																										goto IL_1DFC;
																																																									}
																																																									if (nameHashCode == 280416)
																																																									{
																																																										return false;
																																																									}
																																																									if (nameHashCode == 281955)
																																																									{
																																																										goto IL_2215;
																																																									}
																																																									if (nameHashCode == 320078)
																																																									{
																																																										goto IL_1D3D;
																																																									}
																																																									if (nameHashCode == 322689)
																																																									{
																																																										goto IL_2108;
																																																									}
																																																									if (nameHashCode != 327550)
																																																									{
																																																										if (nameHashCode != 976214)
																																																										{
																																																											if (nameHashCode != 982252)
																																																											{
																																																												if (nameHashCode != 1022986)
																																																												{
																																																													if (nameHashCode != 1027847)
																																																													{
																																																														if (nameHashCode == 1065846)
																																																														{
																																																															goto IL_204E;
																																																														}
																																																														if (nameHashCode == 1071884)
																																																														{
																																																															goto IL_25DA;
																																																														}
																																																														if (nameHashCode == 1112618)
																																																														{
																																																															goto IL_2186;
																																																														}
																																																														if (nameHashCode != 1117479)
																																																														{
																																																															if (nameHashCode != 1286342)
																																																															{
																																																																if (nameHashCode != 1356515)
																																																																{
																																																																	if (nameHashCode != 1441524)
																																																																	{
																																																																		if (nameHashCode != 1482398)
																																																																		{
																																																																			if (nameHashCode != 1524585)
																																																																			{
																																																																				if (nameHashCode != 1619421)
																																																																				{
																																																																					if (nameHashCode == 1750458)
																																																																					{
																																																																						return false;
																																																																					}
																																																																					if (nameHashCode == 1913798)
																																																																					{
																																																																						goto IL_31EB;
																																																																					}
																																																																					if (nameHashCode == 1983971)
																																																																					{
																																																																						goto IL_2438;
																																																																					}
																																																																					if (nameHashCode == 2068980)
																																																																					{
																																																																						goto IL_25ED;
																																																																					}
																																																																					if (nameHashCode == 2109854)
																																																																					{
																																																																						goto IL_2D7C;
																																																																					}
																																																																					if (nameHashCode == 2152041)
																																																																					{
																																																																						goto IL_2508;
																																																																					}
																																																																					if (nameHashCode != 2246877)
																																																																					{
																																																																						if (nameHashCode != 6815845)
																																																																						{
																																																																							if (nameHashCode != 6886018)
																																																																							{
																																																																								if (nameHashCode != 6971027)
																																																																								{
																																																																									if (nameHashCode != 7011901)
																																																																									{
																																																																										if (nameHashCode != 7054088)
																																																																										{
																																																																											if (nameHashCode == 7443301)
																																																																											{
																																																																												goto IL_3250;
																																																																											}
																																																																											if (nameHashCode == 7513474)
																																																																											{
																																																																												goto IL_24FB;
																																																																											}
																																																																											if (nameHashCode == 7598483)
																																																																											{
																																																																												goto IL_26E5;
																																																																											}
																																																																											if (nameHashCode == 7639357)
																																																																											{
																																																																												goto IL_2EA6;
																																																																											}
																																																																											if (nameHashCode != 7681544)
																																																																											{
																																																																												if (nameHashCode != 9133802)
																																																																												{
																																																																													if (nameHashCode != 10723418)
																																																																													{
																																																																														if (nameHashCode == 11642281)
																																																																														{
																																																																															goto IL_1347;
																																																																														}
																																																																														if (nameHashCode == 13526026)
																																																																														{
																																																																															goto IL_2D38;
																																																																														}
																																																																														if (nameHashCode != 15115642)
																																																																														{
																																																																															if (nameHashCode != 47840323)
																																																																															{
																																																																																if (nameHashCode != 50348802)
																																																																																{
																																																																																	if (nameHashCode == 52232547)
																																																																																	{
																																																																																		goto IL_2D49;
																																																																																	}
																																																																																	if (nameHashCode != 54741026)
																																																																																	{
																																																																																		if (nameHashCode != 72669687 && nameHashCode != 103415287)
																																																																																		{
																																																																																			return false;
																																																																																		}
																																																																																		valueHashCode = this.m_xmlAttribute[0].valueHashCode;
																																																																																		if (valueHashCode != 764638571 && valueHashCode != 523367755)
																																																																																		{
																																																																																			Material material;
																																																																																			if (MaterialReferenceManager.TryGetMaterial(valueHashCode, out material))
																																																																																			{
																																																																																				if (this.m_currentFontAsset.atlas.GetInstanceID() != material.GetTexture(ShaderUtilities.ID_MainTex).GetInstanceID())
																																																																																				{
																																																																																					return false;
																																																																																				}
																																																																																				this.m_currentMaterial = material;
																																																																																				this.m_currentMaterialIndex = MaterialReference.AddMaterialReference(this.m_currentMaterial, this.m_currentFontAsset, this.m_materialReferences, this.m_materialReferenceIndexLookup);
																																																																																				this.m_materialReferenceStack.Add(this.m_materialReferences[this.m_currentMaterialIndex]);
																																																																																			}
																																																																																			else
																																																																																			{
																																																																																				material = Resources.Load<Material>(TMP_Settings.defaultFontAssetPath + new string(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength));
																																																																																				if (material == null)
																																																																																				{
																																																																																					return false;
																																																																																				}
																																																																																				if (this.m_currentFontAsset.atlas.GetInstanceID() != material.GetTexture(ShaderUtilities.ID_MainTex).GetInstanceID())
																																																																																				{
																																																																																					return false;
																																																																																				}
																																																																																				MaterialReferenceManager.AddFontMaterial(valueHashCode, material);
																																																																																				this.m_currentMaterial = material;
																																																																																				this.m_currentMaterialIndex = MaterialReference.AddMaterialReference(this.m_currentMaterial, this.m_currentFontAsset, this.m_materialReferences, this.m_materialReferenceIndexLookup);
																																																																																				this.m_materialReferenceStack.Add(this.m_materialReferences[this.m_currentMaterialIndex]);
																																																																																			}
																																																																																			return true;
																																																																																		}
																																																																																		if (this.m_currentFontAsset.atlas.GetInstanceID() != this.m_currentMaterial.GetTexture(ShaderUtilities.ID_MainTex).GetInstanceID())
																																																																																		{
																																																																																			return false;
																																																																																		}
																																																																																		this.m_currentMaterial = this.m_materialReferences[0].material;
																																																																																		this.m_currentMaterialIndex = 0;
																																																																																		this.m_materialReferenceStack.Add(this.m_materialReferences[0]);
																																																																																		return true;
																																																																																	}
																																																																																}
																																																																																this.m_baselineOffset = 0f;
																																																																																return true;
																																																																															}
																																																																															goto IL_2D49;
																																																																														}
																																																																													}
																																																																													this.tag_NoParsing = true;
																																																																													return true;
																																																																												}
																																																																												goto IL_2D38;
																																																																											}
																																																																										}
																																																																										this.m_monoSpacing = 0f;
																																																																										return true;
																																																																									}
																																																																									IL_2EA6:
																																																																									this.m_marginLeft = 0f;
																																																																									this.m_marginRight = 0f;
																																																																									return true;
																																																																								}
																																																																								IL_26E5:
																																																																								this.tag_Indent = this.m_indentStack.Remove();
																																																																								return true;
																																																																							}
																																																																							IL_24FB:
																																																																							this.m_cSpacing = 0f;
																																																																							return true;
																																																																						}
																																																																						IL_3250:
																																																																						if (this.m_isParsingText)
																																																																						{
																																																																							UnityEngine.Debug.Log(string.Concat(new object[]
																																																																							{
																																																																								"Action ID: [",
																																																																								this.m_actionStack.CurrentItem(),
																																																																								"] Last character index: ",
																																																																								this.m_characterCount - 1
																																																																							}));
																																																																						}
																																																																						this.m_actionStack.Remove();
																																																																						return true;
																																																																					}
																																																																				}
																																																																				int valueHashCode2 = this.m_xmlAttribute[0].valueHashCode;
																																																																				TMP_SpriteAsset tmp_SpriteAsset;
																																																																				if (this.m_xmlAttribute[0].valueType == TagType.None || this.m_xmlAttribute[0].valueType == TagType.NumericalValue)
																																																																				{
																																																																					if (this.m_defaultSpriteAsset == null)
																																																																					{
																																																																						if (TMP_Settings.defaultSpriteAsset != null)
																																																																						{
																																																																							this.m_defaultSpriteAsset = TMP_Settings.defaultSpriteAsset;
																																																																						}
																																																																						else
																																																																						{
																																																																							this.m_defaultSpriteAsset = Resources.Load<TMP_SpriteAsset>("Sprite Assets/Default Sprite Asset");
																																																																						}
																																																																					}
																																																																					this.m_currentSpriteAsset = this.m_defaultSpriteAsset;
																																																																					if (this.m_currentSpriteAsset == null)
																																																																					{
																																																																						return false;
																																																																					}
																																																																				}
																																																																				else if (MaterialReferenceManager.TryGetSpriteAsset(valueHashCode2, out tmp_SpriteAsset))
																																																																				{
																																																																					this.m_currentSpriteAsset = tmp_SpriteAsset;
																																																																				}
																																																																				else
																																																																				{
																																																																					if (tmp_SpriteAsset == null)
																																																																					{
																																																																						tmp_SpriteAsset = Resources.Load<TMP_SpriteAsset>(TMP_Settings.defaultSpriteAssetPath + new string(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength));
																																																																					}
																																																																					if (tmp_SpriteAsset == null)
																																																																					{
																																																																						return false;
																																																																					}
																																																																					MaterialReferenceManager.AddSpriteAsset(valueHashCode2, tmp_SpriteAsset);
																																																																					this.m_currentSpriteAsset = tmp_SpriteAsset;
																																																																				}
																																																																				if (this.m_xmlAttribute[0].valueType == TagType.NumericalValue)
																																																																				{
																																																																					int num10 = (int)this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength, this.m_xmlAttribute[0].valueDecimalIndex);
																																																																					if (num10 == -9999)
																																																																					{
																																																																						return false;
																																																																					}
																																																																					if (num10 > this.m_currentSpriteAsset.spriteInfoList.Count - 1)
																																																																					{
																																																																						return false;
																																																																					}
																																																																					this.m_spriteIndex = num10;
																																																																				}
																																																																				else if (this.m_xmlAttribute[1].nameHashCode == 43347 || this.m_xmlAttribute[1].nameHashCode == 30547)
																																																																				{
																																																																					int spriteIndex = this.m_currentSpriteAsset.GetSpriteIndex(this.m_xmlAttribute[1].valueHashCode);
																																																																					if (spriteIndex == -1)
																																																																					{
																																																																						return false;
																																																																					}
																																																																					this.m_spriteIndex = spriteIndex;
																																																																				}
																																																																				else
																																																																				{
																																																																					if (this.m_xmlAttribute[1].nameHashCode != 295562 && this.m_xmlAttribute[1].nameHashCode != 205930)
																																																																					{
																																																																						return false;
																																																																					}
																																																																					int num11 = (int)this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[1].valueStartIndex, this.m_xmlAttribute[1].valueLength, this.m_xmlAttribute[1].valueDecimalIndex);
																																																																					if (num11 == -9999)
																																																																					{
																																																																						return false;
																																																																					}
																																																																					if (num11 > this.m_currentSpriteAsset.spriteInfoList.Count - 1)
																																																																					{
																																																																						return false;
																																																																					}
																																																																					this.m_spriteIndex = num11;
																																																																				}
																																																																				this.m_currentMaterialIndex = MaterialReference.AddMaterialReference(this.m_currentSpriteAsset.material, this.m_currentSpriteAsset, this.m_materialReferences, this.m_materialReferenceIndexLookup);
																																																																				this.m_spriteColor = TMP_Text.s_colorWhite;
																																																																				this.m_tintSprite = false;
																																																																				if (this.m_xmlAttribute[1].nameHashCode == 45819 || this.m_xmlAttribute[1].nameHashCode == 33019)
																																																																				{
																																																																					this.m_tintSprite = (this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[1].valueStartIndex, this.m_xmlAttribute[1].valueLength, this.m_xmlAttribute[1].valueDecimalIndex) != 0f);
																																																																				}
																																																																				else if (this.m_xmlAttribute[2].nameHashCode == 45819 || this.m_xmlAttribute[2].nameHashCode == 33019)
																																																																				{
																																																																					this.m_tintSprite = (this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[2].valueStartIndex, this.m_xmlAttribute[2].valueLength, this.m_xmlAttribute[2].valueDecimalIndex) != 0f);
																																																																				}
																																																																				if (this.m_xmlAttribute[1].nameHashCode == 281955 || this.m_xmlAttribute[1].nameHashCode == 192323)
																																																																				{
																																																																					this.m_spriteColor = this.HexCharsToColor(this.m_htmlTag, this.m_xmlAttribute[1].valueStartIndex, this.m_xmlAttribute[1].valueLength);
																																																																				}
																																																																				else if (this.m_xmlAttribute[2].nameHashCode == 281955 || this.m_xmlAttribute[2].nameHashCode == 192323)
																																																																				{
																																																																					this.m_spriteColor = this.HexCharsToColor(this.m_htmlTag, this.m_xmlAttribute[2].valueStartIndex, this.m_xmlAttribute[2].valueLength);
																																																																				}
																																																																				this.m_xmlAttribute[1].nameHashCode = 0;
																																																																				this.m_xmlAttribute[2].nameHashCode = 0;
																																																																				this.m_textElementType = TMP_TextElementType.Sprite;
																																																																				return true;
																																																																			}
																																																																			IL_2508:
																																																																			num12 = this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength, this.m_xmlAttribute[0].valueDecimalIndex);
																																																																			if (num12 == -9999f || num12 == 0f)
																																																																			{
																																																																				return false;
																																																																			}
																																																																			if (tagUnits != TagUnits.Pixels)
																																																																			{
																																																																				if (tagUnits != TagUnits.FontUnits)
																																																																				{
																																																																					if (tagUnits == TagUnits.Percentage)
																																																																					{
																																																																						return false;
																																																																					}
																																																																				}
																																																																				else
																																																																				{
																																																																					this.m_monoSpacing = num12;
																																																																					this.m_monoSpacing *= this.m_fontScale * this.m_fontAsset.fontInfo.TabWidth / (float)this.m_fontAsset.tabSize;
																																																																				}
																																																																			}
																																																																			else
																																																																			{
																																																																				this.m_monoSpacing = num12;
																																																																			}
																																																																			return true;
																																																																		}
																																																																		IL_2D7C:
																																																																		num12 = this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength, this.m_xmlAttribute[0].valueDecimalIndex);
																																																																		if (num12 == -9999f || num12 == 0f)
																																																																		{
																																																																			return false;
																																																																		}
																																																																		this.m_marginLeft = num12;
																																																																		if (tagUnits != TagUnits.Pixels)
																																																																		{
																																																																			if (tagUnits != TagUnits.FontUnits)
																																																																			{
																																																																				if (tagUnits == TagUnits.Percentage)
																																																																				{
																																																																					this.m_marginLeft = (this.m_marginWidth - ((this.m_width == -1f) ? 0f : this.m_width)) * this.m_marginLeft / 100f;
																																																																				}
																																																																			}
																																																																			else
																																																																			{
																																																																				this.m_marginLeft *= this.m_fontScale * this.m_fontAsset.fontInfo.TabWidth / (float)this.m_fontAsset.tabSize;
																																																																			}
																																																																		}
																																																																		this.m_marginLeft = ((this.m_marginLeft < 0f) ? 0f : this.m_marginLeft);
																																																																		this.m_marginRight = this.m_marginLeft;
																																																																		return true;
																																																																	}
																																																																	IL_25ED:
																																																																	num12 = this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength, this.m_xmlAttribute[0].valueDecimalIndex);
																																																																	if (num12 == -9999f || num12 == 0f)
																																																																	{
																																																																		return false;
																																																																	}
																																																																	if (tagUnits != TagUnits.Pixels)
																																																																	{
																																																																		if (tagUnits != TagUnits.FontUnits)
																																																																		{
																																																																			if (tagUnits == TagUnits.Percentage)
																																																																			{
																																																																				this.tag_Indent = this.m_marginWidth * num12 / 100f;
																																																																			}
																																																																		}
																																																																		else
																																																																		{
																																																																			this.tag_Indent = num12;
																																																																			this.tag_Indent *= this.m_fontScale * this.m_fontAsset.fontInfo.TabWidth / (float)this.m_fontAsset.tabSize;
																																																																		}
																																																																	}
																																																																	else
																																																																	{
																																																																		this.tag_Indent = num12;
																																																																	}
																																																																	this.m_indentStack.Add(this.tag_Indent);
																																																																	this.m_xAdvance = this.tag_Indent;
																																																																	return true;
																																																																}
																																																																IL_2438:
																																																																num12 = this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength, this.m_xmlAttribute[0].valueDecimalIndex);
																																																																if (num12 == -9999f || num12 == 0f)
																																																																{
																																																																	return false;
																																																																}
																																																																if (tagUnits != TagUnits.Pixels)
																																																																{
																																																																	if (tagUnits != TagUnits.FontUnits)
																																																																	{
																																																																		if (tagUnits == TagUnits.Percentage)
																																																																		{
																																																																			return false;
																																																																		}
																																																																	}
																																																																	else
																																																																	{
																																																																		this.m_cSpacing = num12;
																																																																		this.m_cSpacing *= this.m_fontScale * this.m_fontAsset.fontInfo.TabWidth / (float)this.m_fontAsset.tabSize;
																																																																	}
																																																																}
																																																																else
																																																																{
																																																																	this.m_cSpacing = num12;
																																																																}
																																																																return true;
																																																															}
																																																															IL_31EB:
																																																															int valueHashCode3 = this.m_xmlAttribute[0].valueHashCode;
																																																															if (this.m_isParsingText)
																																																															{
																																																																this.m_actionStack.Add(valueHashCode3);
																																																																UnityEngine.Debug.Log(string.Concat(new object[]
																																																																{
																																																																	"Action ID: [",
																																																																	valueHashCode3,
																																																																	"] First character index: ",
																																																																	this.m_characterCount
																																																																}));
																																																															}
																																																															return true;
																																																														}
																																																													}
																																																													this.m_width = -1f;
																																																													return true;
																																																												}
																																																												IL_2186:
																																																												style = TMP_StyleSheet.GetStyle(this.m_xmlAttribute[0].valueHashCode);
																																																												if (style == null)
																																																												{
																																																													int hashCode = this.m_styleStack.CurrentItem();
																																																													style = TMP_StyleSheet.GetStyle(hashCode);
																																																													this.m_styleStack.Remove();
																																																												}
																																																												if (style == null)
																																																												{
																																																													return false;
																																																												}
																																																												for (int i = 0; i < style.styleClosingTagArray.Length; i++)
																																																												{
																																																													if (style.styleClosingTagArray[i] == 60)
																																																													{
																																																														this.ValidateHtmlTag(style.styleClosingTagArray, i + 1, out i);
																																																													}
																																																												}
																																																												return true;
																																																											}
																																																											IL_25DA:
																																																											this.m_htmlColor = this.m_colorStack.Remove();
																																																											return true;
																																																										}
																																																										IL_204E:
																																																										this.m_lineJustification = this.m_textAlignment;
																																																										return true;
																																																									}
																																																								}
																																																								num12 = this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength, this.m_xmlAttribute[0].valueDecimalIndex);
																																																								if (num12 == -9999f || num12 == 0f)
																																																								{
																																																									return false;
																																																								}
																																																								if (tagUnits != TagUnits.Pixels)
																																																								{
																																																									if (tagUnits == TagUnits.FontUnits)
																																																									{
																																																										return false;
																																																									}
																																																									if (tagUnits == TagUnits.Percentage)
																																																									{
																																																										this.m_width = this.m_marginWidth * num12 / 100f;
																																																									}
																																																								}
																																																								else
																																																								{
																																																									this.m_width = num12;
																																																								}
																																																								return true;
																																																							}
																																																							IL_2108:
																																																							style = TMP_StyleSheet.GetStyle(this.m_xmlAttribute[0].valueHashCode);
																																																							if (style == null)
																																																							{
																																																								return false;
																																																							}
																																																							this.m_styleStack.Add(style.hashCode);
																																																							for (int j = 0; j < style.styleOpeningTagArray.Length; j++)
																																																							{
																																																								if (style.styleOpeningTagArray[j] == 60 && !this.ValidateHtmlTag(style.styleOpeningTagArray, j + 1, out j))
																																																								{
																																																									return false;
																																																								}
																																																							}
																																																							return true;
																																																						}
																																																						IL_1D3D:
																																																						num12 = this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength, this.m_xmlAttribute[0].valueDecimalIndex);
																																																						if (num12 == -9999f || num12 == 0f)
																																																						{
																																																							return false;
																																																						}
																																																						if (tagUnits == TagUnits.Pixels)
																																																						{
																																																							this.m_xAdvance += num12;
																																																							return true;
																																																						}
																																																						if (tagUnits != TagUnits.FontUnits)
																																																						{
																																																							return tagUnits != TagUnits.Percentage && false;
																																																						}
																																																						this.m_xAdvance += num12 * this.m_fontScale * this.m_fontAsset.fontInfo.TabWidth / (float)this.m_fontAsset.tabSize;
																																																						return true;
																																																					}
																																																					IL_2215:
																																																					if (this.m_htmlTag[6] == '#' && num == 13)
																																																					{
																																																						this.m_htmlColor = this.HexCharsToColor(this.m_htmlTag, num);
																																																						this.m_colorStack.Add(this.m_htmlColor);
																																																						return true;
																																																					}
																																																					if (this.m_htmlTag[6] == '#' && num == 15)
																																																					{
																																																						this.m_htmlColor = this.HexCharsToColor(this.m_htmlTag, num);
																																																						this.m_colorStack.Add(this.m_htmlColor);
																																																						return true;
																																																					}
																																																					int valueHashCode4 = this.m_xmlAttribute[0].valueHashCode;
																																																					if (valueHashCode4 == -36881330)
																																																					{
																																																						this.m_htmlColor = new Color32(160, 32, 240, byte.MaxValue);
																																																						this.m_colorStack.Add(this.m_htmlColor);
																																																						return true;
																																																					}
																																																					if (valueHashCode4 == 125395)
																																																					{
																																																						this.m_htmlColor = Color.red;
																																																						this.m_colorStack.Add(this.m_htmlColor);
																																																						return true;
																																																					}
																																																					if (valueHashCode4 == 3573310)
																																																					{
																																																						this.m_htmlColor = Color.blue;
																																																						this.m_colorStack.Add(this.m_htmlColor);
																																																						return true;
																																																					}
																																																					if (valueHashCode4 == 26556144)
																																																					{
																																																						this.m_htmlColor = new Color32(byte.MaxValue, 128, 0, byte.MaxValue);
																																																						this.m_colorStack.Add(this.m_htmlColor);
																																																						return true;
																																																					}
																																																					if (valueHashCode4 == 117905991)
																																																					{
																																																						this.m_htmlColor = Color.black;
																																																						this.m_colorStack.Add(this.m_htmlColor);
																																																						return true;
																																																					}
																																																					if (valueHashCode4 == 121463835)
																																																					{
																																																						this.m_htmlColor = Color.green;
																																																						this.m_colorStack.Add(this.m_htmlColor);
																																																						return true;
																																																					}
																																																					if (valueHashCode4 == 140357351)
																																																					{
																																																						this.m_htmlColor = Color.white;
																																																						this.m_colorStack.Add(this.m_htmlColor);
																																																						return true;
																																																					}
																																																					if (valueHashCode4 != 554054276)
																																																					{
																																																						return false;
																																																					}
																																																					this.m_htmlColor = Color.yellow;
																																																					this.m_colorStack.Add(this.m_htmlColor);
																																																					return true;
																																																				}
																																																				IL_1DFC:
																																																				if (this.m_xmlAttribute[0].valueLength != 3)
																																																				{
																																																					return false;
																																																				}
																																																				this.m_htmlColor.a = (byte)(this.HexToInt(this.m_htmlTag[7]) * 16 + this.HexToInt(this.m_htmlTag[8]));
																																																				return true;
																																																			}
																																																			IL_1FE0:
																																																			int valueHashCode5 = this.m_xmlAttribute[0].valueHashCode;
																																																			if (valueHashCode5 == -523808257)
																																																			{
																																																				this.m_lineJustification = TextAlignmentOptions.Justified;
																																																				return true;
																																																			}
																																																			if (valueHashCode5 == -458210101)
																																																			{
																																																				this.m_lineJustification = TextAlignmentOptions.Center;
																																																				return true;
																																																			}
																																																			if (valueHashCode5 == 3774683)
																																																			{
																																																				this.m_lineJustification = TextAlignmentOptions.Left;
																																																				return true;
																																																			}
																																																			if (valueHashCode5 != 136703040)
																																																			{
																																																				return false;
																																																			}
																																																			this.m_lineJustification = TextAlignmentOptions.Right;
																																																			return true;
																																																		}
																																																	}
																																																	this.m_currentFontSize = this.m_sizeStack.Remove();
																																																	this.m_fontScale = this.m_currentFontSize / this.m_currentFontAsset.fontInfo.PointSize * this.m_currentFontAsset.fontInfo.Scale * ((!this.m_isOrthographic) ? 0.1f : 1f);
																																																	return true;
																																																}
																																																IL_1442:
																																																this.m_isNonBreakingSpace = false;
																																																return true;
																																															}
																																															IL_1F79:
																																															if (this.m_isParsingText)
																																															{
																																																this.m_textInfo.linkInfo[this.m_textInfo.linkCount].linkTextLength = this.m_characterCount - this.m_textInfo.linkInfo[this.m_textInfo.linkCount].linkTextfirstCharacterIndex;
																																																this.m_textInfo.linkCount++;
																																															}
																																															return true;
																																														}
																																														IL_1A70:
																																														MaterialReference materialReference = this.m_materialReferenceStack.Remove();
																																														this.m_currentFontAsset = materialReference.fontAsset;
																																														this.m_currentMaterial = materialReference.material;
																																														this.m_currentMaterialIndex = materialReference.index;
																																														this.m_fontScale = this.m_currentFontSize / this.m_currentFontAsset.fontInfo.PointSize * this.m_currentFontAsset.fontInfo.Scale * ((!this.m_isOrthographic) ? 0.1f : 1f);
																																														return true;
																																													}
																																												}
																																												num12 = this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength, this.m_xmlAttribute[0].valueDecimalIndex);
																																												if (num12 == -9999f || num12 == 0f)
																																												{
																																													return false;
																																												}
																																												if (tagUnits != TagUnits.Pixels)
																																												{
																																													if (tagUnits == TagUnits.FontUnits)
																																													{
																																														this.m_currentFontSize = this.m_fontSize * num12;
																																														this.m_sizeStack.Add(this.m_currentFontSize);
																																														this.m_fontScale = this.m_currentFontSize / this.m_currentFontAsset.fontInfo.PointSize * this.m_currentFontAsset.fontInfo.Scale * ((!this.m_isOrthographic) ? 0.1f : 1f);
																																														return true;
																																													}
																																													if (tagUnits != TagUnits.Percentage)
																																													{
																																														return false;
																																													}
																																													this.m_currentFontSize = this.m_fontSize * num12 / 100f;
																																													this.m_sizeStack.Add(this.m_currentFontSize);
																																													this.m_fontScale = this.m_currentFontSize / this.m_currentFontAsset.fontInfo.PointSize * this.m_currentFontAsset.fontInfo.Scale * ((!this.m_isOrthographic) ? 0.1f : 1f);
																																													return true;
																																												}
																																												else
																																												{
																																													if (this.m_htmlTag[5] == '+')
																																													{
																																														this.m_currentFontSize = this.m_fontSize + num12;
																																														this.m_sizeStack.Add(this.m_currentFontSize);
																																														this.m_fontScale = this.m_currentFontSize / this.m_currentFontAsset.fontInfo.PointSize * this.m_currentFontAsset.fontInfo.Scale * ((!this.m_isOrthographic) ? 0.1f : 1f);
																																														return true;
																																													}
																																													if (this.m_htmlTag[5] == '-')
																																													{
																																														this.m_currentFontSize = this.m_fontSize + num12;
																																														this.m_sizeStack.Add(this.m_currentFontSize);
																																														this.m_fontScale = this.m_currentFontSize / this.m_currentFontAsset.fontInfo.PointSize * this.m_currentFontAsset.fontInfo.Scale * ((!this.m_isOrthographic) ? 0.1f : 1f);
																																														return true;
																																													}
																																													this.m_currentFontSize = num12;
																																													this.m_sizeStack.Add(this.m_currentFontSize);
																																													this.m_fontScale = this.m_currentFontSize / this.m_currentFontAsset.fontInfo.PointSize * this.m_currentFontAsset.fontInfo.Scale * ((!this.m_isOrthographic) ? 0.1f : 1f);
																																													return true;
																																												}
																																											}
																																											IL_13F8:
																																											if (this.m_overflowMode == TextOverflowModes.Page)
																																											{
																																												this.m_xAdvance = this.tag_LineIndent + this.tag_Indent;
																																												this.m_lineOffset = 0f;
																																												this.m_pageNumber++;
																																												this.m_isNewPage = true;
																																											}
																																											return true;
																																										}
																																										IL_1439:
																																										this.m_isNonBreakingSpace = true;
																																										return true;
																																									}
																																									IL_1E47:
																																									if (this.m_isParsingText)
																																									{
																																										int linkCount = this.m_textInfo.linkCount;
																																										if (linkCount + 1 > this.m_textInfo.linkInfo.Length)
																																										{
																																											TMP_TextInfo.Resize<TMP_LinkInfo>(ref this.m_textInfo.linkInfo, linkCount + 1);
																																										}
																																										this.m_textInfo.linkInfo[linkCount].textComponent = this;
																																										this.m_textInfo.linkInfo[linkCount].hashCode = this.m_xmlAttribute[0].valueHashCode;
																																										this.m_textInfo.linkInfo[linkCount].linkTextfirstCharacterIndex = this.m_characterCount;
																																										this.m_textInfo.linkInfo[linkCount].linkIdFirstCharacterIndex = startIndex + this.m_xmlAttribute[0].valueStartIndex;
																																										this.m_textInfo.linkInfo[linkCount].linkIdLength = this.m_xmlAttribute[0].valueLength;
																																										this.m_textInfo.linkInfo[linkCount].SetLinkID(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength);
																																									}
																																									return true;
																																								}
																																								IL_1751:
																																								int valueHashCode6 = this.m_xmlAttribute[0].valueHashCode;
																																								int nameHashCode2 = this.m_xmlAttribute[1].nameHashCode;
																																								valueHashCode = this.m_xmlAttribute[1].valueHashCode;
																																								if (valueHashCode6 == 764638571 || valueHashCode6 == 523367755)
																																								{
																																									this.m_currentFontAsset = this.m_materialReferences[0].fontAsset;
																																									this.m_currentMaterial = this.m_materialReferences[0].material;
																																									this.m_currentMaterialIndex = 0;
																																									this.m_fontScale = this.m_currentFontSize / this.m_currentFontAsset.fontInfo.PointSize * this.m_currentFontAsset.fontInfo.Scale * ((!this.m_isOrthographic) ? 0.1f : 1f);
																																									this.m_materialReferenceStack.Add(this.m_materialReferences[0]);
																																									return true;
																																								}
																																								TMP_FontAsset tmp_FontAsset;
																																								if (!MaterialReferenceManager.TryGetFontAsset(valueHashCode6, out tmp_FontAsset))
																																								{
																																									tmp_FontAsset = Resources.Load<TMP_FontAsset>(TMP_Settings.defaultFontAssetPath + new string(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength));
																																									if (tmp_FontAsset == null)
																																									{
																																										return false;
																																									}
																																									MaterialReferenceManager.AddFontAsset(tmp_FontAsset);
																																								}
																																								if (nameHashCode2 == 0 && valueHashCode == 0)
																																								{
																																									this.m_currentMaterial = tmp_FontAsset.material;
																																									this.m_currentMaterialIndex = MaterialReference.AddMaterialReference(this.m_currentMaterial, tmp_FontAsset, this.m_materialReferences, this.m_materialReferenceIndexLookup);
																																									this.m_materialReferenceStack.Add(this.m_materialReferences[this.m_currentMaterialIndex]);
																																								}
																																								else
																																								{
																																									if (nameHashCode2 != 103415287 && nameHashCode2 != 72669687)
																																									{
																																										return false;
																																									}
																																									Material material;
																																									if (MaterialReferenceManager.TryGetMaterial(valueHashCode, out material))
																																									{
																																										this.m_currentMaterial = material;
																																										this.m_currentMaterialIndex = MaterialReference.AddMaterialReference(this.m_currentMaterial, tmp_FontAsset, this.m_materialReferences, this.m_materialReferenceIndexLookup);
																																										this.m_materialReferenceStack.Add(this.m_materialReferences[this.m_currentMaterialIndex]);
																																									}
																																									else
																																									{
																																										material = Resources.Load<Material>(TMP_Settings.defaultFontAssetPath + new string(this.m_htmlTag, this.m_xmlAttribute[1].valueStartIndex, this.m_xmlAttribute[1].valueLength));
																																										if (material == null)
																																										{
																																											return false;
																																										}
																																										MaterialReferenceManager.AddFontMaterial(valueHashCode, material);
																																										this.m_currentMaterial = material;
																																										this.m_currentMaterialIndex = MaterialReference.AddMaterialReference(this.m_currentMaterial, tmp_FontAsset, this.m_materialReferences, this.m_materialReferenceIndexLookup);
																																										this.m_materialReferenceStack.Add(this.m_materialReferences[this.m_currentMaterialIndex]);
																																									}
																																								}
																																								this.m_currentFontAsset = tmp_FontAsset;
																																								this.m_fontScale = this.m_currentFontSize / this.m_currentFontAsset.fontInfo.PointSize * this.m_currentFontAsset.fontInfo.Scale * ((!this.m_isOrthographic) ? 0.1f : 1f);
																																								return true;
																																							}
																																						}
																																						if ((this.m_style & FontStyles.Superscript) == FontStyles.Superscript)
																																						{
																																							if ((this.m_style & FontStyles.Subscript) == FontStyles.Subscript)
																																							{
																																								this.m_fontScaleMultiplier = ((this.m_currentFontAsset.fontInfo.SubSize <= 0f) ? 1f : this.m_currentFontAsset.fontInfo.SubSize);
																																								this.m_baselineOffset = this.m_currentFontAsset.fontInfo.SubscriptOffset * this.m_fontScale * this.m_fontScaleMultiplier;
																																							}
																																							else
																																							{
																																								this.m_baselineOffset = 0f;
																																								this.m_fontScaleMultiplier = 1f;
																																							}
																																							this.m_style &= (FontStyles)(-129);
																																						}
																																						return true;
																																					}
																																					IL_ED3:
																																					if ((this.m_style & FontStyles.Subscript) == FontStyles.Subscript)
																																					{
																																						if ((this.m_style & FontStyles.Superscript) == FontStyles.Superscript)
																																						{
																																							this.m_fontScaleMultiplier = ((this.m_currentFontAsset.fontInfo.SubSize <= 0f) ? 1f : this.m_currentFontAsset.fontInfo.SubSize);
																																							this.m_baselineOffset = this.m_currentFontAsset.fontInfo.SuperscriptOffset * this.m_fontScale * this.m_fontScaleMultiplier;
																																						}
																																						else
																																						{
																																							this.m_baselineOffset = 0f;
																																							this.m_fontScaleMultiplier = 1f;
																																						}
																																						this.m_style &= (FontStyles)(-257);
																																					}
																																					return true;
																																				}
																																				IL_133E:
																																				this.m_isIgnoringAlignment = false;
																																				return true;
																																			}
																																		}
																																		this.m_fontScaleMultiplier = ((this.m_currentFontAsset.fontInfo.SubSize <= 0f) ? 1f : this.m_currentFontAsset.fontInfo.SubSize);
																																		this.m_baselineOffset = this.m_currentFontAsset.fontInfo.SuperscriptOffset * this.m_fontScale * this.m_fontScaleMultiplier;
																																		this.m_style |= FontStyles.Superscript;
																																		return true;
																																	}
																																	IL_E61:
																																	this.m_fontScaleMultiplier = ((this.m_currentFontAsset.fontInfo.SubSize <= 0f) ? 1f : this.m_currentFontAsset.fontInfo.SubSize);
																																	this.m_baselineOffset = this.m_currentFontAsset.fontInfo.SubscriptOffset * this.m_fontScale * this.m_fontScaleMultiplier;
																																	this.m_style |= FontStyles.Subscript;
																																	return true;
																																}
																																IL_1284:
																																num12 = this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength, this.m_xmlAttribute[0].valueDecimalIndex);
																																if (num12 == -9999f)
																																{
																																	return false;
																																}
																																if (tagUnits == TagUnits.Pixels)
																																{
																																	this.m_xAdvance = num12;
																																	return true;
																																}
																																if (tagUnits == TagUnits.FontUnits)
																																{
																																	this.m_xAdvance = num12 * this.m_fontScale * this.m_fontAsset.fontInfo.TabWidth / (float)this.m_fontAsset.tabSize;
																																	return true;
																																}
																																if (tagUnits != TagUnits.Percentage)
																																{
																																	return false;
																																}
																																this.m_xAdvance = this.m_marginWidth * num12 / 100f;
																																return true;
																															}
																														}
																														this.m_style |= FontStyles.Italic;
																														return true;
																													}
																													IL_D84:
																													this.m_style |= FontStyles.Bold;
																													this.m_fontWeightInternal = 700;
																													this.m_fontWeightStack.Add(700);
																													return true;
																												}
																												goto IL_10B7;
																											}
																										}
																										num12 = this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength, this.m_xmlAttribute[0].valueDecimalIndex);
																										if (num12 == -9999f || num12 == 0f)
																										{
																											return false;
																										}
																										this.m_marginRight = num12;
																										if (tagUnits != TagUnits.Pixels)
																										{
																											if (tagUnits != TagUnits.FontUnits)
																											{
																												if (tagUnits == TagUnits.Percentage)
																												{
																													this.m_marginRight = (this.m_marginWidth - ((this.m_width == -1f) ? 0f : this.m_width)) * this.m_marginRight / 100f;
																												}
																											}
																											else
																											{
																												this.m_marginRight *= this.m_fontScale * this.m_fontAsset.fontInfo.TabWidth / (float)this.m_fontAsset.tabSize;
																											}
																										}
																										this.m_marginRight = ((this.m_marginRight < 0f) ? 0f : this.m_marginRight);
																										return true;
																									}
																									IL_2D49:
																									this.m_style &= (FontStyles)(-17);
																									return true;
																								}
																								IL_2D6B:
																								this.m_style &= (FontStyles)(-33);
																								return true;
																							}
																							IL_2D27:
																							this.m_style &= (FontStyles)(-9);
																							return true;
																						}
																						goto IL_1252;
																					}
																					IL_10B7:
																					num12 = this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength, this.m_xmlAttribute[0].valueDecimalIndex);
																					if (num12 == -9999f || num12 == 0f)
																					{
																						return false;
																					}
																					if ((this.m_fontStyle & FontStyles.Bold) == FontStyles.Bold)
																					{
																						return true;
																					}
																					this.m_style &= (FontStyles)(-2);
																					int num13 = (int)num12;
																					if (num13 != 100)
																					{
																						if (num13 != 200)
																						{
																							if (num13 != 300)
																							{
																								if (num13 != 400)
																								{
																									if (num13 != 500)
																									{
																										if (num13 != 600)
																										{
																											if (num13 != 700)
																											{
																												if (num13 != 800)
																												{
																													if (num13 == 900)
																													{
																														this.m_fontWeightInternal = 900;
																													}
																												}
																												else
																												{
																													this.m_fontWeightInternal = 800;
																												}
																											}
																											else
																											{
																												this.m_fontWeightInternal = 700;
																												this.m_style |= FontStyles.Bold;
																											}
																										}
																										else
																										{
																											this.m_fontWeightInternal = 600;
																										}
																									}
																									else
																									{
																										this.m_fontWeightInternal = 500;
																									}
																								}
																								else
																								{
																									this.m_fontWeightInternal = 400;
																								}
																							}
																							else
																							{
																								this.m_fontWeightInternal = 300;
																							}
																						}
																						else
																						{
																							this.m_fontWeightInternal = 200;
																						}
																					}
																					else
																					{
																						this.m_fontWeightInternal = 100;
																					}
																					this.m_fontWeightStack.Add(this.m_fontWeightInternal);
																					return true;
																				}
																				IL_27E6:
																				this.tag_LineIndent = 0f;
																				return true;
																			}
																			IL_31D5:
																			this.m_lineHeight = 0f;
																			return true;
																		}
																		IL_26F8:
																		num12 = this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength, this.m_xmlAttribute[0].valueDecimalIndex);
																		if (num12 == -9999f || num12 == 0f)
																		{
																			return false;
																		}
																		if (tagUnits != TagUnits.Pixels)
																		{
																			if (tagUnits != TagUnits.FontUnits)
																			{
																				if (tagUnits == TagUnits.Percentage)
																				{
																					this.tag_LineIndent = this.m_marginWidth * num12 / 100f;
																				}
																			}
																			else
																			{
																				this.tag_LineIndent = num12;
																				this.tag_LineIndent *= this.m_fontScale * this.m_fontAsset.fontInfo.TabWidth / (float)this.m_fontAsset.tabSize;
																			}
																		}
																		else
																		{
																			this.tag_LineIndent = num12;
																		}
																		this.m_xAdvance += this.tag_LineIndent;
																		return true;
																	}
																	IL_30FA:
																	num12 = this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength, this.m_xmlAttribute[0].valueDecimalIndex);
																	if (num12 == -9999f || num12 == 0f)
																	{
																		return false;
																	}
																	this.m_lineHeight = num12;
																	if (tagUnits != TagUnits.Pixels)
																	{
																		if (tagUnits != TagUnits.FontUnits)
																		{
																			if (tagUnits == TagUnits.Percentage)
																			{
																				this.m_lineHeight = this.m_fontAsset.fontInfo.LineHeight * this.m_lineHeight / 100f * this.m_fontScale;
																			}
																		}
																		else
																		{
																			this.m_lineHeight *= this.m_fontAsset.fontInfo.LineHeight * this.m_fontScale;
																		}
																	}
																	return true;
																}
																IL_2EBE:
																num12 = this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength, this.m_xmlAttribute[0].valueDecimalIndex);
																if (num12 == -9999f || num12 == 0f)
																{
																	return false;
																}
																this.m_marginLeft = num12;
																if (tagUnits != TagUnits.Pixels)
																{
																	if (tagUnits != TagUnits.FontUnits)
																	{
																		if (tagUnits == TagUnits.Percentage)
																		{
																			this.m_marginLeft = (this.m_marginWidth - ((this.m_width == -1f) ? 0f : this.m_width)) * this.m_marginLeft / 100f;
																		}
																	}
																	else
																	{
																		this.m_marginLeft *= this.m_fontScale * this.m_fontAsset.fontInfo.TabWidth / (float)this.m_fontAsset.tabSize;
																	}
																}
																this.m_marginLeft = ((this.m_marginLeft < 0f) ? 0f : this.m_marginLeft);
																return true;
															}
														}
														IL_2D38:
														this.m_style |= FontStyles.UpperCase;
														return true;
													}
													IL_2D5A:
													this.m_style |= FontStyles.SmallCaps;
													return true;
												}
												IL_2D17:
												this.m_style |= FontStyles.LowerCase;
												return true;
											}
											IL_1252:
											this.m_fontWeightInternal = this.m_fontWeightStack.Remove();
											if (this.m_fontWeightInternal == 400)
											{
												this.m_style &= (FontStyles)(-2);
											}
											return true;
										}
										if (this.m_currentMaterial.GetTexture(ShaderUtilities.ID_MainTex).GetInstanceID() != this.m_materialReferenceStack.PreviousItem().material.GetTexture(ShaderUtilities.ID_MainTex).GetInstanceID())
										{
											return false;
										}
										MaterialReference materialReference2 = this.m_materialReferenceStack.Remove();
										this.m_currentMaterial = materialReference2.material;
										this.m_currentMaterialIndex = materialReference2.index;
										return true;
									}
									IL_1347:
									num12 = this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength, this.m_xmlAttribute[0].valueDecimalIndex);
									if (num12 == -9999f || num12 == 0f)
									{
										return false;
									}
									if (tagUnits == TagUnits.Pixels)
									{
										this.m_baselineOffset = num12;
										return true;
									}
									if (tagUnits != TagUnits.FontUnits)
									{
										return tagUnits != TagUnits.Percentage && false;
									}
									this.m_baselineOffset = num12 * this.m_fontScale * this.m_fontAsset.fontInfo.Ascender;
									return true;
								}
								case 446:
									goto IL_E42;
								}
							}
							if ((this.m_fontStyle & FontStyles.Bold) != FontStyles.Bold)
							{
								this.m_style &= (FontStyles)(-2);
								this.m_fontWeightInternal = this.m_fontWeightStack.Remove();
							}
							return true;
						}
						return true;
					case 414:
						goto IL_E42;
					}
					IL_E11:
					if ((this.m_fontStyle & FontStyles.Strikethrough) != FontStyles.Strikethrough)
					{
						this.m_style &= (FontStyles)(-65);
					}
					return true;
					IL_E42:
					if ((this.m_fontStyle & FontStyles.Underline) != FontStyles.Underline)
					{
						this.m_style &= (FontStyles)(-5);
					}
					return true;
				case 117:
					goto IL_E32;
				}
				break;
			case 85:
				goto IL_E32;
			}
			this.m_style |= FontStyles.Strikethrough;
			return true;
			IL_E32:
			this.m_style |= FontStyles.Underline;
			return true;
		}

		[SerializeField]
		protected string m_text;

		[SerializeField]
		protected bool m_isRightToLeft;

		[SerializeField]
		protected TMP_FontAsset m_fontAsset;

		protected TMP_FontAsset m_currentFontAsset;

		protected bool m_isSDFShader;

		[SerializeField]
		protected Material m_sharedMaterial;

		protected Material m_currentMaterial;

		protected MaterialReference[] m_materialReferences = new MaterialReference[32];

		protected Dictionary<int, int> m_materialReferenceIndexLookup = new Dictionary<int, int>();

		protected TMP_XmlTagStack<MaterialReference> m_materialReferenceStack = new TMP_XmlTagStack<MaterialReference>(new MaterialReference[16]);

		protected int m_currentMaterialIndex;

		[SerializeField]
		protected Material[] m_fontSharedMaterials;

		[SerializeField]
		protected Material m_fontMaterial;

		[SerializeField]
		protected Material[] m_fontMaterials;

		protected bool m_isMaterialDirty;

		[SerializeField]
		[FormerlySerializedAs("m_fontColor")]
		protected Color32 m_fontColor32 = Color.white;

		[SerializeField]
		protected Color m_fontColor = Color.white;

		protected static Color32 s_colorWhite = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

		[SerializeField]
		protected bool m_enableVertexGradient;

		[SerializeField]
		protected VertexGradient m_fontColorGradient = new VertexGradient(Color.white);

		[SerializeField]
		protected TMP_ColorGradient m_fontColorGradientPreset;

		protected TMP_SpriteAsset m_spriteAsset;

		[SerializeField]
		protected bool m_tintAllSprites;

		protected bool m_tintSprite;

		protected Color32 m_spriteColor;

		[SerializeField]
		protected bool m_overrideHtmlColors;

		[SerializeField]
		protected Color32 m_faceColor = Color.white;

		[SerializeField]
		protected Color32 m_outlineColor = Color.black;

		protected float m_outlineWidth;

		[SerializeField]
		protected float m_fontSize = 36f;

		protected float m_currentFontSize;

		[SerializeField]
		protected float m_fontSizeBase = 36f;

		protected TMP_XmlTagStack<float> m_sizeStack = new TMP_XmlTagStack<float>(new float[16]);

		[SerializeField]
		protected int m_fontWeight = 400;

		protected int m_fontWeightInternal;

		protected TMP_XmlTagStack<int> m_fontWeightStack = new TMP_XmlTagStack<int>(new int[16]);

		[SerializeField]
		protected bool m_enableAutoSizing;

		protected float m_maxFontSize;

		protected float m_minFontSize;

		[SerializeField]
		protected float m_fontSizeMin;

		[SerializeField]
		protected float m_fontSizeMax;

		[SerializeField]
		protected FontStyles m_fontStyle;

		protected FontStyles m_style;

		protected bool m_isUsingBold;

		[FormerlySerializedAs("m_lineJustification")]
		[SerializeField]
		protected TextAlignmentOptions m_textAlignment;

		protected TextAlignmentOptions m_lineJustification;

		protected Vector3[] m_textContainerLocalCorners = new Vector3[4];

		[SerializeField]
		protected float m_characterSpacing;

		protected float m_cSpacing;

		protected float m_monoSpacing;

		[SerializeField]
		protected float m_lineSpacing;

		protected float m_lineSpacingDelta;

		protected float m_lineHeight;

		[SerializeField]
		protected float m_lineSpacingMax;

		[SerializeField]
		protected float m_paragraphSpacing;

		[SerializeField]
		protected float m_charWidthMaxAdj;

		protected float m_charWidthAdjDelta;

		[SerializeField]
		protected bool m_enableWordWrapping;

		protected bool m_isCharacterWrappingEnabled;

		protected bool m_isNonBreakingSpace;

		protected bool m_isIgnoringAlignment;

		[SerializeField]
		protected float m_wordWrappingRatios = 0.4f;

		[SerializeField]
		protected bool m_enableAdaptiveJustification;

		protected float m_adaptiveJustificationThreshold = 10f;

		[SerializeField]
		protected TextOverflowModes m_overflowMode;

		protected bool m_isTextTruncated;

		[SerializeField]
		protected bool m_enableKerning;

		[SerializeField]
		protected bool m_enableExtraPadding;

		[SerializeField]
		protected bool checkPaddingRequired;

		[SerializeField]
		protected bool m_isRichText = true;

		[SerializeField]
		protected bool m_parseCtrlCharacters = true;

		protected bool m_isOverlay;

		[SerializeField]
		protected bool m_isOrthographic;

		[SerializeField]
		protected bool m_isCullingEnabled;

		[SerializeField]
		protected bool m_ignoreCulling = true;

		[SerializeField]
		protected TextureMappingOptions m_horizontalMapping;

		[SerializeField]
		protected TextureMappingOptions m_verticalMapping;

		protected TextRenderFlags m_renderMode = TextRenderFlags.Render;

		protected int m_maxVisibleCharacters = 99999;

		protected int m_maxVisibleWords = 99999;

		protected int m_maxVisibleLines = 99999;

		[SerializeField]
		protected bool m_useMaxVisibleDescender = true;

		[SerializeField]
		protected int m_pageToDisplay = 1;

		protected bool m_isNewPage;

		[SerializeField]
		protected Vector4 m_margin = new Vector4(0f, 0f, 0f, 0f);

		protected float m_marginLeft;

		protected float m_marginRight;

		protected float m_marginWidth;

		protected float m_marginHeight;

		protected float m_width = -1f;

		[SerializeField]
		protected TMP_TextInfo m_textInfo;

		[SerializeField]
		protected bool m_havePropertiesChanged;

		[SerializeField]
		protected bool m_isUsingLegacyAnimationComponent;

		protected Transform m_transform;

		protected RectTransform m_rectTransform;

		protected Mesh m_mesh;

		[SerializeField]
		protected bool m_isVolumetricText;

		protected float m_flexibleHeight = -1f;

		protected float m_flexibleWidth = -1f;

		protected float m_minHeight;

		protected float m_minWidth;

		protected float m_preferredWidth;

		protected float m_renderedWidth;

		protected bool m_isPreferredWidthDirty;

		protected float m_preferredHeight;

		protected float m_renderedHeight;

		protected bool m_isPreferredHeightDirty;

		protected bool m_isCalculatingPreferredValues;

		protected int m_layoutPriority;

		protected bool m_isCalculateSizeRequired;

		protected bool m_isLayoutDirty;

		protected bool m_verticesAlreadyDirty;

		protected bool m_layoutAlreadyDirty;

		protected bool m_isAwake;

		[SerializeField]
		protected bool m_isInputParsingRequired;

		[SerializeField]
		protected TMP_Text.TextInputSources m_inputSource;

		protected string old_text;

		protected float old_arg0;

		protected float old_arg1;

		protected float old_arg2;

		protected float m_fontScale;

		protected float m_fontScaleMultiplier;

		protected char[] m_htmlTag = new char[128];

		protected XML_TagAttribute[] m_xmlAttribute = new XML_TagAttribute[8];

		protected float tag_LineIndent;

		protected float tag_Indent;

		protected TMP_XmlTagStack<float> m_indentStack = new TMP_XmlTagStack<float>(new float[16]);

		protected bool tag_NoParsing;

		protected bool m_isParsingText;

		protected int[] m_char_buffer;

		private TMP_CharacterInfo[] m_internalCharacterInfo;

		protected char[] m_input_CharArray = new char[256];

		private int m_charArray_Length;

		protected int m_totalCharacterCount;

		protected int m_characterCount;

		protected int m_firstCharacterOfLine;

		protected int m_firstVisibleCharacterOfLine;

		protected int m_lastCharacterOfLine;

		protected int m_lastVisibleCharacterOfLine;

		protected int m_lineNumber;

		protected int m_lineVisibleCharacterCount;

		protected int m_pageNumber;

		protected float m_maxAscender;

		protected float m_maxCapHeight;

		protected float m_maxDescender;

		protected float m_maxLineAscender;

		protected float m_maxLineDescender;

		protected float m_startOfLineAscender;

		protected float m_lineOffset;

		protected Extents m_meshExtents;

		protected Color32 m_htmlColor = new Color(255f, 255f, 255f, 128f);

		protected TMP_XmlTagStack<Color32> m_colorStack = new TMP_XmlTagStack<Color32>(new Color32[16]);

		protected float m_tabSpacing;

		protected float m_spacing;

		protected TMP_XmlTagStack<int> m_styleStack = new TMP_XmlTagStack<int>(new int[16]);

		protected TMP_XmlTagStack<int> m_actionStack = new TMP_XmlTagStack<int>(new int[16]);

		protected float m_padding;

		protected float m_baselineOffset;

		protected float m_xAdvance;

		protected TMP_TextElementType m_textElementType;

		protected TMP_TextElement m_cached_TextElement;

		protected TMP_Glyph m_cached_Underline_GlyphInfo;

		protected TMP_Glyph m_cached_Ellipsis_GlyphInfo;

		protected TMP_SpriteAsset m_defaultSpriteAsset;

		protected TMP_SpriteAsset m_currentSpriteAsset;

		protected int m_spriteCount;

		protected int m_spriteIndex;

		protected InlineGraphicManager m_inlineGraphics;

		protected bool m_ignoreActiveState;

		private readonly float[] k_Power = new float[]
		{
			0.5f,
			0.05f,
			0.005f,
			0.0005f,
			5E-05f,
			5E-06f,
			5E-07f,
			5E-08f,
			5E-09f,
			5E-10f
		};

		protected static Vector2 k_LargePositiveVector2 = new Vector2(2.14748365E+09f, 2.14748365E+09f);

		protected static Vector2 k_LargeNegativeVector2 = new Vector2(-2.14748365E+09f, -2.14748365E+09f);

		protected static float k_LargePositiveFloat = 32768f;

		protected static float k_LargeNegativeFloat = -32768f;

		protected static int k_LargePositiveInt = int.MaxValue;

		protected static int k_LargeNegativeInt = -2147483647;

		protected enum TextInputSources
		{
			Text,
			SetText,
			SetCharArray,
			String
		}
	}
}
