using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;

namespace Guoyongrong.WinFormsUI.Docking
{
    partial class DockPanel
    {
        [SuppressMessage("Microsoft.Naming", "CA1720:AvoidTypeNamesInParameters", MessageId = "0#")]
        public delegate IContent DeserializeDockContent(string persistString);

        private static class Persistor
        {
            private const string ConfigFileVersion = "1.0";
            private static string[] CompatibleConfigFileVersions = new string[] { };

            private class DummyContent : DockContent
            {
            }

            private struct DockPanelStruct
            {
                private double m_dockLeftPortion;
                public double DockLeftPortion
                {
                    get { return m_dockLeftPortion; }
                    set { m_dockLeftPortion = value; }
                }

                private double m_dockRightPortion;
                public double DockRightPortion
                {
                    get { return m_dockRightPortion; }
                    set { m_dockRightPortion = value; }
                }

                private double m_dockTopPortion;
                public double DockTopPortion
                {
                    get { return m_dockTopPortion; }
                    set { m_dockTopPortion = value; }
                }

                private double m_dockBottomPortion;
                public double DockBottomPortion
                {
                    get { return m_dockBottomPortion; }
                    set { m_dockBottomPortion = value; }
                }

                private int m_indexActiveDocumentPane;
                public int IndexActiveDocumentPane
                {
                    get { return m_indexActiveDocumentPane; }
                    set { m_indexActiveDocumentPane = value; }
                }

                private int m_indexActivePane;
                public int IndexActivePane
                {
                    get { return m_indexActivePane; }
                    set { m_indexActivePane = value; }
                }
            }

            private struct ContentStruct
            {
                private string m_persistString;
                public string PersistString
                {
                    get { return m_persistString; }
                    set { m_persistString = value; }
                }

                private double m_autoHidePortion;
                public double AutoHidePortion
                {
                    get { return m_autoHidePortion; }
                    set { m_autoHidePortion = value; }
                }

                private bool m_isHidden;
                public bool IsHidden
                {
                    get { return m_isHidden; }
                    set { m_isHidden = value; }
                }

                private bool m_isFloat;
                public bool IsFloat
                {
                    get { return m_isFloat; }
                    set { m_isFloat = value; }
                }

                private DockState m_DockState;
                public DockState DockState
                {
                    get { return m_DockState; }
                    set { m_DockState = value; }
                }

                private int m_PageRefID;
                public int PageRefID
                {
                    get { return m_PageRefID; }
                    set { m_PageRefID = value; }
                }
            }

            private struct PaneStruct
            {
                private int m_ID;
                public int ID
                {
                    get { return m_ID; }
                    set { m_ID = value; }
                }

                private DockState m_dockState;
                public DockState DockState
                {
                    get { return m_dockState; }
                    set { m_dockState = value; }
                }

                private int m_indexActiveContent;
                public int IndexActiveContent
                {
                    get { return m_indexActiveContent; }
                    set { m_indexActiveContent = value; }
                }

                private int m_WindowRefID;
                public int WindowRefID
                {
                    get { return m_WindowRefID; }
                    set { m_WindowRefID = value; }
                }

                private int[] m_indexContents;
                public int[] IndexContents
                {
                    get { return m_indexContents; }
                    set { m_indexContents = value; }
                }

                private int m_zOrderIndex;
                public int ZOrderIndex
                {
                    get { return m_zOrderIndex; }
                    set { m_zOrderIndex = value; }
                }
            }

            private struct NestedPane
            {
                private int m_indexPane;
                public int IndexPane
                {
                    get { return m_indexPane; }
                    set { m_indexPane = value; }
                }

                private int m_indexPrevPane;
                public int IndexPrevPane
                {
                    get { return m_indexPrevPane; }
                    set { m_indexPrevPane = value; }
                }

                private DockAlignment m_alignment;
                public DockAlignment Alignment
                {
                    get { return m_alignment; }
                    set { m_alignment = value; }
                }

                private double m_proportion;
                public double Proportion
                {
                    get { return m_proportion; }
                    set { m_proportion = value; }
                }
            }

            private struct DockWindowStruct
            {
                private DockState m_dockState;
                public DockState DockState
                {
                    get { return m_dockState; }
                    set { m_dockState = value; }
                }

                private int m_zOrderIndex;
                public int ZOrderIndex
                {
                    get { return m_zOrderIndex; }
                    set { m_zOrderIndex = value; }
                }

                private NestedPane[] m_nestedPanes;
                public NestedPane[] NestedPanes
                {
                    get { return m_nestedPanes; }
                    set { m_nestedPanes = value; }
                }
            }

            private struct FloatWindowStruct
            {
                private Rectangle m_bounds;
                public Rectangle Bounds
                {
                    get { return m_bounds; }
                    set { m_bounds = value; }
                }

                private int m_zOrderIndex;
                public int ZOrderIndex
                {
                    get { return m_zOrderIndex; }
                    set { m_zOrderIndex = value; }
                }

