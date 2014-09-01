using System;
using System.Windows.Forms;
using PaintDotNet;

namespace WebPFileType
{
    internal partial class WebPSaveConfigWidget : SaveConfigWidget
    {
        private int suspendTokenUpdateCounter;

        public WebPSaveConfigWidget()
        {
            InitializeComponent();
        }

        protected override void InitFileType()
        {
            this.fileType = new WebPFileType();
        }

        protected override void InitTokenFromWidget()
        {
            WebPSaveConfigToken configToken = (WebPSaveConfigToken)base.token; 
            
            configToken.Preset = (WebPPreset)presetCbo.SelectedIndex;
            configToken.Quality = qualitySlider.Value;
            configToken.Method = encodeMethodSlider.Value;
            configToken.NoiseShaping = noiseShapingSlider.Value;
            configToken.FilterStrength = strengthSlider.Value;
            configToken.Sharpness = sharpnessSlider.Value;
            configToken.FilterType = (WebPFilterType)filterTypeCbo.SelectedIndex;
            configToken.FileSize = (!fileSizeTxt.Enabled || string.IsNullOrEmpty(fileSizeTxt.Text)) ? 0 : int.Parse(fileSizeTxt.Text);
            configToken.EncodeMetaData = keepMetaDataCb.Checked;
        }

        private void PushSuspendTokenUpdate()
        {
            this.suspendTokenUpdateCounter++;
        }

        private void PopSuspendTokenUpdate()
        {
            this.suspendTokenUpdateCounter--;
        }

        protected override void InitWidgetFromToken(PaintDotNet.SaveConfigToken sourceToken)
        {
            WebPSaveConfigToken configToken = (WebPSaveConfigToken)sourceToken;

            // Disable the UpdateToken function, this fixes a race condition with the controls that are updated by the UpdatePresetSliders method.
            PushSuspendTokenUpdate();

            this.presetCbo.SelectedIndex = (int)configToken.Preset;
            this.qualitySlider.Value = configToken.Quality;
            this.encodeMethodSlider.Value = configToken.Method;
            this.noiseShapingSlider.Value = configToken.NoiseShaping;
            this.strengthSlider.Value = configToken.FilterStrength;
            this.sharpnessSlider.Value = configToken.Sharpness;
            this.filterTypeCbo.SelectedIndex = (int)configToken.FilterType;
            this.fileSizeTxt.Text = configToken.FileSize > 0 ? configToken.FileSize.ToString() : string.Empty;
            this.keepMetaDataCb.Checked = configToken.EncodeMetaData;

            PopSuspendTokenUpdate();
        }

        private void UpdateConfigToken()
        {
            if (suspendTokenUpdateCounter == 0)
            {
                UpdateToken();
            }
        }

        private void UpdatePresetSliders(WebPPreset preset)
        {
            PushSuspendTokenUpdate();

            switch (preset)
            {
                case WebPPreset.Picture:
                    this.noiseShapingSlider.Value = 80;
                    this.sharpnessSlider.Value = 4;
                    this.strengthSlider.Value = 35;
                    break;
                case WebPPreset.Photo:
                    this.noiseShapingSlider.Value = 80;
                    this.sharpnessSlider.Value = 3;
                    this.strengthSlider.Value = 30;
                    break;
                case WebPPreset.Drawing:
                    this.noiseShapingSlider.Value = 80;
                    this.sharpnessSlider.Value = 4;
                    this.strengthSlider.Value = 35;
                    break;
                case WebPPreset.Icon:
                    this.noiseShapingSlider.Value = 25;
                    this.sharpnessSlider.Value = 6;
                    this.strengthSlider.Value = 10;
                    break;
                case WebPPreset.Text:
                    this.noiseShapingSlider.Value = 0;
                    this.sharpnessSlider.Value = 0;
                    this.strengthSlider.Value = 0;
                    break;
                case WebPPreset.Default:
                default:
                    this.noiseShapingSlider.Value = 50;
                    this.strengthSlider.Value = 60;
                    this.sharpnessSlider.Value = 0;
                    break;
            }

            PopSuspendTokenUpdate();
        }

