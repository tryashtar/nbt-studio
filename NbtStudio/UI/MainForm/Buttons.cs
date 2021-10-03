using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TryashtarUtils.Utility;

namespace NbtStudio.UI
{
    public partial class MainForm
    {
        private readonly DualItemCollection ButtonsCollection = new();
        // TO DO: a bit annoying that this is required to be a field
        private DualMenuItem UpdateButton;
        private DualMenuItem DropDownUndoHistory;
        private DualMenuItem DropDownRedoHistory;

        private readonly FormNodeSource OpenFilesSource;
        private readonly FormNodeSource SelectedNodesSource;

        private class FormNodeSource : NodeSource
        {
            public readonly Func<IEnumerable<Node>> NodeGetter;
            public FormNodeSource(Func<IEnumerable<Node>> getter)
            {
                NodeGetter = getter;
            }

            public override IEnumerable<Node> GetNodes()
            {
                return NodeGetter();
            }

            public void NoticeChanges()
            {
                InvokeChanged();
            }
        }

        private void AddActionButtons()
        {
            AddButton(
                no_context_editor: Editors.New(),
                text: "&New",
                hover: "New File",
                icon: IconType.NewFile,
                shortcut: Keys.Control | Keys.N,
                strip: Tools,
                menu: MenuFile
            );
            AddButton(
                no_context_editor: Editors.NewPaste(),
                text: "New from &Clipboard",
                icon: IconType.Paste,
                shortcut: Keys.Control | Keys.Alt | Keys.V,
                menu: MenuFile
            );
            AddButton(
                no_context_editor: Editors.NewRegion(),
                text: "New &Region File",
                icon: IconType.Region,
                shortcut: Keys.Control | Keys.Alt | Keys.R,
                menu: MenuFile
            );
            MenuFile.DropDownItems.Add(new ToolStripSeparator());
            AddButton(
                no_context_editor: Editors.OpenFile(),
                text: "&Open File",
                hover: "Open File",
                icon: IconType.OpenFile,
                shortcut: Keys.Control | Keys.O,
                strip: Tools,
                menu: MenuFile
            );
            AddButton(
                no_context_editor: Editors.OpenFolder(),
                text: "Open &Folder",
                hover: "Open Folder",
                icon: IconType.OpenFolder,
                shortcut: Keys.Control | Keys.Shift | Keys.O,
                strip: Tools,
                menu: MenuFile
            );
            var import = AddButton(
                text: "&Import...",
                menu: MenuFile
            );
            AddButton(
                no_context_editor: Editors.ImportFile(),
                text: "&File",
                icon: IconType.OpenFile,
                shortcut: Keys.Control | Keys.I,
                parent: import
            );
            AddButton(
                no_context_editor: Editors.ImportFolder(),
                text: "F&older",
                icon: IconType.OpenFolder,
                shortcut: Keys.Control | Keys.Shift | Keys.I,
                parent: import
            );
            AddButton(
                no_context_editor: Editors.ImportNew(),
                text: "&New File",
                icon: IconType.NewFile,
                shortcut: Keys.Control | Keys.Alt | Keys.N,
                parent: import
            );
            AddButton(
                no_context_editor: Editors.ImportNewRegion(),
                text: "New &Region File",
                icon: IconType.Region,
                parent: import
            );
            AddButton(
                no_context_editor: Editors.ImportPaste(),
                text: "From &Clipboard",
                icon: IconType.Paste,
                shortcut: Keys.Control | Keys.Alt | Keys.I,
                parent: import
            );
            MenuFile.DropDownItems.Add(new ToolStripSeparator());
            AddButton(
                editor: Editors.Save(),
                source: OpenFilesSource,
                text: "&Save",
                hover: "Save",
                context_icon: x => x.CountGreaterThan(1) ? IconType.SaveAll : IconType.Save,
                shortcut: Keys.Control | Keys.S,
                strip: Tools,
                menu: MenuFile
            );
            AddButton(
                editor: Editors.SaveAs(),
                source: OpenFilesSource,
                text: "Save &As",
                context_icon: x => x.CountGreaterThan(1) ? IconType.SaveAll : IconType.Save,
                shortcut: Keys.Control | Keys.Shift | Keys.S,
                menu: MenuFile
            );
            MenuFile.DropDownItems.Add(new ToolStripSeparator());
            AddButton(
                text: "&Recent...",
                menu: MenuFile
            );
            AddButton(
                editor: Editors.RefreshFiles(),
                source: OpenFilesSource,
                hover: "Refresh",
                icon: IconType.Refresh,
                strip: Tools
            );
            Tools.Items.Add(new ToolStripSeparator());
            AddButton(
                simple_action: () => UndoHistory.Undo(),
                text: "&Undo",
                icon: IconType.Undo,
                shortcut: Keys.Control | Keys.Z,
                menu: MenuEdit
            );
            AddButton(
                simple_action: () => UndoHistory.Redo(),
                text: "&Redo",
                icon: IconType.Redo,
                shortcut: Keys.Control | Keys.Shift | Keys.Z,
                menu: MenuEdit
            );
            MenuEdit.DropDownItems.Add(new ToolStripSeparator());
            AddButton(
                //editor: Editors.Cut(),
                source: SelectedNodesSource,
                text: "Cu&t",
                hover: "Cut",
                icon: IconType.Cut,
                shortcut: Keys.Control | Keys.X,
                strip: Tools,
                menu: MenuEdit
            );
            AddButton(
                //editor: Editors.Copy(),
                source: SelectedNodesSource,
                text: "&Copy",
                hover: "Copy",
                icon: IconType.Copy,
                shortcut: Keys.Control | Keys.C,
                strip: Tools,
                menu: MenuEdit
            );
            AddButton(
                //editor: Editors.Paste(),
                source: SelectedNodesSource,
                text: "&Paste",
                hover: "Paste",
                icon: IconType.Paste,
                shortcut: Keys.Control | Keys.V,
                strip: Tools,
                menu: MenuEdit
            );
            MenuEdit.DropDownItems.Add(new ToolStripSeparator());
            Tools.Items.Add(new ToolStripSeparator());
            AddButton(
                editor: Editors.Rename(),
                source: SelectedNodesSource,
                text: "Re&name",
                hover: "Rename",
                icon: IconType.Rename,
                shortcut: Keys.F2,
                strip: Tools,
                menu: MenuEdit
            );
            AddButton(
                editor: Editors.Edit(),
                source: SelectedNodesSource,
                text: "&Edit Value",
                hover: "Edit",
                icon: IconType.Edit,
                shortcut: Keys.Control | Keys.E,
                strip: Tools,
                menu: MenuEdit
            );
            AddButton(
                //editor: Editors.EditSnbt(),
                source: SelectedNodesSource,
                text: "Edit as &SNBT",
                hover: "Edit as SNBT",
                icon: IconType.EditSnbt,
                shortcut: Keys.Control | Keys.Shift | Keys.E,
                strip: Tools,
                menu: MenuEdit
            );
            AddButton(
                //editor: Editors.Delete(),
                source: SelectedNodesSource,
                text: "&Delete",
                hover: "Delete",
                icon: IconType.Delete,
                shortcut: Keys.Delete,
                strip: Tools,
                menu: MenuEdit
            );
            AddButton(
                //editor: Editors.Sort(),
                source: SelectedNodesSource,
                hover: "Sort",
                icon: IconType.Sort,
                strip: Tools
            );
            MenuEdit.DropDownItems.Add(new ToolStripSeparator());
            Tools.Items.Add(new ToolStripSeparator());
            DropDownUndoHistory = AddButton(
                text: "Undo History...",
                icon: IconType.Undo,
                menu: MenuEdit
            );
            DropDownRedoHistory = AddButton(
                text: "Redo History...",
                icon: IconType.Redo,
                menu: MenuEdit
            );
            AddButton(
                //action: ClearUndoHistory,
                text: "Clear Undo History",
                menu: MenuEdit
            );
            MakeCreateTagButtons();
            AddButton(
                //action: AddSnbt,
                hover: "Add as SNBT",
                icon: IconType.AddSnbt,
                strip: Tools
            );
            AddButton(
                //action: AddChunk,
                hover: "Add Chunk",
                icon: IconType.Chunk,
                strip: Tools
            );
            Tools.Items.Add(new ToolStripSeparator());
            AddButton(
                simple_action: Find,
                text: "&Find",
                hover: "Find",
                icon: IconType.Search,
                shortcut: Keys.Control | Keys.F,
                strip: Tools,
                menu: MenuSearch
            );
            AddButton(
                simple_action: SelectNbtPath,
                text: "&Select NBT by Path",
                icon: IconType.Search,
                shortcut: Keys.Control | Keys.Alt | Keys.F,
                menu: MenuSearch
            );
            AddButton(
                simple_action: About,
                text: "&About",
                icon: IconType.NbtStudio,
                shortcut: Keys.F1,
                menu: MenuHelp
            );
            AddButton(
                simple_action: ChangeIcons,
                text: "&Change Icons",
                icon: IconType.Refresh,
                shortcut: Keys.Control | Keys.I,
                menu: MenuHelp
            );
            AddButton(
                simple_action: ViewLog,
                text: "View &Log",
                icon: IconType.NewFile,
                menu: MenuHelp
            );
            MenuHelp.DropDownItems.Add(new ToolStripSeparator());
            UpdateButton = AddButton(
                simple_action: Update,
                text: "&Update",
                strip: MenuStrip
            );
            UpdateButton.Visible = false;
            AddButton(
                simple_action: CheckForUpdates,
                text: "Check for &Updates",
                shortcut: Keys.Control | Keys.U,
                menu: MenuHelp
            );
        }