                private NestedPane[] m_nestedPanes;
                public NestedPane[] NestedPanes
                {
                    get { return m_nestedPanes; }
                    set { m_nestedPanes = value; }
                }
            }

            public static void SaveAsXml(DockPanel dockPanel, string fileName)
            {
                SaveAsXml(dockPanel, fileName, Encoding.Unicode);
            }

            public static void SaveAsXml(DockPanel dockPanel, string fileName, Encoding encoding)
            {
                FileStream fs = new FileStream(fileName, FileMode.Create);
                try
                {
                    SaveAsXml(dockPanel, fs, encoding);
                }
                finally
                {
                    fs.Close();
                }
            }

            public static void SaveAsXml(DockPanel dockPanel, Stream stream, Encoding encoding)
            {
                SaveAsXml(dockPanel, stream, encoding, false);
            }

            public static void SaveAsXml(DockPanel dockPanel, Stream stream, Encoding encoding, bool upstream)
            {
                dockPanel.Clean();

                XmlTextWriter xmlOut = new XmlTextWriter(stream, encoding);

                // Use indenting for readability
                xmlOut.Formatting = Formatting.Indented;

                if (!upstream)
                    xmlOut.WriteStartDocument();

                // Always begin file with identification and warning
                xmlOut.WriteComment(Strings.DockPanel_Persistor_XmlFileComment1);
                xmlOut.WriteComment(Strings.DockPanel_Persistor_XmlFileComment2);

                // Associate a version number with the root element so that future version of the code
                // will be able to be backwards compatible or at least recognise out of date versions
                xmlOut.WriteStartElement("DockPanel");
                xmlOut.WriteAttributeString("FormatVersion", ConfigFileVersion);
                xmlOut.WriteAttributeString("DockLeftPortion", dockPanel.DockLeftPortion.ToString(CultureInfo.InvariantCulture));
                xmlOut.WriteAttributeString("DockRightPortion", dockPanel.DockRightPortion.ToString(CultureInfo.InvariantCulture));
                xmlOut.WriteAttributeString("DockTopPortion", dockPanel.DockTopPortion.ToString(CultureInfo.InvariantCulture));
                xmlOut.WriteAttributeString("DockBottomPortion", dockPanel.DockBottomPortion.ToString(CultureInfo.InvariantCulture));
                xmlOut.WriteAttributeString("ActiveDocumentPane", dockPanel.DockPages.IndexOf(dockPanel.ActiveDocumentPage).ToString(CultureInfo.InvariantCulture));
                xmlOut.WriteAttributeString("ActivePane", dockPanel.DockPages.IndexOf(dockPanel.ActivePage).ToString(CultureInfo.InvariantCulture));

                // DockContents
                xmlOut.WriteStartElement("DockContents");
                xmlOut.WriteAttributeString("Count", dockPanel.DockContents.Count.ToString(CultureInfo.InvariantCulture));
                foreach (DockContent content in dockPanel.DockContents)
                {
                    xmlOut.WriteStartElement("Content");
                    xmlOut.WriteAttributeString("ID", dockPanel.DockContents.IndexOf(content).ToString(CultureInfo.InvariantCulture));
                    xmlOut.WriteAttributeString("PersistString", content.PersistString);
                    xmlOut.WriteAttributeString("AutoHidePortion", content.AutoHidePortion.ToString(CultureInfo.InvariantCulture));
                    xmlOut.WriteAttributeString("IsHidden", content.IsHidden.ToString(CultureInfo.InvariantCulture));
                    xmlOut.WriteAttributeString("DockState", content.DockState.ToString());
                    xmlOut.WriteAttributeString("PageRefID", dockPanel.DockPages.IndexOf(content.DockPage).ToString(CultureInfo.InvariantCulture));
                    xmlOut.WriteEndElement();
                }
                xmlOut.WriteEndElement();

                // DockPages
                xmlOut.WriteStartElement("DockPages");
                xmlOut.WriteAttributeString("Count", dockPanel.DockPages.Count.ToString(CultureInfo.InvariantCulture));
                foreach (DockPage pane in dockPanel.DockPages)
                {
                    xmlOut.WriteStartElement("Pane");
                    xmlOut.WriteAttributeString("ID", dockPanel.DockPages.IndexOf(pane).ToString(CultureInfo.InvariantCulture));
                    xmlOut.WriteAttributeString("DockState", pane.DockState.ToString());
                    xmlOut.WriteAttributeString("ActiveContent", dockPanel.DockContents.IndexOf(pane.ActiveContent).ToString(CultureInfo.InvariantCulture));
                    if (pane.Parent == null)
                    {
                        xmlOut.WriteAttributeString("WindowRefID", "-1");
                    }
                    else
                    {
                        if (pane.Parent is FloatWindow)
                        {
                            xmlOut.WriteAttributeString("WindowRefID", dockPanel.FloatWindows.IndexOf((FloatWindow)pane.Parent).ToString(CultureInfo.InvariantCulture));
                        }
                        if (pane.Parent is DockWindow)
                        {
                            xmlOut.WriteAttributeString("WindowRefID", dockPanel.DockWindows.IndexOf((DockWindow)pane.Parent).ToString(CultureInfo.InvariantCulture));
                        }
                        if (pane.Parent is AutoHideWindow)
                        {
                            xmlOut.WriteAttributeString("WindowRefID", "-2");
                        }
                    }
                    xmlOut.WriteStartElement("DockContents");
                    xmlOut.WriteAttributeString("Count", pane.DockContents.Count.ToString(CultureInfo.InvariantCulture));
                    foreach (DockContent content in pane.DockContents)
                    {
                        xmlOut.WriteStartElement("Content");
                        xmlOut.WriteAttributeString("ID", pane.DockContents.IndexOf(content).ToString(CultureInfo.InvariantCulture));
                        xmlOut.WriteAttributeString("RefID", dockPanel.DockContents.IndexOf(content).ToString(CultureInfo.InvariantCulture));
                        xmlOut.WriteEndElement();
                    }
                    xmlOut.WriteEndElement();
                    xmlOut.WriteEndElement();
                }
                xmlOut.WriteEndElement();

                // DockWindows
                xmlOut.WriteStartElement("DockWindows");
                int dockWindowId = 0;
                foreach (DockWindow dw in dockPanel.DockWindows)
                {
                    xmlOut.WriteStartElement("DockWindow");
                    xmlOut.WriteAttributeString("ID", dockWindowId.ToString(CultureInfo.InvariantCulture));
                    dockWindowId++;
                    xmlOut.WriteAttributeString("DockState", dw.DockState.ToString());
                    xmlOut.WriteAttributeString("ZOrderIndex", dockPanel.Controls.IndexOf(dw).ToString(CultureInfo.InvariantCulture));
                    xmlOut.WriteStartElement("NestedPanes");
                    xmlOut.WriteAttributeString("Count", dw.NestedPanes.Count.ToString(CultureInfo.InvariantCulture));
                    foreach (DockPage pane in dw.NestedPanes)
                    {
                        xmlOut.WriteStartElement("Pane");
                        xmlOut.WriteAttributeString("ID", dw.NestedPanes.IndexOf(pane).ToString(CultureInfo.InvariantCulture));
                        xmlOut.WriteAttributeString("RefID", dockPanel.DockPages.IndexOf(pane).ToString(CultureInfo.InvariantCulture));
                        NestedDockingStatus status = pane.NestedDockingStatus;
                        xmlOut.WriteAttributeString("PrevPane", dockPanel.DockPages.IndexOf(status.PreviousPane).ToString(CultureInfo.InvariantCulture));
                        xmlOut.WriteAttributeString("Alignment", status.Alignment.ToString());
                        xmlOut.WriteAttributeString("Proportion", status.Proportion.ToString(CultureInfo.InvariantCulture));
                        xmlOut.WriteEndElement();
                    }
                    xmlOut.WriteEndElement();
                    xmlOut.WriteEndElement();
                }
                xmlOut.WriteEndElement();

                // FloatWindows
                RectangleConverter rectConverter = new RectangleConverter();
                xmlOut.WriteStartElement("FloatWindows");
                xmlOut.WriteAttributeString("Count", dockPanel.FloatWindows.Count.ToString(CultureInfo.InvariantCulture));
                foreach (FloatWindow fw in dockPanel.FloatWindows)
                {
                    xmlOut.WriteStartElement("FloatWindow");
                    xmlOut.WriteAttributeString("ID", dockPanel.FloatWindows.IndexOf(fw).ToString(CultureInfo.InvariantCulture));
                    xmlOut.WriteAttributeString("Bounds", rectConverter.ConvertToInvariantString(fw.Bounds));
                    xmlOut.WriteAttributeString("ZOrderIndex", dockPanel.FloatWindows.IndexOf(fw).ToString(CultureInfo.InvariantCulture));
                    xmlOut.WriteStartElement("NestedPanes");
                    xmlOut.WriteAttributeString("Count", fw.NestedPanes.Count.ToString(CultureInfo.InvariantCulture));
                    foreach (DockPage pane in fw.NestedPanes)
                    {
                        xmlOut.WriteStartElement("Pane");
                        xmlOut.WriteAttributeString("ID", fw.NestedPanes.IndexOf(pane).ToString(CultureInfo.InvariantCulture));
                        xmlOut.WriteAttributeString("RefID", dockPanel.DockPages.IndexOf(pane).ToString(CultureInfo.InvariantCulture));
                        NestedDockingStatus status = pane.NestedDockingStatus;
                        xmlOut.WriteAttributeString("PrevPane", dockPanel.DockPages.IndexOf(status.PreviousPane).ToString(CultureInfo.InvariantCulture));
                        xmlOut.WriteAttributeString("Alignment", status.Alignment.ToString());
                        xmlOut.WriteAttributeString("Proportion", status.Proportion.ToString(CultureInfo.InvariantCulture));
                        xmlOut.WriteEndElement();
                    }
                    xmlOut.WriteEndElement();
                    xmlOut.WriteEndElement();
                }
                xmlOut.WriteEndElement();	//	</FloatWindows>

                xmlOut.WriteEndElement();

                if (!upstream)
                {
                    xmlOut.WriteEndDocument();
                    xmlOut.Close();
                }
                else
                    xmlOut.Flush();
            }

