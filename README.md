# <img src="NbtStudio/Resources/app_image_256.png" width=48> NBT Studio

### [Download link](https://github.com/tryashtar/nbt-studio/releases)

NBT Studio is an [NBT](https://wiki.vg/NBT) editing application, the spiritual successor to [NBTExplorer](https://github.com/jaquadro/NBTExplorer). It has been rewritten completely from scratch to support a vast array of new features, while keeping the familiar layout of controls. New features include Bedrock support, SNBT support, undo/redo functionality, drag and drop, multiselect, and more! It's called Studio to make it sound more important than it really is.

## <img src="NbtStudio/Resources/action_open_file_image.png" width=16> Files
NBT Studio supports reading and writing the following NBT formats. Features marked with a star are new NBT Studio features not present in NBTExplorer.

* Java NBT files, such as `level.dat`
* Java region files (`.mca` and `.mcr`)
* ☆ Bedrock NBT files, such as `.mcstructure` files (little-endian NBT)
* ☆ SNBT files (stringified NBT, like in commands: `{Enchantments:[{id:sharpness,lvl:10s}]}`)

<img src="NbtStudio/Resources/action_save_image.png" width=16> ☆ NBT files can be exported to any of these formats using `Save as`. You can load an SNBT file, then export it as a little-endian g-zipped binary file if you wish.

<img src="NbtStudio/Resources/action_new_image.png" width=16> ☆ NBT Studio also allows the creation of blank NBT files. You can start from scratch and export to any format. Additionally, you can use `Ctrl-Alt-V` to create a file from SNBT data on your clipboard.

## SNBT Support
NBT Studio is designed around the easy transfer and conversion between textual SNBT data and structured NBT data. As mentioned, SNBT can be opened as a file or pasted as a new document.

* <img src="NbtStudio/Resources/action_add_snbt_image.png" width=16> You can add a tag as SNBT anywhere in the document. Whatever type is parsed will be the resulting type of the tag. Compounds and lists will be fully parsed with all of their children.
* <img src="NbtStudio/Resources/action_edit_snbt_image.png" width=16> Existing tags can also be inspected and edited as SNBT. You can modify entire compounds in-place, or simpler tags.
* <img src="NbtStudio/Resources/action_paste_image.png" width=16> The cut, copy, and paste actions add the tag to your clipboard as SNBT. You can copy multiple tags as text, or import SNBT data obtained elsewhere.

## Tag Editing
NBT Studio adds an assortment of convenience features to make navigating and editing NBT data easier.

* <img src="NbtStudio/Resources/tag_byte_image.png" width=16> The tag creation and edit menus have fields for both tag name and value, so you don't have to do it in two steps
* <img src="NbtStudio/Resources/tag_int_image.png" width=16> Hold `Shift` while adding a tag to skip the menu and select its name automatically
* <img src="NbtStudio/Resources/tag_list_image.png" width=16> NBT lists of type `byte`, `short`, `int`, or `long` can be edited as hex just like arrays
* <img src="NbtStudio/Resources/action_cut_image.png" width=16> Tags can be selected, dragged, and dropped to move them to a different parent
* <img src="NbtStudio/Resources/action_undo_image.png" width=16> Undo and redo functionality with `Ctrl-Z` and `Ctrl-Shift-Z`
* <img src="NbtStudio/Resources/action_delete_image.png" width=16> Deleting tags automatically selects the next tag for easy obliteration
* <img src="NbtStudio/Resources/action_search_image.png" width=16> New and improved search window allows searching by [regular expression](https://en.wikipedia.org/wiki/Regular_expression), as well as an option to select all matching tags
* <img src="NbtStudio/Resources/action_edit_image.png" width=16> Press `Enter` to edit the selected tag
* <img src="NbtStudio/Resources/action_sort_image.png" width=16> Press `Space` to expand/contract the selected tag, or `Ctrl-Space` to expand all
* <img src="NbtStudio/Resources/action_save_image.png" width=16> Right-click on a file to see options to save it, or open it in File Explorer
* <img src="NbtStudio/Resources/tag_compound_image.png" width=16> Right-click on a container tag to see options to add a child tag
* <img src="NbtStudio/Resources/file_image.png" width=16> Files display an asterisk (`*`) to indicate there are unsaved changes

# Credits
This application was written from scratch by myself, tryashtar. However, it would never have existed without these amazing projects that came before me:

### Design
* [NBTExplorer by jaquadro](https://github.com/jaquadro/NBTExplorer)

### Technologies
* [fNbt by mstefarov](https://github.com/mstefarov/fNbt)
* [TreeViewAdv by agaman](https://sourceforge.net/projects/treeviewadv)
* [Be.HexEditor by bernhardelbl](https://sourceforge.net/projects/hexbox)

### Icons
* [Generic icons by Yusuke Kamiyamane](https://p.yusukekamiyamane.com)
* [NBT icons by AmberW](https://github.com/AmberWat)
