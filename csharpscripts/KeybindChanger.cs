using Godot;
using System.Linq;

public partial class KeybindChanger : Button
{
	[Export]
	string InputActionName;
	string CurrentKeybind;
	Node GameSettings;

	bool Active = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		CurrentKeybind = (InputMap.ActionGetEvents(InputActionName).First() as InputEventKey).AsTextKeyLabel(); ;
		Text = CurrentKeybind;
		GameSettings = GetNode("/root/GameSettings");

	}

	public override void _Input(InputEvent @event)
	{
		if (!Active) return;
		if (@event is InputEventKey inputEventKey && @event.IsPressed())
		{
			
			if (inputEventKey.Keycode == Key.Escape)
			{
				Active = false;
				Text = CurrentKeybind;
				return;
			}
			CurrentKeybind = inputEventKey.AsTextKeyLabel();
			InputMap.ActionEraseEvents(InputActionName);
			InputMap.ActionAddEvent(InputActionName, inputEventKey);
			GameSettings.EmitSignal("keybind_changed", CurrentKeybind);  
			Text = CurrentKeybind;
		}
	}

	public override void _Pressed()
	{
		if (Active)
		{
			Active = false;
			Text = CurrentKeybind;
		}
		else
		{
			Active = true;
			Text = "???";
		}
	}

	public void OnBindingChanged(string NewBinding)
	{
		if (Active)
		{
			Active = false;
			return;
		}
		if (NewBinding == CurrentKeybind)
		{
			InputMap.ActionEraseEvents(InputActionName);
			CurrentKeybind = "";
			Text = "";
		}
	}





}