            public static void LoadFromXml(DockPanel dockPanel, string fileName, DeserializeDockContent deserializeContent)
            {
                FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                try
                {
                    LoadFromXml(dockPanel, fs, deserializeContent);
                }
                finally
                {
                    fs.Close();
                }
            }

            public static void LoadFromXml(DockPanel dockPanel, Stream stream, DeserializeDockContent deserializeContent)
            {
                LoadFromXml(dockPanel, stream, deserializeContent, true);
            }

            private static ContentStruct[] LoadContents(XmlTextReader xmlIn)
            {
                EnumConverter dockStateConverter = new EnumConverter(typeof(DockState));
                int countOfContents = Convert.ToInt32(xmlIn.GetAttribute("Count"), CultureInfo.InvariantCulture);
                ContentStruct[] DockContents = new ContentStruct[countOfContents];
                MoveToNextElement(xmlIn);
                for (int i = 0; i < countOfContents; i++)
                {
                    int id = Convert.ToInt32(xmlIn.GetAttribute("ID"), CultureInfo.InvariantCulture);
                    if (xmlIn.Name != "Content" || id != i)
                        throw new ArgumentException(Strings.DockPanel_LoadFromXml_InvalidXmlFormat);

                    DockContents[i].PersistString = xmlIn.GetAttribute("PersistString");
                    DockContents[i].AutoHidePortion = Convert.ToDouble(xmlIn.GetAttribute("AutoHidePortion"), CultureInfo.InvariantCulture);
                    DockContents[i].IsHidden = Convert.ToBoolean(xmlIn.GetAttribute("IsHidden"), CultureInfo.InvariantCulture);
                    //DockContents[i].IsFloat = Convert.ToBoolean(xmlIn.GetAttribute("IsFloat"), CultureInfo.InvariantCulture);
                    DockContents[i].DockState = (DockState)dockStateConverter.ConvertFrom(xmlIn.GetAttribute("DockState"));
                    DockContents[i].PageRefID = Convert.ToInt32(xmlIn.GetAttribute("PageRefID"), CultureInfo.InvariantCulture);
                    
                    MoveToNextElement(xmlIn);
                }

                return DockContents;
            }

