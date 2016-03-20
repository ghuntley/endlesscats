using System;
using UIKit;
using CoreGraphics;
using Foundation;
using Anotar.Splat;

namespace EndlessCatsApp.iOS.View
{

    [Register("DraggableView")]
    public sealed class DraggableView : UIView
    {
        // distance from center where the aciton applies. Higher = swipe further before the action will be called.
        private const nint ActionMargin = 120;
        // how quicikly the card shirnks. Higher = shrinks less.
        private const nint ScaleStrenth = 4;
        // upper bar for how much the card shrinks. Higher = shrinks less.
        private const nfloat ScaleMax = 0.93f;
        // the maximum rotation allowed in radians. Higher = card can keep rotation longer.
        private const nint RotationMax = 1;
        // the strength of rotation. Higher = weaker rotation.
        private const nint RotationStrength = 320;
        // Higher = stronger rotation angle.
        private const nfloat RotationAngle = 3.14159f * 8;

        private readonly UIPanGestureRecognizer _panGestureRecognizer;
        private CGPoint _originalPoint;
        private nfloat _xFromCenter;
        private nfloat _yFromCenter;

        public DraggableView()
        {
            _panGestureRecognizer = new UIPanGestureRecognizer();
            _panGestureRecognizer.AddTarget(() => HandleCardDrag(_panGestureRecognizer));
            this.AddGestureRecognizer(_panGestureRecognizer);

            this.BackgroundColor = UIColor.White;
        }

        /// <summary>
        /// called when the card is let go.
        /// </summary>
        private void OnCardDragFinished()
        {
            if (_xFromCenter > ActionMargin)
            {
                OnCardSwipedRight();
            }
            else if (_xFromCenter < -ActionMargin)
            {
                OnCardSwipedLeft();
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
                    LogTo.Debug(() => $"View has started to be dragged - x={_xFromCenter} y={_yFromCenter}");
                    _originalPoint = this.Center;
                    break;

                case UIGestureRecognizerState.Changed:
                    LogTo.Debug(() => $"View is being dragged - x={_xFromCenter} y={_yFromCenter}");
                    
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
                    LogTo.Debug(() => $"View center has been changed to - x={x} y={y}");


                    // rotate by a certain amount
                    CGAffineTransform transform = CGAffineTransform.MakeRotation(angle);

                    // scale by a certain amount
                    CGAffineTransform scaleTransform = CGAffineTransform.Scale(transform, scale, scale);

                    // apply transformations
                    this.Transform = scaleTransform;
                    UpdateOverlay(_xFromCenter);

                    break;

                case UIGestureRecognizerState.Ended:
                    LogTo.Debug(() => $"View is no longer being dragged - x={_xFromCenter} y={_yFromCenter}");

                    OnCardDragFinished();
                    break;

                case UIGestureRecognizerState.Possible:
                    LogTo.Debug(() => $"View drag is possible - x={_xFromCenter} y={_yFromCenter}");
                    break;
    
                case UIGestureRecognizerState.Cancelled:
                    LogTo.Debug(() => $"View drag has been cancelled - x={_xFromCenter} y={_yFromCenter}");
                    break;

                case UIGestureRecognizerState.Failed:
                    LogTo.Debug(() => $"View drag has failed - x={_xFromCenter} y={_yFromCenter}");
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
        public void OnCardSwipedLeft()
        {
            RemoveCardFromView();
        }

        /// <summary>
        /// When a swipe exceeds the ActionMargin to the left
        /// </summary>
        public void OnCardSwipedRight()
        {
            RemoveCardFromView();
        }

    }
}

