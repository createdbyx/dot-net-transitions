using System;
using System.Windows.Forms;
using Transitions;
using System.Drawing;
using System.Collections.Generic;

namespace TestSample
{
    /// <summary>
    /// This form demonstrates a number of animated transitions using the Transitions
    /// library. Each event handler (form-load, button click) demonstrates a different
    /// transition.
    /// </summary>
    public partial class Form1 : Form
    {
        #region Public methods

        /// <summary>
        /// Constructor.
        /// </summary>
        public Form1()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Form event handlers

        /// <summary>
        /// Called when the "Swap" button is pressed.
        /// </summary>
        private void cmdSwap_Click(object sender, EventArgs e)
        {
            // We swap over the group-boxes that show the "Bounce" and 
            // "Throw and Catch" transitions. The active one is animated 
            // left off the screen and the inactive one is animated right
            // onto the screen...

            // We work out which box is currently on screen and
            // which is off screen...
            Control ctrlOnScreen, ctrlOffScreen;
            if (this.gbBounce.Left == GROUP_BOX_LEFT)
            {
                ctrlOnScreen = this.gbBounce;
                ctrlOffScreen = this.gbThrowAndCatch;
            }
            else
            {
                ctrlOnScreen = this.gbThrowAndCatch;
                ctrlOffScreen = this.gbBounce;
            }
            ctrlOnScreen.SendToBack();
            ctrlOffScreen.BringToFront();

            // We create a transition to animate the two boxes simultaneously. One is
            // animated onto the screen, the other off the screen.

            // The ease-in-ease-out transition acclerates the rate of change for the 
            // first half of the animation, and decelerates during the second half.

            var t = new Transition(new TransitionType_EaseInEaseOut(1000));
            t.add(ctrlOnScreen, "Left", -1 * ctrlOnScreen.Width);
            t.add(ctrlOffScreen, "Left", GROUP_BOX_LEFT);
            t.Run();
        }

        /// <summary>
        /// Called when the "Bounce Me!" button is pressed.
        /// </summary>
        private void cmdBounceMe_Click(object sender, EventArgs e)
        {
            // We bounce the button down to the bottom of the group box it is in, and 
            // back up again.

            // The Bounce transition accelerates the property to its destination value
            // (as if with gravity) and decelerates it back  to its original value (as
            // if against gravity).

            var iDestination = this.gbBounce.Height - this.cmdBounceMe.Height;
            Transition.Run(this.cmdBounceMe, "Top", iDestination, new TransitionType_Bounce(1500));
        }

        /// <summary>
        /// Called when the "Throw and Catch" button is pressed.
        /// </summary>
        private void cmdThrowAndCatch_Click(object sender, EventArgs e)
        {
            // The button is 'thrown' up to the top of the group-box it is in
            // and then falls back down again. 

            // The throw-and-catch transition starts the animation at a high rate and
            // decelerates to zero (as if against gravity) at the destination value. It 
            // then accelerates the value (as if with gravity) back to the original value.

            Transition.Run(this.cmdThrowAndCatch, "Top", 12, new TransitionType_ThrowAndCatch(1500));
        }

        /// <summary>
        /// Called when the "Flash Me!" button is pressed.
        /// </summary>
        private void cmdFlashMe_Click(object sender, EventArgs e)
        {
            // The Flash transition animates the property to the destination value
            // and back again. You specify how many flashes to show and the length
            // of each flash...
            Transition.Run(this.cmdFlashMe, "BackColor", Color.Pink, new TransitionType_Flash(2, 300));
        }

        /// <summary>
        /// Called when the "Ripple" button is pressed.
        /// </summary>
        private void cmdRipple_Click(object sender, EventArgs e)
        {
            // The ripple is handled by the RippleControl user-control.
            // This performs 100 simultaneous animations to create the 
            // ripple effect...
            this.ctrlRipple.ripple();
        }

        /// <summary>
        /// Called when the "Swap Pictures" button is pressed.
        /// </summary>
        private void cmdSwapPictures_Click(object sender, EventArgs e)
        {
            // The transition is handled by the KittenPuppyControl...
            this.ctrlPictures.transitionPictures();
        }

