using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Guoyongrong.WinFormsUI.Docking.Win32;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Diagnostics.CodeAnalysis;
namespace Guoyongrong.WinFormsUI.Docking
{
    public partial class FormContent : Form,IContent
    {
        public FormContent()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.CompositingMode = CompositingMode.SourceOver;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

        }
        
        #region IContent

        private bool m_hideOnClose = false;
        public bool HideOnClose
        {
            get { return m_hideOnClose; }
            set { m_hideOnClose = value; }
        }

        private bool m_allowEndUserDocking = true;
        [Category("Category_Docking")]
        [Description("DockPanel_AllowEndUserDocking_Description")]
        [DefaultValue(true)]
        public virtual bool AllowEndUserDocking
        {
            get { return m_allowEndUserDocking; }
            set { m_allowEndUserDocking = value; }
        }

        [Category("Category_Docking")]
        [Description("DockContent_DockAreas_Description")]
        [DefaultValue(DockAreas.DockLeft | DockAreas.DockRight | DockAreas.DockTop | DockAreas.DockBottom | DockAreas.Document | DockAreas.Float)]
        private DockAreas m_allowedAreas = DockAreas.DockLeft | DockAreas.DockRight | DockAreas.DockTop | DockAreas.DockBottom | DockAreas.Document | DockAreas.Float;
        public DockAreas DockAreas
        {
            get { return m_allowedAreas; }
            set
            {
                if (m_allowedAreas == value)
                    return;

                m_allowedAreas = value;

                if (!DockHelper.IsDockStateValid(ShowHint, m_allowedAreas))
                    ShowHint = DockState.Unknown;
            }
        }


        [Category("Category_Docking")]
        [Description("DockContent_ShowHint_Description")]
        [DefaultValue(DockState.Unknown)]
        private DockState m_showHint = DockState.Unknown;
        public DockState ShowHint
        {
            get { return m_showHint; }
            set
            {
                if (!DockHelper.IsDockStateValid(value, DockAreas))
                    throw (new InvalidOperationException(Strings.DockContentHandler_ShowHint_InvalidValue));

                if (m_showHint == value)
                    return;

                m_showHint = value;
            }
        }
        
        private string m_toolTipText = null;
        public string ToolTipText
        {
            get { return m_toolTipText; }
            set { m_toolTipText = value; }
        }

        private ContextMenuStrip m_tabPageContextMenuStrip = null;
        public ContextMenuStrip TabPageContextMenuStrip
        {
            get { return m_tabPageContextMenuStrip; }
            set { m_tabPageContextMenuStrip = value; }
        }

        private ContextMenu m_tabPageContextMenu = null;
        public ContextMenu TabPageContextMenu
        {
            get { return m_tabPageContextMenu; }
            set { m_tabPageContextMenu = value; }
        }

        private Guid m_Guid = Guid.NewGuid();
        public Guid GUID
        {
            get { return m_Guid; }
        }

        public IContent Content
        {
            get { return this; }
        }

        private bool m_closeButtonEnable = true;
        public bool CloseButtonEnable
        {
            get { return m_closeButtonEnable; }
            set
            {
                if (m_closeButtonEnable == value)
                    return;

                m_closeButtonEnable = value;
            }
        }

        private bool m_closeButtonVisible = true;
        public bool CloseButtonVisible
        {
            get { return m_closeButtonVisible; }
            set
            {
                if (m_closeButtonVisible == value)
                    return;

                m_closeButtonVisible = value;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public virtual string GetPersistString()
        {
            return this.GetType().ToString();
        }

        #endregion

    }
}
