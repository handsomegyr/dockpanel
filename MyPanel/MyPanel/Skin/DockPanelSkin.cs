using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.ComponentModel;
using System.Windows.Forms;

namespace Guoyongrong.WinFormsUI.Docking
{
    #region DockPanelSkin classes
    /// <summary>
    /// The skin to use when displaying the DockPanel.
    /// The skin allows custom gradient color schemes to be used when drawing the
    /// DockStrips and Tabs.
    /// </summary>
    [TypeConverter(typeof(DockPanelSkinConverter))]
    public class DockPanelSkin
    {
        private AutoHideStripSkin m_autoHideStripSkin;
        private DockPaneStripSkin m_dockPaneStripSkin;
        private DockPaneCaptionSkin m_dockPaneCaptionSkin;

        public DockPanelSkin()
        {
            m_autoHideStripSkin = new AutoHideStripSkin();
            m_dockPaneStripSkin = new DockPaneStripSkin();
            m_dockPaneCaptionSkin = new DockPaneCaptionSkin();
        }

        /// <summary>
        /// The skin used to display the auto hide strips and tabs.
        /// </summary>
        public AutoHideStripSkin AutoHideStripSkin
        {
            get { return m_autoHideStripSkin; }
            set { m_autoHideStripSkin = value; }
        }

        /// <summary>
        /// The skin used to display the Document and ToolWindow style DockStrips and Tabs.
        /// </summary>
        public DockPaneStripSkin DockPaneStripSkin
        {
            get { return m_dockPaneStripSkin; }
            set { m_dockPaneStripSkin = value; }
        }

        public DockPaneCaptionSkin DockPaneCaptionSkin
        {
            get { return m_dockPaneCaptionSkin; }
            set { m_dockPaneCaptionSkin = value; }
        }
    }


    public class DockPaneCaptionSkin
    {
        public DockPaneCaptionSkin()
        {
            m_ToolWindowGradient = new DockPaneStripToolWindowGradient();
            m_ToolWindowGradient.DockStripGradient.StartColor = SystemColors.ControlLight;
            m_ToolWindowGradient.DockStripGradient.EndColor = SystemColors.ControlLight;

            m_ToolWindowGradient.ActiveTabGradient.StartColor = SystemColors.Control;
            m_ToolWindowGradient.ActiveTabGradient.EndColor = SystemColors.Control;

            m_ToolWindowGradient.InactiveTabGradient.StartColor = Color.Transparent;
            m_ToolWindowGradient.InactiveTabGradient.EndColor = Color.Transparent;
            m_ToolWindowGradient.InactiveTabGradient.TextColor = SystemColors.ControlDarkDark;

            m_ToolWindowGradient.ActiveCaptionGradient.StartColor = SystemColors.GradientActiveCaption;
            m_ToolWindowGradient.ActiveCaptionGradient.EndColor = SystemColors.ActiveCaption;
            m_ToolWindowGradient.ActiveCaptionGradient.LinearGradientMode = LinearGradientMode.Vertical;
            m_ToolWindowGradient.ActiveCaptionGradient.TextColor = SystemColors.ActiveCaptionText;

            m_ToolWindowGradient.InactiveCaptionGradient.StartColor = SystemColors.GradientInactiveCaption;
            m_ToolWindowGradient.InactiveCaptionGradient.EndColor = SystemColors.GradientInactiveCaption;
            m_ToolWindowGradient.InactiveCaptionGradient.LinearGradientMode = LinearGradientMode.Vertical;
            m_ToolWindowGradient.InactiveCaptionGradient.TextColor = SystemColors.ControlText;
        }
        /// <summary>
        /// The skin used to display the ToolWindow style DockPane strip and tab.
        /// </summary>
        ///
        public DockPaneStripToolWindowGradient m_ToolWindowGradient;
        public DockPaneStripToolWindowGradient ToolWindowGradient
        {
            get { return m_ToolWindowGradient; }
            set { m_ToolWindowGradient = value; }
        }
        
        private Bitmap _imageButtonClose;
        public Bitmap ImageButtonClose
        {
            get
            {
                if (_imageButtonClose == null)
                    _imageButtonClose = Resources.DockPane_Close;

                return _imageButtonClose;
            }
        }

