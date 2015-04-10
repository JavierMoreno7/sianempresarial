using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Text;

public class CProducto_Datos
{
    public int intProductoID { get; set; }
    public decimal? dcmExistencia { get; set; }
    public int? intCompraID { get; set; }
    public DateTime? dtCompra_fecha { get; set; }
    public decimal? dcmCompra_cantidad { get; set; }
    public decimal? dcmCompra_costo { get; set; }
    public decimal? dcmCompra_total { get; set; }
    public decimal? dcmCompra_cantidad_total { get; set; }
    public decimal? dcmCompra_suma_total { get; set; }
    public decimal? dcmCompra_promedio { get; set; }
    public int? intCompra_minID { get; set; }
    public decimal? dcmCompra_costo_min { get; set; }
    public int? intCompra_maxID { get; set; }
    public decimal? dcmCompra_costo_max { get; set; }
    public int? intFacturaID { get; set; }
    public DateTime? dtFactura_fecha { get; set; }
    public decimal? dcmFactura_cantidad { get; set; }
    public decimal? dcmFactura_costo { get; set; }
    public decimal? dcmFactura_total { get; set; }
    public decimal? dcmFactura_cantidad_total { get; set; }
    public decimal? dcmFactura_suma_total { get; set; }
    public decimal? dcmFactura_promedio { get; set; }
    public int? intNotaID { get; set; }
    public DateTime? dtNota_fecha { get; set; }
    public decimal? dcmNota_cantidad { get; set; }
    public decimal? dcmNota_costo { get; set; }
    public decimal? dcmNota_total { get; set; }
    public decimal? dcmNota_cantidad_total { get; set; }
    public decimal? dcmNota_suma_total { get; set; }
    public decimal? dcmNota_promedio { get; set; }
    public decimal? dcmVenta_cantidad_total { get; set; }
    public decimal? dcmVenta_suma_total { get; set; }
    public decimal? dcmVenta_promedio { get; set; }
    public int? intVenta_minID { get; set; }
    public bool isVenta_min_fact { get; set; }
    public decimal? dcmVenta_costo_min { get; set; }
    public int? intVenta_maxID { get; set; }
    public bool isVenta_max_fact { get; set; }
    public decimal? dcmVenta_costo_max { get; set; }

