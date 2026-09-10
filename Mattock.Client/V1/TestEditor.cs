using Godot;
using Mattock.Core.Scripts;
using Newtonsoft.Json;
using ScriptEditor.Core;
using System.Collections.Generic;
using System.Linq;

// using System.Linq;

namespace Mattock.Client.V1;

public partial class TestEditor 
	: Control
{
	[Export]
	public PackedScene EditorScene { get; set; }

	[Export]
	public string Directory { get; set; }

	[Export]
	public string BaseFile { get; set; }
	
	public override void _Ready()
	{
		var list = GetNode<ItemList>("%List");

		foreach (var file in System.IO.Directory.GetFiles(Directory))
		{
			list.AddItem(file);
		}

		var tabs = GetNode<TabContainer>("%Tabs");
		tabs.GetTabBar().CloseWithMiddleMouse = true;
		tabs.GetTabBar().TabCloseDisplayPolicy = TabBar.CloseButtonDisplayPolicy.ShowAlways;
		tabs.GetTabBar().TabClosePressed += (tabIdx) => tabs.GetChild((int)tabIdx).QueueFree();
	}

	public void OnAddButtonPressed()
	{
		var edit = GetNode<LineEdit>("%NewCardNameEdit");
		var name = edit.Text;
		edit.Clear();
		if (string.IsNullOrEmpty(name)) return;

		var baseFile = FileAccess.Open(BaseFile, FileAccess.ModeFlags.Read);
		var data = baseFile.GetAsText();
		baseFile.Close();

		var path = System.IO.Path.Join(Directory, $"{name}.script.json");
		var file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
		file.StoreString(data);
		file.Close();

		GetNode<ItemList>("%List").AddItem(path);
	}

	public void OnListItemActivated(int idx)
	{
		var path = GetNode<ItemList>("%List").GetItemText(idx);

		var item = path
			.Replace($"{Directory}\\", "")
			.Replace(".script.json", "");

		var tabs = GetNode<TabContainer>("%Tabs");
		foreach (var tab in tabs.GetChildren().Cast<ScriptEditorDisplay>())
		{
			if (tab.Name == item)
			{
				tab.Show();
				return;
			}
		}

		var newTab = EditorScene.Instantiate<ScriptEditorDisplay>();
		newTab.Name = item;
		GetNode<TabContainer>("%Tabs").AddChild(newTab);

		newTab.ObjectColors = new() {
			{ "Effect", new Color(1, 1, 0, 1) },
			{ "SingleEffect", new Color(1, 0.8f, 0.3f, 1) },
			{ "Target", new Color(1, 0, 1, 1) },
			{ "Target.Amount", new Color(0.6f, 0.1f, 0.1f, 1) },
			// { "Effect", new Color(0, 1, 1, 1) },
			{ "PlayerFilter", new Color(0, 1, 0, 1) },
			{ "PlayerMany", new Color(0, 0.66f, 0, 1) },
			{ "PlayerSelect", new Color(0, 0.33f, 0, 1) },
			// { "GigSelect", new Color(0, 0.66f, 0, 1) },
			// { "GigSingle", new Color(0, 0.33f, 0, 1) },
			// { "InPlayCardFilter", new Color(1, 0.64705884f, 0, 1) },
			// { "InPlayCardSelect", new Color(0.60f, 0.40f, 0, 1) },
			// { "InPlayCardSingle", new Color(0.3071876f, 0.18602094f, 0, 1) },
			// { "MatchCardFilter", new Color(1, 0, 0, 1) },
			// { "MatchCardSelect", new Color(0.66f, 0, 0, 1) },
			// { "MatchCardSingle", new Color(0.33f, 0, 0, 1) },
			{ "ActivatedAbility", new Color(0.75f, 0.75f, 0.75f, 1) },
			{ "ActivatedAbilityCollection", new Color(0.5f, 0.5f, 0.5f, 1) },
			{ "ActivatedManaAbility", new Color(0.25f, 0.25f, 0.25f, 1) },
			{ "ActivatedManaAbilityCollection", new Color(0.1f, 0.1f, 0.1f, 1) },
			{ "Number", new Color(0.6f, 0.1f, 0.9f, 1) },
			// { "PlayerFilter", new Color(0, 0, 1, 1) },
			// { "PlayerSelect", new Color(0, 0, 0.66f, 1) },
			// { "PlayerSingle", new Color(0, 0, 0.33f, 1) },
			{ "Cost", new Color(0, 0.5f, 0.5f, 1) },
			{ "CostCollection", new Color(0, 0.1f, 0.1f, 1) },
		};
		newTab.LoadScriptNodes(ScriptLoader.ScriptNodes);

		newTab.StateChanged += () =>
		{
			var state = newTab.State;
			var data = JsonConvert.SerializeObject(state, Formatting.Indented);
			var file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
			file.StoreString(data);
			file.Close();
		};

		var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
		var data = file.GetAsText();

		var state = ScriptLoader.Deserialize(data);
		newTab.LoadState(state);
		file.Close();

		newTab.Show();
	}
}
