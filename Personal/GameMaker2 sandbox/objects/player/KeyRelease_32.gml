spaceUp = true;
ignoreX = false;
vertspeed = 0;
if (floorJumpSteps < maxFloorJumpSteps) {
    floorJumpSteps = 0;
}
if (wallJumpSteps < maxDoubleJumpSteps) {
    wallJumpSteps = 0;
	impulseX = 0;
}
if (doubleJumpSteps < maxDoubleJumpSteps) {
    doubleJumpSteps = 0;
}
