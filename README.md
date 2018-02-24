# Steed's Broglike Starter
This is a 2017.3.0f3 Unity3D project that has been laid out for prototyping a roguelike in the manner of [Michael Brough](http://www.smestorp.com/). This package assumes an intermediate understanding of Unity and some healthy skepticism towards my approach.

I have addressed all the details that I consider essential:

* It's playable. You can die and restart. Ready to be refactored into something new.
* Procedural. No physics. Everything is run from one place. A roguelike is a complex board game, the order of actions determines life or death. (You can add physics, I won't judge.)
* All things on the map are interchangeable - a good position from which to develop crazy polymorphs, traps, and factions. Things have an x,y,z position because some projects might want to chuck stuff into the z axis (terrain, totem-pole-stacks, actual 3D!).
* *Enum Madness:* All things are created from a 32 bit integer. Bitwise flags are used to pack most of the essential states into one int. This allows for very fast prototyping and map generation. It's not scaleable, don't rely on it too much.
* Basic UI like a popup, getting the camera to follow an offset view (an adjustable RectTransform on the UI canvas), stat bars / text, a log - all done.
* Main Scene workshop: All prefabs are in the Main scene to be modified and updated. The engine deletes them on start up.
* Thing Prefabs: Player, Mon(ster), Wall, Hard Wall (indestructible), Shot, and Loot. These last two are special because I realised they needed to stack and allow you to walk over them. As a compromise they use a special trigger lookup table, sort of like a trap (you could turn a loot into one for example).
* Recycling. Always assume mass-spawning is possible. And hilarious.
* Particle Emitter prefabs. Self recycling - go crazy.
* AI: Just a basic floodfill.  I might add A* (I write a pretty fast A*), but flooding is more efficient with swarms (common to roguelikes). It can be debugged by clicking on the Map GameObject at run time - the numbers will show up in the editor scene view.
* Texture Preprocessor. Unity's default texture import settings hate pixel art.
* Lightweight tween class: I need to swap this out for AnimationCurves, when I figure out how to get them to tween arbitrary Vector3s at runtime.

I am open to suggestions for improvement. Including dependencies (libraries) is unsustainable, but useful Asset Store links are welcome, I will add them below. If you have an idea, say it with C#. A snippet of working code is worth a 1000 clever suggestions.