using System;

using Xamarin.Forms;
using WebSocket.Portable;

namespace WebsocketTest
{
    public class App : Application
    {
        public App()
        {
            MainPage = new SocketTestPage2();
        }

        protected override void OnStart()
        {

        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}

