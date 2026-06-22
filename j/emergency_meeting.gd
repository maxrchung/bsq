extends Node2D
@onready var click_fx: AudioStreamPlayer = $click_fx
@onready var emergency_btn_fx: AudioStreamPlayer = $emergency_btn_fx

signal emergency
signal vote_bs
signal vote_no_bs

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	reset()

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass

func reset():
	$NotBsLabel.text = "Not BS: " + str(0)
	$BsLabel.text = "BS: " + str(0)
	
	$EmergencyButton.visible = false
	$NoBsButton.visible = false
	$NotBsLabel.visible = false
	$BsButton.visible = false
	$BsLabel.visible = false

func update_button(player_id, bidder_id):
	if player_id == bidder_id:
		$EmergencyButton.visible = false
	else:
		$EmergencyButton.visible = true

func update_state(player_id, bidder_id, emergency_meeting):
	var is_bidder = player_id == bidder_id
	
	var votes_for = emergency_meeting.votesFor
	var votes_against = emergency_meeting.votesAgainst
	
	var has_voted = false
	
	for vote in votes_for:
		if vote == player_id:
			has_voted = true
			break
			
	for vote in votes_against:
		if vote == player_id:
			has_voted = true
			break
		
		
	$EmergencyButton.visible = false
	$NoBsButton.visible = true
	$BsButton.visible = true
	$NotBsLabel.visible = true
	$BsLabel.visible = true
	
	$NotBsLabel.text = "Not BS: " + str(votes_for.size())
	$BsLabel.text = "BS: " + str(votes_against.size())
		
	if is_bidder or has_voted:
		$NoBsButton.visible = false
		$BsButton.visible = false

func on_emergency_press():
	emergency_btn_fx.play()
	emergency.emit()

func on_not_bs_press():
	click_fx.play()
	vote_no_bs.emit()
	
func on_bs_press():
	click_fx.play()
	vote_bs.emit()
