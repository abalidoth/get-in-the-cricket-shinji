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

	bool Active;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (!InputMap.HasAction(InputActionName))
		{
			InputMap.AddAction(InputActionName);
		} 
		else
		{
			InputMap.ActionEraseEvents(InputActionName);
		}
		InputMap.ActionAddEvent(InputActionName, InitialBinding);
		Text = CurrentKeybind = InitialBinding.AsText();
		GameSettings = GetNode("/root/GameSettings");
		GameSettings.Connect("keybind_changed", new Callable(this, MethodName.OnBindingChanged));
		Active = false;
		GD.Print(CurrentKeybind + " Ready");

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
			Text = CurrentKeybind = inputEventKey.AsText();
			InputMap.ActionEraseEvents(InputActionName);
			InputMap.ActionAddEvent(InputActionName, inputEventKey);
			GameSettings.EmitSignal("keybind_changed", CurrentKeybind);
			Active = false;
			GD.Print(InputActionName + " Changed");  
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
			if (NewBinding != CurrentKeybind) 
			{
				Active = false;
				Text = CurrentKeybind;
			}
			return;
		}
		if (NewBinding == CurrentKeybind)
		{
			InputMap.ActionEraseEvents(InputActionName);
			CurrentKeybind = "---";
			Text = "---";
		}
	}





}
