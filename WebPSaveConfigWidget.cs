using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PaintDotNet;

namespace WebPFileType
{
    public partial class WebPSaveConfigWidget : SaveConfigWidget
    {
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
            ((WebPSaveConfigToken)this.token).Preset = (WebPPreset)presetCbo.SelectedIndex;
            ((WebPSaveConfigToken)this.token).Quality = qualitySlider.Value;
            ((WebPSaveConfigToken)this.token).FilterStrength = strengthSlider.Value;
            ((WebPSaveConfigToken)this.token).Sharpness = sharpnessSlider.Value;
            ((WebPSaveConfigToken)this.token).FilterType = (WebPFilterType)filterTypeCbo.SelectedIndex;
            ((WebPSaveConfigToken)this.token).Method = encodeMethodSlider.Value;
            ((WebPSaveConfigToken)this.token).FileSize = (!fileSizeTxt.Enabled || string.IsNullOrEmpty(fileSizeTxt.Text)) ? 0 : int.Parse(fileSizeTxt.Text);
            ((WebPSaveConfigToken)this.token).EncodeMetaData = keepMetaDataCb.Checked;
        }

        protected override void InitWidgetFromToken(PaintDotNet.SaveConfigToken sourceToken)
        {
            if (sourceToken is WebPSaveConfigToken)
            {
                WebPSaveConfigToken configToken = (WebPSaveConfigToken)sourceToken;
                this.presetCbo.SelectedIndex = (int)configToken.Preset;
                this.qualitySlider.Value = configToken.Quality;
                this.strengthSlider.Value = configToken.FilterStrength;
                this.sharpnessSlider.Value = configToken.Sharpness;
                this.filterTypeCbo.SelectedIndex = (int)configToken.FilterType;
                this.encodeMethodSlider.Value = configToken.Method;
                this.fileSizeTxt.Text = configToken.FileSize > 0 ? configToken.FileSize.ToString() : string.Empty;
                this.keepMetaDataCb.Checked = configToken.EncodeMetaData;
            }
            else
            {
                this.presetCbo.SelectedIndex = (int)WebPPreset.Photo;
                this.qualitySlider.Value = 95;
                this.strengthSlider.Value = 20;
                this.sharpnessSlider.Value = 0;
                this.filterTypeCbo.SelectedIndex = (int)WebPFilterType.Simple;
                this.encodeMethodSlider.Value = 4;
                this.fileSizeTxt.Text = string.Empty;
                this.keepMetaDataCb.Checked = true;
            }
        }


        private void presetCbo_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.UpdateToken();
        }

        private void qualitySlider_ValueChanged(object sender, EventArgs e)
        {
            if (qualityUpDown.Value != qualitySlider.Value)
            {
                this.qualityUpDown.Value = this.qualitySlider.Value;
            }

            fileSizeTxt.Enabled = this.qualitySlider.Value < 100;

            this.UpdateToken();
        }

        private void qualityUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (qualitySlider.Value != (int)qualityUpDown.Value)
            {
                this.qualitySlider.Value = (int)qualityUpDown.Value;
            }
        }

        private void strengthSlider_ValueChanged(object sender, EventArgs e)
        {
            if (strengthUpDown.Value != strengthSlider.Value)
            {
                this.strengthUpDown.Value = this.strengthSlider.Value;
            }

            this.UpdateToken();
        }

        private void strengthUpDown1_ValueChanged(object sender, EventArgs e)
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

            this.UpdateToken();
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
            this.UpdateToken();
        }

        private void encodeMethodSlider_ValueChanged(object sender, EventArgs e)
        {
            if (encodeMethodUpDown.Value != encodeMethodSlider.Value)
            {
                this.encodeMethodUpDown.Value = this.encodeMethodSlider.Value;
            }

            this.UpdateToken();
        }

        private void encodeMethodUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (encodeMethodSlider.Value != (int)encodeMethodUpDown.Value)
            {
                this.encodeMethodSlider.Value = (int)encodeMethodUpDown.Value;
            }
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
            this.UpdateToken();
        }

        private void keepMetaDataCb_CheckedChanged(object sender, EventArgs e)
        {
            this.UpdateToken();
        }

        private void donateLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Services are not provided for filetype plugins so use this method.
            System.Diagnostics.Process.Start(@"http://forums.getpaint.net/index.php?showtopic=21773");
        }

    }
}
