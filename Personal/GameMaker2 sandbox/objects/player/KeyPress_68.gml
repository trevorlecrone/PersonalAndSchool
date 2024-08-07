if(!grounded && hasDash) {
	dashImpulse = 8 * (keyboard_check(vk_right) - keyboard_check(vk_left));
	dashing = true;
	hasDash = false;
	ignoreX = true;
	ignoreGrav = true
}