        private Bitmap _imageButtonAutoHide;
        public Bitmap ImageButtonAutoHide
        {
            get
            {
                if (_imageButtonAutoHide == null)
                    _imageButtonAutoHide = Resources.DockPane_AutoHide;

                return _imageButtonAutoHide;
            }
        }

        private Bitmap _imageButtonDock;
        public Bitmap ImageButtonDock
        {
            get
            {
                if (_imageButtonDock == null)
                    _imageButtonDock = Resources.DockPane_Dock;

                return _imageButtonDock;
            }
        }

        private Bitmap _imageButtonOptions;
        public Bitmap ImageButtonOptions
        {
            get
            {
                if (_imageButtonOptions == null)
                    _imageButtonOptions = Resources.DockPane_Option;

                return _imageButtonOptions;
            }
        }


        private int _TextGapTop = 2;
        private int _TextGapBottom = 0;
        private int _TextGapLeft = 3;
        private int _TextGapRight = 3;
        private int _ButtonGapTop = 2;
        private int _ButtonGapBottom = 1;
        private int _ButtonGapBetween = 1;
        private int _ButtonGapLeft = 1;
        private int _ButtonGapRight = 2;

        public int TextGapTop
        {
            get { return _TextGapTop; }
        }

        public int TextGapBottom
        {
            get { return _TextGapBottom; }
        }

        public int TextGapLeft
        {
            get { return _TextGapLeft; }
        }

        public int TextGapRight
        {
            get { return _TextGapRight; }
        }

        public int ButtonGapTop
        {
            get { return _ButtonGapTop; }
        }

        public int ButtonGapBottom
        {
            get { return _ButtonGapBottom; }
        }

        public int ButtonGapLeft
        {
            get { return _ButtonGapLeft; }
        }

        public int ButtonGapRight
        {
            get { return _ButtonGapRight; }
        }

        public int ButtonGapBetween
        {
            get { return _ButtonGapBetween; }
        }

        private string _toolTipClose;
        public string ToolTipClose
        {
            get
            {
                if (_toolTipClose == null)
                    _toolTipClose = Strings.DockPaneCaption_ToolTipClose;
                return _toolTipClose;
            }
        }

        private string _toolTipOptions;
        public string ToolTipOptions
        {
            get
            {
                if (_toolTipOptions == null)
                    _toolTipOptions = Strings.DockPaneCaption_ToolTipOptions;

                return _toolTipOptions;
            }
        }

        private string _toolTipAutoHide;
        public string ToolTipAutoHide
        {
            get
            {
                if (_toolTipAutoHide == null)
                    _toolTipAutoHide = Strings.DockPaneCaption_ToolTipAutoHide;
                return _toolTipAutoHide;
            }
        }

        public Font TextFont
        {
            get { return SystemInformation.MenuFont; }
        }

        private Blend _activeBackColorGradientBlend;
        public Blend ActiveBackColorGradientBlend
        {
            get
            {
                if (_activeBackColorGradientBlend == null)
                {
                    Blend blend = new Blend(2);

                    blend.Factors = new float[] { 0.5F, 1.0F };
                    blend.Positions = new float[] { 0.0F, 1.0F };
                    _activeBackColorGradientBlend = blend;
                }

                return _activeBackColorGradientBlend;
            }
        }
    }

    /// <summary>
    /// The skin used to display the auto hide strip and tabs.
    /// </summary>
    [TypeConverter(typeof(AutoHideStripConverter))]
    public class AutoHideStripSkin
    {

        public Font TextFont
        {
            get { return SystemInformation.MenuFont; }
        }

        public Pen PenTabBorder
        {
            get { return SystemPens.GrayText; }
        }

        #region Customizable Properties

        private int _ImageHeight = 16;
        private int _ImageWidth = 16;
        private int _ImageGapTop = 2;
        private int _ImageGapLeft = 4;
        private int _ImageGapRight = 2;
        private int _ImageGapBottom = 2;
        private int _TextGapLeft = 0;
        private int _TextGapRight = 0;
        private int _TabGapTop = 3;
        private int _TabGapLeft = 4;
        private int _TabGapBetween = 10;

