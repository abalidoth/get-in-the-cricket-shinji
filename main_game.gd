extends Node2D

var game_over := false

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass

func _input(event):
	if event is InputEventKey and game_over and $GameOverTimer.is_stopped():
		
		get_tree().change_scene_to_file("res://main_menu.tscn")

func _on_cricket_world_win_game():
	$WinRect.visible=true
	game_over = true
	$GameOverTimer.start()
	

func _on_worm_world_collision():
	if GameSettings.challenge_mode:
		$LossRect.visible = true
		game_over = true
		$GameOverTimer.start()
