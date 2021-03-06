using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Api.Model.IServices.Ftp;

namespace Api.Data.Ftp
{
  public class FtpManager : IFtpManagerRepository
  {
    private string basepath = AppContext.BaseDirectory + "ftp\\";

    public string Basepath { get => basepath; }


    /// <summary>
    /// Apagar um arquivo
    /// </summary>
    /// <param name="username">Nome do user que quer apagar o arquivo</param>
    /// <param name="filePath">Caminho/Diretório do arquivo</param>
    /// <returns>Retorna se o arquivo foi apagado</returns>
    public async Task<bool> DeleteFileAsync(string username, string filePath)
    {
      string fullPath = basepath + username + @"\" + filePath;

      if (File.Exists(fullPath))
      {
        try
        {
          File.Delete(fullPath);
          return true;
        }
        catch (System.Exception)
        {
          throw;
        }
      }
      return false;
    }

    /// <summary>
    /// Apagar uma pasta
    /// </summary>
    /// <param name="username">Nome do user que quer apagar a pasta</param>
    /// <param name="folderPath">Caminho/Diretório da pasta</param>
    /// <returns>Retorna se a pasta foi apagada</returns>
    public async Task<bool> DeleteFolderAsync(string username, string folderPath)
    {
      string fullPath = basepath + username + @"\" + folderPath;

      if (Directory.Exists(fullPath))
      {
        try
        {
          Directory.Delete(fullPath);
          return true;
        }
        catch (System.Exception)
        {
          throw;
        }
      }
      return false;
    }


    /// <summary>
    /// Criar uma pasta
    /// </summary>
    /// <param name="username">Nome do user que está a criar a pasta</param>
    /// <param name="path">Caminho/Diretório que queremos para a pasta</param>
    /// <returns>Retorna se a pasta foi criada</returns>
    public async Task<bool> NewFolderAsync(string username, string path)
    {
      string fullPath = basepath + username + @"\" + path;

      if (!Directory.Exists(fullPath))
      {
        try
        {
          Directory.CreateDirectory(fullPath);
          return true;
        }
        catch (System.Exception)
        {
          throw;
        }
      }
      return false;
    }

    /// <summary>
    /// Renomear um arquivo 
    /// </summary>
    /// <param name="username">Username criador do arquivo</param>
    /// <param name="folderPath">Caminho/Diretório do arquivo</param>
    /// <param name="oldName">Nome do arquivo</param>
    /// <param name="newName">Novo nome para o arquivo</param>
    /// <returns>Retorna se foi possivel renomear o arquivo</returns>
    public async Task<bool> RenameFileAsync(string username, string folderPath, string oldName, string newName)
    {
      string oldFullPath = basepath + username + @"\" + folderPath + @"/" + oldName;
      string newFullPath = basepath + username + @"\" + folderPath + @"/" + newName;

      if (!File.Exists(oldFullPath))
      {
        try
        {
          File.Move(oldFullPath, newFullPath);
          return true;
        }
        catch (System.Exception)
        {
          throw;
        }
      }
      return false;
    }

    /// <summary>
    /// Renomear uma pasta
    /// </summary>
    /// <param name="username">Username criador da pasta</param>
    /// <param name="folderPath">Caminho/Diretório da pasta</param>
    /// <param name="oldName">Nome da pasta</param>
    /// <param name="newName">Novo nome para a pasta</param>
    /// <returns>Retorna se foi possivel renomear a pasta</returns>
    public async Task<bool> RenameFolderAsync(string username, string folderPath, string oldName, string newName)
    {
      string oldFullPath, newFullPath;
      if (folderPath.Equals(@"\"))
      {
        oldFullPath = basepath + username + @"\" + oldName;
        newFullPath = basepath + username + @"\" + newName;
      }
      else
      {
        oldFullPath = basepath + username + @"\" + folderPath + @"\" + oldName;
        newFullPath = basepath + username + @"\" + folderPath + @"\" + newName;
      }


      if (Directory.Exists(oldFullPath))
      {
        try
        {
          Directory.Move(oldFullPath, newFullPath);
          return true;
        }
        catch (System.Exception)
        {
          throw;
        }
      }
      return false;
    }


    /// <summary>
    /// Upload de arquivos
    /// </summary>
    /// <param name="username">Username do criador do upload</param>
    /// <param name="folderPath">Caminho/Diretório do arquivo</param>
    /// <param name="name">Nome do arquivo</param>
    /// <param name="file">Arquivo</param>
    /// <returns>Retorna se foi possivel fazer o upload</returns>
    public async Task<bool> UploadFileAsync(string username, string folderPath, string name, MemoryStream file)
    {
      string fullPath = basepath + username + @"\" + folderPath + @"/" + name;

      if (!File.Exists(fullPath))
      {
        try
        {
          using (var stream = System.IO.File.Create(fullPath))
          {
            await file.CopyToAsync(stream);
          }
          return true;
        }
        catch (System.Exception)
        {
          throw;
        }
      }
      return false;
    }


    /// <summary>
    /// Download de um arquivo
    /// </summary>
    /// <param name="username">Username do criador do arquivo</param>
    /// <param name="folderPath">Caminho/Diretório do arquivo</param>
    /// <returns>Download do arquivo</returns>
    public async Task<MemoryStream> DownloadFileAsync(string username, string folderPath)
    {
      string fullPath = basepath + username + @"\" + folderPath;

      if (File.Exists(fullPath))
      {
        try
        {
          var memory = new MemoryStream();
          using (var stream = new FileStream(fullPath, FileMode.Open))
          {
            await stream.CopyToAsync(memory);
          }
          return memory;
        }
        catch (System.Exception)
        {
          throw;
        }
      }
      return null;
    }

    /// <summary>
    /// Obter um arquivo pelo seu caminho/diretorio
    /// </summary>
    /// <param name="username">Username do criador do arquivo</param>
    /// <param name="path">Caminho/Diretório do arquivo</param>
    /// <returns>Retorna uma lista com todos os arquivos no diretorio escolhido</returns>
    public async Task<object> GetByPath(string username, string path)
    {
      string fullPath = path == "\\" ? basepath + username + @"\" : basepath + username + path;

      if (Directory.Exists(fullPath))
      {
        try
        {
          var allFolders = Directory.GetDirectories(fullPath).Select(p => new
          {
            Path = p,
            Name = Path.GetFileName(p)
          }).ToArray();
          var allFiles = Directory.GetFiles(fullPath).Select(p => new
          {
            Path = p,
            Name = Path.GetFileName(p)
          }).ToArray();
          List<object> listFolders = new List<object>();
          List<object> listFiles = new List<object>();

          foreach (var aux in allFolders)
          {
            string tempName = aux.Name;
            string tempPath = path == @"\\" ? aux.Path.Replace(basepath + username + @"\", "") : aux.Path.Replace(basepath + username, "");
            //
            listFolders.Add(new
            {
              name = tempName,
              path = tempPath
            });
          }

          foreach (var aux in allFiles)
          {
            string tempName = aux.Name;
            string tempPath = path == @"\\" ? aux.Path.Replace(basepath + username + @"\", "") : aux.Path.Replace(basepath + username, "");

            listFiles.Add(new
            {
              name = tempName,
              path = tempPath
            });
          }



          return new
          {
            folders = listFolders,
            files = listFiles
          };

        }
        catch (System.Exception)
        {
          throw;
        }
      }
      return null;
    }
  }
}
