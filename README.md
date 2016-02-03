#Xi Game Engine

This is an old 3D game engine I made for XNA 3.1.

I'm updating some bits for resume fodder, but I don't see this code as worth using directly.

##Xi Game Engine Feature / Motivation List

**Game-centric Object Architecture** - Having worked on several games, I factored out a common object structure that applies to all the ones I've worked on. It consists of an Application object, a Screen object, ScreenTransition objects (transition in and transition out), ActorGroup objects, and finally, the main object, Actor. All of these objects are serializable at any time, and little to no custom serialization code need be written for user objects that extend these. A big problem with Ox is that it exposed a lot of functionality to the user, but it gave the user no idea of how to structure an actual application. By providing this object model out of the box, the developer gets started clearly and quickly.

**Facet System** - Many newer game engines use some type of component system to allow actors (AKA Entities in other engines) to be composed at design-time. This allows new actor behaviors to be contrived by combining Facets on an Actor at design-time with little to no additional code. Only a few facets exist currently, but more will be forthcoming.

**Overlay System** - When an actor is configured in the editor, it often shares common property values with other actors. The overlay system allows the developer to specify common property values for objects in XML. The overlay system also has an inheritance semantic that brings in property values from one overlay to another. Multiple overlay inheritance is also supported. This is similar to TorqueX's Template system.

**Pooling System** - Currently, the .Net Compact Framework cannot much handle frame-based object allocation / deallocation. Therefore, a pooling system is put in place to automatically pool objects where specified, resetting properties to the defaults as specified by the overlay system to keep recycled objects uniform.

**3D Functionality** - Xi allows building and editing 3D games using BEPU physics in real-time. Unlike Ox, the physics are integrated into existing actors and need no extra extension from the user to get an interactive physics scene. Currently only simple box and sphere physics is implemented along with static triangle meshes, but ragdolls and other types are forthcoming.

**2D Functionality** - Xi allows building and editing 2D games using farseer physics in real-time. Physics already integrated here as well. Currently only simple sprites are available, but animated sprites as well as composed sprites / physics are forthcoming, as well as tile editing.

**UI System** - Xi implements a basic UI system (currently only Button and Label UIs are available). More UI actors are forth coming.

**Fully-Integrated and Extensible Editor** - Unlike Ox, Xi allows editing of 3D, 2D, and UI actors all in the same editor session. There is also a way to extend the editor with custom editing scenarios for a particular game or object (however, this functionality isn't completely exposed yet).

**Play-In-Editor** - Xi is built to allow the designer to not only design a level in the editor, but to run the level interactively in the editor. This can reduce iteration times.

**Message System** - In addition to .Net events that are exposed where appropriate, Xi offers a message system which allow messages to be sent from one object to for a given event as specified directly in the editor. Message receivers are specified with custom syntax (currently based on some String.Split symbolics) relative to the sender. The messaging system uses memoization to avoid generating garbage when making use of reflection.

**Focus-based Input System** - Xi keeps track of the object that each player currently has control over, and automatically sends both low and high-level input messages to it. This also automates switching from UI controls with the DPad. Currently only the Xbox controller inputs are processed.

The engine is currently mostly documented in code using C# documentation. There is a small bit of external documentation in the source code's Documentation folder. More documentation is forthcoming.

**Todo for Alpha Version**

- Add water physics.
- Remove rounding on shadow edges.
- Add 2D physics debug drawing (along with 2D and UI editor selection drawing).
- Add 'Select Same Overlay' to editor.
- Do the more important TODOs.
- Port to XNA 4.0 (completely re-modify XNAnimation for Xi from scratch using current diff).
- Group actors by type in the editor.
- Build a sample game.
