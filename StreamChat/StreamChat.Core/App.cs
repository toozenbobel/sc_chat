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

			CreatableTypes()
                .EndingWith("Loader")
                .AsInterfaces()
                .RegisterAsLazySingleton();

			Mvx.RegisterSingleton<IChatResolveService>(new ChatFactory());
			new [] { typeof(ChatContainer) }.AsInterfaces().RegisterAsLazySingleton();

            RegisterAppStart<ViewModels.MainPageViewModel>();


        }
    }
}