        public int ImageHeight
        {
            get { return _ImageHeight; }
        }

        public int ImageWidth
        {
            get { return _ImageWidth; }
        }

        public int ImageGapTop
        {
            get { return _ImageGapTop; }
        }

        public int ImageGapLeft
        {
            get { return _ImageGapLeft; }
        }

        public int ImageGapRight
        {
            get { return _ImageGapRight; }
        }

        public int ImageGapBottom
        {
            get { return _ImageGapBottom; }
        }

        public int TextGapLeft
        {
            get { return _TextGapLeft; }
        }

        public int TextGapRight
        {
            get { return _TextGapRight; }
        }

        public int TabGapTop
        {
            get { return _TabGapTop; }
        }

        public int TabGapLeft
        {
            get { return _TabGapLeft; }
        }

        public int TabGapBetween
        {
            get { return _TabGapBetween; }
        }

        #endregion

        private DockPanelGradient m_dockStripGradient;
        private TabGradient m_TabGradient;

        public AutoHideStripSkin()
        {
            m_dockStripGradient = new DockPanelGradient();
            m_dockStripGradient.StartColor = SystemColors.ControlLight;
            m_dockStripGradient.EndColor = SystemColors.ControlLight;

            m_TabGradient = new TabGradient();
            m_TabGradient.TextColor = SystemColors.ControlDarkDark;
        }

        /// <summary>
        /// The gradient color skin for the DockStrips.
        /// </summary>
        public DockPanelGradient DockStripGradient
        {
            get { return m_dockStripGradient; }
            set { m_dockStripGradient = value; }
        }

        /// <summary>
        /// The gradient color skin for the Tabs.
        /// </summary>
        public TabGradient TabGradient
        {
            get { return m_TabGradient; }
            set { m_TabGradient = value; }
        }
    }

    /// <summary>
    /// The skin used to display the document and tool strips and tabs.
    /// </summary>
    [TypeConverter(typeof(DockPaneStripConverter))]
    public class DockPaneStripSkin
    {
        #region Customizable Properties

        private int _ToolWindowStripGapTop = 0;
        private int _ToolWindowStripGapBottom = 1;
        private int _ToolWindowStripGapLeft = 0;
        private int _ToolWindowStripGapRight = 0;
        private int _ToolWindowImageHeight = 16;
        private int _ToolWindowImageWidth = 16;
        private int _ToolWindowImageGapTop = 3;
        private int _ToolWindowImageGapBottom = 1;
        private int _ToolWindowImageGapLeft = 2;
        private int _ToolWindowImageGapRight = 0;
        private int _ToolWindowTextGapRight = 3;
        private int _ToolWindowTabSeperatorGapTop = 3;
        private int _ToolWindowTabSeperatorGapBottom = 3;

        private int _DocumentStripGapTop = 0;
        private int _DocumentStripGapBottom = 1;
        private int _DocumentTabMaxWidth = 200;
        private int _DocumentButtonGapTop = 4;
        private int _DocumentButtonGapBottom = 4;
        private int _DocumentButtonGapBetween = 0;
        private int _DocumentButtonGapRight = 3;
        private int _DocumentTabGapTop = 3;
        private int _DocumentTabGapLeft = 3;
        private int _DocumentTabGapRight = 3;
        private int _DocumentIconGapBottom = 2;
        private int _DocumentIconGapLeft = 8;
        private int _DocumentIconGapRight = 0;
        private int _DocumentIconHeight = 16;
        private int _DocumentIconWidth = 16;
        private int _DocumentTextGapRight = 3;


        public int ToolWindowStripGapTop
        {
            get { return _ToolWindowStripGapTop; }
        }

        public int ToolWindowStripGapBottom
        {
            get { return _ToolWindowStripGapBottom; }
        }

        public int ToolWindowStripGapLeft
        {
            get { return _ToolWindowStripGapLeft; }
        }

        public int ToolWindowStripGapRight
        {
            get { return _ToolWindowStripGapRight; }
        }

        public int ToolWindowImageHeight
        {
            get { return _ToolWindowImageHeight; }
        }

        public int ToolWindowImageWidth
        {
            get { return _ToolWindowImageWidth; }
        }

