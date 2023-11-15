using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TMPro
{
	[AddComponentMenu("UI/TextMeshPro - Input Field", 11)]
	public class TMP_InputField : Selectable, IUpdateSelectedHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, ISubmitHandler, ICanvasElement, IEventSystemHandler
	{
		protected TMP_InputField()
		{
		}

		protected Mesh mesh
		{
			get
			{
				if (this.m_Mesh == null)
				{
					this.m_Mesh = new Mesh();
				}
				return this.m_Mesh;
			}
		}

		public bool shouldHideMobileInput
		{
			get
			{
				RuntimePlatform platform = Application.platform;
				switch (platform)
				{
				case RuntimePlatform.IPhonePlayer:
				case RuntimePlatform.Android:
					break;
				default:
					if (platform != RuntimePlatform.BlackBerryPlayer && platform != RuntimePlatform.TizenPlayer)
					{
						return true;
					}
					break;
				}
				return this.m_HideMobileInput;
			}
			set
			{
				SetPropertyUtility.SetStruct<bool>(ref this.m_HideMobileInput, value);
			}
		}

		public string text
		{
			get
			{
				return this.m_Text;
			}
			set
			{
				if (this.text == value)
				{
					return;
				}
				this.m_Text = value;
				if (this.m_Keyboard != null)
				{
					this.m_Keyboard.text = this.m_Text;
				}
				if (this.m_StringPosition > this.m_Text.Length)
				{
					this.m_StringPosition = (this.m_StringSelectPosition = this.m_Text.Length);
				}
				this.SendOnValueChangedAndUpdateLabel();
			}
		}

		public bool isFocused
		{
			get
			{
				return this.m_AllowInput;
			}
		}

		public float caretBlinkRate
		{
			get
			{
				return this.m_CaretBlinkRate;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<float>(ref this.m_CaretBlinkRate, value) && this.m_AllowInput)
				{
					this.SetCaretActive();
				}
			}
		}

		public int caretWidth
		{
			get
			{
				return this.m_CaretWidth;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<int>(ref this.m_CaretWidth, value))
				{
					this.MarkGeometryAsDirty();
				}
			}
		}

		public RectTransform textViewport
		{
			get
			{
				return this.m_TextViewport;
			}
			set
			{
				SetPropertyUtility.SetClass<RectTransform>(ref this.m_TextViewport, value);
			}
		}

		public TMP_Text textComponent
		{
			get
			{
				return this.m_TextComponent;
			}
			set
			{
				SetPropertyUtility.SetClass<TMP_Text>(ref this.m_TextComponent, value);
			}
		}

		public Graphic placeholder
		{
			get
			{
				return this.m_Placeholder;
			}
			set
			{
				SetPropertyUtility.SetClass<Graphic>(ref this.m_Placeholder, value);
			}
		}

		public Color caretColor
		{
			get
			{
				return (!this.customCaretColor) ? this.textComponent.color : this.m_CaretColor;
			}
			set
			{
				if (SetPropertyUtility.SetColor(ref this.m_CaretColor, value))
				{
					this.MarkGeometryAsDirty();
				}
			}
		}

		public bool customCaretColor
		{
			get
			{
				return this.m_CustomCaretColor;
			}
			set
			{
				if (this.m_CustomCaretColor != value)
				{
					this.m_CustomCaretColor = value;
					this.MarkGeometryAsDirty();
				}
			}
		}

		public Color selectionColor
		{
			get
			{
				return this.m_SelectionColor;
			}
			set
			{
				if (SetPropertyUtility.SetColor(ref this.m_SelectionColor, value))
				{
					this.MarkGeometryAsDirty();
				}
			}
		}

		public TMP_InputField.SubmitEvent onEndEdit
		{
			get
			{
				return this.m_OnEndEdit;
			}
			set
			{
				SetPropertyUtility.SetClass<TMP_InputField.SubmitEvent>(ref this.m_OnEndEdit, value);
			}
		}

		public TMP_InputField.SubmitEvent onSubmit
		{
			get
			{
				return this.m_OnSubmit;
			}
			set
			{
				SetPropertyUtility.SetClass<TMP_InputField.SubmitEvent>(ref this.m_OnSubmit, value);
			}
		}

		public TMP_InputField.SubmitEvent onFocusLost
		{
			get
			{
				return this.m_OnFocusLost;
			}
			set
			{
				SetPropertyUtility.SetClass<TMP_InputField.SubmitEvent>(ref this.m_OnFocusLost, value);
			}
		}

		public TMP_InputField.OnChangeEvent onValueChanged
		{
			get
			{
				return this.m_OnValueChanged;
			}
			set
			{
				SetPropertyUtility.SetClass<TMP_InputField.OnChangeEvent>(ref this.m_OnValueChanged, value);
			}
		}

		public TMP_InputField.OnValidateInput onValidateInput
		{
			get
			{
				return this.m_OnValidateInput;
			}
			set
			{
				SetPropertyUtility.SetClass<TMP_InputField.OnValidateInput>(ref this.m_OnValidateInput, value);
			}
		}

		public int characterLimit
		{
			get
			{
				return this.m_CharacterLimit;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<int>(ref this.m_CharacterLimit, Math.Max(0, value)))
				{
					this.UpdateLabel();
				}
			}
		}

		public TMP_InputField.ContentType contentType
		{
			get
			{
				return this.m_ContentType;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<TMP_InputField.ContentType>(ref this.m_ContentType, value))
				{
					this.EnforceContentType();
				}
			}
		}

		public TMP_InputField.LineType lineType
		{
			get
			{
				return this.m_LineType;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<TMP_InputField.LineType>(ref this.m_LineType, value))
				{
					this.SetTextComponentWrapMode();
				}
				this.SetToCustomIfContentTypeIsNot(new TMP_InputField.ContentType[]
				{
					TMP_InputField.ContentType.Standard,
					TMP_InputField.ContentType.Autocorrected
				});
			}
		}

		public TMP_InputField.InputType inputType
		{
			get
			{
				return this.m_InputType;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<TMP_InputField.InputType>(ref this.m_InputType, value))
				{
					this.SetToCustom();
				}
			}
		}

		public TouchScreenKeyboardType keyboardType
		{
			get
			{
				return this.m_KeyboardType;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<TouchScreenKeyboardType>(ref this.m_KeyboardType, value))
				{
					this.SetToCustom();
				}
			}
		}

		public TMP_InputField.CharacterValidation characterValidation
		{
			get
			{
				return this.m_CharacterValidation;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<TMP_InputField.CharacterValidation>(ref this.m_CharacterValidation, value))
				{
					this.SetToCustom();
				}
			}
		}

		public bool readOnly
		{
			get
			{
				return this.m_ReadOnly;
			}
			set
			{
				this.m_ReadOnly = value;
			}
		}

		public bool richText
		{
			get
			{
				return this.m_RichText;
			}
			set
			{
				this.m_RichText = value;
				this.SetTextComponentRichTextMode();
			}
		}

		public bool multiLine
		{
			get
			{
				return this.m_LineType == TMP_InputField.LineType.MultiLineNewline || this.lineType == TMP_InputField.LineType.MultiLineSubmit;
			}
		}

		public char asteriskChar
		{
			get
			{
				return this.m_AsteriskChar;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<char>(ref this.m_AsteriskChar, value))
				{
					this.UpdateLabel();
				}
			}
		}

		public bool wasCanceled
		{
			get
			{
				return this.m_WasCanceled;
			}
		}

		protected void ClampPos(ref int pos)
		{
			if (pos < 0)
			{
				pos = 0;
			}
			else if (pos > this.text.Length)
			{
				pos = this.text.Length;
			}
		}

		protected int caretPositionInternal
		{
			get
			{
				return this.m_CaretPosition + Input.compositionString.Length;
			}
			set
			{
				this.m_CaretPosition = value;
				this.ClampPos(ref this.m_CaretPosition);
			}
		}

		protected int stringPositionInternal
		{
			get
			{
				return this.m_StringPosition + Input.compositionString.Length;
			}
			set
			{
				this.m_StringPosition = value;
				this.ClampPos(ref this.m_StringPosition);
			}
		}

		protected int caretSelectPositionInternal
		{
			get
			{
				return this.m_CaretSelectPosition + Input.compositionString.Length;
			}
			set
			{
				this.m_CaretSelectPosition = value;
				this.ClampPos(ref this.m_CaretSelectPosition);
			}
		}

		protected int stringSelectPositionInternal
		{
			get
			{
				return this.m_StringSelectPosition + Input.compositionString.Length;
			}
			set
			{
				this.m_StringSelectPosition = value;
				this.ClampPos(ref this.m_StringSelectPosition);
			}
		}

		private bool hasSelection
		{
			get
			{
				return this.stringPositionInternal != this.stringSelectPositionInternal;
			}
		}

		public int caretPosition
		{
			get
			{
				return this.m_StringSelectPosition + Input.compositionString.Length;
			}
			set
			{
				this.selectionAnchorPosition = value;
				this.selectionFocusPosition = value;
			}
		}

		public int selectionAnchorPosition
		{
			get
			{
				this.m_StringPosition = this.GetStringIndexFromCaretPosition(this.m_CaretPosition);
				return this.m_StringPosition + Input.compositionString.Length;
			}
			set
			{
				if (Input.compositionString.Length != 0)
				{
					return;
				}
				this.m_CaretPosition = value;
				this.ClampPos(ref this.m_CaretPosition);
			}
		}

		public int selectionFocusPosition
		{
			get
			{
				this.m_StringSelectPosition = this.GetStringIndexFromCaretPosition(this.m_CaretSelectPosition);
				return this.m_StringSelectPosition + Input.compositionString.Length;
			}
			set
			{
				if (Input.compositionString.Length != 0)
				{
					return;
				}
				this.m_CaretSelectPosition = value;
				this.ClampPos(ref this.m_CaretSelectPosition);
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if (this.m_Text == null)
			{
				this.m_Text = string.Empty;
			}
			this.m_DrawStart = 0;
			this.m_DrawEnd = this.m_Text.Length;
			if (this.m_CachedInputRenderer != null)
			{
				this.m_CachedInputRenderer.SetMaterial(Graphic.defaultGraphicMaterial, Texture2D.whiteTexture);
			}
			if (this.m_TextComponent != null)
			{
				this.m_TextComponent.RegisterDirtyVerticesCallback(new UnityAction(this.MarkGeometryAsDirty));
				this.m_TextComponent.RegisterDirtyVerticesCallback(new UnityAction(this.UpdateLabel));
				this.UpdateLabel();
			}
		}

		protected override void OnDisable()
		{
			this.m_BlinkCoroutine = null;
			this.DeactivateInputField();
			if (this.m_TextComponent != null)
			{
				this.m_TextComponent.UnregisterDirtyVerticesCallback(new UnityAction(this.MarkGeometryAsDirty));
				this.m_TextComponent.UnregisterDirtyVerticesCallback(new UnityAction(this.UpdateLabel));
			}
			CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild(this);
			if (this.m_CachedInputRenderer != null)
			{
				this.m_CachedInputRenderer.Clear();
			}
			if (this.m_Mesh != null)
			{
				UnityEngine.Object.DestroyImmediate(this.m_Mesh);
			}
			this.m_Mesh = null;
			base.OnDisable();
		}

		private IEnumerator CaretBlink()
		{
			this.m_CaretVisible = true;
			yield return null;
			while (this.isFocused && this.m_CaretBlinkRate > 0f)
			{
				float blinkPeriod = 1f / this.m_CaretBlinkRate;
				bool blinkState = (Time.unscaledTime - this.m_BlinkStartTime) % blinkPeriod < blinkPeriod / 2f;
				if (this.m_CaretVisible != blinkState)
				{
					this.m_CaretVisible = blinkState;
					if (!this.hasSelection)
					{
						this.MarkGeometryAsDirty();
					}
				}
				yield return null;
			}
			this.m_BlinkCoroutine = null;
			yield break;
		}

		private void SetCaretVisible()
		{
			if (!this.m_AllowInput)
			{
				return;
			}
			this.m_CaretVisible = true;
			this.m_BlinkStartTime = Time.unscaledTime;
			this.SetCaretActive();
		}

		private void SetCaretActive()
		{
			if (!this.m_AllowInput)
			{
				return;
			}
			if (this.m_CaretBlinkRate > 0f)
			{
				if (this.m_BlinkCoroutine == null)
				{
					this.m_BlinkCoroutine = base.StartCoroutine(this.CaretBlink());
				}
			}
			else
			{
				this.m_CaretVisible = true;
			}
		}

		protected void OnFocus()
		{
			this.SelectAll();
		}

		protected void SelectAll()
		{
			this.stringPositionInternal = this.text.Length;
			this.stringSelectPositionInternal = 0;
		}

		public void MoveTextEnd(bool shift)
		{
			int length = this.text.Length;
			if (shift)
			{
				this.stringSelectPositionInternal = length;
			}
			else
			{
				this.stringPositionInternal = length;
				this.stringSelectPositionInternal = this.stringPositionInternal;
			}
			this.UpdateLabel();
		}

		public void MoveTextStart(bool shift)
		{
			int num = 0;
			if (shift)
			{
				this.stringSelectPositionInternal = num;
			}
			else
			{
				this.stringPositionInternal = num;
				this.stringSelectPositionInternal = this.stringPositionInternal;
			}
			this.UpdateLabel();
		}

		private static string clipboard
		{
			get
			{
				return GUIUtility.systemCopyBuffer;
			}
			set
			{
				GUIUtility.systemCopyBuffer = value;
			}
		}

		private bool InPlaceEditing()
		{
			return !TouchScreenKeyboard.isSupported;
		}

		protected virtual void LateUpdate()
		{
			if (this.m_ShouldActivateNextUpdate)
			{
				if (!this.isFocused)
				{
					this.ActivateInputFieldInternal();
					this.m_ShouldActivateNextUpdate = false;
					return;
				}
				this.m_ShouldActivateNextUpdate = false;
			}
			if (this.InPlaceEditing() || !this.isFocused)
			{
				return;
			}
			this.AssignPositioningIfNeeded();
			if (this.m_Keyboard == null || !this.m_Keyboard.active)
			{
				if (this.m_Keyboard != null)
				{
					if (!this.m_ReadOnly)
					{
						this.text = this.m_Keyboard.text;
					}
					if (this.m_Keyboard.wasCanceled)
					{
						this.m_WasCanceled = true;
					}
				}
				this.OnDeselect(null);
				return;
			}
			string text = this.m_Keyboard.text;
			if (this.m_Text != text)
			{
				if (this.m_ReadOnly)
				{
					this.m_Keyboard.text = this.m_Text;
				}
				else
				{
					this.m_Text = string.Empty;
                    for (int i = 0; i < text.Length; ++i)
                    {
                        char c = text[i];

                        if (c == '\r' || (int)c == 3)
                            c = '\n';

                        if (onValidateInput != null)
                            c = onValidateInput(m_Text, m_Text.Length, c);
                        else if (characterValidation != CharacterValidation.None)
                            c = Validate(m_Text, m_Text.Length, c);

                        if (lineType == LineType.MultiLineSubmit && c == '\n')
                        {
                            m_Keyboard.text = m_Text;

                            OnSubmit(null);
                            OnDeselect(null);
                            return;
                        }

                        if (c != 0)
                            m_Text += c;
                    }
                    if (this.characterLimit > 0 && this.m_Text.Length > this.characterLimit)
					{
						this.m_Text = this.m_Text.Substring(0, this.characterLimit);
					}
					int length = this.m_Text.Length;
					this.stringSelectPositionInternal = length;
					this.stringPositionInternal = length;
					if (this.m_Text != text)
					{
						this.m_Keyboard.text = this.m_Text;
					}
					this.SendOnValueChangedAndUpdateLabel();
				}
			}
			if (this.m_Keyboard.done)
			{
				if (this.m_Keyboard.wasCanceled)
				{
					this.m_WasCanceled = true;
				}
				this.OnDeselect(null);
			}
		}

		protected int GetCharacterIndexFromPosition(Vector2 pos)
		{
			return 0;
		}

		private bool MayDrag(PointerEventData eventData)
		{
			return this.IsActive() && this.IsInteractable() && eventData.button == PointerEventData.InputButton.Left && this.m_TextComponent != null && this.m_Keyboard == null;
		}

		public virtual void OnBeginDrag(PointerEventData eventData)
		{
			if (!this.MayDrag(eventData))
			{
				return;
			}
			this.m_UpdateDrag = true;
		}

		public virtual void OnDrag(PointerEventData eventData)
		{
			if (!this.MayDrag(eventData))
			{
				return;
			}
			CaretPosition caretPosition;
			int cursorIndexFromPosition = TMP_TextUtilities.GetCursorIndexFromPosition(this.m_TextComponent, eventData.position, eventData.pressEventCamera, out caretPosition);
			if (caretPosition == CaretPosition.Left)
			{
				this.stringSelectPositionInternal = this.GetStringIndexFromCaretPosition(cursorIndexFromPosition);
			}
			else if (caretPosition == CaretPosition.Right)
			{
				this.stringSelectPositionInternal = this.GetStringIndexFromCaretPosition(cursorIndexFromPosition) + 1;
			}
			this.caretSelectPositionInternal = this.GetCaretPositionFromStringIndex(this.stringSelectPositionInternal);
			this.MarkGeometryAsDirty();
			this.m_DragPositionOutOfBounds = !RectTransformUtility.RectangleContainsScreenPoint(this.textViewport, eventData.position, eventData.pressEventCamera);
			if (this.m_DragPositionOutOfBounds && this.m_DragCoroutine == null)
			{
				this.m_DragCoroutine = base.StartCoroutine(this.MouseDragOutsideRect(eventData));
			}
			eventData.Use();
		}

		private IEnumerator MouseDragOutsideRect(PointerEventData eventData)
		{
			while (this.m_UpdateDrag && this.m_DragPositionOutOfBounds)
			{
				Vector2 localMousePos;
				RectTransformUtility.ScreenPointToLocalPointInRectangle(this.textViewport, eventData.position, eventData.pressEventCamera, out localMousePos);
				Rect rect = this.textViewport.rect;
				if (this.multiLine)
				{
					if (localMousePos.y > rect.yMax)
					{
						this.MoveUp(true, true);
					}
					else if (localMousePos.y < rect.yMin)
					{
						this.MoveDown(true, true);
					}
				}
				else if (localMousePos.x < rect.xMin)
				{
					this.MoveLeft(true, false);
				}
				else if (localMousePos.x > rect.xMax)
				{
					this.MoveRight(true, false);
				}
				this.UpdateLabel();
				float delay = (!this.multiLine) ? 0.05f : 0.1f;
				yield return new WaitForSeconds(delay);
			}
			this.m_DragCoroutine = null;
			yield break;
		}

		public virtual void OnEndDrag(PointerEventData eventData)
		{
			if (!this.MayDrag(eventData))
			{
				return;
			}
			this.m_UpdateDrag = false;
		}

		public override void OnPointerDown(PointerEventData eventData)
		{
			if (!this.MayDrag(eventData))
			{
				return;
			}
			EventSystem.current.SetSelectedGameObject(base.gameObject, eventData);
			bool allowInput = this.m_AllowInput;
			base.OnPointerDown(eventData);
			if (!this.InPlaceEditing() && (this.m_Keyboard == null || !this.m_Keyboard.active))
			{
				this.OnSelect(eventData);
				return;
			}
			if (allowInput)
			{
				CaretPosition caretPosition;
				int cursorIndexFromPosition = TMP_TextUtilities.GetCursorIndexFromPosition(this.m_TextComponent, eventData.position, eventData.pressEventCamera, out caretPosition);
				int num;
				if (caretPosition == CaretPosition.Left)
				{
					num = this.GetStringIndexFromCaretPosition(cursorIndexFromPosition);
					this.stringSelectPositionInternal = num;
					this.stringPositionInternal = num;
				}
				else if (caretPosition == CaretPosition.Right)
				{
					num = this.GetStringIndexFromCaretPosition(cursorIndexFromPosition) + 1;
					this.stringSelectPositionInternal = num;
					this.stringPositionInternal = num;
				}
				num = this.GetCaretPositionFromStringIndex(this.stringPositionInternal);
				this.caretSelectPositionInternal = num;
				this.caretPositionInternal = num;
			}
			this.UpdateLabel();
			eventData.Use();
		}

		protected TMP_InputField.EditState KeyPressed(Event evt)
		{
			EventModifiers modifiers = evt.modifiers;
			RuntimePlatform platform = Application.platform;
			bool flag = platform == RuntimePlatform.OSXEditor || platform == RuntimePlatform.OSXPlayer;
			bool flag2 = (!flag) ? ((modifiers & EventModifiers.Control) != EventModifiers.None) : ((modifiers & EventModifiers.Command) != EventModifiers.None);
			bool flag3 = (modifiers & EventModifiers.Shift) != EventModifiers.None;
			bool flag4 = (modifiers & EventModifiers.Alt) != EventModifiers.None;
			bool flag5 = flag2 && !flag4 && !flag3;
			KeyCode keyCode = evt.keyCode;
			switch (keyCode)
			{
			case KeyCode.KeypadEnter:
				break;
			default:
				switch (keyCode)
				{
				case KeyCode.A:
					if (flag5)
					{
						this.SelectAll();
						return TMP_InputField.EditState.Continue;
					}
					goto IL_1FE;
				default:
					switch (keyCode)
					{
					case KeyCode.V:
						if (flag5)
						{
							this.Append(TMP_InputField.clipboard);
							return TMP_InputField.EditState.Continue;
						}
						goto IL_1FE;
					default:
						if (keyCode == KeyCode.Backspace)
						{
							this.Backspace();
							return TMP_InputField.EditState.Continue;
						}
						if (keyCode != KeyCode.Return)
						{
							if (keyCode == KeyCode.Escape)
							{
								this.m_WasCanceled = true;
								return TMP_InputField.EditState.Finish;
							}
							if (keyCode != KeyCode.Delete)
							{
								goto IL_1FE;
							}
							this.ForwardSpace();
							return TMP_InputField.EditState.Continue;
						}
						break;
					case KeyCode.X:
						if (flag5)
						{
							if (this.inputType != TMP_InputField.InputType.Password)
							{
								TMP_InputField.clipboard = this.GetSelectedString();
							}
							else
							{
								TMP_InputField.clipboard = string.Empty;
							}
							this.Delete();
							this.SendOnValueChangedAndUpdateLabel();
							return TMP_InputField.EditState.Continue;
						}
						goto IL_1FE;
					}
					break;
				case KeyCode.C:
					if (flag5)
					{
						if (this.inputType != TMP_InputField.InputType.Password)
						{
							TMP_InputField.clipboard = this.GetSelectedString();
						}
						else
						{
							TMP_InputField.clipboard = string.Empty;
						}
						return TMP_InputField.EditState.Continue;
					}
					goto IL_1FE;
				}
				break;
			case KeyCode.UpArrow:
				this.MoveUp(flag3);
				return TMP_InputField.EditState.Continue;
			case KeyCode.DownArrow:
				this.MoveDown(flag3);
				return TMP_InputField.EditState.Continue;
			case KeyCode.RightArrow:
				this.MoveRight(flag3, flag2);
				return TMP_InputField.EditState.Continue;
			case KeyCode.LeftArrow:
				this.MoveLeft(flag3, flag2);
				return TMP_InputField.EditState.Continue;
			case KeyCode.Home:
				this.MoveTextStart(flag3);
				return TMP_InputField.EditState.Continue;
			case KeyCode.End:
				this.MoveTextEnd(flag3);
				return TMP_InputField.EditState.Continue;
			}
			if (this.lineType != TMP_InputField.LineType.MultiLineNewline)
			{
				return TMP_InputField.EditState.Finish;
			}
			IL_1FE:
			char c = evt.character;
			if (!this.multiLine && (c == '\t' || c == '\r' || c == '\n'))
			{
				return TMP_InputField.EditState.Continue;
			}
			if (c == '\r' || c == '\u0003')
			{
				c = '\n';
			}
			if (this.IsValidChar(c))
			{
				this.Append(c);
			}
			if (c == '\0' && Input.compositionString.Length > 0)
			{
				this.UpdateLabel();
			}
			return TMP_InputField.EditState.Continue;
		}

		private bool IsValidChar(char c)
		{
			return c != '\u007f' && (c == '\t' || c == '\n' || this.m_TextComponent.font.HasCharacter(c, true));
		}

		public void ProcessEvent(Event e)
		{
			this.KeyPressed(e);
		}

		public virtual void OnUpdateSelected(BaseEventData eventData)
		{
			if (!this.isFocused)
			{
				return;
			}
			bool flag = false;
			while (Event.PopEvent(this.m_ProcessingEvent))
			{
				if (this.m_ProcessingEvent.rawType == EventType.KeyDown)
				{
					flag = true;
					TMP_InputField.EditState editState = this.KeyPressed(this.m_ProcessingEvent);
					if (editState == TMP_InputField.EditState.Finish)
					{
						this.DeactivateInputField();
						break;
					}
				}
				EventType type = this.m_ProcessingEvent.type;
				if (type == EventType.ValidateCommand || type == EventType.ExecuteCommand)
				{
					string commandName = this.m_ProcessingEvent.commandName;
					if (commandName != null)
					{
						if (commandName == "SelectAll")
						{
							this.SelectAll();
							flag = true;
						}
					}
				}
			}
			if (flag)
			{
				this.UpdateLabel();
			}
			eventData.Use();
		}

		private string GetSelectedString()
		{
			if (!this.hasSelection)
			{
				return string.Empty;
			}
			int num = this.stringPositionInternal;
			int num2 = this.stringSelectPositionInternal;
			if (num > num2)
			{
				int num3 = num;
				num = num2;
				num2 = num3;
			}
			return this.text.Substring(num, num2 - num);
		}

		private int FindtNextWordBegin()
		{
			if (this.stringSelectPositionInternal + 1 >= this.text.Length)
			{
				return this.text.Length;
			}
			int num = this.text.IndexOfAny(TMP_InputField.kSeparators, this.stringSelectPositionInternal + 1);
			if (num == -1)
			{
				num = this.text.Length;
			}
			else
			{
				num++;
			}
			return num;
		}

		private void MoveRight(bool shift, bool ctrl)
		{
			if (this.hasSelection && !shift)
			{
				int num = Mathf.Max(this.stringPositionInternal, this.stringSelectPositionInternal);
				this.stringSelectPositionInternal = num;
				this.stringPositionInternal = num;
				num = this.GetCaretPositionFromStringIndex(this.stringSelectPositionInternal);
				this.caretSelectPositionInternal = num;
				this.caretPositionInternal = num;
				return;
			}
			int caretSelectPositionInternal = this.caretSelectPositionInternal;
			int num2;
			if (ctrl)
			{
				num2 = this.FindtNextWordBegin();
			}
			else
			{
				num2 = this.stringSelectPositionInternal + 1;
			}
			if (shift)
			{
				this.stringSelectPositionInternal = num2;
				this.caretSelectPositionInternal = this.GetCaretPositionFromStringIndex(this.stringSelectPositionInternal);
			}
			else
			{
				int num = num2;
				this.stringPositionInternal = num;
				this.stringSelectPositionInternal = num;
				num = this.GetCaretPositionFromStringIndex(this.stringSelectPositionInternal);
				this.caretPositionInternal = num;
				this.caretSelectPositionInternal = num;
			}
			this.isCaretInsideTag = (caretSelectPositionInternal == this.caretSelectPositionInternal);
			UnityEngine.Debug.Log("Caret is " + ((!this.isCaretInsideTag) ? " [Not Inside Tag]" : " [Inside Tag]"));
		}

		private int FindtPrevWordBegin()
		{
			if (this.stringSelectPositionInternal - 2 < 0)
			{
				return 0;
			}
			int num = this.text.LastIndexOfAny(TMP_InputField.kSeparators, this.stringSelectPositionInternal - 2);
			if (num == -1)
			{
				num = 0;
			}
			else
			{
				num++;
			}
			return num;
		}

		private void MoveLeft(bool shift, bool ctrl)
		{
			if (this.hasSelection && !shift)
			{
				int num = Mathf.Min(this.stringPositionInternal, this.stringSelectPositionInternal);
				this.stringSelectPositionInternal = num;
				this.stringPositionInternal = num;
				num = this.GetCaretPositionFromStringIndex(this.stringSelectPositionInternal);
				this.caretSelectPositionInternal = num;
				this.caretPositionInternal = num;
				return;
			}
			int caretSelectPositionInternal = this.caretSelectPositionInternal;
			int num2;
			if (ctrl)
			{
				num2 = this.FindtPrevWordBegin();
			}
			else
			{
				num2 = this.stringSelectPositionInternal - 1;
			}
			if (shift)
			{
				this.stringSelectPositionInternal = num2;
				this.caretSelectPositionInternal = this.GetCaretPositionFromStringIndex(this.stringSelectPositionInternal);
			}
			else
			{
				int num = num2;
				this.stringPositionInternal = num;
				this.stringSelectPositionInternal = num;
				num = this.GetCaretPositionFromStringIndex(this.stringSelectPositionInternal);
				this.caretPositionInternal = num;
				this.caretSelectPositionInternal = num;
			}
			this.isCaretInsideTag = (caretSelectPositionInternal == this.caretSelectPositionInternal);
			UnityEngine.Debug.Log("Caret is " + ((!this.isCaretInsideTag) ? " [Not Inside Tag]" : " [Inside Tag]"));
		}

		private int LineUpCharacterPosition(int originalPos, bool goToFirstChar)
		{
			if (originalPos >= this.m_TextComponent.textInfo.characterCount)
			{
				originalPos--;
			}
			TMP_CharacterInfo tmp_CharacterInfo = this.m_TextComponent.textInfo.characterInfo[originalPos];
			int lineNumber = (int)tmp_CharacterInfo.lineNumber;
			if (lineNumber - 1 < 0)
			{
				return (!goToFirstChar) ? originalPos : 0;
			}
			int num = this.m_TextComponent.textInfo.lineInfo[lineNumber].firstCharacterIndex - 1;
			int i = this.m_TextComponent.textInfo.lineInfo[lineNumber - 1].firstCharacterIndex;
			while (i < num)
			{
				TMP_CharacterInfo tmp_CharacterInfo2 = this.m_TextComponent.textInfo.characterInfo[i];
				float num2 = (tmp_CharacterInfo.origin - tmp_CharacterInfo2.origin) / (tmp_CharacterInfo2.xAdvance - tmp_CharacterInfo2.origin);
				if (num2 >= 0f && num2 <= 1f)
				{
					if (num2 < 0.5f)
					{
						return i;
					}
					return i + 1;
				}
				else
				{
					i++;
				}
			}
			return num;
		}

		private int LineDownCharacterPosition(int originalPos, bool goToLastChar)
		{
			if (originalPos >= this.m_TextComponent.textInfo.characterCount)
			{
				return this.text.Length;
			}
			TMP_CharacterInfo tmp_CharacterInfo = this.m_TextComponent.textInfo.characterInfo[originalPos];
			int lineNumber = (int)tmp_CharacterInfo.lineNumber;
			if (lineNumber + 1 >= this.m_TextComponent.textInfo.lineCount)
			{
				return (!goToLastChar) ? originalPos : (this.m_TextComponent.textInfo.characterCount - 1);
			}
			int lastCharacterIndex = this.m_TextComponent.textInfo.lineInfo[lineNumber + 1].lastCharacterIndex;
			int i = this.m_TextComponent.textInfo.lineInfo[lineNumber + 1].firstCharacterIndex;
			while (i < lastCharacterIndex)
			{
				TMP_CharacterInfo tmp_CharacterInfo2 = this.m_TextComponent.textInfo.characterInfo[i];
				float num = (tmp_CharacterInfo.origin - tmp_CharacterInfo2.origin) / (tmp_CharacterInfo2.xAdvance - tmp_CharacterInfo2.origin);
				if (num >= 0f && num <= 1f)
				{
					if (num < 0.5f)
					{
						return i;
					}
					return i + 1;
				}
				else
				{
					i++;
				}
			}
			return lastCharacterIndex;
		}

		private void MoveDown(bool shift)
		{
			this.MoveDown(shift, true);
		}

		private void MoveDown(bool shift, bool goToLastChar)
		{
			if (this.hasSelection && !shift)
			{
				int num = Mathf.Max(this.caretPositionInternal, this.caretSelectPositionInternal);
				this.caretSelectPositionInternal = num;
				this.caretPositionInternal = num;
			}
			int num2 = (!this.multiLine) ? this.text.Length : this.LineDownCharacterPosition(this.caretSelectPositionInternal, goToLastChar);
			if (shift)
			{
				this.caretSelectPositionInternal = num2;
				this.stringSelectPositionInternal = this.GetStringIndexFromCaretPosition(this.caretSelectPositionInternal);
			}
			else
			{
				int num = num2;
				this.caretPositionInternal = num;
				this.caretSelectPositionInternal = num;
				num = this.GetStringIndexFromCaretPosition(this.caretSelectPositionInternal);
				this.stringPositionInternal = num;
				this.stringSelectPositionInternal = num;
			}
		}

		private void MoveUp(bool shift)
		{
			this.MoveUp(shift, true);
		}

		private void MoveUp(bool shift, bool goToFirstChar)
		{
			if (this.hasSelection && !shift)
			{
				int num = Mathf.Min(this.caretPositionInternal, this.caretSelectPositionInternal);
				this.caretSelectPositionInternal = num;
				this.caretPositionInternal = num;
			}
			int num2 = (!this.multiLine) ? 0 : this.LineUpCharacterPosition(this.caretSelectPositionInternal, goToFirstChar);
			if (shift)
			{
				this.caretSelectPositionInternal = num2;
				this.stringSelectPositionInternal = this.GetStringIndexFromCaretPosition(this.caretSelectPositionInternal);
			}
			else
			{
				int num = num2;
				this.caretPositionInternal = num;
				this.caretSelectPositionInternal = num;
				num = this.GetStringIndexFromCaretPosition(this.caretSelectPositionInternal);
				this.stringPositionInternal = num;
				this.stringSelectPositionInternal = num;
			}
		}

		private void Delete()
		{
			if (this.m_ReadOnly)
			{
				return;
			}
			if (this.stringPositionInternal == this.stringSelectPositionInternal)
			{
				return;
			}
			if (this.stringPositionInternal < this.stringSelectPositionInternal)
			{
				this.m_Text = this.text.Substring(0, this.stringPositionInternal) + this.text.Substring(this.stringSelectPositionInternal, this.text.Length - this.stringSelectPositionInternal);
				this.stringSelectPositionInternal = this.stringPositionInternal;
			}
			else
			{
				this.m_Text = this.text.Substring(0, this.stringSelectPositionInternal) + this.text.Substring(this.stringPositionInternal, this.text.Length - this.stringPositionInternal);
				this.stringPositionInternal = this.stringSelectPositionInternal;
			}
		}

		private void ForwardSpace()
		{
			if (this.m_ReadOnly)
			{
				return;
			}
			if (this.hasSelection)
			{
				this.Delete();
				this.SendOnValueChangedAndUpdateLabel();
			}
			else if (this.stringPositionInternal < this.text.Length)
			{
				this.m_Text = this.text.Remove(this.stringPositionInternal, 1);
				this.SendOnValueChangedAndUpdateLabel();
			}
		}

		private void Backspace()
		{
			if (this.m_ReadOnly)
			{
				return;
			}
			if (this.hasSelection)
			{
				this.Delete();
				this.SendOnValueChangedAndUpdateLabel();
			}
			else if (this.stringPositionInternal > 0)
			{
				this.m_Text = this.text.Remove(this.stringPositionInternal - 1, 1);
				int num = this.stringPositionInternal - 1;
				this.stringPositionInternal = num;
				this.stringSelectPositionInternal = num;
				this.m_isLastKeyBackspace = true;
				this.SendOnValueChangedAndUpdateLabel();
			}
		}

		private void Insert(char c)
		{
			if (this.m_ReadOnly)
			{
				return;
			}
			string text = c.ToString();
			this.Delete();
			if (this.characterLimit > 0 && this.text.Length >= this.characterLimit)
			{
				return;
			}
			this.m_Text = this.text.Insert(this.m_StringPosition, text);
			this.stringSelectPositionInternal = (this.stringPositionInternal += text.Length);
			this.SendOnValueChanged();
		}

		private void SendOnValueChangedAndUpdateLabel()
		{
			this.SendOnValueChanged();
			this.UpdateLabel();
		}

		private void SendOnValueChanged()
		{
			if (this.onValueChanged != null)
			{
				this.onValueChanged.Invoke(this.text);
			}
		}

		protected void SendOnSubmit()
		{
			if (this.onEndEdit != null)
			{
				this.onEndEdit.Invoke(this.m_Text);
			}
		}

		protected void SendOnFocusLost()
		{
			if (this.onFocusLost != null)
			{
				this.onFocusLost.Invoke(this.m_Text);
			}
		}

		protected virtual void Append(string input)
		{
			if (this.m_ReadOnly)
			{
				return;
			}
			if (!this.InPlaceEditing())
			{
				return;
			}
			int i = 0;
			int length = input.Length;
			while (i < length)
			{
				char c = input[i];
				if (c >= ' ' || c == '\t' || c == '\r' || c == '\n' || c == '\n')
				{
					this.Append(c);
				}
				i++;
			}
		}

		protected virtual void Append(char input)
		{
			if (this.m_ReadOnly)
			{
				return;
			}
			if (!this.InPlaceEditing())
			{
				return;
			}
			if (this.onValidateInput != null)
			{
				input = this.onValidateInput(this.text, this.stringPositionInternal, input);
			}
			else if (this.characterValidation != TMP_InputField.CharacterValidation.None)
			{
				input = this.Validate(this.text, this.stringPositionInternal, input);
			}
			if (input == '\0')
			{
				return;
			}
			this.Insert(input);
		}

		protected void UpdateLabel()
		{
			if (this.m_TextComponent != null && this.m_TextComponent.font != null)
			{
				string text;
				if (Input.compositionString.Length > 0)
				{
					text = this.text.Substring(0, this.m_StringPosition) + Input.compositionString + this.text.Substring(this.m_StringPosition);
				}
				else
				{
					text = this.text;
				}
				string str;
				if (this.inputType == TMP_InputField.InputType.Password)
				{
					str = new string(this.asteriskChar, text.Length);
				}
				else
				{
					str = text;
				}
				bool flag = string.IsNullOrEmpty(text);
				if (this.m_Placeholder != null)
				{
					this.m_Placeholder.enabled = flag;
				}
				if (!this.m_AllowInput)
				{
					this.m_DrawStart = 0;
					this.m_DrawEnd = this.m_Text.Length;
				}
				if (!flag)
				{
					this.SetCaretVisible();
				}
				this.m_TextComponent.text = str + "​";
				this.MarkGeometryAsDirty();
			}
		}

		private int GetCaretPositionFromStringIndex(int stringIndex)
		{
			int characterCount = this.m_TextComponent.textInfo.characterCount;
			for (int i = 0; i < characterCount; i++)
			{
				if ((int)this.m_TextComponent.textInfo.characterInfo[i].index >= stringIndex)
				{
					return i;
				}
			}
			return characterCount;
		}

		private int GetStringIndexFromCaretPosition(int caretPosition)
		{
			return (int)this.m_TextComponent.textInfo.characterInfo[caretPosition].index;
		}

		public void ForceLabelUpdate()
		{
			this.UpdateLabel();
		}

		private void MarkGeometryAsDirty()
		{
			CanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild(this);
		}

		public virtual void Rebuild(CanvasUpdate update)
		{
			if (update == CanvasUpdate.LatePreRender)
			{
				this.UpdateGeometry();
			}
		}

		public virtual void LayoutComplete()
		{
		}

		public virtual void GraphicUpdateComplete()
		{
		}

		private void UpdateGeometry()
		{
			if (!this.shouldHideMobileInput)
			{
				return;
			}
			if (this.m_CachedInputRenderer == null && this.m_TextComponent != null)
			{
				GameObject gameObject = new GameObject(base.transform.name + " Input Caret");
				gameObject.hideFlags = HideFlags.DontSave;
				gameObject.transform.SetParent(this.m_TextComponent.transform.parent);
				gameObject.transform.SetAsFirstSibling();
				gameObject.layer = base.gameObject.layer;
				this.caretRectTrans = gameObject.AddComponent<RectTransform>();
				this.m_CachedInputRenderer = gameObject.AddComponent<CanvasRenderer>();
				this.m_CachedInputRenderer.SetMaterial(Graphic.defaultGraphicMaterial, Texture2D.whiteTexture);
				gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
				this.AssignPositioningIfNeeded();
			}
			if (this.m_CachedInputRenderer == null)
			{
				return;
			}
			this.OnFillVBO(this.mesh);
			this.m_CachedInputRenderer.SetMesh(this.mesh);
		}

		private void AssignPositioningIfNeeded()
		{
			if (this.m_TextComponent != null && this.caretRectTrans != null && (this.caretRectTrans.localPosition != this.m_TextComponent.rectTransform.localPosition || this.caretRectTrans.localRotation != this.m_TextComponent.rectTransform.localRotation || this.caretRectTrans.localScale != this.m_TextComponent.rectTransform.localScale || this.caretRectTrans.anchorMin != this.m_TextComponent.rectTransform.anchorMin || this.caretRectTrans.anchorMax != this.m_TextComponent.rectTransform.anchorMax || this.caretRectTrans.anchoredPosition != this.m_TextComponent.rectTransform.anchoredPosition || this.caretRectTrans.sizeDelta != this.m_TextComponent.rectTransform.sizeDelta || this.caretRectTrans.pivot != this.m_TextComponent.rectTransform.pivot))
			{
				this.caretRectTrans.localPosition = this.m_TextComponent.rectTransform.localPosition;
				this.caretRectTrans.localRotation = this.m_TextComponent.rectTransform.localRotation;
				this.caretRectTrans.localScale = this.m_TextComponent.rectTransform.localScale;
				this.caretRectTrans.anchorMin = this.m_TextComponent.rectTransform.anchorMin;
				this.caretRectTrans.anchorMax = this.m_TextComponent.rectTransform.anchorMax;
				this.caretRectTrans.anchoredPosition = this.m_TextComponent.rectTransform.anchoredPosition;
				this.caretRectTrans.sizeDelta = this.m_TextComponent.rectTransform.sizeDelta;
				this.caretRectTrans.pivot = this.m_TextComponent.rectTransform.pivot;
			}
		}

		private void OnFillVBO(Mesh vbo)
		{
			using (VertexHelper vertexHelper = new VertexHelper())
			{
				if (!this.isFocused)
				{
					vertexHelper.FillMesh(vbo);
				}
				else
				{
					if (!this.hasSelection)
					{
						this.GenerateCaret(vertexHelper, Vector2.zero);
					}
					else
					{
						this.GenerateHightlight(vertexHelper, Vector2.zero);
					}
					vertexHelper.FillMesh(vbo);
				}
			}
		}

		private void GenerateCaret(VertexHelper vbo, Vector2 roundingOffset)
		{
			if (!this.m_CaretVisible)
			{
				return;
			}
			if (this.m_CursorVerts == null)
			{
				this.CreateCursorVerts();
			}
			float num = (float)this.m_CaretWidth;
			int characterCount = this.m_TextComponent.textInfo.characterCount;
			Vector2 zero = Vector2.zero;
			this.caretPositionInternal = this.GetCaretPositionFromStringIndex(this.stringPositionInternal);
			TMP_CharacterInfo tmp_CharacterInfo;
			float num2;
			if (this.caretPositionInternal == 0)
			{
				tmp_CharacterInfo = this.m_TextComponent.textInfo.characterInfo[0];
				zero = new Vector2(tmp_CharacterInfo.origin, tmp_CharacterInfo.descender);
				num2 = tmp_CharacterInfo.ascender - tmp_CharacterInfo.descender;
			}
			else if (this.caretPositionInternal < characterCount)
			{
				tmp_CharacterInfo = this.m_TextComponent.textInfo.characterInfo[this.caretPositionInternal];
				zero = new Vector2(tmp_CharacterInfo.origin, tmp_CharacterInfo.descender);
				num2 = tmp_CharacterInfo.ascender - tmp_CharacterInfo.descender;
			}
			else
			{
				tmp_CharacterInfo = this.m_TextComponent.textInfo.characterInfo[characterCount - 1];
				zero = new Vector2(tmp_CharacterInfo.xAdvance, tmp_CharacterInfo.descender);
				num2 = tmp_CharacterInfo.ascender - tmp_CharacterInfo.descender;
			}
			this.AdjustRectTransformRelativeToViewport(zero, num2, tmp_CharacterInfo.isVisible);
			float num3 = zero.y + num2;
			float y = num3 - Mathf.Min(num2, this.m_TextComponent.rectTransform.rect.height);
			this.m_CursorVerts[0].position = new Vector3(zero.x, y, 0f);
			this.m_CursorVerts[1].position = new Vector3(zero.x, num3, 0f);
			this.m_CursorVerts[2].position = new Vector3(zero.x + num, num3, 0f);
			this.m_CursorVerts[3].position = new Vector3(zero.x + num, y, 0f);
			this.m_CursorVerts[0].color = this.caretColor;
			this.m_CursorVerts[1].color = this.caretColor;
			this.m_CursorVerts[2].color = this.caretColor;
			this.m_CursorVerts[3].color = this.caretColor;
			vbo.AddUIVertexQuad(this.m_CursorVerts);
			int height = Screen.height;
			zero.y = (float)height - zero.y;
			Input.compositionCursorPos = zero;
		}

		private void CreateCursorVerts()
		{
			this.m_CursorVerts = new UIVertex[4];
			for (int i = 0; i < this.m_CursorVerts.Length; i++)
			{
				this.m_CursorVerts[i] = UIVertex.simpleVert;
				this.m_CursorVerts[i].uv0 = Vector2.zero;
			}
		}

		private void GenerateHightlight(VertexHelper vbo, Vector2 roundingOffset)
		{
			TMP_TextInfo textInfo = this.m_TextComponent.textInfo;
			this.caretPositionInternal = this.GetCaretPositionFromStringIndex(this.stringPositionInternal);
			this.caretSelectPositionInternal = this.GetCaretPositionFromStringIndex(this.stringSelectPositionInternal);
			UnityEngine.Debug.Log(string.Concat(new object[]
			{
				"StringPosition:",
				this.stringPositionInternal,
				"  StringSelectPosition:",
				this.stringSelectPositionInternal
			}));
			Vector2 startPosition;
			float height;
			if (this.caretSelectPositionInternal < textInfo.characterCount)
			{
				startPosition = new Vector2(textInfo.characterInfo[this.caretSelectPositionInternal].origin, textInfo.characterInfo[this.caretSelectPositionInternal].descender);
				height = textInfo.characterInfo[this.caretSelectPositionInternal].ascender - textInfo.characterInfo[this.caretSelectPositionInternal].descender;
			}
			else
			{
				startPosition = new Vector2(textInfo.characterInfo[this.caretSelectPositionInternal - 1].xAdvance, textInfo.characterInfo[this.caretSelectPositionInternal - 1].descender);
				height = textInfo.characterInfo[this.caretSelectPositionInternal - 1].ascender - textInfo.characterInfo[this.caretSelectPositionInternal - 1].descender;
			}
			this.AdjustRectTransformRelativeToViewport(startPosition, height, true);
			int num = Mathf.Max(0, this.caretPositionInternal);
			int num2 = Mathf.Max(0, this.caretSelectPositionInternal);
			if (num > num2)
			{
				int num3 = num;
				num = num2;
				num2 = num3;
			}
			num2--;
			int num4 = (int)textInfo.characterInfo[num].lineNumber;
			int lastCharacterIndex = textInfo.lineInfo[num4].lastCharacterIndex;
			UIVertex simpleVert = UIVertex.simpleVert;
			simpleVert.uv0 = Vector2.zero;
			simpleVert.color = this.selectionColor;
			int num5 = num;
			while (num5 <= num2 && num5 < textInfo.characterCount)
			{
				if (num5 == lastCharacterIndex || num5 == num2)
				{
					TMP_CharacterInfo tmp_CharacterInfo = textInfo.characterInfo[num];
					TMP_CharacterInfo tmp_CharacterInfo2 = textInfo.characterInfo[num5];
					Vector2 vector = new Vector2(tmp_CharacterInfo.origin, tmp_CharacterInfo.ascender);
					Vector2 vector2 = new Vector2(tmp_CharacterInfo2.xAdvance, tmp_CharacterInfo2.descender);
					Vector2 min = this.m_TextViewport.rect.min;
					Vector2 max = this.m_TextViewport.rect.max;
					float num6 = this.m_TextComponent.rectTransform.anchoredPosition.x + vector.x - min.x;
					if (num6 < 0f)
					{
						vector.x -= num6;
					}
					float num7 = this.m_TextComponent.rectTransform.anchoredPosition.y + vector2.y - min.y;
					if (num7 < 0f)
					{
						vector2.y -= num7;
					}
					float num8 = max.x - (this.m_TextComponent.rectTransform.anchoredPosition.x + vector2.x);
					if (num8 < 0f)
					{
						vector2.x += num8;
					}
					float num9 = max.y - (this.m_TextComponent.rectTransform.anchoredPosition.y + vector.y);
					if (num9 < 0f)
					{
						vector.y += num9;
					}
					if (this.m_TextComponent.rectTransform.anchoredPosition.y + vector.y >= min.y && this.m_TextComponent.rectTransform.anchoredPosition.y + vector2.y <= max.y)
					{
						int currentVertCount = vbo.currentVertCount;
						simpleVert.position = new Vector3(vector.x, vector2.y, 0f);
						vbo.AddVert(simpleVert);
						simpleVert.position = new Vector3(vector2.x, vector2.y, 0f);
						vbo.AddVert(simpleVert);
						simpleVert.position = new Vector3(vector2.x, vector.y, 0f);
						vbo.AddVert(simpleVert);
						simpleVert.position = new Vector3(vector.x, vector.y, 0f);
						vbo.AddVert(simpleVert);
						vbo.AddTriangle(currentVertCount, currentVertCount + 1, currentVertCount + 2);
						vbo.AddTriangle(currentVertCount + 2, currentVertCount + 3, currentVertCount);
					}
					num = num5 + 1;
					num4++;
					if (num4 < textInfo.lineCount)
					{
						lastCharacterIndex = textInfo.lineInfo[num4].lastCharacterIndex;
					}
				}
				num5++;
			}
		}

		private void AdjustRectTransformRelativeToViewport(Vector2 startPosition, float height, bool isCharVisible)
		{
			float xMin = this.m_TextViewport.rect.xMin;
			float xMax = this.m_TextViewport.rect.xMax;
			float num = xMax - (this.m_TextComponent.rectTransform.anchoredPosition.x + startPosition.x + this.m_TextComponent.margin.z);
			if (num < 0f && (!this.multiLine || (this.multiLine && isCharVisible)))
			{
				this.m_TextComponent.rectTransform.anchoredPosition += new Vector2(num, 0f);
				this.AssignPositioningIfNeeded();
			}
			float num2 = this.m_TextComponent.rectTransform.anchoredPosition.x + startPosition.x - this.m_TextComponent.margin.x - xMin;
			if (num2 < 0f)
			{
				this.m_TextComponent.rectTransform.anchoredPosition += new Vector2(-num2, 0f);
				this.AssignPositioningIfNeeded();
			}
			if (this.m_LineType != TMP_InputField.LineType.SingleLine)
			{
				float num3 = this.m_TextViewport.rect.yMax - (this.m_TextComponent.rectTransform.anchoredPosition.y + startPosition.y + height);
				if (num3 < -0.0001f)
				{
					this.m_TextComponent.rectTransform.anchoredPosition += new Vector2(0f, num3);
					this.AssignPositioningIfNeeded();
				}
				float num4 = this.m_TextComponent.rectTransform.anchoredPosition.y + startPosition.y - this.m_TextViewport.rect.yMin;
				if (num4 < 0f)
				{
					this.m_TextComponent.rectTransform.anchoredPosition -= new Vector2(0f, num4);
					this.AssignPositioningIfNeeded();
				}
			}
			if (this.m_isLastKeyBackspace)
			{
				float num5 = this.m_TextComponent.rectTransform.anchoredPosition.x + this.m_TextComponent.textInfo.characterInfo[0].origin - this.m_TextComponent.margin.x;
				float num6 = this.m_TextComponent.rectTransform.anchoredPosition.x + this.m_TextComponent.textInfo.characterInfo[this.m_TextComponent.textInfo.characterCount - 1].origin + this.m_TextComponent.margin.z;
				if (this.m_TextComponent.rectTransform.anchoredPosition.x + startPosition.x <= xMin + 0.0001f)
				{
					if (num5 < xMin)
					{
						float x = Mathf.Min((xMax - xMin) / 2f, xMin - num5);
						this.m_TextComponent.rectTransform.anchoredPosition += new Vector2(x, 0f);
						this.AssignPositioningIfNeeded();
					}
				}
				else if (num6 < xMax && num5 < xMin)
				{
					float x2 = Mathf.Min(xMax - num6, xMin - num5);
					this.m_TextComponent.rectTransform.anchoredPosition += new Vector2(x2, 0f);
					this.AssignPositioningIfNeeded();
				}
				this.m_isLastKeyBackspace = false;
			}
		}

		protected char Validate(string text, int pos, char ch)
		{
			if (this.characterValidation == TMP_InputField.CharacterValidation.None || !base.enabled)
			{
				return ch;
			}
			if (this.characterValidation == TMP_InputField.CharacterValidation.Integer || this.characterValidation == TMP_InputField.CharacterValidation.Decimal)
			{
				bool flag = pos == 0 && text.Length > 0 && text[0] == '-';
				bool flag2 = this.stringPositionInternal == 0 || this.stringSelectPositionInternal == 0;
				if (!flag)
				{
					if (ch >= '0' && ch <= '9')
					{
						return ch;
					}
					if (ch == '-' && (pos == 0 || flag2))
					{
						return ch;
					}
					if (ch == '.' && this.characterValidation == TMP_InputField.CharacterValidation.Decimal && !text.Contains("."))
					{
						return ch;
					}
				}
			}
			else if (this.characterValidation == TMP_InputField.CharacterValidation.Alphanumeric)
			{
				if (ch >= 'A' && ch <= 'Z')
				{
					return ch;
				}
				if (ch >= 'a' && ch <= 'z')
				{
					return ch;
				}
				if (ch >= '0' && ch <= '9')
				{
					return ch;
				}
			}
			else if (this.characterValidation == TMP_InputField.CharacterValidation.Name)
			{
				char c = (text.Length <= 0) ? ' ' : text[Mathf.Clamp(pos, 0, text.Length - 1)];
				char c2 = (text.Length <= 0) ? '\n' : text[Mathf.Clamp(pos + 1, 0, text.Length - 1)];
				if (char.IsLetter(ch))
				{
					if (char.IsLower(ch) && c == ' ')
					{
						return char.ToUpper(ch);
					}
					if (char.IsUpper(ch) && c != ' ' && c != '\'')
					{
						return char.ToLower(ch);
					}
					return ch;
				}
				else if (ch == '\'')
				{
					if (c != ' ' && c != '\'' && c2 != '\'' && !text.Contains("'"))
					{
						return ch;
					}
				}
				else if (ch == ' ' && c != ' ' && c != '\'' && c2 != ' ' && c2 != '\'')
				{
					return ch;
				}
			}
			else if (this.characterValidation == TMP_InputField.CharacterValidation.EmailAddress)
			{
				if (ch >= 'A' && ch <= 'Z')
				{
					return ch;
				}
				if (ch >= 'a' && ch <= 'z')
				{
					return ch;
				}
				if (ch >= '0' && ch <= '9')
				{
					return ch;
				}
				if (ch == '@' && text.IndexOf('@') == -1)
				{
					return ch;
				}
				if ("!#$%&'*+-/=?^_`{|}~".IndexOf(ch) != -1)
				{
					return ch;
				}
				if (ch == '.')
				{
					char c3 = (text.Length <= 0) ? ' ' : text[Mathf.Clamp(pos, 0, text.Length - 1)];
					char c4 = (text.Length <= 0) ? '\n' : text[Mathf.Clamp(pos + 1, 0, text.Length - 1)];
					if (c3 != '.' && c4 != '.')
					{
						return ch;
					}
				}
			}
			return '\0';
		}

		public void ActivateInputField()
		{
			if (this.m_TextComponent == null || this.m_TextComponent.font == null || !this.IsActive() || !this.IsInteractable())
			{
				return;
			}
			if (this.isFocused && this.m_Keyboard != null && !this.m_Keyboard.active)
			{
				this.m_Keyboard.active = true;
				this.m_Keyboard.text = this.m_Text;
			}
			this.m_HasLostFocus = false;
			this.m_ShouldActivateNextUpdate = true;
		}

		private void ActivateInputFieldInternal()
		{
			if (EventSystem.current == null)
			{
				return;
			}
			if (EventSystem.current.currentSelectedGameObject != base.gameObject)
			{
				EventSystem.current.SetSelectedGameObject(base.gameObject);
			}
			if (TouchScreenKeyboard.isSupported)
			{
				if (Input.touchSupported)
				{
					TouchScreenKeyboard.hideInput = this.shouldHideMobileInput;
				}
				this.m_Keyboard = ((this.inputType != TMP_InputField.InputType.Password) ? TouchScreenKeyboard.Open(this.m_Text, this.keyboardType, this.inputType == TMP_InputField.InputType.AutoCorrect, this.multiLine) : TouchScreenKeyboard.Open(this.m_Text, this.keyboardType, false, this.multiLine, true));
				this.MoveTextEnd(false);
			}
			else
			{
				Input.imeCompositionMode = IMECompositionMode.On;
				this.OnFocus();
			}
			this.m_AllowInput = true;
			this.m_OriginalText = this.text;
			this.m_WasCanceled = false;
			this.SetCaretVisible();
			this.UpdateLabel();
		}

		public override void OnSelect(BaseEventData eventData)
		{
			base.OnSelect(eventData);
			this.ActivateInputField();
		}

		public virtual void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}
			this.ActivateInputField();
		}

		public void DeactivateInputField()
		{
			if (!this.m_AllowInput)
			{
				return;
			}
			this.m_HasDoneFocusTransition = false;
			this.m_AllowInput = false;
			if (this.m_Placeholder != null)
			{
				this.m_Placeholder.enabled = string.IsNullOrEmpty(this.m_Text);
			}
			if (this.m_TextComponent != null && this.IsInteractable())
			{
				if (this.m_WasCanceled)
				{
					this.text = this.m_OriginalText;
				}
				if (this.m_Keyboard != null)
				{
					this.m_Keyboard.active = false;
					this.m_Keyboard = null;
				}
				this.m_StringPosition = (this.m_StringSelectPosition = 0);
				this.m_TextComponent.rectTransform.localPosition = Vector3.zero;
				if (this.caretRectTrans != null)
				{
					this.caretRectTrans.localPosition = Vector3.zero;
				}
				this.SendOnSubmit();
				if (this.m_HasLostFocus)
				{
					this.SendOnFocusLost();
				}
				Input.imeCompositionMode = IMECompositionMode.Auto;
			}
			this.MarkGeometryAsDirty();
		}

		public override void OnDeselect(BaseEventData eventData)
		{
			UnityEngine.Debug.Log("OnDeselect()");
			this.m_HasLostFocus = true;
			this.DeactivateInputField();
			base.OnDeselect(eventData);
		}

		public virtual void OnSubmit(BaseEventData eventData)
		{
			UnityEngine.Debug.Log("OnSubmit()");
			if (!this.IsActive() || !this.IsInteractable())
			{
				return;
			}
			if (!this.isFocused)
			{
				this.m_ShouldActivateNextUpdate = true;
			}
		}

		private void EnforceContentType()
		{
			switch (this.contentType)
			{
			case TMP_InputField.ContentType.Standard:
				this.m_InputType = TMP_InputField.InputType.Standard;
				this.m_KeyboardType = TouchScreenKeyboardType.Default;
				this.m_CharacterValidation = TMP_InputField.CharacterValidation.None;
				return;
			case TMP_InputField.ContentType.Autocorrected:
				this.m_InputType = TMP_InputField.InputType.AutoCorrect;
				this.m_KeyboardType = TouchScreenKeyboardType.Default;
				this.m_CharacterValidation = TMP_InputField.CharacterValidation.None;
				return;
			case TMP_InputField.ContentType.IntegerNumber:
				this.m_LineType = TMP_InputField.LineType.SingleLine;
				this.m_TextComponent.enableWordWrapping = false;
				this.m_InputType = TMP_InputField.InputType.Standard;
				this.m_KeyboardType = TouchScreenKeyboardType.NumberPad;
				this.m_CharacterValidation = TMP_InputField.CharacterValidation.Integer;
				return;
			case TMP_InputField.ContentType.DecimalNumber:
				this.m_LineType = TMP_InputField.LineType.SingleLine;
				this.m_TextComponent.enableWordWrapping = false;
				this.m_InputType = TMP_InputField.InputType.Standard;
				this.m_KeyboardType = TouchScreenKeyboardType.NumbersAndPunctuation;
				this.m_CharacterValidation = TMP_InputField.CharacterValidation.Decimal;
				return;
			case TMP_InputField.ContentType.Alphanumeric:
				this.m_LineType = TMP_InputField.LineType.SingleLine;
				this.m_TextComponent.enableWordWrapping = false;
				this.m_InputType = TMP_InputField.InputType.Standard;
				this.m_KeyboardType = TouchScreenKeyboardType.ASCIICapable;
				this.m_CharacterValidation = TMP_InputField.CharacterValidation.Alphanumeric;
				return;
			case TMP_InputField.ContentType.Name:
				this.m_LineType = TMP_InputField.LineType.SingleLine;
				this.m_TextComponent.enableWordWrapping = false;
				this.m_InputType = TMP_InputField.InputType.Standard;
				this.m_KeyboardType = TouchScreenKeyboardType.Default;
				this.m_CharacterValidation = TMP_InputField.CharacterValidation.Name;
				return;
			case TMP_InputField.ContentType.EmailAddress:
				this.m_LineType = TMP_InputField.LineType.SingleLine;
				this.m_TextComponent.enableWordWrapping = false;
				this.m_InputType = TMP_InputField.InputType.Standard;
				this.m_KeyboardType = TouchScreenKeyboardType.EmailAddress;
				this.m_CharacterValidation = TMP_InputField.CharacterValidation.EmailAddress;
				return;
			case TMP_InputField.ContentType.Password:
				this.m_LineType = TMP_InputField.LineType.SingleLine;
				this.m_TextComponent.enableWordWrapping = false;
				this.m_InputType = TMP_InputField.InputType.Password;
				this.m_KeyboardType = TouchScreenKeyboardType.Default;
				this.m_CharacterValidation = TMP_InputField.CharacterValidation.None;
				return;
			case TMP_InputField.ContentType.Pin:
				this.m_LineType = TMP_InputField.LineType.SingleLine;
				this.m_TextComponent.enableWordWrapping = false;
				this.m_InputType = TMP_InputField.InputType.Password;
				this.m_KeyboardType = TouchScreenKeyboardType.NumberPad;
				this.m_CharacterValidation = TMP_InputField.CharacterValidation.Integer;
				return;
			default:
				return;
			}
		}

		private void SetTextComponentWrapMode()
		{
			if (this.m_TextComponent == null)
			{
				return;
			}
			if (this.m_LineType == TMP_InputField.LineType.SingleLine)
			{
				this.m_TextComponent.enableWordWrapping = false;
			}
			else
			{
				this.m_TextComponent.enableWordWrapping = true;
			}
		}

		private void SetTextComponentRichTextMode()
		{
			if (this.m_TextComponent == null)
			{
				return;
			}
			this.m_TextComponent.richText = this.m_RichText;
		}

		private void SetToCustomIfContentTypeIsNot(params TMP_InputField.ContentType[] allowedContentTypes)
		{
			if (this.contentType == TMP_InputField.ContentType.Custom)
			{
				return;
			}
			for (int i = 0; i < allowedContentTypes.Length; i++)
			{
				if (this.contentType == allowedContentTypes[i])
				{
					return;
				}
			}
			this.contentType = TMP_InputField.ContentType.Custom;
		}

		private void SetToCustom()
		{
			if (this.contentType == TMP_InputField.ContentType.Custom)
			{
				return;
			}
			this.contentType = TMP_InputField.ContentType.Custom;
		}

		protected override void DoStateTransition(Selectable.SelectionState state, bool instant)
		{
			if (this.m_HasDoneFocusTransition)
			{
				state = Selectable.SelectionState.Highlighted;
			}
			else if (state == Selectable.SelectionState.Pressed)
			{
				this.m_HasDoneFocusTransition = true;
			}
			base.DoStateTransition(state, instant);
		}

	
		bool ICanvasElement.IsDestroyed()
		{
			return base.IsDestroyed();
		}

		public TouchScreenKeyboard m_Keyboard;

		private static readonly char[] kSeparators = new char[]
		{
			' ',
			'.',
			',',
			'\t',
			'\r',
			'\n'
		};

		[SerializeField]
		protected RectTransform m_TextViewport;

		[SerializeField]
		protected TMP_Text m_TextComponent;

		protected RectTransform m_TextComponentRectTransform;

		[SerializeField]
		protected Graphic m_Placeholder;

		[SerializeField]
		private TMP_InputField.ContentType m_ContentType;

		[SerializeField]
		private TMP_InputField.InputType m_InputType;

		[SerializeField]
		private char m_AsteriskChar = '*';

		[SerializeField]
		private TouchScreenKeyboardType m_KeyboardType;

		[SerializeField]
		private TMP_InputField.LineType m_LineType;

		[SerializeField]
		private bool m_HideMobileInput;

		[SerializeField]
		private TMP_InputField.CharacterValidation m_CharacterValidation;

		[SerializeField]
		private int m_CharacterLimit;

		[SerializeField]
		private TMP_InputField.SubmitEvent m_OnEndEdit = new TMP_InputField.SubmitEvent();

		[SerializeField]
		private TMP_InputField.SubmitEvent m_OnSubmit = new TMP_InputField.SubmitEvent();

		[SerializeField]
		private TMP_InputField.SubmitEvent m_OnFocusLost = new TMP_InputField.SubmitEvent();

		[SerializeField]
		private TMP_InputField.OnChangeEvent m_OnValueChanged = new TMP_InputField.OnChangeEvent();

		[SerializeField]
		private TMP_InputField.OnValidateInput m_OnValidateInput;

		[SerializeField]
		private Color m_CaretColor = new Color(0.196078435f, 0.196078435f, 0.196078435f, 1f);

		[SerializeField]
		private bool m_CustomCaretColor;

		[SerializeField]
		private Color m_SelectionColor = new Color(0.65882355f, 0.807843149f, 1f, 0.7529412f);

		[SerializeField]
		protected string m_Text = string.Empty;

		[Range(0f, 4f)]
		[SerializeField]
		private float m_CaretBlinkRate = 0.85f;

		[Range(1f, 5f)]
		[SerializeField]
		private int m_CaretWidth = 1;

		[SerializeField]
		private bool m_ReadOnly;

		[SerializeField]
		private bool m_RichText = true;

		protected int m_StringPosition;

		protected int m_StringSelectPosition;

		protected int m_CaretPosition;

		protected int m_CaretSelectPosition;

		private RectTransform caretRectTrans;

		protected UIVertex[] m_CursorVerts;

		private CanvasRenderer m_CachedInputRenderer;

		[NonSerialized]
		protected Mesh m_Mesh;

		private bool m_AllowInput;

		private bool m_HasLostFocus;

		private bool m_ShouldActivateNextUpdate;

		private bool m_UpdateDrag;

		private bool m_DragPositionOutOfBounds;

		private const float kHScrollSpeed = 0.05f;

		private const float kVScrollSpeed = 0.1f;

		protected bool m_CaretVisible;

		private Coroutine m_BlinkCoroutine;

		private float m_BlinkStartTime;

		protected int m_DrawStart;

		protected int m_DrawEnd;

		private Coroutine m_DragCoroutine;

		private string m_OriginalText = string.Empty;

		private bool m_WasCanceled;

		private bool m_HasDoneFocusTransition;

		private bool m_isLastKeyBackspace;

		private const string kEmailSpecialCharacters = "!#$%&'*+-/=?^_`{|}~";

		private bool isCaretInsideTag;

		private Event m_ProcessingEvent = new Event();

		public enum ContentType
		{
			Standard,
			Autocorrected,
			IntegerNumber,
			DecimalNumber,
			Alphanumeric,
			Name,
			EmailAddress,
			Password,
			Pin,
			Custom
		}

		public enum InputType
		{
			Standard,
			AutoCorrect,
			Password
		}

		public enum CharacterValidation
		{
			None,
			Integer,
			Decimal,
			Alphanumeric,
			Name,
			EmailAddress
		}

		public enum LineType
		{
			SingleLine,
			MultiLineSubmit,
			MultiLineNewline
		}

		public delegate char OnValidateInput(string text, int charIndex, char addedChar);

		[Serializable]
		public class SubmitEvent : UnityEvent<string>
		{
		}

		[Serializable]
		public class OnChangeEvent : UnityEvent<string>
		{
		}

		protected enum EditState
		{
			Continue,
			Finish
		}
	}
}
