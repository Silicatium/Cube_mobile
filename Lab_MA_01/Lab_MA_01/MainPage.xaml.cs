using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Collections;
using System.Runtime.CompilerServices;
using Xamarin.Forms.Shapes;
using static Xamarin.Essentials.Permissions;

namespace Lab_MA_01
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        bool activatedAccelerometer = false;
        bool activatedGyroscope = false;

        Label labelAccelerometer = new Label();
        Label labelGyroscope = new Label();

        Button button = new Button();
        Switch swh = new Switch();
        Slider speedSlider = new Slider();

        StackLayout stack = new StackLayout();
        RelativeLayout relative = new RelativeLayout();

        Color colorButton = Color.Red;
        Color colorBackground = Color.DarkCyan;

        int maxSize = 200;
        int minSize = 50;

        protected override void OnAppearing()
        {
            Gyroscope.ReadingChanged += Gyroscope_ReadingChanged;
            Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
            swh.Toggled += Swh_Toggled;

            var width = DeviceDisplay.MainDisplayInfo.Width;
            var height = DeviceDisplay.MainDisplayInfo.Height;

            speedSlider.Minimum = 0;
            speedSlider.Maximum = 25;
            speedSlider.WidthRequest = 150;
            speedSlider.Rotation = -90;

            stack.BackgroundColor = Color.GreenYellow;

            relative.BackgroundColor = colorBackground;
            relative.Margin = 10;
            stack.Children.Add(relative);

            button = createButton(colorButton, 100, 100);
            relative.Children.Add(button,
                Constraint.RelativeToParent((parent) => 0), // X = 0
                Constraint.RelativeToParent((parent) => 0), // Y = 0
                Constraint.RelativeToParent((parent) => parent.Width),
                Constraint.RelativeToParent((parent) => parent.Height));

            relative.Children.Add(labelAccelerometer,
                xConstraint: Constraint.RelativeToParent((parent) => 20),
                yConstraint: Constraint.RelativeToParent((parent) => parent.Height - 50));

            relative.Children.Add(swh,
                xConstraint: Constraint.RelativeToParent((parent) => 10),
                yConstraint: Constraint.RelativeToParent((parent) => 10));

            relative.Children.Add(labelGyroscope,
                xConstraint: Constraint.RelativeToParent((parent) => parent.Width - 140),
                yConstraint: Constraint.RelativeToParent((parent) => 15));

            relative.Children.Add(speedSlider,
                xConstraint: Constraint.RelativeToParent((parent) => -45),
                yConstraint: Constraint.RelativeToParent((parent) => 105));

            Content = stack;
        }

        private void Swh_Toggled(object sender, ToggledEventArgs e)
        {
            if (stack.BackgroundColor != Color.Gold) stack.BackgroundColor = Color.Gold;
            else stack.BackgroundColor = Color.GreenYellow;

            Switch temp = sender as Switch;
            if (!(temp is null))
            {
                try
                {
                    if (!activatedGyroscope) { activatedGyroscope = true; Gyroscope.Start(SensorSpeed.UI); }
                    else { activatedGyroscope = false; Gyroscope.Stop(); labelGyroscope.Text = ""; }

                }
                catch (Exception) { 
                    if (activatedGyroscope) labelGyroscope.Text = "No working";
                    else labelGyroscope.Text = "";
                };
            }
        }

        private void Gyroscope_ReadingChanged(object sender, GyroscopeChangedEventArgs e)
        {
            var X = e.Reading.AngularVelocity.X;
            var Y = e.Reading.AngularVelocity.Y;
            var Z = e.Reading.AngularVelocity.Z;
            int sizeChange = 5;

            labelGyroscope.Text = $"Г: {Math.Round(X, 3)} {Math.Round(Y, 3)} {Math.Round(Z, 3)}";
            if (X < -0.5 && button.Width > minSize) {button.WidthRequest -= sizeChange; button.HeightRequest -= sizeChange;}
            else if (X > 0.5 && button.Width < maxSize && button.X > 0 && button.X + button.Width < relative.Width 
                && button.Y > 0 && button.Y + button.Height < relative.Height) { button.WidthRequest += sizeChange; button.HeightRequest += sizeChange; }

        }

        private Button createButton(Color color, double width, double height)
        {
            Button button = new Button();
            button.BackgroundColor = color;
            button.HeightRequest = height;
            button.WidthRequest = width;
            button.CornerRadius = 10;
            button.HorizontalOptions = LayoutOptions.Center;
            button.VerticalOptions = LayoutOptions.Center;
            button.Clicked += Button_Clicked;

            return button;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Button temp = sender as Button;
            if (!(temp is null))
            {
                if (!activatedAccelerometer) { activatedAccelerometer = true; button.BackgroundColor = Color.White; Accelerometer.Start(SensorSpeed.UI); }
                else { activatedAccelerometer = false; Accelerometer.Stop(); button.BackgroundColor = colorButton; labelAccelerometer.Text = ""; }
            }
        }

        private void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
        {
            var X = e.Reading.Acceleration.X;
            var Y = e.Reading.Acceleration.Y;
            var Z = e.Reading.Acceleration.Z;
            var speed = speedSlider.Value;

            labelAccelerometer.Text = $"А: {Math.Round(X, 3)} {Math.Round(Y, 3)} {Math.Round(Z, 3)}";

            if (X > 0.3 && button.X >= 5)
            {
                button.Margin = new Thickness(button.Margin.Left - speed, button.Margin.Top, button.Margin.Right, button.Margin.Bottom);
            }
            else if (X < -0.3 && button.X + button.Width <= relative.Width - 5)
            {
                button.Margin = new Thickness(button.Margin.Left + speed, button.Margin.Top, button.Margin.Right, button.Margin.Bottom);
            }
            else if (Math.Abs(X) > 0.3)
            {
                Vibration.Vibrate();
            }

            if (Y > 0.3 && button.Y + button.Height <= relative.Height - 10)
            {
                button.Margin = new Thickness(button.Margin.Left, button.Margin.Top + speed, button.Margin.Right, button.Margin.Bottom);
            }
            else if (Y < -0.3 && button.Y >= 0)
            {
                button.Margin = new Thickness(button.Margin.Left, button.Margin.Top - speed, button.Margin.Right, button.Margin.Bottom);
            }
            else if (Math.Abs(Y) > 0.3)
            {
                Vibration.Vibrate();
            }

            relative.ForceLayout();
        }
    }
    
}
