using SwissAddressManager.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SwissAddressManager.Services.Interfaces
{
    public interface IUnsavedChangesPage
    {
        bool ConfirmUnsavedChanges();
    }
}
