using System;
using UIKit;
using CoreGraphics;
using Foundation;
using SDWebImage;

using Anotar.Splat;
using EndlessCatsApp.Services.Api;
using EndlessCatsApp.Utility;

namespace EndlessCatsApp.iOS.Views
{
    public enum SwipeDirection
    {
        Left,
        Right
    }

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
                    _imageView.Image = UIImage.FromBundle("DislikeCatOverlay");
                    LogTo.Debug(() => "Overlay has been set to dislike.");
                    break;
                case SwipeDirection.Right:
                    _imageView.Image = UIImage.FromBundle("LikeCatOverlay");
                    LogTo.Debug(() => "Overlay has been set to like.");
                    break;
                default:
                    LogTo.Fatal(() => "Unknown swipe direction.");
                    break;
            }
        }

    }
}

