extends Control


# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass




func _on_challenge_speed_slider_value_changed(value):
	%ChallengeModeSpeed.text = "Move Speed (%s steps/sec)" % value


func _on_check_box_toggled(toggled_on):
	if toggled_on:
		%ChallengeSpeedSlider.editable = true
		%ChallengeModeSpeed.text = "Move Speed (%s steps/sec)" % %ChallengeSpeedSlider.value
	else:
		%ChallengeSpeedSlider.editable = false
		%ChallengeModeSpeed.text = "Move Speed (N/A)"
		


func _on_worm_ticks_slider_value_changed(value):
	%WormTicksLabel.text = "%s Worm move(s) per cricket move" % value


func _on_start_game_pressed():
	GameSettings.challenge_mode = %CheckBox.button_pressed
	GameSettings.challenge_speed = %ChallengeSpeedSlider.value
	GameSettings.worm_ticks = %WormTicksSlider.value
	get_tree().change_scene_to_file("res://main_game.tscn")
