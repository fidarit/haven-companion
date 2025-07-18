using LLMUnity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;
using Whisper;
using Whisper.Utils;

public class GameLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterEntryPoint<STTWhisper>().AsSelf();

        builder.RegisterComponentInHierarchy<WhisperManager>();
        builder.RegisterComponentInHierarchy<MicrophoneRecord>();
        builder.RegisterComponentInHierarchy<LLMCharacter>();
        builder.RegisterComponentInHierarchy<Agent>();

        InjectAllExistedGameObjects(builder);
    }

    private static void InjectAllExistedGameObjects(IContainerBuilder builder)
    {
        builder.RegisterBuildCallback(container =>
        {
            var activeScene = SceneManager.GetActiveScene();
            var rootObjects = new List<GameObject>(activeScene.rootCount);

            activeScene.GetRootGameObjects(rootObjects);

            foreach (var item in rootObjects)
                container.InjectGameObject(item);
        });
    }
}