        public int ToolWindowImageGapTop
        {
            get { return _ToolWindowImageGapTop; }
        }

        public int ToolWindowImageGapBottom
        {
            get { return _ToolWindowImageGapBottom; }
        }

        public int ToolWindowImageGapLeft
        {
            get { return _ToolWindowImageGapLeft; }
        }

        public int ToolWindowImageGapRight
        {
            get { return _ToolWindowImageGapRight; }
        }

        public int ToolWindowTextGapRight
        {
            get { return _ToolWindowTextGapRight; }
        }

        public int ToolWindowTabSeperatorGapTop
        {
            get { return _ToolWindowTabSeperatorGapTop; }
        }

        public int ToolWindowTabSeperatorGapBottom
        {
            get { return _ToolWindowTabSeperatorGapBottom; }
        }

        public int DocumentStripGapTop
        {
            get { return _DocumentStripGapTop; }
        }

        public int DocumentStripGapBottom
        {
            get { return _DocumentStripGapBottom; }
        }

        public int DocumentTabMaxWidth
        {
            get { return _DocumentTabMaxWidth; }
        }

        public int DocumentButtonGapTop
        {
            get { return _DocumentButtonGapTop; }
        }

        public int DocumentButtonGapBottom
        {
            get { return _DocumentButtonGapBottom; }
        }

        public int DocumentButtonGapBetween
        {
            get { return _DocumentButtonGapBetween; }
        }

        public int DocumentButtonGapRight
        {
            get { return _DocumentButtonGapRight; }
        }

        public int DocumentTabGapTop
        {
            get { return _DocumentTabGapTop; }
        }

        public int DocumentTabGapLeft
        {
            get { return _DocumentTabGapLeft; }
        }

        public int DocumentTabGapRight
        {
            get { return _DocumentTabGapRight; }
        }

        public int DocumentIconGapBottom
        {
            get { return _DocumentIconGapBottom; }
        }

        public int DocumentIconGapLeft
        {
            get { return _DocumentIconGapLeft; }
        }

        public int DocumentIconGapRight
        {
            get { return _DocumentIconGapRight; }
        }

        public int DocumentIconWidth
        {
            get { return _DocumentIconWidth; }
        }

        public int DocumentIconHeight
        {
            get { return _DocumentIconHeight; }
        }

        public int DocumentTextGapRight
        {
            get { return _DocumentTextGapRight; }
        }
        #endregion

        private string _toolTipClose;
        public string ToolTipClose
        {
            get
            {
                if (_toolTipClose == null)
                    _toolTipClose = Strings.DockPaneStrip_ToolTipClose;
                return _toolTipClose;
            }
        }

        private string _toolTipSelect;
        public string ToolTipSelect
        {
            get
            {
                if (_toolTipSelect == null)
                    _toolTipSelect = Strings.DockPaneStrip_ToolTipWindowList;
                return _toolTipSelect;
            }
        }

        public Pen PenToolWindowTabBorder
        {
            get { return SystemPens.GrayText; }
        }

        public Pen PenDocumentTabActiveBorder
        {
            get { return SystemPens.ControlDarkDark; }
        }

        public Pen PenDocumentTabInactiveBorder
        {
            get { return SystemPens.GrayText; }
        }

        private Bitmap _imageButtonClose;
        public Bitmap ImageButtonClose
        {
            get
            {
                if (_imageButtonClose == null)
                    _imageButtonClose = Resources.DockPane_Close;

                return _imageButtonClose;
            }
        }

        private Bitmap _imageButtonWindowList;
        public Bitmap ImageButtonWindowList
        {
            get
            {
                if (_imageButtonWindowList == null)
                    _imageButtonWindowList = Resources.DockPane_Option;

                return _imageButtonWindowList;
            }
        }

        private Bitmap _imageButtonWindowListOverflow;
        public Bitmap ImageButtonWindowListOverflow
        {
            get
            {
                if (_imageButtonWindowListOverflow == null)
                    _imageButtonWindowListOverflow = Resources.DockPane_OptionOverflow;

                return _imageButtonWindowListOverflow;
            }
        }

        public Font TextFont
        {
            get { return SystemInformation.MenuFont; }
        }

