﻿using Sisfarma.RestClient;
using Sisfarma.Sincronizador.Core.Extensions;
using Sisfarma.Sincronizador.Domain.Core.ExternalServices.Fisiotes;
using Sisfarma.Sincronizador.Domain.Entities.Fisiotes;
using Sisfarma.Sincronizador.Infrastructure.Fisiotes;
using Sisfarma.Sincronizador.Infrastructure.Fisiotes.DTO;
using System;
using FAR = Sisfarma.Sincronizador.Domain.Entities.Farmacia;

namespace Sisfarma.Sincronizador.Unycop.Infrastructure.ExternalServices.Sisfarma
{
    public class ClientesExternalService : FisiotesExternalService, IClientesExternalService, IClientesExternalServiceNew
    {
        public ClientesExternalService(IRestClient restClient, FisiotesConfig config) 
            : base(restClient, config)
        {}

        public bool AnyWithDni(string dni)
        {
            throw new NotImplementedException();
        }

        public string GetDniTrackingLast()
        {
            throw new NotImplementedException();
        }

        public void Insert(string trabajador, string tarjeta, string idCliente, string nombre, string telefono, string direccion, string movil, string email, decimal puntos, long fechaNacimiento, string sexo, string tipo, DateTime? fechaAlta, int baja, int lopd, bool withTrack = false)
        {
            throw new NotImplementedException();
        }

        public void InsertOrUpdate(string trabajador, string tarjeta, string idCliente, string nombre, string telefono, string direccion, string movil, string email, decimal puntos, long fechaNacimiento, string sexo, DateTime? fechaAlta, int baja, int lopd, bool esHueco = false)
        {
            throw new NotImplementedException();
        }

        public void InsertOrUpdate(string trabajador, string tarjeta, string idCliente, string dniCliente, string nombre, string telefono, string direccion, string movil, string email, decimal puntos, long fechaNacimiento, string sexo, DateTime? fechaAlta, int baja, int lopd, bool esHueco = false)
        {
            throw new NotImplementedException();
        }

        public void InsertOrUpdate(string trabajador, string tarjeta, string idCliente, string dniCliente, string nombre, string telefono, string direccion, string movil, string email, long fechaNacimiento, string sexo, DateTime? fechaAlta, int baja, int lopd, bool esHueco = false)
        {
            throw new NotImplementedException();
        }

        public void InsertOrUpdate(Cliente cliente)
        {
            throw new NotImplementedException();
        }

        public void InsertOrUpdateBeBlue(string trabajador, string tarjeta, string idCliente, string dniCliente, string nombre, string telefono, string direccion, string movil, string email, decimal puntos, long fechaNacimiento, string sexo, DateTime? fechaAlta, int baja, int lopd, int esBeBlue, bool esHueco = false)
        {
            throw new NotImplementedException();
        }

        public void InsertOrUpdateBeBlue(string trabajador, string tarjeta, string idCliente, string dniCliente, string nombre, string telefono, string direccion, string movil, string email, long fechaNacimiento, string sexo, DateTime? fechaAlta, int baja, int lopd, int esBeBlue, bool esHueco = false)
        {
            throw new NotImplementedException();
        }

        public void ResetDniTracking()
        {
            _restClient
                .Resource(_config.Clientes.ResetDniTracking)
                .SendPut();
        }

        public void Sincronizar(FAR.Cliente cliente, bool cargarPuntos = false)
        {
            var resource = _config.Clientes.Insert.Replace("{dni}", $"{cliente.Id}");
            Sincronizar(cliente, cargarPuntos, resource);
        }

        public void Sincronizar(FAR.Cliente cliente, bool beBlue, bool cargarPuntos = false)
        {
            var resource = _config.Clientes.Insert.Replace("{dni}", $"{cliente.Id}");
            Sincronizar(cliente, beBlue, cargarPuntos, resource);
        }

        public void SincronizarHueco(FAR.Cliente cliente, bool cargarPuntos = false)
        {
            var resource = _config.Clientes.InsertHueco.Replace("{dni}", $"{cliente.Id}");
            Sincronizar(cliente, cargarPuntos, resource);
        }

        public void SincronizarHueco(FAR.Cliente cliente, bool beBlue, bool cargarPuntos = false)
        {
            var resource = _config.Clientes.InsertHueco.Replace("{dni}", $"{cliente.Id}");
            Sincronizar(cliente, beBlue, cargarPuntos, resource);
        }


        private void Sincronizar(FAR.Cliente cliente, bool cargarPuntos, string resource)
        {
            var clienteToSend = (cargarPuntos) ?
                GenerarAnonymousClientePuntuado(cliente) :
                GenerarAnonymousClienteSinPuntuar(cliente);

            _restClient
                .Resource(resource)
                .SendPut(clienteToSend);
        }