        private void MakeCreateTagButtons()
        {
            foreach (var type in NbtUtil.NormalTagTypes())
            {
                AddButton(
                    hover: $"Add {NbtUtil.TagTypeName(type)} Tag",
                    icon: NbtUtil.TagIconType(type),
                    strip: Tools,
                    editor: Editors.AddTag(type),
                    source: SelectedNodesSource
                );
            }
        }

        public void RunEditor(Editor editor, IEnumerable<Node> nodes)
        {
            var command = editor.Edit(nodes);
            if (command is not null)
                UndoHistory.PerformAction(command);
            NbtTree.RefreshModelNodes(nodes.Select(x => x.Path));
        }

        public void RunEditor(ContextFreeEditor editor)
        {
            var command = editor.Edit();
            if (command is not null)
                UndoHistory.PerformAction(command);
            NbtTree.RefreshModelNodes(App.Tree.RootNodes.Select(x => x.Path));
        }

        private DualMenuItem AddButton(
            Editor editor = null,
            ContextFreeEditor no_context_editor = null,
            Action simple_action = null,
            NodeSource source = null,
            string text = null,
            string hover = null,
            IconType? icon = null,
            Func<IEnumerable<Node>, IconType?> context_icon = null,
            Keys? shortcut = null,
            ToolStrip strip = null,
            ToolStripMenuItem menu = null,
            DualMenuItem parent = null
            )
        {
            var button = new DualMenuItem(text, hover, icon, shortcut ?? Keys.None);
            if (context_icon is not null)
            {
                void changer() => button.IconType = context_icon(source.GetNodes());
                changer();
                source.Changed += (s, e) => changer();
            }
            if (editor is not null)
            {
                void enabler() => button.Enabled = editor.CanEdit(source.GetNodes());
                enabler();
                source.Changed += (s, e) => enabler();
                button.Click += (s, e) => RunEditor(editor, source.GetNodes());
            }
            if (no_context_editor is not null)
            {
                button.Click += (s, e) => RunEditor(no_context_editor);
            }
            if (simple_action is not null)
            {
                button.Click += (s, e) => simple_action();
            }
            if (strip != null)
                button.AddToToolStrip(strip);
            if (menu != null)
                button.AddToMenuItem(menu);
            if (parent != null)
                button.AddToDual(parent);
            ButtonsCollection.Add(button);
            return button;
        }

