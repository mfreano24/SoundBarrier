# Sound Barrier
Texas Aggie Game Developers Fall 2020 Game Jam - "Something's Missing."  
**Received 2nd place from a panel of judges.**  
All Art, Music, Code, Design by Michael Freaney.

## Concept
Navigate your way through a maze-like building security system in which the walls are invisible. The walls will be rendered visible upon shooting them with your laser. Use this to stealthily avoid guards and make your way to the top of the tower.

## Notable Code
### Finite State Machine AI
The major programming challenge in this project was the creation of enemy AI to create a proper stealth situation. The state machine was inspired primarily by _Legend of Zelda_ style stealth segments, in which the enemy cannot be harmed, the enemy can react to and investigate noises and things that may lead them to you, and on sight, the enemy will do anything in its power to hit you.  
<img src="https://michaelfreaney.com/assets/img/sound_barrier/Enemy%20AI%20Design.png" width="450px">
#### The "Investigation Point" System
While the "attacking" and "patrolling" states are fairly easy and don't really rely on the player's actions, triggering the state machine's "investigation" state has to account for the unpredictability of the player a bit more- as such, the "investigation point" is an object which spawns at the player's location upon certain actions that are considered noisy (such as shooting a gun)- the investigation point runs a script which alerts any enemies in some arbitrary radius of the noise, and switches their state to "investigating" as their pathfinding switches destinations. The newest investigation point will always take priority for the enemy, so that any conflicts can be safely ignored. This allows for a continuous system that responds to anything the player might do.
### Simple Shader Graph Effects
To produce the wall effects in the game, as well as the electric fences, simple shader effects were used- these combined a continuous offsetting noise and the alpha clip threshold fields of the shaders to create "dissolving" waves.
