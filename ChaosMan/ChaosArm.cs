using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ChaosMan
{
    internal class ChaosArm
    {
        private static readonly Random Rnd = new Random();
        private readonly int mArmLength;
        private Point mAnchor;
        private Point mPos;
        private readonly List<ChaosArm> mChildren = new List<ChaosArm>();
        private double mAngle;
        private double mRadius;
        private double mDirection;
        private Brush mBrush;
        private Pen mPen;
        private Point mCenterOfBalance;
        private const double MDivider = 2000.0F;
        private readonly int mLineWidth;
        private readonly int mCircleSize;

        private const double Tolerance = 0.0001;


        public ChaosArm(Point origin, int armDepth, int maxChildren, int armLength, int lineWidth, int circleSize)
        {
            this.mArmLength = armLength;
            this.mAnchor = origin;
            this.mPos = origin;
            this.mAngle = 0;
            this.mRadius = Math.PI / 2;
            this.mLineWidth = lineWidth;
            this.mCircleSize = circleSize;

            var color = Color.FromArgb((int)((UInt32)Rnd.Next(0xffffff) | 0xff000000));

            this.mBrush = new SolidBrush(color);
            this.mPen = new Pen(color, this.mLineWidth);

            if (armDepth <= 0)
            {
                return;
            }

            int index;
            var count = Rnd.Next(1, maxChildren + 1);

            for (index = 0; index < count; index++)
            {
                var arm = new ChaosArm(armLength, lineWidth, circleSize);

                arm.CreateChaosArm(this.mPos, armDepth - 1, maxChildren);
                this.mChildren.Add(arm);
            }
        }

        public ChaosArm(int baseDistance, int lineWidth, int circleSize)
        {
            this.mArmLength = baseDistance;
            this.mLineWidth = lineWidth;
            this.mCircleSize = circleSize;
        }

        private void CreateChaosArm(Point origin, int armDepth, int maxChildren)
        {
            this.mAnchor = origin;
            this.mAngle = (double)(Rnd.Next(1000)) / 500 * Math.PI;
            this.mRadius = this.mArmLength * (double)(armDepth + 1);

            this.UpdatePosition();

            var color = Color.FromArgb((int)((UInt32)Rnd.Next(0xffffff) | 0xff000000));

            this.mBrush = new SolidBrush(color);
            this.mPen = new Pen(color, this.mLineWidth);

            if (armDepth <= 0)
            {
                return;
            }

            int index;
            var count = Rnd.Next(1, maxChildren + 1);

            for (index = 0; index < count; index++)
            {
                var arm = new ChaosArm(this.mArmLength, this.mLineWidth, this.mCircleSize);

                arm.CreateChaosArm(this.mPos, armDepth - 1, maxChildren);
                this.mChildren.Add(arm);
            }
        }

        private void CalculateVectorAngles(double parentDirection, double parentAngle)
        {
            double dir;
            const double div = Math.PI / MDivider;

            CartesianToPolar(this.mAnchor, this.mCenterOfBalance, out var angle, out _);

            if (Math.Abs(angle - Math.PI / 2) < Tolerance)
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

            if (this.mDirection * dir < 0)
            {
                this.mDirection += dir * div * 1.65F;
            }
            else
            {
                this.mDirection += dir * div;
            }

            if (Math.Abs(parentDirection) > Tolerance)
            {
                double forceAngle;
                double forceDir = 0;

                if (parentDirection > 0)
                {
                    forceAngle = parentAngle + (Math.PI / 2);

                    if (forceAngle > 2 * Math.PI)
                    {
                        forceAngle += 2 * Math.PI;
                    }
                }
                else
                {
                    forceAngle = parentAngle - (Math.PI / 2);

                    if (forceAngle < 0)
                    {
                        forceAngle -= 2 * Math.PI;
                    }
                }

                var diffAngle = Math.Abs(angle - forceAngle);

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

                this.mDirection += forceDir;
            }

            foreach (var arm in this.mChildren)
            {
                arm.CalculateVectorAngles(this.mDirection, this.mAngle);
            }
        }

        private void UpdatePosition()
        {
            this.UpdatePosition(this.mAnchor);
        }

        private void UpdatePosition(Point parent)
        {
            this.mAnchor = parent;

            this.mAngle += this.mDirection;

            if (this.mAngle < 0)
            {
                this.mAngle += 2 * Math.PI;
            }
            else if (this.mAngle > (2 * Math.PI))
            {
                this.mAngle -= 2 * Math.PI;
            }

            PolarToCartesian(this.mAnchor, out this.mPos, this.mAngle, this.mRadius);

            foreach (var arm in this.mChildren)
            {
                arm.UpdatePosition(this.mPos);
            }
        }

        public void DrawArm(Graphics gfx)
        {
            gfx.DrawLine(this.mPen, this.mAnchor, this.mPos);
            gfx.FillEllipse(this.mBrush, this.mPos.X - (this.mCircleSize / 2), this.mPos.Y - (this.mCircleSize / 2), this.mCircleSize, this.mCircleSize);

            foreach (var arm in this.mChildren)
            {
                arm.DrawArm(gfx);
            }
        }

        public void Animate()
        {
            this.GetCenterOfBalance();
            this.CalculateVectorAngles(0, 0);
            this.UpdatePosition();
        }

        private Point GetCenterOfBalance()
        {
            if (this.mChildren.Count > 0)
            {
                var cob = new Point(0, 0);

                foreach (var centerOfBalance in this.mChildren.Select(arm => arm.GetCenterOfBalance()))
                {
                    cob.X += centerOfBalance.X;
                    cob.Y += centerOfBalance.Y;
                }

                cob.X /= this.mChildren.Count;
                cob.Y /= this.mChildren.Count;

                foreach (var arm in this.mChildren)
                {
                    arm.mCenterOfBalance.X = cob.X;
                    arm.mCenterOfBalance.Y = cob.Y;
                }

                this.mCenterOfBalance.X = ((2 * this.mPos.X) + cob.X) / 3;
                this.mCenterOfBalance.Y = ((2 * this.mPos.Y) + cob.Y) / 3;
            }
            else
            {
                this.mCenterOfBalance = this.mPos;
            }

            return this.mCenterOfBalance;
        }

        public static void PolarToCartesian(Point origin, out Point p, double angle, double radius)
        {
            var x = (int)(origin.X + (radius * Math.Cos(angle)));
            var y = (int)(origin.Y + (radius * Math.Sin(angle)));

            p = new Point(x, y);
        }

        public static void CartesianToPolar(Point origin, Point p, out double angle, out double radius)
        {
            var x = p.X - origin.X;
            var y = p.Y - origin.Y;
            radius = Math.Sqrt((x * x) + (y * y));


            if (x > 0)
            {
                if (y < 0)
                {
                    angle = Math.Atan(y / (double)x) + (2 * Math.PI);
                    return;
                }

                angle = Math.Atan(y / (double)x);
                return;
            }

            if (x < 0)
            {
                angle = Math.Atan(y / (double)x) + Math.PI;
                return;
            }

            if (y > 0)
            {
                angle = Math.PI / 2;
                return;
            }
            else if (y < 0)
            {
                angle = 3 * Math.PI / 2;
                return;
            }

            angle = 0;

        }
    }
}
