using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Total_DMX_Control_WPF.FixtureClasses;

namespace Total_DMX_Control_WPF
{
    /// <summary>
    /// Interaction logic for EffectsDesk.xaml
    /// </summary>
    public partial class EffectsDesk : Window
    {
        #region Data Members
        private Effect[] _parEffects = new Effect[13];
        private Effect[] _ledEffects = new Effect[9];
        private Effect[] _CSEffects = new Effect[7];
        private Effect[] _otherEffects = new Effect[6];
        
        private Effect PAR01 = new Effect("PAR01");
        private Effect PAR02 = new Effect("PAR02");
        private Effect PAR03 = new Effect("PAR03");
        private Effect PAR04 = new Effect("PAR04");
        private Effect PAR05 = new Effect("PAR05");
        private Effect PAR06 = new Effect("PAR06");
        private Effect PAR07 = new Effect("PAR07");
        private Effect PAR08 = new Effect("PAR08");
        private Effect PAR09 = new Effect("PAR09");
        private Effect PAR10 = new Effect("PAR10");
        private Effect PAR11 = new Effect("PAR11");
        private Effect PAR12 = new Effect("PAR12");

        private Effect LED01 = new Effect("LED01");
        private Effect LED02 = new Effect("LED02");
        private Effect LED03 = new Effect("LED03");
        private Effect LED04 = new Effect("LED04");
        private Effect LED05 = new Effect("LED05");
        private Effect LED06 = new Effect("LED06");
        private Effect LED07 = new Effect("LED07");
        private Effect LED08 = new Effect("LED08");

        private LedBackdropFixture ledBack = new LedBackdropFixture("LEDBACKDROP", 17);

        private ColorStripFixture _CS1 = new ColorStripFixture("CS1", 35);
        private ColorStripFixture _CS2 = new ColorStripFixture("CS2", 39);
        private Effect CS01 = new Effect("CS01");
        private Effect CS02 = new Effect("CS02");
        private Effect CS03 = new Effect("CS03");
        private Effect CS04 = new Effect("CS04");
        private Effect CS05 = new Effect("CS05");
        private Effect CS06 = new Effect("CS06");

        private Effect RopeLightBlink = new Effect("Rope Light Blink");

        private VueFixture vueA = new VueFixture("Vue A", 44);
        private VueFixture vueB = new VueFixture("Vue B", 50);

        private List<Photon> AllLEDRed = new List<Photon>();
        private List<Photon> AllLEDGreen = new List<Photon>();
        private List<Photon> AllLEDBlue = new List<Photon>();

        private bool[] ledRightClick = new bool[4];
        private bool[] csRightClick = new bool[4];

        private bool toggleStrobe = false;
        private int strobeSpeed = 0;
        private int strobeIntensity = 0;
        private bool strobeSoloing = false;

        private bool shiftIsDown = false;

        private DateTime[] presses = new DateTime[8];
        private int stabilizing = 0;
        private bool doubleTime = false;

        #endregion