    public void Leer()
    {
        string strQuery = "SELECT existencia" +
                         ", compraID" +
                         ", compra_fecha" +
                         ", compra_cantidad" +
                         ", compra_costo" +
                         ", compra_total" +
                         ", compra_cantidad_total" +
                         ", compra_suma_total" +
                         ", compra_promedio" +
                         ", compra_minID" +
                         ", compra_costo_min" +
                         ", compra_maxID" +
                         ", compra_costo_max" +
                         ", facturaID" +
                         ", factura_fecha" +
                         ", factura_cantidad" +
                         ", factura_costo" +
                         ", factura_total" +
                         ", factura_cantidad_total" +
                         ", factura_suma_total" +
                         ", factura_promedio" +
                         ", notaID" +
                         ", nota_fecha" +
                         ", nota_cantidad" +
                         ", nota_costo" +
                         ", nota_total" +
                         ", nota_cantidad_total" +
                         ", nota_suma_total" +
                         ", nota_promedio" +
                         ", venta_cantidad_total" +
                         ", venta_suma_total" +
                         ", venta_promedio" +
                         ", venta_minID" +
                         ", venta_min_fact" +
                         ", venta_costo_min" +
                         ", venta_maxID" +
                         ", venta_max_fact" +
                         ", venta_costo_max" +
                         " FROM producto_datos" +
                         " WHERE productoID =" + intProductoID;

        DataSet objDataResult = CComunDB.CCommun.Ejecutar_SP(strQuery);

        if (objDataResult.Tables[0].Rows.Count > 0)
        {
            dcmExistencia = (decimal)objDataResult.Tables[0].Rows[0][0];
            if (!objDataResult.Tables[0].Rows[0].IsNull(1)) intCompraID = (int)objDataResult.Tables[0].Rows[0][1];
            if (!objDataResult.Tables[0].Rows[0].IsNull(2)) dtCompra_fecha = (DateTime)objDataResult.Tables[0].Rows[0][2];
            if (!objDataResult.Tables[0].Rows[0].IsNull(3)) dcmCompra_cantidad = (decimal)objDataResult.Tables[0].Rows[0][3];
            if (!objDataResult.Tables[0].Rows[0].IsNull(4)) dcmCompra_costo = (decimal)objDataResult.Tables[0].Rows[0][4];
            if (!objDataResult.Tables[0].Rows[0].IsNull(5)) dcmCompra_total = (decimal)objDataResult.Tables[0].Rows[0][5];
            if (!objDataResult.Tables[0].Rows[0].IsNull(6)) dcmCompra_cantidad_total = (decimal)objDataResult.Tables[0].Rows[0][6];
            if (!objDataResult.Tables[0].Rows[0].IsNull(7)) dcmCompra_suma_total = (decimal)objDataResult.Tables[0].Rows[0][7];
            if (!objDataResult.Tables[0].Rows[0].IsNull(8)) dcmCompra_promedio = (decimal)objDataResult.Tables[0].Rows[0][8];
            if (!objDataResult.Tables[0].Rows[0].IsNull(9)) intCompra_minID = (int)objDataResult.Tables[0].Rows[0][9];
            if (!objDataResult.Tables[0].Rows[0].IsNull(10)) dcmCompra_costo_min = (decimal)objDataResult.Tables[0].Rows[0][10];
            if (!objDataResult.Tables[0].Rows[0].IsNull(11)) intCompra_maxID = (int)objDataResult.Tables[0].Rows[0][11];
            if (!objDataResult.Tables[0].Rows[0].IsNull(12)) dcmCompra_costo_max = (decimal)objDataResult.Tables[0].Rows[0][12];
            if (!objDataResult.Tables[0].Rows[0].IsNull(13)) intFacturaID = (int)objDataResult.Tables[0].Rows[0][13];
            if (!objDataResult.Tables[0].Rows[0].IsNull(14)) dtFactura_fecha = (DateTime)objDataResult.Tables[0].Rows[0][14];
            if (!objDataResult.Tables[0].Rows[0].IsNull(15)) dcmFactura_cantidad = (decimal)objDataResult.Tables[0].Rows[0][15];
            if (!objDataResult.Tables[0].Rows[0].IsNull(16)) dcmFactura_costo = (decimal)objDataResult.Tables[0].Rows[0][16];
            if (!objDataResult.Tables[0].Rows[0].IsNull(17)) dcmFactura_total = (decimal)objDataResult.Tables[0].Rows[0][17];
            if (!objDataResult.Tables[0].Rows[0].IsNull(18)) dcmFactura_cantidad_total = (decimal)objDataResult.Tables[0].Rows[0][18];
            if (!objDataResult.Tables[0].Rows[0].IsNull(19)) dcmFactura_suma_total = (decimal)objDataResult.Tables[0].Rows[0][19];
            if (!objDataResult.Tables[0].Rows[0].IsNull(20)) dcmFactura_promedio = (decimal)objDataResult.Tables[0].Rows[0][20];
            if (!objDataResult.Tables[0].Rows[0].IsNull(21)) intNotaID = (int)objDataResult.Tables[0].Rows[0][21];
            if (!objDataResult.Tables[0].Rows[0].IsNull(22)) dtNota_fecha = (DateTime)objDataResult.Tables[0].Rows[0][22];
            if (!objDataResult.Tables[0].Rows[0].IsNull(23)) dcmNota_cantidad = (decimal)objDataResult.Tables[0].Rows[0][23];
            if (!objDataResult.Tables[0].Rows[0].IsNull(24)) dcmNota_costo = (decimal)objDataResult.Tables[0].Rows[0][24];
            if (!objDataResult.Tables[0].Rows[0].IsNull(25)) dcmNota_total = (decimal)objDataResult.Tables[0].Rows[0][25];
            if (!objDataResult.Tables[0].Rows[0].IsNull(26)) dcmNota_cantidad_total = (decimal)objDataResult.Tables[0].Rows[0][26];
            if (!objDataResult.Tables[0].Rows[0].IsNull(27)) dcmNota_suma_total = (decimal)objDataResult.Tables[0].Rows[0][27];
            if (!objDataResult.Tables[0].Rows[0].IsNull(28)) dcmNota_promedio = (decimal)objDataResult.Tables[0].Rows[0][28];
            if (!objDataResult.Tables[0].Rows[0].IsNull(29)) dcmVenta_cantidad_total = (decimal)objDataResult.Tables[0].Rows[0][29];
            if (!objDataResult.Tables[0].Rows[0].IsNull(30)) dcmVenta_suma_total = (decimal)objDataResult.Tables[0].Rows[0][30];
            if (!objDataResult.Tables[0].Rows[0].IsNull(31)) dcmVenta_promedio = (decimal)objDataResult.Tables[0].Rows[0][31];
            if (!objDataResult.Tables[0].Rows[0].IsNull(32)) intVenta_minID = (int)objDataResult.Tables[0].Rows[0][32];
            if (!objDataResult.Tables[0].Rows[0].IsNull(33)) isVenta_min_fact = Convert.ToBoolean(objDataResult.Tables[0].Rows[0][33]);
            if (!objDataResult.Tables[0].Rows[0].IsNull(34)) dcmVenta_costo_min = (decimal)objDataResult.Tables[0].Rows[0][34];
            if (!objDataResult.Tables[0].Rows[0].IsNull(35)) intVenta_maxID = (int)objDataResult.Tables[0].Rows[0][35];
            if (!objDataResult.Tables[0].Rows[0].IsNull(36)) isVenta_max_fact = Convert.ToBoolean(objDataResult.Tables[0].Rows[0][36]);
            if (!objDataResult.Tables[0].Rows[0].IsNull(37)) dcmVenta_costo_max = (decimal)objDataResult.Tables[0].Rows[0][37]; 
        }
    }

    public void Verificar_Compra()
    {
        if (dcmCompra_cantidad_total.HasValue)
        {
            dcmCompra_cantidad_total += dcmCompra_cantidad;
            dcmCompra_suma_total += dcmCompra_total;
        }
        else
        {
            dcmCompra_cantidad_total = dcmCompra_cantidad;
            dcmCompra_suma_total = dcmCompra_total;
        }

        if (dcmCompra_cantidad_total > 0)
            dcmCompra_promedio = dcmCompra_suma_total / dcmCompra_cantidad_total;
        else
            dcmCompra_cantidad_total = dcmCompra_suma_total;

        if (intCompra_minID.HasValue)
        {
            if (dcmCompra_costo_min >= dcmCompra_costo)
            {
                intCompra_minID = intCompraID;
                dcmCompra_costo_min = dcmCompra_costo;
            }
        }
        else
        {
            intCompra_minID = intCompraID;
            dcmCompra_costo_min = dcmCompra_costo;
        }

        if (intCompra_maxID.HasValue)
        {
            if (dcmCompra_costo_max <= dcmCompra_costo)
            {
                intCompra_maxID = intCompraID;
                dcmCompra_costo_max = dcmCompra_costo;
            }
        }
        else
        {
            intCompra_maxID = intCompraID;
            dcmCompra_costo_max = dcmCompra_costo;
        }
    }

