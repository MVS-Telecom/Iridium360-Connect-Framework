using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BivyStick.Sample
{
    public partial class MainPage : ContentPage
    {
        private BivyStickFramework framework;

        public MainPage()
        {
            InitializeComponent();

            framework = new BivyStickFramework();
            framework.BatteryUpdated += Framework_BatteryUpdated;
            framework.DeviceConnected += Framework_DeviceConnected;
            framework.DeviceDisconnected += Framework_DeviceDisconnected;
            framework.DeviceDiscovered += Framework_DeviceDiscovered;
        }

        private void Framework_DeviceDiscovered(object sender, DeviceDiscoveredEventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                bool ok = await this.DisplayAlert("Device found", $"{e.Device.Name}", "Connect", "Cancel");

                if (ok)
                    await framework.Connect(e.Device.Id);
            });
        }

        private void Framework_DeviceDisconnected(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                deviceState.Text = $"Disconnected";
            });
        }

        private void Framework_DeviceConnected(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                deviceState.Text = $"Connected";
            });
        }

        private void Framework_BatteryUpdated(object sender, BatteryUpdatedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                battery.Text = $"{e.Value}%";
            });
        }

        private async void startSearch_Clicked(object sender, EventArgs e)
        {
            await framework.StartSearch();
        }

        private async void stopSearch_Clicked(object sender, EventArgs e)
        {
            await framework.StopSearch();
        }

        private async void emailSend_Clicked(object sender, EventArgs e)
        {
            await framework.SendEmail(email.Text, emailText.Text);
            email.Text = emailText.Text = string.Empty;
        }

        private async void smsSend_Clicked(object sender, EventArgs e)
        {
            await framework.SendSms(sms.Text, sms.Text);
            sms.Text = sms.Text = string.Empty;
        }
    }
}