        public EffectsDesk()
        {
            InitializeComponent();

            DMXController.Suppress(29);
            DMXController.Suppress(30);

            #region PARS
            //****Add Effects to list
            #region Effect List
            _parEffects[1] = PAR01;
            _parEffects[2] = PAR02;
            _parEffects[3] = PAR03;
            _parEffects[4] = PAR04;
            _parEffects[5] = PAR05;
            _parEffects[6] = PAR06;
            _parEffects[7] = PAR07;
            _parEffects[8] = PAR08;
            _parEffects[9] = PAR09;
            _parEffects[10] = PAR10;
            _parEffects[11] = PAR11;
            _parEffects[12] = PAR12;
            #endregion

            //******Define Built-in effects*******
            #region PAR01
            PAR01.AddStep(1);
            PAR01.AddStep(2);
            PAR01.AddStep(3);
            PAR01.AddStep(4);
            PAR01.AddStep(5);
            PAR01.AddStep(6);
            PAR01.AddStep(7);
            PAR01.AddStep(8);
            PAR01.Attributes.Add(EffectAttribute.BOUNCE);
            #endregion
            #region PAR02
            PAR02.AddStep(new int[] { 1, 8 });
            PAR02.AddStep(new int[] { 2, 7 });
            PAR02.AddStep(new int[] { 3, 6 });
            PAR02.AddStep(new int[] { 4, 5 });
            PAR02.Attributes.Add(EffectAttribute.BOUNCE);
            #endregion
            #region PAR03
            PAR03.AddStep(new int[] { 1, 8 });
            PAR03.AddStep(new int[] { 2, 7 });
            PAR03.AddStep(new int[] { 3, 6 });
            PAR03.Attributes.Add(EffectAttribute.RANDOM);
            #endregion
            #region PAR04
            for (int i = 1; i <= 8; i++)
            {
                PAR04.AddStep(i);
            }
            PAR04.Attributes.Add(EffectAttribute.RANDOM);
            #endregion
            #region PAR05
            for (int i = 1; i <= 8; i++)
            {
                for (int j = 1; j <= 8; j++)
                {
                    if (i != j) PAR05.AddStep(new int[] { i, j });
                }
            }
            PAR05.Attributes.Add(EffectAttribute.RANDOM);
            #endregion
            #region PAR06
            for (int i = 1; i <= 8; i++)
            {
                for (int j = 1; j <= 8; j++)
                {
                    for (int k = 1; k <= 8; k++)
                    {
                        if (i != j && j != k && i != k) PAR06.AddStep(new int[] { i, j, k });
                    }
                }
            }
            PAR06.Attributes.Add(EffectAttribute.RANDOM);
            #endregion
            #region PAR07
            PAR07.AddStep(1);
            PAR07.AddStep(8);
            PAR07.AddStep(2);
            PAR07.AddStep(7);
            PAR07.AddStep(3);
            PAR07.AddStep(6);
            PAR07.AddStep(4);
            PAR07.AddStep(5);
            PAR07.Attributes.Add(EffectAttribute.BOUNCE);
            #endregion
            #region PAR08
            PAR08.AddStep(new int[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 0.4, 0, 0.2, 0, 0, 255);
            #endregion
            #region PAR09
            for (int i = 1; i <= 8; i++)
            {
                PAR09.AddStep(i, 0.2, 0, 0.2, 0.4, 0, 255);
            }
            #endregion
            #region PAR10
            PAR10.AddStep(new int[] { 1, 3, 5, 7 });
            PAR10.AddStep(new int[] { 2, 4, 6, 8 });
            #endregion
            #region PAR11
            PAR11.AddStep(new int[] { 1, 2, 3, 4 });
            PAR11.AddStep(new int[] { 5, 6, 7, 8 });
            #endregion
            #region PAR12
            for (int i = 1; i <= 4; i++)
            {
                for (int j = 5; j <= 8; j++)
                {
                    PAR12.AddStep(new int[] { i, j });
                }
            }
            PAR12.Attributes.Add(EffectAttribute.RANDOM);
            #endregion
            #endregion

            #region LEDS
            //*****Add Effects to list
            #region Effect List
            _ledEffects[1] = LED01;
            _ledEffects[2] = LED02;
            _ledEffects[3] = LED03;
            _ledEffects[4] = LED04;
            _ledEffects[5] = LED05;
            _ledEffects[6] = LED06;
            _ledEffects[7] = LED07;
            _ledEffects[8] = LED08;
            #endregion

            //****Define Effects
            #region LED01
            LED01.AddStep(new int[] { 17, 20, 23, 26 });
            LED01.AddStep(new int[] { 18, 21, 24, 27 });
            LED01.AddStep(new int[] { 19, 22, 25, 28 });
            #endregion
            #region LED02
            LED02.AddStep(new int[] { 17, 20, 23, 26 });
            LED02.AddStep(new int[] { 17, 20, 23, 26, 18, 21, 24, 27 });
            LED02.AddStep(new int[] { 18, 21, 24, 27 });
            LED02.AddStep(new int[] { 18, 21, 24, 27, 19, 22, 25, 28 });
            LED02.AddStep(new int[] { 19, 22, 25, 28 });
            LED02.AddStep(new int[] { 19, 22, 25, 28, 17, 20, 23, 26 });
            #endregion
            #region LED03
            LED03.AddStep(new int[] { 17, 23 });
            LED03.AddStep(new int[] { 21, 27 });
            LED03.AddStep(new int[] { 19, 25 });
            LED03.AddStep(new int[] { 20, 26 });
            LED03.AddStep(new int[] { 18, 24 });
            LED03.AddStep(new int[] { 22, 28 });
            #endregion
            #region LED04
            LED04.AddStep(new int[] { 17 });
            LED04.AddStep(new int[] { 17, 18, 20 });
            LED04.AddStep(new int[] { 18, 20, 21, 23 });
            LED04.AddStep(new int[] { 18, 19, 21, 23, 24, 26 });
            LED04.AddStep(new int[] { 19, 21, 22, 24, 26, 27 });
            LED04.AddStep(new int[] { 19, 17, 22, 24, 25, 27 });
            LED04.AddStep(new int[] { 22, 20, 25, 27, 28 });
            LED04.AddStep(new int[] { 25, 23, 28 });
            LED04.AddStep(new int[] { 28, 26 });
            LED04.AddStep();
            #endregion
            #region LED05
            for (int m = 17; m <= 19; m++)
                for (int n = 20; n <= 22; n++)
                    for (int o = 23; o <= 25; o++)
                        for (int p = 26; p <= 28; p++)
                        {
                            LED05.AddStep(new int[] { m, n, o, p }, 0.2, 0, 0.1, 0, 0, 255);
                        }
            LED05.Attributes.Add(EffectAttribute.RANDOM);
            #endregion
            #region LED06
            LED06.AddStep(new int[] { 17, 20, 23, 26 }, 0.2, 0, 0.1, 0, 0, 255);
            LED06.AddStep(new int[] { 18, 21, 24, 27 }, 0.2, 0, 0.1, 0, 0, 255);
            LED06.AddStep(new int[] { 19, 22, 25, 28 }, 0.2, 0, 0.1, 0, 0, 255);
            LED06.Attributes.Add(EffectAttribute.RANDOM);
            #endregion
            #region LED07
            for (int i = 17; i <= 28; i++)
            {
                LED07.AddStep(i, .2, 0, .2, .4, 0, 255);
            }
            LED07.Attributes.Add(EffectAttribute.RANDOM);
            #endregion
            #region LED08
            LED08.AddStep(new int[] { 17, 20, 23, 26 }, .2, 0, .1, 0, 0, 255);
            LED08.AddStep(new int[] { 17, 20, 23, 26 }, .6, 0, .1, 0, 0, 255);
            LED08.AddStep(new int[] { 17, 20, 23, 26, 18, 21, 24, 27 }, .2, 0, .1, 0, 0, 255);
            LED08.AddStep(new int[] { 17, 20, 23, 26, 18, 21, 24, 27 }, .6, 0, .1, 0, 0, 255);
            LED08.AddStep(new int[] { 18, 21, 24, 27 }, .2, 0, .1, 0, 0, 255);
            LED08.AddStep(new int[] { 18, 21, 24, 27 }, .6, 0, .1, 0, 0, 255);
            LED08.AddStep(new int[] { 18, 21, 24, 27, 19, 22, 25, 28 }, .2, 0, .1, 0, 0, 255);
            LED08.AddStep(new int[] { 18, 21, 24, 27, 19, 22, 25, 28 }, .6, 0, .1, 0, 0, 255);
            LED08.AddStep(new int[] { 19, 22, 25, 28 }, .2, 0, .1, 0, 0, 255);
            LED08.AddStep(new int[] { 19, 22, 25, 28 }, .6, 0, .1, 0, 0, 255);
            LED08.AddStep(new int[] { 19, 22, 25, 28, 17, 20, 23, 26 }, .2, 0, .1, 0, 0, 255);
            LED08.AddStep(new int[] { 19, 22, 25, 28, 17, 20, 23, 26 }, .6, 0, .1, 0, 0, 255);
            #endregion
            #endregion

            #region Colorstrips
            //*****Add Effects to list
            #region Effect List
            _CSEffects[1] = CS01;
            _CSEffects[2] = CS02;
            _CSEffects[3] = CS03;
            _CSEffects[4] = CS04;
            _CSEffects[5] = CS05;
            _CSEffects[6] = CS06;
            #endregion

            //****Define Effects
            #region CS01
            CS01.AddStep(new int[] { 36, 40 });
            CS01.AddStep(new int[] { 37, 41 });
            CS01.AddStep(new int[] { 38, 42 });
            #endregion
            #region CS02
            CS02.AddStep(new int[] { 36, 40 });
            CS02.AddStep(new int[] { 36, 37, 40, 41 });
            CS02.AddStep(new int[] { 37, 41 });
            CS02.AddStep(new int[] { 37, 38, 41, 42 });
            CS02.AddStep(new int[] { 38, 42 });
            CS02.AddStep(new int[] { 38, 36, 42, 40 });
            #endregion
            #region CS03
            CS03.AddStep(36);
            CS03.AddStep(37);
            CS03.AddStep(38);
            CS03.AddStep(40);
            CS03.AddStep(41);
            CS03.AddStep(42);
            CS03.Attributes.Add(EffectAttribute.RANDOM);
            #endregion
            #region CS04
            CS04.AddStep(new int[] { 36, 40 }, 0.4, 0.2, 0.2, 0.2, 0, 255);
            CS04.AddStep(new int[] { 36, 37, 40, 41 }, 0.4, 0.2, 0.2, 0.2, 0, 255);
            CS04.AddStep(new int[] { 37, 41 }, 0.4, 0.2, 0.2, 0.2, 0, 255);
            CS04.AddStep(new int[] { 37, 38, 41, 42 }, 0.4, 0.2, 0.2, 0.2, 0, 255);
            CS04.AddStep(new int[] { 38, 42 }, 0.4, 0.2, 0.2, 0.2, 0, 255);
            CS04.AddStep(new int[] { 38, 36, 42, 40 }, 0.4, 0.2, 0.2, 0.2, 0, 255);
            #endregion
            #region CS05
            for (int m = 36; m <= 38; m++)
                for (int n = 40; n <= 42; n++)
                {
                    CS05.AddStep(new int[] { m, n }, 0.2, 0, 0.1, 0, 0, 255);
                }
            CS05.Attributes.Add(EffectAttribute.RANDOM);
            #endregion
            #region CS06
            CS06.AddStep(new int[] { 36, 40 }, 0.2, 0, 0.1, 0, 0, 255);
            CS06.AddStep(new int[] { 37, 41 }, 0.2, 0, 0.1, 0, 0, 255);
            CS06.AddStep(new int[] { 38, 42 }, 0.2, 0, 0.1, 0, 0, 255);
            CS06.Attributes.Add(EffectAttribute.RANDOM);
            #endregion
            #endregion

            #region Other Effects
            RopeLightBlink.AddStep(15, .4, 0, .2, 0, 0, 255);

            AllLEDRed.Add(new Photon(17, 255));
            AllLEDRed.Add(new Photon(20, 255));
            AllLEDRed.Add(new Photon(23, 255));
            AllLEDRed.Add(new Photon(26, 255));
            AllLEDRed.Add(new Photon(35, 217));
            AllLEDRed.Add(new Photon(36, 255));
            AllLEDRed.Add(new Photon(39, 217));
            AllLEDRed.Add(new Photon(40, 255));

            AllLEDGreen.Add(new Photon(18, 255));
            AllLEDGreen.Add(new Photon(21, 255));
            AllLEDGreen.Add(new Photon(24, 255));
            AllLEDGreen.Add(new Photon(27, 255));
            AllLEDGreen.Add(new Photon(35, 217));
            AllLEDGreen.Add(new Photon(37, 255));
            AllLEDGreen.Add(new Photon(39, 217));
            AllLEDGreen.Add(new Photon(41, 255));

            AllLEDBlue.Add(new Photon(19, 255));
            AllLEDBlue.Add(new Photon(22, 255));
            AllLEDBlue.Add(new Photon(25, 255));
            AllLEDBlue.Add(new Photon(28, 255));
            AllLEDBlue.Add(new Photon(35, 217));
            AllLEDBlue.Add(new Photon(38, 255));
            AllLEDBlue.Add(new Photon(39, 217));
            AllLEDBlue.Add(new Photon(42, 255));

            #endregion

        }

        #region Tap Sync

        private void TapSync()
        {
            //If we've tapped 8 times, reset count
            if (stabilizing == 8)
            {
                stabilizing = 0;
            }

            //If it's been 3 seconds, reset count and turn text to red
            if (DateTime.Now.Subtract(presses[presses.Length - 1]) > new TimeSpan(0, 0, 3))
            {
                stabilizing = 0;
                tapsyncpanel.Background = Brushes.DarkRed;
            }

            stabilizing++;
            
            //Slide values down one
            for (int i = 1; i < presses.Length; i++)
            {
                presses[i - 1] = presses[i];
            }

            presses[presses.Length - 1] = System.DateTime.Now;

            //Calculate total time elapsed between oldest and most recent presses
            double totalSeconds = 0;

            for (int i = 1; i < presses.Length; i++)
            {
                System.TimeSpan span = presses[i].Subtract(presses[i - 1]);
                totalSeconds += span.TotalSeconds;
            }

            double secondsBetweenBeats = totalSeconds / (presses.Length - 1);
            double beatsPerSecond = 1 / secondsBetweenBeats;
            double rate = ((1 / secondsBetweenBeats) * 20);

            lbl_BPS.Content = "BPS: " + Math.Round(beatsPerSecond, 2);
            lbl_rate.Content = "Rate: " + String.Format("{0:0.00}", rate);


            //If we've stabilized, update the rate and set the text green
            if (stabilizing == 8)
            {
                SetRate(rate);
                tapsyncpanel.Background = Brushes.Green;
            }
        }

        private void SetRate(double rate)
        {
            if (doubleTime) rate = rate * 2.0;
            if (rad_rate.IsChecked == true) Controller.Rate = rate;
            else Controller.BackgroundRate = rate;
            lbl_primary.Content = String.Format("{0:0.00}", Controller.Rate);
            lbl_BG.Content = String.Format("{0:0.00}", Controller.BackgroundRate);
        }

        private void btn_tap_Click(object sender, RoutedEventArgs e)
        {
            TapSync();
        }

        private void btn_x2_Click(object sender, RoutedEventArgs e)
        {
            if (shiftIsDown)
            {
                Controller.BackgroundRate = Controller.Rate * 0.5;
                lbl_BG.Content = String.Format("{0:0.00}", Controller.BackgroundRate);
            }
            else
            {
                Controller.BackgroundRate = Controller.Rate * 2.0;
                lbl_BG.Content = String.Format("{0:0.00}", Controller.BackgroundRate);
            }
        }

        private void btn_switch_Click(object sender, RoutedEventArgs e)
        {
            Controller.SwitchRates();
            lbl_primary.Content = String.Format("{0:0.00}", Controller.Rate);
            lbl_BG.Content = String.Format("{0:0.00}", Controller.BackgroundRate);
        }
        #endregion

        private void EffectDesk_KeyDown(object sender, KeyEventArgs e)
        {
            MouseButtonEventArgs f = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left);

            if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                shiftIsDown = true;
                par_solo_suppress.Label = "Suppress";
                led_solo_suppress.Label = "Suppress";
                cs_solo_suppress.Label = "Suppress";
                btn_x2.Content = "Make BG 1/2";
            }
            switch (e.Key)
            {
                //*****PARS
                case Key.D1:
                    DMXController.SetLevel("keypress", 1, 255);
                    break;
                case Key.D2:
                    DMXController.SetLevel("keypress", 2, 255);
                    break;
                case Key.D3:
                    DMXController.SetLevel("keypress", 3, 255);
                    break;
                case Key.D4:
                    DMXController.SetLevel("keypress", 4, 255);
                    break;
                case Key.D5:
                    DMXController.SetLevel("keypress", 5, 255);
                    break;
                case Key.D6:
                    DMXController.SetLevel("keypress", 6, 255);
                    break;
                case Key.D7:
                    DMXController.SetLevel("keypress", 7, 255);
                    break;
                case Key.D8:
                    DMXController.SetLevel("keypress", 8, 255);
                    break;
                case Key.D9:
                    for (int i = 1; i <= 4; i++) DMXController.SetLevel("keypress", i, 255);
                    break;
                case Key.D0:
                    for (int i = 5; i <= 8; i++) DMXController.SetLevel("keypress", i, 255);
                    break;

                //*****LEDS
                case Key.Q:
                    DMXController.SetLevel("Effect DeskR", 17, 255);
                    DMXController.SetLevel("Effect DeskR", 20, 255);
                    led_r_bump.LitLeft = true;
                    break;
                case Key.W:
                    DMXController.SetLevel("Effect DeskG", 18, 255);
                    DMXController.SetLevel("Effect DeskG", 21, 255);
                    led_g_bump.LitLeft = true;
                    break;
                case Key.E:
                    DMXController.SetLevel("Effect DeskB", 19, 255);
                    DMXController.SetLevel("Effect DeskB", 22, 255);
                    led_b_bump.LitLeft = true;
                    break;
                case Key.R:
                    DMXController.SetLevel("Effect DeskR", 23, 255);
                    DMXController.SetLevel("Effect DeskR", 26, 255);
                    led_r_bump.LitRight = true;
                    break;
                case Key.T:
                    DMXController.SetLevel("Effect DeskG", 24, 255);
                    DMXController.SetLevel("Effect DeskG", 27, 255);
                    led_g_bump.LitRight = true;
                    break;
                case Key.Y:
                    DMXController.SetLevel("Effect DeskB", 25, 255);
                    DMXController.SetLevel("Effect DeskB", 28, 255);
                    led_b_bump.LitRight = true;
                    break;
                case Key.U:
                    led_r_bump_MouseLeftButtonDown(sender, f);
                    break;
                case Key.I:
                    led_g_bump_MouseLeftButtonDown(sender, f);
                    break;
                case Key.O:
                    led_b_bump_MouseLeftButtonDown(sender, f);
                    break;
                case Key.P:
                    led_w_bump_MouseLeftButtonDown(sender, f);
                    break;

                //*****COLORSTRIPS
                case Key.A:
                    _CS1.Red();
                    cs_r_bump.LitLeft = true;
                    break;
                case Key.S:
                    _CS1.Green();
                    cs_g_bump.LitLeft = true;
                    break;
                case Key.D:
                    _CS1.Blue();
                    cs_b_bump.LitLeft = true;
                    break;
                case Key.F:
                    _CS2.Red();
                    cs_r_bump.LitRight = true;
                    break;
                case Key.G:
                    _CS2.Green();
                    cs_g_bump.LitRight = true;
                    break;
                case Key.H:
                    _CS2.Blue();
                    cs_b_bump.LitRight = true;
                    break;
                case Key.J:
                    cs_r_bump_MouseLeftButtonDown(sender, f);
                    break;
                case Key.K:
                    cs_g_bump_MouseLeftButtonDown(sender, f);
                    break;
                case Key.L:
                    cs_b_bump_MouseLeftButtonDown(sender, f);
                    break;
                case Key.OemSemicolon:
                    cs_b_bump_MouseLeftButtonDown(sender, f);
                    break;
                case Key.OemQuotes:
                    cs_enable_Click(sender, new RoutedEventArgs());
                    break;

                //*****OTHER
                case Key.N:
                    DMXController.SetLevel("LMINE_bump", 13, 255);
                    break;
                case Key.M:
                    DMXController.SetLevel("RMINE_bump", 14, 255);
                    break;
                case Key.Z:
                    DMXController.SetLevels("AllLEDRed", AllLEDRed, 255);
                    break;
                case Key.X:
                    DMXController.SetLevels("AllLEDGreen", AllLEDGreen, 255);
                    break;
                case Key.C:
                    DMXController.SetLevels("AllLEDBlue", AllLEDBlue, 255);
                    break;
                case Key.V:
                    DMXController.SetLevels("AllLEDWhite", AllLEDRed, 255);
                    DMXController.SetLevels("AllLEDWhite", AllLEDGreen, 255);
                    DMXController.SetLevels("AllLEDWhite", AllLEDBlue, 255);
                    break;
                case Key.B:
                    DMXController.SetLevel("Rope Bump", 15, 255);
                    break;

                case Key.OemQuestion:
                    TapSync();
                    break;
                default:
                    break;
            }
        }

