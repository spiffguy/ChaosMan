using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ChaosMan
{
    public partial class OptionsForm : Form
    {
        //int m_lineWidth;
        //int m_circleSize;

        public OptionsForm()
        {
            InitializeComponent();

            armDepthTrackBar.Value = Helpers.ReadSettingInt("ArmDepth", 3);
            maxChildrenTrackBar.Value = Helpers.ReadSettingInt("MaxChildren", 3);
            armLengthTrackBar.Value = Helpers.ReadSettingInt("BaseDistance", 25);
            manCountTrackBar.Value = Helpers.ReadSettingInt("Count", 1);

            armDepthTextBox.DataBindings.Add("Text", armDepthTrackBar, "Value");
            maxChildrenTextBox.DataBindings.Add("Text", maxChildrenTrackBar, "Value");
            armLengthTextBox.DataBindings.Add("Text", armLengthTrackBar, "Value");
            manCountTextBox.DataBindings.Add("Text", manCountTrackBar, "Value");
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Helpers.WriteSettingInt("ArmDepth", armDepthTrackBar.Value);
            Helpers.WriteSettingInt("MaxChildren", maxChildrenTrackBar.Value);
            Helpers.WriteSettingInt("BaseDistance", armLengthTrackBar.Value);
            Helpers.WriteSettingInt("Count", manCountTrackBar.Value);

            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
