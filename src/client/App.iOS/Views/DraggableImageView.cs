using System;
using UIKit;
using CoreGraphics;
using Foundation;
using SDWebImage;

using Anotar.Splat;
using EndlessCatsApp.Services.Api;
using EndlessCatsApp.Utility;

namespace EndlessCatsApp.iOS.View
{
    public interface IDraggableView
    {
        EventHandler OnSwipedToTheLeft { get; set; }

        EventHandler OnSwipedToTheRight { get; set; }

    }

    [Register("DraggableView")]
    public sealed class DraggableView : UIView, IDraggableView
    {
        // distance from center where the aciton applies. Higher = swipe further before the action will be called.
        private readonly nint ActionMargin = 120;
        // how quicikly the card shirnks. Higher = shrinks less.
        private readonly nint ScaleStrenth = 4;
        // upper bar for how much the card shrinks. Higher = shrinks less.
        private readonly nfloat ScaleMax = 0.93f;
        // the maximum rotation allowed in radians. Higher = card can keep rotation longer.
        private readonly nint RotationMax = 1;
        // the strength of rotation. Higher = weaker rotation.
        private readonly nint RotationStrength = 320;
        // Higher = stronger rotation angle.
        private readonly nfloat RotationAngle = 3.14159f * 8;

        private readonly UIImageView _imageView;
        private readonly UIPanGestureRecognizer _panGestureRecognizer;
        private readonly Uri _url;

        private CGPoint _originalPoint;
        private nfloat _xFromCenter;
        private nfloat _yFromCenter;


        public DraggableView(Uri url)
        {
            Ensure.ArgumentNotNull(url, nameof(url));
            _url = url;

            _panGestureRecognizer = new UIPanGestureRecognizer();
            _panGestureRecognizer.AddTarget(() => HandleCardDrag(_panGestureRecognizer));
            this.AddGestureRecognizer(_panGestureRecognizer);

            _imageView = new UIImageView(this.Frame);
            _imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            _imageView.SetImage(url: _url, 
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

            this.AddSubview(_imageView);


            //this.Opaque = true;
            // TODO: make this opqaue, and not red.
            //this.ClearsContextBeforeDrawing = false;
            //this.BackgroundColor = UIColor.Red;
            this.Layer.CornerRadius = 4;
            this.Layer.ShadowRadius = 4;
            this.Layer.ShadowOpacity = 0.2f;
            this.Layer.ShadowOffset = new CGSize(1, 1);
        }


        public EventHandler OnSwipedToTheLeft { get; set; }

        public EventHandler OnSwipedToTheRight { get; set; }

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
            else
            {
                // reset the card
                UIView.Animate(duration: 0.3, animation: new Action(() =>
                        {
                            this.Center = _originalPoint;
                            this.Transform = CGAffineTransform.MakeRotation(0);
                        }));
            }
            
        }

        private void UpdateOverlay(nfloat distance)
        {
            if (distance > 0)
            {
                // Right    
            }
            else
            {
                // Left
            }

            // overlayView.Alpha = Math.Min(Math.Abs(distance)/100, 0.4);
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
                    double strength = Math.Min(_xFromCenter / RotationStrength, RotationMax);

                    // degree in radians
                    nfloat angle = (nfloat)(RotationAngle * strength);

                    // amount the height changes when you move the card up to a certain point
                    nfloat scale = (nfloat)Math.Max(1 - Math.Abs(strength) / strength, ScaleMax);

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

        private void RemoveCardFromView()
        {
            CGPoint finishPoint = new CGPoint(500, 2 * _yFromCenter + _originalPoint.Y);
            UIView.Animate(duration: 0.3, animation: new Action(() =>
                    {
                        this.Center = finishPoint;
                    }), completion: new Action(() =>
                    {
                        this.RemoveFromSuperview();
                    }));
        }

        /// <summary>
        /// When a swipe exceeds the ActionMargin to the right
        /// </summary>
        public void CardWasSwipedToTheLeft()
        {
            if (OnSwipedToTheLeft != null)
            {
                OnSwipedToTheLeft.Invoke(_url, null);
            }
            RemoveCardFromView();
        }

        /// <summary>
        /// When a swipe exceeds the ActionMargin to the left
        /// </summary>
        public void CardWasSwipedToTheRight()
        {
            if (OnSwipedToTheRight != null)
            {
                OnSwipedToTheRight.Invoke(_url, null);
            }
            RemoveCardFromView();
        }
    }
}

