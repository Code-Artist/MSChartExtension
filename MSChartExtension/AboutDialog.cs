using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;

namespace System.Windows.Forms.DataVisualization.Charting
{
    internal partial class AboutDialog : Form
    {
        public string WebsiteLink { get; set; }
        public string FacebookLink { get; set; }
        public string GitHubLink { get; set; }

        public AboutDialog(string appName)
        {
            InitializeComponent();
            Text += " " + appName;
            lbVersion.Text = "V" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

            //AssemblyName[] assemblyNames = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
            //foreach (AssemblyName assembly in assemblyNames)
            //{
            //    TbComponents.Rows.Add(new string[] { assembly.Name, assembly.Version.ToString() });
            //}
            //TbComponents.Sort(TbComponents.Columns[0], ListSortDirection.Ascending);
        }

        private void IconClick(object sender, EventArgs e)
        {
            if (sender == bloggerIcon) Process.Start(WebsiteLink);
            else if (sender == facebookIcon) Process.Start(FacebookLink);
            else if (sender == githubIcon) Process.Start(GitHubLink);
        }

        private void AboutDialog_Shown(object sender, EventArgs e)
        {
            bloggerIcon.Visible = !string.IsNullOrEmpty(WebsiteLink);
            facebookIcon.Visible = !string.IsNullOrEmpty(FacebookLink);
        }
    }
}
