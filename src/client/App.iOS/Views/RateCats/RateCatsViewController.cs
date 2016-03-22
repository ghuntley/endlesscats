using System;

using UIKit;

using EndlessCatsApp.Services.Api;
using EndlessCatsApp.Services.Rating;
using EndlessCatsApp.Services.State;
using EndlessCatsApp.ViewModels;
using ReactiveUI;
using Splat;
using Anotar.Splat;
using System.Reactive.Linq;

namespace EndlessCatsApp.iOS.Views
{
    public partial class RateCatsViewController : ReactiveViewController, IViewFor<RateCatsViewModel>
    {
        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (RateCatsViewModel)value; }
        }

        private RateCatsView _rateCatsView;

        public RateCatsViewModel ViewModel { get; set; }

        public RateCatsViewController(CompositionRoot compositionRoot)
        {
            ViewModel = compositionRoot.ResolveRateCatsViewModel();


            this.WhenActivated(autoDispose =>
            {
                // automatically retrieve data from cache/api when the view is activated.
                ViewModel.Refresh.Execute(null);

                // bind the cats viewmodel property to the view viewmodel property.
                //this.WhenAnyValue(view => view.ViewModel.Cats).Subscribe(x =>
                //{
                //    _rateCatsView.ViewModel = x;
                //});

                autoDispose(this.Bind(ViewModel, vm => vm.Cats, v => v._rateCatsView.ViewModel));

                autoDispose(this.WhenAnyObservable(view => view._rateCatsView.Swipes).Where(direction => direction == SwipeDirection.Left).Subscribe(x =>
                {
                    ViewModel.DislikeCat.Execute(null);
                }));
                autoDispose(this.WhenAnyObservable(view => view._rateCatsView.Swipes).Where(direction => direction == SwipeDirection.Right).Subscribe(x =>
                {
                    ViewModel.LikeCat.Execute(null);
                }));


            });
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            _rateCatsView = new RateCatsView(View.Frame);
            this.View.AddSubview(_rateCatsView);
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        public override bool PrefersStatusBarHidden()
        {
            return true;
        }
    }
}