    public void Verificar_Factura()
    {
        if (dcmFactura_cantidad_total.HasValue)
        {
            dcmFactura_cantidad_total += dcmFactura_cantidad;
            dcmFactura_suma_total += dcmFactura_total;
        }
        else
        {
            dcmFactura_cantidad_total = dcmFactura_cantidad;
            dcmFactura_suma_total = dcmFactura_total;
        }

        if (dcmFactura_cantidad_total > 0)
            dcmFactura_promedio = dcmFactura_suma_total / dcmFactura_cantidad_total;
        else
            dcmFactura_promedio = dcmFactura_suma_total;

        if (dcmVenta_cantidad_total.HasValue)
        {
            dcmVenta_cantidad_total += dcmFactura_cantidad;
            dcmVenta_suma_total += dcmFactura_total;
        }
        else
        {
            dcmVenta_cantidad_total = dcmFactura_cantidad;
            dcmVenta_suma_total = dcmFactura_total;
        }
        dcmVenta_promedio = dcmVenta_suma_total / dcmVenta_cantidad_total;

        if (intVenta_minID.HasValue)
        {
            if (dcmVenta_costo_min >= dcmFactura_costo)
            {
                intVenta_minID = intFacturaID;
                isVenta_min_fact = true;
                dcmVenta_costo_min = dcmFactura_costo;
            }
        }
        else
        {
            intVenta_minID = intFacturaID;
            isVenta_min_fact = true;
            dcmVenta_costo_min = dcmFactura_costo;
        }

        if (intVenta_maxID.HasValue)
        {
            if (dcmVenta_costo_max <= dcmFactura_costo)
            {
                intVenta_maxID = intFacturaID;
                isVenta_max_fact = true;
                dcmVenta_costo_max = dcmFactura_costo;
            }
        }
        else
        {
            intVenta_maxID = intFacturaID;
            isVenta_max_fact = true;
            dcmVenta_costo_max = dcmFactura_costo;
        }
    }

    public void Verificar_Nota()
    {
        if (dcmNota_cantidad_total.HasValue)
        {
            dcmNota_cantidad_total += dcmNota_cantidad;
            dcmNota_suma_total += dcmNota_total;
        }
        else
        {
            dcmNota_cantidad_total = dcmNota_cantidad;
            dcmNota_suma_total = dcmNota_total;
        }
        dcmNota_promedio = dcmNota_suma_total / dcmNota_cantidad_total;

        if (dcmVenta_cantidad_total.HasValue)
        {
            dcmVenta_cantidad_total += dcmNota_cantidad;
            dcmVenta_suma_total += dcmNota_total;
        }
        else
        {
            dcmVenta_cantidad_total = dcmNota_cantidad;
            dcmVenta_suma_total = dcmNota_total;
        }
        if (dcmVenta_cantidad_total > 0)
            dcmVenta_promedio = dcmVenta_suma_total / dcmVenta_cantidad_total;
        else
            dcmVenta_promedio = dcmVenta_suma_total;

        if (intVenta_minID.HasValue)
        {
            if (dcmVenta_costo_min >= dcmNota_costo)
            {
                intVenta_minID = intNotaID;
                isVenta_min_fact = true;
                dcmVenta_costo_min = dcmNota_costo;
            }
        }
        else
        {
            intVenta_minID = intNotaID;
            isVenta_min_fact = true;
            dcmVenta_costo_min = dcmNota_costo;
        }

        if (intVenta_maxID.HasValue)
        {
            if (dcmVenta_costo_max <= dcmNota_costo)
            {
                intVenta_maxID = intNotaID;
                isVenta_max_fact = true;
                dcmVenta_costo_max = dcmNota_costo;
            }
        }
        else
        {
            intVenta_maxID = intNotaID;
            isVenta_max_fact = true;
            dcmVenta_costo_max = dcmNota_costo;
        }
    }

    public void Recalcular_Compras()
    {
        intCompraID = null;
        dtCompra_fecha = null;
        dcmCompra_cantidad = null;
        dcmCompra_costo = null;
        dcmCompra_total = null;
        dcmCompra_cantidad_total = null;
        dcmCompra_suma_total = null;
        dcmCompra_promedio = null;
        intCompra_minID = null;
        dcmCompra_costo_min = null;
        intCompra_maxID = null;
        dcmCompra_costo_max = null;

        DataSet objDT = new DataSet();
        StringBuilder strQuery = new StringBuilder();

        strQuery.Append(//Table 0 - Ultima compra
                           " SELECT F.ID, F.fecha_creacion, R.cantidad, R.costo" +
                           " FROM" +
                           " (" +
                           "    SELECT SUM(cantidad) as cantidad, SUM(costo) as costo, compraID" +
                           "    FROM compra_productos" +
                           "    WHERE compraID =" +
                           "       (SELECT MAX(F.ID)" +
                           "        FROM compra F" +
                           "        INNER JOIN compra_productos P" +
                           "        ON P.compraID = F.ID" +
                           "        AND P.productoID = " + intProductoID +
                           "        AND F.estatus < 8)" +
                           "    AND productoID = " + intProductoID +
                           " ) AS R" +
                           " LEFT JOIN compra F" +
                           " ON R.compraID = F.ID;" +
            // Table 1 - Compra promedio
                           " SELECT sum(cantidad) as cantidad, sum(costo) as costo" +
                           " FROM compra F" +
                           " INNER JOIN compra_productos P" +
                           " ON P.compraID = F.ID" +
                           " AND P.productoID = " + intProductoID +
                           " AND F.estatus < 8;" +
            // Table 2 - Compra minima
                           " SELECT MAX(F.ID) as ID, P.costo_unitario" +
                           " FROM compra F" +
                           " INNER JOIN compra_productos P" +
                           " ON P.compraID = F.ID" +
                           " AND F.estatus < 8" +
                           " AND P.productoID = " + intProductoID +
                           " AND P.costo_unitario =" +
                           " (SELECT MIN(costo_unitario) as costo_unitario" +
                           " FROM compra F" +
                           " INNER JOIN compra_productos P" +
                           " ON P.compraID = F.ID" +
                           " AND P.productoID = " + intProductoID +
                           " AND F.estatus < 8);" +
            // Table 3 - Compra maxima
                           " SELECT MAX(F.ID) as ID, P.costo_unitario" +
                           " FROM compra F" +
                           " INNER JOIN compra_productos P" +
                           " ON P.compraID = F.ID" +
                           " AND F.estatus < 8" +
                           " AND P.productoID = " + intProductoID +
                           " AND P.costo_unitario =" +
                           " (SELECT MAX(costo_unitario) as costo_unitario" +
                           " FROM compra F" +
                           " INNER JOIN compra_productos P" +
                           " ON P.compraID = F.ID" +
                           " AND P.productoID = " + intProductoID +
                           " AND F.estatus < 8);"
                           );

        objDT = CComunDB.CCommun.Ejecutar_SP(strQuery.ToString());

        // Ultima Compra
        if (!objDT.Tables[0].Rows[0].IsNull(0))
        {
            intCompraID = (int)objDT.Tables[0].Rows[0][0];
            dtCompra_fecha = (DateTime)objDT.Tables[0].Rows[0][1];
            dcmCompra_cantidad = (decimal)objDT.Tables[0].Rows[0][2];
            dcmCompra_costo = (decimal)objDT.Tables[0].Rows[0][3] / (decimal)objDT.Tables[0].Rows[0][2];
            dcmCompra_total = (decimal)objDT.Tables[0].Rows[0][3];
        }

        // Compra promedio
        if (!objDT.Tables[1].Rows[0].IsNull(0))
        {
            dcmCompra_cantidad_total = (decimal)objDT.Tables[1].Rows[0][0];
            dcmCompra_promedio = (decimal)objDT.Tables[1].Rows[0][1] / (decimal)objDT.Tables[1].Rows[0][0];
            dcmCompra_suma_total = (decimal)objDT.Tables[1].Rows[0][1];
        }

        // Compra minima
        if (!objDT.Tables[2].Rows[0].IsNull(0))
        {
            intCompra_minID = (int)objDT.Tables[2].Rows[0][0];
            dcmCompra_costo_min = (decimal)objDT.Tables[2].Rows[0][1];
        }

        // Compra minima
        if (!objDT.Tables[3].Rows[0].IsNull(0))
        {
            intCompra_maxID = (int)objDT.Tables[3].Rows[0][0];
            dcmCompra_costo_max = (decimal)objDT.Tables[3].Rows[0][1];
        }
    }

