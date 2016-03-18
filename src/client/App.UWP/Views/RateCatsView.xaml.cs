using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using EndlessCatsApp.Services.Api;
using EndlessCatsApp.Services.Rating;
using EndlessCatsApp.Services.State;
using EndlessCatsApp.ViewModels;
using ReactiveUI;
using Splat;

namespace App.UWP.Views
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RateCatsView : IViewFor<RateCatsViewModel>
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel",
            typeof(RateCatsViewModel),
            typeof(RateCatsView),
            new PropertyMetadata(default(RateCatsViewModel)));

        public RateCatsView()
        {
            InitializeComponent();
            ViewModel = new RateCatsViewModel(Locator.Current.GetService<ICatsApiService>(), Locator.Current.GetService<IStateService>(), Locator.Current.GetService<IRatingService>());

            this.WhenActivated(d =>
            {
                // automatically retrieve data from cache/api when the view is activated.
                ViewModel.Refresh.Execute(true);
            });
        }

        private RateCatsViewModel BindingRoot => ViewModel;


        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (RateCatsViewModel)value; }
        }

        public RateCatsViewModel ViewModel
        {
            get { return (RateCatsViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        private void OnRefreshRequested(object sender, EventArgs e)
        {
            ViewModel.Refresh.Execute(true);
        }

    }
}