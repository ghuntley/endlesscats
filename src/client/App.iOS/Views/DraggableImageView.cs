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
    public interface IDraggableView
    {
        EventHandler OnSwipedToTheLeft { get; set; }

        EventHandler OnSwipedToTheRight { get; set; }

        Uri ImageUrl { get; set; }

    }

    [Register("DraggableImageView")]
    public sealed class DraggableImageView : UIView, IDraggableView
    {
        // distance from center where the aciton applies. Higher = swipe further before the action will be called.
        private readonly nint ActionMargin = 120;
        // how quicikly the card shirnks. Higher = shrinks less.
        private readonly nint ScaleStrength = 4;
        // upper bar for how much the card shrinks. Higher = shrinks less.
        private readonly nfloat ScaleMax = 0.93f;
        // the maximum rotation allowed in radians. Higher = card can keep rotation longer.
        private readonly nint RotationMax = 1;
        // the strength of rotation. Higher = weaker rotation.
        private readonly nint RotationStrength = 320;
        // Higher = stronger rotation angle.
        private readonly nfloat RotationAngle = 3.14159f * 8;

        private readonly DraggableImageOverlayView _overlayView;
        private readonly UIImageView _imageView;
        private readonly UIPanGestureRecognizer _panGestureRecognizer;
        private Uri _imageUrl;

        private CGPoint _originalPoint;
        private nfloat _xFromCenter;
        private nfloat _yFromCenter;

        public DraggableImageView(CGRect frame) : base(frame)
        {

            this.BackgroundColor = UIColor.White;
            this.Layer.CornerRadius = 4;
            this.Layer.ShadowRadius = 4;
            this.Layer.ShadowOpacity = 0.2f;
            this.Layer.ShadowOffset = new CGSize(1, 1);

            _panGestureRecognizer = new UIPanGestureRecognizer();
            _panGestureRecognizer.AddTarget(() => HandleCardDrag(_panGestureRecognizer));
            this.AddGestureRecognizer(_panGestureRecognizer);

            _imageView = new UIImageView(this.Bounds);
            _imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            _imageView.Image = UIImage.FromBundle("CatDownloadingPlaceholder");

            this.AddSubview(_imageView);

            var overlayViewRect = new CGRect(Frame.Size.Width / 2 - 100, 0, 100, 100);
            _overlayView = new DraggableImageOverlayView(overlayViewRect);
            this.AddSubview(_overlayView);

        }


        public EventHandler OnSwipedToTheLeft { get; set; }

        public EventHandler OnSwipedToTheRight { get; set; }

        public Uri ImageUrl
        {
            get
            {
                return _imageUrl;
            }

            set
            {
                _imageUrl = value;
                LoadImage();
            }
        }

        private void LoadImage()
        {
            _imageView.SetImage(
                url: _imageUrl,
                placeholder: UIImage.FromBundle("CatDownloadingPlaceholder"),
                completedBlock: (UIImage image, NSError error, SDImageCacheType cacheType, NSUrl imageUrl) =>
                {
                    if (error != null)
                    {
                        LogTo.Error(() => $"Image failed to load: '{error}' from: '{imageUrl}'");
                        return;
                    }

                    switch (cacheType)
                    {
                        case SDImageCacheType.Disk:
                            LogTo.Debug(() => $"Image was successfully loaded from disk: '{imageUrl}'");
                            break;
                        case SDImageCacheType.Memory:
                            LogTo.Debug(() => $"Image was successfully loaded from memory: '{imageUrl}'");
                            break;
                        case SDImageCacheType.None:
                            LogTo.Debug(() => $"Image was successfully loaded from the network: '{imageUrl}'");
                            break;
                    }
                });
        }

        /// <summary>
        /// called when the card is let go.
        /// </summary>
        private void OnCardDragFinished()
        {
            if (_xFromCenter > ActionMargin)
            {
                CardWasSwipedToTheRight();
            }
            else if (_xFromCenter < -ActionMargin)
            {
                CardWasSwipedToTheLeft();
            }
            else {
                // reset the card
                UIView.Animate(duration: 0.3, animation: new Action(() =>
                {
                    this.Center = _originalPoint;
                    this.Transform = CGAffineTransform.MakeRotation(0);
                    _overlayView.Alpha = 0;
                }));
            }

        }

        private void UpdateOverlay(nfloat distance)
        {
            if (distance > 0)
            {
                _overlayView.SetSwipeDirection(SwipeDirection.Right);
            }
            else {
                _overlayView.SetSwipeDirection(SwipeDirection.Left);
            }

            _overlayView.Alpha = (nfloat)Math.Min(Math.Abs(distance) / 100, 0.4);
        }


        private void HandleCardDrag(UIPanGestureRecognizer recognizer)
        {
            // positive for right swipe, negative for left
            _xFromCenter = recognizer.TranslationInView(this).X;

            // positive for up, negative for down
            _yFromCenter = recognizer.TranslationInView(this).Y;


            switch (recognizer.State)
            {
                case UIGestureRecognizerState.Began:
                    LogTo.Debug(() => $"View has started to be dragged: x={_xFromCenter} y={_yFromCenter}");
                    _originalPoint = this.Center;
                    break;

                case UIGestureRecognizerState.Changed:
                    LogTo.Debug(() => $"View is being dragged: x={_xFromCenter} y={_yFromCenter}");

                    // dictates rotation
                    double rotationStrength = Math.Min(_xFromCenter / RotationStrength, RotationMax);

                    // degree in radians
                    nfloat angle = (nfloat)(RotationAngle * rotationStrength);

                    // amount the height changes when you move the card up to a certain point
                    nfloat scale = (nfloat)Math.Max(1 - Math.Abs(rotationStrength) / ScaleStrength, ScaleMax);

                    // move the object's center by center + gesture coordinate
                    nfloat x = _originalPoint.X + _xFromCenter;
                    nfloat y = _originalPoint.Y + _yFromCenter;
                    this.Center = new CGPoint(x, y);
                    LogTo.Debug(() => $"View center has been changed to: x={x} y={y}");


                    // rotate by a certain amount
                    CGAffineTransform transform = CGAffineTransform.MakeRotation(angle);

                    // scale by a certain amount
                    CGAffineTransform scaleTransform = CGAffineTransform.Scale(transform, scale, scale);

                    // apply transformations
                    this.Transform = scaleTransform;
                    UpdateOverlay(_xFromCenter);

                    break;

                case UIGestureRecognizerState.Ended:
                    LogTo.Debug(() => $"View is no longer being dragged: x={_xFromCenter} y={_yFromCenter}");

                    OnCardDragFinished();
                    break;

                case UIGestureRecognizerState.Possible:
                    LogTo.Debug(() => $"View drag is possible: x={_xFromCenter} y={_yFromCenter}");
                    break;

                case UIGestureRecognizerState.Cancelled:
                    LogTo.Debug(() => $"View drag has been cancelled: x={_xFromCenter} y={_yFromCenter}");
                    break;

                case UIGestureRecognizerState.Failed:
                    LogTo.Debug(() => $"View drag has failed: x={_xFromCenter} y={_yFromCenter}");
                    break;
            }
        }

        private void RemoveCardFromView(SwipeDirection swipeDirection)
        {
            CGPoint finishPoint;
            nfloat rotation;

            if (swipeDirection == SwipeDirection.Left)
            {
                finishPoint = new CGPoint(-600, this.Center.Y);
                rotation = -1;

            }
            else if (swipeDirection == SwipeDirection.Right)
            {

                finishPoint = new CGPoint(600, this.Center.Y);
                rotation = 1;

            }
            else
            {
                LogTo.Fatal(() => "Unknown swipe direction, view will not be animated.");
                finishPoint = new CGPoint(this.Center.Y, this.Center.Y);
                rotation = 0;
            }

            UIView.Animate(duration: 0.3, animation: new Action(() =>
            {
                this.Center = finishPoint;
                this.Transform = CGAffineTransform.MakeRotation(rotation);
            }), completion: new Action(() =>
            {
                this.RemoveFromSuperview();

                LogTo.Debug(() => "View has been removed from the superview.");
            }));
        }

        /// <summary>
        /// When a swipe exceeds the ActionMargin to the right
        /// </summary>
        public void CardWasSwipedToTheLeft()
        {
            LogTo.Info(() => "View was swiped to the left.");
            if (OnSwipedToTheLeft != null)
            {
                OnSwipedToTheLeft.Invoke(_imageUrl, null);
            }
            RemoveCardFromView(SwipeDirection.Left);
        }

        /// <summary>
        /// When a swipe exceeds the ActionMargin to the left
        /// </summary>
        public void CardWasSwipedToTheRight()
        {
            LogTo.Info(() => "View was swiped to the right.");
            if (OnSwipedToTheRight != null)
            {
                OnSwipedToTheRight.Invoke(_imageUrl, null);
            }
            RemoveCardFromView(SwipeDirection.Right);
        }
    }
}