            private static PaneStruct[] LoadPanes(XmlTextReader xmlIn)
            {
                EnumConverter dockStateConverter = new EnumConverter(typeof(DockState));
                int countOfPanes = Convert.ToInt32(xmlIn.GetAttribute("Count"), CultureInfo.InvariantCulture);
                PaneStruct[] DockPages = new PaneStruct[countOfPanes];
                MoveToNextElement(xmlIn);
                for (int i = 0; i < countOfPanes; i++)
                {
                    int id = Convert.ToInt32(xmlIn.GetAttribute("ID"), CultureInfo.InvariantCulture);
                    if (xmlIn.Name != "Pane" || id != i)
                        throw new ArgumentException(Strings.DockPanel_LoadFromXml_InvalidXmlFormat);

                    DockPages[i].ID = Convert.ToInt32(id, CultureInfo.InvariantCulture);
                    DockPages[i].DockState = (DockState)dockStateConverter.ConvertFrom(xmlIn.GetAttribute("DockState"));
                    DockPages[i].IndexActiveContent = Convert.ToInt32(xmlIn.GetAttribute("ActiveContent"), CultureInfo.InvariantCulture);
                    DockPages[i].ZOrderIndex = -1;
                    DockPages[i].WindowRefID = Convert.ToInt32(xmlIn.GetAttribute("WindowRefID"), CultureInfo.InvariantCulture);
                    
                    MoveToNextElement(xmlIn);
                    if (xmlIn.Name != "DockContents")
                        throw new ArgumentException(Strings.DockPanel_LoadFromXml_InvalidXmlFormat);
                    int countOfPaneContents = Convert.ToInt32(xmlIn.GetAttribute("Count"), CultureInfo.InvariantCulture);
                    DockPages[i].IndexContents = new int[countOfPaneContents];
                    MoveToNextElement(xmlIn);
                    for (int j = 0; j < countOfPaneContents; j++)
                    {
                        int id2 = Convert.ToInt32(xmlIn.GetAttribute("ID"), CultureInfo.InvariantCulture);
                        if (xmlIn.Name != "Content" || id2 != j)
                            throw new ArgumentException(Strings.DockPanel_LoadFromXml_InvalidXmlFormat);

                        DockPages[i].IndexContents[j] = Convert.ToInt32(xmlIn.GetAttribute("RefID"), CultureInfo.InvariantCulture);
                        MoveToNextElement(xmlIn);
                    }
                }

                return DockPages;
            }