        private Font m_boldFont;
        public Font BoldFont
        {
            get
            {
                if (m_boldFont == null)
                {
                    m_boldFont = new Font(TextFont, FontStyle.Bold);
                }
                return m_boldFont;
            }
        }

        private DockPaneStripGradient m_DocumentGradient;
        public DockPaneStripToolWindowGradient m_ToolWindowGradient;

        public DockPaneStripSkin()
        {
            m_DocumentGradient = new DockPaneStripGradient();
            m_DocumentGradient.DockStripGradient.StartColor = SystemColors.Control;
            m_DocumentGradient.DockStripGradient.EndColor = SystemColors.Control;
            m_DocumentGradient.ActiveTabGradient.StartColor = SystemColors.ControlLightLight;
            m_DocumentGradient.ActiveTabGradient.EndColor = SystemColors.ControlLightLight;
            m_DocumentGradient.InactiveTabGradient.StartColor = SystemColors.ControlLight;
            m_DocumentGradient.InactiveTabGradient.EndColor = SystemColors.ControlLight;

            m_ToolWindowGradient = new DockPaneStripToolWindowGradient();
            m_ToolWindowGradient.DockStripGradient.StartColor = SystemColors.ControlLight;
            m_ToolWindowGradient.DockStripGradient.EndColor = SystemColors.ControlLight;

            m_ToolWindowGradient.ActiveTabGradient.StartColor = SystemColors.Control;
            m_ToolWindowGradient.ActiveTabGradient.EndColor = SystemColors.Control;

            m_ToolWindowGradient.InactiveTabGradient.StartColor = Color.Transparent;
            m_ToolWindowGradient.InactiveTabGradient.EndColor = Color.Transparent;
            m_ToolWindowGradient.InactiveTabGradient.TextColor = SystemColors.ControlDarkDark;

            m_ToolWindowGradient.ActiveCaptionGradient.StartColor = SystemColors.GradientActiveCaption;
            m_ToolWindowGradient.ActiveCaptionGradient.EndColor = SystemColors.ActiveCaption;
            m_ToolWindowGradient.ActiveCaptionGradient.LinearGradientMode = LinearGradientMode.Vertical;
            m_ToolWindowGradient.ActiveCaptionGradient.TextColor = SystemColors.ActiveCaptionText;

            m_ToolWindowGradient.InactiveCaptionGradient.StartColor = SystemColors.GradientInactiveCaption;
            m_ToolWindowGradient.InactiveCaptionGradient.EndColor = SystemColors.GradientInactiveCaption;
            m_ToolWindowGradient.InactiveCaptionGradient.LinearGradientMode = LinearGradientMode.Vertical;
            m_ToolWindowGradient.InactiveCaptionGradient.TextColor = SystemColors.ControlText;
        }

        /// <summary>
        /// The skin used to display the Document style DockPane strip and tab.
        /// </summary>
        public DockPaneStripGradient DocumentGradient
        {
            get { return m_DocumentGradient; }
            set { m_DocumentGradient = value; }
        }

        /// <summary>
        /// The skin used to display the ToolWindow style DockPane strip and tab.
        /// </summary>
        public DockPaneStripToolWindowGradient ToolWindowGradient
        {
            get { return m_ToolWindowGradient; }
            set { m_ToolWindowGradient = value; }
        }
    }

    /// <summary>
    /// The skin used to display the DockPane ToolWindow strip and tab.
    /// </summary>
    [TypeConverter(typeof(DockPaneStripGradientConverter))]
    public class DockPaneStripToolWindowGradient : DockPaneStripGradient
    {
        private TabGradient m_activeCaptionGradient;
        private TabGradient m_inactiveCaptionGradient;

        public DockPaneStripToolWindowGradient()
        {
            m_activeCaptionGradient = new TabGradient();
            m_inactiveCaptionGradient = new TabGradient();
        }

        /// <summary>
        /// The skin used to display the active ToolWindow caption.
        /// </summary>
        public TabGradient ActiveCaptionGradient
        {
            get { return m_activeCaptionGradient; }
            set { m_activeCaptionGradient = value; }
        }

        /// <summary>
        /// The skin used to display the inactive ToolWindow caption.
        /// </summary>
        public TabGradient InactiveCaptionGradient
        {
            get { return m_inactiveCaptionGradient; }
            set { m_inactiveCaptionGradient = value; }
        }
    }

