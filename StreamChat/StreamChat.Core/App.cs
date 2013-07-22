using Cirrious.CrossCore;
using Cirrious.CrossCore.IoC;
using StreamChat.Core.ChatLoaders;
using StreamChat.Core.Chats;
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

	        ChatFactory factory = new ChatFactory();
	
			Mvx.RegisterSingleton<IChatResolveService>(factory);
			new [] { typeof(ChatContainer) }.AsInterfaces().RegisterAsLazySingleton();



            RegisterAppStart<ViewModels.MainPageViewModel>();


        }
    }
}