            private static DockWindowStruct[] LoadDockWindows(XmlTextReader xmlIn, DockPanel dockPanel)
            {
                EnumConverter dockStateConverter = new EnumConverter(typeof(DockState));
                EnumConverter dockAlignmentConverter = new EnumConverter(typeof(DockAlignment));
                int countOfDockWindows = dockPanel.DockWindows.Count;
                DockWindowStruct[] dockWindows = new DockWindowStruct[countOfDockWindows];
                MoveToNextElement(xmlIn);
                for (int i = 0; i < countOfDockWindows; i++)
                {
                    int id = Convert.ToInt32(xmlIn.GetAttribute("ID"), CultureInfo.InvariantCulture);
                    if (xmlIn.Name != "DockWindow" || id != i)
                        throw new ArgumentException(Strings.DockPanel_LoadFromXml_InvalidXmlFormat);

                    dockWindows[i].DockState = (DockState)dockStateConverter.ConvertFrom(xmlIn.GetAttribute("DockState"));
                    dockWindows[i].ZOrderIndex = Convert.ToInt32(xmlIn.GetAttribute("ZOrderIndex"), CultureInfo.InvariantCulture);
                    MoveToNextElement(xmlIn);
                    if (xmlIn.Name != "DockList" && xmlIn.Name != "NestedPanes")
                        throw new ArgumentException(Strings.DockPanel_LoadFromXml_InvalidXmlFormat);
                    int countOfNestedPanes = Convert.ToInt32(xmlIn.GetAttribute("Count"), CultureInfo.InvariantCulture);
                    dockWindows[i].NestedPanes = new NestedPane[countOfNestedPanes];
                    MoveToNextElement(xmlIn);
                    for (int j = 0; j < countOfNestedPanes; j++)
                    {
                        int id2 = Convert.ToInt32(xmlIn.GetAttribute("ID"), CultureInfo.InvariantCulture);
                        if (xmlIn.Name != "Pane" || id2 != j)
                            throw new ArgumentException(Strings.DockPanel_LoadFromXml_InvalidXmlFormat);
                        dockWindows[i].NestedPanes[j].IndexPane = Convert.ToInt32(xmlIn.GetAttribute("RefID"), CultureInfo.InvariantCulture);
                        dockWindows[i].NestedPanes[j].IndexPrevPane = Convert.ToInt32(xmlIn.GetAttribute("PrevPane"), CultureInfo.InvariantCulture);
                        dockWindows[i].NestedPanes[j].Alignment = (DockAlignment)dockAlignmentConverter.ConvertFrom(xmlIn.GetAttribute("Alignment"));
                        dockWindows[i].NestedPanes[j].Proportion = Convert.ToDouble(xmlIn.GetAttribute("Proportion"), CultureInfo.InvariantCulture);
                        MoveToNextElement(xmlIn);
                    }
                }

                return dockWindows;
            }

            private static FloatWindowStruct[] LoadFloatWindows(XmlTextReader xmlIn)
            {
                EnumConverter dockAlignmentConverter = new EnumConverter(typeof(DockAlignment));
                RectangleConverter rectConverter = new RectangleConverter();
                int countOfFloatWindows = Convert.ToInt32(xmlIn.GetAttribute("Count"), CultureInfo.InvariantCulture);
                FloatWindowStruct[] floatWindows = new FloatWindowStruct[countOfFloatWindows];
                MoveToNextElement(xmlIn);
                for (int i = 0; i < countOfFloatWindows; i++)
                {
                    int id = Convert.ToInt32(xmlIn.GetAttribute("ID"), CultureInfo.InvariantCulture);
                    if (xmlIn.Name != "FloatWindow" || id != i)
                        throw new ArgumentException(Strings.DockPanel_LoadFromXml_InvalidXmlFormat);

                    floatWindows[i].Bounds = (Rectangle)rectConverter.ConvertFromInvariantString(xmlIn.GetAttribute("Bounds"));
                    floatWindows[i].ZOrderIndex = Convert.ToInt32(xmlIn.GetAttribute("ZOrderIndex"), CultureInfo.InvariantCulture);
                    MoveToNextElement(xmlIn);
                    if (xmlIn.Name != "DockList" && xmlIn.Name != "NestedPanes")
                        throw new ArgumentException(Strings.DockPanel_LoadFromXml_InvalidXmlFormat);
                    int countOfNestedPanes = Convert.ToInt32(xmlIn.GetAttribute("Count"), CultureInfo.InvariantCulture);
                    floatWindows[i].NestedPanes = new NestedPane[countOfNestedPanes];
                    MoveToNextElement(xmlIn);
                    for (int j = 0; j < countOfNestedPanes; j++)
                    {
                        int id2 = Convert.ToInt32(xmlIn.GetAttribute("ID"), CultureInfo.InvariantCulture);
                        if (xmlIn.Name != "Pane" || id2 != j)
                            throw new ArgumentException(Strings.DockPanel_LoadFromXml_InvalidXmlFormat);
                        floatWindows[i].NestedPanes[j].IndexPane = Convert.ToInt32(xmlIn.GetAttribute("RefID"), CultureInfo.InvariantCulture);
                        floatWindows[i].NestedPanes[j].IndexPrevPane = Convert.ToInt32(xmlIn.GetAttribute("PrevPane"), CultureInfo.InvariantCulture);
                        floatWindows[i].NestedPanes[j].Alignment = (DockAlignment)dockAlignmentConverter.ConvertFrom(xmlIn.GetAttribute("Alignment"));
                        floatWindows[i].NestedPanes[j].Proportion = Convert.ToDouble(xmlIn.GetAttribute("Proportion"), CultureInfo.InvariantCulture);
                        MoveToNextElement(xmlIn);
                    }
                }

                return floatWindows;
            }