    public void Recalcular_Facturas()
    {
        intFacturaID = null;
        dtFactura_fecha = null;
        dcmFactura_cantidad = null;
        dcmFactura_costo = null;
        dcmFactura_total = null;
        dcmFactura_cantidad_total = null;
        dcmFactura_suma_total = null;
        dcmFactura_promedio = null;

        this.dcmVenta_cantidad_total = null;
        this.dcmVenta_suma_total = null;
        this.dcmVenta_promedio = null;
        intVenta_minID = null;
        isVenta_min_fact = true;
        dcmVenta_costo_min = null;
        intVenta_maxID = null;
        isVenta_max_fact = true;
        dcmVenta_costo_max = null;

        DataSet objDT = new DataSet();
        StringBuilder strQuery = new StringBuilder();

        strQuery.Append(//Table 0 - Ultima facturas_liq
                           " SELECT F.ID, F.fecha, R.cantidad, R.costo" +
                           " FROM" +
                           " (" +
                           "    SELECT SUM(cantidad) as cantidad, SUM(costo) as costo, factura_ID" +
                           "    FROM facturas_liq_prod" +
                           "    WHERE factura_ID =" +
                           "       (SELECT MAX(F.ID)" +
                           "        FROM facturas_liq F" +
                           "        INNER JOIN facturas_liq_prod P" +
                           "        ON P.factura_ID = F.ID" +
                           "        AND P.producto_ID = " + intProductoID +
                           "        AND F.status < 8)" +
                           "    AND producto_ID = " + intProductoID +
                           " ) AS R" +
                           " LEFT JOIN facturas_liq F" +
                           " ON R.factura_ID = F.ID;" +
            // Table 1 - facturas_liq promedio
                           " SELECT sum(cantidad) as cantidad, sum(costo) as costo" +
                           " FROM facturas_liq F" +
                           " INNER JOIN facturas_liq_prod P" +
                           " ON P.factura_ID = F.ID" +
                           " AND P.producto_ID = " + intProductoID +
                           " AND F.status < 8;" +
            // Table 2 - notas promedio
                           " SELECT sum(cantidad) as cantidad, sum(costo) as costo" +
                           " FROM notas F" +
                           " INNER JOIN notas_prod P" +
                           " ON P.nota_ID = F.ID" +
                           " AND P.producto_ID = " + intProductoID +
                           " AND P.cantidad > 0" +
                           " AND F.status < 4;" +
            // Table 3 - facturas_liq minima
                           " SELECT MAX(F.ID) as ID, P.costo_unitario" +
                           " FROM facturas_liq F" +
                           " INNER JOIN facturas_liq_prod P" +
                           " ON P.factura_ID = F.ID" +
                           " AND F.status < 8" +
                           " AND P.producto_ID = " + intProductoID +
                           " AND P.costo_unitario =" +
                           " (SELECT MIN(costo_unitario) as costo_unitario" +
                           " FROM facturas_liq F" +
                           " INNER JOIN facturas_liq_prod P" +
                           " ON P.factura_ID = F.ID" +
                           " AND P.producto_ID = " + intProductoID +
                           " AND F.status < 8);" +
            // Table 4 - facturas_liq maxima
                           " SELECT MAX(F.ID) as ID, P.costo_unitario" +
                           " FROM facturas_liq F" +
                           " INNER JOIN facturas_liq_prod P" +
                           " ON P.factura_ID = F.ID" +
                           " AND F.status < 8" +
                           " AND P.producto_ID = " + intProductoID +
                           " AND P.costo_unitario =" +
                           " (SELECT MAX(costo_unitario) as costo_unitario" +
                           " FROM facturas_liq F" +
                           " INNER JOIN facturas_liq_prod P" +
                           " ON P.factura_ID = F.ID" +
                           " AND P.producto_ID = " + intProductoID +
                           " AND F.status < 8);" +
            // Table 5 - notas minima
                           " SELECT MAX(F.ID) as ID, P.costo_unitario" +
                           " FROM notas F" +
                           " INNER JOIN notas_prod P" +
                           " ON P.nota_ID = F.ID" +
                           " AND F.status < 4" +
                           " AND P.producto_ID = " + intProductoID +
                           " AND P.costo_unitario =" +
                           " (SELECT MIN(costo_unitario) as costo_unitario" +
                           " FROM notas F" +
                           " INNER JOIN notas_prod P" +
                           " ON P.nota_ID = F.ID" +
                           " AND P.producto_ID = " + intProductoID +
                           " AND F.status < 4);" +
            // Table 6 - notas maxima
                           " SELECT MAX(F.ID) as ID, P.costo_unitario" +
                           " FROM notas F" +
                           " INNER JOIN notas_prod P" +
                           " ON P.nota_ID = F.ID" +
                           " AND F.status < 4" +
                           " AND P.producto_ID = " + intProductoID +
                           " AND P.costo_unitario =" +
                           " (SELECT MAX(costo_unitario) as costo_unitario" +
                           " FROM notas F" +
                           " INNER JOIN notas_prod P" +
                           " ON P.nota_ID = F.ID" +
                           " AND P.producto_ID = " + intProductoID +
                           " AND F.status < 4);"
                           );

        objDT = CComunDB.CCommun.Ejecutar_SP(strQuery.ToString());

        // Ultima Factura
        if (!objDT.Tables[0].Rows[0].IsNull(0))
        {
            intFacturaID = (int)objDT.Tables[0].Rows[0][0];
            dtFactura_fecha = (DateTime)objDT.Tables[0].Rows[0][1];
            dcmFactura_cantidad = (decimal)objDT.Tables[0].Rows[0][2];
            dcmFactura_costo = (decimal)objDT.Tables[0].Rows[0][3] / (decimal)objDT.Tables[0].Rows[0][2];
            dcmFactura_total = (decimal)objDT.Tables[0].Rows[0][3];
        }

        decimal dcmVenta_cantidad_total = 0;
        decimal dcmVenta_suma_total = 0;
        // Factura promedio
        if (!objDT.Tables[1].Rows[0].IsNull(0))
        {
            dcmFactura_cantidad_total = (decimal)objDT.Tables[1].Rows[0][0];
            dcmFactura_promedio = (decimal)objDT.Tables[1].Rows[0][1] / (decimal)objDT.Tables[1].Rows[0][0];
            dcmFactura_suma_total = (decimal)objDT.Tables[1].Rows[0][1];
            dcmVenta_cantidad_total += dcmFactura_cantidad_total.Value;
            dcmVenta_suma_total += dcmFactura_suma_total.Value;
        }

        // Nota promedio
        if (!objDT.Tables[2].Rows[0].IsNull(0))
        {
            dcmNota_cantidad_total = (decimal)objDT.Tables[2].Rows[0][0];
            dcmNota_promedio = (decimal)objDT.Tables[2].Rows[0][1] / (decimal)objDT.Tables[2].Rows[0][0];
            dcmNota_suma_total = (decimal)objDT.Tables[2].Rows[0][1];
            dcmVenta_cantidad_total += dcmNota_cantidad_total.Value;
            dcmVenta_suma_total += dcmNota_suma_total.Value;
        }

        // Venta promedio
        if (dcmVenta_cantidad_total > 0)
        {
            this.dcmVenta_cantidad_total = dcmVenta_cantidad_total;
            this.dcmVenta_promedio = dcmVenta_suma_total / dcmVenta_cantidad_total;
            this.dcmVenta_suma_total = dcmVenta_suma_total;
        }

        // Venta minima
        if (!objDT.Tables[3].Rows[0].IsNull(0) ||
            !objDT.Tables[5].Rows[0].IsNull(0))
        {
            if (!objDT.Tables[3].Rows[0].IsNull(0) &&
                !objDT.Tables[5].Rows[0].IsNull(0))
            {
                if ((decimal)objDT.Tables[3].Rows[0][1] <= (decimal)objDT.Tables[5].Rows[0][1])
                {
                    intVenta_minID = (int)objDT.Tables[3].Rows[0][0];
                    isVenta_min_fact = true;
                    dcmVenta_costo_min = (decimal)objDT.Tables[3].Rows[0][1];
                }
                else
                {
                    intVenta_minID = (int)objDT.Tables[5].Rows[0][0];
                    isVenta_min_fact = false;
                    dcmVenta_costo_min = (decimal)objDT.Tables[5].Rows[0][1];
                }
            }
            else
                if (!objDT.Tables[3].Rows[0].IsNull(0))
                {
                    intVenta_minID = (int)objDT.Tables[3].Rows[0][0];
                    isVenta_min_fact = true;
                    dcmVenta_costo_min = (decimal)objDT.Tables[3].Rows[0][1];
                }
                else
                {
                    intVenta_minID = (int)objDT.Tables[5].Rows[0][0];
                    isVenta_min_fact = false;
                    dcmVenta_costo_min = (decimal)objDT.Tables[5].Rows[0][1];
                }
        }

        // Venta maxima
        if (!objDT.Tables[4].Rows[0].IsNull(0) ||
            !objDT.Tables[5].Rows[0].IsNull(0))
        {
            if (!objDT.Tables[4].Rows[0].IsNull(0) &&
                !objDT.Tables[5].Rows[0].IsNull(0))
            {
                if ((decimal)objDT.Tables[4].Rows[0][1] >= (decimal)objDT.Tables[5].Rows[0][1])
                {
                    intVenta_maxID = (int)objDT.Tables[4].Rows[0][0];
                    isVenta_max_fact = true;
                    dcmVenta_costo_max = (decimal)objDT.Tables[4].Rows[0][1];
                }
                else
                {
                    intVenta_maxID = (int)objDT.Tables[5].Rows[0][0];
                    isVenta_max_fact = false;
                    dcmVenta_costo_max = (decimal)objDT.Tables[5].Rows[0][1];
                }
            }
            else
                if (!objDT.Tables[4].Rows[0].IsNull(0))
                {
                    intVenta_maxID = (int)objDT.Tables[4].Rows[0][0];
                    isVenta_max_fact = true;
                    dcmVenta_costo_max = (decimal)objDT.Tables[4].Rows[0][1];
                }
                else
                {
                    intVenta_maxID = (int)objDT.Tables[5].Rows[0][0];
                    isVenta_max_fact = false;
                    dcmVenta_costo_max = (decimal)objDT.Tables[5].Rows[0][1];
                }
        }
    }

