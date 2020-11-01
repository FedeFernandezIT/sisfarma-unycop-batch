﻿using System;
using Sisfarma.Sincronizador.Unycop.Domain.Core.Repositories.Farmacia;

namespace Sisfarma.Sincronizador.Unycop.Domain.Core.UnitOfWorks
{
    public interface IFarmaciaService
    {
        IVentasRepository Ventas { get; }

        IClientesRepository Clientes { get; }

        IFarmacosRepository Farmacos { get; }

        IPedidosRepository Pedidos { get; }

        IEncargosRepository Encargos { get; }

        IListaRepository Listas { get; }

        IRecepcionRespository Recepciones { get; }
    }

    public class FarmaciaService : IFarmaciaService
    {
        public FarmaciaService(
            IVentasRepository ventas,
            IClientesRepository clientes,
            IFarmacosRepository farmacos,
            IPedidosRepository pedidos,
            IEncargosRepository encargos,
            IListaRepository listas,
            IRecepcionRespository recepciones)
        {
            Ventas = ventas ?? throw new ArgumentNullException(nameof(ventas));
            Clientes = clientes ?? throw new ArgumentNullException(nameof(clientes));
            Farmacos = farmacos ?? throw new ArgumentNullException(nameof(farmacos));
            Pedidos = pedidos ?? throw new ArgumentNullException(nameof(pedidos));
            Encargos = encargos ?? throw new ArgumentNullException(nameof(encargos));
            Listas = listas ?? throw new ArgumentNullException(nameof(listas));
            Recepciones = recepciones ?? throw new ArgumentNullException(nameof(recepciones));
        }

        public IVentasRepository Ventas { get; }

        public IClientesRepository Clientes { get; }

        public IFarmacosRepository Farmacos { get; }

        public IPedidosRepository Pedidos { get; }

        public IEncargosRepository Encargos { get; }

        public IListaRepository Listas { get; }

        public IRecepcionRespository Recepciones { get; set; }
    }
}