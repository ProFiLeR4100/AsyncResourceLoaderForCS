using System.Threading.Tasks;
using Godot;

#nullable enable

namespace AsyncResourceLoaderRuntime {
    public static class AsyncResourceLoader {
        public static async Task<T?> LoadResource<T>(
            string path,
            SceneTree? providedSceneTree = null,
            string typeHint = "",
            bool useSubThreads = false,
            ResourceLoader.CacheMode cacheMode = ResourceLoader.CacheMode.Reuse
        ) where T : Resource {
            Error requestError = ResourceLoader.LoadThreadedRequest(path, typeHint, useSubThreads, cacheMode);

            if (requestError is not Error.Ok) {
                GD.PrintErr($"Failed to load resource at path \"{path}\" with error \"{requestError}\", returning null.");
                return default;
            }

            ResourceLoader.ThreadLoadStatus status = ResourceLoader.ThreadLoadStatus.Failed;
            SceneTree sceneTree = providedSceneTree ?? (SceneTree) Engine.GetMainLoop();

            do {
                if (status == ResourceLoader.ThreadLoadStatus.InProgress) {
                    await sceneTree.ToSignal(sceneTree, "process_frame");
                }

                status = ResourceLoader.LoadThreadedGetStatus(path);
            } while (status == ResourceLoader.ThreadLoadStatus.InProgress);

            if (status != ResourceLoader.ThreadLoadStatus.Loaded) {
                GD.PrintErr($"Failed to load resource at path \"{path}\" with status \"{status}\", returning null.");
            }

            return status == ResourceLoader.ThreadLoadStatus.Loaded
                ? (T?)ResourceLoader.LoadThreadedGet(path)
                : default;
        }
    }
}

#nullable disable