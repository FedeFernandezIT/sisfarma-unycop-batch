﻿using Microsoft.Win32;
using Sisfarma.ClickOnce;
using Sisfarma.Sincronizador.Domain.Core.Sincronizadores;
using Sisfarma.Sincronizador.Unycop.Domain.Core.Factories;
using Sisfarma.Sincronizador.Unycop.IoC.Factories;
using Sisfarma.Sincronizador.Unycop.Properties;
using System;
using System.Configuration;
using System.Deployment.Application;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sisfarma.Sincronizador.Unycop
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            if (!AppProcessHelper.SetSingleInstance())
            {
                Environment.Exit(-1);
            }

            ServicePointManager.DefaultConnectionLimit = 100;

            RegisterStartup(Globals.ProductName);
            var clickOnce = new ClickOnceHelper(Globals.PublisherName, Globals.ProductName);
            clickOnce.UpdateUninstallParameters();

            Initialize();

            //  SisfarmaFactory.Create().Configuraciones.Update("versionSincronizador", $"{ApplicationDeployment.CurrentDeployment.CurrentVersion}");
            SisfarmaFactory.Create().Configuraciones.Update("versionSincronizador", "4.0");

            SincronizadorTaskManager.TaskSincronizadores
                //.AddSincronizador(new Domain.Core.Sincronizadores.PuntoPendienteSincronizador(
                //    farmacia: FarmaciaFactory.Create(),
                //    fisiotes: SisfarmaFactory.Create()),
                //    delay: SincronizadorTaskManager.DelayPuntosPendiente)
                //.AddSincronizador(new Domain.Core.Sincronizadores.ClienteSincronizador(
                //        farmacia: FarmaciaFactory.Create(),
                //        fisiotes: SisfarmaFactory.Create())
                //            .SetHorarioVaciemientos("1500", "2300"),
                //        delay: SincronizadorTaskManager.DelayClientes)
                .AddSincronizador(new Domain.Core.Sincronizadores.CategoriaSincronizador(
                        farmacia: FarmaciaFactory.Create(),
                        fisiotes: SisfarmaFactory.Create()),
                        delay: SincronizadorTaskManager.DelayCategoria)
                //.AddSincronizador(new Domain.Core.Sincronizadores.ProductoBorradoActualizacionSincronizador(
                //        farmacia: FarmaciaFactory.Create(),
                //        fisiotes: SisfarmaFactory.Create()),
                //        delay: SincronizadorTaskManager.DelayProductosBorrar)
                //.AddSincronizador(new Domain.Core.Sincronizadores.ControlStockSincronizador(
                //        farmacia: FarmaciaFactory.Create(),
                //        fisiotes: SisfarmaFactory.Create()),
                //        delay: SincronizadorTaskManager.DelayControlStock)
                //.AddSincronizador(new Domain.Core.Sincronizadores.ControlSinStockSincronizador(
                //        farmacia: FarmaciaFactory.Create(),
                //        fisiotes: SisfarmaFactory.Create()),
                //        delay: SincronizadorTaskManager.DelayControlStock)
                //.AddSincronizador(new Domain.Core.Sincronizadores.ProductoCriticoSincronizador(
                //        farmacia: FarmaciaFactory.Create(),
                //        fisiotes: SisfarmaFactory.Create()),
                //        delay: SincronizadorTaskManager.DelayProductosCriticos)
                //.AddSincronizador(new Domain.Core.Sincronizadores.EncargoSincronizador(
                //        farmacia: FarmaciaFactory.Create(),
                //        fisiotes: SisfarmaFactory.Create()),
                //        delay: SincronizadorTaskManager.DelayEncargos)
                //.AddSincronizador(new Domain.Core.Sincronizadores.FamiliaSincronizador(
                //        farmacia: FarmaciaFactory.Create(),
                //        fisiotes: SisfarmaFactory.Create()),
                //        delay: SincronizadorTaskManager.DelayFamilia)
                //.AddSincronizador(new Domain.Core.Sincronizadores.ListaSincronizador(
                //        farmacia: FarmaciaFactory.Create(),
                //        fisiotes: SisfarmaFactory.Create()),
                //        delay: SincronizadorTaskManager.DelayListas)
                //.AddSincronizador(new Domain.Core.Sincronizadores.SinonimoSincronizador(
                //            farmacia: FarmaciaFactory.Create(),
                //            fisiotes: SisfarmaFactory.Create())
                //        .SetHorarioVaciamientos("1000", "1230", "1730", "1930"),
                //    delay: SincronizadorTaskManager.DelaySinomimos)
                //.AddSincronizador(new Domain.Core.Sincronizadores.PedidoSincronizador(
                //        farmacia: FarmaciaFactory.Create(),
                //        fisiotes: SisfarmaFactory.Create()),
                //        delay: SincronizadorTaskManager.DelayPedidos)
                //.AddSincronizador(new Domain.Core.Sincronizadores.ProveedorSincronizador(
                //        farmacia: FarmaciaFactory.Create(),
                //        fisiotes: SisfarmaFactory.Create()),
                //        delay: SincronizadorTaskManager.DelayProveedores)
                ;

            // TODO: Clientes SUCCESS
            // TODO: Categoria SUCCESS
            // TODO: Familia SUCCESS
            // TODO: Encargos SUCCESS
            // TODO: Sinonimos SUCCESS
            // TODO: Pedido SUCCESS
            // TODO: Proveedores SUCCESS
            // TODO: PuntoPendiente SUCCESS
            // TODO: ProductoBorrado SUCCESS
            // TODO: ControlStock SUCCESS
            // TODO: ControlSinStock SUCCESS
            // TODO: ProductoCritico SUCCESS
            // TODO: Lista SUCCESS

            //Task.Factory.StartNew(() => new Domain.Core.Sincronizadores.SinonimoSincronizador(FarmaciaFactory.Create(), SisfarmaFactory.Create())
            //    .SetHorarioVaciamientos("1000", "1230", "1730", "1930")
            //        .SincronizarAsync(Updater.GetCancellationToken(), delayLoop: 1));
            //Task.Factory.StartNew(() => new Domain.Core.Sincronizadores.ListaSincronizador(
            //            farmacia: FarmaciaFactory.Create(),
            //            fisiotes: SisfarmaFactory.Create())
            //                .SincronizarAsync(Updater.GetCancellationToken(), delayLoop: SincronizadorTaskManager.DelayListas));
            Task.Factory.StartNew(() => new PowerSwitchProgramado(SisfarmaFactory.Create()).SincronizarAsync(Updater.GetCancellationToken(), delayLoop: 60000));
            Task.Factory.StartNew(() => new PowerSwitchManual(SisfarmaFactory.Create()).SincronizarAsync(Updater.GetCancellationToken(), delayLoop: 60000));
            Task.Factory.StartNew(() => new UpdateVersionSincronizador().SincronizarAsync(new CancellationToken(), delayLoop: 20000));

            var notifyIcon = new NotifyIcon();
            notifyIcon.ContextMenuStrip = GetSincronizadorMenuStrip();
            notifyIcon.Icon = Resources.sync;
            notifyIcon.Visible = true;
            Application.ApplicationExit += ApplicationExit;
            Application.ApplicationExit += (sender, @event) => notifyIcon.Visible = false;
            Application.Run(new SincronizadorApplication());
        }

        private static void ApplicationExit(object sender, EventArgs e)
        {
            // last change for cleanup code here!

            // only restart if user requested, not an unhandled app exception...
            AppProcessHelper.RestartIfRequired();
        }

        private static ContextMenuStrip GetSincronizadorMenuStrip()
        {
            var cms = new ContextMenuStrip();
            // cms.Items.Add($"Salir {ApplicationDeployment.CurrentDeployment.CurrentVersion}", null, (sender, @event) => Application.Exit());
            cms.Items.Add($"Salir", null, (sender, @event) => Application.Exit());
            return cms;
        }

        private static void Initialize()
        {
            //new UnycopClient().ExtractArticulos();
            //Task.Factory.StartNew(() => new UnycopClient().ExtractArticulos());
            //Task.Factory.StartNew(() => new UnycopClient().ExtractArticulos());
            try
            {
                var dir = ConfigurationManager.AppSettings["Directory.Setup"];

                var path = ConfigurationManager.AppSettings["File.Remote.Server"];
                var stream = new StreamReader(Path.Combine(dir, path));
                var remoteServer = stream.ReadLine();

                remoteServer = remoteServer.Replace("https://sisfarma.es", "https://api.sisfarma.es");
                remoteServer = remoteServer.Replace("https://sisfarma.pro", "https://api.sisfarma.pro");
                remoteServer = remoteServer.Replace("https://sistemasfarmaceuticos.es", "https://api.sistemasfarmaceuticos.es");

                var remoteToken = stream.ReadLine();
                SisfarmaFactory.Setup(remoteServer, remoteToken);
            }
            catch (IOException)
            {
                throw new IOException("Ha habido un error en la lectura de algún fichero de configuración. Compruebe que existen dichos ficheros de configuración.");
            }
        }

        internal static void RegisterStartup(string productName)
        {
            if (!ApplicationDeployment.IsNetworkDeployed)
                return;

            var location = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Programs),
                @"Sisfarma.es", @"Sisfarma", "Sincronizador.Unycop.appref-ms");

            RegistryKey reg = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            reg.SetValue(productName, location);
        }
    }
}