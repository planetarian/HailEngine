# HailEngine
Hail game engine + Graupel entity definition language

This was written as an experiment at creating an extensible
Entity System based game engine in XNA/MonoGame.

It uses a modified port of the Artemis Entity System library as the base entity system.

Scenes are defined in a custom entity definition language I created, called Graupel.
Graupel was designed to be simple and flexible. I based it on languages like JSON,
and removed some of the verbosity of such languages by simplifying basic object creation.

The main purpose of designing Graupel was to be able to construct game scenes without
having to modify the actual engine code. Instead, the engine code is limited to the
components which define characteristics of game entities, and the systems which act
on those components.
Meanwhile, the actual game scenes are written in Graupel, outside of the game code itself,
and the game engine parses and interprets the Graupel code and generates the entities
with the defined characteristics.

This allows the game development work to be divided into engine/component logic and
actual scene logic, which in turn allows developers to focus purely on component logic
while designers can build the game world without affecting the engine code at all.

As a personal project, Hail/Graupel was designed with two purposes in mind:
1) to familiarize myself with concepts of Entity System based game development, and
2) to learn how language/expression parsers and execution engines are built.


A component within an entity can contain multiple value setters:
```
viewport {
	cameratag: "camera1";
	bounds: 0 0 1 1;
}
```

...or just a single inline setter:
```
transform scale: .5;
```

...or if it just wants to add the component with all values at defaults:
```
movement;
```

Graupel in pseudocode:
```
[global | scene*]

global {
	template*
}

scene sceneName {
	template* | entity*
}

template templateName [: templateExtends [, templateExtends]*]? {
	group "groupName";?
	component*
}

entity [entityName]? [: templateExtends [, templateExtends]*]? {
	group "groupName";?
	component*
}

component
	; | property | {
	property*
}

property: value;
```

And an example how how it might be used: (note: this is merely an example; some of the below components haven't actually been implemented!)
```
global {
	template camera {
		transform position: 10 20 30;
		camera;
	}
	
	template mouseTarget {
		transform;
		movement;
		mouseTarget;
	}
	
	template mapObject {
		group "mapObjects";
		transform;
		model;
	}

	template collidableObject : mapObject {
		collision {
			type: "polygon";
			collidesWithGroups: "player", "bullet";
			collisionDamage: 100;
			solid: true;
		}
		health value: 200; // hard to kill
	}
}

/*
Scenes can be included by other scenes like templates.
In this instance, we're storing the level's terrain data
in one scene and loading it into another.
This lets us create multiple levels that are set in
the same location without having to duplicate the data.
*/
scene levelData {
	
	entity : collidableObject {
		model name: "ground";
	}

	entity : collidableObject {
		model name: "wall01";
		transform position: 4 0 10 , 10 4 30;
	}
}

scene scene001 : levelData {

	template boxBaddie {
		group "enemy";
		transform;
		movement;
		ai script: "box";
		model name: "box";
		health value: 30;
		collision {
			type: "aabb";
			collidesWithGroups: "player", "bullet";
			collisionDamage: 30;
			solid: false;
		}
	}

	entity : boxBaddie {
		transform position: rand(-20,20) 0 rand(-20,20); 
	}

	entity boxboss : boxBaddie {
		ai script: "boxboss";
		health value: (100-2)/3;
	}

	entity mouse : mouseTarget {
		transform {
			scale: (.5 .25 .5) * (3 2 1);
			rotation: y 45 deg;
		}
	}

}
```
