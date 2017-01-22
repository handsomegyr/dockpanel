using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Guoyongrong.WinFormsUI.Docking;

namespace WindowsFormsApplication1
{
    public partial class DummyPropertyWindow : FormContent
    {
        public DummyPropertyWindow()
        {
            InitializeComponent();
			comboBox.SelectedIndex = 0;
			propertyGrid.SelectedObject = propertyGrid;
        }
    }
}