using System;
using UIKit;
using CoreGraphics;
using Foundation;
using Anotar.Splat;
using EndlessCatsApp.Utility;
using EndlessCatsApp.Services.Api;

namespace EndlessCatsApp.iOS.Views
{
    public class RateCatsView : UIView
    {
        // max number of cards loaded as any given time, must be greater than 1.
        private const int MaxBufferSize = 2;

        // height of the draggable card
        private readonly nfloat CardHeight = 386;

        // width of the draggable card
        private readonly nfloat CardWidth = 290;

        public RateCatsView(CGRect frame) : base(frame)
        {

            this.LayoutSubviews();

            this.BackgroundColor = UIColor.Yellow;

            var cat = new Cat() { Identifier = "134", Url = new Uri("http://28.media.tumblr.com/Jjkybd3nSnisiguqJuNKixjxo1_500.jpg"), SourceUrl = new Uri("http://thecatapi.com/?id=2ad") };

            this.AddSubview(CreateDraggableImageView(cat.Url));
            this.AddSubview(CreateDraggableImageView(cat.Url));
            this.AddSubview(CreateDraggableImageView(cat.Url));
            this.AddSubview(CreateDraggableImageView(cat.Url));
            this.AddSubview(CreateDraggableImageView(cat.Url));
        }

        public DraggableImageView CreateDraggableImageView(Uri uri)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));

            var x = (this.Frame.Size.Width - CardWidth) / 2;
            var y = (this.Frame.Size.Height - CardHeight) / 2;
            var frame = new CGRect(x, y, CardWidth, CardHeight);

            var draggableImageView = new DraggableImageView(frame);
            draggableImageView.ImageUrl = uri;

            return draggableImageView;
        }
    }
}

