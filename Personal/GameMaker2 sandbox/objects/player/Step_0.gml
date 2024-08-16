//@description Step info
var speedX = ignoreX ? impulseX + dashImpulse : moveSpeed * (keyboard_check(vk_right) - keyboard_check(vk_left)) + impulseX;
var speedY = vertSpeed;

function groundCollision(tileMap, isOneWay, isBreakable, objSpeed) {
    var br = tilemap_get_at_pixel(tileMap, bbox_right, ceil(bbox_bottom) + 1) & tile_index_mask;
	var bl = tilemap_get_at_pixel(tileMap, bbox_left, ceil(bbox_bottom) + 1) & tile_index_mask;
	var bCenter = tilemap_get_at_pixel(tileMap, bbox_left + 12, ceil(bbox_bottom) + 1) & tile_index_mask;
	
	
	var oneWayBr = tilemap_get_at_pixel(tileMap, bbox_right, ceil(bbox_bottom) - objSpeed) & tile_index_mask;
	var oneWayBl = tilemap_get_at_pixel(tileMap, bbox_left, ceil(bbox_bottom) - objSpeed) & tile_index_mask;
	var oneWayBCenter = tilemap_get_at_pixel(tileMap, bbox_left + 12, ceil(bbox_bottom) - objSpeed) & tile_index_mask;
	
	//we have some collision
	if ((bl != 0 || br != 0 || bCenter != 0) && (!isOneWay || (oneWayBr + oneWayBl + oneWayBCenter == 0))) {
		var ibr = tilemap_get_at_pixel(tileMap, bbox_right, bbox_bottom) & tile_index_mask;
	    var ibl = tilemap_get_at_pixel(tileMap, bbox_left, bbox_bottom) & tile_index_mask;
		var ibCenter = tilemap_get_at_pixel(tileMap, bbox_left + 12, bbox_bottom) & tile_index_mask;
		while(ibl != 0 || ibr != 0 || ibCenter != 0){
			y--;
			y = floor(y);
			ibr = tilemap_get_at_pixel(tileMap, bbox_right, bbox_bottom) & tile_index_mask;
	        ibl = tilemap_get_at_pixel(tileMap, bbox_left, bbox_bottom) & tile_index_mask;
		    ibCenter = tilemap_get_at_pixel(tileMap, bbox_left + 12, bbox_bottom) & tile_index_mask;
		}
		if(br != 0 || bl != 0 || bCenter != 0) {
			if(isBreakable && canBreak) {
				if(br != 0) {
					tilemap_set_at_pixel(tileMap, 0, bbox_right, ceil(bbox_bottom) + 1)
				}
				if(bl != 0) {
					tilemap_set_at_pixel(tileMap, 0, bbox_left, ceil(bbox_bottom) + 1)
				}
				if(bCenter != 0) {
				    tilemap_set_at_pixel(tileMap, 0, bbox_left + 12, ceil(bbox_bottom) + 1)
				}
			}
			else{
				vertSpeed = 0;
				doubleJumpSteps = maxDoubleJumpSteps;
			    floorJumpSteps = maxFloorJumpSteps;
				wallJumpSteps = 0
				hasDash = true;
				dashing = false;
				ignoreX = false;
				impulseX = 0;
				dashImpulse = 0;
				dashSteps = maxDashSteps;
				groundPound = false;
				return true;
			}
		}
	} else {
	    return false;	
	}	
}

function wallCollision(tileMap, isBreakable, objSpeed) {
    if (objSpeed > 0) {
	    var sideTR = tilemap_get_at_pixel(tileMap, bbox_right, bbox_top + 1) & tile_index_mask;
	    var sideBR = tilemap_get_at_pixel(tileMap, bbox_right, bbox_bottom - 1) & tile_index_mask;
	
	    //we have some collision
	    if (sideTR != 0 || sideBR != 0) {
			if(isBreakable && canBreak) {
				if(sideTR != 0) {
					tilemap_set_at_pixel(tileMap, 0, bbox_right, bbox_top + 1)
				}
				if(sideBR != 0) {
					tilemap_set_at_pixel(tileMap, 0, bbox_right, bbox_bottom - 1)
				}
				return false;
			}
			else {
		    	x = ((bbox_right & ~15) - 1) - spriteBoundingRight;
		    	if (!grounded) {
		    		wallJumpSteps = maxDoubleJumpSteps;
					impulseX = 0;
		    		wallJumpSide = 2;
		    	}
				else {
					wallJumpSteps = 0;
				}
				return true;
			}
	    }
    } else if (objSpeed < 0) {
	    var sideTL = tilemap_get_at_pixel(tileMap, bbox_left, bbox_top + 1) & tile_index_mask;
	    var sideBL = tilemap_get_at_pixel(tileMap, bbox_left, bbox_bottom - 1) & tile_index_mask;
	
	    //we have some collision
	    if (sideTL != 0 || sideBL != 0) {
			if(isBreakable && canBreak) {
				if(sideTL != 0) {
					tilemap_set_at_pixel(tileMap, 0, bbox_left, bbox_top + 1)
				}
				if(sideBL != 0) {
					tilemap_set_at_pixel(tileMap, 0, bbox_left, bbox_bottom - 1)
				}
				return false;
			}
			else {
			    x = ((bbox_left + 16) & ~15) - spriteBoundingLeft;
			    if (!grounded) {
			    	wallJumpSteps = maxDoubleJumpSteps;
					impulseX = 0;
			    	wallJumpSide = 1;
			    }
				else {
					wallJumpSteps = 0;
				}
				return true;
			}
	    }
    }
}

//move vert
if(!dashing){
    y += speedY;
}
if (speedY >= 0) {
	grounded = groundCollision(collTiles, false, false, speedY) || groundCollision(jumpThroughCollTiles, true, false, speedY) || groundCollision(breakableCollTiles, false, true, speedY);
	if (speedY == 0 && !grounded) {
	    var here = true;
	}
} else {
	var tr = tilemap_get_at_pixel(collTiles, bbox_right, bbox_top) & tile_index_mask;
	var tl = tilemap_get_at_pixel(collTiles, bbox_left, bbox_top) & tile_index_mask;
	
	//we have some collision
	if (tr != 0 || tl != 0) {
		y = ((bbox_top + 16) & ~15) - spriteBoundingTop;
		vertSpeed = 0;
	}
}
if (!grounded) {
	if(vertSpeed < terminalV || (keyboard_check(vk_down) && vertSpeed < fastTerminalV) || groundPound && vertSpeed < gpTerminalV){
		if (keyboard_check(vk_down) || groundPound) {
			vertSpeed += grav * 1.5;
		}
		else {
            vertSpeed += grav;
		}
	}
}

//move horizontal
if(dashing){
    x += dashImpulse;
	canBreak = true;
}
else {
    x += speedX;
}
wallCollision(collTiles, false, speedX);
wallCollision(breakableCollTiles, true, speedX)

if(dashing) {
    dashSteps--;
    if(dashSteps <= 0) {
	    dashImpulse = 0;
		dashing = false;
		ignoreX = false;
		vertSpeed = 0;
		canBreak = false;
	 }
}

if(!groundPound) {
    canBreak = false;
}