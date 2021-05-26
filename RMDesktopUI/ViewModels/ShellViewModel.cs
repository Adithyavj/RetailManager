using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using RMDesktopUI.EventModels;

namespace RMDesktopUI.ViewModels
{
    public class ShellViewModel : Conductor<object>, IHandle<LogOnEvent>
    {
        private SalesViewModel _salesVM;
        private IEventAggregator _events;

        public ShellViewModel(IEventAggregator events, SalesViewModel salesVM)
        {
            // Constructor dependancy injection
            _events = events;
            _salesVM = salesVM;

            // Subscribes the shellviewModel to listen to the events
            _events.Subscribe(this);

            // Activate LoginViewModel (PopUp Login Screen)
            ActivateItem(IoC.Get<LoginViewModel>());
        }

        // The ShellViewModel Listens for the LogOnEvent fired in the LoginViewModel.....
        // And the IHandle handles what to do in case this event is fired
        public void Handle(LogOnEvent message)
        {
            // Close LogInForm and Open SalesForm

            // To activate the Sales Screen
            ActivateItem(_salesVM);
            // In SimpleContainer system only single container can be open at all times, so on activating
            // the SalesVM, LoginVM will close. But we need to somehow destory the instance that was created.
            // we can do this by creating a new instance.
            // This is always done in the constructor of this class.
            // it always creates a new instance
        }
    }
}
