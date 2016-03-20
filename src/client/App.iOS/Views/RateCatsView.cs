using System;
using UIKit;
using CoreGraphics;
using Foundation;
using Anotar.Splat;

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

        public RateCatsView ()
        {

            this.LayoutSubviews ();
            SetupView ();
        }

        public void SetupView ()
        {

        }
    }
}