    public void Recalcular_Notas()
    {
        intNotaID = null;
        dtNota_fecha = null;
        dcmNota_cantidad = null;
        dcmNota_costo = null;
        dcmNota_total = null;
        dcmNota_cantidad_total = null;
        dcmNota_suma_total = null;
        dcmNota_promedio = null;

        this.dcmVenta_cantidad_total = null;
        this.dcmVenta_suma_total = null;
        this.dcmVenta_promedio = null;
        intVenta_minID = null;
        isVenta_min_fact = true;
        dcmVenta_costo_min = null;
        intVenta_maxID = null;
        isVenta_max_fact = true;
        dcmVenta_costo_max = null;

        DataSet objDT = new DataSet();
        StringBuilder strQuery = new StringBuilder();

        strQuery.Append(//Table 0 - Ultima notas
                           " SELECT F.ID, F.fecha, R.cantidad, R.costo" +
                           " FROM" +
                           " (" +
                           "    SELECT SUM(cantidad) as cantidad, SUM(costo) as costo, nota_ID" +
                           "    FROM notas_prod" +
                           "    WHERE nota_ID =" +
                           "       (SELECT MAX(F.ID)" +
                           "        FROM notas F" +
                           "        INNER JOIN notas_prod P" +
                           "        ON P.nota_ID = F.ID" +
                           "        AND P.producto_ID = " + intProductoID +
                           "        AND P.cantidad > 0" +
                           "        AND F.status < 4)" +
                           "    AND producto_ID = " + intProductoID +
                           " ) AS R" +
                           " LEFT JOIN notas F" +
                           " ON R.nota_ID = F.ID;" +
            // Table 1 - facturas_liq promedio
                           " SELECT sum(cantidad) as cantidad, sum(costo) as costo" +
                           " FROM facturas_liq F" +
                           " INNER JOIN facturas_liq_prod P" +
                           " ON P.factura_ID = F.ID" +
                           " AND P.producto_ID = " + intProductoID +
                           " AND F.status < 8;" +
            // Table 2 - notas promedio
                           " SELECT sum(cantidad) as cantidad, sum(costo) as costo" +
                           " FROM notas F" +
                           " INNER JOIN notas_prod P" +
                           " ON P.nota_ID = F.ID" +
                           " AND P.producto_ID = " + intProductoID +
                           " AND P.cantidad > 0" +
                           " AND F.status < 4;" +
            // Table 3 - facturas_liq minima
                           " SELECT MAX(F.ID) as ID, P.costo_unitario" +
                           " FROM facturas_liq F" +
                           " INNER JOIN facturas_liq_prod P" +
                           " ON P.factura_ID = F.ID" +
                           " AND F.status < 8" +
                           " AND P.producto_ID = " + intProductoID +
                           " AND P.costo_unitario =" +
                           " (SELECT MIN(costo_unitario) as costo_unitario" +
                           " FROM facturas_liq F" +
                           " INNER JOIN facturas_liq_prod P" +
                           " ON P.factura_ID = F.ID" +
                           " AND P.producto_ID = " + intProductoID +
                           " AND F.status < 8);" +
            // Table 4 - facturas_liq maxima
                           " SELECT MAX(F.ID) as ID, P.costo_unitario" +
                           " FROM facturas_liq F" +
                           " INNER JOIN facturas_liq_prod P" +
                           " ON P.factura_ID = F.ID" +
                           " AND F.status < 8" +
                           " AND P.producto_ID = " + intProductoID +
                           " AND P.costo_unitario =" +
                           " (SELECT MAX(costo_unitario) as costo_unitario" +
                           " FROM facturas_liq F" +
                           " INNER JOIN facturas_liq_prod P" +
                           " ON P.factura_ID = F.ID" +
                           " AND P.producto_ID = " + intProductoID +
                           " AND F.status < 8);" +
            // Table 5 - notas minima
                           " SELECT MAX(F.ID) as ID, P.costo_unitario" +
                           " FROM notas F" +
                           " INNER JOIN notas_prod P" +
                           " ON P.nota_ID = F.ID" +
                           " AND F.status < 4" +
                           " AND P.producto_ID = " + intProductoID +
                           " AND P.costo_unitario =" +
                           " (SELECT MIN(costo_unitario) as costo_unitario" +
                           " FROM notas F" +
                           " INNER JOIN notas_prod P" +
                           " ON P.nota_ID = F.ID" +
                           " AND P.producto_ID = " + intProductoID +
                           " AND F.status < 4);" +
            // Table 6 - notas maxima
                           " SELECT MAX(F.ID) as ID, P.costo_unitario" +
                           " FROM notas F" +
                           " INNER JOIN notas_prod P" +
                           " ON P.nota_ID = F.ID" +
                           " AND F.status < 4" +
                           " AND P.producto_ID = " + intProductoID +
                           " AND P.costo_unitario =" +
                           " (SELECT MAX(costo_unitario) as costo_unitario" +
                           " FROM notas F" +
                           " INNER JOIN notas_prod P" +
                           " ON P.nota_ID = F.ID" +
                           " AND P.producto_ID = " + intProductoID +
                           " AND F.status < 4);"
                           );

        objDT = CComunDB.CCommun.Ejecutar_SP(strQuery.ToString());

        // Ultima Nota
        if (!objDT.Tables[0].Rows[0].IsNull(0))
        {
            intNotaID = (int)objDT.Tables[0].Rows[0][0];
            dtNota_fecha = (DateTime)objDT.Tables[0].Rows[0][1];
            dcmNota_cantidad = (decimal)objDT.Tables[0].Rows[0][2];
            dcmNota_costo = (decimal)objDT.Tables[0].Rows[0][3] / (decimal)objDT.Tables[0].Rows[0][2];
            dcmNota_total = (decimal)objDT.Tables[0].Rows[0][3];
        }

        decimal dcmVenta_cantidad_total = 0;
        decimal dcmVenta_suma_total = 0;
        // Factura promedio
        if (!objDT.Tables[1].Rows[0].IsNull(0))
        {
            dcmFactura_cantidad_total = (decimal)objDT.Tables[1].Rows[0][0];
            dcmFactura_promedio = (decimal)objDT.Tables[1].Rows[0][1] / (decimal)objDT.Tables[1].Rows[0][0];
            dcmFactura_suma_total = (decimal)objDT.Tables[1].Rows[0][1];
            dcmVenta_cantidad_total += dcmFactura_cantidad_total.Value;
            dcmVenta_suma_total += dcmFactura_suma_total.Value;
        }

        // Nota promedio
        if (!objDT.Tables[2].Rows[0].IsNull(0))
        {
            dcmNota_cantidad_total = (decimal)objDT.Tables[2].Rows[0][0];
            dcmNota_promedio = (decimal)objDT.Tables[2].Rows[0][1] / (decimal)objDT.Tables[2].Rows[0][0];
            dcmNota_suma_total = (decimal)objDT.Tables[2].Rows[0][1];
            dcmVenta_cantidad_total += dcmNota_cantidad_total.Value;
            dcmVenta_suma_total += dcmNota_suma_total.Value;
        }

        // Venta promedio
        if (dcmVenta_cantidad_total > 0)
        {
            this.dcmVenta_cantidad_total = dcmVenta_cantidad_total;
            this.dcmVenta_promedio = dcmVenta_suma_total / dcmVenta_cantidad_total;
            this.dcmVenta_suma_total = dcmVenta_suma_total;
        }

        // Venta minima
        if (!objDT.Tables[3].Rows[0].IsNull(0) ||
            !objDT.Tables[5].Rows[0].IsNull(0))
        {
            if (!objDT.Tables[3].Rows[0].IsNull(0) &&
                !objDT.Tables[5].Rows[0].IsNull(0))
            {
                if ((decimal)objDT.Tables[3].Rows[0][1] <= (decimal)objDT.Tables[5].Rows[0][1])
                {
                    intVenta_minID = (int)objDT.Tables[3].Rows[0][0];
                    isVenta_min_fact = true;
                    dcmVenta_costo_min = (decimal)objDT.Tables[3].Rows[0][1];
                }
                else
                {
                    intVenta_minID = (int)objDT.Tables[5].Rows[0][0];
                    isVenta_min_fact = false;
                    dcmVenta_costo_min = (decimal)objDT.Tables[5].Rows[0][1];
                }
            }
            else
                if (!objDT.Tables[3].Rows[0].IsNull(0))
                {
                    intVenta_minID = (int)objDT.Tables[3].Rows[0][0];
                    isVenta_min_fact = true;
                    dcmVenta_costo_min = (decimal)objDT.Tables[3].Rows[0][1];
                }
                else
                {
                    intVenta_minID = (int)objDT.Tables[5].Rows[0][0];
                    isVenta_min_fact = false;
                    dcmVenta_costo_min = (decimal)objDT.Tables[5].Rows[0][1];
                }
        }

        // Venta maxima
        if (!objDT.Tables[4].Rows[0].IsNull(0) ||
            !objDT.Tables[5].Rows[0].IsNull(0))
        {
            if (!objDT.Tables[4].Rows[0].IsNull(0) &&
                !objDT.Tables[5].Rows[0].IsNull(0))
            {
                if ((decimal)objDT.Tables[4].Rows[0][1] >= (decimal)objDT.Tables[5].Rows[0][1])
                {
                    intVenta_maxID = (int)objDT.Tables[4].Rows[0][0];
                    isVenta_max_fact = true;
                    dcmVenta_costo_max = (decimal)objDT.Tables[4].Rows[0][1];
                }
                else
                {
                    intVenta_maxID = (int)objDT.Tables[5].Rows[0][0];
                    isVenta_max_fact = false;
                    dcmVenta_costo_max = (decimal)objDT.Tables[5].Rows[0][1];
                }
            }
            else
                if (!objDT.Tables[4].Rows[0].IsNull(0))
                {
                    intVenta_maxID = (int)objDT.Tables[4].Rows[0][0];
                    isVenta_max_fact = true;
                    dcmVenta_costo_max = (decimal)objDT.Tables[4].Rows[0][1];
                }
                else
                {
                    intVenta_maxID = (int)objDT.Tables[5].Rows[0][0];
                    isVenta_max_fact = false;
                    dcmVenta_costo_max = (decimal)objDT.Tables[5].Rows[0][1];
                }
        }
    }

