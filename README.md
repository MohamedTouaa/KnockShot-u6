# KnockShot

First-person arena survival game made in Unity 6 by a 4-person team in a few days for the **HYPLAY Indie Galactic #2** game jam (Oct 2024).
Your shotgun's recoil is also your movement: every shot knocks you back, and you survive waves of enemies that keep getting bigger.

▶ **Play in the browser:** https://shiroyashadono.itch.io/knockshot

## What I built (Mohamed / ShiroyashaDono)
- **Shotgun + knockback movement:** hitscan shooting with trail visuals, a fire cooldown, and recoil applied to the player's Rigidbody. [`Assets/_Shiro/Shotgun.cs`](Assets/_Shiro/Shotgun.cs)
- **Kill-streak power-up:** a UI bar fills as you kill enemies and then gives a timed one-shot-kill mode. [`Assets/_Shiro/PowerSurge.cs`](Assets/_Shiro/PowerSurge.cs)
- **Player movement and camera:** [`Assets/_Shiro/PlayerMovement.cs`](Assets/_Shiro/PlayerMovement.cs), [`CameraController.cs`](Assets/_Shiro/CameraController.cs), [`MoveCamera.cs`](Assets/_Shiro/MoveCamera.cs)
- **Enemy health:** [`Assets/_Shiro/EnemyHealth.cs`](Assets/_Shiro/EnemyHealth.cs)
- **Enemy waves:** a NavMesh-based spawner that grows every wave and takes enemies from object pools. I adapted a common tutorial pattern for this (ObjectPool, EnemyScriptableObject, AgentLinkMover). [`Assets/MyScripts/Ennemy/`](Assets/MyScripts/Ennemy/), [`Assets/MyScripts/ObjectPool/`](Assets/MyScripts/ObjectPool/)
- **VFX and shaders:** a blood VFX Graph, a blood shader and an outline Shader Graph. [`Assets/Shaders/`](Assets/Shaders/), [`Assets/MyPrefabs/VFX_Blood.vfx`](Assets/MyPrefabs/VFX_Blood.vfx)

## Team
ShiroyashaDono (me), Nomad_2088, Nano, HaruHiru. See the [itch.io page](https://shiroyashadono.itch.io/knockshot) for credits.
Teammates' commits in this repo: **Denver987** (game manager, score, waves, pause/death flow, flanking AI: `GameManager.cs`, `Flank.cs`) and **Abadli-Badro** (UI fixes).

## Third-party
The art and VFX packs (BattleDragon, SimpleToon, Cartoon FX Remaster, Unity Particle Pack) come from the Asset Store, the sound manager is by Small Hedge Games, and the HYPLAY SDK was provided by the jam platform. These are not my code.

## Tech
Unity 6000.0.22f1 · C# · URP · VFX Graph · Shader Graph · NavMesh · WebGL
