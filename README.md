# Async Resource Loader (C#)

Allows to use standard C# async/await to load resources for Godot engine.

_Supports only Godot 4.0+_

# How to install

Download the project and copy the addon folder into your godot project.

Go to Project Settings > Plugins, and enable Async Resource Loader (C#).

# How to use

Add import of namespace at the beginning of C# file.
```csharp
using AsyncResourceLoaderRuntime;
```

Load resource that you want.
```csharp
await AsyncResourceLoader.LoadResource<AudioStream>("res://sound.mp3");
```

If you have to override Engine.MainLoop, then you must provide your SceneTree.
```csharp
await AsyncResourceLoader.LoadResource<AudioStream>(
    "res://sound.mp3",
    GetTree()
);
```

Other parameters are the same as on original `ResourceLoader.LoadThreadedRequest(path, typeHint, useSubThreads, cacheMode)`.

Please check official Engine manual for these params explanation.

# Example

```csharp
using System.Threading.Tasks;
using Godot;
using AsyncResourceLoaderRuntime;

public partial class ExampleClass: Node {
    // I've put a _Ready method just as a way to trigger async task.
    public override void _Ready() {
        base._Ready();
        PlayAudio("res://sound.mp3");
    }

    private async Task<Resource> PlayAudio(string pathToAudioFile) {
        // Create new Audio Stream Player
        AudioStreamPlayer streamPlayer = new();

        // Asynchronously loading and saving audio stream
        streamPlayer.Stream = await AsyncResourceLoader.LoadResource<AudioStream>(pathToAudioFile);

        // Add Audio Stream Player to the tree
        CallDeferred(Node.MethodName.AddChild, streamPlayer);

        // Call "Play" method on player
        streamPlayer.CallDeferred(AudioStreamPlayer.MethodName.Play);

        // Awaits player to finish playing
        await streamPlayer.ToSignal(
            streamPlayer,
            AudioStreamPlayer.SignalName.Finished
        );

        // Safely queues stream player to free mamory.
        streamPlayer.QueueFree();
    }
}
```
