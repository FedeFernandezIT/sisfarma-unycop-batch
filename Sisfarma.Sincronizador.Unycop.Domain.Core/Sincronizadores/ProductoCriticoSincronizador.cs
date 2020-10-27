﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Sisfarma.Client.Unycop;
using Sisfarma.Sincronizador.Core.Extensions;
using Sisfarma.Sincronizador.Domain.Core.Services;
using Sisfarma.Sincronizador.Domain.Core.Sincronizadores.SuperTypes;
using Sisfarma.Sincronizador.Domain.Entities.Fisiotes;
using UNYCOP = Sisfarma.Client.Unycop.Model;

namespace Sisfarma.Sincronizador.Unycop.Domain.Core.Sincronizadores
{
    public class ProductoCriticoSincronizador : TaskSincronizador
    {
        protected const string TIPO_CLASIFICACION_DEFAULT = "Familia";
        protected const string TIPO_CLASIFICACION_CATEGORIA = "Categoria";
        protected const string SISTEMA_UNYCOP = "unycop";

        private string _clasificacion;

        protected const string LABORATORIO_DEFAULT = "<Sin Laboratorio>";
        protected const string FAMILIA_DEFAULT = "<Sin Clasificar>";
        protected const int STOCK_CRITICO = 0;

        protected Falta _falta;

        public ProductoCriticoSincronizador(IFarmaciaService farmacia, ISisfarmaService fisiotes) :
            base(farmacia, fisiotes)
        { }

        public override void LoadConfiguration()
        {
            base.LoadConfiguration();
            _clasificacion = !string.IsNullOrWhiteSpace(ConfiguracionPredefinida[Configuracion.FIELD_TIPO_CLASIFICACION])
                ? ConfiguracionPredefinida[Configuracion.FIELD_TIPO_CLASIFICACION]
                : TIPO_CLASIFICACION_DEFAULT;
        }

        public override void PreSincronizacion()
        {
            base.PreSincronizacion();
            _falta = _sisfarma.Faltas.LastOrDefault();
        }

        public override void Process()
        {
            // _falta se carga en PreSincronizacion
            var sw = new Stopwatch();
            sw.Start();
            var pedidos = (_falta == null)
                ? _farmacia.Pedidos.GetAllByFechaGreaterOrEqual(DateTime.Now.Date.AddYears(-2)).ToList()
                : _farmacia.Pedidos.GetAllByIdGreaterOrEqual(_falta.idPedido);
            Console.WriteLine($"pedidos recuperados en {sw.ElapsedMilliseconds}ms");
            if (!pedidos.Any())
                return;

            var set = pedidos.SelectMany(x => x.lineasItem).Select(x => x.CNArticulo.ToIntegerOrDefault()).Distinct().ToArray();
            Console.WriteLine($"cant articulos {set.Count()}");
            sw.Restart();
            var sourceFarmacos = _farmacia.Farmacos.GetBySetId(set).ToList();
            Console.WriteLine($"articulos recuperados en {sw.ElapsedMilliseconds}ms");
            var farmacosCriticos = sourceFarmacos.Where(x => x.Stock == STOCK_CRITICO || x.Stock <= x.Stock).ToArray();

            var batchSize = 1000;
            for (int index = 0; index < pedidos.Count(); index += batchSize)
            {
                var pedidosBatch = pedidos.Skip(index).Take(batchSize).ToList();
                foreach (var pedido in pedidosBatch)
                {
                    Task.Delay(5).Wait();

                    _cancellationToken.ThrowIfCancellationRequested();
                    var lineasConProductosCriticos = pedido.lineasItem.Where(x => farmacosCriticos.Any(f => f.CNArticulo == x.CNArticulo)).ToArray();
                    var currentLinea = 0;
                    foreach (var linea in pedido.lineasItem)
                    {
                        Task.Delay(1).Wait();
                        currentLinea++;

                        var farmaco = farmacosCriticos.FirstOrDefault(x => x.CNArticulo == linea.CNArticulo);
                        if (farmaco == null)
                            continue;

                        var tipoFalta = "Normal";
                        if (farmaco.Stock <= farmaco.Stock && farmaco.Stock > 0)
                            tipoFalta = "StockMinimo";

                        if (!_sisfarma.Faltas.ExistsLineaDePedido(linea.IdPedido, currentLinea))
                            _sisfarma.Faltas.Sincronizar(GenerarFaltante(pedido, linea, currentLinea, farmaco), tipoFalta);
                    }

                    if (_falta == null)
                        _falta = new Falta();

                    _falta.idPedido = pedido.IdPedido;
                }
            }
        }

        private Falta GenerarFaltante(UNYCOP.Pedido pedido, UNYCOP.Pedido.Lineasitem lineaPedido, int currentLinea, UNYCOP.Articulo farmaco)
        {
            var culture = UnycopFormat.GetCultureTwoDigitYear();
            var familia = farmaco.NombreFamilia ?? FAMILIA_DEFAULT;

            var fechaPedido = pedido.FechaPedido.ToDateTimeOrDefault(UnycopFormat.FechaCompletaDataBase, culture);
            var fechaActual = DateTime.Now;

            return new Falta
            {
                idPedido = lineaPedido.IdPedido,
                idLinea = currentLinea,
                cod_nacional = farmaco.CNArticulo,
                descripcion = farmaco.Denominacion,
                familia = _clasificacion == TIPO_CLASIFICACION_CATEGORIA
                        ? farmaco.NombreSubCategoria ?? FAMILIA_DEFAULT
                        : familia,
                superFamilia = _clasificacion == TIPO_CLASIFICACION_CATEGORIA
                        ? farmaco.NombreCategoria ?? FAMILIA_DEFAULT
                        : string.Empty,
                categoria = farmaco.NombreCategoria ?? string.Empty,
                subcategoria = farmaco.NombreSubCategoria ?? string.Empty,
                superFamiliaAux = string.Empty,
                familiaAux = _clasificacion == TIPO_CLASIFICACION_CATEGORIA ? familia : string.Empty,
                cambioClasificacion = _clasificacion == TIPO_CLASIFICACION_CATEGORIA,

                cantidadPedida = lineaPedido.Pedidas,
                fechaFalta = fechaActual,
                cod_laboratorio = farmaco.CodLaboratorio ?? string.Empty,
                laboratorio = farmaco.NombreLaboratorio ?? LABORATORIO_DEFAULT,
                proveedor = farmaco.NombreProveedor ?? string.Empty,
                fechaPedido = fechaPedido,
                pvp = (float)farmaco.PVP,
                puc = (float)(farmaco.PC ?? 0m),
                sistema = SISTEMA_UNYCOP
            };
        }
    }
}