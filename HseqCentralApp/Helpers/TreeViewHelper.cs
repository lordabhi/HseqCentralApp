using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using DevExpress.Web;
using HseqCentralApp.Models;

namespace HseqCentralApp.Helpers
{
    public class TreeViewHelper
    {
        const string FileImageUrl = "~/Content/TreeView/FileSystem/file.png";
        const string DirImageUrl = "~/Content/TreeView/FileSystem/directory.png";
        static HttpRequest Request { get { return HttpContext.Current.Request; } }

        public static void CreateChildren2(TreeViewVirtualModeCreateChildrenEventArgs e)
        {
            string parentNodePath = string.IsNullOrEmpty(e.NodeName) ? Request.MapPath("~/") : e.NodeName;
            List<TreeViewVirtualNode> children = new List<TreeViewVirtualNode>();
            if (Directory.Exists(parentNodePath))
            {
                foreach (string childPath in Directory.GetDirectories(parentNodePath))
                {
                    string childDirName = Path.GetFileName(childPath);
                    if (IsSystemName(childDirName))
                        continue;
                    TreeViewVirtualNode childNode = new TreeViewVirtualNode(childPath, childDirName);
                    childNode.Image.Url = DirImageUrl;
                    children.Add(childNode);
                }
                foreach (string childPath in Directory.GetFiles(parentNodePath))
                {
                    string childFileName = Path.GetFileName(childPath);
                    if (IsSystemName(childFileName))
                        continue;
                    TreeViewVirtualNode childNode = new TreeViewVirtualNode(childPath, childFileName);
                    childNode.IsLeaf = true;
                    childNode.Image.Url = FileImageUrl;
                    children.Add(childNode);
                }
            }
            e.Children = children;
        }

        public static void CreateChildren(TreeViewVirtualModeCreateChildrenEventArgs e)
        {
            string parentNodePath = string.IsNullOrEmpty(e.NodeName) ? Request.MapPath("~/") : e.NodeName;
            List<TreeViewVirtualNode> children = new List<TreeViewVirtualNode>();
            var discrepancyTypes = Utils.DiscrepancyTypes();

            if (string.IsNullOrEmpty(e.NodeName))
            {
                var nod = new TreeViewVirtualNode("NCR", "NCR");
                children.Add(nod);

                nod = new TreeViewVirtualNode("CAR", "CAR");
                children.Add(nod);

                nod = new TreeViewVirtualNode("FIS", "FIS");
                children.Add(nod);

                nod = new TreeViewVirtualNode("PAR", "PAR");
                children.Add(nod);

            }
            else if (e.NodeName == "NCR")
            {
                foreach (var u in Utils.DiscrepancyTypes()) {
            
                    TreeViewVirtualNode childNode = new TreeViewVirtualNode(u.DiscrepancyTypeID.ToString(), u.Name) { IsLeaf = false };
                    children.Add(childNode);
                }
            }
            e.Children = children;
        }

        public static void RecordTypes(TreeViewVirtualModeCreateChildrenEventArgs e)
        {
            List<TreeViewVirtualNode> children = new List<TreeViewVirtualNode>();

            if (e.NodeName == null)
            {

                TreeViewVirtualNode allRecords = new TreeViewVirtualNode("All Records", "All Records");
                //allRecords.Checked = true;
                children.Add(allRecords);
            }

            if (e.NodeName == "All Records")
            {
                var recordTypes = Enum.GetValues(typeof(RecordType)).Cast<RecordType>();
                foreach (var recordType in recordTypes)
                {
                    children.Add(new TreeViewVirtualNode(recordType.ToString(), recordType.ToString())
                    {
                        IsLeaf = true,
                        Checked = recordType == RecordType.NCR ? true : false
                    });
                }
            }
            e.Children = children;
        }

        public static void ResponsibleAreaTypes(TreeViewVirtualModeCreateChildrenEventArgs e)
        {
            string parentNodePath = string.IsNullOrEmpty(e.NodeName) ? Request.MapPath("~/") : e.NodeName;
            List<TreeViewVirtualNode> children = new List<TreeViewVirtualNode>();
            var responsibleAreaTypes = Utils.ResponsibleAreas();

            if (e.NodeName == null)
            {
                children.Add(new TreeViewVirtualNode("All Responsible Areas", "All Responsible Areas")
                {
                   // Checked = true
                });
            }

            if (e.NodeName == "All Responsible Areas")
            {
                foreach (var recordType in responsibleAreaTypes)
                {

                    children.Add(new TreeViewVirtualNode(recordType.BusinessAreaID.ToString(), recordType.Name)
                    {
                        IsLeaf = true
                    });
                }
            }
            e.Children = children;
        }

        public static void CoordinatorTypes(TreeViewVirtualModeCreateChildrenEventArgs e)
        {
            string parentNodePath = string.IsNullOrEmpty(e.NodeName) ? Request.MapPath("~/") : e.NodeName;
            List<TreeViewVirtualNode> children = new List<TreeViewVirtualNode>();
            var coordinators = Utils.AppUsers();

            if (e.NodeName == null)
            {
                TreeViewVirtualNode allNodes = new TreeViewVirtualNode("All Coordinators", "All Coordinators") 
                { 
                    //Checked = true
                };

                children.Add(allNodes);
            }

            if (e.NodeName == "All Coordinators")
            {
                foreach (var recordType in coordinators)
                {

                    children.Add(new TreeViewVirtualNode(recordType.HseqUserID.ToString(), recordType.FullName)
                    {
                        IsLeaf = true
                    });
                }
            }
            e.Children = children;
        }

        static bool IsSystemName(string name)
        {
            name = name.ToLower();
            return name.StartsWith("app_") || name == "bin" || name == "obj";
        }
    }
}