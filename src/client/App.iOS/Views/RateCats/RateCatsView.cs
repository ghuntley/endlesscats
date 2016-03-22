using System;
using UIKit;
using CoreGraphics;
using Foundation;
using Anotar.Splat;
using EndlessCatsApp.Utility;
using EndlessCatsApp.Services.Api;
using ReactiveUI;
using System.Reactive.Linq;

namespace EndlessCatsApp.iOS.Views
{
    [Register("RateCatsView")]
    public class RateCatsView : ReactiveView, IViewFor<ReactiveList<Cat>>
    {
        // max number of cards loaded as any given time, must be greater than 1.
        private const int MaxBufferSize = 2;

        // height of the draggable card
        private readonly nfloat _cardHeight;

        // width of the draggable card
        private readonly nfloat _cardWidth;

        public RateCatsView(CGRect frame) : base(frame)
        {
            _cardWidth = Frame.Size.Height / 2;
            _cardHeight = (nfloat)(Frame.Size.Height / 1.1);

            this.BackgroundColor = UIColor.LightGray;

            var cat = new Cat() { Identifier = "134", Url = new Uri("http://28.media.tumblr.com/Jjkybd3nSnisiguqJuNKixjxo1_500.jpg"), SourceUrl = new Uri("http://thecatapi.com/?id=2ad") };

            this.AddSubview(CreateDraggableImageView(cat));
            this.AddSubview(CreateDraggableImageView(cat));
            this.AddSubview(CreateDraggableImageView(cat));
            this.AddSubview(CreateDraggableImageView(cat));
            this.AddSubview(CreateDraggableImageView(cat));
        }

        public ReactiveList<Cat> ViewModel
        {
            get;
            set;
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (ReactiveList<Cat>)value; }
        }

        public DraggableImageView CreateDraggableImageView(Cat cat)
        {
            Ensure.ArgumentNotNull(cat, nameof(cat));

            var x = (this.Frame.Size.Width - _cardWidth) / 2;
            var y = (this.Frame.Size.Height - _cardHeight) / 2;
            var frame = new CGRect(x, y, _cardWidth, _cardHeight);

            var draggableImageView = new DraggableImageView(frame);
            draggableImageView.ViewModel = cat;

            draggableImageView.OnViewSwipedToTheLeft += OnViewSwipedToTheLeft;
            draggableImageView.OnViewSwipedToTheRight += OnViewSwipedToTheRight;
            return draggableImageView;
        }

        public IObservable<SwipeDirection> SwipeDirection { get; private set; }

        private void OnViewSwipedToTheLeft(object sender, EventArgs e)
        {
        }

        private void OnViewSwipedToTheRight(object sender, EventArgs e)
        {
        }
    }
}

