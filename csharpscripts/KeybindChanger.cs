using Godot;
using System.Linq;

public partial class KeybindChanger : Button
{
	[Export]
	string InputActionName;
        [Export]
	InputEventKey InitialBinding;
	string CurrentKeybind;
	Node GameSettings;

	bool Active = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		InputMap.ActionAddEvent(InputActionName, InitialBinding)
		Text = CurrentKeybind = InitialBinding.AsTextKeyLabel();
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
                        GameSettings.EmitSignal("keybind_changed", CurrentKeybind); 
		}
	}

	 public void OnBindingChanged(string NewBinding)
	{
		if (Active)
		{
			Active = false;
                        if (NewBinding != CurrentKeybind) Text = CurrentKeybind;
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
