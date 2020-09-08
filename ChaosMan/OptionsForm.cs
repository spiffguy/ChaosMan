using System;
using System.Windows.Forms;

namespace ChaosMan
{
    public partial class OptionsForm : Form
    {
        //int m_lineWidth;
        //int m_circleSize;

        public OptionsForm()
        {
            this.InitializeComponent();

            this.armDepthTrackBar.Value = Helpers.ReadSettingInt("ArmDepth", 3);
            this.maxChildrenTrackBar.Value = Helpers.ReadSettingInt("MaxChildren", 3);
            this.armLengthTrackBar.Value = Helpers.ReadSettingInt("BaseDistance", 25);
            this.manCountTrackBar.Value = Helpers.ReadSettingInt("Count", 1);

            this.armDepthTextBox.DataBindings.Add("Text", this.armDepthTrackBar, "Value");
            this.maxChildrenTextBox.DataBindings.Add("Text", this.maxChildrenTrackBar, "Value");
            this.armLengthTextBox.DataBindings.Add("Text", this.armLengthTrackBar, "Value");
            this.manCountTextBox.DataBindings.Add("Text", this.manCountTrackBar, "Value");
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Helpers.WriteSettingInt("ArmDepth", this.armDepthTrackBar.Value);
            Helpers.WriteSettingInt("MaxChildren", this.maxChildrenTrackBar.Value);
            Helpers.WriteSettingInt("BaseDistance", this.armLengthTrackBar.Value);
            Helpers.WriteSettingInt("Count", this.manCountTrackBar.Value);

            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
