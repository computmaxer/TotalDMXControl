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
using System.ComponentModel;

namespace Total_DMX_Control_WPF
{
    /// <summary>
    /// Interaction logic for RoutinePlayerWindow.xaml
    /// </summary>
    public partial class RoutinePlayerWindow : Window
    {

        #region Data Members

        private BindingList<RoutinePlayer> _players;
        private BindingList<FixtureWrapperFAOverride> _fixtureWrappers;
        //store current preset we are building, to be added to the list later.
        private AttributePreset _preset;
        //are we actively overriding?
        private bool _active;
        private bool _createNew;
        #endregion

        #region Properties

        public AttributePreset CurrentPreset
        {
            get { return _preset; }
        }

        #endregion

        #region Methods

        public RoutinePlayerWindow()
        {
            //Initialization
            InitializeComponent();
            _players = new BindingList<RoutinePlayer>();
            _fixtureWrappers = new BindingList<FixtureWrapperFAOverride>();
            _active = false;
            _createNew = false;

            //Routine list
            foreach (Routine routine in Controller.Routines)
            {
                RoutinePlayer player = new RoutinePlayer(routine);
                _players.Add(player);
            }
            lbxRoutines.ItemsSource = _players;
            
            lbxPresets.ItemsSource = Controller.AttributePresets;

            //Add Fixtures to Affects list
            foreach (Fixture fixture in Controller.Fixtures)
            {
                _fixtureWrappers.Add(new FixtureWrapperFAOverride(fixture));
            }
            lbxFixturesToAffect.ItemsSource = _fixtureWrappers;

            //create initial preset
            GenerateFreshPreset();

        }

        #endregion

        private void lbxRoutines_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lbxRoutines.SelectedItem != null)
            {
                Routine theRoutine = ((RoutinePlayer)lbxRoutines.SelectedItem).Routine;
                (new RoutineBuilder(theRoutine)).Show();
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Routine newRoutine = new Routine();
            Controller.Routines.Add(newRoutine);
            RoutineBuilder builder = new RoutineBuilder(newRoutine);
            builder.Show();
            //BADDDDD
            WindowManager.CurrentViews[typeof(RoutineBuilder)] = builder;
        }

        private void btnDeleteRoutine_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Do you really want to delete this routine?","Delete confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                RoutinePlayer player = (RoutinePlayer)lbxRoutines.SelectedItem;
                int index = _players.IndexOf(player);
                _players.Remove(player);
                Controller.Routines.Remove(player.Routine);
                lbxRoutines.SelectedIndex = index;
            }
        }


        //FA OVERRIDE
        private void GenerateFreshPreset()
        {
            _preset = new AttributePreset();
            foreach (ATTRIBUTE_TYPE type in (ATTRIBUTE_TYPE[])Enum.GetValues(typeof(ATTRIBUTE_TYPE)))
            {
                if (!(type == ATTRIBUTE_TYPE.PAN_COARSE || type == ATTRIBUTE_TYPE.PAN_FINE || type == ATTRIBUTE_TYPE.TILT_COARSE || type == ATTRIBUTE_TYPE.TILT_FINE))
                {
                    _preset.AttributePointSettings.Add(new AttributePresetSetting(type));
                }
            }
            lbxAttributeSettings.ItemsSource = _preset.AttributePointSettings;
        }

        private void btnSavePreset_Click(object sender, RoutedEventArgs e)
        {
            if (_createNew)
            {
                lbxPresets.SelectedIndex = -1;
                GenerateFreshPreset();
                btnSavePreset.Content = "Save Preset";
                tbxPresetName.IsEnabled = true;
                _createNew = false;
            }
            else
            {
                if (tbxPresetName.Text == "" || tbxPresetName.Text == null)
                {
                    MessageBox.Show("Enter a name for this preset before saving.");
                    return;
                }
                _preset.Name = tbxPresetName.Text;
                tbxPresetName.Text = "";
                Controller.AttributePresets.Add(_preset);
                GenerateFreshPreset();
            }
        }

        private void lbxPresets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbxPresets.SelectedIndex != -1)
            {
                foreach (AttributePreset preset in Controller.AttributePresets)
                {
                    if(lbxPresets.SelectedItems.Contains(preset)) 
                    {
                        ActivateCurrentPreset(preset);
                    }
                    else
                    {
                        DeactivateCurrentPreset(preset);
                    }
                }
                
                _preset = (AttributePreset)lbxPresets.SelectedItem;
                lbxAttributeSettings.ItemsSource = _preset.AttributePointSettings;

                //Change save button into "new"
                btnSavePreset.Content = "New Preset";
                tbxPresetName.IsEnabled = false;
                _createNew = true;
            }
        }

        //TODO: Change name to DeactivatePreset ?
        public void DeactivateCurrentPreset(AttributePreset preset)
        {
            foreach (FixtureWrapperFAOverride fixtureWrapper in _fixtureWrappers)
            {
                //Not checking if IsAffected cause we are still using HTP.  IF we stop using HTP, change this.
                foreach (FixtureAttribute attr in fixtureWrapper.Fixture.Attributes)
                {
                    attr.SetOverride(false);
                    attr.SetLevel(0, "_FA_OVERRIDE_nohtp_");
                }
            }
        }

        // TODO: Change name to ActivatePreset ?
        public void ActivateCurrentPreset(AttributePreset preset)
        {
            if (_active)
            {
                foreach (FixtureWrapperFAOverride fixtureWrapper in _fixtureWrappers)
                {
                    if (fixtureWrapper.IsAffected)
                    {
                        foreach (AttributePresetSetting setting in preset.AttributePointSettings)
                        {
                            if (setting.Active)
                            {
                                FixtureAttribute containsSameType = null;
                                foreach (FixtureAttribute attr in fixtureWrapper.Fixture.Attributes)
                                {
                                    if (attr.Type == setting.Type)
                                    {
                                        containsSameType = attr;
                                        break;
                                    }
                                }
                                if (containsSameType != null)
                                {
                                    containsSameType.SetOverride(true);
                                    containsSameType.SetLevel(setting.Value, "_FA_OVERRIDE_nohtp_");
                                }
                            }
                        }
                    }
                }
            }
        }

        private void btnActive_Click(object sender, RoutedEventArgs e)
        {
            if (_active)
            {
                _active = false;
                btnActive.Content = "INACTIVE";
                foreach (AttributePreset preset in Controller.AttributePresets)
                {
                    DeactivateCurrentPreset(preset);
                }
            }
            else
            {
                //set active
                _active = true;
                btnActive.Content = "ACTIVE!";
                foreach (AttributePreset preset in Controller.AttributePresets)
                {
                    ActivateCurrentPreset(preset);
                }
            }
            
        }

        private void DeletePreset_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Image img = (Image)sender;
            if (img.DataContext.GetType() == typeof(AttributePreset))
            {
                AttributePreset preset = (AttributePreset)img.DataContext;
                AttributePreset temp = _preset;
                _preset = preset;
                DeactivateCurrentPreset(_preset);
                Controller.AttributePresets.Remove(preset);
                _preset = temp;
                ActivateCurrentPreset(_preset);
            }
        }

        private void PulsePresetDown()
        {
            _active = true;
            ActivateCurrentPreset(_preset);
        }

        private void PulsePresetUp()
        {
            _active = false;
            DeactivateCurrentPreset(_preset);
        }

        private void btnPulse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PulsePresetDown();
        }

        private void btnPulse_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            PulsePresetUp();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.P)
            {
                PulsePresetDown();
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.P)
            {
                PulsePresetUp();
            }
        }
    }
}
