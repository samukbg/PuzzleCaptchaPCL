using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using PuzzleCaptchaPCL.Models;
using PuzzleCaptchaPCL.Services;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace PuzzleCaptchaPCL
{
    public partial class PuzzleCaptcha : ContentView
    {
        PuzzleService puzzleService;

        Puzzle resultMap;

        PuzzleInfo submissionInfo;
        PuzzleInfo recordInfo;
        private SKBitmap bgImage;
        private SKBitmap picImage;
        public static readonly BindableProperty ImageCollectionProperty = BindableProperty.Create(
                                                                        "ImageCollection",
                                                                        typeof(ObservableCollection<object>),
                                                                        typeof(PuzzleCaptcha),
                                                                        new ObservableCollection<object>());

        public static readonly BindableProperty SliderThumbImageProperty = BindableProperty.Create(
                                                                        "SliderThumbImage",
                                                                        typeof(string),
                                                                        typeof(PuzzleCaptcha),
                                                                        "");

        public static readonly BindableProperty ReloadButtonImageProperty = BindableProperty.Create(
                                                                        "ReloadButtonImage",
                                                                        typeof(string),
                                                                        typeof(PuzzleCaptcha),
                                                                        "");

        public static readonly BindableProperty IsRemoteProperty = BindableProperty.Create(
                                                                        "IsRemote",
                                                                        typeof(bool),
                                                                        typeof(PuzzleCaptcha),
                                                                        false);

        public event Action<bool,int> MatchAction; // Match event

        public ObservableCollection<object> ImageCollection
        {
            get => (ObservableCollection<object>)GetValue(ImageCollectionProperty);
            set => SetValue(ImageCollectionProperty, value);
        }

        public string SliderThumbImage
        {
            get => (string)GetValue(SliderThumbImageProperty);
            set => SetValue(SliderThumbImageProperty, value);
        }

        public string ReloadButtonImage
        {
            get => (string)GetValue(ReloadButtonImageProperty);
            set => SetValue(ReloadButtonImageProperty, value);
        }

        public bool IsRemote
        {
            get => (bool)GetValue(IsRemoteProperty);
            set => SetValue(IsRemoteProperty, value);
        }


        public PuzzleCaptcha()
        {
            InitializeComponent();
        }

        public async void ShufflePuzzle()
        {
            slider.Value = 0;

            if (string.IsNullOrEmpty(ReloadButtonImage))
                reloadBtn.Text = "o";

            slider.ThumbImageSource = ImageSource.FromFile(SliderThumbImage);

            reloadBtn.ImageSource = ImageSource.FromFile(ReloadButtonImage);

            puzzleService = new PuzzleService();

            //Random local image
            var rand = new Random();
            if (ImageCollection == null)
            {
                Console.Write(" - ImageCollection is null");
            }
            else if (ImageCollection.Count == 0)
            {
                Console.Write(" - ImageCollection is empty");
            }
            else
            {
                var chosenImageSource = ImageCollection[rand.Next(ImageCollection.Count)];

                //ToDo: Finish the URL automatic recognition
                //Uri uriResult;
                //bool isRemoteCollection = Uri.TryCreate(chosenImageSource, UriKind.Absolute, out uriResult)
                //    && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

                // Creating puzzle
                if (IsRemote)
                    resultMap = await puzzleService.CreateRemotePuzzleAsync((chosenImageSource as string));
                else
                    resultMap = puzzleService.CreateLocalPuzzleAsync((chosenImageSource as Stream));

                
                picImage = resultMap.MissingPieceImage;
                bgImage = resultMap.BackgroundImage;


                backgroundView.InvalidateSurface();
                pieceView.InvalidateSurface();
               
                submissionInfo = new PuzzleInfo
                {
                    Id = resultMap.Id,
                    X = resultMap.X,
                    SubmittedAt = DateTimeOffset.Now
                };
            }
        }


        void OnBGPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();

            if (bgImage != null)
            {

                float scale = Math.Min((float)info.Width / bgImage.Width,
                                       (float)info.Height / bgImage.Height);
                float x = (info.Width - scale * bgImage.Width) / 2;
                float y = (info.Height - scale * bgImage.Height) / 2;
                SKRect destRect = new SKRect(x, y, x + scale * bgImage.Width,
                                                   y + scale * bgImage.Height);

                canvas.DrawBitmap(bgImage, destRect);
            }
        }

        void OnPiecePaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;


            canvas.Clear();

            if (picImage != null)
            {
                canvas.DrawBitmap(picImage, resultMap.MissingPieceImage.Info.Rect);
                pieceView.TranslationY = resultMap.Y / 3;
            }
        }


        void OnSlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            double value = e.NewValue;
            pieceView.TranslationX = value * 1.2;

        }

        void OnSlider_DragCompleted(object sender, EventArgs e)
        {
            submissionInfo.NumberOfTries++;

            recordInfo = new PuzzleInfo
            {
                Id = resultMap.Id,
                X = (int)(pieceView.TranslationX * 3),
                ExpiredAt = DateTimeOffset.Now.AddSeconds(30)
            };

            bool isPuzzleSolved = puzzleService.IsPuzzleSolved(submissionInfo, recordInfo);

            if (MatchAction != null) // check if subscribed
            {
                if (submissionInfo.NumberOfTries < 3)
                {
                    if (isPuzzleSolved)
                        MatchAction(true, submissionInfo.NumberOfTries);
                    else
                    {
                        MatchAction(false, submissionInfo.NumberOfTries);
                    }
                }
                else
                {
                    MatchAction(false, submissionInfo.NumberOfTries);
                    ShufflePuzzle(); //Reload puzzle after 3 tries
                }
            }

            slider.Value = 0;
        }

        void OnReload_Click(object sender, EventArgs e)
        {
            ShufflePuzzle();
        }
    }
}
