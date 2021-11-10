using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using PuzzleSample.ViewModels;
using Xamarin.Forms;

namespace PuzzleSample
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<string> ImageCollection { get; set; }
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

        }


        void OpenRemotePuzzleCaptcha(object sender, EventArgs args)
        {
            var pageModel = (BindingContext as MainViewModel);
            pageModel.ImageCollection = new ObservableCollection<object>();
            pageModel.ImageCollection.Clear();
            pageModel.ImageCollection.Add("https://wallpapertip.com/wmimgs/211-2117975_wallpaper-gunung-hd.jpg");
            pageModel.ImageCollection.Add("https://desktopbackground.org/p/2013/04/05/556333_backgrounds-one-piece-by-xxhxaxx-on-deviantart_1024x574_h.jpg");
            pageModel.ImageCollection.Add("https://teahub.io/photos/full/31-313973_go-wallpapers.jpg");

            Device.BeginInvokeOnMainThread(() =>
            {
                captchaStack.IsRemote = true;
                captchaStack.ShufflePuzzle();

                captchaStack.MatchAction += MatchAction;
            });

            ShowCaptcha();
        }

        void OpenLocalPuzzleCaptcha(object sender, EventArgs args)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            var pageModel = (BindingContext as MainViewModel);
            pageModel.ImageCollection = new ObservableCollection<object>();
            pageModel.ImageCollection.Clear();
            pageModel.ImageCollection.Add(assembly.GetManifestResourceStream("PuzzleSample.Resources.captcha1.png"));
            pageModel.ImageCollection.Add(assembly.GetManifestResourceStream("PuzzleSample.Resources.captcha2.png"));
            pageModel.ImageCollection.Add(assembly.GetManifestResourceStream("PuzzleSample.Resources.captcha3.png"));
            pageModel.ImageCollection.Add(assembly.GetManifestResourceStream("PuzzleSample.Resources.captcha4.png"));
            pageModel.ImageCollection.Add(assembly.GetManifestResourceStream("PuzzleSample.Resources.captcha5.png"));
            pageModel.ImageCollection.Add(assembly.GetManifestResourceStream("PuzzleSample.Resources.captcha6.png"));
            
            Device.BeginInvokeOnMainThread(() =>
            {
                captchaStack.IsRemote = false;
                captchaStack.ShufflePuzzle();

                captchaStack.MatchAction += MatchAction;
            });

            ShowCaptcha();

        }

        async void ShowCaptcha()
        {
            wrapperStack.IsVisible = true;
            captchaStack.IsVisible = true;

            await Task.WhenAll(
                wrapperStack.FadeTo(.7, 500, Easing.SpringIn),
                captchaStack.FadeTo(1, 500, Easing.SpringIn),
                captchaStack.TranslateTo(0, 0, 500, Easing.SpringOut)
            );
        }

        public void HideCaptcha()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Task.WhenAll(
                    wrapperStack.FadeTo(0, 500, Easing.SpringOut),
                    captchaStack.FadeTo(0, 500, Easing.SpringOut)
                );

                wrapperStack.IsVisible = false;
                captchaStack.IsVisible = false;

            });
        }

        async void MatchAction(bool success, int tries)
        {
            if (success)
            {
                await DisplayAlert("Success", "The captcha matches!", "OK");
                HideCaptcha();
            }
            else
                await DisplayAlert("Failed", $"Try again ({tries})", "OK");

        }
    }
}
