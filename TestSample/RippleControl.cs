using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Transitions;

namespace TestSample
{
    /// <summary>
    /// This control shows a color-rippling effect when you call its ripple()
    /// method. It holds a grid of 10x10 label controls with an initial white 
    /// background color. The ripple method uses a separate transition on each
    /// label to move it to pink and back again.
    /// 
    /// The code to set up the labels (in the Load method) is a bit complicated, 
    /// but the ripple transition itself is very simple.
    /// </summary>
    public partial class RippleControl : UserControl
    {
        #region Public methods

        /// <summary>
        /// Constructor.
        /// </summary>
        public RippleControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Starts the ripple effect.
        /// </summary>
        public void ripple()
        {
            // We run a transition on each of the labels shown on the control.
            // This means that we will be running 100 simulataneous transitions...
            foreach (var info in this.m_CellInfos)
            {
                Transition.Run(info.Control, "BackColor", Color.Pink, new TransitionType_Flash(1, info.TransitionInterval));
            }
        }
        
        #endregion

        #region Private functions

        /// <summary>
        /// Called when the control is first loaded.
        /// </summary>
        private void RippleControl_Load(object sender, EventArgs e)
        {
            var dCellWidth = this.Width / 10.0;
            var dCellHeight = this.Height / 10.0;

            // We set up a 10x10 grid of labels...
            double dTop = 0;
            for (var iRow = 0; iRow <10; ++iRow)
            {
                double dLeft = 0;
                var dBottom = dTop + dCellHeight;
                
                for (var iCol = 0; iCol < 10; ++iCol)
                {
                    // We work out the size of this label...
                    var dRight = dLeft + dCellWidth;
                    var iLeft = (int)dLeft;
                    var iTop = (int)dTop;
                    var iRight = (int)dRight;
                    var iBottom = (int)dBottom;
                    var iWidth = iRight - iLeft;
                    var iHeight = iBottom - iTop;

                    // We create the label...
                    var label = new Label();
                    label.Left = iLeft;
                    label.Top = iTop;
                    label.Width = iWidth;
                    label.Height = iHeight;
                    label.BackColor = Color.White;

                    // And add it to the control...
                    this.Controls.Add(label);

                    // We work out a transition time for it, and store the information
                    // to use when we do the ripple effect...
                    var iTransitionInterval = iRow * 100 + iCol * 100;
                    this.m_CellInfos.Add(new CellInfo { Control = label, TransitionInterval = iTransitionInterval });

                    // The left for the next column is the right for this one...
                    dLeft = dRight;
                }

                // The top of the next row is the bottom of this one...
                dTop = dBottom;
            }
        }

        #endregion

        #region Private data

        // A small class that holds information about one of the labels on the control.
        private class CellInfo
        {
            public Control Control { get; set; }
            public int TransitionInterval { get; set; }
        }

        // A collection of cell-infos, i.e. info on each label on the control...
        private IList<CellInfo> m_CellInfos = new List<CellInfo>();

        #endregion
    }
}
