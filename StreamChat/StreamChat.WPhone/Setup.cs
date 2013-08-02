using Cirrious.CrossCore;
using Cirrious.CrossCore.Platform;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.MvvmCross.WindowsPhone.Platform;
using Microsoft.Phone.Controls;
using StreamChat.Core.ServiceContracts;
using StreamChat.WPhone.Services;

namespace StreamChat.WPhone
{
    public class Setup : MvxPhoneSetup
    {
        public Setup(PhoneApplicationFrame rootFrame) : base(rootFrame)
        {
        }

        protected override IMvxApplication CreateApp()
        {
			Mvx.LazyConstructAndRegisterSingleton<IScreenLockService, ScreenLockService>();

            return new Core.App();
        }
		
        protected override IMvxTrace CreateDebugTrace()
        {
            return new DebugTrace();
        }
    }
}