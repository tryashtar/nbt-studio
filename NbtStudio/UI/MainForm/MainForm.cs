using fNbt;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Aga.Controls.Tree;
using System.Diagnostics;
using TryashtarUtils.Utility;
using TryashtarUtils.Nbt;
using TryashtarUtils.Forms;

namespace NbtStudio.UI
{
    public partial class MainForm : Form
    {
        private IconSource IconSource;
        private readonly Studio App;
        private readonly Updater UpdateChecker = new();
        private UndoHistory UndoHistory => App.UndoHistory;

        public readonly MainFormEditors Editors;

        public MainForm(Studio application)
        {
            App = application;
            Editors = new(() => App, () => this, () => NbtTree, () => IconSource);

            // add controls
            InitializeComponent();

            NbtTree.Font = new Font(NbtTree.Font.FontFamily, Properties.Settings.Default.TreeZoom);
            NbtTree.Model = App.Tree;

            OpenFilesSource = new FormNodeSource(() => App.Tree.GetFiles());
            SelectedNodesSource = new FormNodeSource(() => NbtTree.SelectedModelNodes);
            App.Tree.NodesInserted += (s, e) => { OpenFilesSource.NoticeChanges(); SelectedNodesSource.NoticeChanges(); };
            App.Tree.NodesRemoved += (s, e) => { OpenFilesSource.NoticeChanges(); SelectedNodesSource.NoticeChanges(); };
            NbtTree.SelectionChanged += (s,e) => SelectedNodesSource.NoticeChanges();

            AddActionButtons();

            IconSetWindow.TryImportSources(Properties.Settings.Default.CustomIconSets, this);
            SetIconSource(IconSourceRegistry.FromID(Properties.Settings.Default.IconSet));
#if !DEBUG
            CheckForUpdatesInBackground();
#endif
        }

        private void CheckForUpdatesInBackground()
        {
            UpdateChecker.StartCheckingAsync();
            UpdateChecker.ContinueWith(x =>
            {
                if (!x.Failed)
                    UpdateButton.Visible = true;
            });
        }

        private void CheckForUpdates()
        {
            // TO DO: calling this repeatedly shouldn't spam the window multiple times (likewise for CheckForUpdatesInBackground)
            UpdateChecker.StartCheckingAsync();
            UpdateChecker.ContinueWith(x =>
            {
                if (x.Failed)
                {
                    var window = new ExceptionWindow("Update check failed",
                        "Failed to check for updates.", x,
                        "Would you like to go to the update page?\n" +
                        Updater.GitHubUrl(),
                        ExceptionWindowButtons.OKCancel
                    );
                    window.ShowDialog(this);
                    if (window.DialogResult == DialogResult.OK)
                        IOUtils.OpenUrlInBrowser(Updater.GitHubUrl());
                }
                else
                {
                    if (x.Result is null)
                    {
                        if (MessageBox.Show("You already seem to have the latest update.\n" +
                            "Would you like to go to the update page?\n\n" +
                            Updater.GitHubUrl(),
                            "No update found", MessageBoxButtons.OKCancel) == DialogResult.OK)
                            IOUtils.OpenUrlInBrowser(Updater.GitHubUrl());
                    }
                    else
                    {
                        UpdateButton.Visible = true;
                        ShowUpdate(x.Result);
                    }
                }
            });
        }

        private void SetIconSource(IconSource source)
        {
            IconSource = source;
            ButtonsCollection.SetIconSource(source);
            NbtTree.SetIconSource(source);
            NbtTree.Refresh();
            this.Icon = source.GetImage(IconType.NbtStudio).Icon;
            Properties.Settings.Default.IconSet = IconSourceRegistry.GetID(source);
        }

        private void OpenRecentFile()
        {
            var files = Properties.Settings.Default.RecentFiles;
            if (files.Count >= 1)
                Editors.OpenFiles(files.GetFirst());
        }

        private void NbtTree_NodeMouseDoubleClick(object sender, TreeNodeAdvMouseEventArgs e)
        {
            var node = NbtTree.ModelNodeFromClick(e);
            if (!e.Node.CanExpand)
                Editors.Edit(node);
        }

        private void NbtTree_ItemDrag(object sender, ItemDragEventArgs e)
        {
            DoDragDrop(NbtTree.SelectedModelNodes.ToArray(), DragDropEffects.Move);
        }

        private void NbtTree_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = Control.ModifierKeys == Keys.Shift ? DragDropEffects.Copy : DragDropEffects.Move;
            else
            {
                var tags = NbtTree.ModelNodesFromDrag(e);
                var drop = NbtTree.DropModelNode;
                if (tags.Any()
                    && NbtTree.DropModelNode is not null
                    && CanMoveObjects(tags, drop, NbtTree.DropPosition.Position))
                    e.Effect = e.AllowedEffect;
                else
                    e.Effect = DragDropEffects.None;
            }
        }

