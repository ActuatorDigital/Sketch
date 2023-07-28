<img src="Logo.png" alt="Sketch Logo">
<img src="https://img.shields.io/badge/unity-2021.3-green.svg?style=flat-square" alt="unity 2021.3">

A Sketch Framework for the Unity Editor.

## Description
Actuators's sketch framework allows you to build and interactively run small code snippets (called "Sketches") in isolation, without needing to build them into a game or scene.

Often, the best way to build new features are in isolation, and this framework facilitates that. Features built as sketches can be easily found, demonstrated and worked on, and much like tests, these sketches can then also serve as reference for certain features working in isolation.

As of version 0.1.0, we've made a significant update to our framework - the top-level namespace has changed. This is a breaking change, and you will need to update your imports accordingly.

## Creating Sketches

### Prerequisites
For the Unity Editor to recognise and run sketches, they must:
1. The sketch class must exists inside of an assembly with a name ending in ".Sketches". eg `Example.Sketches.asmdef`.
2. The sketch class must have the `[SketchFixture]` attribute.
3. Inherit from MonoBehaviour or ScriptableObject.
4. **(New in version 0.1.0)** The sketch class should be in the updated top-level namespace.

### Running

SketchRunner puts the game into playmode in an empty scene with the Sketch in it. If you have inherited from MonoBehaviour, this acts as normal. If you have inherited from ScriptableObject, SketchFixtureRunner will attempt to invoke a number of default Unity Messages. Such as `Start`, `Update`, `OnGUI`, `OnDrawGizmos`, `OnDestroy`, and more.

### Data

Sketches inherit from UnityEngine.Object so that you can assign default editor values in the MonoScript asset importer.

### Adding Descriptions
It is recommended that when creating a new sketch, you use the `[SketchDescription]` attribute on the class. Doing so provides developers more information about what the sketch is expected to demonstrate, and the text appears along with the test in in the sketch runner.

## Integrations

### Flume

The Sketch runner has additional functionality if included in a project also using [Flume](https://github.com/ActuatorDigital/Flume), which can provide sketches with dependency injection.

If your sketch has a Dependency, add the `[SketchDependsOn(Type serviceType, Type serviceImplementation)]` attribute.

By adding this attribute along with the given dependency type and implementation type, Sketch will create the necessary dependencies for your sketch to run. Note that dependencies don't have to be a sketch's direct dependency. A dependency will be injected for **any** object instantiated in the sketch.

## Sketch Runner
The sketch runner window can be found in the Unity Editor Window dropdown beside the test runner. To open the sketch runner, in the unity editor, navigate to:
```
Window > General > Sketch Runner
```
The sketch runner window will now appear, and list any/all valid sketches in your project, along with  names, descriptions, and run buttons for each. Feel free to dock it anywhere in your workspace.

## Samples

The Sketch package comes with two samples: `Empty` which contains a set up assembly definition file with a set up empty sketch, for adding into your project, and `Examples` which contains some simple examples of Sketch usage.