    public void Guardar()
    {
        if (intProductoID != 0)
        {
            StringBuilder strQuery = new StringBuilder("DELETE FROM producto_datos" +
                                                      " WHERE productoID =" + intProductoID);

            CComunDB.CCommun.Ejecutar_SP(strQuery.ToString());

            strQuery.Length = 0;

            strQuery.Append("INSERT INTO producto_datos (" +
                           " productoID" +
                           ",existencia" +
                           ",compraID" +
                           ",compra_fecha" +
                           ",compra_cantidad" +
                           ",compra_costo" +
                           ",compra_total" +
                           ",compra_cantidad_total" +
                           ",compra_suma_total" +
                           ",compra_promedio" +
                           ",compra_minID" +
                           ",compra_costo_min" +
                           ",compra_maxID" +
                           ",compra_costo_max" +
                           ",facturaID" +
                           ",factura_fecha" +
                           ",factura_cantidad" +
                           ",factura_costo" +
                           ",factura_total" +
                           ",factura_cantidad_total" +
                           ",factura_suma_total" +
                           ",factura_promedio" +
                           ",notaID" +
                           ",nota_fecha" +
                           ",nota_cantidad" +
                           ",nota_costo" +
                           ",nota_total" +
                           ",nota_cantidad_total" +
                           ",nota_suma_total" +
                           ",nota_promedio" +
                           ",venta_cantidad_total" +
                           ",venta_suma_total" +
                           ",venta_promedio" +
                           ",venta_minID" +
                           ",venta_min_fact" +
                           ",venta_costo_min" +
                           ",venta_maxID" +
                           ",venta_max_fact" +
                           ",venta_costo_max" +
                           ") VALUES (" +
                           "" + intProductoID +
                           "," + dcmExistencia +
                           "," + (intCompraID.HasValue ? intCompraID.Value.ToString() : "null") +
                           "," + (dtCompra_fecha.HasValue ? "'" + dtCompra_fecha.Value.ToString("yyyy-MM-dd") + "'" : "null") +
                           "," + (dcmCompra_cantidad.HasValue ? dcmCompra_cantidad.Value.ToString() : "null") +
                           "," + (dcmCompra_costo.HasValue ? dcmCompra_costo.Value.ToString() : "null") +
                           "," + (dcmCompra_total.HasValue ? dcmCompra_total.Value.ToString() : "null") +
                           "," + (dcmCompra_cantidad_total.HasValue ? dcmCompra_cantidad_total.Value.ToString() : "null") +
                           "," + (dcmCompra_suma_total.HasValue ? dcmCompra_suma_total.Value.ToString() : "null") +
                           "," + (dcmCompra_promedio.HasValue ? dcmCompra_promedio.Value.ToString() : "null") +
                           "," + (intCompra_minID.HasValue ? intCompra_minID.Value.ToString() : "null") +
                           "," + (dcmCompra_costo_min.HasValue ? dcmCompra_costo_min.Value.ToString() : "null") +
                           "," + (intCompra_maxID.HasValue ? intCompra_maxID.Value.ToString() : "null") +
                           "," + (dcmCompra_costo_max.HasValue ? dcmCompra_costo_max.Value.ToString() : "null") +
                           "," + (intFacturaID.HasValue ? intFacturaID.Value.ToString() : "null") +
                           "," + (dtFactura_fecha.HasValue ? "'" + dtFactura_fecha.Value.ToString("yyyy-MM-dd") + "'" : "null") +
                           "," + (dcmFactura_cantidad.HasValue ? dcmFactura_cantidad.Value.ToString() : "null") +
                           "," + (dcmFactura_costo.HasValue ? dcmFactura_costo.Value.ToString() : "null") +
                           "," + (dcmFactura_total.HasValue ? dcmFactura_total.Value.ToString() : "null") +
                           "," + (dcmFactura_cantidad_total.HasValue ? dcmFactura_cantidad_total.Value.ToString() : "null") +
                           "," + (dcmFactura_suma_total.HasValue ? dcmFactura_suma_total.Value.ToString() : "null") +
                           "," + (dcmFactura_promedio.HasValue ? dcmFactura_promedio.Value.ToString() : "null") +
                           "," + (intNotaID.HasValue ? intNotaID.Value.ToString() : "null") +
                           "," + (dtNota_fecha.HasValue ? "'" + dtNota_fecha.Value.ToString("yyyy-MM-dd") + "'" : "null") +
                           "," + (dcmNota_cantidad.HasValue ? dcmNota_cantidad.Value.ToString() : "null") +
                           "," + (dcmNota_costo.HasValue ? dcmNota_costo.Value.ToString() : "null") +
                           "," + (dcmNota_total.HasValue ? dcmNota_total.Value.ToString() : "null") +
                           "," + (dcmNota_cantidad_total.HasValue ? dcmNota_cantidad_total.Value.ToString() : "null") +
                           "," + (dcmNota_suma_total.HasValue ? dcmNota_suma_total.Value.ToString() : "null") +
                           "," + (dcmNota_promedio.HasValue ? dcmNota_promedio.Value.ToString() : "null") +
                           "," + (dcmVenta_cantidad_total.HasValue ? dcmVenta_cantidad_total.Value.ToString() : "null") +
                           "," + (dcmVenta_suma_total.HasValue ? dcmVenta_suma_total.Value.ToString() : "null") +
                           "," + (dcmVenta_promedio.HasValue ? dcmVenta_promedio.Value.ToString() : "null") +
                           "," + (intVenta_minID.HasValue ? intVenta_minID.Value.ToString() : "null") +
                           "," + (isVenta_min_fact ? "1" : "0") +
                           "," + (dcmVenta_costo_min.HasValue ? dcmVenta_costo_min.Value.ToString() : "null") +
                           "," + (intVenta_maxID.HasValue ? intVenta_maxID.Value.ToString() : "null") +
                           "," + (isVenta_max_fact ? "1" : "0") +
                           "," + (dcmVenta_costo_max.HasValue ? dcmVenta_costo_max.Value.ToString() : "null") +
                           ")"
                           );
            CComunDB.CCommun.Ejecutar_SP(strQuery.ToString());
        }
    }