        private void Sincronizar(FAR.Cliente cliente, bool beBlue, bool cargarPuntos, string resource)
        {
            var clienteToSend = (cargarPuntos) ?
                GenerarAnonymousClientePuntuado(cliente, beBlue) :
                GenerarAnonymousClienteSinPuntuar(cliente, beBlue);

            _restClient
                .Resource(resource)
                .SendPut(clienteToSend);
        }

        private object GenerarAnonymousClientePuntuado(FAR.Cliente cliente)
        {
            return new
            {
                dni_tra = "0",                
                tarjeta = cliente.Tarjeta,
                dniCliente = cliente.NumeroIdentificacion,
                apellidos = cliente.NombreCompleto.Strip(),
                telefono = cliente.Telefono.Strip(),
                direccion = cliente.Direccion.Strip(),
                movil = cliente.Celular.Strip(),
                email = cliente.Email,
                fecha_nacimiento = cliente.FechaNacimiento.ToDateInteger(),
                puntos = cliente.Puntos,
                sexo = cliente.Sexo,
                fechaAlta = cliente.FechaAlta.ToIsoString(),
                baja = cliente.Baja.ToInteger(),
                estado_civil = cliente.EstadoCivil.Strip(),
                lopd = cliente.LOPD.ToInteger()                
            };
        }

        private object GenerarAnonymousClientePuntuado(FAR.Cliente cliente, bool beBlue)
        {
            return new
            {
                dni_tra = "0",
                tarjeta = cliente.Tarjeta,
                dniCliente = cliente.NumeroIdentificacion,
                apellidos = cliente.NombreCompleto.Strip(),
                telefono = cliente.Telefono.Strip(),
                direccion = cliente.Direccion.Strip(),
                movil = cliente.Celular.Strip(),
                email = cliente.Email,
                fecha_nacimiento = cliente.FechaNacimiento.ToDateInteger(),
                puntos = cliente.Puntos,
                sexo = cliente.Sexo,
                fechaAlta = cliente.FechaAlta.ToIsoString(),
                baja = cliente.Baja.ToInteger(),
                estado_civil = cliente.EstadoCivil.Strip(),
                lopd = cliente.LOPD.ToInteger(),
                beBlue = beBlue.ToInteger()
            };
        }

        private object GenerarAnonymousClienteSinPuntuar(FAR.Cliente cliente)
        {
            return new
            {
                dni_tra = "0",
                tarjeta = cliente.Tarjeta,
                dniCliente = cliente.NumeroIdentificacion,
                apellidos = cliente.NombreCompleto.Strip(),
                telefono = cliente.Telefono.Strip(),
                direccion = cliente.Direccion.Strip(),
                movil = cliente.Celular.Strip(),
                email = cliente.Email,
                fecha_nacimiento = cliente.FechaNacimiento.ToDateInteger(),                
                sexo = cliente.Sexo,
                fechaAlta = cliente.FechaAlta.ToIsoString(),
                baja = cliente.Baja.ToInteger(),
                estado_civil = cliente.EstadoCivil.Strip(),
                lopd = cliente.LOPD.ToInteger()
            };
        }

        private object GenerarAnonymousClienteSinPuntuar(FAR.Cliente cliente, bool beBlue)
        {
            return new
            {
                dni_tra = "0",
                tarjeta = cliente.Tarjeta,
                dniCliente = cliente.NumeroIdentificacion,
                apellidos = cliente.NombreCompleto.Strip(),
                telefono = cliente.Telefono.Strip(),
                direccion = cliente.Direccion.Strip(),
                movil = cliente.Celular.Strip(),
                email = cliente.Email,
                fecha_nacimiento = cliente.FechaNacimiento.ToDateInteger(),
                sexo = cliente.Sexo,
                fechaAlta = cliente.FechaAlta.ToIsoString(),
                baja = cliente.Baja.ToInteger(),
                estado_civil = cliente.EstadoCivil.Strip(),
                lopd = cliente.LOPD.ToInteger(),
                beBlue = beBlue.ToInteger()
            };
        }        

        public void Update(string trabajador, string tarjeta, string nombre, string telefono, string direccion, string movil, string email, decimal puntos, long fechaNacimiento, string sexo, DateTime? fechaAlta, int baja, int lopd, string idCliente, bool withTrack = false)
        {
            throw new NotImplementedException();
        }

        public void UpdatePuntos(UpdatePuntaje pp)
        {
            throw new NotImplementedException();
        }
    }
}
