using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtStudio
{
    public interface IHavePath
    {
        string Path { get; }
    }

    public interface ISaveable
    {
        event Action OnSaved;
        bool HasUnsavedChanges { get; }
        bool CanSave { get; }
        void Save();
    }

    public interface IExportable
    {
        void SaveAs(string path);
    }

    public interface IRefreshable
    {
        bool CanRefresh { get; }
        void Refresh();
    }

    public interface IFile : IHavePath, ISaveable, IExportable, IRefreshable
    {
    }
}
