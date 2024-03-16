extends Sprite2D

const HEX_SE = Vector3i(0,1,-1)
const HEX_E = Vector3i(1,0,-1)
const HEX_NE = Vector3i(1,-1,0)
const HEX_NW = Vector3i(0,-1,1)
const HEX_W= Vector3i(-1,0,1)
const HEX_SW = Vector3i(-1,1,0)

const TX_S = 32

const diff_to_coord = {
	HEX_SE:0,
	HEX_E:1,
	HEX_NE:2,
	HEX_NW:3,
	HEX_W:4,
	HEX_SW:5
}

func set_tail_segment(target_diff):
	var c1 = diff_to_coord[target_diff]
	texture.region = Rect2(6*TX_S, c1*TX_S, TX_S, TX_S)
	
func set_head_segment(source_diff):
	var c2 = diff_to_coord[source_diff]
	texture.region = Rect2(c2*TX_S, 6*TX_S, TX_S, TX_S)

func set_middle_segment(target_diff, source_diff):
	var c1 = diff_to_coord[target_diff]
	var c2 = diff_to_coord[source_diff]
	texture.region = Rect2(c2*TX_S, c1*TX_S, TX_S, TX_S)
