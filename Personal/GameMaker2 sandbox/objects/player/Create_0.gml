/// @description Initialize variables and collision
standardMoveSpeed = 2;
fastMoveSpeed = 4;
moveSpeed = standardMoveSpeed;
jumpImpulse = -4.0;
jumpHeld = -0.37;
wallJumpImpulse = -8.0;
impulseX = 0;
dashImpulse = 0;
terminalV = 5;
fastTerminalV = 15;
grav = 0.3;
vertSpeed = 0;
maxFloorJumpSteps = 12;
floorJumpSteps = maxFloorJumpSteps;
maxDoubleJumpSteps = 10;
doubleJumpSteps = maxDoubleJumpSteps;
wallJumpSteps = 0;
dashSteps = 0;
maxDashSteps = 10;
// STATE INFO
spaceUp = true;
ignoreX = false;
ignoreGrav = false;
// 1 is left, 2 is right
wallJumpSide = 0;
grounded = false;
hasDash = true;
dashing = false


//collision info
var coll = layer_get_id("coll");
collTiles = layer_tilemap_get_id(coll);
var jumpThroughColl = layer_get_id("jumpThroughColl");
jumpThroughCollTiles = layer_tilemap_get_id(jumpThroughColl);

//Sprite info
spriteBoundingLeft = sprite_get_bbox_left(sprite_index) - sprite_get_xoffset(sprite_index);
spriteBoundingRight = sprite_get_bbox_right(sprite_index) + sprite_get_xoffset(sprite_index);
spriteBoundingBottom = sprite_get_bbox_bottom(sprite_index) - sprite_get_yoffset(sprite_index);
spriteBoundingTop = sprite_get_bbox_top(sprite_index) + sprite_get_yoffset(sprite_index);
