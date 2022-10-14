# ColorParkour
A very barebones prototype of some puzzle platforming mechanics based off something I made in Minecraft.
(Created in 2020)

This is based off of something I made in Minecraft on the DiamondFire server (username = ShadeBlade__)

The problem I had was that I wasn't sure where to go next, I wasn't sure how to turn this into an actual, substantial game.

There are many different colored platforms, and different powers you can acquire (but you can only have one at a time).

Controls are WASD to move, Shift to run, Space to jump, Left click to use powers

### Platforms
- White - Safe: This platform constantly saves your position while you are on top of it (you get sent back to the saved position under some conditions)
- Black - Reset: Standing on this platform resets your position
- Red - Reset on look: Looking at this platform (while close to it) will reset your position
- Cyan - Pull on look: Looking at this platform pulls you towards it
- Green - Push on look: Looking at this platform pushes you away from it
- Magenta - Stasis on look: Looking at this platform makes you stay in place
- Blue - Negate powers: Standing on this platform prevents you from activating powers
- Yellow - Sticky: Standing on this platform prevents you from jumping

### Powers
- White - Lift: Lifts you up into the air slightly (best used right after jumping due to the wonky jump physics)
- Green - Dash: Pushes you forward
- Red - Hover: Lets you hover in the air for a short time
- Yellow - Sticky: Activating this while touching a wall will let you stick to it (the physics are weird though)
- Blue - Negate: Passively gives you a blue filter, stops all effects relating to looking at certain platforms (note that yellow platforms still stop you from jumping, this is an oversight)
- Magenta - Invert: Inverts the colors of the screen, which also inverts the properties of all the colored platforms accordingly.
- Black - Teleport: Teleport to a point on a black surface if you are looking at one
