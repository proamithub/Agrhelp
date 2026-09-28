using System;

namespace HelpRepo
{
    public interface IUnitOfWork : IDisposable
    {
        AdminMasteHelpRepository IAdminMaster { get; }
        int SaveChanges();
    }
}