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
    public class PedidosRepository : FarmaciaRepository, IPedidosRepository
    {
        private readonly IProveedorRepository _proveedorRepository;
        private readonly IFarmacoRepository _farmacoRepository;
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IFamiliaRepository _familiaRepository;
        private readonly ILaboratorioRepository _laboratorioRepository;

        private readonly decimal _factorCentecimal = 0.01m;

        public PedidosRepository(LocalConfig config) : base(config)
        { }

        public PedidosRepository(
            IProveedorRepository proveedorRepository,
            IFarmacoRepository farmacoRepository,
            ICategoriaRepository categoriaRepository,
            IFamiliaRepository familiaRepository,
            ILaboratorioRepository laboratorioRepository)
        {
            _proveedorRepository = proveedorRepository ?? throw new ArgumentNullException(nameof(proveedorRepository));
            _farmacoRepository = farmacoRepository ?? throw new ArgumentNullException(nameof(farmacoRepository));
            _categoriaRepository = categoriaRepository ?? throw new ArgumentNullException(nameof(categoriaRepository));
            _familiaRepository = familiaRepository ?? throw new ArgumentNullException(nameof(familiaRepository));
            _laboratorioRepository = laboratorioRepository ?? throw new ArgumentNullException(nameof(laboratorioRepository));
        }

        public IEnumerable<Pedido> GetAllByFechaGreaterOrEqual(DateTime fecha)
        {
            try
            {
                var rs = Enumerable.Empty<DTO.Pedido>();
                using (var db = FarmaciaContext.Proveedores())
                {
                    var sql = @"SELECT ID_NumPedido as Id, ID_Proveedor as Proveedor, ID_Farmaco as Farmaco, CantInicial, Fecha From recibir WHERE datevalue(Fecha) >= DateValue (@fecha) Order by ID_NumPedido ASC";
                    rs = db.Database.SqlQuery<DTO.Pedido>(sql,
                        new OleDbParameter("fecha", fecha))
                            .Take(10)
                            .ToList();
                }

                var keys = rs.GroupBy(k => new PedidoCompositeKey { Id = k.Id, Proveedor = k.Proveedor });
                return GenerarPedidos(keys);
            }
            catch (Exception ex) when (ex.Message.Contains(FarmaciaContext.MessageUnderlyngProviderFailed))
            {
                return GetAllByFechaGreaterOrEqual(fecha);
            }
        }

        internal class PedidoCompositeKey
        {
            internal short Id { get; set; }

            internal int Proveedor { get; set; }
        }

        public IEnumerable<Pedido> GetAllByIdGreaterOrEqual(long pedido)
        {
            try
            {
                var rs = Enumerable.Empty<DTO.Pedido>();
                using (var db = FarmaciaContext.Proveedores())
                {
                    var sql = @"SELECT ID_NumPedido as Id, ID_Proveedor as Proveedor, ID_Farmaco as Farmaco, CantInicial, Fecha From recibir WHERE ID_NumPedido >= @pedido Order by ID_NumPedido ASC";
                    rs = db.Database.SqlQuery<DTO.Pedido>(sql,
                        new OleDbParameter("pedido", (int)pedido))
                            .Take(10)
                            .ToList();
                }

                var keys = rs.GroupBy(k => new PedidoCompositeKey { Id = k.Id, Proveedor = k.Proveedor });
                return GenerarPedidos(keys);
            }
            catch (Exception ex) when (ex.Message.Contains(FarmaciaContext.MessageUnderlyngProviderFailed))
            {
                return GetAllByIdGreaterOrEqual(pedido);
            }
        }

        private IEnumerable<Pedido> GenerarPedidos(IEnumerable<IGrouping<PedidoCompositeKey, DTO.Pedido>> groups)
        {
            var pedidos = new List<Pedido>();
            foreach (var group in groups)
            {
                var linea = 0;
                var fecha = group.FirstOrDefault()?.Fecha;
                var detalle = new List<PedidoDetalle>();
                foreach (var item in group)
                {
                    var pedidoDetalle = new PedidoDetalle()
                    {
                        Linea = ++linea,
                        CantidadPedida = item.CantInicial,
                        PedidoId = item.Id
                    };

                    var farmaco = _farmacoRepository.GetOneOrDefaultById(item.Farmaco);
                    if (farmaco != null)
                    {
                        var pcoste = farmaco.PrecioUnicoEntrada.HasValue && farmaco.PrecioUnicoEntrada != 0
                            ? (decimal)farmaco.PrecioUnicoEntrada.Value
                            : ((decimal?)farmaco.PrecioMedio ?? 0m);

                        //var proveedor = _proveedorRepository.GetOneOrDefaultByCodigoNacional(farmaco.Id);
                        var proveedor = _proveedorRepository.GetOneOrDefaultById(item.Proveedor);

                        var categoria = farmaco.CategoriaId.HasValue
                            ? _categoriaRepository.GetOneOrDefaultById(farmaco.CategoriaId.Value)
                            : null;

                        var subcategoria = farmaco.CategoriaId.HasValue && farmaco.SubcategoriaId.HasValue
                            ? _categoriaRepository.GetSubcategoriaOneOrDefaultByKey(
                                farmaco.CategoriaId.Value,
                                farmaco.SubcategoriaId.Value)
                            : null;

                        var familia = _familiaRepository.GetOneOrDefaultById(farmaco.FamiliaId);
                        var laboratorio = _laboratorioRepository.GetOneOrDefaultByCodigo(farmaco.CodigoLaboratorio);

                        pedidoDetalle.Farmaco = new Farmaco
                        {
                            Id = farmaco.Id,
                            Codigo = item.Farmaco.ToString(),
                            PrecioCoste = pcoste,
                            Proveedor = proveedor,
                            Categoria = categoria,
                            Subcategoria = subcategoria,
                            Familia = familia,
                            Laboratorio = laboratorio,
                            Denominacion = farmaco.Denominacion,
                            Precio = farmaco.PVP,
                            Stock = farmaco.ExistenciasAux ?? 0
                        };

                        detalle.Add(pedidoDetalle);

                        pedidos.Add(new Pedido { Id = group.Key.Id, Fecha = fecha }.AddRangeDetalle(detalle));
                    }
                }
            }
            return pedidos;
        }
    }
}