            public static void LoadFromXml(DockPanel dockPanel, Stream stream, DeserializeDockContent deserializeContent, bool closeStream)
            {

                if (dockPanel.DockContents.Count != 0)
                    throw new InvalidOperationException(Strings.DockPanel_LoadFromXml_AlreadyInitialized);

                XmlTextReader xmlIn = new XmlTextReader(stream);
                xmlIn.WhitespaceHandling = WhitespaceHandling.None;
                xmlIn.MoveToContent();

                while (!xmlIn.Name.Equals("DockPanel"))
                {
                    if (!MoveToNextElement(xmlIn))
                        throw new ArgumentException(Strings.DockPanel_LoadFromXml_InvalidXmlFormat);
                }

                string formatVersion = xmlIn.GetAttribute("FormatVersion");
                if (!IsFormatVersionValid(formatVersion))
                    throw new ArgumentException(Strings.DockPanel_LoadFromXml_InvalidFormatVersion);

                DockPanelStruct dockPanelStruct = new DockPanelStruct();
                dockPanelStruct.DockLeftPortion = Convert.ToDouble(xmlIn.GetAttribute("DockLeftPortion"), CultureInfo.InvariantCulture);
                dockPanelStruct.DockRightPortion = Convert.ToDouble(xmlIn.GetAttribute("DockRightPortion"), CultureInfo.InvariantCulture);
                dockPanelStruct.DockTopPortion = Convert.ToDouble(xmlIn.GetAttribute("DockTopPortion"), CultureInfo.InvariantCulture);
                dockPanelStruct.DockBottomPortion = Convert.ToDouble(xmlIn.GetAttribute("DockBottomPortion"), CultureInfo.InvariantCulture);
                dockPanelStruct.IndexActiveDocumentPane = Convert.ToInt32(xmlIn.GetAttribute("ActiveDocumentPane"), CultureInfo.InvariantCulture);
                dockPanelStruct.IndexActivePane = Convert.ToInt32(xmlIn.GetAttribute("ActivePane"), CultureInfo.InvariantCulture);

                // Load DockContents
                MoveToNextElement(xmlIn);
                if (xmlIn.Name != "DockContents")
                    throw new ArgumentException(Strings.DockPanel_LoadFromXml_InvalidXmlFormat);
                ContentStruct[] DockContents = LoadContents(xmlIn);

                // Load DockPages
                if (xmlIn.Name != "DockPages")
                    throw new ArgumentException(Strings.DockPanel_LoadFromXml_InvalidXmlFormat);
                PaneStruct[] DockPages = LoadPanes(xmlIn);

                // Load DockWindows
                if (xmlIn.Name != "DockWindows")
                    throw new ArgumentException(Strings.DockPanel_LoadFromXml_InvalidXmlFormat);
                DockWindowStruct[] dockWindows = LoadDockWindows(xmlIn, dockPanel);

                // Load FloatWindows
                if (xmlIn.Name != "FloatWindows")
                    throw new ArgumentException(Strings.DockPanel_LoadFromXml_InvalidXmlFormat);
                FloatWindowStruct[] floatWindows = LoadFloatWindows(xmlIn);

                if (closeStream)
                    xmlIn.Close();

                dockPanel.SuspendLayout(true);

                dockPanel.DockLeftPortion = dockPanelStruct.DockLeftPortion;
                dockPanel.DockRightPortion = dockPanelStruct.DockRightPortion;
                dockPanel.DockTopPortion = dockPanelStruct.DockTopPortion;
                dockPanel.DockBottomPortion = dockPanelStruct.DockBottomPortion;

                // Set DockWindow ZOrders
                int prevMaxDockWindowZOrder = int.MaxValue;
                for (int i = 0; i < dockWindows.Length; i++)
                {
                    int maxDockWindowZOrder = -1;
                    int index = -1;
                    for (int j = 0; j < dockWindows.Length; j++)
                    {
                        if (dockWindows[j].ZOrderIndex > maxDockWindowZOrder && dockWindows[j].ZOrderIndex < prevMaxDockWindowZOrder)
                        {
                            maxDockWindowZOrder = dockWindows[j].ZOrderIndex;
                            index = j;
                        }
                    }

                    dockPanel.DockWindows[dockWindows[index].DockState].BringToFront();
                    prevMaxDockWindowZOrder = maxDockWindowZOrder;
                }

                // Create DockContents
                for (int i = 0; i < DockContents.Length; i++)
                {
                    IContent c = deserializeContent(DockContents[i].PersistString);

                    DockContent content = new DockContent(c,DockContents[i].DockState);
                    if (content == null)
                        content = new DummyContent();
                    //content.DockHandler.DockPanel = dockPanel;
                    content.AutoHidePortion = DockContents[i].AutoHidePortion;
                    content.IsHidden = DockContents[i].IsHidden;
                    //content.IsFloat = DockContents[i].IsFloat;
                    dockPanel.DockPanelHandler.AddDockContent(content);
                }

                while (dockPanel.DockContents.Count != DockContents.Length)
                {
                    ;
                }

                // Create DockPages
                for (int i = 0; i < DockPages.Length; i++)
                {
                    DockPage pane = dockPanel.DockPanelHandler.CreateDockPage(DockPages[i].DockState);
                }
                //wait
                while (dockPanel.DockPages.Count != DockPages.Length)
                {
                    ;
                }

                //Content DockTo page
                for (int i = 0; i < DockPages.Length; i++)
                {
                    DockPage pane = dockPanel.DockPages[DockPages[i].ID];
                    for (int j = 0; j < DockPages[i].IndexContents.Length; j++)
                    {
                        DockContent content = dockPanel.DockContents[DockPages[i].IndexContents[j]];
                        DockState oldState = content.DockState;
                        pane.AddDockContent(content);
                        content.DockState = oldState;
                        content.DockPage = dockPanel.DockPages[DockContents[DockPages[i].IndexContents[j]].PageRefID];
                    }
                }

                // Assign DockPages to DockWindows
                for (int i = 0; i < dockWindows.Length; i++)
                {
                    for (int j = 0; j < dockWindows[i].NestedPanes.Length; j++)
                    {
                        DockWindow dw = dockPanel.DockWindows[dockWindows[i].DockState];
                        int indexPane = dockWindows[i].NestedPanes[j].IndexPane;
                        DockPage pane = dockPanel.DockPages[indexPane];
                        DockState oldState = pane.DockState;
                        int indexPrevPane = dockWindows[i].NestedPanes[j].IndexPrevPane;
                        DockPage prevPane = (indexPrevPane == -1) ? dw.NestedPanes.GetDefaultPreviousPane(pane) : dockPanel.DockPages[indexPrevPane];
                        DockAlignment alignment = dockWindows[i].NestedPanes[j].Alignment;
                        double proportion = dockWindows[i].NestedPanes[j].Proportion;
                        dockPanel.AutoHideStripWindow.AddDockPage(pane);
                        dw.AddDockPage(pane, prevPane, alignment, proportion, -1);
                        if (DockPages[indexPane].DockState == dw.DockState)
                            DockPages[indexPane].ZOrderIndex = dockWindows[i].ZOrderIndex;
                        pane.DockState = oldState;

                    }
                }

                // Create float windows
                for (int i = 0; i < floatWindows.Length; i++)
                {
                    FloatWindow fw = dockPanel.FloatWindows.CreateOrGetFloatWindow();
                    if (fw.StartPosition != FormStartPosition.Manual)
                    {
                        fw.StartPosition = FormStartPosition.Manual;
                    }
                    fw.Bounds = floatWindows[i].Bounds;

                    for (int j = 0; j < floatWindows[i].NestedPanes.Length; j++)
                    {
                        int indexPane = floatWindows[i].NestedPanes[j].IndexPane;
                        DockPage pane = dockPanel.DockPages[indexPane];
                        DockState oldState = pane.DockState;
                        int indexPrevPane = floatWindows[i].NestedPanes[j].IndexPrevPane;
                        DockPage prevPane = indexPrevPane == -1 ? null : dockPanel.DockPages[indexPrevPane];
                        DockAlignment alignment = floatWindows[i].NestedPanes[j].Alignment;
                        double proportion = floatWindows[i].NestedPanes[j].Proportion;
                        fw.AddDockPage(pane, prevPane, alignment, proportion, -1);
                        if (DockPages[indexPane].DockState == fw.DockState)
                            DockPages[indexPane].ZOrderIndex = floatWindows[i].ZOrderIndex;
                        pane.DockState = oldState;
                    }
                    fw.Visible = true;
                }

                //DockPages parent
                for (int i = 0; i < DockPages.Length; i++)
                {
                    DockPage pane = dockPanel.DockPages[DockPages[i].ID];

                    if (DockPages[i].WindowRefID == -1)
                    {
                        pane.SetParent(null);
                        pane.Splitter.Parent = null;
                    }
                    else
                    {
                        if (DockPages[i].WindowRefID == -2)
                        {
                            dockPanel.AutoHideWindow.SetDockPageParent(pane);
                        }

                    }
                }

                // sort IDockContent by its Pane's ZOrder
                int[] sortedContents = null;
                if (DockContents.Length > 0)
                {
                    sortedContents = new int[DockContents.Length];
                    for (int i = 0; i < DockContents.Length; i++)
                        sortedContents[i] = i;

                    int lastDocument = DockContents.Length;
                    for (int i = 0; i < DockContents.Length - 1; i++)
                    {
                        for (int j = i + 1; j < DockContents.Length; j++)
                        {
                            DockPage pane1 = dockPanel.DockContents[sortedContents[i]].DockPage;
                            int ZOrderIndex1 = pane1 == null ? 0 : DockPages[dockPanel.DockPages.IndexOf(pane1)].ZOrderIndex;
                            DockPage pane2 = dockPanel.DockContents[sortedContents[j]].DockPage;
                            int ZOrderIndex2 = pane2 == null ? 0 : DockPages[dockPanel.DockPages.IndexOf(pane2)].ZOrderIndex;
                            if (ZOrderIndex1 > ZOrderIndex2)
                            {
                                int temp = sortedContents[i];
                                sortedContents[i] = sortedContents[j];
                                sortedContents[j] = temp;
                            }
                        }
                    }
                }

                // show non-document IDockContent first to avoid screen flickers
                for (int i = 0; i < DockContents.Length; i++)
                {
                    DockContent content = dockPanel.DockContents[sortedContents[i]];
                    //if (content.Page != null && content.Page.DockState != DockState.Document)
                    //{
                    //    //content.DockHandler.IsHidden = DockContents[sortedContents[i]].IsHidden;
                    //}
                }

                // after all non-document IDockContent, show document IDockContent
                for (int i = 0; i < DockContents.Length; i++)
                {
                    DockContent content = dockPanel.DockContents[sortedContents[i]];
                    //if (content.Page != null && content.Page.DockState == DockState.Document)
                    //{
                    //    //content.DockHandler.IsHidden = DockContents[sortedContents[i]].IsHidden;
                    //}
                }

                for (int i = 0; i < DockPages.Length; i++)
                    dockPanel.DockPages[i].ActiveContent = DockPages[i].IndexActiveContent == -1 ? null : dockPanel.DockContents[DockPages[i].IndexActiveContent];

                if (dockPanelStruct.IndexActiveDocumentPane != -1)
                {
                    //dockPanel.DockPages[dockPanelStruct.IndexActiveDocumentPane].Activate();
                    dockPanel.ActiveDocumentPage = dockPanel.DockPages[dockPanelStruct.IndexActiveDocumentPane]; 
                }

                if (dockPanelStruct.IndexActivePane != -1)
                {
                    //dockPanel.DockPages[dockPanelStruct.IndexActivePane].Activate();
                    dockPanel.ActivePage = dockPanel.DockPages[dockPanelStruct.IndexActivePane];
                }

                //for (int i = dockPanel.DockContents.Count - 1; i >= 0; i--)
                //    if (dockPanel.DockContents[i] is DummyContent)
                //        dockPanel.DockContents[i].DockHandler.Form.Close();

                dockPanel.ResumeLayout(true, true);
            }

