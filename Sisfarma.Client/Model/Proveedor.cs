﻿using Sisfarma.Sincronizador.Core.Extensions;

namespace Sisfarma.Client.Model
{
    public class Proveedor
    {
        public Proveedor(string idProveedor, string nombre)
        {
            this.idProveedor = idProveedor;
            this.nombre = nombre.Strip();
        }

        public string idProveedor { get; set; }

        public string nombre { get; set; }
    }
}