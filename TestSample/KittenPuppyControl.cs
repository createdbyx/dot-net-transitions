using System;
using System.Windows.Forms;
using Transitions;

namespace TestSample
{
    /// <summary>
    /// This is a simple user-control that hosts two picture-boxes (one showing
    /// a kitten and the other showing a puppy). The transitionPictures method
    /// performs a random animated transition between the two pictures.
    /// </summary>
    public partial class KittenPuppyControl : UserControl
    {
        #region Public methods

        /// <summary>
        /// Constructor.
        /// </summary>
        public KittenPuppyControl()
        {
            this.InitializeComponent();
            this.m_ActivePicture = this.ctrlPuppy;
            this.m_InactivePicture = this.ctrlKitten;
        }

        /// <summary>
        /// Performs a random tarnsition between the two pictures.
        /// </summary>
        public void transitionPictures()
        {
            // We randomly choose where the current image is going to 
            // slide off to (and where we are going to slide the inactive
            // image in from)...
            var iDestinationLeft = (this.m_Random.Next(2) == 0) ? this.Width : -this.Width;
            var iDestinationTop = (this.m_Random.Next(3) - 1) * this.Height;

            // We move the inactive image to this location...
            this.SuspendLayout();
            this.m_InactivePicture.Top = iDestinationTop;
            this.m_InactivePicture.Left = iDestinationLeft;
            this.m_InactivePicture.BringToFront();
            this.ResumeLayout();

            // We perform the transition which moves the active image off the
            // screen, and the inactive one onto the screen...
            var t = new Transition(new TransitionType_EaseInEaseOut(1000));
            t.add(this.m_InactivePicture, "Left", 0);
            t.add(this.m_InactivePicture, "Top", 0);
            t.add(this.m_ActivePicture, "Left", iDestinationLeft);
            t.add(this.m_ActivePicture, "Top", iDestinationTop);
            t.Run();

            // We swap over which image is active and inactive for next time
            // the function is called...
            var tmp = this.m_ActivePicture;
            this.m_ActivePicture = this.m_InactivePicture;
            this.m_InactivePicture = tmp;
        }

        #endregion

        #region Private data

        private PictureBox m_ActivePicture = null;
        private PictureBox m_InactivePicture = null;
        private Random m_Random = new Random();

        #endregion
    }
}
