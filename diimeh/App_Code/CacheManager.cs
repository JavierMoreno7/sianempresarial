using System;
using System.Collections.Generic;
using System.Web;
using System.Xml;
using System.IO;
using System.Data;

/// <summary>
/// Summary description for CacheManager
/// </summary>
public class CacheManager
{
    private static Dictionary<string, string> _lstCache = new Dictionary<string, string>();

    public static bool ObtenerValor(string strLlave, out string strValor)
    {
        strValor = string.Empty;
        if (_lstCache.ContainsKey(strLlave.ToLower()))
        {
            strValor = _lstCache[strLlave.ToLower()];
            return true;
        }
        
        return false;
    }

    public static void ColocarValor(string strLlave, string strValor)
    {
        if (_lstCache.ContainsKey(strLlave.ToLower()))
            _lstCache[strLlave.ToLower()] = strValor;
        else
            _lstCache.Add(strLlave.ToLower(), strValor);
    }

    public static void Clear()
    {
        _lstCache = new Dictionary<string, string>();
    }

    public static Dictionary<string, string> ObtenerCache()
    {
        return _lstCache;
    }

	public CacheManager()
	{
	}
}
