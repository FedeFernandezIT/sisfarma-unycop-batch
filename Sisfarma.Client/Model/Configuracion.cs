namespace Sisfarma.Client.Model
{
    public partial class Configuracion
    {
        public string campo { get; set; }

        public string valor { get; set; }

        public const string FIELD_STOCK_ENTRADA = "fechaActualizacionStockEntrada";
        public const string FIELD_STOCK_SALIDA = "fechaActualizacionStockSalida";
        public const string FIELD_POR_DONDE_VOY_CON_STOCK = "porDondeVoyConStock";
        public const string FIELD_POR_DONDE_VOY_SIN_STOCK = "porDondeVoySinStock";
        public const string FIELD_POR_DONDE_VOY_BORRAR = "porDondeVoyBorrar";
        public const string FIELD_POR_DONDE_VOY_ENTREGAS_CLIENTES = "porDondeEntregasClientes";
        public const string FIELD_POR_DONDE_VOY_VENTA_MES = "porDondeVoyActualizarVentasMes";
        public const string FIELD_POR_DONDE_VOY_VENTA_MES_ID = "porDondeVoyActualizarVentasMesIdVenta";
        public const string FIELD_POR_DONDE_VOY_PAGOS = "porDondeActualizandoPagos";
        public const string FIELD_REVISAR_VENTA_MES_DESDE = "revisarVentasDesdeMeses";
        public const string FIELD_FECHA_PUNTOS = "fechaPuntos";
        public const string FIELD_CARGAR_PUNTOS = "cargarPuntos";
        public const string FIELD_SOLO_PUNTOS_CON_TARJETA = "soloPuntosConTarjeta";
        public const string FIELD_CANJEO_PUNTOS = "canjeoPuntos";
        public const string FIELD_LOG_ERRORS = "logErrors";
        public const string FIELD_ENCENDIDO = "estadoSincro";
        public const string FIELD_ANIO_INICIO = "anioInicioSincro";
        public const string FIELD_PUNTOS_SISFARMA = "puntosPorSisfarma";
        public const string FIELD_COPIAS_CLIENTES = "copiarClientes";
        public const string FIELD_ES_FARMAZUL = "esFarmazul";
        public const string FIELD_TIPO_CLASIFICACION = "clasificar";
        public const string FIELD_TIPO_BEBLUE = "valorBeBlue";
    }
}