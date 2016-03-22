using System;
using UIKit;
using CoreGraphics;
using Foundation;
using Anotar.Splat;
using EndlessCatsApp.Utility;
using EndlessCatsApp.Services.Api;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

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

            this.WhenActivated(autoDispose =>
            {
                this.WhenAnyObservable(x => x.ViewModel.ItemsAdded).Subscribe(x =>
                   {
                       if (this.Subviews.Length < 10)
                       {
                           var view = CreateDraggableImageView(x);

                           // insert additional cats behind the active cat
                           if (this.Subviews.Length > 1)
                           {
                               this.InsertSubviewBelow(view, this.Subviews[1]);
                           }
                           else {
                               this.InsertSubviewBelow(view, this);
                           }
                       }
                   });
            });
        }

        public ReactiveList<Cat> ViewModel
        {
            get { return _viewModel; }
            set
            {
                _viewModel = value;
            }
        }

        private ReactiveList<Cat> _viewModel;


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

        public IObservable<SwipeDirection> Swipes => _swipeDirection.AsObservable();
        private Subject<SwipeDirection> _swipeDirection = new Subject<SwipeDirection>();

        private void OnViewSwipedToTheLeft(object sender, EventArgs e)
        {
            _swipeDirection.OnNext(SwipeDirection.Left);
        }

        private void OnViewSwipedToTheRight(object sender, EventArgs e)
        {
            _swipeDirection.OnNext(SwipeDirection.Right);
        }
    }
}

