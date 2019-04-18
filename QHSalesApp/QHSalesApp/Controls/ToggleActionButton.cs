﻿using System;
using Xamarin.Forms;

namespace QHSalesApp
{
    public class ToggleActionButton : ActionButton
    {
        #region Private Members

        /// <summary>
        /// The org icon.
        /// </summary>
        private string _orgIcon;

        /// <summary>
        /// The in toggle.
        /// </summary>
        private bool _inToggle;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="NControl.Controls.ToggleActionButton"/> class.
        /// </summary>
        public ToggleActionButton()
        {

        }

        #region Properties

        /// <summary>
        /// The toggled property.
        /// </summary>
        public static BindableProperty IsToggledProperty =
            BindableProperty.Create<ToggleActionButton, bool>(p => p.IsToggled, false,
                BindingMode.Default, null, (bindable, oldValue, newValue) => {
                    var ctrl = (ToggleActionButton)bindable;
                    ctrl.IsToggled = newValue;
                });

        /// <summary>
        /// Gets or sets the Toggled state of the buton.
        /// </summary>
        /// <value>The color of the buton.</value>
        public bool IsToggled
        {
            get { return (bool)GetValue(IsToggledProperty); }
            set
            {

                if (_inToggle)
                    return;

                _inToggle = true;

                SetValue(IsToggledProperty, value);

                //Task.Run (() => 

                //    Device.BeginInvokeOnMainThread(async () => {
                System.Diagnostics.Debug.WriteLine("Toggle " + value);

                if (!value)
                {
                    ButtonIcon = _orgIcon;
                    ButtonIconLabel.RotateTo(0, 140, Easing.CubicInOut);
                }
                else
                {
                    _orgIcon = ButtonIcon;
                    ButtonIcon = AwasomeIcon.FAPlus;
                    ButtonIconLabel.RotateTo(585, 140, Easing.CubicInOut);
                }

                _inToggle = false;
                //    })
                //);								
            }
        }

        #endregion

        #region Touches

        /// <summary>
        /// Toucheses the began.
        /// </summary>
        /// <param name="points">Points.</param>
        public override bool TouchesBegan(System.Collections.Generic.IEnumerable<NGraphics.Point> points)
        {
            base.TouchesBegan(points);

            System.Diagnostics.Debug.WriteLine("TouchesBegan");

            if (!IsEnabled)
                return false;

            IsToggled = !IsToggled;

            return true;
        }

        #endregion
    }
}
