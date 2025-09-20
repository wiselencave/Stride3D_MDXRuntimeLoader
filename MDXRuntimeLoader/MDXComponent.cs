using MDXRuntimeLoader.MDXStuff;
using Stride.Animations;
using Stride.Core.Diagnostics;
using Stride.Engine;
using System.Collections.Generic;
using Stride.UI.Controls;
using System.IO;

namespace MDXRuntimeLoader
{
    public class MDXComponent : SyncScript
    {
        //[Stride.Core.DataMember("Path to file")]
        private string modelPath = "F:\\refUnpacked\\war3.w3mod\\_hd.w3mod\\units\\human\\jaina\\jaina.mdx";

        private string[] testModels =
        {
            "F:\\refUnpacked\\war3.w3mod\\_hd.w3mod\\units\\human\\jaina\\jaina.mdx",
            "F:\\refUnpacked\\war3.w3mod\\_hd.w3mod\\units\\undead\\abomination\\abomination.mdx",
            "F:\\refUnpacked\\war3.w3mod\\_hd.w3mod\\units\\human\\mortarteam\\mortarteam.mdx",
            "F:\\refUnpacked\\war3.w3mod\\_hd.w3mod\\units\\human\\arthaswithsword\\arthaswithsword.mdx",
            "F:\\refUnpacked\\war3.w3mod\\_hd.w3mod\\units\\naga\\ladyvashj\\ladyvashj.mdx",
            "F:\\refUnpacked\\war3.w3mod\\_hd.w3mod\\units\\orc\\shaman\\Shaman.mdx",
            "F:\\refUnpacked\\war3.w3mod\\_hd.w3mod\\units\\undead\\cryptfiend\\cryptfiend.mdx",
            "F:\\refUnpacked\\war3.w3mod\\_hd.w3mod\\units\\demon\\viletemptress\\viletemptress.mdx",
            "F:\\refUnpacked\\war3.w3mod\\_hd.w3mod\\units\\undead\\detheroc\\detheroc.mdx",
            "F:\\refUnpacked\\war3.w3mod\\_hd.w3mod\\buildings\\other\\mercenary\\mercenary.mdx",
            "F:\\refUnpacked\\war3.w3mod\\_hd.w3mod\\units\\orc\\kotobeast\\kotobeast.mdx"
        };
        private Dictionary<string, LoadedModel> loadedModels = new();
        private int currentModel = -1;
        private bool isLoaded = false;

        Dictionary<string, AnimationClip> Clips;

        private TextBlock textBlock;
        private TextBlock modelName;
        private AnimationComponent animCmp;
        private int currentId = -1;

        private Slider playbackSlider;

        public override void Start()
        {
            Log.ActivateLog(LogMessageType.Debug);
            Log.Debug("Start parsing");
            
            try
            {
                foreach (string path in testModels)
                {
                    Log.Debug(path);
                    MDXReForged.MDX.Model mdx = new MDXReForged.MDX.Model(path);
                    var (model, clips) = MDXLoader.LoadMDX(GraphicsDevice, Content, mdx);
                    Clips = clips;

                    var animCmp = new AnimationComponent(); //Entity.GetOrCreate<AnimationComponent>();
                    var clipNames = new List<string>();
                    foreach (var anim in Clips)
                    {
                        animCmp.Animations.Add(anim.Key, anim.Value);
                        clipNames.Add(anim.Key);
                    }

                    loadedModels.Add(path, new LoadedModel(new ModelComponent(model), animCmp, clipNames));
                    //Entity.Add(new ModelComponent(model));
                }
            }
            finally
            {
                Log.Debug("Parsed succesfully");
            }
        }

        public override void Update()
        {
            if (!isLoaded)
            {
                isLoaded = true;
                animCmp = Entity.GetOrCreate<AnimationComponent>();
                
                InitUI();
                
                SelectNextModel();
                SelectNextAnimation();
            }
        }

        private void InitUI()
        {
            var page = Entity.Get<UIComponent>().Page;
            textBlock = page.RootElement.VisualChildren[0] as TextBlock;

            var button = page.RootElement.VisualChildren[1] as Button;
            button.Click += Button_Click;

            var nextModelButton = page.RootElement.VisualChildren[3] as Button;
            nextModelButton.Click += NextModelButton_Click;

            modelName = page.RootElement.VisualChildren[2] as TextBlock;

            playbackSlider = page.RootElement.VisualChildren[4] as Slider;
            playbackSlider.Value = 1;
            playbackSlider.ValueChanged += PlaybackSlider_ValueChanged;
        }

        private void PlaybackSlider_ValueChanged(object sender, Stride.UI.Events.RoutedEventArgs e)
        {
            ChangeAnimationPlaybackSpeed();
        }

        private void ChangeAnimationPlaybackSpeed()
        {
            var animation = animCmp.PlayingAnimations[0];
            animation.TimeFactor = playbackSlider.Value;
        }

        private void NextModelButton_Click(object sender, Stride.UI.Events.RoutedEventArgs e)
        {
            SelectNextModel();
            currentId = -1;
            SelectNextAnimation();

            playbackSlider.Value = 1;
            ChangeAnimationPlaybackSpeed();
        }

        private void Button_Click(object sender, Stride.UI.Events.RoutedEventArgs e)
        {
            SelectNextAnimation();
            ChangeAnimationPlaybackSpeed();
        }

        private void SelectNextModel()
        {
            currentModel++;
            if (currentModel >= loadedModels.Count)
            {
                currentModel = 0;
            }

            //isLoaded = false;
            modelName.Text = Path.GetFileNameWithoutExtension(testModels[currentModel]);

            Entity.Remove(Entity.Get<AnimationComponent>());
            Entity.Remove(Entity.Get<ModelComponent>());

            Entity.Add(loadedModels[testModels[currentModel]].Model);
            Entity.Add(loadedModels[testModels[currentModel]].Animation);

            animCmp = Entity.GetOrCreate<AnimationComponent>();
        }

        private void SelectNextAnimation()
        {
            var model = loadedModels[testModels[currentModel]];

            currentId++;
            if (currentId >= model.ClipNames.Count)
            {
                currentId = 0;
            }

            animCmp.Play(model.ClipNames[currentId]);
            textBlock.Text = model.ClipNames[currentId];
        }
    }
    public struct LoadedModel(ModelComponent modelComponent, AnimationComponent animationComponent, List<string> clipNames)
    {
        public ModelComponent Model { get; set; } = modelComponent;
        public AnimationComponent Animation { get; set; } = animationComponent;
        public List<string> ClipNames { get; set; } = clipNames;
    }
}
