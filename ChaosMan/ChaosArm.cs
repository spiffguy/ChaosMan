using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace ChaosMan
{
    class ChaosArm
    {
        private static Random s_rnd = new Random();
        private  int m_armLength;
        private Point m_anchor;
        private Point m_pos;
        private List<ChaosArm> m_children = new List<ChaosArm>();
        private double m_angle;
        private double m_radius;
        private double m_direction = 0;
        private Brush m_brush;
        private Pen m_pen;
        private Point m_centerOfBalance;
        private const double m_divider = 2000.0F;
        private int m_lineWidth;
        private int m_circleSize;

        public ChaosArm(Point origin, int armdepth, int maxchildren, int armlength, int linewidth, int circlesize)
        {
            m_armLength = armlength;
            m_anchor = origin;
            m_pos = origin;
            m_angle = 0;
            m_radius = Math.PI / 2;
            m_lineWidth = linewidth;
            m_circleSize = circlesize;

            Color color = Color.FromArgb((int)((UInt32)s_rnd.Next(0xffffff) | 0xff000000));

            m_brush = new SolidBrush(color);
            m_pen = new Pen(color, m_lineWidth);

            if(armdepth > 0)
            {
                int index;
                int count = s_rnd.Next(1, maxchildren + 1);

                for (index = 0; index < count; index++)
                {
                    ChaosArm arm = new ChaosArm(armlength, linewidth, circlesize);

                    arm.CreateChaosArm(m_pos, armdepth - 1, maxchildren);
                    m_children.Add(arm);
                }
            }
        }

        public ChaosArm(int basedistance, int linewidth, int circlesize)
        {
            m_armLength = basedistance;
            m_lineWidth = linewidth;
            m_circleSize = circlesize;
        }

        private void CreateChaosArm(Point origin, int armdepth, int maxchildren)
        {
            m_anchor = origin;
            m_angle = (double)(s_rnd.Next(1000)) / 500 * Math.PI;
            m_radius = m_armLength * (double)(armdepth + 1);

            UpdatePosition();

            Color color = Color.FromArgb((int)((UInt32)s_rnd.Next(0xffffff) | 0xff000000));

            m_brush = new SolidBrush(color);
            m_pen = new Pen(color, m_lineWidth);

            if (armdepth > 0)
            {
                int index;
                int count = s_rnd.Next(1, maxchildren + 1);

                for (index = 0; index < count; index++)
                {
                    ChaosArm arm = new ChaosArm(m_armLength, m_lineWidth, m_circleSize);

                    arm.CreateChaosArm(m_pos, armdepth - 1, maxchildren);
                    m_children.Add(arm);
                }
            }
        }

        private void CalculateVectorAngles(double parentdir, double parentangle)
        {
            double dir;
            const double div = Math.PI / m_divider;
            double angle = 0;
            double radius = 0;

            CartesianToPolar(m_anchor, m_centerOfBalance, out angle, out radius);

            if (angle == Math.PI / 2)
            {
                dir = 0;
            }
            else if (angle < Math.PI / 2)
            {
                dir = 1;
            }
            else if (angle > (3 * Math.PI / 2))
            {
                dir = 1;
            }
            else if (angle < Math.PI)
            {
                dir = -1;
            }
            else
            {
                dir = -1;
            }

            if (m_direction * dir < 0)
            {
                m_direction += dir * div * 1.65F;
            }
            else
            {
                m_direction += dir * div;
            }

            if (parentdir != 0)
            {
                double forceAngle;
                double diffAngle;
                double forceDir = 0;

                if (parentdir > 0)
                {
                    forceAngle = parentangle + (Math.PI / 2);

                    if (forceAngle > 2 * Math.PI)
                    {
                        forceAngle += 2 * Math.PI;
                    }
                }
                else
                {
                    forceAngle = parentangle - (Math.PI / 2);

                    if (forceAngle < 0)
                    {
                        forceAngle -= 2 * Math.PI;
                    }
                }

                diffAngle = Math.Abs(angle - forceAngle);

                if (diffAngle > Math.PI)
                {
                    if (forceAngle > angle)
                    {
                        forceDir = div;
                    }
                    else if (forceDir < angle)
                    {
                        forceDir = -div;
                    }
                }
                else if (diffAngle < Math.PI)
                {
                    if (forceAngle > angle)
                    {
                        forceDir = -div;
                    }
                    else if (forceDir < angle)
                    {
                        forceDir = div;
                    }
                }

                m_direction += forceDir;
            }

            foreach (ChaosArm arm in m_children)
            {
                arm.CalculateVectorAngles(m_direction, m_angle);
            }
        }

        private void UpdatePosition()
        {
            UpdatePosition(m_anchor);
        }

        private void UpdatePosition(Point parent)
        {
            m_anchor = parent;

            m_angle += m_direction;

            if (m_angle < 0)
            {
                m_angle += 2 * Math.PI;
            }
            else if (m_angle > (2 * Math.PI))
            {
                m_angle -= 2 * Math.PI;
            }

            PolarToCartesian(m_anchor, out m_pos, m_angle, m_radius);

            foreach (ChaosArm arm in m_children)
            {
                arm.UpdatePosition(m_pos);
            }
        }

        public void DrawArm(Graphics gfx)
        {
            gfx.DrawLine(m_pen, m_anchor, m_pos);
            gfx.FillEllipse(m_brush, m_pos.X - (m_circleSize / 2), m_pos.Y - (m_circleSize / 2), m_circleSize, m_circleSize);

            foreach (ChaosArm arm in m_children)
            {
                arm.DrawArm(gfx);
            }
        }

        public void Animate()
        {
            GetCenterOfBalance();
            CalculateVectorAngles(0, 0);
            UpdatePosition();
        }

        private Point GetCenterOfBalance()
        {
            if (m_children.Count > 0)
            {
                Point cob = new Point(0, 0);

                foreach (ChaosArm arm in m_children)
                {
                    Point ccob = arm.GetCenterOfBalance();
                    cob.X += ccob.X;
                    cob.Y += ccob.Y;
                }

                cob.X = cob.X / m_children.Count;
                cob.Y = cob.Y / m_children.Count;

                foreach (ChaosArm arm in m_children)
                {
                    arm.m_centerOfBalance.X = cob.X;
                    arm.m_centerOfBalance.Y = cob.Y;
                }

                m_centerOfBalance.X = ((2 * m_pos.X) + cob.X) / 3;
                m_centerOfBalance.Y = ((2 * m_pos.Y) + cob.Y) / 3;
            }
            else
            {
                m_centerOfBalance = m_pos;
            }

            return m_centerOfBalance;
        }

        static void PolarToCartesian(Point origin, out Point p, double angle, double radius)
        {
            int x = (int)(origin.X + (radius * Math.Cos(angle)));
            int y = (int)(origin.Y + (radius * Math.Sin(angle)));

            p = new Point(x, y);
        }

        static void CartesianToPolar(Point origin, Point p, out double angle, out double radius)
        {
            int x = p.X - origin.X;
            int y = p.Y - origin.Y;
            radius = Math.Sqrt((x * x) + (y * y));


            if (x > 0)
            {
                if (y < 0)
                {
                    angle = Math.Atan(y / x) + (2 * Math.PI);
                    return;
                }
                angle = Math.Atan(y / x);
                return;
            }
            if (x < 0)
            {
                angle = Math.Atan(y / x) + Math.PI;
                return;
            }
            if (y > 0) { angle = Math.PI / 2; return; }
            else if (y < 0) { angle = 3 * Math.PI / 2; return; }
            angle = 0;

        }
    }
}