        private void EffectDesk_KeyUp(object sender, KeyEventArgs e)
        {
            MouseButtonEventArgs f = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left);

            if ((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift)
            {
                shiftIsDown = false;
                par_solo_suppress.Label = "Solo";
                led_solo_suppress.Label = "Solo";
                cs_solo_suppress.Label = "Solo";
                btn_x2.Content = "Make BG x2";
            }
            switch (e.Key)
            {
                //*****PARS
                case Key.D1:
                    DMXController.SetLevel("keypress", 1, 0);
                    break;
                case Key.D2:
                    DMXController.SetLevel("keypress", 2, 0);
                    break;
                case Key.D3:
                    DMXController.SetLevel("keypress", 3, 0);
                    break;
                case Key.D4:
                    DMXController.SetLevel("keypress", 4, 0);
                    break;
                case Key.D5:
                    DMXController.SetLevel("keypress", 5, 0);
                    break;
                case Key.D6:
                    DMXController.SetLevel("keypress", 6, 0);
                    break;
                case Key.D7:
                    DMXController.SetLevel("keypress", 7, 0);
                    break;
                case Key.D8:
                    DMXController.SetLevel("keypress", 8, 0);
                    break;
                case Key.D9:
                    for (int i = 1; i <= 4; i++) DMXController.SetLevel("keypress", i, 0);
                    break;
                case Key.D0:
                    for (int i = 5; i <= 8; i++) DMXController.SetLevel("keypress", i, 0);
                    break;

                //*****LEDS
                case Key.Q:
                    DMXController.SetLevel("Effect DeskR", 17, 0);
                    DMXController.SetLevel("Effect DeskR", 20, 0);
                    if(!ledRightClick[0]) led_r_bump.LitLeft = false;
                    break;
                case Key.W:
                    DMXController.SetLevel("Effect DeskG", 18, 0);
                    DMXController.SetLevel("Effect DeskG", 21, 0);
                    if (!ledRightClick[1]) led_g_bump.LitLeft = false;
                    break;
                case Key.E:
                    DMXController.SetLevel("Effect DeskB", 19, 0);
                    DMXController.SetLevel("Effect DeskB", 22, 0);
                    if (!ledRightClick[2]) led_b_bump.LitLeft = false;
                    break;
                case Key.R:
                    DMXController.SetLevel("Effect DeskR", 23, 0);
                    DMXController.SetLevel("Effect DeskR", 26, 0);
                    if (!ledRightClick[0]) led_r_bump.LitRight = false;
                    break;
                case Key.T:
                    DMXController.SetLevel("Effect DeskG", 24, 0);
                    DMXController.SetLevel("Effect DeskG", 27, 0);
                    if (!ledRightClick[1]) led_g_bump.LitRight = false;
                    break;
                case Key.Y:
                    DMXController.SetLevel("Effect DeskB", 25, 0);
                    DMXController.SetLevel("Effect DeskB", 28, 0);
                    if (!ledRightClick[2]) led_b_bump.LitRight = false;
                    break;
                case Key.U:
                    led_r_bump_MouseLeftButtonUp(sender, f);
                    break;
                case Key.I:
                    led_g_bump_MouseLeftButtonUp(sender, f);
                    break;
                case Key.O:
                    led_b_bump_MouseLeftButtonUp(sender, f);
                    break;
                case Key.P:
                    led_w_bump_MouseLeftButtonUp(sender, f);
                    break;

                //*****COLORSTRIPS
                case Key.A:
                    if (!csRightClick[0])
                    {
                        _CS1.Red(0);
                        cs_r_bump.LitLeft = false;
                    }
                    break;
                case Key.S:
                    if (!csRightClick[1])
                    {
                        _CS1.Green(0);
                        cs_g_bump.LitLeft = false;
                    }
                    break;
                case Key.D:
                    if (!csRightClick[2])
                    {
                        _CS1.Blue(0);
                        cs_b_bump.LitLeft = false;
                    }
                    break;
                case Key.F:
                    if (!csRightClick[0])
                    {
                        _CS2.Red(0);
                        cs_r_bump.LitRight = false;
                    }
                    break;
                case Key.G:
                    if (!csRightClick[1])
                    {
                        _CS2.Green(0);
                        cs_g_bump.LitRight = false;
                    }
                    break;
                case Key.H:
                    if (!csRightClick[2])
                    {
                        _CS2.Blue(0);
                        cs_b_bump.LitRight = false;
                    }
                    break;
                case Key.J:
                    cs_r_bump_MouseLeftButtonUp(sender, f);
                    break;
                case Key.K:
                    cs_g_bump_MouseLeftButtonUp(sender, f);
                    break;
                case Key.L:
                    cs_b_bump_MouseLeftButtonUp(sender, f);
                    break;
                case Key.OemSemicolon:
                    cs_b_bump_MouseLeftButtonUp(sender, f);
                    break;

                //*****OTHER
                case Key.N:
                    DMXController.SetLevel("LMINE_bump", 13, 0);
                    break;
                case Key.M:
                    DMXController.SetLevel("RMINE_bump", 14, 0);
                    break;
                case Key.Z:
                    DMXController.SetLevels("AllLEDRed", AllLEDRed, 0);
                    break;
                case Key.X:
                    DMXController.SetLevels("AllLEDGreen", AllLEDGreen, 0);
                    break;
                case Key.C:
                    DMXController.SetLevels("AllLEDBlue", AllLEDBlue, 0);
                    break;
                case Key.V:
                    DMXController.SetLevels("AllLEDWhite", AllLEDRed, 0);
                    DMXController.SetLevels("AllLEDWhite", AllLEDGreen, 0);
                    DMXController.SetLevels("AllLEDWhite", AllLEDBlue, 0);
                    break;
                case Key.B:
                    DMXController.SetLevel("Rope Bump", 15, 0);
                    break;
                default:
                    break;
            }
        }