            private static bool MoveToNextElement(XmlTextReader xmlIn)
            {
                if (!xmlIn.Read())
                    return false;

                while (xmlIn.NodeType == XmlNodeType.EndElement)
                {
                    if (!xmlIn.Read())
                        return false;
                }

                return true;
            }

            private static bool IsFormatVersionValid(string formatVersion)
            {
                if (formatVersion == ConfigFileVersion)
                    return true;

                foreach (string s in CompatibleConfigFileVersions)
                    if (s == formatVersion)
                        return true;

                return false;
            }
        }

        public void SaveAsXml(string fileName)
        {
            Persistor.SaveAsXml(this, fileName);
        }

        public void SaveAsXml(string fileName, Encoding encoding)
        {
            Persistor.SaveAsXml(this, fileName, encoding);
        }

        public void SaveAsXml(Stream stream, Encoding encoding)
        {
            Persistor.SaveAsXml(this, stream, encoding);
        }

        public void SaveAsXml(Stream stream, Encoding encoding, bool upstream)
        {
            Persistor.SaveAsXml(this, stream, encoding, upstream);
        }

        public void LoadFromXml(string fileName, DeserializeDockContent deserializeContent)
        {
            Persistor.LoadFromXml(this, fileName, deserializeContent);
        }

        public void LoadFromXml(Stream stream, DeserializeDockContent deserializeContent)
        {
            Persistor.LoadFromXml(this, stream, deserializeContent);
        }

        public void LoadFromXml(Stream stream, DeserializeDockContent deserializeContent, bool closeStream)
        {
            Persistor.LoadFromXml(this, stream, deserializeContent, closeStream);
        }
    }
}
