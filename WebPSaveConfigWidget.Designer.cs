using PaintDotNet;
namespace WebPFileType
{
    partial class WebPSaveConfigWidget
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.presetCbo = new System.Windows.Forms.ComboBox();
            this.qualitySlider = new System.Windows.Forms.TrackBar();
            this.qualityUpDown = new System.Windows.Forms.NumericUpDown();
            this.strengthUpDown = new System.Windows.Forms.NumericUpDown();
            this.strengthSlider = new System.Windows.Forms.TrackBar();
            this.sharpnessUpDown = new System.Windows.Forms.NumericUpDown();
            this.sharpnessSlider = new System.Windows.Forms.TrackBar();
            this.filterTypeCbo = new System.Windows.Forms.ComboBox();
            this.encodeMethodUpDown = new System.Windows.Forms.NumericUpDown();
            this.encodeMethodSlider = new System.Windows.Forms.TrackBar();
            this.fileSizeTxt = new System.Windows.Forms.TextBox();
            this.donateLink = new System.Windows.Forms.LinkLabel();
            this.keepMetaDataCb = new System.Windows.Forms.CheckBox();
            this.noiseShapingUpDown = new System.Windows.Forms.NumericUpDown();
            this.noiseShapingSlider = new System.Windows.Forms.TrackBar();
            this.strengthLbl = new PaintDotNet.HeaderLabel();
            this.noiseShapingLbl = new PaintDotNet.HeaderLabel();
            this.encodeMethodLbl = new PaintDotNet.HeaderLabel();
            this.fileSizeLbl = new PaintDotNet.HeaderLabel();
            this.filterTypeLbl = new PaintDotNet.HeaderLabel();
            this.sharpnessLbl = new PaintDotNet.HeaderLabel();
            this.qualityLbl = new PaintDotNet.HeaderLabel();
            this.presetLbl = new PaintDotNet.HeaderLabel();
            ((System.ComponentModel.ISupportInitialize)(this.qualitySlider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.qualityUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.strengthUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.strengthSlider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sharpnessUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sharpnessSlider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.encodeMethodUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.encodeMethodSlider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.noiseShapingUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.noiseShapingSlider)).BeginInit();
            this.SuspendLayout();
            // 
            // presetCbo
            // 
            this.presetCbo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.presetCbo.FormattingEnabled = true;
            this.presetCbo.Items.AddRange(new object[] {
            "Default",
            "Picture",
            "Photo",
            "Drawing",
            "Icon",
            "Text"});
            this.presetCbo.Location = new System.Drawing.Point(7, 21);
            this.presetCbo.Name = "presetCbo";
            this.presetCbo.Size = new System.Drawing.Size(121, 21);
            this.presetCbo.TabIndex = 1;
            this.presetCbo.SelectedIndexChanged += new System.EventHandler(this.presetCbo_SelectedIndexChanged);
            // 
            // qualitySlider
            // 
            this.qualitySlider.Location = new System.Drawing.Point(0, 69);
            this.qualitySlider.Maximum = 100;
            this.qualitySlider.Name = "qualitySlider";
            this.qualitySlider.Size = new System.Drawing.Size(104, 45);
            this.qualitySlider.TabIndex = 3;
            this.qualitySlider.TickFrequency = 10;
            this.qualitySlider.ValueChanged += new System.EventHandler(this.qualitySlider_ValueChanged);
            // 
            // qualityUpDown
            // 
            this.qualityUpDown.Location = new System.Drawing.Point(103, 69);
            this.qualityUpDown.Name = "qualityUpDown";
            this.qualityUpDown.Size = new System.Drawing.Size(43, 20);
            this.qualityUpDown.TabIndex = 4;
            this.qualityUpDown.ValueChanged += new System.EventHandler(this.qualityUpDown_ValueChanged);
            // 
            // strengthUpDown
            // 
            this.strengthUpDown.Location = new System.Drawing.Point(103, 227);
            this.strengthUpDown.Name = "strengthUpDown";
            this.strengthUpDown.Size = new System.Drawing.Size(43, 20);
            this.strengthUpDown.TabIndex = 7;
            this.strengthUpDown.ValueChanged += new System.EventHandler(this.strengthUpDown_ValueChanged);
            // 
            // strengthSlider
            // 
            this.strengthSlider.Location = new System.Drawing.Point(0, 226);
            this.strengthSlider.Maximum = 100;
            this.strengthSlider.Name = "strengthSlider";
            this.strengthSlider.Size = new System.Drawing.Size(104, 45);
            this.strengthSlider.TabIndex = 6;
            this.strengthSlider.TickFrequency = 10;
            this.strengthSlider.ValueChanged += new System.EventHandler(this.strengthSlider_ValueChanged);
            // 
            // sharpnessUpDown
            // 
            this.sharpnessUpDown.Location = new System.Drawing.Point(103, 279);
            this.sharpnessUpDown.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.sharpnessUpDown.Name = "sharpnessUpDown";
            this.sharpnessUpDown.Size = new System.Drawing.Size(43, 20);
            this.sharpnessUpDown.TabIndex = 13;
            this.sharpnessUpDown.ValueChanged += new System.EventHandler(this.sharpnessUpDown_ValueChanged);
            // 
            // sharpnessSlider
            // 
            this.sharpnessSlider.Location = new System.Drawing.Point(0, 279);
            this.sharpnessSlider.Maximum = 7;
            this.sharpnessSlider.Name = "sharpnessSlider";
            this.sharpnessSlider.Size = new System.Drawing.Size(104, 45);
            this.sharpnessSlider.TabIndex = 12;
            this.sharpnessSlider.ValueChanged += new System.EventHandler(this.sharpnessSlider_ValueChanged);
            // 
            // filterTypeCbo
            // 
            this.filterTypeCbo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.filterTypeCbo.FormattingEnabled = true;
            this.filterTypeCbo.Items.AddRange(new object[] {
            "Simple",
            "Strong"});
            this.filterTypeCbo.Location = new System.Drawing.Point(3, 330);
            this.filterTypeCbo.Name = "filterTypeCbo";
            this.filterTypeCbo.Size = new System.Drawing.Size(124, 21);
            this.filterTypeCbo.TabIndex = 15;
            this.filterTypeCbo.SelectedIndexChanged += new System.EventHandler(this.filterTypeCbo_SelectedIndexChanged);
            // 
            // encodeMethodUpDown
            // 
            this.encodeMethodUpDown.Location = new System.Drawing.Point(103, 124);
            this.encodeMethodUpDown.Maximum = new decimal(new int[] {
            6,
            0,
            0,
            0});
            this.encodeMethodUpDown.Name = "encodeMethodUpDown";
            this.encodeMethodUpDown.Size = new System.Drawing.Size(43, 20);
            this.encodeMethodUpDown.TabIndex = 6;
            this.encodeMethodUpDown.ValueChanged += new System.EventHandler(this.encodeMethodUpDown_ValueChanged);
            // 
            // encodeMethodSlider
            // 
            this.encodeMethodSlider.Location = new System.Drawing.Point(0, 125);
            this.encodeMethodSlider.Maximum = 6;
            this.encodeMethodSlider.Name = "encodeMethodSlider";
            this.encodeMethodSlider.Size = new System.Drawing.Size(104, 45);
            this.encodeMethodSlider.TabIndex = 5;
            this.encodeMethodSlider.ValueChanged += new System.EventHandler(this.encodeMethodSlider_ValueChanged);
            // 
            // fileSizeTxt
            // 
            this.fileSizeTxt.Location = new System.Drawing.Point(2, 375);
            this.fileSizeTxt.Name = "fileSizeTxt";
            this.fileSizeTxt.Size = new System.Drawing.Size(100, 20);
            this.fileSizeTxt.TabIndex = 20;
            this.fileSizeTxt.TextChanged += new System.EventHandler(this.fileSizeTxt_TextChanged);
            this.fileSizeTxt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.fileSizeTxt_KeyDown);
            // 
            // donateLink
            // 
            this.donateLink.AutoSize = true;
            this.donateLink.Location = new System.Drawing.Point(4, 422);
            this.donateLink.Name = "donateLink";
            this.donateLink.Size = new System.Drawing.Size(45, 13);
            this.donateLink.TabIndex = 21;
            this.donateLink.TabStop = true;
            this.donateLink.Text = "Donate!";
            this.donateLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.donateLink_LinkClicked);
            // 
            // keepMetaDataCb
            // 
            this.keepMetaDataCb.AutoSize = true;
            this.keepMetaDataCb.Location = new System.Drawing.Point(7, 402);
            this.keepMetaDataCb.Name = "keepMetaDataCb";
            this.keepMetaDataCb.Size = new System.Drawing.Size(101, 17);
            this.keepMetaDataCb.TabIndex = 22;
            this.keepMetaDataCb.Text = "Keep MetaData";
            this.keepMetaDataCb.UseVisualStyleBackColor = true;
            this.keepMetaDataCb.CheckedChanged += new System.EventHandler(this.keepMetaDataCb_CheckedChanged);
            // 
            // noiseShapingUpDown
            // 
            this.noiseShapingUpDown.Location = new System.Drawing.Point(103, 176);
            this.noiseShapingUpDown.Name = "noiseShapingUpDown";
            this.noiseShapingUpDown.Size = new System.Drawing.Size(43, 20);
            this.noiseShapingUpDown.TabIndex = 8;
            this.noiseShapingUpDown.ValueChanged += new System.EventHandler(this.noiseShapingUpDown_ValueChanged);
            // 
            // noiseShapingSlider
            // 
            this.noiseShapingSlider.Location = new System.Drawing.Point(0, 176);
            this.noiseShapingSlider.Maximum = 100;
            this.noiseShapingSlider.Name = "noiseShapingSlider";
            this.noiseShapingSlider.Size = new System.Drawing.Size(104, 45);
            this.noiseShapingSlider.TabIndex = 7;
            this.noiseShapingSlider.TickFrequency = 10;
            this.noiseShapingSlider.ValueChanged += new System.EventHandler(this.noiseShapingSlider_ValueChanged);
            // 
            // strengthLbl
            // 
            this.strengthLbl.ForeColor = System.Drawing.SystemColors.Highlight;
            this.strengthLbl.Location = new System.Drawing.Point(0, 206);
            this.strengthLbl.Name = "strengthLbl";
            this.strengthLbl.Size = new System.Drawing.Size(144, 14);
            this.strengthLbl.TabIndex = 5;
            this.strengthLbl.TabStop = false;
            this.strengthLbl.Text = "Filter Strength";
            // 
            // noiseShapingLbl
            // 
            this.noiseShapingLbl.ForeColor = System.Drawing.SystemColors.Highlight;
            this.noiseShapingLbl.Location = new System.Drawing.Point(0, 155);
            this.noiseShapingLbl.Name = "noiseShapingLbl";
            this.noiseShapingLbl.Size = new System.Drawing.Size(144, 14);
            this.noiseShapingLbl.TabIndex = 23;
            this.noiseShapingLbl.TabStop = false;
            this.noiseShapingLbl.Text = "Spatial Noise Shaping";
            // 
            // encodeMethodLbl
            // 
            this.encodeMethodLbl.ForeColor = System.Drawing.SystemColors.Highlight;
            this.encodeMethodLbl.Location = new System.Drawing.Point(5, 104);
            this.encodeMethodLbl.Name = "encodeMethodLbl";
            this.encodeMethodLbl.Size = new System.Drawing.Size(144, 14);
            this.encodeMethodLbl.TabIndex = 16;
            this.encodeMethodLbl.TabStop = false;
            this.encodeMethodLbl.Text = "Encoding Speed / Quality";
            // 
            // fileSizeLbl
            // 
            this.fileSizeLbl.ForeColor = System.Drawing.SystemColors.Highlight;
            this.fileSizeLbl.Location = new System.Drawing.Point(5, 354);
            this.fileSizeLbl.Name = "fileSizeLbl";
            this.fileSizeLbl.Size = new System.Drawing.Size(144, 14);
            this.fileSizeLbl.TabIndex = 19;
            this.fileSizeLbl.TabStop = false;
            this.fileSizeLbl.Text = "File size in bytes";
            // 
            // filterTypeLbl
            // 
            this.filterTypeLbl.ForeColor = System.Drawing.SystemColors.Highlight;
            this.filterTypeLbl.Location = new System.Drawing.Point(2, 310);
            this.filterTypeLbl.Name = "filterTypeLbl";
            this.filterTypeLbl.Size = new System.Drawing.Size(144, 14);
            this.filterTypeLbl.TabIndex = 14;
            this.filterTypeLbl.TabStop = false;
            this.filterTypeLbl.Text = "Filter Type";
            // 
            // sharpnessLbl
            // 
            this.sharpnessLbl.ForeColor = System.Drawing.SystemColors.Highlight;
            this.sharpnessLbl.Location = new System.Drawing.Point(3, 259);
            this.sharpnessLbl.Name = "sharpnessLbl";
            this.sharpnessLbl.Size = new System.Drawing.Size(144, 14);
            this.sharpnessLbl.TabIndex = 11;
            this.sharpnessLbl.TabStop = false;
            this.sharpnessLbl.Text = "Sharpness";
            // 
            // qualityLbl
            // 
            this.qualityLbl.ForeColor = System.Drawing.SystemColors.Highlight;
            this.qualityLbl.Location = new System.Drawing.Point(4, 48);
            this.qualityLbl.Name = "qualityLbl";
            this.qualityLbl.Size = new System.Drawing.Size(144, 14);
            this.qualityLbl.TabIndex = 2;
            this.qualityLbl.TabStop = false;
            this.qualityLbl.Text = "Quality";
            // 
            // presetLbl
            // 
            this.presetLbl.ForeColor = System.Drawing.SystemColors.WindowText;
            this.presetLbl.Location = new System.Drawing.Point(4, 4);
            this.presetLbl.Name = "presetLbl";
            this.presetLbl.Size = new System.Drawing.Size(65535, 13);
            this.presetLbl.TabIndex = 0;
            this.presetLbl.TabStop = false;
            this.presetLbl.Text = "Preset";
            // 
            // WebPSaveConfigWidget
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.strengthLbl);
            this.Controls.Add(this.noiseShapingUpDown);
            this.Controls.Add(this.noiseShapingSlider);
            this.Controls.Add(this.noiseShapingLbl);
            this.Controls.Add(this.encodeMethodLbl);
            this.Controls.Add(this.keepMetaDataCb);
            this.Controls.Add(this.donateLink);
            this.Controls.Add(this.fileSizeTxt);
            this.Controls.Add(this.fileSizeLbl);
            this.Controls.Add(this.filterTypeCbo);
            this.Controls.Add(this.filterTypeLbl);
            this.Controls.Add(this.sharpnessUpDown);
            this.Controls.Add(this.sharpnessSlider);
            this.Controls.Add(this.sharpnessLbl);
            this.Controls.Add(this.strengthUpDown);
            this.Controls.Add(this.strengthSlider);
            this.Controls.Add(this.qualityUpDown);
            this.Controls.Add(this.qualitySlider);
            this.Controls.Add(this.qualityLbl);
            this.Controls.Add(this.presetCbo);
            this.Controls.Add(this.presetLbl);
            this.Controls.Add(this.encodeMethodUpDown);
            this.Controls.Add(this.encodeMethodSlider);
            this.Name = "WebPSaveConfigWidget";
            this.Size = new System.Drawing.Size(150, 442);
            ((System.ComponentModel.ISupportInitialize)(this.qualitySlider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.qualityUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.strengthUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.strengthSlider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sharpnessUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sharpnessSlider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.encodeMethodUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.encodeMethodSlider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.noiseShapingUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.noiseShapingSlider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private HeaderLabel presetLbl;
        private System.Windows.Forms.ComboBox presetCbo;
        private HeaderLabel qualityLbl;
        private System.Windows.Forms.TrackBar qualitySlider;
        private System.Windows.Forms.NumericUpDown qualityUpDown;
        private System.Windows.Forms.NumericUpDown strengthUpDown;
        private System.Windows.Forms.TrackBar strengthSlider;
        private HeaderLabel strengthLbl;
        private System.Windows.Forms.NumericUpDown sharpnessUpDown;
        private System.Windows.Forms.TrackBar sharpnessSlider;
        private HeaderLabel sharpnessLbl;
        private HeaderLabel filterTypeLbl;
        private System.Windows.Forms.ComboBox filterTypeCbo;
        private System.Windows.Forms.NumericUpDown encodeMethodUpDown;
        private System.Windows.Forms.TrackBar encodeMethodSlider;
        private HeaderLabel encodeMethodLbl;
        private HeaderLabel fileSizeLbl;
        private System.Windows.Forms.TextBox fileSizeTxt;
        private System.Windows.Forms.LinkLabel donateLink;
        private System.Windows.Forms.CheckBox keepMetaDataCb;
        private System.Windows.Forms.NumericUpDown noiseShapingUpDown;
        private System.Windows.Forms.TrackBar noiseShapingSlider;
        private HeaderLabel noiseShapingLbl;
    }
}