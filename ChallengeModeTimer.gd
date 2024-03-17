extends Timer
# Called when the node enters the scene tree for the first time.
func _ready():
	set_paused(GameSettings.challenge_mode)
	set_wait_time ( 1 / GameSettings.challenge_speed)
