using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TypeSystem;
using System.Text.Json;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace DataServices
{
    public class Services
    {
        protected IHttpContextAccessor accessor;
        RestClient restClient;
        public Services(IHttpContextAccessor ctx)
        {
            this.accessor = ctx;
            restClient = new RestClient(ctx);
        }
        public delegate Task<SPSite> ResourceDelegate(string url);
        public delegate Task<List<SPFolder>> ArrayResourceDelegate(string url);
        public delegate Task<List<SPSite>> ArrayResourceDelegateSite(string url);

        public delegate Task<List<SPFile>> ArrayResourceDelegateFiles(string url);
        public async Task<SPSite> sp_getSite(string url)
        {            
            ResourceDelegate getSpSite = restClient.getResource<SPSite>;
            return await getSpSite(url.TrimEnd(new char[] { ' ', '/' }) + "/_api/web") as SPSite;
        }

        public async Task<List<SPFolder>> sp_getSiteFolders(string siteurl)
        {            
            ArrayResourceDelegate getFolders = restClient.getResourceArray<SPFolder>;
            return await getFolders(siteurl.TrimEnd(new char[] { ' ', '/' }) + "/_api/web/folders");
        }
        public async Task<List<SPSite>> sp_getSubsites(string siteurl)
        {            
            ArrayResourceDelegateSite getWebs = restClient.getResourceArray<SPSite>;
            return await getWebs(siteurl.TrimEnd(new char[] { ' ', '/' }) + "/_api/web/webs");
        }

        public async Task<List<SPFile>> sp_getFilesInFolder(string folderUrl)
        {            
            ArrayResourceDelegateFiles getFiles = restClient.getResourceArray<SPFile>;
            return await getFiles(folderUrl + "/files");
        }
        public async Task<List<SPFolder>> sp_getFoldersInFolder(string folderUrl)
        {            
            ArrayResourceDelegate getFolders = restClient.getResourceArray<SPFolder>;
            return await getFolders(folderUrl + "/folders");
        }

    }

    public class RestClient
    {
        protected IHttpContextAccessor context;
        private string bearerToken;
        private string spcookie
        {
            get
            {
                if (string.IsNullOrEmpty(bearerToken))
                    try
                    {
                        return bearerToken = context.HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                else
                    return bearerToken;
            }
        }
        public RestClient(IHttpContextAccessor ctx) => this.context = ctx;

        protected HttpClient client;

        public async Task<SPSite> getResource<T>(string url)
        {
            client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("accept", "application/json");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", spcookie);
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            dynamic jsonResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);

            if (typeof(T) == typeof(SPSite))
            {
                return new SPSite
                {
                    siteId = jsonResponse.Id,
                    siteUrl = jsonResponse.Url,
                    siteTitle = jsonResponse.Title,
                    metadata = responseContent,
                };
            }
            // else if (typeof(T) == typeof(SPFolder))
            // {
            //     var folders = jsonResponse.value;

            //     foreach (var f in folders)
            //     {

            //         return new SPFolder
            //         {
            //             folderId = f.UniqueId,
            //             folderTitle = f.Name,
            //             itemCount = f.ItemCount,
            //             metadata = content,
            //             restUrl = f["odata.id"]
            //         };
            //     }
            // }
            // else
            // {
            //     return new SPFiles
            //     {
            //         fileName = null,
            //         blob = null,
            //         metadata = string.Empty
            //     };
            // }
            return null;
        }

        public async Task<List<T>> getResourceArray<T>(string url)
        {
            List<T> typeObjects = default(List<T>);

            using (client = new HttpClient())
            {
                try
                {
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
                    request.Headers.Add("accept", "application/json");
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", spcookie);
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStringAsync();
                    dynamic jsonResponse = JsonConvert.DeserializeObject<dynamic>(content);

                    //load a list of folders
                    if (typeof(T) == typeof(SPFolder))
                    {
                        var folders = jsonResponse.value;
                        List<SPFolder> foldersList = new List<SPFolder>();
                        foreach (var f in folders)
                        {
                            foldersList.Add(new SPFolder
                            {
                                folderId = f.UniqueId,
                                folderTitle = f.Name,
                                itemCount = f.ItemCount,
                                metadata = content,
                                restUrl = f["odata.id"]
                            });
                        }
                        typeObjects = foldersList as List<T>;
                    }
                    //load list of sites
                    if (typeof(T) == typeof(SPSite))
                    {
                        var webs = jsonResponse.value;
                        List<SPSite> sitesList = new List<SPSite>();
                        foreach (var s in webs)
                        {
                            sitesList.Add(new SPSite
                            {
                                siteId = s.Id,
                                siteTitle = s.Title,
                                siteUrl = s.Url,
                                metadata = content
                            }
                            );
                        }
                        typeObjects = sitesList as List<T>;
                    }
                    //load list of files
                    else if (typeof(T) == typeof(SPFile))
                    {
                        var files = jsonResponse.value;
                        List<SPFile> filesList = new List<SPFile>();
                        foreach (var file in files)
                        {
                            filesList.Add(new SPFile
                            {
                                fileId = file.UniqueId,
                                fileTitle = file.Name,
                                metadata = content,
                            });
                        }
                        typeObjects = filesList as List<T>;
                    }
                }
                catch (Exception)
                {

                }
            }
            return typeObjects;
        }
    }
}