using System;

using UIKit;

using EndlessCatsApp.Services.Api;
using EndlessCatsApp.Services.Rating;
using EndlessCatsApp.Services.State;
using EndlessCatsApp.ViewModels;
using ReactiveUI;
using Splat;


namespace EndlessCatsApp.iOS
{
    public partial class RateCatsViewController : ReactiveViewController, IViewFor<RateCatsViewModel>
    {
        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (RateCatsViewModel)value; }
        }

        public RateCatsViewModel ViewModel { get;  set; }

        public RateCatsViewController () : base ("RateCatsViewController", null)
        {
            ViewModel = new RateCatsViewModel(Locator.Current.GetService<ICatsApiService>(), Locator.Current.GetService<IStateService>(), Locator.Current.GetService<IRatingService>());


            this.WhenActivated(d =>
            {
                // automatically retrieve data from cache/api when the view is activated.
                ViewModel.Refresh.Execute(true);
            });
        }

        public override async void ViewDidLoad ()
        {
            base.ViewDidLoad ();
            // Perform any additional setup after loading the view, typically from a nib.

            ICatsApiService service = Locator.Current.GetService<ICatsApiService> ();

            var data = await service.Background.GetCatsTwo();

        }

        public override void DidReceiveMemoryWarning ()
        {
            base.DidReceiveMemoryWarning ();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}


