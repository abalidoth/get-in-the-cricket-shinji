extends Node2D

const UP= Vector2i(0,1)
const DOWN = Vector2i(0,-1)
const LEFT = Vector2i(-1,0)
const RIGHT = Vector2i(1,0)

const ROCK = Vector2i(0,0)
const STICK = Vector2i(1,0)
const EMPTY = Vector2i(2,0)
const SHORE = Vector2i(3,0)
const WATER = Vector2i(4,0)

signal win_game()

var cricket_pos := Vector2i(5,11)
var cricket_facing := Vector2i(0,-1)

@export var debug := true

var cricket_go = false

var worm_steps := 0

func set_cricket_pos(pos: Vector2i):
	cricket_pos = pos
	%Cricket.position = $CricketMap.map_to_local(pos)
	
func cricket_forward():
	var target = cricket_pos + cricket_facing
	var target_sq = $CricketMap.get_cell_atlas_coords(0,target)
	match target_sq:
		EMPTY:
			set_cricket_pos(target)
		SHORE:
			set_cricket_pos(target)
		WATER:
			set_cricket_pos(target)
			win_game.emit()
		_: #crash
			cricket_stop()
			
func cricket_left():
	cricket_facing = Vector2i(cricket_facing.y, -cricket_facing.x)
	%Cricket.frame = posmod((%Cricket.frame - 1), 4)

func cricket_right():
	cricket_facing = Vector2i(-cricket_facing.y, cricket_facing.x)
	%Cricket.frame = posmod((%Cricket.frame + 1), 4)

func cricket_jump():
	var obstacle = cricket_pos + cricket_facing
	var target = obstacle + cricket_facing
	var obs_sq = $CricketMap.get_cell_atlas_coords(0,obstacle)
	var target_sq = $CricketMap.get_cell_atlas_coords(0,target)
	if (obs_sq != ROCK):
		match target_sq:
			EMPTY:
				set_cricket_pos(target)
			SHORE:
				set_cricket_pos(target)
			WATER:
				set_cricket_pos(target)
				win_game.emit()
		


func make_maze(height=11, stick_percentage = 0.5):
	##idea: start with random grid of sticks and rocks
	##remove a tile, then check whether all removed nodes are connected
	##once they are, stop
	var rock_rows = range(height)
	rock_rows.shuffle()
	var rock_cols = range(height)
	rock_cols.shuffle()
	for i in range(height):
		$CricketMap.set_cell(
			0,
			Vector2i(rock_rows[i], rock_cols[i]),
			0,
			ROCK
		)
	var stick_rows = range(height) + range(height)
	stick_rows.shuffle()
	var stick_cols = range(height) +range(height)
	stick_cols.shuffle()
	for i in range(2*height):
		$CricketMap.set_cell(
			0,
			Vector2i(stick_rows[i], stick_cols[i]),
			0,
			STICK
		)
	
func _ready():
	make_maze()
	set_cricket_pos(cricket_pos)
	$CricketMap.set_cell(
			0,
			cricket_pos,
			0,
			EMPTY
		)
	
func _input(event):
	if event is InputEventKey and event.pressed and debug:
		match event.keycode:
			KEY_W:
				cricket_forward()
			KEY_A:
				cricket_left()
			KEY_S:
				cricket_jump()
			KEY_D:
				cricket_right()
	

func cricket_stop():
	cricket_go = false
	worm_steps = 0

func _on_worm_world_go():
	cricket_go = true
	worm_steps = 1


func _on_worm_world_jump():
	cricket_jump()


func _on_worm_world_left():
	cricket_left()


func _on_worm_world_on_move():
	if cricket_go:
		worm_steps += 1
	if worm_steps == GameSettings.worm_ticks:
		print(worm_steps)
		cricket_forward()
		worm_steps = 0


func _on_worm_world_right():
	cricket_right()


func _on_worm_world_step():
	cricket_forward()


func _on_worm_world_stop():
	cricket_stop()

