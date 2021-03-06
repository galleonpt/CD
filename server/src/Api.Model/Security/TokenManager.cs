using System;
using System.Collections.Generic;
using System.Linq;

namespace Api.Model.Security
{
  public class TokenManager
  {
    /* INSTANCE CLASS */
    private string _username;

    private string _name;

    private string _token;

    private DateTime _expirateDate;


    /// <summary>
    /// Gestor de tokens
    /// </summary>
    /// <param name="name">Nome</param>
    /// <param name="username">Username</param>
    /// <param name="expirateDate">Data de expiração do token</param>
    /// <param name="token">Token</param>
    public TokenManager(string name, string username, DateTime expirateDate, string token)
    {
      _name = name;
      _username = username;
      _expirateDate = expirateDate;
      _token = token;
      if (list.ContainsKey(this._username))
        list[username] = this;
      else
        list.Add(this._username, this);
    }


    /* STATIC CLASS */
    private static Dictionary<string, TokenManager> list = new Dictionary<string, TokenManager>();

    /// <summary>
    /// Remover um token
    /// </summary>
    /// <param name="token">Token</param>
    /// <returns></returns>
    public static bool RemoveToken(string token)
    {
      var aux = list.FirstOrDefault(a => a.Value._token == token).Key;
      return list.Remove(aux);
    }

    /// <summary>
    /// Validar se um token é valido
    /// </summary>
    /// <param name="token">Token</param>
    /// <param name="info"></param>
    /// <returns>Retorna se foi possivel apagar o token</returns>
    public static bool ValidateToken(string token, out object info)
    {
      ValidateAll();
      var tokenKey = list.FirstOrDefault(a => a.Value._token == token);
      if (tokenKey.Key != null)
      {
        info = new
        {
          authenticate = true,
          name = tokenKey.Value._name,
          user = tokenKey.Value._username
        };
        return true;
      }

      else
      {
        info = new
        {
          authenticate = false
        };
        return false;
      }
    }

    /// <summary>
    /// Obter o user pelo token
    /// </summary>
    /// <param name="token">Token</param>
    /// <returns></returns>
    public static string GetUserByToken(string token)
    {
      var tokenKey = list.FirstOrDefault(a => a.Value._token == token).Key;
      return tokenKey != null ? tokenKey : null;
    }

    /// <summary>
    /// Validar todos os tokens
    /// </summary>
    public static void ValidateAll()
    {
      DateTime now = DateTime.Now;
      List<string> keys = new List<string>();
      foreach (KeyValuePair<string, TokenManager> aux in list)
        if (DateTime.Compare(aux.Value._expirateDate, now) > 1)
          keys.Add(aux.Key);

      foreach (string aux in keys)
        list.Remove(aux);
    }
  }
}
