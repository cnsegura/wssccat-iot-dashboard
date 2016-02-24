﻿/**
 * This is a port of a project from hackster.io
 *https://www.hackster.io/LaurenBuchholz/restful-weather-5bc747
**/
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Core;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ECGCAT_IoT_Vending_Client
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const int LED_PIN = 12;
        private GpioPin pin;
        private GpioPinValue pinValue;
        private DispatcherTimer LoggerTimer;
        private DispatcherTimer Timer;
        //A class which wraps the color sensor
        TCS34725 colorSensor;
        //A class which wraps the barometric sensor
        BMP280 BMP280;
        
        private const string I2C_CONTROLLER_NAME = "I2C1"; //use for RPI2
        
        public MainPage()
        {
            this.InitializeComponent();

            LoggerTimer = new DispatcherTimer();
            LoggerTimer.Interval = TimeSpan.FromMinutes(1);
            LoggerTimer.Tick += LoggerTimer_Tick;

            InitGPIO();
        }

        private void InitGPIO()
        {
            var gpio = GpioController.GetDefault();

            // Show an error if there is no GPIO controller
            if (gpio == null)
            {
                pin = null;
                //GpioStatus.Text = "There is no GPIO controller on this device.";
                return;
            }

            pin = gpio.OpenPin(LED_PIN);
            pinValue = GpioPinValue.High;
            pin.Write(pinValue);
            pin.SetDriveMode(GpioPinDriveMode.Output);

            //GpioStatus.Text = "GPIO pin initialized correctly.";

        }

        private void LoggerTimer_Tick(object sender, object e)
        {
            if (pinValue == GpioPinValue.High)
            {
                pinValue = GpioPinValue.Low;
                pin.Write(pinValue);
                ReadWeatherData().ContinueWith((t) =>
                {
                    WeatherData wd = t.Result;
                    
                    Debug.WriteLine(wd.Created);
                    //Write the values to your debug console
                    Debug.WriteLine("Created: " + wd.Created + " ft");
                    txtTime.Text = "Created: " + wd.Created + " ft";
                    Debug.WriteLine("Temperature: " + wd.TemperatureinF + " deg F");
                    txtTemp.Text = "Temperature: " + wd.TemperatureinF + " deg F";
                    Debug.WriteLine("Pressure: " + wd.Pressureinmb + " mb");
                    txtPressure.Text = "Pressure: " + wd.Pressureinmb + " mb";
                    

                }, TaskScheduler.FromCurrentSynchronizationContext());
                ReadLightData().ContinueWith((t) =>
                {
                    LightData ld = t.Result;
                    
                    Debug.WriteLine("Lux: " + ld.Lux);
                    Debug.WriteLine("Color Temp:" + ld.ColorTempinK + " K");
                    
                }, TaskScheduler.FromCurrentSynchronizationContext());
                pinValue = GpioPinValue.High;
                pin.Write(pinValue);
            }
            else
            {
                pinValue = GpioPinValue.High;
                pin.Write(pinValue);
            }
        }

        private async Task<WeatherData> ReadWeatherData()
        {
            WeatherData wd = null;
            try
            {
                if (BMP280 == null)
                {
                    //Create a new object for our barometric sensor class
                    BMP280 = new BMP280();
                    //Initialize the sensor
                    await BMP280.Initialize();
                }

                //Create variables to store the sensor data: temperature, pressure and altitude. 
                //Initialize them to 0.
                float temp = 0;
                float pressure = 0;
                float altitude = 0;

                //Create a constant for pressure at sea level. 
                //This is based on your local sea level pressure (Unit: Hectopascal)
                const float seaLevelPressure = 1018.34f;

                temp = await BMP280.ReadTemperature();
                temp = ConvertUnits.ConvertCelsiusToFahrenheit(temp);
                pressure = await BMP280.ReadPreasure();
                pressure = ConvertUnits.ConvertPascalToMillibar(pressure);
                altitude = await BMP280.ReadAltitude(seaLevelPressure);
                altitude = ConvertUnits.ConvertMeterToFoot(altitude);

                wd = new WeatherData();
                wd.Created = DateTime.Now;
                wd.TemperatureinF = temp;
                wd.Pressureinmb = pressure;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return wd;
        }

        private async Task<LightData> ReadLightData()
        {
            LightData ld = null;
            try
            {
                if (colorSensor == null)
                {
                    colorSensor = new TCS34725();
                    await colorSensor.Initialize();
                }

                //Read the approximate color from the sensor
                string colorRead = await colorSensor.getClosestColor();
                //Output the colr name to the speaker
                await Speak(" The current color is: " + colorRead);
                RgbData rgb = await colorSensor.getRgbData();

                float lux = TCS34725.getLuxSimple(rgb);
                ld = new LightData();
                ld.Created = DateTime.Now;
                ld.rgbData = rgb;
                ld.Lux = lux;
                ld.ColorTempinK = TCS34725.calculateColorTemperature(rgb);
                Debug.WriteLine("Current lux: " + lux);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return ld;
        }
    }
}