    /// <summary>
    /// The skin used to display the DockPane strip and tab.
    /// </summary>
    [TypeConverter(typeof(DockPaneStripGradientConverter))]
    public class DockPaneStripGradient
    {
        private DockPanelGradient m_dockStripGradient;
        private TabGradient m_activeTabGradient;
        private TabGradient m_inactiveTabGradient;

        public DockPaneStripGradient()
        {
            m_dockStripGradient = new DockPanelGradient();
            m_activeTabGradient = new TabGradient();
            m_inactiveTabGradient = new TabGradient();
        }

        /// <summary>
        /// The gradient color skin for the DockStrip.
        /// </summary>
        public DockPanelGradient DockStripGradient
        {
            get { return m_dockStripGradient; }
            set { m_dockStripGradient = value; }
        }

        /// <summary>
        /// The skin used to display the active DockPane tabs.
        /// </summary>
        public TabGradient ActiveTabGradient
        {
            get { return m_activeTabGradient; }
            set { m_activeTabGradient = value; }
        }

        /// <summary>
        /// The skin used to display the inactive DockPane tabs.
        /// </summary>
        public TabGradient InactiveTabGradient
        {
            get { return m_inactiveTabGradient; }
            set { m_inactiveTabGradient = value; }
        }
    }

    /// <summary>
    /// The skin used to display the dock pane tab
    /// </summary>
    [TypeConverter(typeof(DockPaneTabGradientConverter))]
    public class TabGradient : DockPanelGradient
    {
        private Color m_textColor;

        public TabGradient()
        {
            m_textColor = SystemColors.ControlText;
        }

        /// <summary>
        /// The text color.
        /// </summary>
        [DefaultValue(typeof(SystemColors), "ControlText")]
        public Color TextColor
        {
            get { return m_textColor; }
            set { m_textColor = value; }
        }
    }

    /// <summary>
    /// The gradient color skin.
    /// </summary>
    [TypeConverter(typeof(DockPanelGradientConverter))]
    public class DockPanelGradient
    {
        private Color m_startColor;
        private Color m_endColor;
        private LinearGradientMode m_linearGradientMode;

        public DockPanelGradient()
        {
            m_startColor = SystemColors.Control;
            m_endColor = SystemColors.Control;
            m_linearGradientMode = LinearGradientMode.Horizontal;
        }

        /// <summary>
        /// The beginning gradient color.
        /// </summary>
        [DefaultValue(typeof(SystemColors), "Control")]
        public Color StartColor
        {
            get { return m_startColor; }
            set { m_startColor = value; }
        }

        /// <summary>
        /// The ending gradient color.
        /// </summary>
        [DefaultValue(typeof(SystemColors), "Control")]
        public Color EndColor
        {
            get { return m_endColor; }
            set { m_endColor = value; }
        }

        /// <summary>
        /// The gradient mode to display the colors.
        /// </summary>
        [DefaultValue(LinearGradientMode.Horizontal)]
        public LinearGradientMode LinearGradientMode
        {
            get { return m_linearGradientMode; }
            set { m_linearGradientMode = value; }
        }
    }

    #endregion

    #region Converters
    public class DockPanelSkinConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(DockPanelSkin))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(String) && value is DockPanelSkin)
            {
                return "DockPanelSkin";
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public class DockPanelGradientConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(DockPanelGradient))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(String) && value is DockPanelGradient)
            {
                return "DockPanelGradient";
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public class AutoHideStripConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(AutoHideStripSkin))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(String) && value is AutoHideStripSkin)
            {
                return "AutoHideStripSkin";
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public class DockPaneStripConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(DockPaneStripSkin))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(String) && value is DockPaneStripSkin)
            {
                return "DockPaneStripSkin";
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public class DockPaneStripGradientConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(DockPaneStripGradient))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(String) && value is DockPaneStripGradient)
            {
                return "DockPaneStripGradient";
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public class DockPaneTabGradientConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(TabGradient))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(String) && value is TabGradient)
            {
                return "DockPaneTabGradient";
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
    #endregion
}
