﻿using Sisfarma.Sincronizador.Core.Config;
using Sisfarma.Sincronizador.Domain.Core.Repositories.Farmacia;
using Sisfarma.Sincronizador.Domain.Entities.Farmacia;
using Sisfarma.Sincronizador.Unycop.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;

namespace Sisfarma.Sincronizador.Unycop.Infrastructure.Repositories.Farmacia
{
    public class CategoriasRepository : FarmaciaRepository, ICategoriasRepository
    {        
        public CategoriasRepository(LocalConfig config) : base(config)
        { }

        public CategoriasRepository() { }
        

        public IEnumerable<Categoria> GetAll()
        {
            try
            {
                using (var db = FarmaciaContext.Default())
                {
                    var sql = "select Nombre from categorias";
                    return db.Database.SqlQuery<Categoria>(sql)
                        .ToList();
                }
            }
            catch (Exception ex) when (ex.Message.Contains(FarmaciaContext.MessageUnderlyngProviderFailed))
            {
                return GetAll();
            }
            
        }

        public IEnumerable<Categoria> GetAllByDescripcion()
        {
            try
            {
                var rs = new List<Categoria>();
                using (var db = FarmaciaContext.Default())
                {
                    var sql = @"select IDCategoria as Id, Nombre from categorias WHERE nombre NOT IN ('ESPECIALIDAD', 'EFP', 'SIN FAMILIA') AND nombre NOT LIKE '%ESPECIALIDADES%' AND nombre NOT LIKE '%Medicamento%'";
                    rs = db.Database.SqlQuery<DTO.Categoria>(sql)
                        .Select(x => new Categoria { Id = x.Id, Nombre = x.Nombre })
                            .ToList();
                }

                rs.ForEach(item =>
                    item.Subcategorias = GetAllNombreSubcategoriaByCategoriaId(item.Id));
                return rs;
            }
            catch (Exception ex) when (ex.Message.Contains(FarmaciaContext.MessageUnderlyngProviderFailed))
            {
                return GetAllByDescripcion();
            }            
        }

        public IEnumerable<string> GetAllNombreSubcategoriaByCategoriaId(long id)
        {
            try
            {
                using (var db = FarmaciaContext.Default())
                {
                    var sql = @"SELECT nombre FROM Subcategorias WHERE  IdCategoria = @id";
                    return db.Database.SqlQuery<string>(sql,
                        new OleDbParameter("id", (int)id))
                        .ToList();
                }
            }
            catch (Exception ex) when (ex.Message.Contains(FarmaciaContext.MessageUnderlyngProviderFailed))
            {
                return GetAllNombreSubcategoriaByCategoriaId(id);
            }            
        }

        public string GetSubCategoriaById(string id)
        {
            try
            {
                using (var db = FarmaciaContext.Default())
                {
                    var sql = @"SELECT nombre FROM Subcategorias WHERE  IdCategoria = @id";
                    return db.Database.SqlQuery<string>(sql,
                        new OleDbParameter("id", int.Parse(id)))
                        .FirstOrDefault();
                }
            }
            catch (Exception ex) when (ex.Message.Contains(FarmaciaContext.MessageUnderlyngProviderFailed))
            {
                return GetSubCategoriaById(id);
            }            
        }
    }
}