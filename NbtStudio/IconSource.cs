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
            Sources["builtin_wiki"] = WikiIconSource.Instance;
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

        public bool HasImage(IconType type)
        {
            return Cache.ContainsKey(type);
        }

        protected void Add(IconType type, Image image, Icon icon)
        {
            Cache[type] = new ImageIcon(image, icon);
        }

        protected void Add(IconType type, Image image)
        {
            using (var stream = ConvertToIcon(image))
            {
                var icon = new Icon(stream);
                Add(type, image, icon);
            }
        }

        private static Stream ConvertToIcon(Image input)
        {
            using (var stream = new MemoryStream())
            {
                var output = new MemoryStream();
                input.Save(stream, ImageFormat.Png);
                var writer = new BinaryWriter(output);
                writer.Write((byte)0);
                writer.Write((byte)0);
                writer.Write((short)1);
                writer.Write((short)1);
                writer.Write((byte)input.Width);
                writer.Write((byte)input.Height);
                writer.Write((byte)0);
                writer.Write((byte)0);
                writer.Write((short)0);
                writer.Write((short)32);
                writer.Write((int)stream.Length);
                writer.Write((int)(6 + 16));
                writer.Write(stream.ToArray());
                writer.Flush();
                output.Position = 0;
                return output;
            }
        }
    }

    public abstract class DeferToDefaultIconSource : SimpleIconSource
    {
        public override ImageIcon GetImage(IconType type)
        {
            if (!HasImage(type))
            {
                var defer = IconSourceRegistry.DefaultSource;
                if (defer != null)
                    return defer.GetImage(type);
            }
            return base.GetImage(type);
        }

        public bool IsDeferring(IconType type)
        {
            return !HasImage(type);
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
            Add(IconType.File, Properties.Resources.yusuke_file_file);
            Add(IconType.Folder, Properties.Resources.yusuke_file_folder);
            Add(IconType.Region, Properties.Resources.tryashtar_file_region);
            Add(IconType.Chunk, Properties.Resources.tryashtar_file_chunk);

            Add(IconType.NewFile, Properties.Resources.yusuke_action_new_file);
            Add(IconType.OpenFile, Properties.Resources.yusuke_action_open_file);
            Add(IconType.OpenFolder, Properties.Resources.yusuke_action_open_folder);
            Add(IconType.Save, Properties.Resources.yusuke_action_save);
            Add(IconType.SaveAll, Properties.Resources.yusuke_action_save_all);

            Add(IconType.Sort, Properties.Resources.yusuke_action_sort);
            Add(IconType.Cut, Properties.Resources.yusuke_action_cut);
            Add(IconType.Copy, Properties.Resources.yusuke_action_copy);
            Add(IconType.Paste, Properties.Resources.yusuke_action_paste);
            Add(IconType.Rename, Properties.Resources.yusuke_action_rename);
            Add(IconType.Edit, Properties.Resources.yusuke_action_edit);
            Add(IconType.EditSnbt, Properties.Resources.tryashtar_action_edit_snbt);
            Add(IconType.Delete, Properties.Resources.yusuke_action_delete);

            Add(IconType.Undo, Properties.Resources.yusuke_action_undo);
            Add(IconType.Redo, Properties.Resources.yusuke_action_redo);
            Add(IconType.Refresh, Properties.Resources.yusuke_action_refresh);
            Add(IconType.Search, Properties.Resources.yusuke_action_search);
            Add(IconType.AddSnbt, Properties.Resources.tryashtar_action_add_snbt);

            Add(IconType.ByteTag, Properties.Resources.amber_tag_byte);
            Add(IconType.ShortTag, Properties.Resources.amber_tag_short);
            Add(IconType.IntTag, Properties.Resources.amber_tag_int);
            Add(IconType.LongTag, Properties.Resources.amber_tag_long);
            Add(IconType.FloatTag, Properties.Resources.amber_tag_float);
            Add(IconType.DoubleTag, Properties.Resources.amber_tag_double);
            Add(IconType.StringTag, Properties.Resources.amber_tag_string);
            Add(IconType.ByteArrayTag, Properties.Resources.amber_tag_byte_array);
            Add(IconType.IntArrayTag, Properties.Resources.amber_tag_int_array);
            Add(IconType.LongArrayTag, Properties.Resources.amber_tag_long_array);
            Add(IconType.CompoundTag, Properties.Resources.amber_tag_compound);
            Add(IconType.ListTag, Properties.Resources.amber_tag_list);

            Add(IconType.NbtStudio, Properties.Resources.nbt_studio_image_256);
        }
    }

    // classic icons by Yusuke Kamiyamane
    public class YusukeIconSource : SimpleIconSource
    {
        public override string Name => "NBTExplorer";
        public static YusukeIconSource Instance = new YusukeIconSource();
        private YusukeIconSource()
        {
            Add(IconType.File, Properties.Resources.yusuke_file_chunk);
            Add(IconType.Folder, Properties.Resources.yusuke_file_folder);
            Add(IconType.Region, Properties.Resources.yusuke_file_region);
            Add(IconType.Chunk, Properties.Resources.yusuke_tag_compound);

            Add(IconType.NewFile, Properties.Resources.yusuke_action_new_file);
            Add(IconType.OpenFile, Properties.Resources.yusuke_action_open_file);
            Add(IconType.OpenFolder, Properties.Resources.yusuke_action_open_folder);
            Add(IconType.Save, Properties.Resources.yusuke_action_save);
            Add(IconType.SaveAll, Properties.Resources.yusuke_action_save_all);

            Add(IconType.Sort, Properties.Resources.yusuke_action_sort);
            Add(IconType.Cut, Properties.Resources.yusuke_action_cut);
            Add(IconType.Copy, Properties.Resources.yusuke_action_copy);
            Add(IconType.Paste, Properties.Resources.yusuke_action_paste);
            Add(IconType.Rename, Properties.Resources.yusuke_action_rename);
            Add(IconType.Edit, Properties.Resources.yusuke_action_edit);
            Add(IconType.EditSnbt, Properties.Resources.tryashtar_action_edit_snbt);
            Add(IconType.Delete, Properties.Resources.yusuke_action_delete);

            Add(IconType.Undo, Properties.Resources.yusuke_action_undo);
            Add(IconType.Redo, Properties.Resources.yusuke_action_redo);
            Add(IconType.Refresh, Properties.Resources.yusuke_action_refresh);
            Add(IconType.Search, Properties.Resources.yusuke_action_search);
            Add(IconType.AddSnbt, Properties.Resources.tryashtar_action_add_snbt);

            Add(IconType.ByteTag, Properties.Resources.yusuke_tag_byte);
            Add(IconType.ShortTag, Properties.Resources.yusuke_tag_short);
            Add(IconType.IntTag, Properties.Resources.yusuke_tag_int);
            Add(IconType.LongTag, Properties.Resources.yusuke_tag_long);
            Add(IconType.FloatTag, Properties.Resources.yusuke_tag_float);
            Add(IconType.DoubleTag, Properties.Resources.yusuke_tag_double);
            Add(IconType.StringTag, Properties.Resources.yusuke_tag_string);
            Add(IconType.ByteArrayTag, Properties.Resources.yusuke_tag_byte_array);
            Add(IconType.IntArrayTag, Properties.Resources.yusuke_tag_int_array);
            Add(IconType.LongArrayTag, Properties.Resources.yusuke_tag_long_array);
            Add(IconType.CompoundTag, Properties.Resources.yusuke_tag_compound);
            Add(IconType.ListTag, Properties.Resources.yusuke_tag_list);

            Add(IconType.NbtStudio, Properties.Resources.nbt_studio_image_256);
        }
    }

    // new icons by AmberW
    public class AmberIconSource : SimpleIconSource
    {
        public override string Name => "Amber";
        public static AmberIconSource Instance = new AmberIconSource();
        private AmberIconSource()
        {
            Add(IconType.File, Properties.Resources.amber_file_file);
            Add(IconType.Folder, Properties.Resources.amber_file_folder);
            Add(IconType.Region, Properties.Resources.amber_file_region);
            Add(IconType.Chunk, Properties.Resources.amber_file_chunk);

            Add(IconType.NewFile, Properties.Resources.amber_action_new_file);
            Add(IconType.OpenFile, Properties.Resources.amber_action_open_file);
            Add(IconType.OpenFolder, Properties.Resources.amber_action_open_folder);
            Add(IconType.Save, Properties.Resources.amber_action_save);
            Add(IconType.SaveAll, Properties.Resources.amber_action_save_all);

            Add(IconType.Sort, Properties.Resources.amber_action_sort);
            Add(IconType.Cut, Properties.Resources.amber_action_cut);
            Add(IconType.Copy, Properties.Resources.amber_action_copy);
            Add(IconType.Paste, Properties.Resources.amber_action_paste);
            Add(IconType.Rename, Properties.Resources.amber_action_rename);
            Add(IconType.Edit, Properties.Resources.amber_action_edit);
            Add(IconType.EditSnbt, Properties.Resources.amber_action_edit_snbt);
            Add(IconType.Delete, Properties.Resources.amber_action_delete);

            Add(IconType.Undo, Properties.Resources.amber_action_undo);
            Add(IconType.Redo, Properties.Resources.amber_action_redo);
            Add(IconType.Refresh, Properties.Resources.amber_action_refresh);
            Add(IconType.Search, Properties.Resources.amber_action_search);
            Add(IconType.AddSnbt, Properties.Resources.amber_action_add_snbt);

            Add(IconType.ByteTag, Properties.Resources.amber_tag_byte);
            Add(IconType.ShortTag, Properties.Resources.amber_tag_short);
            Add(IconType.IntTag, Properties.Resources.amber_tag_int);
            Add(IconType.LongTag, Properties.Resources.amber_tag_long);
            Add(IconType.FloatTag, Properties.Resources.amber_tag_float);
            Add(IconType.DoubleTag, Properties.Resources.amber_tag_double);
            Add(IconType.StringTag, Properties.Resources.amber_tag_string);
            Add(IconType.ByteArrayTag, Properties.Resources.amber_tag_byte_array);
            Add(IconType.IntArrayTag, Properties.Resources.amber_tag_int_array);
            Add(IconType.LongArrayTag, Properties.Resources.amber_tag_long_array);
            Add(IconType.CompoundTag, Properties.Resources.amber_tag_compound);
            Add(IconType.ListTag, Properties.Resources.amber_tag_list);

            Add(IconType.NbtStudio, Properties.Resources.nbt_studio_image_256);
        }
    }

    public class WikiIconSource : DeferToDefaultIconSource
    {
        public override string Name => "Wiki";
        public static WikiIconSource Instance = new WikiIconSource();
        private WikiIconSource()
        {
            Add(IconType.ByteTag, Properties.Resources.wiki_tag_byte);
            Add(IconType.ShortTag, Properties.Resources.wiki_tag_short);
            Add(IconType.IntTag, Properties.Resources.wiki_tag_int);
            Add(IconType.LongTag, Properties.Resources.wiki_tag_long);
            Add(IconType.FloatTag, Properties.Resources.wiki_tag_float);
            Add(IconType.DoubleTag, Properties.Resources.wiki_tag_double);
            Add(IconType.StringTag, Properties.Resources.wiki_tag_string);
            Add(IconType.ByteArrayTag, Properties.Resources.wiki_tag_byte_array);
            Add(IconType.IntArrayTag, Properties.Resources.wiki_tag_int_array);
            Add(IconType.LongArrayTag, Properties.Resources.wiki_tag_long_array);
            Add(IconType.CompoundTag, Properties.Resources.wiki_tag_compound);
            Add(IconType.ListTag, Properties.Resources.wiki_tag_list);
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
            bool any = false;
            foreach (var entry in zip.Entries)
            {
                var name = entry.FullName;
                if (name.Contains('/'))
                    continue;
                if (Path.GetExtension(name) != ".png")
                    continue;
                var simple = Path.GetFileNameWithoutExtension(name);
                if (!FileNameMap.TryGetValue(simple, out var type))
                    continue;
                var image = Image.FromStream(entry.Open());
                Add(type, image);
                any = true;
            }
            if (!any)
                throw new FormatException("No images were found in this icon set");
        }
    }
}
