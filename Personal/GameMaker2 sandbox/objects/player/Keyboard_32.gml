if(spaceUp && floorJumpSteps == maxFloorJumpSteps && grounded) {
    spaceUp = false;
	vertSpeed = jumpImpulse;
    floorJumpSteps--;
	grounded = false;
} else if(spaceUp && wallJumpSteps == maxDoubleJumpSteps) {
    spaceUp = false;
	vertSpeed = jumpImpulse;
    wallJumpSteps--;
	if (wallJumpSide == 1) {
		impulseX = -wallJumpImpulse;
		ignoreX = true;
	} 
	if (wallJumpSide == 2) {
		impulseX = wallJumpImpulse;
		ignoreX = true;
	}
} else if(spaceUp && doubleJumpSteps == maxDoubleJumpSteps) {
    spaceUp = false;
	vertSpeed = jumpImpulse;
    doubleJumpSteps--;
} else if (floorJumpSteps > 0 && floorJumpSteps < maxFloorJumpSteps) {
  vertSpeed += jumpHeld * (0.02 * (floorJumpSteps * floorJumpSteps));
  floorJumpSteps--;
}  else if (wallJumpSteps > 0 && wallJumpSteps < maxDoubleJumpSteps) {
  vertSpeed += jumpHeld * (0.02 * (wallJumpSteps * wallJumpSteps));
  if(impulseX * impulseX > 0.25){
      impulseX += impulseX / -5;
  } else {
	impulseX = 0;  
  }
  wallJumpSteps--;
  if (wallJumpSteps == 0) {
	  impulseX = 0;
	  ignoreX = false;
  }
} else if (doubleJumpSteps > 0 && doubleJumpSteps < maxDoubleJumpSteps) {
  vertSpeed += jumpHeld * (0.02 * (doubleJumpSteps * doubleJumpSteps));
  doubleJumpSteps--;
}