        private void NbtTree_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (e.Effect == DragDropEffects.Move)
                    Editors.OpenFiles(files);
                else if (e.Effect == DragDropEffects.Copy)
                    Editors.ImportFiles(files);
            }
            else
            {
                var nodes = NbtTree.ModelNodesFromDrag(e);
                var drop = NbtTree.DropModelNode;
                if (nodes.Any())
                    MoveObjects(nodes, drop, NbtTree.DropPosition.Position);
            }
        }

        private bool CanMoveObjects(IEnumerable<Node> nodes, Node target, NodePosition position)
        {
            return false;

            // var (destination, index) = App.Tree.GetInsertionLocation(target, position);
            // if (destination is null) return false;
            // return destination.CanReceiveDrop(nodes);
        }

        private void MoveObjects(IEnumerable<Node> nodes, Node target, NodePosition position)
        {
            //var (destination, index) = App.Tree.GetInsertionLocation(target, position);
            //if (destination is null) return;
            //UndoHistory.StartBatchOperation();
            //destination.ReceiveDrop(nodes, index);
            //UndoHistory.FinishBatchOperation(new DescriptionHolder("Move {0} into {1} at position {2}", nodes, destination, index), true);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
#if !DEBUG
            if (!ConfirmIfUnsaved("Exit anyway?"))
                e.Cancel = true;
#endif
        }

        private void SetAllSelected(IEnumerable<TreeNodeAdv> nodes, bool selected)
        {
            foreach (var node in nodes)
            {
                node.IsSelected = selected;
            }
        }

        private ContextMenuStrip CreateContextMenu(TreeNodeAdvMouseEventArgs e)
        {
            return null;
            //var menu = new ContextMenuStrip();
            //var obj = NbtTree.ModelNodeFromClick(e);
            //var root_items = new List<ToolStripItem>();
            //var node_items = new List<ToolStripItem>();
            //var file_items = new List<ToolStripItem>();
            //var nbt_items = new List<ToolStripItem>();
            //if (App.Tree.RootNodes.Contains(obj))
            //    root_items.Add(new ToolStripMenuItem("&Discard", IconSource.GetImage(IconType.Delete).Image, Discard_Click));
            //if (e.Node.CanExpand)
            //{
            //    if (e.Node.IsExpanded)
            //        node_items.Add(new ToolStripMenuItem("&Collapse", null, Collapse_Click));
            //    else
            //        node_items.Add(new ToolStripMenuItem("&Expand All", null, ExpandAll_Click));
            //    var children = NbtTree.AllChildren(e.Node);
            //    if (children.All(x => x.IsSelected))
            //        node_items.Add(new ToolStripMenuItem("Dese&lect all Children", null, DeselectChildren_Click));
            //    else
            //        node_items.Add(new ToolStripMenuItem("Se&lect all Children", null, SelectChildren_Click));
            //}
            //var saveable = obj.Get<ISaveable>();
            //if (saveable is not null && saveable.CanSave)
            //    file_items.Add(new ToolStripMenuItem("&Save File", IconSource.GetImage(IconType.Save).Image, Save_Click));
            //if (obj.Get<IExportable>() is not null)
            //    file_items.Add(new ToolStripMenuItem("Save File &As", IconSource.GetImage(IconType.Save).Image, SaveAs_Click));
            //var refresh = obj.Get<IRefreshable>();
            //if (refresh is not null && refresh.CanRefresh)
            //    file_items.Add(new ToolStripMenuItem("&Refresh", IconSource.GetImage(IconType.Refresh).Image, Refresh_Click));
            //var path = obj.Get<IHavePath>();
            //if (path is not null && path.Path is not null)
            //    file_items.Add(new ToolStripMenuItem("&Open in Explorer", IconSource.GetImage(IconType.OpenFile).Image, OpenInExplorer_Click));
            //var container = obj.GetNbtTag() as NbtContainerTag;
            //if (container is not null)
            //{
            //    var addable = NbtUtil.NormalTagTypes().Where(x => container.CanAdd(x));
            //    bool single = ListUtils.ExactlyOne(addable);
            //    var display = single ? (Func<NbtTagType, string>)(x => $"Add {NbtUtil.TagTypeName(x)} Tag") : (x => $"{NbtUtil.TagTypeName(x)} Tag");
            //    var items = addable.Select(x => new ToolStripMenuItem(display(x), NbtUtil.TagTypeImage(IconSource, x).Image, (s, ea) => AddTag_Click(x))).ToArray();
            //    if (single)
            //        nbt_items.AddRange(items);
            //    else
            //    {
            //        var add = new ToolStripMenuItem("Add...");
            //        add.DropDownItems.AddRange(items);
            //        nbt_items.Add(add);
            //    }
            //}
            //AddMenuSections(menu.Items, root_items, node_items, file_items, nbt_items);
            //return menu;
        }

        private void AddMenuSections(ToolStripItemCollection collection, params IEnumerable<ToolStripItem>[] sources)
        {
            for (int i = 0; i < sources.Length - 1; i++)
            {
                collection.AddRange(sources[i].ToArray());
                if (sources[i].Any())
                    collection.Add(new ToolStripSeparator());
            }
            collection.AddRange(sources[^1].ToArray());
        }

        private void Discard_Click(object sender, EventArgs e)
        {
            // var selected_roots = NbtTree.SelectedModelNodes.Where(x => x.Parent is ModelRootNode);
            // Discard(selected_roots);
        }

        private void Collapse_Click(object sender, EventArgs e)
        {
            var selected = NbtTree.SelectedNodes;
            foreach (var node in selected)
            {
                node.CollapseAll();
            }
        }

        private void ExpandAll_Click(object sender, EventArgs e)
        {
            var selected = NbtTree.SelectedNodes;
            foreach (var node in selected)
            {
                node.ExpandAll();
            }
        }

        private void SelectChildren_Click(object sender, EventArgs e)
        {
            var selected = NbtTree.SelectedNodes.ToList();
            foreach (var node in selected)
            {
                SetAllSelected(NbtTree.AllChildren(node), true);
            }
        }

        private void DeselectChildren_Click(object sender, EventArgs e)
        {
            var selected = NbtTree.SelectedNodes.ToList();
            foreach (var node in selected)
            {
                SetAllSelected(NbtTree.AllChildren(node), false);
            }
        }

        private void NbtTree_NodeMouseClick(object sender, TreeNodeAdvMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var menu = CreateContextMenu(e);
                menu.Show(NbtTree, e.Location);
                e.Handled = true;
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.TreeZoom = (int)NbtTree.Font.Size;
            var icon_sets = Properties.Settings.Default.CustomIconSets.Cast<string>().Distinct().ToArray();
            Properties.Settings.Default.CustomIconSets.Clear();
            Properties.Settings.Default.CustomIconSets.AddRange(icon_sets);
            Properties.Settings.Default.Save();
        }

        private void MenuEdit_DropDownOpening(object sender, EventArgs e)
        {
            var undo_history = UndoHistory.GetUndoHistory();
            var redo_history = UndoHistory.GetRedoHistory();

            var undo_dropdown = new ToolStripDropDown();
            DropDownUndoHistory.DropDown = undo_dropdown;
            var undo_actions = new ActionHistory(undo_history,
                x => { UndoHistory.Undo(x + 1); MenuEdit.HideDropDown(); },
                x => $"Undo {StringUtils.Pluralize(x + 1, "action")}",
                DropDownUndoHistory.Font);
            undo_dropdown.Items.Add(new ToolStripControlHost(undo_actions));

            var redo_dropdown = new ToolStripDropDown();
            DropDownRedoHistory.DropDown = redo_dropdown;
            var redo_actions = new ActionHistory(redo_history,
                x => { UndoHistory.Redo(x + 1); MenuEdit.HideDropDown(); },
                x => $"Redo {StringUtils.Pluralize(x + 1, "action")}",
                DropDownRedoHistory.Font);
            redo_dropdown.Items.Add(new ToolStripControlHost(redo_actions));

            DropDownUndoHistory.Enabled = undo_history.Any();
            DropDownRedoHistory.Enabled = redo_history.Any();
            //ActionClearUndoHistory.Enabled = undo_history.Any() || redo_history.Any();
        }

        private void MenuFile_DropDownOpening(object sender, EventArgs e)
        {
            //ActionNewClipboard.Enabled = Clipboard.ContainsFileDropList() || Clipboard.ContainsText();
        }

        private ToolStripMenuItem RecentEntry(string path)
        {
            return null;
            //bool directory = Directory.Exists(path);
            //Image image;
            //EventHandler click;
            //if (directory)
            //{
            //    image = IconSource.GetImage(IconType.Folder).Image;
            //    click = (s, e) => OpenFolder(path);
            //}
            //else
            //{
            //    if (!File.Exists(path))
            //        return null;
            //    image = IconSource.GetImage(IconType.File).Image;
            //    click = (s, e) => OpenFiles(new[] { path });
            //}
            //return new ToolStripMenuItem(path, image, click);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                Editors.Edit();
                return true;
            }
            if (keyData == (Keys.Control | Keys.Shift | Keys.T))
            {
                OpenRecentFile();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
