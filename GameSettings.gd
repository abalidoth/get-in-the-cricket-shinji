extends Node

@export var challenge_mode := false
@export var challenge_speed := 1
@export var worm_ticks := 4
@export var seed := 1
#and then the keyboard bindings

signal updated()


# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.

func update():
	updated.emit()

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass
