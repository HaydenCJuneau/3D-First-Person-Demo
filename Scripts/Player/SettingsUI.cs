using Godot;
using System;
using Game.Entities;

/*
    When pulling up the settings UI, this script needs to collect all the current values of these properties
    and assign them correctly. Then, settings can be switched around by changing the values of these variables.
    Once the user tries to exit the menu, we need to ask if they want to apply changes (if any have been made) 
    and if they apply, then send all the data from this node back to where it was retrieved.
 
 */

namespace Game.Scenes.UI
{
    public class SettingsUI : Control
    {
        // - - UI stats and variables - - 

        //--State
        private bool ChangesMade { get; set; } = false;

        //TODO: Make this system modular, use dictionaries so that editable settings to not have to be hard coded.

        //--Editable stats
        public int FOVSetting { get { return (int)FOVBox.Value; } }
        public float SensitivitySetting { get { return (float)SensitivityBox.Value; } }
        public int PlayerMoveSpeedSetting { get { return (int)MoveSpeedBox.Value; } }
        public int PlayerJumpHeightSetting { get { return (int)JumpHeightBox.Value; } }
        public float ObjectHoldDistanceSetting { get { return (float)HoldDistanceBox.Value; } }

        //--Nodes
        private Player PlayerParent { get; set; }
        private SpinBox FOVBox { get; set; }
        private SpinBox SensitivityBox { get; set; }
        private SpinBox MoveSpeedBox { get; set; }
        private SpinBox JumpHeightBox { get; set; }
        private SpinBox HoldDistanceBox { get; set; }
        private PopupDialog SaveChangesDialog { get; set; }

        // - - - GDMethods - - -
        public override void _Ready()
        {
            base._Ready();

            FOVBox = GetNode<SpinBox>("FOVBox");
            SensitivityBox = GetNode<SpinBox>("SensitivityBox");
            MoveSpeedBox = GetNode<SpinBox>("MoveSpeedBox");
            JumpHeightBox = GetNode<SpinBox>("JumpHeightBox");
            HoldDistanceBox = GetNode<SpinBox>("HoldDistanceBox");
            SaveChangesDialog = GetNode<PopupDialog>("SaveChangesDialog");
        }

        public override void _Input(InputEvent @event)
        {
            base._Input(@event);

            if (Visible && @event.IsActionPressed("ui_cancel")) 
            {
                GetTree().SetInputAsHandled();
                ResumeButtonPressed();
            }
        }

        // - - Importing and managing settings data - - 
        public void ImportSettings(Player player)
        {
            PlayerParent = player;
            FOVBox.Value = player.CameraFOV;
            SensitivityBox.Value = player.CameraSensitivity;
            MoveSpeedBox.Value = player.MoveSpeed;
            JumpHeightBox.Value = player.JumpHeight;
            HoldDistanceBox.Value = player.ObjectHoldDistance;
            ChangesMade = false;
        }

        // - - Signals - - 
        public void SettingValueChanged(float value) => ChangesMade = true;

        public void ResumeButtonPressed()
        {
            //If there are changes, ask to save them, otherwise return to game
            if(ChangesMade) { SaveChangesDialog.PopupCentered(); }
            else { PlayerParent.ApplySettings(this, false, true); }
        }

        public void ExitButtonPressed()
        {
            //Send out some sort of event that the game is closing and then quit game
            GetTree().Quit();
        }

        public void ApplyButtonPressed()
        {
            PlayerParent.ApplySettings(this, true, false);
            ChangesMade = false;
        }

        public void PopupSaveButtonPressed()
        {
            PlayerParent.ApplySettings(this, true, true);
            SaveChangesDialog.Visible = false;
        }
        public void PopupCancelButtonPressed()
        {
            PlayerParent.ApplySettings(this, false, true);
            SaveChangesDialog.Visible = false;
        }
    }
}