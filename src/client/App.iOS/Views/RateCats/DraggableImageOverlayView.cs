using System;
using UIKit;
using CoreGraphics;
using Foundation;
using SDWebImage;

using Anotar.Splat;
using EndlessCatsApp.Services.Api;
using EndlessCatsApp.Utility;
using ReactiveUI;

namespace EndlessCatsApp.iOS.Views
{

    [Register("DraggableImageViewOverlay")]
    public class DraggableImageOverlayView : UIView
    {
        private UIImageView _imageView;
        private SwipeDirection _swipeDirection;

        public DraggableImageOverlayView(CGRect frame) : base(frame)
        {
            _imageView = new UIImageView();

            this.AddSubview(_imageView);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            _imageView.Frame = new CGRect(50, 50, 100, 100);
        }

        public void SetSwipeDirection(SwipeDirection swipeDirection)
        {
            if (_swipeDirection == swipeDirection)
            {
                return;
            }

            _swipeDirection = swipeDirection;
            switch (swipeDirection)
            {
                case SwipeDirection.Left:
                    _imageView.Image = UIImage.FromBundle("Nope");
                    LogTo.Debug(() => "Overlay has been set to nope.");
                    break;
                case SwipeDirection.Right:
                    _imageView.Image = UIImage.FromBundle("Liked");
                    LogTo.Debug(() => "Overlay has been set to like.");
                    break;
                default:
                    LogTo.Fatal(() => "Unknown swipe direction.");
                    break;
            }
        }

    }
}

