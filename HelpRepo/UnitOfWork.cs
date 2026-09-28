using Microsoft.Extensions.Configuration;
using HelpData;
using HelpData.Dapper;
using HelpRepo.Utilities;
using System;

namespace HelpRepo
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext db;
         private IMailClient _mailClient; 
        IDapperContext _dapperContext;
        protected IConfiguration config;
        public UnitOfWork(DataContext _db, IDapperContext dapperContext, IMailClient mailClient, IConfiguration _config)
        {
            db = _db;
            _dapperContext = dapperContext;
            config = _config;
        }

        private AdminMasteHelpRepository _IAdminMaster;
        public AdminMasteHelpRepository IAdminMaster
        {
            get
            {
                if (this._IAdminMaster == null)
                {
                    this._IAdminMaster = new AdminMasteHelpRepository(db, _dapperContext, config);
                }
                return this._IAdminMaster;
            }
        }


        public int SaveChanges()
        {
            return db.SaveChanges();
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}
