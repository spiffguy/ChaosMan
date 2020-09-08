using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ChaosMan
{
    public partial class WindowedForm : Form
    {
        private readonly List<ChaosArm> mArms = new List<ChaosArm>();
        private readonly Timer mAnimate = new Timer();
        private readonly Timer mRestart = new Timer();
        private  int mArmDepth;
        private  int mMaxChildren;
        private  int mArmLength;
        private  int mManCount;
        private readonly int mLineWidth;
        private readonly int mCircleSize;

        public WindowedForm()
        {
            this.InitializeComponent();

            this.mLineWidth = 3;
            this.mCircleSize = 21;

            this.InitArms();
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            this.mRestart.Tick += this.m_restart_Tick;
            this.mRestart.Interval = 300000;
            this.mRestart.Start();

            this.mAnimate.Tick += this.m_animate_Tick;
            this.mAnimate.Interval = 16;
            this.mAnimate.Start();
        }

        private void ChaosManForm_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.Clear(Color.Black);

            foreach (var arm in this.mArms)
            {
                arm.DrawArm(e.Graphics);
            }
        }

        private void m_restart_Tick(object sender, EventArgs e)
        {
            this.InitArms();
        }

        private void m_animate_Tick(object sender, EventArgs e)
        {
            foreach (ChaosArm arm in this.mArms)
            {
                arm.Animate();
            }
            
            this.ViewPort.Invalidate();
        }

        private void InitArms()
        {
            this.mArmLength = Helpers.ReadSettingInt("BaseDistance", 25);
            this.mArmDepth = Helpers.ReadSettingInt("ArmDepth", 3);
            this.mMaxChildren = Helpers.ReadSettingInt("MaxChildren", 3);
            this.mManCount = Helpers.ReadSettingInt("Count", 1);

            List<Point> anchors = new List<Point>();
            this.mArms.Clear();

            switch (this.mManCount)
            {
                case 1:
                    {
                        anchors.Add(
                            new Point(
                                (this.Width / 2),
                                (this.Height / 2)));
                    }
                    break;
                case 2:
                    {
                        anchors.Add(
                            new Point(
                                (this.Width / 4),
                                (this.Height / 2)));
                        anchors.Add(
                            new Point(
                                (this.Width / 4) + (this.Width / 2),
                                (this.Height / 2)));
                    }
                    break;
                case 3:
                    {
                        anchors.Add(
                            new Point(
                                (this.Width / 4),
                                (this.Height / 2)));
                        anchors.Add(
                            new Point(
                                (this.Width / 2),
                                (this.Height / 2)));
                        anchors.Add(
                            new Point(
                                (this.Width / 4) + (this.Width / 2),
                                (this.Height / 2)));
                    }
                    break;
                case 4:
                    {
                        anchors.Add(
                            new Point(
                                (this.Width / 4),
                                (this.Height / 4)));
                        anchors.Add(
                            new Point(
                                (this.Width / 4) + (this.Width / 2),
                                (this.Height / 4)));
                        anchors.Add(
                            new Point(
                                (this.Width / 4),
                                (this.Height / 4) + (this.Height / 2)));
                        anchors.Add(
                            new Point(
                                (this.Width / 4) + (this.Width / 2),
                                (this.Height / 4) + (this.Height / 2)));
                    }
                    break;
                case 5:
                    {
                        anchors.Add(
                            new Point(
                                (this.Width / 4),
                                (this.Height / 4)));
                        anchors.Add(
                            new Point(
                                (this.Width / 4) + (this.Width / 2),
                                (this.Height / 4)));
                        anchors.Add(
                            new Point(
                                (this.Width / 2),
                                (this.Height / 2)));
                        anchors.Add(
                            new Point(
                                (this.Width / 4),
                                (this.Height / 4) + (this.Height / 2)));
                        anchors.Add(
                            new Point(
                                (this.Width / 4) + (this.Width / 2),
                                (this.Height / 4) + (this.Height / 2)));
                    }
                    break;
            }

            foreach (Point anchor in anchors)
            {
                this.mArms.Add(
                    new ChaosArm(
                        anchor, this.mArmDepth, this.mMaxChildren, this.mArmLength, this.mLineWidth, this.mCircleSize));
            }
        }

        private void OptionsMenuItem_Click(object sender, EventArgs e)
        {
            var optionsForm = new OptionsForm();

            optionsForm.ShowDialog(this);
        }

        private void ResetMenuItem_Click(object sender, EventArgs e)
        {
            this.InitArms();
        }
    }
}
