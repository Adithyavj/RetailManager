using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using RMDesktopUI.EventModels;
using RMDesktopUI.Library.Api;
using RMDesktopUI.Library.Models;

namespace RMDesktopUI.ViewModels
{
    public class ShellViewModel : Conductor<object>, IHandle<LogOnEvent>
    {
        private SalesViewModel _salesVM;
        private IEventAggregator _events;
        private ILoggedInUserModel _user;
        private IAPIHelper _apiHelper;

        public ShellViewModel(IEventAggregator events, SalesViewModel salesVM, ILoggedInUserModel user, IAPIHelper apiHelper)
        {
            // Constructor dependancy injection
            _events = events;
            _salesVM = salesVM;
            _user = user;
            _apiHelper = apiHelper;
            // Subscribes the shellviewModel to listen to the events
            _events.SubscribeOnPublishedThread(this);

            // Activate LoginViewModel (PopUp Login Screen)
            ActivateItemAsync(IoC.Get<LoginViewModel>(),new CancellationToken());
        }

        public bool IsLoggedIn
        {
            get
            {
                bool output = false;

                if (string.IsNullOrWhiteSpace(_user.Token) == false)
                {
                    output = true;
                }

                return output;
            }
        }

        // To close our WPF application
        public void ExitApplication()
        {
            TryCloseAsync();
        }

        // Logout from our applcation
        public async Task LogOut()
        {
            // Close out everything and reset the User
            _user.ResetUserModel();
            // Clear default headers in API
            _apiHelper.LogOffUser();
            // Activate loginVM
            await ActivateItemAsync(IoC.Get<LoginViewModel>(),new CancellationToken());
            // Notify UI that user has loggedOff
            NotifyOfPropertyChange(() => IsLoggedIn);
        }

        public async Task UserManagement()
        {
            // Activate UserVM
            await ActivateItemAsync(IoC.Get<UserDisplayViewModel>(),new CancellationToken());
        }

        //// The ShellViewModel Listens for the LogOnEvent fired in the LoginViewModel.....
        //// And the IHandle handles what to do in case this event is fired
        //public void Handle(LogOnEvent message)
        //{
        //    // Close LogInForm and Open SalesForm

        //    // To activate the Sales Screen
        //    ActivateItem(_salesVM);
        //    // In SimpleContainer system only single container can be open at all times, so on activating
        //    // the SalesVM, LoginVM will close. But we need to somehow destory the instance that was created.
        //    // we can do this by creating a new instance.
        //    // This is always done in the constructor of this class.
        //    // it always creates a new instance


        //    NotifyOfPropertyChange(() => IsLoggedIn);
        //}

        public async Task HandleAsync(LogOnEvent message, CancellationToken cancellationToken)
        {
            // Close LogInForm and Open SalesForm

            // To activate the Sales Screen
            await ActivateItemAsync(_salesVM, cancellationToken);
            // In SimpleContainer system only single container can be open at all times, so on activating
            // the SalesVM, LoginVM will close. But we need to somehow destory the instance that was created.
            // we can do this by creating a new instance.
            // This is always done in the constructor of this class.
            // it always creates a new instance

            NotifyOfPropertyChange(() => IsLoggedIn);
        }
    }
}
