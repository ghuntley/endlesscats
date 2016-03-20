using System;

using UIKit;

using EndlessCatsApp.Services.Api;
using EndlessCatsApp.Services.Rating;
using EndlessCatsApp.Services.State;
using EndlessCatsApp.ViewModels;
using ReactiveUI;
using Splat;


namespace EndlessCatsApp.iOS.Views
{
    public partial class RateCatsViewController : ReactiveViewController, IViewFor<RateCatsViewModel>
    {
        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (RateCatsViewModel)value; }
        }

        private UIView _rateCatsView;

        public RateCatsViewModel ViewModel { get; set; }

        public RateCatsViewController(CompositionRoot compositionRoot)
        {
            ViewModel = compositionRoot.ResolveRateCatsViewModel();
            //           ViewModel = 
            //new RateCatsViewModel(Locator.Current.GetService<ICatsApiService>(), Locator.Current.GetService<IStateService>(), Locator.Current.GetService<IRatingService>());


            this.WhenActivated(d =>
            {
                // automatically retrieve data from cache/api when the view is activated.
                ViewModel.Refresh.Execute(true);
            });
        }

        public override async void ViewDidLoad()
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


