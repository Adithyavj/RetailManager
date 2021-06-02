using Caliburn.Micro;
using RMDesktopUI.EventModels;
using RMDesktopUI.Library.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RMDesktopUI.ViewModels
{
    public class LoginViewModel : Screen
    {
        // property with _ means that it is a backing field meaning
        // its sole purpose is to supply & hold values for this public property
        private string _userName = "adithyavj@gmail.com";
        private string _password = "Pwd12345.";
        // instance of apihelper interface
        private IAPIHelper _apiHelper;
        // instance of event aggergator
        private IEventAggregator _events;

        // We use constructor here to use dependency injection (only one instance of a class is created using interface
        // ie, we don't user class c1= new class here..
        public LoginViewModel(IAPIHelper apiHelper, IEventAggregator events)
        {
            _apiHelper = apiHelper;
            _events = events;
        }

        public string UserName
        {
            get { return _userName; }
            set 
            { 
                _userName = value;
                NotifyOfPropertyChange(() => UserName);
                NotifyOfPropertyChange(() => CanLogIn);
            }
        }

        public string Password
        {
            get { return _password; }
            set 
            {
                _password = value;
                NotifyOfPropertyChange(() => Password);
                NotifyOfPropertyChange(() => CanLogIn);
            }
        }

        public bool IsErrorVisible
        {
            get 
            {
                return ErrorMessage?.Length > 0 ? true : false;
            }
            
        }

        private string _errorMessage;

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                NotifyOfPropertyChange(() => IsErrorVisible);
                NotifyOfPropertyChange(() => ErrorMessage);
            }
        }

        public bool CanLogIn
        {
            get
            {
                bool output = false;
                if (UserName?.Length > 0 && Password?.Length > 0) // ? mark after property checks if it is null
                {
                    output = true;
                }
                return output;
            }
        }

        public async Task LogIn()
        {
            // When user hits login, this is fired, we pass the username and password to the authenticate method of apihelper
            try
            {
                ErrorMessage = "";
                var result = await _apiHelper.Authenticate(UserName, Password); //the result with be a Model - AuthenticateUser Model

                // Capture more information about the user from DB using the Authentication token
                // we pass the accesstoken we got to this method to get detailed info of user
                await _apiHelper.GetLoggedInUserInfo(result.Access_Token);

                // This lets everyone know someone has logged in
                // all listeners on the UI thread will know that this event fires
                // We created a Class (EventModel) for this just to know the event occured. The eventModel won't have any code
                await _events.PublishOnUIThreadAsync(new LogOnEvent(),new CancellationToken()); //broadcast.... LogOnEvent Happends


            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

    }
}
