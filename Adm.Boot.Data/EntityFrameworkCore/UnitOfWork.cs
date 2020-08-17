﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adm.Boot.Data.EntityFrameworkCore
{
    public class UnitOfWork : IDisposable
    {
        private readonly AdmDbContext _dbContext;

        public UnitOfWork(AdmDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        //开启事务
        public IDbContextTransaction BeginTransaction()
        {
            var scope = _dbContext.Database.BeginTransaction();
            return scope;
        }
        /// <summary>
        /// 提交事务保存
        /// </summary>
        /// <returns></returns>
        public int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }
        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
        /// <summary>
        /// 回滚
        /// </summary>
        public void RollBackChanges()
        {
            var items = _dbContext.ChangeTracker.Entries().ToList();
            items.ForEach(o => o.State = EntityState.Unchanged);
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();//随着工作单元的销毁而销毁
                }
            }
            _disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}