        private FindWindow FindWindow;
        private void Find()
        {
            if (FindWindow is null || FindWindow.IsDisposed)
                FindWindow = new FindWindow(IconSource, App.Tree, NbtTree);
            if (!FindWindow.Visible)
                FindWindow.Show(this);
            FindWindow.Focus();
        }

        private NbtPathWindow NbtPathWindow;
        private void SelectNbtPath()
        {
            if (NbtPathWindow is null || NbtPathWindow.IsDisposed)
                NbtPathWindow = new NbtPathWindow(IconSource, NbtTree);
            if (!NbtPathWindow.Visible)
                NbtPathWindow.Show(this);
            NbtPathWindow.Focus();
        }

        private AboutWindow AboutWindow;
        private void About()
        {
            if (AboutWindow is null || AboutWindow.IsDisposed)
                AboutWindow = new AboutWindow(IconSource);
            if (!AboutWindow.Visible)
                AboutWindow.Show(this);
            AboutWindow.Focus();
        }

        private IconSetWindow IconSetWindow;
        private void ChangeIcons()
        {
            if (IconSetWindow == null || IconSetWindow.IsDisposed)
            {
                IconSetWindow = new IconSetWindow(IconSource);
                IconSetWindow.FormClosed += (s, e) =>
                {
                    if (IconSetWindow.SelectedSource is not null)
                        SetIconSource(IconSetWindow.SelectedSource);
                };
            }
            if (!IconSetWindow.Visible)
                IconSetWindow.Show(this);
            IconSetWindow.Focus();
        }

        private UpdateWindow UpdateWindow;
        private void ShowUpdate(AvailableUpdate update)
        {
            if (update is null)
                return;
            if (UpdateWindow is null || UpdateWindow.IsDisposed)
                UpdateWindow = new UpdateWindow(IconSource, update);
            if (!UpdateWindow.Visible)
                UpdateWindow.Show(this);
            UpdateWindow.Focus();
        }

        private InfoWindow LogWindow;
        private void ViewLog()
        {
            if (LogWindow is null || LogWindow.IsDisposed)
            {
                var log = DebugLog.Get();
                LogWindow = new InfoWindow("Log",
                    $"Found {StringUtils.Pluralize(log.Count, "log entry", "log entries")}",
                    String.Join(Environment.NewLine, log)
                );
            }
            if (!LogWindow.Visible)
                LogWindow.Show(this);
            LogWindow.Focus();
        }
    }
}
