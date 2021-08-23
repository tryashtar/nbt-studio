using fNbt;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private void AddActionButtons()
        {
            bool has_files() => App.Tree.GetSaveables().Any();

            AddButton(
                action: () => New(),
                text: "&New",
                hover: "New File",
                icon: IconType.NewFile,
                shortcut: Keys.Control | Keys.N,
                strip: Tools,
                menu: MenuFile
            );
            AddButton(
                action: () => NewPaste(),
                text: "New from &Clipboard",
                icon: IconType.Paste,
                shortcut: Keys.Control | Keys.Alt | Keys.V,
                menu: MenuFile
            );
            AddButton(
                action: () => NewRegion(),
                text: "New &Region File",
                icon: IconType.Region,
                shortcut: Keys.Control | Keys.Alt | Keys.R,
                menu: MenuFile
            );
            MenuFile.DropDownItems.Add(new ToolStripSeparator());
            AddButton(
                action: () => OpenFile(),
                text: "&Open File",
                hover: "Open File",
                icon: IconType.OpenFile,
                shortcut: Keys.Control | Keys.O,
                strip: Tools,
                menu: MenuFile
            );
            AddButton(
                action: () => OpenFolder(),
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
                action: () => ImportFile(),
                text: "&File",
                icon: IconType.OpenFile,
                shortcut: Keys.Control | Keys.I,
                parent: import
            );
            AddButton(
                action: () => ImportFolder(),
                text: "F&older",
                icon: IconType.OpenFolder,
                shortcut: Keys.Control | Keys.Shift | Keys.I,
                parent: import
            );
            AddButton(
                action: () => ImportNew(),
                text: "&New File",
                icon: IconType.NewFile,
                shortcut: Keys.Control | Keys.Alt | Keys.N,
                parent: import
            );
            AddButton(
                action: () => ImportNewRegion(),
                text: "New &Region File",
                icon: IconType.Region,
                parent: import
            );
            AddButton(
                action: () => ImportClipboard(),
                text: "From &Clipboard",
                icon: IconType.Paste,
                shortcut: Keys.Control | Keys.Alt | Keys.I,
                parent: import
            );
            MenuFile.DropDownItems.Add(new ToolStripSeparator());
            AddButton(
                action: Save,
                text: "&Save",
                hover: "Save",
                icon: IconType.Save,
                shortcut: Keys.Control | Keys.S,
                enabled_when: has_files,
                enable_trigger: EnableTrigger.TreeChanged,
                strip: Tools,
                menu: MenuFile
            );
            AddButton(
                action: SaveAs,
                text: "Save &As",
                icon: IconType.Save,
                shortcut: Keys.Control | Keys.Shift | Keys.S,
                enabled_when: has_files,
                enable_trigger: EnableTrigger.TreeChanged,
                menu: MenuFile
            );
            MenuFile.DropDownItems.Add(new ToolStripSeparator());
            AddButton(
                text: "&Recent...",
                menu: MenuFile
            );
            AddButton(
                action: RefreshFiles,
                hover: "Refresh",
                icon: IconType.Refresh,
                enabled_when: has_files,
                enable_trigger: EnableTrigger.TreeChanged,
                strip: Tools
            );
            Tools.Items.Add(new ToolStripSeparator());
            AddButton(
                action: Undo,
                text: "&Undo",
                icon: IconType.Undo,
                shortcut: Keys.Control | Keys.Z,
                enabled_when: () => UndoHistory.CanUndo,
                enable_trigger: EnableTrigger.TreeChanged,
                menu: MenuEdit
            );
            AddButton(
                action: Redo,
                text: "&Redo",
                icon: IconType.Redo,
                shortcut: Keys.Control | Keys.Shift | Keys.Z,
                enabled_when: () => UndoHistory.CanRedo,
                enable_trigger: EnableTrigger.TreeChanged,
                menu: MenuEdit
            );
            MenuEdit.DropDownItems.Add(new ToolStripSeparator());
            AddButton(
                action: Cut,
                text: "Cu&t",
                hover: "Cut",
                icon: IconType.Cut,
                shortcut: Keys.Control | Keys.X,
                strip: Tools,
                menu: MenuEdit
            );
            AddButton(
                action: Copy,
                text: "&Copy",
                hover: "Copy",
                icon: IconType.Copy,
                shortcut: Keys.Control | Keys.C,
                strip: Tools,
                menu: MenuEdit
            );
            AddButton(
                action: Paste,
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
                action: Rename,
                text: "Re&name",
                hover: "Rename",
                icon: IconType.Rename,
                shortcut: Keys.F2,
                strip: Tools,
                menu: MenuEdit
            );
            AddButton(
                action: Edit,
                text: "&Edit Value",
                hover: "Edit",
                icon: IconType.Edit,
                shortcut: Keys.Control | Keys.E,
                strip: Tools,
                menu: MenuEdit
            );
            AddButton(
                action: EditSnbt,
                text: "Edit as &SNBT",
                hover: "Edit as SNBT",
                icon: IconType.EditSnbt,
                shortcut: Keys.Control | Keys.Shift | Keys.E,
                strip: Tools,
                menu: MenuEdit
            );
            AddButton(
                action: Delete,
                text: "&Delete",
                hover: "Delete",
                icon: IconType.Delete,
                shortcut: Keys.Delete,
                strip: Tools,
                menu: MenuEdit
            );
            AddButton(
                action: Sort,
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
                action: ClearUndoHistory,
                text: "Clear Undo History",
                menu: MenuEdit
            );
            var tag_buttons = MakeCreateTagButtons();
            foreach (var button in tag_buttons)
            {
                button.AddToToolStrip(Tools);
            }
            AddButton(
                action: AddSnbt,
                hover: "Add as SNBT",
                icon: IconType.AddSnbt,
                strip: Tools
            );
            AddButton(
                action: AddChunk,
                hover: "Add Chunk",
                icon: IconType.Chunk,
                strip: Tools
            );
            Tools.Items.Add(new ToolStripSeparator());
            AddButton(
                action: Find,
                text: "&Find",
                hover: "Find",
                icon: IconType.Search,
                shortcut: Keys.Control | Keys.F,
                strip: Tools,
                menu: MenuSearch
            );
            AddButton(
                action: About,
                text: "&About",
                icon: IconType.NbtStudio,
                shortcut: Keys.F1,
                menu: MenuHelp
            );
            AddButton(
                action: ChangeIcons,
                text: "&Change Icons",
                icon: IconType.Refresh,
                shortcut: Keys.Control | Keys.I,
                menu: MenuHelp
            );
            MenuHelp.DropDownItems.Add(new ToolStripSeparator());
            UpdateButton = AddButton(
                action: Update,
                text: "&Update",
                strip: MenuStrip
            );
            UpdateButton.Visible = false;
            AddButton(
                action: CheckForUpdates,
                text: "Check for &Updates",
                shortcut: Keys.Control | Keys.U,
                menu: MenuHelp
            );
        }

        private List<DualMenuItem> MakeCreateTagButtons()
        {
            var buttons = new List<DualMenuItem>();
            foreach (var type in NbtUtil.NormalTagTypes())
            {
                var button = DualMenuItem.SingleButton(
                    hover: $"Add {NbtUtil.TagTypeName(type)} Tag",
                    icon: NbtUtil.TagIconType(type));
                button.Click += (s, e) => AddTag(type);
                buttons.Add(button);
                ButtonsCollection.Add(button);
            }
            return buttons;
        }

        private readonly Dictionary<EnableTrigger, Action> EnableTriggers = new()
        {
            { EnableTrigger.SelectionChanged, () => { } },
            { EnableTrigger.TreeChanged, () => { } }
        };
        private DualMenuItem AddButton(
            Action action = null,
            string text = null,
            string hover = null,
            IconType? icon = null,
            Keys? shortcut = null,
            ToolStrip strip = null,
            ToolStripMenuItem menu = null,
            DualMenuItem parent = null,
            Func<bool> enabled_when = null,
            EnableTrigger? enable_trigger = null
            )
        {
            var button = new DualMenuItem(text, hover, icon, shortcut ?? Keys.None);
            if (action != null)
                button.Click += (s, e) => action();
            if (strip != null)
                button.AddToToolStrip(strip);
            if (menu != null)
                button.AddToMenuItem(menu);
            if (parent != null)
                button.AddToDual(parent);
            if (enable_trigger.HasValue && enabled_when != null)
            {
                EnableTriggers[enable_trigger.Value] += () => button.Enabled = enabled_when();
            }
            ButtonsCollection.Add(button);
            return button;
        }

        private enum EnableTrigger
        {
            TreeChanged,
            SelectionChanged
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
    }
}
