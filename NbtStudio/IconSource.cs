using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtStudio
{
    public static class IconSourceRegistry
    {
        private static readonly Dictionary<int, IconSource> Sources = new Dictionary<int, IconSource>();
        public static IEnumerable<KeyValuePair<int, IconSource>> RegisteredSources => Sources.AsEnumerable();
        static IconSourceRegistry()
        {
            Sources[0] = AmberIconSource.Instance;
            Sources[1] = YusukeIconSource.Instance;
            Sources[2] = MixedIconSource.Instance;
        }
        public static int MaxID => Sources.Keys.Max();

        public static int GetID(IconSource source)
        {
            foreach (var item in Sources)
            {
                if (item.Value == source)
                    return item.Key;
            }
            return -1;
        }

        public static IconSource FromID(int id)
        {
            if (Sources.TryGetValue(id, out var source))
                return source;
            return NullIconSource.Instance;
        }
    }

    public struct ImageIcon
    {
        public readonly Image Image;
        public readonly Icon Icon;
        public ImageIcon(Image image, Icon icon)
        {
            Image = image;
            Icon = icon;
        }
    }

    public abstract class IconSource
    {
        public abstract string Name { get; }

        public abstract ImageIcon File { get; }
        public abstract ImageIcon Folder { get; }
        public abstract ImageIcon Region { get; }
        public abstract ImageIcon Chunk { get; }

        public abstract ImageIcon NewFile { get; }
        public abstract ImageIcon OpenFile { get; }
        public abstract ImageIcon OpenFolder { get; }
        public abstract ImageIcon Save { get; }
        public abstract ImageIcon SaveAll { get; }

        public abstract ImageIcon Sort { get; }
        public abstract ImageIcon Cut { get; }
        public abstract ImageIcon Copy { get; }
        public abstract ImageIcon Paste { get; }
        public abstract ImageIcon Rename { get; }
        public abstract ImageIcon Edit { get; }
        public abstract ImageIcon EditSnbt { get; }
        public abstract ImageIcon Delete { get; }

        public abstract ImageIcon Undo { get; }
        public abstract ImageIcon Redo { get; }
        public abstract ImageIcon Refresh { get; }
        public abstract ImageIcon Search { get; }
        public abstract ImageIcon AddSnbt { get; }

        public abstract ImageIcon ByteTag { get; }
        public abstract ImageIcon ShortTag { get; }
        public abstract ImageIcon IntTag { get; }
        public abstract ImageIcon LongTag { get; }
        public abstract ImageIcon FloatTag { get; }
        public abstract ImageIcon DoubleTag { get; }
        public abstract ImageIcon StringTag { get; }
        public abstract ImageIcon ByteArrayTag { get; }
        public abstract ImageIcon IntArrayTag { get; }
        public abstract ImageIcon LongArrayTag { get; }
        public abstract ImageIcon CompoundTag { get; }
        public abstract ImageIcon ListTag { get; }

        public abstract ImageIcon NbtStudio { get; }
    }

    public abstract class SimpleIconSource : IconSource
    {
        protected ImageIcon _File;
        protected ImageIcon _Folder;
        protected ImageIcon _Region;
        protected ImageIcon _Chunk;

        protected ImageIcon _NewFile;
        protected ImageIcon _OpenFile;
        protected ImageIcon _OpenFolder;
        protected ImageIcon _Save;
        protected ImageIcon _SaveAll;

        protected ImageIcon _Sort;
        protected ImageIcon _Cut;
        protected ImageIcon _Copy;
        protected ImageIcon _Paste;
        protected ImageIcon _Rename;
        protected ImageIcon _Edit;
        protected ImageIcon _EditSnbt;
        protected ImageIcon _Delete;

        protected ImageIcon _Undo;
        protected ImageIcon _Redo;
        protected ImageIcon _Refresh;
        protected ImageIcon _Search;
        protected ImageIcon _AddSnbt;

        protected ImageIcon _ByteTag;
        protected ImageIcon _ShortTag;
        protected ImageIcon _IntTag;
        protected ImageIcon _LongTag;
        protected ImageIcon _FloatTag;
        protected ImageIcon _DoubleTag;
        protected ImageIcon _StringTag;
        protected ImageIcon _ByteArrayTag;
        protected ImageIcon _IntArrayTag;
        protected ImageIcon _LongArrayTag;
        protected ImageIcon _CompoundTag;
        protected ImageIcon _ListTag;

        protected ImageIcon _NbtStudio;

        public override ImageIcon File => _File;
        public override ImageIcon Folder => _Folder;
        public override ImageIcon Region => _Region;
        public override ImageIcon Chunk => _Chunk;

        public override ImageIcon NewFile => _NewFile;
        public override ImageIcon OpenFile => _OpenFile;
        public override ImageIcon OpenFolder => _OpenFolder;
        public override ImageIcon Save => _Save;
        public override ImageIcon SaveAll => _SaveAll;

        public override ImageIcon Sort => _Sort;
        public override ImageIcon Cut => _Cut;
        public override ImageIcon Copy => _Copy;
        public override ImageIcon Paste => _Paste;
        public override ImageIcon Rename => _Rename;
        public override ImageIcon Edit => _Edit;
        public override ImageIcon EditSnbt => _EditSnbt;
        public override ImageIcon Delete => _Delete;

        public override ImageIcon Undo => _Undo;
        public override ImageIcon Redo => _Redo;
        public override ImageIcon Refresh => _Refresh;
        public override ImageIcon Search => _Search;
        public override ImageIcon AddSnbt => _AddSnbt;

        public override ImageIcon ByteTag => _ByteTag;
        public override ImageIcon ShortTag => _ShortTag;
        public override ImageIcon IntTag => _IntTag;
        public override ImageIcon LongTag => _LongTag;
        public override ImageIcon FloatTag => _FloatTag;
        public override ImageIcon DoubleTag => _DoubleTag;
        public override ImageIcon StringTag => _StringTag;
        public override ImageIcon ByteArrayTag => _ByteArrayTag;
        public override ImageIcon IntArrayTag => _IntArrayTag;
        public override ImageIcon LongArrayTag => _LongArrayTag;
        public override ImageIcon CompoundTag => _CompoundTag;
        public override ImageIcon ListTag => _ListTag;

        public override ImageIcon NbtStudio => _NbtStudio;
    }

    public class NullIconSource : SimpleIconSource
    {
        public override string Name => "Null";
        public static NullIconSource Instance = new NullIconSource();
        private NullIconSource()
        { }
    }

    public class MixedIconSource : SimpleIconSource
    {
        public override string Name => "Mixed";
        public static MixedIconSource Instance = new MixedIconSource();
        private MixedIconSource()
        {
            _File = new ImageIcon(Properties.Resources.classic_file_image, Properties.Resources.classic_file_icon);
            _Folder = new ImageIcon(Properties.Resources.classic_folder_image, Properties.Resources.classic_folder_icon);
            _Region = new ImageIcon(Properties.Resources.classic_region_image, Properties.Resources.classic_region_icon);
            _Chunk = new ImageIcon(Properties.Resources.classic_chunk_image, Properties.Resources.classic_chunk_icon);

            _NewFile = new ImageIcon(Properties.Resources.classic_action_new_image, Properties.Resources.classic_action_new_icon);
            _OpenFile = new ImageIcon(Properties.Resources.classic_action_open_file_image, Properties.Resources.classic_action_open_file_icon);
            _OpenFolder = new ImageIcon(Properties.Resources.classic_action_open_folder_image, Properties.Resources.classic_action_open_folder_icon);
            _Save = new ImageIcon(Properties.Resources.classic_action_save_image, Properties.Resources.classic_action_save_icon);
            _SaveAll = new ImageIcon(Properties.Resources.classic_action_save_all_image, Properties.Resources.classic_action_save_all_icon);

            _Sort = new ImageIcon(Properties.Resources.classic_action_sort_image, Properties.Resources.classic_action_sort_icon);
            _Cut = new ImageIcon(Properties.Resources.classic_action_cut_image, Properties.Resources.classic_action_cut_icon);
            _Copy = new ImageIcon(Properties.Resources.classic_action_copy_image, Properties.Resources.classic_action_copy_icon);
            _Paste = new ImageIcon(Properties.Resources.classic_action_paste_image, Properties.Resources.classic_action_paste_icon);
            _Rename = new ImageIcon(Properties.Resources.classic_action_rename_image, Properties.Resources.classic_action_rename_icon);
            _Edit = new ImageIcon(Properties.Resources.classic_action_edit_image, Properties.Resources.classic_action_edit_icon);
            _EditSnbt = new ImageIcon(Properties.Resources.classic_action_edit_snbt_image, Properties.Resources.classic_action_edit_snbt_icon);
            _Delete = new ImageIcon(Properties.Resources.classic_action_delete_image, Properties.Resources.classic_action_delete_icon);

            _Undo = new ImageIcon(Properties.Resources.classic_action_undo_image, Properties.Resources.classic_action_undo_icon);
            _Redo = new ImageIcon(Properties.Resources.classic_action_redo_image, Properties.Resources.classic_action_redo_icon);
            _Refresh = new ImageIcon(Properties.Resources.classic_action_refresh_image, Properties.Resources.classic_action_refresh_icon);
            _Search = new ImageIcon(Properties.Resources.classic_action_search_image, Properties.Resources.classic_action_search_icon);
            _AddSnbt = new ImageIcon(Properties.Resources.classic_action_add_snbt_image, Properties.Resources.classic_action_add_snbt_icon);

            _ByteTag = new ImageIcon(Properties.Resources.tag_byte_image, Properties.Resources.tag_byte_icon);
            _ShortTag = new ImageIcon(Properties.Resources.tag_short_image, Properties.Resources.tag_short_icon);
            _IntTag = new ImageIcon(Properties.Resources.tag_int_image, Properties.Resources.tag_int_icon);
            _LongTag = new ImageIcon(Properties.Resources.tag_long_image, Properties.Resources.tag_long_icon);
            _FloatTag = new ImageIcon(Properties.Resources.tag_float_image, Properties.Resources.tag_float_icon);
            _DoubleTag = new ImageIcon(Properties.Resources.tag_double_image, Properties.Resources.tag_double_icon);
            _StringTag = new ImageIcon(Properties.Resources.tag_string_image, Properties.Resources.tag_string_icon);
            _ByteArrayTag = new ImageIcon(Properties.Resources.tag_byte_array_image, Properties.Resources.tag_byte_array_icon);
            _IntArrayTag = new ImageIcon(Properties.Resources.tag_int_array_image, Properties.Resources.tag_int_array_icon);
            _LongArrayTag = new ImageIcon(Properties.Resources.tag_long_array_image, Properties.Resources.tag_long_array_icon);
            _CompoundTag = new ImageIcon(Properties.Resources.tag_compound_image, Properties.Resources.tag_compound_icon);
            _ListTag = new ImageIcon(Properties.Resources.tag_list_image, Properties.Resources.tag_list_icon);

            _NbtStudio = new ImageIcon(Properties.Resources.app_image_256, Properties.Resources.app_icon_256);
        }
    }

    // classic icons by Yusuke Kamiyamane
    public class YusukeIconSource : SimpleIconSource
    {
        public override string Name => "NBTExplorer";
        public static YusukeIconSource Instance = new YusukeIconSource();
        private YusukeIconSource()
        {
            _File = new ImageIcon(Properties.Resources.legacy_file_image, Properties.Resources.legacy_file_icon);
            _Folder = new ImageIcon(Properties.Resources.legacy_folder_image, Properties.Resources.classic_folder_icon);
            _Region = new ImageIcon(Properties.Resources.legacy_region_image, Properties.Resources.legacy_region_icon);
            _Chunk = new ImageIcon(Properties.Resources.legacy_tag_compound_image, Properties.Resources.legacy_tag_compound_icon);

            _NewFile = new ImageIcon(Properties.Resources.classic_action_new_image, Properties.Resources.classic_action_new_icon);
            _OpenFile = new ImageIcon(Properties.Resources.classic_action_open_file_image, Properties.Resources.classic_action_open_file_icon);
            _OpenFolder = new ImageIcon(Properties.Resources.classic_action_open_folder_image, Properties.Resources.classic_action_open_folder_icon);
            _Save = new ImageIcon(Properties.Resources.classic_action_save_image, Properties.Resources.classic_action_save_icon);
            _SaveAll = new ImageIcon(Properties.Resources.classic_action_save_all_image, Properties.Resources.classic_action_save_all_icon);

            _Sort = new ImageIcon(Properties.Resources.classic_action_sort_image, Properties.Resources.classic_action_sort_icon);
            _Cut = new ImageIcon(Properties.Resources.classic_action_cut_image, Properties.Resources.classic_action_cut_icon);
            _Copy = new ImageIcon(Properties.Resources.classic_action_copy_image, Properties.Resources.classic_action_copy_icon);
            _Paste = new ImageIcon(Properties.Resources.classic_action_paste_image, Properties.Resources.classic_action_paste_icon);
            _Rename = new ImageIcon(Properties.Resources.classic_action_rename_image, Properties.Resources.classic_action_rename_icon);
            _Edit = new ImageIcon(Properties.Resources.classic_action_edit_image, Properties.Resources.classic_action_edit_icon);
            _EditSnbt = new ImageIcon(Properties.Resources.classic_action_edit_snbt_image, Properties.Resources.classic_action_edit_snbt_icon);
            _Delete = new ImageIcon(Properties.Resources.classic_action_delete_image, Properties.Resources.classic_action_delete_icon);

            _Undo = new ImageIcon(Properties.Resources.classic_action_undo_image, Properties.Resources.classic_action_undo_icon);
            _Redo = new ImageIcon(Properties.Resources.classic_action_redo_image, Properties.Resources.classic_action_redo_icon);
            _Refresh = new ImageIcon(Properties.Resources.classic_action_refresh_image, Properties.Resources.classic_action_refresh_icon);
            _Search = new ImageIcon(Properties.Resources.classic_action_search_image, Properties.Resources.classic_action_search_icon);
            _AddSnbt = new ImageIcon(Properties.Resources.classic_action_add_snbt_image, Properties.Resources.classic_action_add_snbt_icon);

            _ByteTag = new ImageIcon(Properties.Resources.legacy_tag_byte_image, Properties.Resources.legacy_tag_byte_icon);
            _ShortTag = new ImageIcon(Properties.Resources.legacy_tag_short_image, Properties.Resources.legacy_tag_short_icon);
            _IntTag = new ImageIcon(Properties.Resources.legacy_tag_int_image, Properties.Resources.legacy_tag_int_icon);
            _LongTag = new ImageIcon(Properties.Resources.legacy_tag_long_image, Properties.Resources.legacy_tag_long_icon);
            _FloatTag = new ImageIcon(Properties.Resources.legacy_tag_float_image, Properties.Resources.legacy_tag_float_icon);
            _DoubleTag = new ImageIcon(Properties.Resources.legacy_tag_double_image, Properties.Resources.legacy_tag_double_icon);
            _StringTag = new ImageIcon(Properties.Resources.legacy_tag_string_image, Properties.Resources.legacy_tag_string_icon);
            _ByteArrayTag = new ImageIcon(Properties.Resources.legacy_tag_byte_array_image, Properties.Resources.legacy_tag_byte_array_icon);
            _IntArrayTag = new ImageIcon(Properties.Resources.legacy_tag_int_array_image, Properties.Resources.legacy_tag_int_array_icon);
            _LongArrayTag = new ImageIcon(Properties.Resources.legacy_tag_long_array_image, Properties.Resources.legacy_tag_long_array_icon);
            _CompoundTag = new ImageIcon(Properties.Resources.legacy_tag_compound_image, Properties.Resources.legacy_tag_compound_icon);
            _ListTag = new ImageIcon(Properties.Resources.legacy_tag_list_image, Properties.Resources.legacy_tag_list_icon);

            _NbtStudio = new ImageIcon(Properties.Resources.app_image_256, Properties.Resources.app_icon_256);
        }
    }

    // new icons by AmberW
    public class AmberIconSource : SimpleIconSource
    {
        public override string Name => "Amber";
        public static AmberIconSource Instance = new AmberIconSource();
        private AmberIconSource()
        {
            _File = new ImageIcon(Properties.Resources.new_file_image, Properties.Resources.new_file_icon);
            _Folder = new ImageIcon(Properties.Resources.new_folder_image, Properties.Resources.new_folder_icon);
            _Region = new ImageIcon(Properties.Resources.new_region_image, Properties.Resources.new_region_icon);
            _Chunk = new ImageIcon(Properties.Resources.new_chunk_image, Properties.Resources.new_chunk_icon);

            _NewFile = new ImageIcon(Properties.Resources.new_action_new_image, Properties.Resources.new_action_new_icon);
            _OpenFile = new ImageIcon(Properties.Resources.new_action_open_file_image, Properties.Resources.new_action_open_file_icon);
            _OpenFolder = new ImageIcon(Properties.Resources.new_action_open_folder_image, Properties.Resources.new_action_open_folder_icon);
            _Save = new ImageIcon(Properties.Resources.new_action_save_image, Properties.Resources.new_action_save_icon);
            _SaveAll = new ImageIcon(Properties.Resources.new_action_save_all_image, Properties.Resources.new_action_save_all_icon);

            _Sort = new ImageIcon(Properties.Resources.new_action_sort_image, Properties.Resources.new_action_sort_icon);
            _Cut = new ImageIcon(Properties.Resources.new_action_cut_image, Properties.Resources.new_action_cut_icon);
            _Copy = new ImageIcon(Properties.Resources.new_action_copy_image, Properties.Resources.new_action_copy_icon);
            _Paste = new ImageIcon(Properties.Resources.new_action_paste_image, Properties.Resources.new_action_paste_icon);
            _Rename = new ImageIcon(Properties.Resources.new_action_rename_image, Properties.Resources.new_action_rename_icon);
            _Edit = new ImageIcon(Properties.Resources.new_action_edit_image, Properties.Resources.new_action_edit_icon);
            _EditSnbt = new ImageIcon(Properties.Resources.new_action_edit_snbt_image, Properties.Resources.new_action_edit_snbt_icon);
            _Delete = new ImageIcon(Properties.Resources.new_action_delete_image, Properties.Resources.new_action_delete_icon);

            _Undo = new ImageIcon(Properties.Resources.new_action_undo_image, Properties.Resources.new_action_undo_icon);
            _Redo = new ImageIcon(Properties.Resources.new_action_redo_image, Properties.Resources.new_action_redo_icon);
            _Refresh = new ImageIcon(Properties.Resources.new_action_refresh_image, Properties.Resources.new_action_refresh_icon);
            _Search = new ImageIcon(Properties.Resources.new_action_search_image, Properties.Resources.new_action_search_icon);
            _AddSnbt = new ImageIcon(Properties.Resources.new_action_add_snbt_image, Properties.Resources.new_action_add_snbt_icon);

            _ByteTag = new ImageIcon(Properties.Resources.tag_byte_image, Properties.Resources.tag_byte_icon);
            _ShortTag = new ImageIcon(Properties.Resources.tag_short_image, Properties.Resources.tag_short_icon);
            _IntTag = new ImageIcon(Properties.Resources.tag_int_image, Properties.Resources.tag_int_icon);
            _LongTag = new ImageIcon(Properties.Resources.tag_long_image, Properties.Resources.tag_long_icon);
            _FloatTag = new ImageIcon(Properties.Resources.tag_float_image, Properties.Resources.tag_float_icon);
            _DoubleTag = new ImageIcon(Properties.Resources.tag_double_image, Properties.Resources.tag_double_icon);
            _StringTag = new ImageIcon(Properties.Resources.tag_string_image, Properties.Resources.tag_string_icon);
            _ByteArrayTag = new ImageIcon(Properties.Resources.tag_byte_array_image, Properties.Resources.tag_byte_array_icon);
            _IntArrayTag = new ImageIcon(Properties.Resources.tag_int_array_image, Properties.Resources.tag_int_array_icon);
            _LongArrayTag = new ImageIcon(Properties.Resources.tag_long_array_image, Properties.Resources.tag_long_array_icon);
            _CompoundTag = new ImageIcon(Properties.Resources.tag_compound_image, Properties.Resources.tag_compound_icon);
            _ListTag = new ImageIcon(Properties.Resources.tag_list_image, Properties.Resources.tag_list_icon);

            _NbtStudio = new ImageIcon(Properties.Resources.app_image_256, Properties.Resources.app_icon_256);
        }
    }
}
