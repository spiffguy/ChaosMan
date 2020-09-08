using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace ChaosMan
{
    public partial class ChaosManForm : Form
    {
        List<ChaosArm> m_arms = new List<ChaosArm>();
        Timer m_animate = new Timer();
        Timer m_restart = new Timer();
        int m_armDepth;
        int m_maxChildren;
        int m_armLength;
        int m_manCount;
        int m_lineWidth;
        int m_circleSize;
        private IntPtr m_parentWindowHandle;    // Handle to preview window, if applicable
        private Point m_mouseLocation;          // Keep track of the location of the mouse

        public ChaosManForm(int parentWindowHandle)
        {
            InitializeComponent();

            m_parentWindowHandle = new IntPtr(parentWindowHandle);

            if (m_parentWindowHandle == IntPtr.Zero)
            {
                
                Left = ScreenArea.LeftMostBound;
                Top = ScreenArea.TopMostBound;
                Width = ScreenArea.TotalWidth;
                Height = ScreenArea.TotalHeight;

                Capture = true;
                m_mouseLocation = Control.MousePosition;
                Cursor.Hide();

                m_lineWidth = 3;
                m_circleSize = 21;

                m_armLength = Helpers.ReadSettingInt("BaseDistance", 25);
            }
            else
            {
                // Screen saver is in the preview window, set the new parent window
                WinAPI.SetParent(Handle, m_parentWindowHandle);

                // Make this a child window
                WinAPI.SetWindowLong(Handle, WinAPI.GWL_STYLE,
                    WinAPI.GetWindowLong(Handle, WinAPI.GWL_STYLE) | WinAPI.WS_CHILD);

                // Get the parent window size
                Rectangle rect;
                WinAPI.GetClientRect(m_parentWindowHandle, out rect);
                Size = rect.Size;
                Location = new Point(0, 0);

                m_lineWidth = 1;
                m_circleSize = 5;
                m_armLength = 6;
            }

            m_armDepth = Helpers.ReadSettingInt("ArmDepth", 3);
            m_maxChildren = Helpers.ReadSettingInt("MaxChildren", 3);
            m_manCount = Helpers.ReadSettingInt("Count", 1);

            InitArms();
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            m_restart.Tick += new EventHandler(m_restart_Tick);
            m_restart.Interval = 300000;
            m_restart.Start();

            m_animate.Tick += new EventHandler(m_animate_Tick);
            m_animate.Interval = 16;
            m_animate.Start();
        }

        void m_restart_Tick(object sender, EventArgs e)
        {
            InitArms();
        }

        void m_animate_Tick(object sender, EventArgs e)
        {
            foreach (ChaosArm arm in m_arms)
            {
                arm.Animate();
            }
            Invalidate();
        }

        private void ChaosManForm_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.Clear(Color.Black);

            foreach (ChaosArm arm in m_arms)
            {
                arm.DrawArm(e.Graphics);
            }
        }

        private void InitArms()
        {
            List<Point> anchors = new List<Point>();
            m_arms.Clear();

            switch (m_manCount)
            {
                case 1:
                    {
                        anchors.Add(
                            new Point(
                                (Width / 2),
                                (Height / 2)));
                    }
                    break;
                case 2:
                    {
                        anchors.Add(
                            new Point(
                                (Width / 4),
                                (Height / 2)));
                        anchors.Add(
                            new Point(
                                (Width / 4) + (Width / 2),
                                (Height / 2)));
                    }
                    break;
                case 3:
                    {
                        anchors.Add(
                            new Point(
                                (Width / 4),
                                (Height / 2)));
                        anchors.Add(
                            new Point(
                                (Width / 2),
                                (Height / 2)));
                        anchors.Add(
                            new Point(
                                (Width / 4) + (Width / 2),
                                (Height / 2)));
                    }
                    break;
                case 4:
                    {
                        anchors.Add(
                            new Point(
                                (Width / 4),
                                (Height / 4)));
                        anchors.Add(
                            new Point(
                                (Width / 4) + (Width / 2),
                                (Height / 4)));
                        anchors.Add(
                            new Point(
                                (Width / 4),
                                (Height / 4) + (Height / 2)));
                        anchors.Add(
                            new Point(
                                (Width / 4) + (Width / 2),
                                (Height / 4) + (Height / 2)));
                    }
                    break;
                case 5:
                    {
                        anchors.Add(
                            new Point(
                                (Width / 4),
                                (Height / 4)));
                        anchors.Add(
                            new Point(
                                (Width / 4) + (Width / 2),
                                (Height / 4)));
                        anchors.Add(
                            new Point(
                                (Width / 2),
                                (Height / 2)));
                        anchors.Add(
                            new Point(
                                (Width / 4),
                                (Height / 4) + (Height / 2)));
                        anchors.Add(
                            new Point(
                                (Width / 4) + (Width / 2),
                                (Height / 4) + (Height / 2)));
                    }
                    break;
            }

            foreach (Point anchor in anchors)
            {
                m_arms.Add(
                    new ChaosArm(
                        anchor,
                        m_armDepth,
                        m_maxChildren,
                        m_armLength,
                        m_lineWidth,
                        m_circleSize));
            }
        }

        protected bool IsPreviewMode
        {
            get
            {
                return m_parentWindowHandle != IntPtr.Zero;
            }
        }

        private void ChaosManForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (!IsPreviewMode)
            {
                Close();
            }
        }

        private void ChaosManForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (Math.Abs(MousePosition.X - m_mouseLocation.X) > 5 || Math.Abs(MousePosition.Y - m_mouseLocation.Y) > 5)
            {
                if (!IsPreviewMode)
                {
                    Close();
                }
            }
        }

        private void ChaosManForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (!IsPreviewMode)
            {
                Close();
            }
        }

        private void ChaosManForm_Deactivate(object sender, EventArgs e)
        {
            Activate();
        }
    }
}