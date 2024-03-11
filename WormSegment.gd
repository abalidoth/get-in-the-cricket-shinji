extends Sprite2D

const HEX_SE = Vector3(0,1,-1)
const HEX_E = Vector3(1,0,-1)
const HEX_NE = Vector3(1,-1,0)
const HEX_NW = Vector3(0,-1,1)
const HEX_W= Vector3(-1,0,1)
const HEX_SW = Vector3(-1,1,0)

const TX_S = 96

const diff_to_coord = {
	HEX_SE:0,
	HEX_E:1,
	HEX_NE:2,
	HEX_NW:3,
	HEX_W:4,
	HEX_SW:5
}

func set_tail_segment(diff):
	var c1 = diff_to_coord[diff]
	texture.region = Rect2(6*TX_S, c1*TX_S, TX_S, TX_S)
	
func set_head_segment(diff):
	var c2 = diff_to_coord[diff]
	texture.region = Rect2(c2*TX_S, 6*TX_S, TX_S, TX_S)

func set_bodyhey_segment(diff1, diff2):
	var c1 = diff_to_coord[diff1]
	var c2 = diff_to_coord[diff2]
	texture.region = Rect2(c2*TX_S, c1*TX_S, TX_S, TX_S)
