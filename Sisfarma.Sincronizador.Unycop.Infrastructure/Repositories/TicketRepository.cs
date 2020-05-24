﻿using Sisfarma.Sincronizador.Core.Config;
using Sisfarma.Sincronizador.Unycop.Infrastructure.Data;
using Sisfarma.Sincronizador.Unycop.Infrastructure.Repositories.Farmacia.DTO;
using System;
using System.Data.OleDb;
using System.Linq;

namespace Sisfarma.Sincronizador.Unycop.Infrastructure.Repositories.Farmacia
{
    public interface ITicketRepository
    {
        Ticket GetOneOrdefaultByVentaId(long venta, int year);
    }


    public class TicketRepository : FarmaciaRepository, ITicketRepository
    {
        public TicketRepository(LocalConfig config) : base(config)
        { }

        public TicketRepository()
        { }

        public Ticket GetOneOrdefaultByVentaId(long venta, int year)
        {
            try
            {
                var ventaInteger = (int)venta;

                using (var db = FarmaciaContext.VentasByYear(year))
                {
                    var sql = @"SELECT Id_Ticket as Numero, Serie FROM Tickets_D WHERE Id_Venta = @venta";
                    return db.Database.SqlQuery<Ticket>(sql,
                        new OleDbParameter("venta", ventaInteger))
                        .FirstOrDefault();
                }
            }
            catch (Exception ex) when (ex.Message.Contains(FarmaciaContext.MessageUnderlyngProviderFailed))
            {
                return GetOneOrdefaultByVentaId(venta, year);
            }            
        }
    }
}
