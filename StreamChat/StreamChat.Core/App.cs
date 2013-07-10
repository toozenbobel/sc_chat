using Cirrious.CrossCore;
using Cirrious.CrossCore.IoC;
using StreamChat.Core.ChatLoaders;
using StreamChat.Core.ServiceContracts;

namespace StreamChat.Core
{
    public class App : Cirrious.MvvmCross.ViewModels.MvxApplication
    {
        public override void Initialize()
        {
            CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();
				
			Mvx.RegisterSingleton<IChatResolveService>(new ChatFactory());
			Mvx.RegisterSingleton<IChatContainer>(new ChatContainer());

            RegisterAppStart<ViewModels.MainPageViewModel>();
        }
    }
}