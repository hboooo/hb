﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace hb.wpf.Animations
{
    /// <summary>
    /// author     :habo
    /// date       :2022/4/17 2:03:31
    /// description:
    /// </summary>
    public class GridLengthAnimation : AnimationTimeline
    {
        static GridLengthAnimation()
        {
            FromProperty = DependencyProperty.Register("From", typeof(GridLength),
                typeof(GridLengthAnimation));

            ToProperty = DependencyProperty.Register("To", typeof(GridLength),
                typeof(GridLengthAnimation));

            EasingFunctionProperty = DependencyProperty.Register("EasingFunction", typeof(IEasingFunction),
                typeof(GridLengthAnimation));
        }

        public override Type TargetPropertyType
        {
            get
            {
                return typeof(GridLength);
            }
        }

        protected override System.Windows.Freezable CreateInstanceCore()
        {
            return new GridLengthAnimation();
        }

        public static readonly DependencyProperty FromProperty;
        public GridLength From
        {
            get
            {
                return (GridLength)GetValue(GridLengthAnimation.FromProperty);
            }
            set
            {
                SetValue(GridLengthAnimation.FromProperty, value);
            }
        }

        public static readonly DependencyProperty ToProperty;
        public GridLength To
        {
            get
            {
                return (GridLength)GetValue(GridLengthAnimation.ToProperty);
            }
            set
            {
                SetValue(GridLengthAnimation.ToProperty, value);
            }
        }

        public static readonly DependencyProperty EasingFunctionProperty;
        public IEasingFunction EasingFunction
        {
            get
            {
                return (IEasingFunction)GetValue(GridLengthAnimation.EasingFunctionProperty);
            }
            set
            {
                SetValue(GridLengthAnimation.EasingFunctionProperty, value);
            }

        }

        public override object GetCurrentValue(object defaultOriginValue,
            object defaultDestinationValue, AnimationClock animationClock)
        {
            double fromValue = ((GridLength)GetValue(GridLengthAnimation.FromProperty)).Value;
            double toValue = ((GridLength)GetValue(GridLengthAnimation.ToProperty)).Value;

            IEasingFunction easingFunction = this.EasingFunction;
            double progress = (easingFunction != null) ? easingFunction.Ease(animationClock.CurrentProgress.Value) : animationClock.CurrentProgress.Value;

            if (fromValue > toValue)
            {
                return new GridLength((1 - progress) * (fromValue - toValue) + toValue, this.To.IsStar ? GridUnitType.Star : GridUnitType.Pixel);
            }
            else
            {
                return new GridLength((progress) * (toValue - fromValue) + fromValue, this.To.IsStar ? GridUnitType.Star : GridUnitType.Pixel);
            }
        }
    }
}
