using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtStudio
{
    public static class IconSourceRegistry
    {
        public static IconSource DefaultSource { get; private set; }
        private static readonly Dictionary<string, IconSource> Sources = new Dictionary<string, IconSource>();
        public static IEnumerable<KeyValuePair<string, IconSource>> RegisteredSources => Sources.AsEnumerable();
        static IconSourceRegistry()
        {
            SetDefault(AmberIconSource.Instance);
            Sources["builtin_amber"] = AmberIconSource.Instance;
            Sources["builtin_yusuke"] = YusukeIconSource.Instance;
            Sources["builtin_mixed"] = MixedIconSource.Instance;
        }

        public static void SetDefault(IconSource source)
        {
            DefaultSource = source;
        }

        public static void Register(string id, IconSource source)
        {
            Sources[id] = source;
        }

        public static void Unregister(string id)
        {
            Sources.Remove(id);
        }

        public static string GetID(IconSource source)
        {
            foreach (var item in Sources)
            {
                if (item.Value == source)
                    return item.Key;
            }
            return null;
        }

        public static IconSource FromID(string id)
        {
            if (Sources.TryGetValue(id, out var source))
                return source;
            return DefaultSource;
        }

        public static void RegisterCustomSource(string filepath)
        {
            using (ZipArchive zip = ZipFile.OpenRead(filepath))
            {
                var source = new ZippedIconSource(filepath, zip);
                Register(filepath, source);
            }
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

        public bool HasNull => Image == null || Icon == null;
    }

    public enum IconType
    {
        File,
        Folder,
        Region,
        Chunk,

        NewFile,
        OpenFile,
        OpenFolder,
        Save,
        SaveAll,

        Sort,
        Cut,
        Copy,
        Paste,
        Rename,
        Edit,
        EditSnbt,
        Delete,

        Undo,
        Redo,
        Refresh,
        Search,
        AddSnbt,

        ByteTag,
        ShortTag,
        IntTag,
        LongTag,
        FloatTag,
        DoubleTag,
        StringTag,
        ByteArrayTag,
        IntArrayTag,
        LongArrayTag,
        CompoundTag,
        ListTag,

        NbtStudio
    }

    public abstract class IconSource
    {
        public abstract string Name { get; }
        public abstract ImageIcon GetImage(IconType type);
    }

    public abstract class SimpleIconSource : IconSource
    {
        private readonly Dictionary<IconType, ImageIcon> Cache = new Dictionary<IconType, ImageIcon>();

        public override ImageIcon GetImage(IconType type)
        {
            if (Cache.TryGetValue(type, out var result))
                return result;
            return default;
        }

        protected void Add(IconType type, Image image, Icon icon)
        {
            Cache[type] = new ImageIcon(image, icon);
        }
    }

    public abstract class DeferToDefaultIconSource : IconSource
    {
        private readonly Dictionary<IconType, ImageIcon> Cache = new Dictionary<IconType, ImageIcon>();

        public override ImageIcon GetImage(IconType type)
        {
            if (Cache.TryGetValue(type, out var result))
                return result;
            var defer = IconSourceRegistry.DefaultSource;
            if (defer == null)
                return default;
            return defer.GetImage(type);
        }

        public bool IsDeferring(IconType type)
        {
            return !Cache.ContainsKey(type);
        }

        protected void Add(IconType type, Image image, Icon icon)
        {
            Cache[type] = new ImageIcon(image, icon);
        }
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
            Add(IconType.File, Properties.Resources.classic_file_image, Properties.Resources.classic_file_icon);
            Add(IconType.Folder, Properties.Resources.classic_folder_image, Properties.Resources.classic_folder_icon);
            Add(IconType.Region, Properties.Resources.classic_region_image, Properties.Resources.classic_region_icon);
            Add(IconType.Chunk, Properties.Resources.classic_chunk_image, Properties.Resources.classic_chunk_icon);

            Add(IconType.NewFile, Properties.Resources.classic_action_new_image, Properties.Resources.classic_action_new_icon);
            Add(IconType.OpenFile, Properties.Resources.classic_action_open_file_image, Properties.Resources.classic_action_open_file_icon);
            Add(IconType.OpenFolder, Properties.Resources.classic_action_open_folder_image, Properties.Resources.classic_action_open_folder_icon);
            Add(IconType.Save, Properties.Resources.classic_action_save_image, Properties.Resources.classic_action_save_icon);
            Add(IconType.SaveAll, Properties.Resources.classic_action_save_all_image, Properties.Resources.classic_action_save_all_icon);

            Add(IconType.Sort, Properties.Resources.classic_action_sort_image, Properties.Resources.classic_action_sort_icon);
            Add(IconType.Cut, Properties.Resources.classic_action_cut_image, Properties.Resources.classic_action_cut_icon);
            Add(IconType.Copy, Properties.Resources.classic_action_copy_image, Properties.Resources.classic_action_copy_icon);
            Add(IconType.Paste, Properties.Resources.classic_action_paste_image, Properties.Resources.classic_action_paste_icon);
            Add(IconType.Rename, Properties.Resources.classic_action_rename_image, Properties.Resources.classic_action_rename_icon);
            Add(IconType.Edit, Properties.Resources.classic_action_edit_image, Properties.Resources.classic_action_edit_icon);
            Add(IconType.EditSnbt, Properties.Resources.classic_action_edit_snbt_image, Properties.Resources.classic_action_edit_snbt_icon);
            Add(IconType.Delete, Properties.Resources.classic_action_delete_image, Properties.Resources.classic_action_delete_icon);

            Add(IconType.Undo, Properties.Resources.classic_action_undo_image, Properties.Resources.classic_action_undo_icon);
            Add(IconType.Redo, Properties.Resources.classic_action_redo_image, Properties.Resources.classic_action_redo_icon);
            Add(IconType.Refresh, Properties.Resources.classic_action_refresh_image, Properties.Resources.classic_action_refresh_icon);
            Add(IconType.Search, Properties.Resources.classic_action_search_image, Properties.Resources.classic_action_search_icon);
            Add(IconType.AddSnbt, Properties.Resources.classic_action_add_snbt_image, Properties.Resources.classic_action_add_snbt_icon);

            Add(IconType.ByteTag, Properties.Resources.tag_byte_image, Properties.Resources.tag_byte_icon);
            Add(IconType.ShortTag, Properties.Resources.tag_short_image, Properties.Resources.tag_short_icon);
            Add(IconType.IntTag, Properties.Resources.tag_int_image, Properties.Resources.tag_int_icon);
            Add(IconType.LongTag, Properties.Resources.tag_long_image, Properties.Resources.tag_long_icon);
            Add(IconType.FloatTag, Properties.Resources.tag_float_image, Properties.Resources.tag_float_icon);
            Add(IconType.DoubleTag, Properties.Resources.tag_double_image, Properties.Resources.tag_double_icon);
            Add(IconType.StringTag, Properties.Resources.tag_string_image, Properties.Resources.tag_string_icon);
            Add(IconType.ByteArrayTag, Properties.Resources.tag_byte_array_image, Properties.Resources.tag_byte_array_icon);
            Add(IconType.IntArrayTag, Properties.Resources.tag_int_array_image, Properties.Resources.tag_int_array_icon);
            Add(IconType.LongArrayTag, Properties.Resources.tag_long_array_image, Properties.Resources.tag_long_array_icon);
            Add(IconType.CompoundTag, Properties.Resources.tag_compound_image, Properties.Resources.tag_compound_icon);
            Add(IconType.ListTag, Properties.Resources.tag_list_image, Properties.Resources.tag_list_icon);

            Add(IconType.NbtStudio, Properties.Resources.app_image_256, Properties.Resources.app_icon_256);
        }
    }

    // classic icons by Yusuke Kamiyamane
    public class YusukeIconSource : SimpleIconSource
    {
        public override string Name => "NBTExplorer";
        public static YusukeIconSource Instance = new YusukeIconSource();
        private YusukeIconSource()
        {
            Add(IconType.File, Properties.Resources.legacy_file_image, Properties.Resources.legacy_file_icon);
            Add(IconType.Folder, Properties.Resources.legacy_folder_image, Properties.Resources.classic_folder_icon);
            Add(IconType.Region, Properties.Resources.legacy_region_image, Properties.Resources.legacy_region_icon);
            Add(IconType.Chunk, Properties.Resources.legacy_tag_compound_image, Properties.Resources.legacy_tag_compound_icon);

            Add(IconType.NewFile, Properties.Resources.classic_action_new_image, Properties.Resources.classic_action_new_icon);
            Add(IconType.OpenFile, Properties.Resources.classic_action_open_file_image, Properties.Resources.classic_action_open_file_icon);
            Add(IconType.OpenFolder, Properties.Resources.classic_action_open_folder_image, Properties.Resources.classic_action_open_folder_icon);
            Add(IconType.Save, Properties.Resources.classic_action_save_image, Properties.Resources.classic_action_save_icon);
            Add(IconType.SaveAll, Properties.Resources.classic_action_save_all_image, Properties.Resources.classic_action_save_all_icon);

            Add(IconType.Sort, Properties.Resources.classic_action_sort_image, Properties.Resources.classic_action_sort_icon);
            Add(IconType.Cut, Properties.Resources.classic_action_cut_image, Properties.Resources.classic_action_cut_icon);
            Add(IconType.Copy, Properties.Resources.classic_action_copy_image, Properties.Resources.classic_action_copy_icon);
            Add(IconType.Paste, Properties.Resources.classic_action_paste_image, Properties.Resources.classic_action_paste_icon);
            Add(IconType.Rename, Properties.Resources.classic_action_rename_image, Properties.Resources.classic_action_rename_icon);
            Add(IconType.Edit, Properties.Resources.classic_action_edit_image, Properties.Resources.classic_action_edit_icon);
            Add(IconType.EditSnbt, Properties.Resources.classic_action_edit_snbt_image, Properties.Resources.classic_action_edit_snbt_icon);
            Add(IconType.Delete, Properties.Resources.classic_action_delete_image, Properties.Resources.classic_action_delete_icon);

            Add(IconType.Undo, Properties.Resources.classic_action_undo_image, Properties.Resources.classic_action_undo_icon);
            Add(IconType.Redo, Properties.Resources.classic_action_redo_image, Properties.Resources.classic_action_redo_icon);
            Add(IconType.Refresh, Properties.Resources.classic_action_refresh_image, Properties.Resources.classic_action_refresh_icon);
            Add(IconType.Search, Properties.Resources.classic_action_search_image, Properties.Resources.classic_action_search_icon);
            Add(IconType.AddSnbt, Properties.Resources.classic_action_add_snbt_image, Properties.Resources.classic_action_add_snbt_icon);

            Add(IconType.ByteTag, Properties.Resources.legacy_tag_byte_image, Properties.Resources.legacy_tag_byte_icon);
            Add(IconType.ShortTag, Properties.Resources.legacy_tag_short_image, Properties.Resources.legacy_tag_short_icon);
            Add(IconType.IntTag, Properties.Resources.legacy_tag_int_image, Properties.Resources.legacy_tag_int_icon);
            Add(IconType.LongTag, Properties.Resources.legacy_tag_long_image, Properties.Resources.legacy_tag_long_icon);
            Add(IconType.FloatTag, Properties.Resources.legacy_tag_float_image, Properties.Resources.legacy_tag_float_icon);
            Add(IconType.DoubleTag, Properties.Resources.legacy_tag_double_image, Properties.Resources.legacy_tag_double_icon);
            Add(IconType.StringTag, Properties.Resources.legacy_tag_string_image, Properties.Resources.legacy_tag_string_icon);
            Add(IconType.ByteArrayTag, Properties.Resources.legacy_tag_byte_array_image, Properties.Resources.legacy_tag_byte_array_icon);
            Add(IconType.IntArrayTag, Properties.Resources.legacy_tag_int_array_image, Properties.Resources.legacy_tag_int_array_icon);
            Add(IconType.LongArrayTag, Properties.Resources.legacy_tag_long_array_image, Properties.Resources.legacy_tag_long_array_icon);
            Add(IconType.CompoundTag, Properties.Resources.legacy_tag_compound_image, Properties.Resources.legacy_tag_compound_icon);
            Add(IconType.ListTag, Properties.Resources.legacy_tag_list_image, Properties.Resources.legacy_tag_list_icon);

            Add(IconType.NbtStudio, Properties.Resources.app_image_256, Properties.Resources.app_icon_256);
        }
    }

    // new icons by AmberW
    public class AmberIconSource : SimpleIconSource
    {
        public override string Name => "Amber";
        public static AmberIconSource Instance = new AmberIconSource();
        private AmberIconSource()
        {
            Add(IconType.File, Properties.Resources.new_file_image, Properties.Resources.new_file_icon);
            Add(IconType.Folder, Properties.Resources.new_folder_image, Properties.Resources.new_folder_icon);
            Add(IconType.Region, Properties.Resources.new_region_image, Properties.Resources.new_region_icon);
            Add(IconType.Chunk, Properties.Resources.new_chunk_image, Properties.Resources.new_chunk_icon);

            Add(IconType.NewFile, Properties.Resources.new_action_new_image, Properties.Resources.new_action_new_icon);
            Add(IconType.OpenFile, Properties.Resources.new_action_open_file_image, Properties.Resources.new_action_open_file_icon);
            Add(IconType.OpenFolder, Properties.Resources.new_action_open_folder_image, Properties.Resources.new_action_open_folder_icon);
            Add(IconType.Save, Properties.Resources.new_action_save_image, Properties.Resources.new_action_save_icon);
            Add(IconType.SaveAll, Properties.Resources.new_action_save_all_image, Properties.Resources.new_action_save_all_icon);

            Add(IconType.Sort, Properties.Resources.new_action_sort_image, Properties.Resources.new_action_sort_icon);
            Add(IconType.Cut, Properties.Resources.new_action_cut_image, Properties.Resources.new_action_cut_icon);
            Add(IconType.Copy, Properties.Resources.new_action_copy_image, Properties.Resources.new_action_copy_icon);
            Add(IconType.Paste, Properties.Resources.new_action_paste_image, Properties.Resources.new_action_paste_icon);
            Add(IconType.Rename, Properties.Resources.new_action_rename_image, Properties.Resources.new_action_rename_icon);
            Add(IconType.Edit, Properties.Resources.new_action_edit_image, Properties.Resources.new_action_edit_icon);
            Add(IconType.EditSnbt, Properties.Resources.new_action_edit_snbt_image, Properties.Resources.new_action_edit_snbt_icon);
            Add(IconType.Delete, Properties.Resources.new_action_delete_image, Properties.Resources.new_action_delete_icon);

            Add(IconType.Undo, Properties.Resources.new_action_undo_image, Properties.Resources.new_action_undo_icon);
            Add(IconType.Redo, Properties.Resources.new_action_redo_image, Properties.Resources.new_action_redo_icon);
            Add(IconType.Refresh, Properties.Resources.new_action_refresh_image, Properties.Resources.new_action_refresh_icon);
            Add(IconType.Search, Properties.Resources.new_action_search_image, Properties.Resources.new_action_search_icon);
            Add(IconType.AddSnbt, Properties.Resources.new_action_add_snbt_image, Properties.Resources.new_action_add_snbt_icon);

            Add(IconType.ByteTag, Properties.Resources.tag_byte_image, Properties.Resources.tag_byte_icon);
            Add(IconType.ShortTag, Properties.Resources.tag_short_image, Properties.Resources.tag_short_icon);
            Add(IconType.IntTag, Properties.Resources.tag_int_image, Properties.Resources.tag_int_icon);
            Add(IconType.LongTag, Properties.Resources.tag_long_image, Properties.Resources.tag_long_icon);
            Add(IconType.FloatTag, Properties.Resources.tag_float_image, Properties.Resources.tag_float_icon);
            Add(IconType.DoubleTag, Properties.Resources.tag_double_image, Properties.Resources.tag_double_icon);
            Add(IconType.StringTag, Properties.Resources.tag_string_image, Properties.Resources.tag_string_icon);
            Add(IconType.ByteArrayTag, Properties.Resources.tag_byte_array_image, Properties.Resources.tag_byte_array_icon);
            Add(IconType.IntArrayTag, Properties.Resources.tag_int_array_image, Properties.Resources.tag_int_array_icon);
            Add(IconType.LongArrayTag, Properties.Resources.tag_long_array_image, Properties.Resources.tag_long_array_icon);
            Add(IconType.CompoundTag, Properties.Resources.tag_compound_image, Properties.Resources.tag_compound_icon);
            Add(IconType.ListTag, Properties.Resources.tag_list_image, Properties.Resources.tag_list_icon);

            Add(IconType.NbtStudio, Properties.Resources.app_image_256, Properties.Resources.app_icon_256);
        }
    }

    public class ZippedIconSource : DeferToDefaultIconSource
    {
        private readonly string Filepath;
        public override string Name => Path.GetFileNameWithoutExtension(Filepath);
        private readonly Dictionary<string, IconType> FileNameMap = new Dictionary<string, IconType>
        {
            { "file", IconType.File },
            { "folder", IconType.Folder },
            { "region", IconType.Region },
            { "chunk", IconType.Chunk },
            { "new_file", IconType.NewFile },
            { "open_file", IconType.OpenFile },
            { "open_folder", IconType.OpenFolder },
            { "save", IconType.Save },
            { "save_all", IconType.SaveAll },
            { "sort", IconType.Sort },
            { "cut", IconType.Cut },
            { "copy", IconType.Copy },
            { "paste", IconType.Paste },
            { "rename", IconType.Rename },
            { "edit", IconType.Edit },
            { "edit_snbt", IconType.EditSnbt },
            { "delete", IconType.Delete },
            { "undo", IconType.Undo },
            { "redo", IconType.Redo },
            { "refresh", IconType.Refresh },
            { "search", IconType.Search },
            { "add_snbt", IconType.AddSnbt },
            { "byte_tag", IconType.ByteTag },
            { "short_tag", IconType.ShortTag },
            { "int_tag", IconType.IntTag },
            { "long_tag", IconType.LongTag },
            { "float_tag", IconType.FloatTag },
            { "double_tag", IconType.DoubleTag },
            { "string_tag", IconType.StringTag },
            { "byte_array_tag", IconType.ByteArrayTag },
            { "int_array_tag", IconType.IntArrayTag },
            { "long_array_tag", IconType.LongArrayTag },
            { "compound_tag", IconType.CompoundTag },
            { "list_tag", IconType.ListTag },
            { "nbt_studio", IconType.NbtStudio },
        };
        public ZippedIconSource(string path, ZipArchive zip)
        {
            Filepath = path;
            foreach (var entry in zip.Entries)
            {
                var name = entry.FullName;
                if (Path.GetExtension(name) != ".png")
                    continue;
                var simple = Path.GetFileNameWithoutExtension(name);
                if (!FileNameMap.TryGetValue(simple, out var type))
                    continue;
                var image = Image.FromStream(entry.Open());
                Icon icon;
                using (var icon_stream = new MemoryStream())
                {
                    ConvertToIcon(image, icon_stream);
                    icon_stream.Position = 0;
                    icon = new Icon(icon_stream);
                }
                Add(type, image, icon);
            }
        }

        private static void ConvertToIcon(Image input, Stream output)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                input.Save(memoryStream, ImageFormat.Png);
                var iconWriter = new BinaryWriter(output);
                iconWriter.Write((byte)0);
                iconWriter.Write((byte)0);
                iconWriter.Write((short)1);
                iconWriter.Write((short)1);
                iconWriter.Write((byte)input.Width);
                iconWriter.Write((byte)input.Height);
                iconWriter.Write((byte)0);
                iconWriter.Write((byte)0);
                iconWriter.Write((short)0);
                iconWriter.Write((short)32);
                iconWriter.Write((int)memoryStream.Length);
                iconWriter.Write((int)(6 + 16));
                iconWriter.Write(memoryStream.ToArray());
                iconWriter.Flush();
            }
        }
    }
}
