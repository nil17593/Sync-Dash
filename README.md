---------------------------Sync Dash----------------

*Objective
Sync Dash is a simple hyper-casual game where the screen is divided into two halves:

Right Side: The player controls their character.
Left Side: The ghost player mimics the player's actions in real-time, simulating network syncing locally.
This project tests real-time state synchronization, shader and particle effects, and performance optimization without using a multiplayer server.

*Game Concept
In Sync Dash, a glowing cube moves forward automatically, and the player must tap to jump and avoid obstacles while collecting glowing orbs. The left side of the screen shows a ghost player that mirrors the player’s actions in real-time, giving the impression of a networked opponent.

*Features:
Automatic forward movement for the player-controlled cube.
Tap to jump, avoid obstacles, and collect glowing orbs for points.
Real-time syncing of the ghost player, with a configurable network delay to simulate lag.
Increasing game speed over time.
Scoring system based on distance traveled and orbs collected.
Core Gameplay
*Player Movement:

The player controls a glowing cube that moves forward automatically on the right side.
The player taps to jump and avoid obstacles while collecting orbs for points.
Ghost Player:

The left side of the screen mirrors the player’s movements in real-time, simulating a networked opponent.
The ghost player mimics jumping, movement, orb collection, and obstacle collision.
Score System:

The score increases as the player moves forward and collects orbs.
The game speed increases over time, adding more challenge.
Real-Time State Syncing (Simulating Multiplayer Locally)
The left side of the screen mimics the player’s actions with real-time syncing.
Configurable network lag can be added to simulate a real-world multiplayer delay.
Smoothing interpolation ensures the ghost player’s movements are not jittery.