    public CProducto_Datos()
    {
        intProductoID = 0;
        dcmExistencia = 0;
        intCompraID = null;
        dtCompra_fecha = null;
        dcmCompra_cantidad = null;
        dcmCompra_costo = null;
        dcmCompra_total = null;
        dcmCompra_cantidad_total = null;
        dcmCompra_suma_total = null;
        dcmCompra_promedio = null;
        intCompra_minID = null;
        dcmCompra_costo_min = null;
        intCompra_maxID = null;
        dcmCompra_costo_max = null;

        intFacturaID = null;
        dtFactura_fecha = null;
        dcmFactura_cantidad = null;
        dcmFactura_costo = null;
        dcmFactura_total = null;
        dcmFactura_cantidad_total = null;
        dcmFactura_suma_total = null;
        dcmFactura_promedio = null;

        intNotaID = null;
        dtNota_fecha = null;
        dcmNota_cantidad = null;
        dcmNota_costo = null;
        dcmNota_total = null;
        dcmNota_cantidad_total = null;
        dcmNota_suma_total = null;
        dcmNota_promedio = null;

        dcmVenta_cantidad_total = null;
        dcmVenta_suma_total = null;
        dcmVenta_promedio = null;
        intVenta_minID = null;
        isVenta_min_fact = true;
        dcmVenta_costo_min = null;
        intVenta_maxID = null;
        isVenta_max_fact = true;
        dcmVenta_costo_max = null;
    }
}
