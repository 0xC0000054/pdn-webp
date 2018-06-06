////////////////////////////////////////////////////////////////////////
//
// This file is part of pdn-webp, a FileType plugin for Paint.NET
// that loads and saves WebP images.
//
// Copyright (c) 2011-2018 Nicholas Hayes
//
// This file is licensed under the MIT License.
// See LICENSE.txt for complete licensing and attribution information.
//
////////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;
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

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);

            Color backColor = BackColor;
            foreach (Control item in Controls)
            {
                item.BackColor = backColor;
            }
        }

        protected override void OnForeColorChanged(EventArgs e)
        {
            base.OnForeColorChanged(e);

            Color foreColor = ForeColor;
            foreach (Control item in Controls)
            {
                if (item is LinkLabel link)
                {
                    if (foreColor != DefaultForeColor)
                    {
                        link.LinkColor = foreColor;
                    }
                    else
                    {
                        // If the control is using the default foreground color set the link color
                        // to Color.Empty so the LinkLabel will use its default colors.
                        link.LinkColor = Color.Empty;
                    }
                }
                else
                {
                    item.ForeColor = foreColor;
                }
            }
        }

        protected override void InitFileType()
        {
            fileType = new WebPFileType();
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
            suspendTokenUpdateCounter++;
        }

        private void PopSuspendTokenUpdate()
        {
            suspendTokenUpdateCounter--;
        }

        protected override void InitWidgetFromToken(PaintDotNet.SaveConfigToken sourceToken)
        {
            WebPSaveConfigToken configToken = (WebPSaveConfigToken)sourceToken;

            // Disable the UpdateToken function, this fixes a race condition with the controls that are updated by the UpdatePresetSliders method.
            PushSuspendTokenUpdate();

            presetCbo.SelectedIndex = (int)configToken.Preset;
            qualitySlider.Value = configToken.Quality;
            encodeMethodSlider.Value = configToken.Method;
            noiseShapingSlider.Value = configToken.NoiseShaping;
            strengthSlider.Value = configToken.FilterStrength;
            sharpnessSlider.Value = configToken.Sharpness;
            filterTypeCbo.SelectedIndex = (int)configToken.FilterType;
            fileSizeTxt.Text = configToken.FileSize > 0 ? configToken.FileSize.ToString() : string.Empty;
            keepMetaDataCb.Checked = configToken.EncodeMetaData;

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
                    noiseShapingSlider.Value = 80;
                    sharpnessSlider.Value = 4;
                    strengthSlider.Value = 35;
                    break;
                case WebPPreset.Photo:
                    noiseShapingSlider.Value = 80;
                    sharpnessSlider.Value = 3;
                    strengthSlider.Value = 30;
                    break;
                case WebPPreset.Drawing:
                    noiseShapingSlider.Value = 80;
                    sharpnessSlider.Value = 4;
                    strengthSlider.Value = 35;
                    break;
                case WebPPreset.Icon:
                    noiseShapingSlider.Value = 25;
                    sharpnessSlider.Value = 6;
                    strengthSlider.Value = 10;
                    break;
                case WebPPreset.Text:
                    noiseShapingSlider.Value = 0;
                    sharpnessSlider.Value = 0;
                    strengthSlider.Value = 0;
                    break;
                case WebPPreset.Default:
                default:
                    noiseShapingSlider.Value = 50;
                    strengthSlider.Value = 60;
                    sharpnessSlider.Value = 0;
                    break;
            }

            PopSuspendTokenUpdate();
        }

        private void EnableLossyEncodingOptions(bool enabled)
        {
            noiseShapingSlider.Enabled = enabled;
            noiseShapingUpDown.Enabled = enabled;
            strengthSlider.Enabled = enabled;
            strengthUpDown.Enabled = enabled;
            sharpnessSlider.Enabled = enabled;
            sharpnessUpDown.Enabled = enabled;
            filterTypeCbo.Enabled = enabled;
            fileSizeTxt.Enabled = enabled;
        }

        private void presetCbo_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePresetSliders((WebPPreset)presetCbo.SelectedIndex);

            UpdateConfigToken();
        }

        private void qualitySlider_ValueChanged(object sender, EventArgs e)
        {
            if (qualityUpDown.Value != qualitySlider.Value)
            {
                qualityUpDown.Value = qualitySlider.Value;
            }

            EnableLossyEncodingOptions(qualitySlider.Value < 100);

            UpdateConfigToken();
        }

        private void qualityUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (qualitySlider.Value != (int)qualityUpDown.Value)
            {
                qualitySlider.Value = (int)qualityUpDown.Value;
            }
        }

        private void encodeMethodSlider_ValueChanged(object sender, EventArgs e)
        {
            if (encodeMethodUpDown.Value != encodeMethodSlider.Value)
            {
                encodeMethodUpDown.Value = encodeMethodSlider.Value;
            }

            UpdateConfigToken();
        }

        private void encodeMethodUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (encodeMethodSlider.Value != (int)encodeMethodUpDown.Value)
            {
                encodeMethodSlider.Value = (int)encodeMethodUpDown.Value;
            }
        }

        private void noiseShapingSlider_ValueChanged(object sender, EventArgs e)
        {
            if (noiseShapingUpDown.Value != noiseShapingSlider.Value)
            {
                noiseShapingUpDown.Value = noiseShapingSlider.Value;
            }

            UpdateConfigToken();
        }

        private void noiseShapingUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (noiseShapingSlider.Value != noiseShapingUpDown.Value)
            {
                noiseShapingSlider.Value = (int)noiseShapingUpDown.Value;
            }
        }

        private void strengthSlider_ValueChanged(object sender, EventArgs e)
        {
            if (strengthUpDown.Value != strengthSlider.Value)
            {
                strengthUpDown.Value = strengthSlider.Value;
            }

            UpdateConfigToken();
        }

        private void strengthUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (strengthSlider.Value != (int)strengthUpDown.Value)
            {
                strengthSlider.Value = (int)strengthUpDown.Value;
            }
        }

        private void sharpnessSlider_ValueChanged(object sender, EventArgs e)
        {
            if (sharpnessUpDown.Value != sharpnessSlider.Value)
            {
                sharpnessUpDown.Value = sharpnessSlider.Value;
            }

            UpdateConfigToken();
        }

        private void sharpnessUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (sharpnessSlider.Value != (int)sharpnessUpDown.Value)
            {
                sharpnessSlider.Value = (int)sharpnessUpDown.Value;
            }
        }

        private void filterTypeCbo_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateConfigToken();
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
            UpdateConfigToken();
        }

        private void keepMetaDataCb_CheckedChanged(object sender, EventArgs e)
        {
            UpdateConfigToken();
        }

        private void donateLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Services are not provided for filetype plugins so use this method.
            System.Diagnostics.Process.Start(@"http://forums.getpaint.net/index.php?showtopic=21773");
        }
    }
}
