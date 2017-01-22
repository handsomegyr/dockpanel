using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Guoyongrong.WinFormsUI.Docking;
using System.IO;
namespace WindowsFormsApplication1
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();

            m_deserializeDockContent = new Guoyongrong.WinFormsUI.Docking.DockPanel.DeserializeDockContent(GetContentFromPersistString);
        }

        DummyDoc doc1 = new DummyDoc();
        DummyDoc doc2 = new DummyDoc();
        DummyDoc doc3 = new DummyDoc();

        DummyTaskList tasklist = new DummyTaskList();
        DummySolutionExplorer solutionExplorer = new DummySolutionExplorer();

        private void Form2_Load(object sender, EventArgs e)
        {
            
            string configFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DockPanel.config");

            if (File.Exists(configFile))
                dockPanel1.LoadFromXml(configFile, m_deserializeDockContent);
        }

        private void removeDocToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void removeTaskToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void removeSolutionToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }
        private DummyTaskList tl = new DummyTaskList();
        private DummySolutionExplorer t2 = new DummySolutionExplorer();
        private DummyPropertyWindow t3 = new DummyPropertyWindow();
        private DummyToolbox t4 = new DummyToolbox();

        private DummyOutputWindow t5 = new DummyOutputWindow();
        private DummySolutionExplorer t6 = new DummySolutionExplorer();
        private DummyPropertyWindow t7 = new DummyPropertyWindow();
        private DummyToolbox t8 = new DummyToolbox();


        private DummyOutputWindow t9 = new DummyOutputWindow();
        private DummySolutionExplorer t10 = new DummySolutionExplorer();
        private DummyPropertyWindow t11 = new DummyPropertyWindow();
        private DummyToolbox t12 = new DummyToolbox();

        private void initialToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doc1.Text = "Document1";
            doc2.Text = "Document2";
            doc3.Text = "Document3";

            ////DockPage FloatAt test
            //dockPanel1.SuspendLayout(true);
            //dockPanel1.Show(tl, DockStyle.Left);
            //dockPanel1.Show(t2, tl, DockAlignment.Right);
            //dockPanel1.Show(t3, t2, DockAlignment.Fill);
            //dockPanel1.Show(t4, t2, DockAlignment.Fill);

            //dockPanel1.Show(t5, DockState.Float);
            //dockPanel1.Show(t6, t5, DockAlignment.Right);

            //dockPanel1.Show(doc1, new Rectangle(100, 100, 600, 600));
            //dockPanel1.Show(doc2, doc1, DockAlignment.Fill);
            //dockPanel1.Show(doc3, doc1, DockAlignment.Right, 0.8);
            //dockPanel1.ResumeLayout(true, true);


            //DockContent FloatAt test
            dockPanel1.SuspendLayout(true);
            dockPanel1.Show(tl, DockState.DockLeft);
            dockPanel1.Show(t2, tl, DockAlignment.Right);
            dockPanel1.Show(t3, t2, DockAlignment.Fill);
            dockPanel1.Show(t4, t2, DockAlignment.Fill);

            dockPanel1.Show(t5, DockState.Float);
            dockPanel1.Show(t6, t5, DockAlignment.Fill);
            dockPanel1.Show(t7, t5, DockAlignment.Fill);

            //dockPanel1.Show(doc1, new Rectangle(100, 100, 600, 600));
            //dockPanel1.Show(doc2, doc1, DockAlignment.Fill);
            //dockPanel1.Show(doc3, doc1, DockAlignment.Right, 0.8);

            //dockPanel1.Show(t9, new Rectangle(100, 100, 600, 600));
            //dockPanel1.Show(t10, t9, DockAlignment.Right);
            //dockPanel1.Show(t11, t10, DockAlignment.Fill);
            //dockPanel1.Show(t12, t10, DockAlignment.Fill);

            dockPanel1.ResumeLayout(true, true);

        }

        private void showdialogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void showCaptionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Form3 f1 = new Form3();
            //f1.Show();
        }

        private bool m_bSaveLayout = false;
        private Guoyongrong.WinFormsUI.Docking.DockPanel.DeserializeDockContent m_deserializeDockContent;

        private void MainForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string configFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DockPanel.config");
            if (m_bSaveLayout)
                dockPanel1.SaveAsXml(configFile);
            else if (File.Exists(configFile))
                File.Delete(configFile);
        }

        private IContent GetContentFromPersistString(string persistString)
        {
            if (persistString == typeof(DummySolutionExplorer).ToString())
                return t2;
            else if (persistString == typeof(DummyPropertyWindow).ToString())
                return t3;
            else if (persistString == typeof(DummyToolbox).ToString())
                return t4;
            else if (persistString == typeof(DummyOutputWindow).ToString())
                return t5;
            else if (persistString == typeof(DummyTaskList).ToString())
                return tl;
            else
            {
                string[] parsedStrings = persistString.Split(new char[] { ',' });
                if (parsedStrings.Length != 3)
                    return null;

                if (parsedStrings[0] != typeof(DummyDoc).ToString())
                    return null;

                DummyDoc dummyDoc = new DummyDoc();
                if (parsedStrings[1] != string.Empty)
                    dummyDoc.FileName = parsedStrings[1];
                if (parsedStrings[2] != string.Empty)
                    dummyDoc.Text = parsedStrings[2];

                return dummyDoc;
            }
        }

        private void menuItemNewWindow_Click(object sender, EventArgs e)
        {
            MainForm newWindow = new MainForm();
            newWindow.Text = newWindow.Text + " - New";
            newWindow.Show();
        }

    }
}