        private void EnableLossyEncodingOptions(bool enabled)
        {
            this.noiseShapingSlider.Enabled = enabled;
            this.noiseShapingUpDown.Enabled = enabled;
            this.strengthSlider.Enabled = enabled;
            this.strengthUpDown.Enabled = enabled;
            this.sharpnessSlider.Enabled = enabled;
            this.sharpnessUpDown.Enabled = enabled;
            this.filterTypeCbo.Enabled = enabled;
            this.fileSizeTxt.Enabled = enabled;
        }

        private void presetCbo_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePresetSliders((WebPPreset)this.presetCbo.SelectedIndex);

            this.UpdateConfigToken();
        }

        private void qualitySlider_ValueChanged(object sender, EventArgs e)
        {
            if (qualityUpDown.Value != qualitySlider.Value)
            {
                this.qualityUpDown.Value = this.qualitySlider.Value;
            }

            EnableLossyEncodingOptions(this.qualitySlider.Value < 100);

            this.UpdateConfigToken();
        }

        private void qualityUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (qualitySlider.Value != (int)qualityUpDown.Value)
            {
                this.qualitySlider.Value = (int)qualityUpDown.Value;
            }
        }

        private void encodeMethodSlider_ValueChanged(object sender, EventArgs e)
        {
            if (encodeMethodUpDown.Value != encodeMethodSlider.Value)
            {
                this.encodeMethodUpDown.Value = this.encodeMethodSlider.Value;
            }

            this.UpdateConfigToken();
        }

        private void encodeMethodUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (encodeMethodSlider.Value != (int)encodeMethodUpDown.Value)
            {
                this.encodeMethodSlider.Value = (int)encodeMethodUpDown.Value;
            }
        }

        private void noiseShapingSlider_ValueChanged(object sender, EventArgs e)
        {
            if (noiseShapingUpDown.Value != noiseShapingSlider.Value)
            {
                this.noiseShapingUpDown.Value = this.noiseShapingSlider.Value;
            }

            this.UpdateConfigToken();
        }

        private void noiseShapingUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (noiseShapingSlider.Value != noiseShapingUpDown.Value)
            {
                this.noiseShapingSlider.Value = (int)this.noiseShapingUpDown.Value;
            }
        }

        private void strengthSlider_ValueChanged(object sender, EventArgs e)
        {
            if (strengthUpDown.Value != strengthSlider.Value)
            {
                this.strengthUpDown.Value = this.strengthSlider.Value;
            }

            this.UpdateConfigToken();
        }

        private void strengthUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (strengthSlider.Value != (int)strengthUpDown.Value)
            {
                this.strengthSlider.Value = (int)strengthUpDown.Value;
            }
        }

        private void sharpnessSlider_ValueChanged(object sender, EventArgs e)
        {
            if (sharpnessUpDown.Value != sharpnessSlider.Value)
            {
                this.sharpnessUpDown.Value = this.sharpnessSlider.Value;
            }

            this.UpdateConfigToken();
        }

        private void sharpnessUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (sharpnessSlider.Value != (int)sharpnessUpDown.Value)
            {
                this.sharpnessSlider.Value = (int)sharpnessUpDown.Value;
            }
        }

        private void filterTypeCbo_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.UpdateConfigToken();
        }

        private void fileSizeTxt_KeyDown(object sender, KeyEventArgs e)
        {
            if (((e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9 || e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9) && !e.Shift) 
                || e.KeyCode == Keys.Back || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right || e.KeyCode == Keys.Delete)
            {
                e.Handled = true;
                e.SuppressKeyPress = false;
            }
            else
            {
                e.Handled = false;
                e.SuppressKeyPress = true;
            }
        }

        private void fileSizeTxt_TextChanged(object sender, EventArgs e)
        {
            this.UpdateConfigToken();
        }

        private void keepMetaDataCb_CheckedChanged(object sender, EventArgs e)
        {
            this.UpdateConfigToken();
        }

        private void donateLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Services are not provided for filetype plugins so use this method.
            System.Diagnostics.Process.Start(@"http://forums.getpaint.net/index.php?showtopic=21773");
        }

    }
}