        private void EffectDesk_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (Effect fx in _parEffects) if (fx != null) fx.Stop();
            foreach (Effect fx in _ledEffects) if (fx != null) fx.Stop();
            foreach (Effect fx in _CSEffects) if (fx != null) fx.Stop();
            if (RopeLightBlink != null) RopeLightBlink.Stop();
            _CS1.AllOff();
            _CS2.AllOff();
            DMXController.RemoveSetterFromAll("StrobeModule");
            DMXController.RemoveSetterFromAll("DISCO");
            DMXController.RemoveSetterFromAll("MINES");
            DMXController.RemoveSetterFromAll("REVO");
            DMXController.RemoveSetterFromAll("Effect DeskR");
            DMXController.RemoveSetterFromAll("Effect DeskG");
            DMXController.RemoveSetterFromAll("Effect DeskB");
            DMXController.RemoveSetterFromAll("Effect DeskW");
            vueA.SetMode(VueFixture.MANUAL_MODE);
            vueB.SetMode(VueFixture.MANUAL_MODE);
            vueA.Off();
            vueB.Off();
        }        

        #region PAR Event Handlers
        private void slider_parlevel_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            foreach (Effect fx in _parEffects)
            {
                if (fx != null) fx.Level = Convert.ToInt32(slider_parlevel.Value);
            }
            if (lbl_parlevel != null) lbl_parlevel.Content = Convert.ToInt32(slider_parlevel.Value);
        }

        private void btn_par_effect_Click(object sender, RoutedEventArgs e)
        {
            string buttonName = ((Button)sender).Name;
            int buttonNumber = Int32.Parse(buttonName.Substring((buttonName.Length - 2), 2));

            if (!((SingleBumpButton)sender).Lit)
            {
                ((SingleBumpButton)sender).Lit = true;
                _parEffects[buttonNumber].Start();
            }
            else
            {
                ((SingleBumpButton)sender).Lit = false;
                _parEffects[buttonNumber].Stop();
            }
        }

        private void par_solo_suppress_Click(object sender, RoutedEventArgs e)
        {
            List<int> pars = new List<int>();
            for (int i = 1; i <= 8; i++) pars.Add(i);
            if (shiftIsDown)
            {
                if (par_solo_suppress.LitRight)
                {
                    DMXController.Unsuppress(pars);
                    par_solo_suppress.LitRight = false;
                }
                else
                {
                    DMXController.Suppress(pars);
                    par_solo_suppress.LitRight = true;
                }
            }
            else
            {
                if (par_solo_suppress.LitLeft)
                {
                    DMXController.Unsolo();
                    par_solo_suppress.LitLeft = false;
                    par_solo_suppress.LitRight = false;

                    led_solo_suppress.LitRight = false;
                    cs_solo_suppress.LitRight = false;
                }
                else
                {
                    DMXController.Unsuppress(pars);
                    DMXController.Solo(pars);
                    par_solo_suppress.LitLeft = true;
                    par_solo_suppress.LitRight = false;

                    led_solo_suppress.LitRight = true;
                    led_solo_suppress.LitLeft = false;
                    cs_solo_suppress.LitRight = true;
                    cs_solo_suppress.LitLeft = false;
                }
            }
        }
        #endregion

        #region LED Event Handlers
        private void slider_ledlevel_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            foreach (Effect fx in _ledEffects)
            {
                if (fx != null) fx.Level = Convert.ToInt32(slider_ledlevel.Value);
            }
            if (lbl_ledlevel != null) lbl_ledlevel.Content = Convert.ToInt32(slider_ledlevel.Value);
        }

        private void btn_led_effect_Click(object sender, RoutedEventArgs e)
        {
            string buttonName = ((Button)sender).Name;
            int buttonNumber = Int32.Parse(buttonName.Substring((buttonName.Length - 2), 2));

            if (!((SingleBumpButton)sender).Lit)
            {
                ((SingleBumpButton)sender).Lit = true;
                _ledEffects[buttonNumber].Start();
            }
            else
            {
                ((SingleBumpButton)sender).Lit = false;
                _ledEffects[buttonNumber].Stop();
            }
        }

        #region Color Bumps
        private void led_r_bump_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!ledRightClick[0])
            {
                DMXController.SetLevel("Effect DeskR", 17, 255);
                DMXController.SetLevel("Effect DeskR", 20, 255);
                DMXController.SetLevel("Effect DeskR", 23, 255);
                DMXController.SetLevel("Effect DeskR", 26, 255);
                led_r_bump.LitLeft = true;
                led_r_bump.LitRight = true;
            }
        }

        private void led_r_bump_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!ledRightClick[0])
            {
                DMXController.SetLevel("Effect DeskR", 17, 0);
                DMXController.SetLevel("Effect DeskR", 20, 0);
                DMXController.SetLevel("Effect DeskR", 23, 0);
                DMXController.SetLevel("Effect DeskR", 26, 0);
                led_r_bump.LitLeft = false;
                led_r_bump.LitRight = false;
            }
        }

        private void led_r_bump_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            led_r_bump_MouseLeftButtonDown(sender, e);
            ledRightClick[0] = !ledRightClick[0];
            led_r_bump_MouseLeftButtonUp(sender, e);
        }
   
        private void led_g_bump_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!ledRightClick[1])
            {
                DMXController.SetLevel("Effect DeskG", 18, 255);
                DMXController.SetLevel("Effect DeskG", 21, 255);
                DMXController.SetLevel("Effect DeskG", 24, 255);
                DMXController.SetLevel("Effect DeskG", 27, 255);
                led_g_bump.LitLeft = true;
                led_g_bump.LitRight = true;
            }
        }

        private void led_g_bump_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!ledRightClick[1])
            {
                DMXController.SetLevel("Effect DeskG", 18, 0);
                DMXController.SetLevel("Effect DeskG", 21, 0);
                DMXController.SetLevel("Effect DeskG", 24, 0);
                DMXController.SetLevel("Effect DeskG", 27, 0);
                led_g_bump.LitLeft = false;
                led_g_bump.LitRight = false;
            }
        }

        private void led_g_bump_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            led_g_bump_MouseLeftButtonDown(sender, e);
            ledRightClick[1] = !ledRightClick[1];
            led_g_bump_MouseLeftButtonUp(sender, e);
        }

        private void led_b_bump_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!ledRightClick[2])
            {
                DMXController.SetLevel("Effect DeskB", 19, 255);
                DMXController.SetLevel("Effect DeskB", 22, 255);
                DMXController.SetLevel("Effect DeskB", 25, 255);
                DMXController.SetLevel("Effect DeskB", 28, 255);
                led_b_bump.LitLeft = true;
                led_b_bump.LitRight = true;
            }
        }

        private void led_b_bump_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!ledRightClick[2])
            {
                DMXController.SetLevel("Effect DeskB", 19, 0);
                DMXController.SetLevel("Effect DeskB", 22, 0);
                DMXController.SetLevel("Effect DeskB", 25, 0);
                DMXController.SetLevel("Effect DeskB", 28, 0);
                led_b_bump.LitLeft = false;
                led_b_bump.LitRight = false;
            }
        }

        private void led_b_bump_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            led_b_bump_MouseLeftButtonDown(sender, e);
            ledRightClick[2] = !ledRightClick[2];
            led_b_bump_MouseLeftButtonUp(sender, e);
        }

        private void led_w_bump_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!ledRightClick[3])
            {
                DMXController.SetLevel("Effect DeskW", 17, 255);
                DMXController.SetLevel("Effect DeskW", 20, 255);
                DMXController.SetLevel("Effect DeskW", 23, 255);
                DMXController.SetLevel("Effect DeskW", 26, 255);
                DMXController.SetLevel("Effect DeskW", 18, 255);
                DMXController.SetLevel("Effect DeskW", 21, 255);
                DMXController.SetLevel("Effect DeskW", 24, 255);
                DMXController.SetLevel("Effect DeskW", 27, 255);
                DMXController.SetLevel("Effect DeskW", 19, 255);
                DMXController.SetLevel("Effect DeskW", 22, 255);
                DMXController.SetLevel("Effect DeskW", 25, 255);
                DMXController.SetLevel("Effect DeskW", 28, 255);
                led_w_bump.LitLeft = true;
                led_w_bump.LitRight = true;
            }
        }

        private void led_w_bump_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!ledRightClick[3])
            {
                DMXController.SetLevel("Effect DeskW", 17, 0);
                DMXController.SetLevel("Effect DeskW", 20, 0);
                DMXController.SetLevel("Effect DeskW", 23, 0);
                DMXController.SetLevel("Effect DeskW", 26, 0);
                DMXController.SetLevel("Effect DeskW", 18, 0);
                DMXController.SetLevel("Effect DeskW", 21, 0);
                DMXController.SetLevel("Effect DeskW", 24, 0);
                DMXController.SetLevel("Effect DeskW", 27, 0);
                DMXController.SetLevel("Effect DeskW", 19, 0);
                DMXController.SetLevel("Effect DeskW", 22, 0);
                DMXController.SetLevel("Effect DeskW", 25, 0);
                DMXController.SetLevel("Effect DeskW", 28, 0);
                led_w_bump.LitLeft = false;
                led_w_bump.LitRight = false;
            }
        }

        private void led_w_bump_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            led_w_bump_MouseLeftButtonDown(sender, e);
            ledRightClick[3] = !ledRightClick[3];
            led_w_bump_MouseLeftButtonUp(sender, e);
        }
        #endregion

        private void led_solo_suppress_Click(object sender, RoutedEventArgs e)
        {               
            List<int> leds = new List<int>();
            for (int i = 17; i <= 28; i++) leds.Add(i);
            if (shiftIsDown)
            {
                if (led_solo_suppress.LitRight)
                {
                    DMXController.Unsuppress(leds);
                    led_solo_suppress.LitRight = false;
                }
                else
                {
                    DMXController.Suppress(leds);
                    led_solo_suppress.LitRight = true;
                }
            }
            else
            {
                if (led_solo_suppress.LitLeft)
                {
                    DMXController.Unsolo();
                    led_solo_suppress.LitLeft = false;
                    led_solo_suppress.LitRight = false;

                    par_solo_suppress.LitRight = false;
                    cs_solo_suppress.LitRight = false;
                }
                else
                {
                    DMXController.Unsuppress(leds);
                    DMXController.Solo(leds);
                    led_solo_suppress.LitLeft = true;
                    led_solo_suppress.LitRight = false;

                    par_solo_suppress.LitRight = true;
                    par_solo_suppress.LitLeft = false;
                    cs_solo_suppress.LitRight = true;
                    cs_solo_suppress.LitLeft = false;
                }
            }
        }
        #endregion

        #region Colorstrip Event Handlers
        private void btn_cs_effect_Click(object sender, RoutedEventArgs e)
        {
            string buttonName = ((Button)sender).Name;
            int buttonNumber = Int32.Parse(buttonName.Substring((buttonName.Length - 2), 2));

            if (!((SingleBumpButton)sender).Lit)
            {
                ((SingleBumpButton)sender).Lit = true;
                _CSEffects[buttonNumber].Start();
            }
            else
            {
                ((SingleBumpButton)sender).Lit = false;
                _CSEffects[buttonNumber].Stop();
            }
        } 

        private void cs_solo_suppress_Click(object sender, RoutedEventArgs e)
        {
            List<int> colorStrips = new List<int>() { 36, 37, 38, 40, 41, 42 };
            if (shiftIsDown)
            {
                if (cs_solo_suppress.LitRight)
                {
                    DMXController.Unsuppress(colorStrips);
                    cs_solo_suppress.LitRight = false;
                }
                else
                {
                    DMXController.Suppress(colorStrips);
                    cs_solo_suppress.LitRight = true;
                }
            }
            else
            {
                if (cs_solo_suppress.LitLeft)
                {
                    DMXController.Unsolo();
                    cs_solo_suppress.LitLeft = false;
                    cs_solo_suppress.LitRight = false;

                    par_solo_suppress.LitRight = false;
                    led_solo_suppress.LitRight = false;
                }
                else
                {
                    DMXController.Unsuppress(colorStrips);
                    DMXController.Solo(colorStrips);
                    cs_solo_suppress.LitLeft = true;
                    cs_solo_suppress.LitRight = false;

                    par_solo_suppress.LitRight = true;
                    par_solo_suppress.LitLeft = false;
                    led_solo_suppress.LitRight = true;
                    led_solo_suppress.LitLeft = false;
                }
            }
        }

        private void slider_cslevel_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            foreach (Effect fx in _CSEffects)
            {
                if (fx != null) fx.Level = Convert.ToInt32(slider_cslevel.Value);
            }
            if (lbl_cslevel != null) lbl_cslevel.Content = Convert.ToInt32(slider_cslevel.Value);
        }

        #region Color Bumps
        private void cs_r_bump_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!csRightClick[0])
            {
                _CS1.Red();
                _CS2.Red();
                cs_r_bump.LitLeft = true;
                cs_r_bump.LitRight = true;
            }
        }

        private void cs_r_bump_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!csRightClick[0])
            {
                _CS1.Red(0);
                _CS2.Red(0);
                cs_r_bump.LitLeft = false;
                cs_r_bump.LitRight = false;
            }
        }

        private void cs_r_bump_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            cs_r_bump_MouseLeftButtonDown(sender, e);
            csRightClick[0] = !csRightClick[0];
            cs_r_bump_MouseLeftButtonUp(sender, e);
        }

        private void cs_g_bump_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!csRightClick[1])
            {
                _CS1.Green();
                _CS2.Green();
                cs_g_bump.LitLeft = true;
                cs_g_bump.LitRight = true;
            }
        }

        private void cs_g_bump_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!csRightClick[1])
            {
                _CS1.Green(0);
                _CS2.Green(0);
                cs_g_bump.LitLeft = false;
                cs_g_bump.LitRight = false;
            }
        }

        private void cs_g_bump_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            cs_g_bump_MouseLeftButtonDown(sender, e);
            csRightClick[1] = !csRightClick[1];
            cs_g_bump_MouseLeftButtonUp(sender, e);
        }

        private void cs_b_bump_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!csRightClick[2])
            {
                _CS1.Blue();
                _CS2.Blue();
                cs_b_bump.LitLeft = true;
                cs_b_bump.LitRight = true;
            }
        }

        private void cs_b_bump_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!csRightClick[2])
            {
                _CS1.Blue(0);
                _CS2.Blue(0);
                cs_b_bump.LitLeft = false;
                cs_b_bump.LitRight = false;
            }
        }

        private void cs_b_bump_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            cs_b_bump_MouseLeftButtonDown(sender, e);
            csRightClick[2] = !csRightClick[2];
            cs_b_bump_MouseLeftButtonUp(sender, e);
        }

        private void cs_w_bump_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!csRightClick[3])
            {
                _CS1.White();
                _CS2.White();
                cs_w_bump.LitLeft = true;
                cs_w_bump.LitRight = true;
            }
        }

        private void cs_w_bump_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!csRightClick[3])
            {
                _CS1.White(0);
                _CS2.White(0);
                cs_w_bump.LitLeft = false;
                cs_w_bump.LitRight = false;
            }
        }

        private void cs_w_bump_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            cs_w_bump_MouseLeftButtonDown(sender, e);
            csRightClick[3] = !csRightClick[3];
            cs_w_bump_MouseLeftButtonUp(sender, e);
        }
        #endregion

        private void cs_enable_Click(object sender, RoutedEventArgs e)
        {
            if (cs_enable.Lit)
            {
                _CS1.StopColorStrip();
                _CS2.StopColorStrip();
                cs_enable.Lit = false;
            }
            else
            {
                _CS1.StartColorStrip();
                _CS2.StartColorStrip();
                cs_enable.Lit = true;
            }
        }
        #endregion

        #region Strobe Event Handlers
        private void slider_strobespeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (toggleStrobe) DMXController.SetLevel("StrobeModule", 29, Convert.ToInt32(slider_strobespeed.Value));
            if(lbl_strobespeed != null) lbl_strobespeed.Content = Convert.ToInt32(slider_strobespeed.Value).ToString();
            strobeSpeed = Convert.ToInt32(slider_strobespeed.Value);
        }

        private void slider_strobeintensity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (toggleStrobe) DMXController.SetLevel("StrobeModule", 30, Convert.ToInt32(slider_strobeintensity.Value));
            if(lbl_strobeintensity != null) lbl_strobeintensity.Content = Convert.ToInt32(slider_strobeintensity.Value).ToString();
            strobeIntensity = Convert.ToInt32(slider_strobeintensity.Value);
        }

        private void btn_strobe_enable_Click(object sender, RoutedEventArgs e)
        {
            List<int> strobe = new List<int>() {29, 30};
            if (!toggleStrobe)
            {
                btn_strobe_enable.Content = "ON!";
                btn_strobe_enable.FontWeight = FontWeights.Bold;
                DMXController.SetLevel("StrobeModule", 29, strobeSpeed);
                DMXController.SetLevel("StrobeModule", 30, strobeIntensity);
                toggleStrobe = true;
                if (!shiftIsDown)
                {
                    DMXController.Solo(strobe);
                    strobeSoloing = true;
                }
            }
            else
            {
                btn_strobe_enable.Content = "OFF";
                btn_strobe_enable.FontWeight = FontWeights.Regular;
                DMXController.SetLevel("StrobeModule", 29, 0);
                DMXController.SetLevel("StrobeModule", 30, 0);
                toggleStrobe = false;
                if (!shiftIsDown && strobeSoloing)
                {
                    DMXController.Unsolo();
                    DMXController.Suppress(strobe);
                }
            }
        }
        #endregion

        #region Other Effect Handlers

        private void btn_general_cancel_Click(object sender, RoutedEventArgs e)
        {
            ledRightClick = new bool[4];
            csRightClick = new bool[4];

            EffectDesk_Closing(sender, new System.ComponentModel.CancelEventArgs());
            foreach (SingleBumpButton sing in pars.Children.OfType<SingleBumpButton>())
            {
                sing.Lit = false;
            }
            foreach (SingleBumpButton sing in leds.Children.OfType<SingleBumpButton>())
            {
                sing.Lit = false;
            }
            foreach (DoubleBumpButton doub in leds.Children.OfType<DoubleBumpButton>())
            {
                doub.LitLeft = false;
                doub.LitRight = false;
            }
            foreach (SingleBumpButton sing in colorstrips.Children.OfType<SingleBumpButton>())
            {
                sing.Lit = false;
            }
            foreach (DoubleBumpButton doub in colorstrips.Children.OfType<DoubleBumpButton>())
            {
                doub.LitLeft = false;
                doub.LitRight = false;
            }
            foreach (SingleBumpButton sing in other.Children.OfType<SingleBumpButton>())
            {
                sing.Lit = false;
            }
            btn_strobe_enable.Content = "OFF";
            btn_strobe_enable.FontWeight = FontWeights.Regular;

            DMXController.FlushHTPAll();
        }

        private void btn_global_pause_Click(object sender, RoutedEventArgs e)
        {
            if (!DMXController.Paused)
            {
                DMXController.Paused = true;
                btn_global_pause.Content = "PAUSED!";
            }
            else
            {
                DMXController.Paused = false;
                btn_global_pause.Content = "Global Pause";
            }
        }

        private void btn_blackout_Click(object sender, RoutedEventArgs e)
        {
            if (!DMXController.Blackout)
            {
                DMXController.Blackout = true;
                btn_blackout.Content = "BLACKOUT!";
                btn_blackout.FontWeight = FontWeights.ExtraBold;
            }
            else
            {
                DMXController.Blackout = false;
                btn_blackout.Content = "Blackout";
                btn_blackout.FontWeight = FontWeights.Regular;
            }
        }

        private void btn_unsuppressall_Click(object sender, RoutedEventArgs e)
        {
            DMXController.UnsuppressAll();
            DMXController.Suppress(new List<int>() { 29, 30 });

            btn_strobe_enable.Content = "OFF";
            btn_strobe_enable.FontWeight = FontWeights.Regular;

            par_solo_suppress.LitLeft = false;
            par_solo_suppress.LitRight = false;
            led_solo_suppress.LitLeft = false;
            led_solo_suppress.LitRight = false;
            cs_solo_suppress.LitLeft = false;
            cs_solo_suppress.LitRight = false;
        }

        private void disco_Click(object sender, RoutedEventArgs e)
        {
            if (!disco.Lit)
            {
                DMXController.SetLevel("DISCO", 16, 255);
                disco.Lit = true;
            }
            else
            {
                DMXController.SetLevel("DISCO", 16, 0);
                disco.Lit = false;
            }
        }

        private void mines_Click(object sender, RoutedEventArgs e)
        {
            if (!mines.Lit)
            {
                DMXController.SetLevel("MINES", 13, 255);
                DMXController.SetLevel("MINES", 14, 255);
                mines.Lit = true;
            }
            else
            {
                DMXController.SetLevel("MINES", 13, 0);
                DMXController.SetLevel("MINES", 14, 0);
                mines.Lit = false;
            }
        }

        private void ropelight_blink_Click(object sender, RoutedEventArgs e)
        {
            if (!ropelight_blink.Lit)
            {
                RopeLightBlink.Start();
                ropelight_blink.Lit = true;
            }
            else
            {
                RopeLightBlink.Stop();
                ropelight_blink.Lit = false;
            }
        }

        private void revo_sound_Click(object sender, RoutedEventArgs e)
        {
            if (!revo_sound.Lit)
            {
                DMXController.SetLevel("REVO", 31, 255);
                revo_sound.Lit = true;
            }
            else
            {
                DMXController.SetLevel("REVO", 31, 0);
                revo_sound.Lit = false;
            }
        }

        private void vue_sound_Click(object sender, RoutedEventArgs e)
        {
            if (!vue_sound.Lit)
            {
                vueA.SetMode(VueFixture.SOUND_ACTIVE_MODE);
                vueB.SetMode(VueFixture.SOUND_ACTIVE_MODE);
                vue_sound.Lit = true;
            }
            else
            {
                vueA.SetMode(VueFixture.MANUAL_MODE);
                vueB.SetMode(VueFixture.MANUAL_MODE);
                vue_sound.Lit = false;
            }
        }

        private void btn_htpflush_Click(object sender, RoutedEventArgs e)
        {
            DMXController.FlushHTPAll();
        }

        #endregion

        private void ledColorFade_Click(object sender, RoutedEventArgs e)
        {
            if (!((SingleBumpButton)sender).Lit)
            {
                ((SingleBumpButton)sender).Lit = true;
                ledBack.colorRotation(2);
            }
            else
            {
                ((SingleBumpButton)sender).Lit = false;
                ledBack.StopColorRotation();
            }
        }

     }
}
