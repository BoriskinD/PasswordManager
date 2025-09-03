using CommunityToolkit.Maui.Animations;

namespace Client
{
    public class SampleScaleToAnimation : BaseAnimation
    {
        public double Scale { get; set; }

        public override Task Animate(VisualElement view, CancellationToken token = default)
        {
            return view.ScaleTo(Scale, Length, Easing);
        }
    }
}