        /// <summary>
        /// Called when the "Text Transition" button is pressed.
        /// </summary>
        private void cmdTextTransition_Click(object sender, EventArgs e)
        {
            // We transition four properties simulataneously here:
            // - The two labels' text is changed.
            // - The two labels' colors are changed.

            // We work out the new text and colors to transition to...
            string strText1, strText2;
            Color color1, color2;
            if (this.lblTextTransition1.Text == STRING_SHORT)
            {
                strText1 = STRING_LONG;
                color1 = Color.Red;
                strText2 = STRING_SHORT;
                color2 = Color.Blue;
            }
            else
            {
                strText1 = STRING_SHORT;
                color1 = Color.Blue;
                strText2 = STRING_LONG;
                color2 = Color.Red;
            }

            // We create a transition to animate all four properties at the same time...
            var t = new Transition(new TransitionType_Linear(1000));
            t.add(this.lblTextTransition1, "Text", strText1);
            t.add(this.lblTextTransition1, "ForeColor", color1);
            t.add(this.lblTextTransition2, "Text", strText2);
            t.add(this.lblTextTransition2, "ForeColor", color2);
            t.Run();
        }

        /// <summary>
        /// Called when the "Change Form Color" button is pressed.
        /// </summary>
        private void ctrlChangeFormColor_Click(object sender, EventArgs e)
        {
            // We alternate the form's background color...
            var destination = (this.BackColor == this.BACKCOLOR_PINK) ? this.BACK_COLOR_YELLOW : this.BACKCOLOR_PINK;
            Transition.Run(this, "BackColor", destination, new TransitionType_Linear(1000));
        }

        /// <summary>
        /// Called when the "More" or "Less" button is pressed.
        /// </summary>
        private void cmdMore_Click(object sender, EventArgs e)
        {
            // We either show more screen or less screen depending on the current state.
            // We find out whether we need to make the screen wider or narrower...
            int iFormWidth;
            if (this.cmdMore.Text == "More >>")
            {
                iFormWidth = 984;
                this.cmdMore.Text = "<< Less";
            }
            else
            {
                iFormWidth = 452;
                this.cmdMore.Text = "More >>";
            }

            // We animate it with an ease-in-ease-out transition...
            Transition.Run(this, "Width", iFormWidth, new TransitionType_EaseInEaseOut(1000));
        }

        /// <summary>
        /// Called when the "Drop and Bounce" button is pressed.
        /// </summary>
        private void cmdDropAndBounce_Click(object sender, EventArgs e)
        {
            // We animate the button to drop and bounce twice with bounces
            // of diminishing heights. While it does this, it is moving to 
            // the right, as if thrown to the right. When this animation has
            // finished, the button moves back to its original position.

            // The diminishing-bounce is not one of the built-in transition types,
            // so we create it here as a user-defined transition type. You define 
            // these as a collection of TransitionElements. These define how far the
            // animated properties will have moved at various times, and how the 
            // transition between different elements is to be done.

            // So in the example below:
            //  0% - 40%    The button acclerates to 100% distance (i.e. the bottom of the screen)
            // 40% - 65%    The button bounces back (decelerating) to 70% distance.
            // etc...

            IList<TransitionElement> elements = new List<TransitionElement>();
            elements.Add(new TransitionElement(40, 100, InterpolationMethod.Accleration));
            elements.Add(new TransitionElement(65, 70, InterpolationMethod.Deceleration));
            elements.Add(new TransitionElement(80, 100, InterpolationMethod.Accleration));
            elements.Add(new TransitionElement(90, 92, InterpolationMethod.Deceleration));
            elements.Add(new TransitionElement(100, 100, InterpolationMethod.Accleration));

            var iDestination = this.gbDropAndBounce.Height - this.cmdDropAndBounce.Height - 10;
            Transition.Run(this.cmdDropAndBounce, "Top", iDestination, new TransitionType_UserDefined(elements, 2000));

            // The transition above just animates the vertical bounce of the button, but not
            // the left-to-right movement. This can't use the same transition, as otherwise the
            // left-to-right movement would bounce back and forth.

            // We run the left-to-right animation as a second, simultaneous transition. 
            // In fact, we run a transition chain, with the animation of the button back
            // to its starting position as the second item in the chain. The second 
            // transition starts as soon as the first is complete...

            var t1 = new Transition(new TransitionType_Linear(2000));
            t1.add(this.cmdDropAndBounce, "Left", this.cmdDropAndBounce.Left + 400);

            var t2 = new Transition(new TransitionType_EaseInEaseOut(2000));
            t2.add(this.cmdDropAndBounce, "Top", 19);
            t2.add(this.cmdDropAndBounce, "Left", 6);

            Transition.runChain(t1, t2);
        }

        #endregion

        #region Private data

        // Colors used by the change-form-color transition...
        private Color BACKCOLOR_PINK = Color.FromArgb(255, 220, 220);
        private Color BACK_COLOR_YELLOW = Color.FromArgb(255, 255, 220);

        // The left point of the 'bounce' and 'throw-and-catch' group boxes...
        private const int GROUP_BOX_LEFT = 12;

        // Strings used for the text transition...
        private const string STRING_SHORT = "Hello, World!";
        private const string STRING_LONG = "A longer piece of text.";

        #endregion

    }
}
