﻿using System.Reactive.Linq;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using Toggl.Foundation.Login;
using Toggl.Foundation.MvvmCross.Services;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Multivac;
using Toggl.PrimeRadiant.Settings;

namespace Toggl.Foundation.MvvmCross
{
    public sealed class App : MvxApplication
    {
        public override void Initialize()
        {

        }

        public void Initialize(ILoginManager loginManager, IMvxNavigationService navigationService, IAccessRestrictionStorage accessRestrictionStorage)
        {
            Ensure.Argument.IsNotNull(loginManager, nameof(loginManager));
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));
            Ensure.Argument.IsNotNull(accessRestrictionStorage, nameof(accessRestrictionStorage));

            Mvx.RegisterSingleton<IPasswordManagerService>(new StubPasswordManagerService());

            RegisterAppStart(new AppStart(loginManager, navigationService, accessRestrictionStorage));
        }
    }

    public sealed class AppStart : IMvxAppStart
    {
        private readonly ILoginManager loginManager;
        private readonly IMvxNavigationService navigationService;
        private readonly IAccessRestrictionStorage accessRestrictionStorage;

        public AppStart(ILoginManager loginManager, IMvxNavigationService navigationService, IAccessRestrictionStorage accessRestrictionStorage)
        {
            Ensure.Argument.IsNotNull(loginManager, nameof(loginManager));
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));
            Ensure.Argument.IsNotNull(accessRestrictionStorage, nameof(accessRestrictionStorage));

            this.loginManager = loginManager;
            this.navigationService = navigationService;
            this.accessRestrictionStorage = accessRestrictionStorage;
        }

        public async void Start(object hint = null)
        {
            Mvx.RegisterSingleton(loginManager);

            if (accessRestrictionStorage.IsApiOutdated() || accessRestrictionStorage.IsClientOutdated())
            {
                await navigationService.Navigate<OnboardingViewModel>();
                await navigationService.Navigate<OutdatedAppViewModel>();
                return;
            }

            var dataSource = loginManager.GetDataSourceIfLoggedIn();
            if (dataSource == null)
            {
                await navigationService.Navigate<OnboardingViewModel>();
                return;
            }

            Mvx.RegisterSingleton(dataSource);

            var user = dataSource.User.Current.Wait();
            if (accessRestrictionStorage.IsUnauthorized(user.ApiToken))
            {
                await navigationService.Navigate<TokenResetViewModel>();
                return;
            }

            var _ = dataSource.SyncManager.ForceFullSync();

            await navigationService.Navigate<MainViewModel>();
        }
    }
}
