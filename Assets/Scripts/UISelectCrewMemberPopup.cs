using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UISelectCrewMemberPopup : MonoBehaviour
{
	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<int, Skill> OnCrewHasBeenSelected;

	public List<Skill> SelectedCrewMembers
	{
		get
		{
			return (from x in this.alreadySelectedCrews.Values
			where x != null
			select x.Crew).ToList<Skill>();
		}
	}

	public void RefreshAllAvailableCrewMembers()
	{
		IList<Skill> crewMembers = SkillManager.Instance.CrewMembers;
		foreach (Skill crew in crewMembers)
		{
			UISelectCrewMemberPopup.UICrewIconItem uicrewIconItem = this.CreateCrewMemberGridItem(crew);
			uicrewIconItem.Icon.transform.SetParent(this.gridContent, false);
			uicrewIconItem.Icon.SetActive(false);
			this.allAvailableCrewMembersAsUIItems.Add(uicrewIconItem);
		}
	}

	private void UpdateGrid()
	{
		List<Skill> selectedCrewMembers = this.SelectedCrewMembers;
		foreach (UISelectCrewMemberPopup.UICrewIconItem uicrewIconItem in this.allAvailableCrewMembersAsUIItems)
		{
			uicrewIconItem.Icon.SetActive(uicrewIconItem.ShowInGrid);
			bool selected = selectedCrewMembers.Contains(uicrewIconItem.Crew);
			uicrewIconItem.SetSelected(selected);
		}
	}

	public void Show(int index)
	{
		if (this.selectionIndex == index && base.gameObject.activeSelf)
		{
			this.Hide();
			return;
		}
		base.gameObject.SetActive(true);
		this.selectionIndex = index;
		this.UpdateGrid();
	}

	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	public UISelectCrewMemberPopup.UICrewIconItem CreateCrewMemberGridItem(Skill crew)
	{
		GameObject gameObject = new GameObject("icon");
		gameObject.AddComponent<Image>();
		gameObject.AddComponent<Button>();
		return new UISelectCrewMemberPopup.UICrewIconItem(gameObject, crew, new Action<UISelectCrewMemberPopup.UICrewIconItem>(this.OnCrewSelected));
	}

	public void SetSelected(int selectionIndex, Skill crew)
	{
		this.selectionIndex = selectionIndex;
		UISelectCrewMemberPopup.UICrewIconItem uicrewIconItem = this.allAvailableCrewMembersAsUIItems.Find((UISelectCrewMemberPopup.UICrewIconItem x) => x.Crew == crew);
		if (uicrewIconItem != null)
		{
			this.OnCrewSelected(uicrewIconItem);
		}
	}

	private void OnCrewSelected(UISelectCrewMemberPopup.UICrewIconItem selectedItem)
	{
		this.alreadySelectedCrews[this.selectionIndex] = selectedItem;
		this.Hide();
		if (this.OnCrewHasBeenSelected != null)
		{
			this.OnCrewHasBeenSelected(this.selectionIndex, selectedItem.Crew);
		}
	}

	private bool IsSelected(UISelectCrewMemberPopup.UICrewIconItem item)
	{
		foreach (KeyValuePair<int, UISelectCrewMemberPopup.UICrewIconItem> keyValuePair in this.alreadySelectedCrews)
		{
			if (keyValuePair.Value != null && item.Crew == keyValuePair.Value.Crew)
			{
				return true;
			}
		}
		return false;
	}

	[SerializeField]
	private Transform gridContent;

	private List<UISelectCrewMemberPopup.UICrewIconItem> allAvailableCrewMembersAsUIItems = new List<UISelectCrewMemberPopup.UICrewIconItem>();

	private List<UISelectCrewMemberPopup.UICrewIconItem> crewMembersInGrid = new List<UISelectCrewMemberPopup.UICrewIconItem>();

	private Dictionary<int, UISelectCrewMemberPopup.UICrewIconItem> alreadySelectedCrews = new Dictionary<int, UISelectCrewMemberPopup.UICrewIconItem>
	{
		{
			0,
			null
		},
		{
			1,
			null
		},
		{
			2,
			null
		}
	};

	private int selectionIndex;

	public class UICrewIconItem
	{
		public UICrewIconItem(GameObject icon, Skill crew, Action<UISelectCrewMemberPopup.UICrewIconItem> onSelected)
		{
			UISelectCrewMemberPopup.UICrewIconItem _0024this = this;
			this.Icon = icon;
			this.Crew = crew;
			this.Img = icon.GetComponent<Image>();
			this.Btn = icon.GetComponent<Button>();
			this.Img.sprite = crew.GetExtraInfo().Icon;
			this.Btn.onClick.AddListener(delegate()
			{
				if (onSelected != null)
				{
					onSelected(_0024this);
				}
			});
		}

		public bool ShowInGrid
		{
			get
			{
				return this.Crew.CurrentLevel > 0;
			}
		}

		public void SetSelected(bool selected)
		{
			this.Btn.interactable = !selected;
		}

		public GameObject Icon;

		public Skill Crew;

		public Button Btn;

		public Image Img;
	}
}
